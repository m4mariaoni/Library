using AutoMapper;
using LibraryAPI.Data.Entity;
using LibraryAPI.Data.Models;
using LibraryAPI.Infrastructure.Interface;
using LibraryAPI.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryAPI.Service.Service
{
    public class BookService : IBookService
    {
        private readonly IMapper _mapper;
        public IAppRepository _appRepository;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public BookService(IAppRepository appRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
        {
            _mapper = mapper;
            _configuration = configuration;
            _appRepository = appRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public string LibraryAPIUrl
        {
            get { return _configuration.GetSection("LibraryAPIUrl").Value; }
        }

        public async Task<BorrowedBookResponse> AddBorrowedBooks(string isbn, long userId)
        {
            var _books = new List<BorrowedBookViewModel>();
            var resp = new BorrowedBookResponse();
            var books = _appRepository.Books.Search(x => x.ISBN == isbn).FirstOrDefault();
            if(books.Copies == 0)
            {
                resp = new BorrowedBookResponse()
                {
                    Message = $"Sorry, There are no copies available.",
                    Borrowed = _books
                };
                return resp;
            
            }

            var user =_appRepository.Users.Search(x => x.Id == userId).Select(y => y.Username).FirstOrDefault();
            var getBorrowedBook = _appRepository.BorrowedBooks.Search(x => x.ISBN == isbn && x.DateReturned == null
            && x.UserName == user).FirstOrDefault();
            if (getBorrowedBook != null)
            {
                resp = new BorrowedBookResponse()
                {
                    Message = $"You have already borrowed this book",
                    Borrowed = _books
                };
                return resp;
            }

            var saveBook = new BorrowedBook();
            saveBook.ISBN = isbn;
            saveBook.UserName = user;
            saveBook.DateBorrowed = DateTime.Now;
            saveBook.ExpectedReturnDate = DateTime.Now.AddDays(1);
            _appRepository.BorrowedBooks.Add(saveBook);
            _appRepository.Save();

            var allbooks = _appRepository.BorrowedBooks.Search(x => x.UserName == user).ToList();
            _books = _mapper.Map<List<BorrowedBookViewModel>>(allbooks);

            
            int noOfCopies  = books.Copies - 1;
            books.Copies = noOfCopies;
            _appRepository.Books.Update(books);
            _appRepository.Save();
            var ExpectReturned = saveBook.ExpectedReturnDate.Value.ToString("dd-MMM-yyyy");
            resp = new BorrowedBookResponse()
            {
                Message = $"You have borrowed '{books.Title}' until {ExpectReturned}",
                Borrowed = _books
            };

            return resp;
        }
        public async Task<BorrowedBookResponse> ReturnBorrowedBooks(string isbn, long userId)
        {
            var _books = new List<BorrowedBookViewModel>();
            var user = _appRepository.Users.Search(x => x.Id == userId).Select(y => y.Username).FirstOrDefault();
            var book = _appRepository.BorrowedBooks.Search(x => x.ISBN == isbn && x.UserName == user && x.DateReturned==null).FirstOrDefault();

            //var days = (DateTime.Now - book.DateBorrowed).TotalDays;
            //var getday = Math.Truncate(days); 
            var getday = new double();
            if (DateTime.Now > book.ExpectedReturnDate)
            {
                var days2 = (DateTime.Now - book.ExpectedReturnDate.Value).TotalDays;
                getday = Math.Truncate(days2);
            }

            book.DateReturned = DateTime.Now;
            book.OverDue = Convert.ToInt16(getday);

         
            _appRepository.BorrowedBooks.Update(book);
            _appRepository.Save();

            var allbooks = _appRepository.BorrowedBooks.Search(x => x.UserName == user).ToList();
            _books = _mapper.Map<List<BorrowedBookViewModel>>(allbooks);

            var books = _appRepository.Books.Search(x => x.ISBN == isbn).FirstOrDefault();

            int noOfCopies = books.Copies + 1;
            books.Copies = noOfCopies;
           
            _appRepository.Books.Update(books);
            _appRepository.Save();
            var viewModel = new InvoiceViewModel();
            var resp = new BorrowedBookResponse();
            if (book.OverDue > 0)
            {
                InvoiceModel invoiceModel = new InvoiceModel();
                invoiceModel.DueDate = book.ExpectedReturnDate.Value;
                invoiceModel.StudentId = user;
                invoiceModel.Amount = 5 * getday;
                invoiceModel.Type = 0;
                viewModel = await CreateInvoice(invoiceModel);
                resp = new BorrowedBookResponse()
                {
                    Message = $"Thank you for your return. You have been fined ${invoiceModel.Amount}. Please log into the Payment portal and pay the invoice reference {viewModel.Reference}",
                    Borrowed = _books
                };
            }
            else
            {
                resp = new BorrowedBookResponse()
                {
                    Message = $"Thanks for your return",
                    Borrowed = _books
                };
            }

           
            return resp;
        }

        public async Task<IEnumerable<BorrowedBook>> StudentBorrowedBooks(long userId)
        {
            var viewModel = new List<BorrowedBook>();
            var user = _appRepository.Users.Search(x => x.Id == userId).Select(y => y.Username).FirstOrDefault();
            var book =  _appRepository.BorrowedBooks.Search(x =>  x.UserName == user).ToList();
            foreach (var item in book)
            {
                if (item.DateReturned == null)
                {
                    var getday = new double();
                    if (DateTime.Now > item.ExpectedReturnDate)
                    {
                        var days2 = (DateTime.Now - item.ExpectedReturnDate.Value).TotalDays;
                        getday = Math.Truncate(days2);
                    }

                    item.OverDue = Convert.ToInt16(getday);

                    viewModel.Add(item);
                }
                else
                {
                    viewModel.Add(item);
                }

            }
            return viewModel;
        }



        private async Task<InvoiceViewModel> CreateInvoice(InvoiceModel model)
        {

            var token2 = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            var answer = "";
            var viewModel = new InvoiceViewModel();
            using (var client = new HttpClient())
            {
                var apiUrl = LibraryAPIUrl + "api/Invoice";
                apiUrl = "http://localhost:5179/api/Invoice";
                var json = JsonConvert.SerializeObject(model);
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Add("Token", token2);
               
                var reply = client.PostAsync(apiUrl, content).Result;

                var result = reply.Content.ReadAsStringAsync().Result;
                if (reply.IsSuccessStatusCode)
                {
                   viewModel = JsonConvert.DeserializeObject<InvoiceViewModel>(result);

                }
                if (!reply.IsSuccessStatusCode)
                {
                    throw new Exception(reply.ReasonPhrase + result);
                }

                return viewModel;
            }


            //using (var client = new HttpClient())
            //{
            //    client.BaseAddress = new Uri(LibraryAPIUrl);
            //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token2);

            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //    HttpResponseMessage Resp = await client.PostAsJsonAsync($"api/Student/BorrowBook/{ISBN}", ISBN);
            //    if (Resp.IsSuccessStatusCode)
            //    {
            //        answer = Resp.Content.ReadAsStringAsync().Result;
            //        //await response.Content.ReadAsAsync<BookModel>();
            //        var books = JsonConvert.DeserializeObject<BorrowedBookResponse>(answer);
            //        copiesMsg = books.Message;
            //        ViewData["message"] = copiesMsg;

            //        if (books.Borrowed == null)
            //        {
            //            return RedirectToAction("GetAllBooks", "Book", copiesMsg);
            //        }

            //        foreach (var item in books.Borrowed)
            //        {
            //            var _borrow = new BorrowedBookViewModel();
            //            _borrow.ISBN = item.ISBN;
            //            _borrow.DateBorrowed = item.DateBorrowed.ToString("dd-MMM-yyyy");
            //            if (item.DateReturned != null)
            //            {
            //                _borrow.DateReturned = item.DateReturned.Value.ToString("dd-MMM-yyyy");
            //            }
            //            else
            //            {
            //                _borrow.DateReturned = null;
            //            }
            //            _borrow.OverDue = item.OverDue;
            //            viewModel.Add(_borrow);
            //        }

            //    }

            //    return View("ListBorrowedBooks", viewModel);
            //}
        }



    }
}
