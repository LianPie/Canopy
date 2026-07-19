using Canopy.Models;
using Canopy.Repositories;
using Canopy.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Canopy.Controllers
{
    [Authorize]
    [Route("api/Notifications")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {

        private readonly INotificationRepository _Repo;

        public NotificationsController(INotificationRepository Repo)
        {
            _Repo = Repo;
        }
        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(claim?.Value ?? throw new UnauthorizedAccessException("User not authenticated"));
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var tasks = _Repo.GetAllByUser(GetUserId());
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var task = _Repo.GetByIdForUser(id, GetUserId());

            if (task == null)
                return NotFound();

            return Ok(task);
        }
        

        [HttpPatch("{id}/read")]
        public IActionResult read(int id)
        {
            var notif = _Repo.GetByIdForUser(id, GetUserId());
            if (notif == null) return NotFound();

            notif.IsRead = true;
            _Repo.Update(notif);

            return Ok(new { notif.Id });
        }



        [HttpPatch("read-all")]
        public IActionResult readAll()
        {
            _Repo.MarkAllAsRead(GetUserId());
            return Ok();
        }

    }
}
