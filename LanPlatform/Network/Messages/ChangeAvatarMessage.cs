using LanPlatform.Accounts;

namespace LanPlatform.Network.Messages
{
    public class ChangeAvatarMessage : NetMessage
    {
        public long User { get; set; }

        public override string GetMessageType()
        {
            return "ChangeAvatar";
        }

        public ChangeAvatarMessage(UserAccount account)
        {
            User = account.Id;
        }
    }
}