using LanPlatform.Apps;

namespace LanPlatform.Network.Messages
{
    public class LoanerCheckoutMessage : NetMessage
    {
        public long Loaner { get; set; }
        public long CheckoutUser { get; set; }

        public LoanerCheckoutMessage(LoanerAccount account)
            : base()
        {
            Loaner = account.Id;
            CheckoutUser = account.CheckoutUser;
        }

        public override string GetMessageType()
        {
            return "LoanerCheckout";
        }
    }
}