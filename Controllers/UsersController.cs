using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Empty_MVC.Data;
using Empty_MVC.Models;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Empty_MVC.Controllers
{
    public class UsersController : Controller
    {
        //Session Constants
        const string SessionUserName = "_UserName";
        const string SessionRole = "_Role";
        const string SessionEmail = "_Email";

        private readonly MyDataContext db;

        private readonly ILogger<UsersController> _logger;

        private readonly IConfiguration _config;

        public UsersController(ILogger<UsersController> logger, MyDataContext db, IConfiguration configuration)
        {
            _logger = logger;
            _config = configuration;
            this.db = db;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        //User Login/Index
        [HttpPost]
        public async Task<ActionResult> Index(string UserName, string Password)
        {
            //Intialize User Model
            var users = new UsersModel();

            //Check User
            var user_check = db.UsersModels.Where(x => x.UserName == UserName);

            if (user_check.Count() > 0)
            {
                //Get User
                var user_record = user_check.First();

                //Check Password
                //Salt/HashPassword
                using (SHA512 shaM = new SHA512Managed())
                {
                    //GetSalt
                    var salt = _config["PasswordSalt"];

                    //Convert Salted Password to Bytes
                    var data = Encoding.UTF8.GetBytes(salt + Password + salt);

                    //Get Hash - Bytes
                    var hash_password = shaM.ComputeHash(data);

                    //Convert Bytes to String
                    var hashedInputStringBuilder = new System.Text.StringBuilder(64);
                    foreach (var b in hash_password)
                    {
                        hashedInputStringBuilder.Append(b.ToString("X2"));
                    }

                    //Databse String Hash
                    var db_hash = hashedInputStringBuilder.ToString();

                    //Validate Password
                    if(user_record.Password == db_hash)
                    {
                        
                        //Cookie
                        var userClaims = new List<Claim>(){
                                            new Claim(ClaimTypes.Name, user_record.UserName)
                                        };

                        var identity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);

                        var userPrincipal = new ClaimsPrincipal(new[] { identity });

                        await HttpContext.SignInAsync(userPrincipal);

                        //Set Session Values
                        HttpContext.Session.SetString(SessionUserName, user_record.UserName);
                        HttpContext.Session.SetString(SessionRole, user_record.Role);
                        HttpContext.Session.SetString(SessionEmail, user_record.Email);

                        //Determine User Landing Page
                        switch(user_record.Role)
                        {
                            case "User":
                                return RedirectToAction("Index", "Home");                          
                            case "Admin":
                                return RedirectToAction("Index", "Admin");
                            default:
                                return RedirectToAction("Index", "Home");
                        }

                    }
                    else
                    {
                        //Error User Not Found
                        ViewBag.ErrorMessage = "Password incorrect!";

                        return View();
                    }
                }
            }
            else
            {
                //Error User Not Found
                ViewBag.ErrorMessage = "User not found!";

                return View();
            }

        }

        //User Logout
        [HttpPost]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);           
            return RedirectToAction("Index", new UsersModel());
        }

    }
}
