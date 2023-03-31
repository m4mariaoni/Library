using Library.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Library.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IConfiguration _configuration;
        string token = "";
        string copiesMsg = "";


        public HomeController(IConfiguration configuration, ILogger<AdminController> logger)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public string LibraryAPIUrl
        {
            get { return _configuration.GetSection("LibraryAPIUrl").Value; }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBooks(string msg)
        {
            token = HttpContext.Request.Cookies["token"];
            List<BookModel> books = new List<BookModel>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(LibraryAPIUrl);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage Resp = await client.GetAsync("api/Admin/GetAllBooks");

                if (Resp.IsSuccessStatusCode)
                {
                    var answer = Resp.Content.ReadAsStringAsync().Result;
                    books = JsonConvert.DeserializeObject<List<BookModel>>(answer);
                }
                return View(books);
            }
        }
    }
}
