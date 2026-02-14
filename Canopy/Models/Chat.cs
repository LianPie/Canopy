namespace Canopy.Models
{
    public class Chat
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public Group Group { get; set; } = null!;
        public DateTime DateStarted { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
    public class Message
    {
        public int Id { get; set; }
        public int ChatId { get; set; }
        public Chat Chat { get; set; } = null!;
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public string? Text { get; set; }
        public string Type { get; set; } = string.Empty;
        //text, audio, video, image, document
        public DateTime DateCreated { get; set; } = DateTime.Now;

        public ICollection<MessageAttachment> MessageAttachments { get; set; } = new List<MessageAttachment>();
        public ICollection<MessageSeenStatus> SeenStatuses { get; set; } = new List<MessageSeenStatus>();
    }
    public class MessageAttachment
    {
        public int Id { get; set; }
        public int MessageId { get; set; }
        public Message Message { get; set; } = null!;
        public string FilePath { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Size { get; set; }
        public string? MimeType { get; set; }
        public DateTime DateUploaded { get; set; } = DateTime.Now;
    }
    public class MessageSeenStatus
    {
        public int MessageId { get; set; }
        public Message Message { get; set; } = null!;
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public DateTime SeenDate { get; set; } = DateTime.Now;
    }
}
