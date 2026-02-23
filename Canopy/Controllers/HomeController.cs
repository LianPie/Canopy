using Canopy.Data;
using Canopy.Helpers;
using Canopy.Models;
using Canopy.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Diagnostics;

namespace Canopy.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly IUserRepository _repo;


        public HomeController(
        ApplicationDbContext context,
        ILogger<HomeController> logger,
        IStringLocalizer<SharedResources> localizer,
        IUserRepository repo)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }



        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult SignUp()
        {
            return View();
        }

        public IActionResult Welcome()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            //auth proccess
            var user = await _repo.GetByUserNameOrEmailAsync(model.Username);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid credentials.");
                return View(model);
            }

            var security = await _repo.GetSecurityByUserIdAsync(user.Id);
            if (security == null)
            {
                ModelState.AddModelError(string.Empty, "Unable to verify account status.");
                return View(model);
            }
            if (security.LockoutUntil.HasValue && security.LockoutUntil.Value > DateTime.UtcNow)
            {
                ModelState.AddModelError(string.Empty,
                    $"Account locked until {security.LockoutUntil.Value:u}.");
                return View(model);
            }


            const int maxAttempts = 5;
            var lockoutUntil = DateTime.UtcNow.AddMinutes(15);

            var passwordOk = await _repo.VerifyPasswordAsync(user, model.Password);
            if (!passwordOk)
            {
                await _repo.IncrementFailedAttemptsAsync(user.Id);

                if (security.FailedLoginAttempts > maxAttempts)
                {
                    await _repo.LockoutAsync(user.Id, lockoutUntil);
                    ModelState.AddModelError(string.Empty, $"Too many failed attempts. Account locked until {lockoutUntil:u}.");
                }
                else
                    ModelState.AddModelError(string.Empty, "Invalid credentials.");

                return View(model);
            }

            await _repo.ResetFailedAttemptsAsync(user.Id);


            //session and cookie
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.UserName);


            return RedirectToAction("index", "Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Signup(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (!model.AcceptTerms)
            {
                ModelState.AddModelError(nameof(model.AcceptTerms),
                                         "You must accept the terms and conditions.");
                return View(model);
            }

            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError(nameof(model.ConfirmPassword),
                                         "Passwords do not match.");
                return View(model);
            }



            if (await _repo.UserNameExistsAsync(model.Username))
            {
                ModelState.AddModelError(nameof(model.Username),
                                         "Username is already taken");
                return View(model);
            }

            if (await _repo.EmailExistsAsync(model.Email))
            {
                ModelState.AddModelError(nameof(model.Username),
                                         "Email is already taken");
                return View(model);
            }


            var hashed = PasswordHelper.HashPassword(model.Password);
            model.Password = hashed;



            User usermodel = new User
            {
                Id = 0,
                UserName = model.Username,
                Email = model.Email,
                Password = model.Password,
            };

            await _repo.AddAsync(usermodel);

            return RedirectToAction("Welcome", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
