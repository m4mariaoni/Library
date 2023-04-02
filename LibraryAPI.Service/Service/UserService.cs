using AutoMapper;
using LibraryAPI.Data.Entity;
using LibraryAPI.Data.Models;
using LibraryAPI.Infrastructure.Interface;
using LibraryAPI.Service.Interface;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryAPI.Service.Service
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        public IAppRepository _appRepository;
        public UserService(IAppRepository appRepository,IMapper mapper)
        {
            _mapper = mapper;
            _appRepository = appRepository;
        }
        public async Task<UserModel> CreateAccount(UserModel user, string url)
        {
            try
            {
                if(user.Role == 0)
                {
                    user.Password = "Password";
                }
                var hashed = ServiceExtension.SecretHasher.Hash(user.Password);
                User _user = _mapper.Map<User>(user);
                _user.isAuthenticated = true;
                _user.Password = hashed;
                await _appRepository.Users.Add(_user);
                _appRepository.Save();
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<UserModel>> GetAllUsers(string url)
        {
            var _users = await _appRepository.Users.GetAll();
            var users =_mapper.Map<List<UserModel>>(_users);
            return users;
        }
        public User GetLogin(Login login)
        {       
            var user = _appRepository.Users.Search(x => x.Username.Trim().ToLower() == login.UserName.Trim().ToLower()).FirstOrDefault();            
            if(user != null)
            {
                var unhashed = ServiceExtension.SecretHasher.Verify(login.Password, user.Password);
                if (unhashed)
                    return user;                                         
            }          
            return null;         
        }
    }
}
