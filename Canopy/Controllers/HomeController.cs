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
