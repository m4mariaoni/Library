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
    public class AdminService : IAdminService
    {
        private readonly IMapper _mapper;
        public IAppRepository _appRepository;
        public AdminService(IAppRepository appRepository, IMapper mapper)
        {
            _mapper = mapper;
            _appRepository = appRepository;
        }

        public async Task<Book> AddBook(BookModel model, string url)
        {
            Book book = _mapper.Map<Book>(model);
            await _appRepository.Books.Add(book);
            _appRepository.Save();
            return book;
        }

        public Task<IEnumerable<Book>> GetAllBooks(string url)
        {
            return _appRepository.Books.GetAll();
        }
        public async Task<IEnumerable<StudentModel>> StudentListBooks()
        {
            var model = new List<StudentModel>();
            var books =  await _appRepository.BorrowedBooks.GetAll();
            var status = books.GroupBy(x => x.userId)  //UserName
                            .Select(y => new StudentModel
                            {
                                 TotalLoan = y.Select(v => v.DateReturned == null).Count(),
                                 TotalOverDue = y.Sum(v=>v.OverDue.GetValueOrDefault()),
                                userId = y.Select(v=>v.userId).FirstOrDefault(), 
                                
                            }).ToList();
            foreach (var item in status)
            {
                var addStudent = new StudentModel();
                addStudent.StudentId = _appRepository.Users.Search(x => x.Id == item.userId).Select(y => y.Username).FirstOrDefault();
                addStudent.TotalLoan = item.TotalLoan;
                addStudent.TotalOverDue = item.TotalOverDue;
                addStudent.userId = item.userId;
                model.Add(addStudent);
            }
            return model;
        }

        public async Task<IEnumerable<StudentLoanModel>> StudentLoanDetails()
        {
            var model = new List<StudentLoanModel>();
            var books = await _appRepository.BorrowedBooks.GetAll();
            var status = books.Where(x=>x.DateReturned == null).ToList();
            foreach (var item in status)
            {
               var getBook = _appRepository.Books.Search(x => x.Id == item.bookId).FirstOrDefault();
               var loan = new StudentLoanModel();
               loan.Title = getBook.Title;
               loan.ISBN = getBook.ISBN;
               loan.Student = _appRepository.Users.Search(x=>x .Id == item.userId).Select(y=>y.Username).FirstOrDefault();
               loan.Borrowed = item.DateBorrowed.ToString("dd-MMM-yyyy");
               model.Add(loan);
            }
            return model;
        }

        public async Task<IEnumerable<StudentLoanModel>> StudentOverDueDetails()
        {
            var model = new List<StudentLoanModel>();
            var books = await _appRepository.BorrowedBooks.GetAll();
            var status = books.Where(x => x.OverDue == null).ToList();     //(x => x.OverDue > 0 ).ToList();
            foreach (var item in status)
            {
                var getBook = _appRepository.Books.Search(x => x.Id == item.bookId).FirstOrDefault();
                var loan = new StudentLoanModel();
                loan.Title = getBook.Title;
                loan.ISBN = getBook.ISBN;
                loan.Student = _appRepository.Users.Search(x => x.Id == item.userId).Select(y => y.Username).FirstOrDefault();
                loan.Borrowed = item.DateBorrowed.ToString("dd-MMM-yyyy");
                model.Add(loan);
            }
            return model;
        }

    }
}
