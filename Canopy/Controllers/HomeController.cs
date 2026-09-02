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
        private readonly IEmailSender _emailSender;


        public HomeController(
        ApplicationDbContext context,
        ILogger<HomeController> logger,
        IStringLocalizer<HomeController> localizer,
        IUserRepository repo,
        ITokenService tokenService,
        IEmailSender emailSender)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
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

        public IActionResult TermsOfService()
        {
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


            // CHECK IF EMAIL IS NOT VERIFIED (Status 2 = Unverified)
            if (user.Status == 2)
            {
                // Re-send verification code if expired
                if (!user.VerificationCodeExpiry.HasValue || user.VerificationCodeExpiry < DateTime.UtcNow)
                {
                    var code = new Random().Next(100000, 999999).ToString();
                    user.EmailVerificationCode = code;
                    user.VerificationCodeExpiry = DateTime.UtcNow.AddMinutes(15);
                    await _repo.UpdateAsync(user);

                    await _emailSender.SendEmailAsync(
                        user.Email,
                        "Verify your email",
                        $"Your verification code is: {code}"
                    );
                }

                return Json(new
                {
                    requiresVerification = true,
                    redirectUrl = Url.Action("VerifyEmail", "Home", new { email = user.Email })
                });
            }

            // CREATE SESSION VIA HELPER METHOD
            await IssueSessionCookiesAsync(user, model.RememberMe);

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

            // Generate a 6-digit random code
            var verificationCode = new Random().Next(100000, 999999).ToString();

            User usermodel = new User
            {
                UserName = model.Username,
                Email = model.Email,
                Password = hashed,
                EmailVerificationCode = verificationCode,
                VerificationCodeExpiry = DateTime.UtcNow.AddMinutes(15),
                Status = 2
            };

            await _repo.AddAsync(usermodel);

            // Send the email with the code
            await _emailSender.SendEmailAsync(
                model.Email,
                "Verify your email",
                $"Your verification code is: {verificationCode}"
            );

            // Pass the email or user identifier to the verification view
            return RedirectToAction("VerifyEmail", new { email = model.Email });

        }

        [HttpGet]
        public IActionResult VerifyEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Signup");

            var model = new VerifyEmailViewModel { Email = email };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword([FromForm] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new { code = new[] { "InvalidData" } });
            }

            var user = await _repo.GetByUserNameOrEmailAsync(email);
            if (user == null)
            {
                return BadRequest(new { general = new[] { "UserNotFound" } });
            }

            // 1. Generate a secure random password (e.g. 10 chars)
            string temporaryPassword = GenerateRandomPassword(10);
            string hashedPassword = PasswordHelper.HashPassword(temporaryPassword);

            // 2. Generate a 6-digit verification code
            string verificationCode = new Random().Next(100000, 999999).ToString();

            // 3. Update user record: new password, deactivate status, set verification code
            user.Password = hashedPassword;
            user.Status = 2; // Deactivated / Pending Verification
            user.EmailVerificationCode = verificationCode;
            user.VerificationCodeExpiry = DateTime.UtcNow.AddMinutes(15);

            await _repo.UpdateAsync(user);

            // 4. Send email with temporary password and verification code
            try
            {
                string requestTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm UTC", System.Globalization.CultureInfo.InvariantCulture);
                string emailBody = $@"
            <h3>Password Change Request</h3>
            <p>Password change request at: <strong>{requestTime}</strong></p>
            <p>Your new temporary password: <strong>{temporaryPassword}</strong></p>
            <p>Your verification code: <strong>{verificationCode}</strong></p>
            <p>Please proceed to verify your email so you can log in again.</p>";

                await _emailSender.SendEmailAsync(
                    user.Email,
                    "Password Change Request & Email Verification",
                    emailBody
                );
            }
            catch
            {
                return BadRequest(new { general = new[] { "EmailSendFailed" } });
            }

            // 5. Redirect to VerifyEmail page with the email in query string
            return Json(new { redirectUrl = Url.Action("VerifyEmail", "Home", new { email = user.Email }) });
        }

        // Helper method for random password generation
        private string GenerateRandomPassword(int length)
        {
            const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var random = new Random();
            var chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = validChars[random.Next(0, validChars.Length)];
            }
            return new string(chars);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Code = new[] { "InvalidData" } });
            }

            var user = await _repo.GetByUserNameOrEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest(new { General = new[] { "UserNotFound"} });
            }

            if (user.EmailVerificationCode != model.Code)
            {
                return BadRequest(new { Code = new[] {"InvalidCode"} });
            }

            if (user.VerificationCodeExpiry < DateTime.UtcNow)
            {
                return BadRequest(new { Code = new[] { "CodeExpired" } });
            }

            // Activate user account
            user.Status = 1; // 1 = Active
            user.EmailVerificationCode = null;
            user.VerificationCodeExpiry = null;

            // Issue JWT tokens and Session cookies directly upon verification
            await IssueSessionCookiesAsync(user, rememberMe: false);

            return Json(new { redirectUrl = Url.Action("Welcome", "Home") });
        }

        [HttpGet("/Logout")]
        public IActionResult Logout()
        {
            CookieHelper.Delete(Response, "access_token");
            CookieHelper.Delete(Response, "session_type");
            return RedirectToAction("Login", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        private async Task IssueSessionCookiesAsync(User user, bool rememberMe)
        {
            var token = rememberMe
                ? _tokenService.GenerateToken(user.Id, user.UserName, expiryDays: 7)
                : _tokenService.GenerateToken(user.Id, user.UserName);

            if (rememberMe)
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
                // Session only
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

            user.LastLogin = DateTime.UtcNow;
            await _repo.UpdateAsync(user);
        }
    }
}
