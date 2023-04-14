using Library.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Library.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IConfiguration _configuration;

        public AuthenticationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string LibraryAPIUrl
        {
            get { return _configuration.GetSection("LibraryAPIUrl").Value; }
        }

        [HttpGet]
        public IActionResult Login()
        {
            var login = new LoginModel();
            return View(login) ;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            string Baseurl = LibraryAPIUrl;
 
                using (var client = new HttpClient())
                {

                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                    HttpResponseMessage response = await client.PostAsJsonAsync("api/User/Login", model);

                        if (response.IsSuccessStatusCode)
                        {
                            var answer2 = await response.Content.ReadFromJsonAsync<LoginResponse>();
                            if (answer2 != null)
                            {
                      
                                HttpContext.Response.Cookies.Append("role_name", answer2.Role.ToString());
                                HttpContext.Response.Cookies.Append("user_name", answer2.Username);
                                HttpContext.Response.Cookies.Append("user_status", answer2.isAuthenticated.ToString());
                                HttpContext.Response.Cookies.Append("token", answer2.Token);
                            }
                            return RedirectToAction("GetAllBooks", "Home");
                        }
                        return View();
             
            }
           
        }


        [HttpGet]
        public IActionResult Index()
        {
            var login = new LoginModel();
            return View(login);
        }

        [HttpGet]
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

    }
}
