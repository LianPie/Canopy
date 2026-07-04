using Canopy.Models;

namespace Canopy.Repositories
{
    public interface IGroupsRepository
    {
        // Groups
        List<Group> GetAllByUser(int userId);
        Group? GetById(int id, int userId);
        Group Create(Group group);
        Group Update(Group group);
        void Delete(Group group);

        // Membership
        UserGroup? GetMembership(int groupId, int userId);
        UserGroup? GetMembershipById(int userGroupId, int userId);
        List<UserGroup> GetPendingInvites(int userId);
        List<UserGroup> GetSentInvites(int userId);
        List<Project> GetGroupProjects(int groupId);
        List<PlannedTask> GetGroupTasks(int groupId);
        UserGroup Invite(UserGroup userGroup);
        UserGroup UpdateMembership(UserGroup userGroup);
    }
}
