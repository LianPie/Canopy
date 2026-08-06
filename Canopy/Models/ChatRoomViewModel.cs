namespace Canopy.Models
{
    public class ChatRoomViewModel
    {
        public int ChatId { get; set; }
        public int GroupId { get; set; }
        public int CurrentUserId { get; set; }
        public List<MessageDto> Messages { get; set; } = new();
    }
}
