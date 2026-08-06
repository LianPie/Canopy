namespace Canopy.Models
{
    public class MessageDto
    {
        public int Id { get; set; }
        public int ChatId { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? Text { get; set; }
        public string Type { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }

        public static MessageDto FromEntity(Message m) => new()
        {
            Id = m.Id,
            ChatId = m.ChatId,
            UserId = m.UserId,
            UserName = m.User?.UserName,
            Text = m.Text,
            Type = m.Type,
            DateCreated = m.DateCreated
        };


    }
    public class SendMessageRequest
    {
        public string Text { get; set; } = string.Empty;
    }
}
