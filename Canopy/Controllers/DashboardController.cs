using Canopy.Models;
using Canopy.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Canopy.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ITasksRepository _tasksRepo;
        public DashboardController(ITasksRepository taskRepo)
        {
            _tasksRepo = taskRepo;
        }
        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(claim?.Value ?? throw new UnauthorizedAccessException("User not authenticated"));
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Tasks()
        {
            var model = _tasksRepo.GetAllByUser(GetUserId());


            return View(model);
        }

        [HttpGet]
        public IActionResult TaskForm(int? id)
        {
            var model = new TaskFormViewModel();

            if (id.HasValue)
            {
                var task = _tasksRepo.GetByIdForUser(id.Value, GetUserId());
                if (task != null)
                {
                    model.Id = task.Id;
                    model.Title = task.Title;
                    model.Description = task.Description;
                    model.DeadLine = task.DeadLine;
                    model.Status = task.Status;
                }
            }

            return View(model);
        }
    }
}
