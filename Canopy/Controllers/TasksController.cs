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
    [Route("api/tasks")]
    [ApiController]
    public class TasksController : ControllerBase
    {

        private readonly ITasksRepository _taskRepo;
        private readonly INotificationService _notificationService;

        public TasksController(ITasksRepository taskRepo, INotificationService notificationService)
        {
            _taskRepo = taskRepo;
            _notificationService = notificationService;
        }
        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(claim?.Value ?? throw new UnauthorizedAccessException("User not authenticated"));
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var tasks = _taskRepo.GetAllByUser(GetUserId());
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var task = _taskRepo.GetByIdForUser(id, GetUserId());

            if (task == null)
                return NotFound();

            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TaskDataViewModel viewModel)
        {
            try
            {
                var creatorId = GetUserId();
                var assigneeId = viewModel.AssigneeId ?? creatorId;

                var task = new PlannedTask
                {
                    Title = viewModel.Title,
                    Description = viewModel.Description,
                    DeadLine = viewModel.DeadLine,
                    Status = viewModel.Status,
                    ProjectId = viewModel.ProjectId,
                    GroupId = viewModel.GroupId,
                    CreatorId = creatorId,
                    AssignedToUID = assigneeId,
                    DateCreated = DateTime.UtcNow
                };

                var created = _taskRepo.Create(task);

                if (assigneeId != creatorId)
                {
                    await _notificationService.SendAsync(
                        assigneeId,
                        NotificationType.TaskAssigned,
                        JsonSerializer.Serialize(new { taskId = created.Id, taskTitle = created.Title, assignedBy = User.Identity!.Name })
                    );
                }

                return Ok(created);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to create task");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TaskDataViewModel viewModel)
        {
            try
            {
                var target = _taskRepo.GetByIdForUser(id, GetUserId());
                if (target == null)
                {
                    return NotFound("Task not found or access denied");
                }

                var previousAssigneeId = target.AssignedToUID;
                var newAssigneeId = viewModel.AssigneeId ?? GetUserId();

                target.Title = viewModel.Title;
                target.Description = viewModel.Description;
                target.DeadLine = viewModel.DeadLine;
                target.Status = viewModel.Status;
                target.AssignedToUID = newAssigneeId;

                _taskRepo.Update(target);

                if (newAssigneeId != previousAssigneeId && newAssigneeId != GetUserId())
                {
                    await _notificationService.SendAsync(
                        newAssigneeId,
                        NotificationType.TaskAssigned,
                        JsonSerializer.Serialize(new { taskId = target.Id, taskTitle = target.Title, assignedBy = User.Identity!.Name })
                    );
                }

                return Ok(target);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to Update task");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var target = _taskRepo.GetByIdForUser(id, GetUserId());
                if (target == null)
                {
                    return NotFound("Task not found or access denied");
                }
                _taskRepo.Delete(target);

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to remove task");
            }
        }

        [HttpPatch("{id}/status")]
        public IActionResult ToggleStatus(int id)
        {
            var task = _taskRepo.GetAssignedByIdForUser(id, GetUserId());
            if (task == null) return NotFound();

            task.Status = !task.Status; 
            _taskRepo.Update(task);

            return Ok(new { task.Id, task.Status });
        }

    }
}
