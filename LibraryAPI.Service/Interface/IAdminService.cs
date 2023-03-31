using LibraryAPI.Data.Entity;
using LibraryAPI.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryAPI.Service.Interface
{
    public interface IAdminService
    {
        Task<Book> AddBook(BookModel model, string url);
        Task<IEnumerable<Book>> GetAllBooks(string url);
        Task<IEnumerable<StudentModel>> StudentListBooks();
        Task<IEnumerable<StudentLoanModel>> StudentLoanDetails();
        Task<IEnumerable<StudentLoanModel>> StudentOverDueDetails();
    }
}
