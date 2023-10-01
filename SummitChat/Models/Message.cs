namespace SummitChat.Models
{
    public class Message

    {
        public int ID { get; set; }

        public string MessageID { get; set; }

        public int SenderID { get; set; }

        public string ReceiverID { get; set; }

        public long Timestamp { get; set; }


        public Message()
        {
            Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        }
    }
}
