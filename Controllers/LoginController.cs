using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using mongoAuth.Models;
using MongoDB.Bson;

namespace mongoAuth.Controllers
{
    public class LoginController : Controller
    {
        private readonly IMongoCollection<UserModel> _mongoDB;

        public LoginController(IMongoClient mongoClient)
        {
            _mongoDB = mongoClient.GetDatabase("CodeCraft").GetCollection<UserModel>("users");
        }
        public ActionResult Login(bool registered)
        {
            if (registered)
            {
                ViewBag.Registered = "Thank you for Registering";
            } else
            {
                ViewBag.Registered = null;
            }
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            bool loginGood = await CheckCredentials(email, password);
            // Your login logic here
            if (loginGood)
            {
                // Redirect to the main page or wherever you want to redirect after successful login
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Return to the login page with an error message
                ViewBag.Error = "Invalid Email or password";
                return View();
            }
        }



        [HttpPost]
        public async Task<IActionResult> Register(string email, string password)
        {
            // Check if the user already exists in the database
            var userExists = await CheckUserExists(email);
            if (userExists)
            {
                ViewBag.Error = "User with this email already exists";
                return View("Register");
            } else
            {
                // Create a new user document
                var userDocument = new UserModel
                {
                    Email = email,
                    Password = password,
                    Role = "user",
                    Projects = new List<String> { },
                    Level = "1"
                };


                ;

                // Insert the new user document into the database
                await CreateUser(userDocument);

                // Registration successful, you can redirect to the login page or any other desired page
                return RedirectToAction("Login", "Login", new { registered = true });
            }


        }

        private async Task CreateUser(UserModel user)
        {
            await _mongoDB.InsertOneAsync(user);
        }

        private async Task<bool> CheckUserExists(string email)
        {
            var filter = Builders<UserModel>.Filter.Eq("Email", email);
            var user = await _mongoDB.Find(filter).FirstOrDefaultAsync();

            if (user != null)
            {

                // User Exists
                return true;

            }

            // User Doesn't exist
            return false;
        }


        private async Task<bool> CheckCredentials(string email, string password)
        {
            var filter = Builders<UserModel>.Filter.Eq("Email", email);
            var user = await _mongoDB.Find(filter).FirstOrDefaultAsync();

            if (user != null)
            {
                string storedPassword = user.Password;

                // Compare the provided password with the stored password
                if (password == storedPassword)
                {
                    // Passwords match
                    return true;
                }
            }

            // Email or password is incorrect
            return false;
        }
    }
}
