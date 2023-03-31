using LibraryAPI.Data.Models;
using LibraryAPI.Service.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController :  ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IConfiguration _configuration;
        Uri address;
        string url;

        public AdminController(IAdminService adminService, IConfiguration configuration)
        {
            _adminService = adminService;
            _configuration = configuration;
        }
        public string LibraryAPIUrl
        {
            get { return _configuration.GetSection("LibraryAPIUrl").Value; }
        }

        /// <summary>
        /// Add a new Student/Admin Account to Library
        /// </summary>
        /// <param name="Account"></param>
        /// <returns></returns>
        [HttpPost("AddBook")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> AddBooks(BookModel model)
        {

            address = new Uri(Request.Host.ToString());
            url = address.ToString() + "/account";

            if (model == null)
            {
                return BadRequest(ModelState);
            }
            var result = await _adminService.AddBook(model, url);
            return Ok(result);
        }

        /// <summary>
        /// Get the list of Users
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllBooks")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllBooks()
        {
            address = new Uri(Request.Host.ToString());
            url = address.ToString() + "/account";

            var accountList = await _adminService.GetAllBooks(url);
            if (accountList == null)
            {
                return NotFound();
            }
            return Ok(accountList);
        }


        [HttpGet("GetStudentStatus")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetStudentStatus()
        {           
            var result = await _adminService.StudentListBooks();
            return Ok(result);
        }

        [HttpGet("GetStudentLoanDetails")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetStudentLoanDetails()
        {
            var result = await _adminService.StudentLoanDetails();
            return Ok(result);
        }

        [HttpGet("GetStudentOverDueDetails")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetStudentOverDueLoanDetails()
        {
            var result = await _adminService.StudentOverDueDetails();
            return Ok(result);
        }

    }
}
