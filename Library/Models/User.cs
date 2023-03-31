namespace Library.Models
{
    public class User
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool isAuthenticated { get; set; }
        public int Role { get; set; }
    }

    public class LoginModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    //public class LoginResponse
    //{
    //    public Dictionary<string, Contact> Resp { get; set; }
    //}

    public class LoginResponse
    {
        public string Username { get; set; }
        public string Role { get; set; }
        public bool isAuthenticated { get; set; }
        public string Token { get; set; }
    }
}
