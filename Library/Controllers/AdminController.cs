using Library.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace Library.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IConfiguration _configuration;
        string token = "";
        string copiesMsg = "";


        public AdminController(IConfiguration configuration,ILogger<AdminController> logger)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public string LibraryAPIUrl
        {
            get { return _configuration.GetSection("LibraryAPIUrl").Value; }
        }

       [HttpGet]
       public IActionResult AddBook()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddBook(BookModel model)
        {
            token = HttpContext.Request.Cookies["token"];
            var token2 = token;
            var answer = "";
            var viewModel = new BookModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(LibraryAPIUrl);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token2);

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Resp = await client.PostAsJsonAsync($"api/Admin/AddBook/", model);
                if (Resp.IsSuccessStatusCode)
                {
                    answer = Resp.Content.ReadAsStringAsync().Result;
                    viewModel = JsonConvert.DeserializeObject<BookModel>(answer);
                
                }

                return RedirectToAction("GetAllBooks");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
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

        [HttpGet]
        public async Task<IActionResult> GetStudentStatus()
        {
            token = HttpContext.Request.Cookies["token"];
            List<StudentModel> books = new List<StudentModel>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(LibraryAPIUrl);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage Resp = await client.GetAsync("api/Admin/GetStudentStatus");

                if (Resp.IsSuccessStatusCode)
                {
                    var answer = Resp.Content.ReadAsStringAsync().Result;
                    books = JsonConvert.DeserializeObject<List<StudentModel>>(answer);
                }
                return View(books);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStudentLoanDetails()
        {
            token = HttpContext.Request.Cookies["token"];
            List<StudentLoanModel> books = new List<StudentLoanModel>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(LibraryAPIUrl);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage Resp = await client.GetAsync("api/Admin/GetStudentLoanDetails");

                if (Resp.IsSuccessStatusCode)
                {
                    var answer = Resp.Content.ReadAsStringAsync().Result;
                    books = JsonConvert.DeserializeObject<List<StudentLoanModel>>(answer);
                }
                return View(books);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetStudentOverDueDetails()
        {
            token = HttpContext.Request.Cookies["token"];
            List<StudentLoanModel> books = new List<StudentLoanModel>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(LibraryAPIUrl);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage Resp = await client.GetAsync("api/Admin/GetStudentOverDueDetails");

                if (Resp.IsSuccessStatusCode)
                {
                    var answer = Resp.Content.ReadAsStringAsync().Result;
                    books = JsonConvert.DeserializeObject<List<StudentLoanModel>>(answer);
                }
                return View(books);
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}