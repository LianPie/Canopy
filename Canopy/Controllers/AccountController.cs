using Canopy.Repositories;
using Canopy.Repositories.TaskManager.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Canopy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserRepository _repo;
        public AccountController(IUserRepository repo) => _repo = repo;

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
                user.Email
            });
        }
    }
}
