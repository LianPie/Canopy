namespace Canopy.Models
{
    public class AttachmentDto
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? MimeType { get; set; }
        public string? Size { get; set; }
    }

    public class MessageDto
    {
        public int Id { get; set; }
        public int ChatId { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? ImageUrl { get; set; }
        public string? Text { get; set; }
        public string Type { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }
        public List<AttachmentDto> Attachments { get; set; } = new();

        public static MessageDto FromEntity(Message m) => new()
        {
            Id = m.Id,
            ChatId = m.ChatId,
            UserId = m.UserId,
            UserName = m.User?.UserName,
            ImageUrl = m.User?.ImageUrl,
            Text = m.Text,
            Type = m.Type,
            DateCreated = m.DateCreated,
            Attachments = m.MessageAttachments.Select(a => new AttachmentDto
            {
                Id = a.Id,
                Url = "/uploads/chat/" + a.FilePath,
                Name = a.Name,
                MimeType = a.MimeType,
                Size = a.Size
            }).ToList()
        };
    }
    public class SendMessageRequest
    {
        public string Text { get; set; } = string.Empty;
    }
}
