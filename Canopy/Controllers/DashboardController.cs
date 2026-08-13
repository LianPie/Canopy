using Canopy.Models;
using Canopy.Repositories;
using Canopy.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Canopy.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IUserRepository _userRepo;
        private readonly ITasksRepository _tasksRepo;
        private readonly IProjectsRepository _projectRepo;
        private readonly IGroupsRepository _groupRepo;
        private readonly IChatService _chatService;
        public DashboardController(IUserRepository userRepo, ITasksRepository taskRepo, IProjectsRepository projectRepo,
            IGroupsRepository groupRepo, IChatService chatService)
        {
            _userRepo = userRepo;
            _tasksRepo = taskRepo;
            _projectRepo = projectRepo;
            _groupRepo = groupRepo;
            _chatService = chatService;
        }
        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(claim?.Value ?? throw new UnauthorizedAccessException("User not authenticated"));
        }

        public IActionResult Index()
        {
            DashboardViewData model = _tasksRepo.GetDashboardStats(GetUserId()) ?? new DashboardViewData();

            return View(model);
        }
        public IActionResult Tasks()
        {

            return View();
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
                    model.Recurrence = task.Recurrence;
                    model.RecurrenceWeekday = task.RecurrenceWeekday;
                    model.RecurrenceMonthDay = task.RecurrenceMonthDay;
                    model.IsRecurrenceEnded = task.IsRecurrenceEnded;
                }
            }

            return View(model);
        }

        public IActionResult Projects()
        {
            return View();
        }

        public IActionResult Groups()
        {
            var model = _groupRepo.GetAllByUser(GetUserId());
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
                    model.Status = project.Status;
                    model.Tasks = project.Tasks; 
                    model.CreatorName = project.Creator?.UserName ?? "Unknown";
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ChatRoomTab(int groupId)
        {
            var userId = GetUserId();

            var isMember = _groupRepo.GetMembership(groupId, userId) is not null;
            if (!isMember)
                return Forbid();


            var chat = await _chatService.GetOrCreateChatForGroupAsync(groupId);
            var messages = await _chatService.GetMessagesAsync(chat.Id);


            var vm = new ChatRoomViewModel
            {
                ChatId = chat.Id,
                GroupId = groupId,
                CurrentUserId = userId,
                Messages = messages,

            };

            return PartialView("_ChatRoomPartial", vm);
        }

        public IActionResult Profile()
        {
            var user = _userRepo.GetById(GetUserId());
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new ProfileViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                ProfilePictureUrl = user.ImageUrl,
                DateCreated = user.DateCreated,
                LastLoginAt = user.LastLogin
            };

            return View(model);
        }

        public IActionResult Settings()
        {
            return View();
        }

    }
}
