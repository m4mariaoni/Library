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
            var status = books.GroupBy(x => x.UserName)
                            .Select(y => new StudentModel
                            {
                                 TotalLoan = y.Select(v => v.DateReturned == null).Count(),
                                 TotalOverDue = y.Sum(v=>v.OverDue.GetValueOrDefault()),
                                 StudentId = y.Select(v=>v.UserName).FirstOrDefault(),
                            }).ToList();

            return status;
        }

        public async Task<IEnumerable<StudentLoanModel>> StudentLoanDetails()
        {
            var model = new List<StudentLoanModel>();
            var books = await _appRepository.BorrowedBooks.GetAll();
            var status = books.Where(x=>x.DateReturned == null).ToList();
            foreach (var item in status)
            {
                var loan = new StudentLoanModel();
               var title =  _appRepository.Books.Search(x => x.ISBN == item.ISBN).Select(y=>y.Title).FirstOrDefault();
               loan.ISBN = item.ISBN;
               loan.Student = item.UserName;
               loan.Borrowed = item.DateBorrowed.ToString("dd-MMM-yyyy");
               loan.Title = title;
               model.Add(loan);
            }
            return model;
        }

        public async Task<IEnumerable<StudentLoanModel>> StudentOverDueDetails()
        {
            var model = new List<StudentLoanModel>();
            var books = await _appRepository.BorrowedBooks.GetAll();
            var status = books.Where(x => x.OverDue > 0 ).ToList();
            foreach (var item in status)
            {
                var loan = new StudentLoanModel();
                var title = _appRepository.Books.Search(x => x.ISBN == item.ISBN).Select(y => y.Title).FirstOrDefault();
                loan.ISBN = item.ISBN;
                loan.Student = item.UserName;
                loan.Borrowed = item.DateBorrowed.ToString("dd-MMM-yyyy");
                loan.Title = title;
                model.Add(loan);
            }
            return model;
        }

    }
}
