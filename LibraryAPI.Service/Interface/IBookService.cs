using LibraryAPI.Data.Entity;
using LibraryAPI.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryAPI.Service.Interface
{
    public interface IBookService
    {
        
        Task<BorrowedBookResponse> AddBorrowedBooks(string isbn, long userId);
        Task<BorrowedBookResponse> ReturnBorrowedBooks(string isbn, long userId);
        Task<IEnumerable<BorrowedBookViewModel>> StudentBorrowedBooks(long userId);

        //Task<AccountViewModel> GetAccountByStudentId(string studentId, string url);

        //Task<Account> GetAccountById(long id);

        //Task<bool> UpdateAccount(Account account);
    }
}
