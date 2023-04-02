using LibraryAPI.Data.Entity;
using LibraryAPI.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryAPI.Service.Interface
{
    public interface IUserService
    {
        Task<UserModel> CreateAccount(UserModel user, string url);
        Task<IEnumerable<UserModel>> GetAllUsers(string url);
        User GetLogin(Login login);

        //Task<UserModel> GetAccountByStudentId(string studentId, string url);

        //Task<UserModel> GetAccountById(long id);

        //Task<bool> UpdateAccount(UserModel account);
    }
}
