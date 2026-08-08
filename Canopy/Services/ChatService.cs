using Canopy.Models;
using Canopy.Repositories;

namespace Canopy.Services
{
    public class ChatService : IChatService
    {
        private readonly IMessagesRepository _messagesRepo;
        private readonly IChatsRepository _chatsRepo;
        private readonly IGroupsRepository _groupsRepo;
        private readonly INotificationService _notificationService;
        private readonly IMessageEncryptionService _encryptionService;

        public ChatService(
            IMessagesRepository messagesRepo,
            IChatsRepository chatsRepo,
            IGroupsRepository groupsRepo,
            INotificationService notificationService,
            IMessageEncryptionService encryptionService)
        {
            _messagesRepo = messagesRepo;
            _chatsRepo = chatsRepo;
            _groupsRepo = groupsRepo;
            _notificationService = notificationService;
            _encryptionService = encryptionService;
        }

        public async Task<List<int>> GetUserChatIdsAsync(int userId)
        {
            return await _chatsRepo.GetChatIdsForUserAsync(userId);
        }

        public async Task<bool> IsUserMemberOfChatAsync(int chatId, int userId)
        {
            var chat = await _chatsRepo.GetByIdAsync(chatId);
            if (chat is null || !chat.IsActive) return false;

            var membership = _groupsRepo.GetMembership(chat.GroupId, userId);
            return membership is not null;
        }

        public async Task<MessageDto> SendMessageAsync(int chatId, int userId, string text)
        {
            if (!await IsUserMemberOfChatAsync(chatId, userId))
                throw new UnauthorizedAccessException("User is not a member of this chat.");

            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Message text cannot be empty.");

            var message = new Message
            {
                ChatId = chatId,
                UserId = userId,
                Text = _encryptionService.Encrypt(text),
                Type = "text",
                DateCreated = DateTime.Now
            };

            await _messagesRepo.CreateAsync(message);
            var dto = MessageDto.FromEntity(message);
            dto.Text = text;

            var chat = await _chatsRepo.GetByIdAsync(chatId);
            var memberIds = _groupsRepo.GetMembers(chat!.GroupId, userId);
            var recipients = memberIds.Where(x => x.UserId != userId).Select(x => x.UserId).ToList();

            await _notificationService.NotifyNewMessageAsync(chatId, dto, recipients);

            return dto;
        }

        public async Task MarkMessageAsSeenAsync(int messageId, int userId, int chatId)
        {
            await _messagesRepo.MarkAsSeenAsync(messageId, userId);
        }

        public async Task<List<MessageDto>> GetMessagesAsync(int chatId, int skip = 0, int take = 50)
        {
            var messages = await _messagesRepo.GetByChatIdAsync(chatId, skip, take);
            return messages.Select(m =>
            {
                var dto = MessageDto.FromEntity(m);
                dto.Text = _encryptionService.Decrypt(m.Text ?? string.Empty);
                return dto;
            }).ToList();
        }
        public async Task<Chat> GetOrCreateChatForGroupAsync(int groupId)
        {
            var chat = await _chatsRepo.GetByGroupIdAsync(groupId);
            if (chat is not null) return chat;

            var newChat = new Chat
            {
                GroupId = groupId,
                DateStarted = DateTime.Now,
                IsActive = true
            };

            return await _chatsRepo.Create(newChat);
        }
    }
}
