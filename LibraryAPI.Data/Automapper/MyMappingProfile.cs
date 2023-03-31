using AutoMapper;
using LibraryAPI.Data.Entity;
using LibraryAPI.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryAPI.Data.Automapper
{
    public class MyMappingProfile : Profile
    {
        public MyMappingProfile()
        {
            CreateMap<User, UserModel>().ReverseMap();
            CreateMap<Book, BookModel>().ReverseMap();
            CreateMap<BorrowedBook, BorrowedBookViewModel>().ReverseMap();
        }
    }
}
