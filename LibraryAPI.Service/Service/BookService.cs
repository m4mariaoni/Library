using AutoMapper;
using LibraryAPI.Data.Entity;
using LibraryAPI.Data.Models;
using LibraryAPI.Infrastructure;
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

        public string InvoiceAPIUrl
        {
            get { return _configuration.GetSection("InvoiceAPIUrl").Value; }
        }

        public async Task<BorrowedBookResponse> AddBorrowedBooks(string isbn, long userId)
        {
            var _books = new List<BorrowedBookViewModel>();
            var resp = new BorrowedBookResponse();
            var books = _appRepository.Books.Search(x => x.ISBN == isbn).FirstOrDefault();
            if(books != null && books.Copies  <= 0)
            {
                resp = new BorrowedBookResponse()
                {
                    Message = $"Sorry, There are no copies available.",
                    Borrowed = _books
                };
                return resp;
            
            }

            var user =_appRepository.Users.Search(x => x.Id == userId).Select(y => y.Username).FirstOrDefault();

            var getBorrowedBook = _appRepository.BorrowedBooks.Search(x => x.Book.ISBN == isbn && x.DateReturned == null
           && x.userId == userId).FirstOrDefault();
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

            saveBook.bookId = books.Id;
            saveBook.userId = userId;
            saveBook.DateBorrowed = DateTime.Now;
            saveBook.ExpectedReturnDate = DateTime.Now.AddDays(1);

            _appRepository.BorrowedBooks.Add(saveBook);
            _appRepository.Save();
       
            //Remove from available copies
            int noOfCopies = books.Copies - 1;
            books.Copies = noOfCopies;
           _appRepository.Books.Update(books);
           _appRepository.Save();

            //Get all the books user have ever borrowed.
            _books = getUserBorrowedbooks(userId, isbn);

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
            var resp = new BorrowedBookResponse();

            var user = _appRepository.Users.Search(x => x.Id == userId).FirstOrDefault();

            var getBookDetails = _appRepository.Books.Search(x => x.ISBN == isbn).FirstOrDefault();
        

            var book = _appRepository.BorrowedBooks.Search(x => x.bookId == getBookDetails.Id && x.userId == userId && x.DateReturned == null).FirstOrDefault();
            if(book == null)
            {
                resp = new BorrowedBookResponse()
                {
                    Message = $"You did not borrow any book with isbn {isbn}",
                    Borrowed = _books
                };

                return resp;
            }

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

           
            int noOfCopies = getBookDetails.Copies + 1;
            getBookDetails.Copies = noOfCopies;
           
            _appRepository.Books.Update(getBookDetails);
            _appRepository.Save();

            //Get all the books user have ever borrowed.
            _books = getUserBorrowedbooks(userId, isbn);
            var viewModel = new InvoiceViewModel();           
            resp = new BorrowedBookResponse();
            if (book.OverDue > 0)
            {
                InvoiceModel invoiceModel = new InvoiceModel();
                invoiceModel.DueDate = book.ExpectedReturnDate.Value;
                invoiceModel.StudentId = user.Username;
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

        public async Task<IEnumerable<BorrowedBookViewModel>> StudentBorrowedBooks(long userId)
        {
            var viewModel = new List<BorrowedBookViewModel>();
            var user =  _appRepository.Users.Search(x => x.Id == userId).Select(y => y.Username).FirstOrDefault();
            //var book =  _appRepository.BorrowedBooks.Search(x =>  x.UserName == user).ToList();
            var book =   _appRepository.BorrowedBooks.Search(x =>  x.userId == userId).ToList();
            foreach (var item in book)
            {
                var bookModel = new BorrowedBookViewModel();
                bookModel.Id = item.Id;
                bookModel.ISBN = _appRepository.Books.Search(x => x.Id == item.bookId).Select(y => y.ISBN).LastOrDefault();
                bookModel.DateBorrowed = item.DateBorrowed;
                bookModel.DateReturned = item.DateReturned;
                bookModel.OverDue = item.OverDue;
                bookModel.UserName = user;
                if (item.DateReturned == null)
                {
                    var getday = new double();
                    if (DateTime.Now > item.ExpectedReturnDate)
                    {
                        var days2 = (DateTime.Now - item.ExpectedReturnDate.Value).TotalDays;
                        getday = Math.Truncate(days2);
                    }

                    bookModel.OverDue = Convert.ToInt16(getday);

                    viewModel.Add(bookModel);
                }
                else
                {
                    bookModel.OverDue = 0;
                    viewModel.Add(bookModel);
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
                var apiUrl = InvoiceAPIUrl + "api/Invoice";

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

        }

        private List<BorrowedBookViewModel> getUserBorrowedbooks(long userId, string isbn)
        {
            //Get all the books user have ever borrowed.
            var _books = new List<BorrowedBookViewModel>();
            var allbooks = _appRepository.BorrowedBooks.Search(x => x.userId == userId).ToList();
            foreach (var item in allbooks)
            {
                var bookItem = new BorrowedBookViewModel();
                bookItem.Id = item.Id;
                bookItem.UserName = _appRepository.Users.Search(s => s.Id == userId).Select(y => y.Username).FirstOrDefault();
                bookItem.ISBN = _appRepository.Books.Search(x=>x.Id == item.bookId).Select(y=>y.ISBN).FirstOrDefault();
                bookItem.DateBorrowed = item.DateBorrowed;
                bookItem.DateReturned = item.DateReturned;
                bookItem.ExpectedReturnDate = item.ExpectedReturnDate.Value;
                bookItem.OverDue = getOverDue(item.ExpectedReturnDate.Value);
                _books.Add(bookItem);
            }
            return _books;
        }

        private int getOverDue(DateTime expectedReturnDate)
        {
            var getday = new double();
            if (DateTime.Now > expectedReturnDate)
            {
                var days2 = (DateTime.Now - expectedReturnDate).TotalDays;
                getday = Math.Truncate(days2);
            }
            else
            {
                getday = 0;
            }

            return Convert.ToInt16(getday);
        }
    }
}
