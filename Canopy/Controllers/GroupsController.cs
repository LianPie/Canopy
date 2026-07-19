using Canopy.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Canopy.Models;
using Canopy.Services;
using System.Text.Json;

namespace Canopy.Controllers
{
    [Authorize]
    [Route("api/Groups")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupsRepository _repo;
        private readonly IUserRepository _userRepo;
        private readonly INotificationService _notificationService;

        public GroupsController(IGroupsRepository repo, IUserRepository userRepo, INotificationService notificationService)
        {
            _repo = repo;
            _userRepo = userRepo;
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
            var groups = _repo.GetAllByUser(GetUserId());
            return Ok(groups);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var group = _repo.GetById(id, GetUserId());
            if (group == null) return NotFound();
            return Ok(group);
        }

        [HttpPost]
        public IActionResult Create([FromBody] GroupViewModel model)
        {
            try
            {
                var userId = GetUserId();
                var group = new Group
                {
                    Title = model.Title,
                    CreatorId = userId,
                    DateCreated = DateTime.UtcNow,
                    UserGroups = new List<UserGroup>
                    {
                        new UserGroup
                        {
                            UserId = userId,
                            InvitedById = userId,
                            InvitedAt = DateTime.UtcNow,
                            JoinedDate = DateTime.UtcNow,
                            RoleInGroup = "Owner",
                            IsActive = true,
                            Status = InvitationStatus.Accepted
                        }
                    }
                };

                _repo.Create(group);
                return Ok(new { group.Id });
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to create group");
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] GroupViewModel model)
        {
            try
            {
                var group = _repo.GetById(id, GetUserId());
                if (group == null) return NotFound();
                if (group.CreatorId != GetUserId()) return Forbid();

                group.Title = model.Title;
                _repo.Update(group);
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to update group");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var group = _repo.GetById(id, GetUserId());
                if (group == null) return NotFound();
                if (group.CreatorId != GetUserId()) return Forbid();

                _repo.Delete(group);
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to delete group");
            }
        }

        [HttpPost("{id}/invite")]
        public async Task<IActionResult> Invite(int id, [FromBody] InviteUserViewModel model)
        {
            try
            {
                var group = _repo.GetById(id, GetUserId());
                if (group == null) return NotFound("Group not found");

                var target = _userRepo.GetByUserNameOrEmailAsync(model.UsernameOrEmail).Result;
                if (target == null) return NotFound("User not found");

                var existing = _repo.GetMembership(id, target.Id);
                if (existing != null) return BadRequest("User is already a member or has a pending invite");

                var userGroup = new UserGroup
                {
                    GroupId = id,
                    UserId = target.Id,
                    InvitedById = GetUserId(),
                    InvitedAt = DateTime.UtcNow,
                    RoleInGroup = "Member",
                    IsActive = false,
                    Status = InvitationStatus.Pending
                };

                _repo.Invite(userGroup);

                await _notificationService.SendAsync(
                    target.Id,
                    NotificationType.GroupInvitation,
                    JsonSerializer.Serialize(new { groupId = id, groupTitle = group.Title, invitedBy = User.Identity!.Name })
                );

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to send invite");
            }
        }

        [HttpPost("invites/{userGroupId}/accept")]
        public async Task<IActionResult> Accept(int userGroupId)
        {
            try
            {
                var membership = _repo.GetMembershipById(userGroupId, GetUserId());
                if (membership == null) return NotFound();

                membership.Status = InvitationStatus.Accepted;
                membership.JoinedDate = DateTime.UtcNow;
                membership.IsActive = true;

                _repo.UpdateMembership(membership);

                await _notificationService.SendAsync(
                    membership.InvitedById,
                    NotificationType.GroupInvitationAccepted,
                    JsonSerializer.Serialize(new { groupId = membership.GroupId, groupTitle = membership.Group!.Title, acceptedBy = User.Identity!.Name })
                );

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to accept invite");
            }
        }

        [HttpPost("invites/{userGroupId}/decline")]
        public async Task<IActionResult> Decline(int userGroupId)
        {
            try
            {
                var membership = _repo.GetMembershipById(userGroupId, GetUserId());
                if (membership == null) return NotFound();

                membership.Status = InvitationStatus.Declined;

                _repo.UpdateMembership(membership);

                await _notificationService.SendAsync(
                    membership.InvitedById,
                    NotificationType.GroupInvitationDeclined,
                    JsonSerializer.Serialize(new { groupId = membership.GroupId, groupTitle = membership.Group!.Title, declinedBy = User.Identity!.Name })
                );

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to decline invite");
            }
        }

        [HttpGet("{id}/projects")]
        public IActionResult GetGroupProjects(int id)
        {
            var group = _repo.GetById(id, GetUserId());
            if (group == null) return NotFound();
            return Ok(_repo.GetGroupProjects(id));
        }

        [HttpGet("{id}/tasks")]
        public IActionResult GetGroupTasks(int id)
        {
            var group = _repo.GetById(id, GetUserId());
            if (group == null) return NotFound();
            return Ok(_repo.GetGroupTasks(id));
        }

        [HttpGet("invites/pending")]
        public IActionResult GetPendingInvites()
        {
            var invites = _repo.GetPendingInvites(GetUserId());
            return Ok(invites);
        }

        [HttpGet("invites/sent")]
        public IActionResult GetSentInvites()
        {
            var invites = _repo.GetSentInvites(GetUserId());
            return Ok(invites);
        }
    }
}
