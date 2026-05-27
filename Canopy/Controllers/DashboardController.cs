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
        private readonly IProjectsRepository _projectRepo;
        public DashboardController(ITasksRepository taskRepo, IProjectsRepository projectRepo)
        {
            _tasksRepo = taskRepo;
            _projectRepo = projectRepo;
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

        public IActionResult Projects()
        {
            var model = _projectRepo.GetAllByUser(GetUserId());


            return View(model);
        }

        [HttpGet]
        public IActionResult ProjectForm(int? id)
        {
            var model = new ProjectViewModel();

            if (id.HasValue)
            {
                var project = _projectRepo.GetByIdForUser(id.Value, GetUserId());
                if (project != null)
                {
                    model.Id = project.Id;
                    model.Title = project.Title;
                    model.Description = project.Description;
                    model.Deadline = project.Deadline;
                    model.IsActive = project.IsActive;
                    model.Tasks = project.Tasks; 
                    model.CreatorName = project.Creator?.UserName ?? "Unknown";
                }
            }

            return View(model);
        }
    }
}
