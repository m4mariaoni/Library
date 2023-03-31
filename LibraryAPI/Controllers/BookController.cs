using LibraryAPI.Data.Entity;
using LibraryAPI.Data.Models;
using LibraryAPI.Service.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {

        private readonly IBookService _bookService;
        Uri address;
        string url;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

  


        [HttpPost("BorrowBook/{isbn}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> BorrowBook(string isbn)
        {
            long id = CurrentUserId;
           var result = await _bookService.AddBorrowedBooks(isbn,id);
           return Ok(result);
        }

        [HttpPost("ReturnBook/{isbn}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ReturnBook(string isbn)
        {
            long id = CurrentUserId;
            var result = await _bookService.ReturnBorrowedBooks(isbn, id);
            return Ok(result);
        }


        [HttpGet("StudentBookAccount")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> StudentBorrowedBooks()
        {
            long id = CurrentUserId;
            var result = await _bookService.StudentBorrowedBooks(id);
            return Ok(result);
        }


        public long CurrentUserId
        {
            get
            {
                var _user = HttpContext.User;

                var _id = 0L;
                long.TryParse(_user.FindFirst(ClaimTypes.PrimarySid).Value, out _id);
                return _id;
            }
        }
    }
}
