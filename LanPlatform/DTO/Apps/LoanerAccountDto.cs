using System;
using System.Collections.Generic;
using LanPlatform.Apps;

namespace LanPlatform.DTO.Apps
{
    public class LoanerAccountDto : EditableGabionDto
    {
        public String Username { get; set; }
        public String Password { get; set; }
        public long CheckoutUser { get; set; }

        public LoanerAccountDto()
        {
            Username = "";
            Password = "";
            CheckoutUser = 0;
        }

        public LoanerAccountDto(LoanerAccount account)
            : base(account)
        {
            Username = account.Username;
            Password = account.Password;
            CheckoutUser = account.CheckoutUser;
        }

        public override string GetClassname()
        {
            return "LoanerAccount";
        }

        public static List<GabionDto> ConvertList(ICollection<LoanerAccount> objects)
        {
            var models = new List<GabionDto>();

            foreach (LoanerAccount target in objects)
            {
                models.Add(new LoanerAccountDto(target));
            }

            return models;
        }
    }
}