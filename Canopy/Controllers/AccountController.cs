using Canopy.Helpers;
using Canopy.Models;
using Canopy.Repositories;
using Canopy.Repositories.TaskManager.Repositories;
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
        public AccountController(IUserRepository repo) => _repo = repo;

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

            return Ok(new {emailTaken });
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
    }
}
