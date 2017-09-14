namespace LanPlatform.Network.Messages
{
    public class NewsStatusChangeMessage : NetMessage
    {
        public long Status { get; set; }

        public override string GetMessageType()
        {
            return "NewsStatusChange";
        }

        public NewsStatusChangeMessage()
        {
            Status = 0;
        }

        public NewsStatusChangeMessage(long status)
        {
            Status = status;
        }
    }
}