using Canopy.Repositories;
using Microsoft.AspNetCore.Mvc;

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
    }
}
