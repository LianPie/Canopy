using Canopy.Data;
using Canopy.Models;
using Microsoft.EntityFrameworkCore;

namespace Canopy.Repositories
{
    public class GroupsRepository : IGroupsRepository
    {
        private readonly ApplicationDbContext _ctx;
        public GroupsRepository(ApplicationDbContext ctx) => _ctx = ctx;

        public List<Group> GetAllByUser(int userId)
        {
            return _ctx.Group
                .Where(g => g.CreatorId == userId ||
                            g.UserGroups.Any(ug => ug.UserId == userId && ug.Status == InvitationStatus.Accepted))
                .Include(g => g.Creator)
                .Include(g => g.UserGroups)
                    .ThenInclude(ug => ug.User)
                .ToList();
        }

        public Group? GetById(int id, int userId)
        {
            return _ctx.Group
                .Include(g => g.Creator)
                .Include(g => g.UserGroups)
                    .ThenInclude(ug => ug.User)
                .FirstOrDefault(g => g.Id == id &&
                    (g.CreatorId == userId ||
                     g.UserGroups.Any(ug => ug.UserId == userId && ug.Status == InvitationStatus.Accepted)));
        }

        public Group Create(Group group)
        {
            _ctx.Group.Add(group);
            _ctx.SaveChanges();
            return group;
        }

        public Group Update(Group group)
        {
            _ctx.Group.Update(group);
            _ctx.SaveChanges();
            return group;
        }

        public void Delete(Group group)
        {
            _ctx.Group.Remove(group);
            _ctx.SaveChanges();
        }

        public UserGroup? GetMembership(int groupId, int userId)
        {
            return _ctx.UserGroup
                .FirstOrDefault(ug => ug.GroupId == groupId && ug.UserId == userId);
        }

        public UserGroup? GetMembershipById(int userGroupId, int userId)
        {
            return _ctx.UserGroup
                .Include(ug => ug.Group)
                .FirstOrDefault(ug => ug.Id == userGroupId && ug.UserId == userId);
        }

        public List<UserGroup> GetPendingInvites(int userId)
        {
            return _ctx.UserGroup
                .Where(ug => ug.UserId == userId && ug.Status == InvitationStatus.Pending)
                .Include(ug => ug.Group)
                .Include(ug => ug.InvitedBy)
                .ToList();
        }

        public List<UserGroup> GetSentInvites(int userId)
        {
            return _ctx.UserGroup
                .Where(ug => ug.InvitedById == userId && ug.Status == InvitationStatus.Pending)
                .Include(ug => ug.Group)
                .Include(ug => ug.User)
                .ToList();
        }

        public List<Project> GetGroupProjects(int groupId)
        {
            return _ctx.Projects
                .Where(p => p.GroupId == groupId)
                .Include(p => p.Creator)
                .Include(p => p.Tasks)
                .ToList();
        }

        public List<PlannedTask> GetGroupTasks(int groupId)
        {
            return _ctx.PlannedTask
                .Where(t => (t.Project != null && t.Project.GroupId == groupId) || t.GroupId == groupId)
                .Include(t => t.Project)
                .Include(t => t.AssignedTo)
                .ToList();
        }

        public UserGroup Invite(UserGroup userGroup)
        {
            _ctx.UserGroup.Add(userGroup);
            _ctx.SaveChanges();
            return userGroup;
        }

        public UserGroup UpdateMembership(UserGroup userGroup)
        {
            _ctx.UserGroup.Update(userGroup);
            _ctx.SaveChanges();
            return userGroup;
        }
    }
}
