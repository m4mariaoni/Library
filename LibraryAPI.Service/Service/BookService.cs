using AutoMapper;
using LibraryAPI.Data.Entity;
using LibraryAPI.Data.Models;
using LibraryAPI.Infrastructure.Interface;
using LibraryAPI.Service.Interface;
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
        public BookService(IAppRepository appRepository, IMapper mapper)
        {
            _mapper = mapper;
            _appRepository = appRepository;
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
                    Borrowed = null
                };
                return resp;
            }
            var user =_appRepository.Users.Search(x => x.Id == userId).Select(y => y.Username).FirstOrDefault();

            var saveBook = new BorrowedBook();
            saveBook.ISBN = isbn;
            saveBook.UserName = user;
            saveBook.DateBorrowed = DateTime.Now;
            _appRepository.BorrowedBooks.Add(saveBook);
            _appRepository.Save();

            var allbooks = _appRepository.BorrowedBooks.Search(x => x.UserName == user).ToList();
            _books = _mapper.Map<List<BorrowedBookViewModel>>(allbooks);

            
            int noOfCopies  = books.Copies - 1;
            books.Copies = noOfCopies;
            _appRepository.Books.Update(books);
            _appRepository.Save();
            var ExpectReturned = saveBook.DateBorrowed.AddDays(5).ToString("dd-MMM-yyyy");
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
            var book = _appRepository.BorrowedBooks.Search(x => x.ISBN == isbn && x.UserName == user).FirstOrDefault();

            var days = (DateTime.Now - book.DateBorrowed).TotalDays;
            var newday = Math.Truncate(days); //Math.Round(days, 1);

            book.DateReturned = DateTime.Now;
            book.OverDue = Convert.ToInt16(newday);
            _appRepository.BorrowedBooks.Update(book);
            _appRepository.Save();

            var allbooks = _appRepository.BorrowedBooks.Search(x => x.UserName == user).ToList();
            _books = _mapper.Map<List<BorrowedBookViewModel>>(allbooks);

            var books = _appRepository.Books.Search(x => x.ISBN == isbn).FirstOrDefault();

            int noOfCopies = books.Copies + 1;
            books.Copies = noOfCopies;
           
            _appRepository.Books.Update(books);
            _appRepository.Save();
            
            var resp = new BorrowedBookResponse()
            {
                Message = $"Thanks for your return",
                Borrowed = _books
            }; 
            return resp;
        }

        public async Task<IEnumerable<BorrowedBook>> StudentBorrowedBooks(long userId)
        {
            var user = _appRepository.Users.Search(x => x.Id == userId).Select(y => y.Username).FirstOrDefault();
            var book =  _appRepository.BorrowedBooks.Search(x =>  x.UserName == user).ToList();           
            return book;
        }
    }
}
