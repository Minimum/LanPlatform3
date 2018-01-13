using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.Accounts;
using LanPlatform.Apps;
using LanPlatform.DTO.Accounts;
using LanPlatform.DTO.Apps;
using Newtonsoft.Json;

namespace LanPlatform.Models.Responses
{
    public class SiteInit
    {
        [JsonIgnore]
        protected AppInstance Instance;

        // Local Account
        public UserAccountDto LocalUserAccount { get; set; }
        public List<UserPermissionDto> LocalPermissions { get; set; }

        // Accounts
        public List<UserAccountDto> ActiveUsers { get; set; } 

        // Loaners
        public List<LoanerAccountDto> LoanerAccounts { get; set; }

        // Apps
        public List<AppDto> Apps { get; set; }

        public SiteInit(AppInstance instance)
        {
            Instance = instance;
        }

        public void LoadData()
        {
            // Local Account
            LocalUserAccount = Instance.LocalAccount != null ? new UserAccountDto(Instance.LocalAccount) : null;
            LocalPermissions = LocalUserAccount != null ? UserPermissionDto.ConvertList(Instance.Accounts.GetAccountFlags(Instance.LocalAccount)) : new List<UserPermissionDto>();

            //ActiveUsers = UserAccountDto.ConvertList(Instance.Accounts.GetActiveAccounts(EngineUtil.CurrentTime - 1800, 100, 0));

            AppManager apps = new AppManager(Instance);

            LoanerAccounts = LoanerAccountDto.ConvertList(apps.GetLoanerAccounts());

            Apps = AppDto.ConvertList(apps.GetApps());

            return;
        }
    }
}