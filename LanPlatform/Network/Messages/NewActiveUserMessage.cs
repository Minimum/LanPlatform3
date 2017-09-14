namespace LanPlatform.Network.Messages
{
    public class NewActiveUserMessage : NetMessage
    {
        public long UserId { get; set; }

        public NewActiveUserMessage(long userId)
        {
            UserId = userId;
        }

        public override string GetMessageType()
        {
            return "NewActiveUser";
        }
    }
}