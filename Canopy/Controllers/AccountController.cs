using Canopy.Helpers;
using Canopy.Models;
using Canopy.Repositories;
using Canopy.Repositories.TaskManager.Repositories;
using Canopy.Services;
using crypto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace Canopy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserRepository _repo;
        private readonly IEmailSender _emailSender;
        public AccountController(IUserRepository repo,
        IEmailSender emailSender)
        {
            _repo = repo;
            _emailSender = emailSender;
        }

        [Authorize]
        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(claim?.Value ?? throw new UnauthorizedAccessException("User not authenticated"));
        }

        [HttpGet("checkUser")]
        public async Task<IActionResult> CheckAvailability(
           [FromQuery] string username)
        {
            var userNameTaken = await _repo.UserNameExistsAsync(username);

            return Ok(new { userNameTaken });
        }

        [HttpGet("checkEmail")]
        public async Task<IActionResult> CheckEmailAvailability(
           [FromQuery] string email)
        {
            var emailTaken = await _repo.EmailExistsAsync(email);

            return Ok(new { emailTaken });
        }

        [HttpGet("sendCode")]
        public async Task<IActionResult> sendCode(
           [FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new { code = new[] { "InvalidData" } });
            }

            var user = await _repo.GetByUserNameOrEmailAsync(email);
            if (user == null)
            {
                return BadRequest(new { general = new[] { "UserNotFound" } });
            }

            // Generate a new 6-digit verification code
            var newCode = new Random().Next(100000, 999999).ToString();
            user.EmailVerificationCode = newCode;
            user.VerificationCodeExpiry = DateTime.UtcNow.AddMinutes(15);

            await _repo.UpdateAsync(user);

            // Send email
            try
            {
                await _emailSender.SendEmailAsync(
                    user.Email,
                    "Verify your email",
                    $"Your new verification code is: {newCode}"
                );
            }
            catch
            {
                return BadRequest(new { general = new[] { "EmailSendFailed" } });
            }

            return Ok(new { success = true, key = "CodeResent" });
        }

        [Authorize]
        [HttpPatch("changePfp")]
        public IActionResult changePfp([FromQuery] string img)
        {
            var user = _repo.GetById(GetUserId());
            if (user == null) return NotFound();

            user.ImageUrl = img;
            _repo.UpdateAsync(user);

            return Ok();
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ChangePasswordRequest model)
        {
            try
            {
                var user = _repo.GetById(GetUserId());
                if (user == null) return NotFound();


                var passwordOk = await _repo.VerifyPasswordAsync(user, model.OldPassword);
                if (!passwordOk)
                {
                    await _repo.IncrementFailedAttemptsAsync(user.Id);

                    ModelState.AddModelError(string.Empty, "InvalidCredentials");
                    return BadRequest(new
                    {
                        message = "InvalidCredentials",
                        errors = ModelState.ToDictionary(
                        x => x.Key,
                        x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    )
                    });
                }

                if (model.NewPassword != model.ConfirmNewPassword)
                    return BadRequest(new
                    {
                        message = "ConfirmNoMatch",
                        errors = ModelState.ToDictionary(
                            x => x.Key,
                            x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        )
                    });

                var hashed = PasswordHelper.HashPassword(model.NewPassword);
                user.Password = hashed;

                await _repo.UpdateAsync(user);

                return Ok(user);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to Update task");
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete()
        {
            try
            {
                var user = _repo.GetById(GetUserId());
                if (user == null) return NotFound();

                _repo.DeleteAsync(user);
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to delete account");
            }
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var user = _repo.GetById(userId);

            if (user == null)
                return NotFound();

            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.ImageUrl
            });
        }

        [Authorize]
        [HttpPost("RequestDeleteAccountCode")]
        public async Task<IActionResult> RequestDeleteAccountCode()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var user = _repo.GetById(userId);
            if (user == null)
                return NotFound(new { general = new[] { "UserNotFound" } });

            // Generate 6-digit deletion verification code
            var verificationCode = new Random().Next(100000, 999999).ToString();
            user.EmailVerificationCode = verificationCode;
            user.VerificationCodeExpiry = DateTime.UtcNow.AddMinutes(15);

            await _repo.UpdateAsync(user);

            try
            {
                string requestTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm UTC", System.Globalization.CultureInfo.InvariantCulture);
                string emailBody = $@"
            <h3>Account Deletion Request</h3>
            <p>A request was made to permanently delete your Canopy account on: <strong>{requestTime}</strong></p>
            <p>Your verification code is: <strong>{verificationCode}</strong></p>
            <p>If you did not request this, please change your password immediately.</p>";

                await _emailSender.SendEmailAsync(
                    user.Email,
                    "Confirm Account Deletion",
                    emailBody
                );
            }
            catch
            {
                return BadRequest(new { general = new[] { "EmailSendFailed" } });
            }

            return Ok(new { success = true, key = "CodeResent" });
        }

        [Authorize]
        [HttpPost("DeleteAccount")]
        public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountRequestModel model)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            if (model == null || string.IsNullOrWhiteSpace(model.Code))
            {
                return BadRequest(new { code = new[] { "InvalidData" } });
            }

            var user = _repo.GetById(userId);
            if (user == null)
                return NotFound(new { general = new[] { "UserNotFound" } });

            // Verify confirmation code
            if (user.EmailVerificationCode != model.Code)
            {
                return BadRequest(new { code = new[] { "InvalidCode" } });
            }

            if (!user.VerificationCodeExpiry.HasValue || user.VerificationCodeExpiry < DateTime.UtcNow)
            {
                return BadRequest(new { code = new[] { "CodeExpired" } });
            }

            _repo.DeleteAsync(user);

            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("session_type");

            return Ok(new { success = true });
        }


    }
}
