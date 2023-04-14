using Library.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Library.Controllers
{
    public class StudentController : Controller
    {
        private readonly IConfiguration _configuration;
        string token = "";
        string copiesMsg = "";

        public StudentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string LibraryAPIUrl
        {
            get { return _configuration.GetSection("LibraryAPIUrl").Value; }
        }
   

        public IActionResult Index()
        {
            return View();
        }
     

        [HttpGet]
        public IActionResult Return()
        {
            token = HttpContext.Request.Cookies["token"];
            ViewData["token"] = token;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Return(string ISBN)
        {
            //get the user token
            token = HttpContext.Request.Cookies["token"];
            var token2 =  token;
            var answer = "";
            var viewModel = new List<BorrowedBookViewModel>();
          
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(LibraryAPIUrl);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token2);

                //Define request data format
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                 //Sending request to find web api REST service resource to return book using HttpClient
                HttpResponseMessage Resp = await client.PostAsJsonAsync($"api/Student/ReturnBook/{ISBN}",ISBN);

                //Checking the response is successful or not which is sent using HttpClient
                if (Resp.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api
                    answer = Resp.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list
                    var books = JsonConvert.DeserializeObject<BorrowedBookResponse> (answer);
                    copiesMsg = books.Message;
                    ViewData["message"] = copiesMsg;
                    if(books == null)
                    {
                        return View();
                    }
                    foreach (var item in books.Borrowed)
                    {
                        var _borrow = new BorrowedBookViewModel();
                        _borrow.ISBN = item.ISBN;
                        _borrow.DateBorrowed = item.DateBorrowed.ToString("dd-MMM-yyyy");
                        if (item.DateReturned != null)
                        {
                            _borrow.DateReturned = item.DateReturned.Value.ToString("dd-MMM-yyyy");
                        }
                        else
                        {
                            _borrow.DateReturned = null;
                        }
                        _borrow.OverDue = item.OverDue;
                        viewModel.Add(_borrow);
                    }
                        
                }
         
                return View("ListBorrowedBooks",viewModel);
            }
        }


        [HttpGet]
        public IActionResult Borrow()
        {
            token = HttpContext.Request.Cookies["token"];
            ViewData["token"] = token;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Borrow(string ISBN)
        {
            token = HttpContext.Request.Cookies["token"];
            var token2 = token;
            var answer = "";
            var viewModel = new List<BorrowedBookViewModel>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(LibraryAPIUrl);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token2);

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Resp = await client.PostAsJsonAsync($"api/Student/BorrowBook/{ISBN}", ISBN);
                if (Resp.IsSuccessStatusCode)
                {
                    answer = Resp.Content.ReadAsStringAsync().Result;

                    var books = JsonConvert.DeserializeObject<BorrowedBookResponse>(answer);
                    copiesMsg = books.Message;
                    ViewData["message"] = copiesMsg;

                    if (books.Borrowed == null)
                    {
                        return RedirectToAction("GetAllBooks", "Book", copiesMsg);
                    }

                    foreach (var item in books.Borrowed)
                    {
                        var _borrow = new BorrowedBookViewModel();
                        _borrow.ISBN = item.ISBN;
                        _borrow.DateBorrowed = item.DateBorrowed.ToString("dd-MMM-yyyy");
                        if (item.DateReturned != null)
                        {
                            _borrow.DateReturned = item.DateReturned.Value.ToString("dd-MMM-yyyy");
                        }
                        else
                        {
                            _borrow.DateReturned = null;
                        }
                        _borrow.OverDue = item.OverDue;
                        viewModel.Add(_borrow);
                    }

                }

                return View("ListBorrowedBooks", viewModel);
            }
        }




        [HttpGet]
        public async Task<IActionResult> BookAccount()
        {
            token = HttpContext.Request.Cookies["token"];
            ViewData["token"] = token;
            var token2 = token;
            var answer = "";
            var viewModel = new List<BorrowedBookViewModel>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(LibraryAPIUrl);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token2);

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Resp = await client.GetAsync($"api/Student/StudentBookAccount");
                if (Resp.IsSuccessStatusCode)
                {
                    answer = Resp.Content.ReadAsStringAsync().Result;

                    var books = JsonConvert.DeserializeObject<List<BorrowedBookMsg>>(answer);      
                    if(books== null)
                    {
                        return View("ListBorrowedBooks", viewModel);
                    }
                    foreach (var item in books)
                    {
                        var _borrow = new BorrowedBookViewModel();
                        _borrow.ISBN = item.ISBN;
                        _borrow.DateBorrowed = item.DateBorrowed.ToString("dd-MMM-yyyy");
                        if (item.DateReturned != null)
                        {
                            _borrow.DateReturned = item.DateReturned.Value.ToString("dd-MMM-yyyy");
                        }
                        else
                        {
                            _borrow.DateReturned = null;
                        }
                        _borrow.OverDue = item.OverDue;
                        viewModel.Add(_borrow);
                    }

                }

                return View("ListBorrowedBooks", viewModel);
            }
        }
    }
}