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
    [Route("api/Projects")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {

        private readonly IProjectsRepository _projectRepo;
        private readonly ITasksRepository _taskRepo;
        private readonly INotificationService _notificationService;

        public ProjectsController(ITasksRepository taskRepo, IProjectsRepository projectRepo, INotificationService notificationService)
        {
            _projectRepo = projectRepo;
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
            var tasks = _projectRepo.GetAllByUser(GetUserId());
            return Ok(tasks);
        }

        [HttpGet("paged")]
        public IActionResult GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 12)
        {
            var (items, hasMore) = _projectRepo.GetPage(GetUserId(), page, pageSize);
            return Ok(new { items, hasMore });
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var task = _projectRepo.GetByIdForUser(id, GetUserId());

            if (task == null)
                return NotFound();

            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProjectDataViewModel viewModel)
        {
            try
            {
                var userId = GetUserId();
                var project = new Project
                {
                    Title = viewModel.Title,
                    Description = viewModel.Description,
                    Deadline = viewModel.Deadline,
                    Status = false,
                    GroupId = viewModel.Group,
                    CreatorId = userId,
                    DateCreated = DateTime.UtcNow
                };

                var created = _projectRepo.Create(project);
                if (viewModel.Tasks?.Count > 0)
                {
                    var projectTasks = new List<PlannedTask>();
                    foreach (var task in viewModel.Tasks)
                    {
                        projectTasks.Add(new PlannedTask
                        {
                            Title = task.Title,
                            Description = task.Description,
                            DeadLine = task.DeadLine,
                            Status = task.Status,
                            CreatorId = userId,
                            AssignedToUID = task.AssigneeId ?? userId,
                            ProjectId = created.Id,
                            DateCreated = DateTime.UtcNow
                        });
                    }
                    _taskRepo.AddRange(projectTasks);

                    var payload = JsonSerializer.Serialize(new { projectId = created.Id, projectTitle = created.Title, assignedBy = User.Identity!.Name });
                    var assigneeIds = viewModel.Tasks
                        .Select(t => t.AssigneeId ?? userId)
                        .Where(id => id != userId)
                        .Distinct();

                    foreach (var assigneeId in assigneeIds)
                        await _notificationService.SendAsync(assigneeId, NotificationType.ProjectAssigned, payload);
                }

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to create project");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProjectDataViewModel viewModel)
        {
            try
            {
                var target = _projectRepo.GetByIdForUser(id, GetUserId());
                if (target == null)
                {
                    return NotFound("Project not found or access denied");
                }

                target.Title = viewModel.Title;
                target.Description = viewModel.Description;
                target.Deadline = viewModel.Deadline;
                target.Status = viewModel.Status;

                _projectRepo.Update(target);

                var existingTasks = _taskRepo.GetByProjectId(id, GetUserId());

                if (viewModel.Tasks == null)
                {
                    viewModel.Tasks = new List<TaskDataViewModel>();
                }

                var incomingTaskIds = viewModel.Tasks.Where(t => t.Id > 0).Select(t => t.Id).ToList();
                var tasksToDelete = existingTasks.Where(t => !incomingTaskIds.Contains(t.Id)).ToList();
                if (tasksToDelete.Any())
                {
                    _taskRepo.RemoveRange(tasksToDelete); 
                }

                var tasksToAdd = new List<PlannedTask>();

                foreach (var incomingTask in viewModel.Tasks)
                {
                    if (incomingTask.Id == 0)
                    {
                        // It's a new task
                        tasksToAdd.Add(new PlannedTask
                        {
                            Title = incomingTask.Title,
                            Description = incomingTask.Description,
                            DeadLine = incomingTask.DeadLine,
                            Status = incomingTask.Status,
                            ProjectId = id,
                            CreatorId = GetUserId(),
                            AssignedToUID = incomingTask.AssigneeId ?? GetUserId(),
                            DateCreated = DateTime.UtcNow
                        });
                    }
                    else
                    {
                        // It's an existing task, update it
                        var dbTask = existingTasks.FirstOrDefault(t => t.Id == incomingTask.Id);
                        if (dbTask != null)
                        {
                            dbTask.Title = incomingTask.Title;
                            dbTask.Description = incomingTask.Description;
                            dbTask.DeadLine = incomingTask.DeadLine;
                            dbTask.Status = incomingTask.Status;
                            if (incomingTask.AssigneeId.HasValue)
                                dbTask.AssignedToUID = incomingTask.AssigneeId.Value;

                            _taskRepo.Update(dbTask); // Save individual task changes
                        }
                    }
                }

                if (tasksToAdd.Any())
                {
                    _taskRepo.AddRange(tasksToAdd);
                }

                var userId = GetUserId();
                var payload = JsonSerializer.Serialize(new { projectId = id, projectTitle = target.Title, assignedBy = User.Identity!.Name });
                var previousAssigneeIds = existingTasks.Select(t => t.AssignedToUID).ToHashSet();
                var newAssigneeIds = viewModel.Tasks
                    .Select(t => t.AssigneeId ?? userId)
                    .Where(aid => aid != userId && !previousAssigneeIds.Contains(aid))
                    .Distinct();

                foreach (var assigneeId in newAssigneeIds)
                    await _notificationService.SendAsync(assigneeId, NotificationType.ProjectAssigned, payload);

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to Update");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var target = _projectRepo.GetByIdForUser(id, GetUserId());
                if (target == null)
                {
                    return NotFound("Project not found or access denied");
                }
                _projectRepo.Delete(target);

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
            var project = _projectRepo.GetByIdForUser(id, GetUserId());
            if (project == null) return NotFound();

            project.Status = !project.Status;
            _projectRepo.Update(project);

            return Ok(new { project.Id, project.Status });
        }

    }
}
