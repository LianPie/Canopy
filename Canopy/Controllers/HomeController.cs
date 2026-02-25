using Canopy.Data;
using Canopy.Helpers;
using Canopy.Models;
using Canopy.Repositories;
using Canopy.Services;
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
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly IUserRepository _repo;
        private readonly ITokenService _tokenService;


        public HomeController(
        ApplicationDbContext context,
        ILogger<HomeController> logger,
        IStringLocalizer<HomeController> localizer,
        IUserRepository repo,
        ITokenService tokenService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }



        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        public IActionResult SignUp()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        public IActionResult Welcome()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            //auth proccess
            var user = await _repo.GetByUserNameOrEmailAsync(model.Username);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, _localizer["InvalidCredentials"]);
                return BadRequest(new
                {
                    message = _localizer["InvalidCredentials"],
                    errors = ModelState.ToDictionary(
                        x => x.Key,
                        x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    )
                });
            }

            var security = await _repo.GetSecurityByUserIdAsync(user.Id);
            if (security == null)
            {
                ModelState.AddModelError(string.Empty, _localizer["UnableToVerifyAccount"]);
                return BadRequest(new
                {
                    message = _localizer["UnableToVerifyAccount"],
                    errors = ModelState.ToDictionary(
                        x => x.Key,
                        x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    )
                });
            }
            if (security.LockoutUntil.HasValue && security.LockoutUntil.Value > DateTime.UtcNow)
            {
                DateTime now = DateTime.UtcNow;
                double minutes = (security.LockoutUntil.Value - now).TotalMinutes;
                var lockoutMessage = _localizer["AccountLocked", (int)Math.Round(minutes)];
                ModelState.AddModelError(string.Empty, lockoutMessage);
                return BadRequest(new
                {
                    message = lockoutMessage,
                    errors = ModelState.ToDictionary(
                        x => x.Key,
                        x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    )
                });
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

                    DateTime now = DateTime.UtcNow;
                    double minutes = (security.LockoutUntil.Value - now).TotalMinutes;
                    var lockoutMessage = _localizer["AccountLocked", (int)Math.Round(minutes)];
                    ModelState.AddModelError(string.Empty, lockoutMessage);
                    return BadRequest(new
                    {
                        message = lockoutMessage,
                        errors = ModelState.ToDictionary(
                            x => x.Key,
                            x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        )
                    });

                }
                else
                {
                    ModelState.AddModelError(string.Empty, _localizer["InvalidCredentials"]);
                    return BadRequest(new
                    {
                        message = _localizer["InvalidCredentials"],
                        errors = ModelState.ToDictionary(
                        x => x.Key,
                        x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    )
                    });
                }
            }


            await _repo.ResetFailedAttemptsAsync(user.Id);


            //session and cookie
            var token = _tokenService.GenerateToken(user.Id);

            if (model.RememberMe)
            {
                // Remember Me = 7 days 
                CookieHelper.Set(
                    response: Response,
                    key: "access_token",
                    value: token,
                    expiresDays: 7,
                    httpOnly: true,
                    secure: true
                );

                CookieHelper.Set(
                    response: Response,
                    key: "session_type",
                    value: "Remember",
                    expiresDays: 7,
                    httpOnly: false,
                    secure: true
                );
            }
            else
            {
                // Session only = browser closes = NO expiration set
                var options = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                };
                Response.Cookies.Append("access_token", token, options);

                Response.Cookies.Append("session_type", "temporary", new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddDays(1),
                    HttpOnly = false,
                    Secure = true
                });
            }

            // Return user data
            return Ok(new
            {
                message = _localizer["Success", user.UserName],
                user = new
                {
                    user.Id,
                    user.UserName,
                    user.Email
                },
                rememberMe = model.RememberMe
            });

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
