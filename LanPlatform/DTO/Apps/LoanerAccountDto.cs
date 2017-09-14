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

        public List<AppDto> Apps { get; set; }

        public LoanerAccountDto()
        {
            Username = "";
            Password = "";
            CheckoutUser = 0;

            Apps = new List<AppDto>();
        }

        public LoanerAccountDto(LoanerAccount account)
            : base(account)
        {
            Username = account.Username;
            Password = account.Password;
            CheckoutUser = account.CheckoutUser;

            Apps = AppDto.ConvertList(account.Apps);
        }

        public static List<LoanerAccountDto> ConvertList(ICollection<LoanerAccount> objects)
        {
            List<LoanerAccountDto> models = new List<LoanerAccountDto>();

            foreach (LoanerAccount target in objects)
            {
                models.Add(new LoanerAccountDto(target));
            }

            return models;
        }
    }
}