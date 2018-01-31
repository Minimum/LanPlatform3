using System;
using System.Collections.Generic;
using LanPlatform.Accounts;

namespace LanPlatform.DTO.Accounts
{
    public class UserAccountDto : EditableGabionDto
    {
        // Access
        public bool Root { get; set; }
        public UserAccountType AccountType { get; set; }     // Advanced Edit

        // Information Variables
        public Gender Gender { get; set; }                  // Basic Edit
        public String FirstName { get; set; }               // Basic Edit
        public String LastName { get; set; }                // Basic Edit
        public long Birthday { get; set; }                  // Basic Edit
        public String ContactEmail { get; set; }            // Basic Edit
        public String ContactPhone { get; set; }            // Basic Edit
        public String ContactFacebook { get; set; }         // Basic Edit
        public long ContactSteam { get; set; }              // Basic Edit

        // Player Variables
        public long TotalEvents { get; set; }               // Advanced Edit
        public long EventOffset { get; set; }               // Advanced Edit
        public long RemoteEvents { get; set; }              // Advanced Edit
        public long LastEvent { get; set; }                 // Advanced Edit
        public String DisplayName { get; set; }             // Basic Edit
        public String CustomUrl { get; set; }               
        public long LastActive { get; set; }
        public long Avatar { get; set; }                    // Basic Edit
        public AccountVisibility Visibility { get; set; }   // Basic Edit

        // Award Variables
        public bool AwardsEnabled { get; set; }
        public bool AwardsXpEnabled { get; set; }
        public long AwardsLevel { get; set; }
        public long AwardsXp { get; set; }

        public UserAccountDto()
        {
            Root = false;
            AccountType = UserAccountType.Player;

            Gender = Gender.None;
            FirstName = "";
            LastName = "";
            Birthday = 0;
            ContactEmail = "";
            ContactPhone = "";
            ContactFacebook = "";
            ContactSteam = 0;

            TotalEvents = 0;
            EventOffset = 0;
            RemoteEvents = 0;
            LastEvent = 0;
            DisplayName = "";
            CustomUrl = "";
            LastActive = 0;
            Avatar = 0;
            Visibility = AccountVisibility.Visible;

            AwardsEnabled = true;
            AwardsXpEnabled = true;
            AwardsLevel = 0;
            AwardsXp = 0;
        }

        public UserAccountDto(UserAccount account)
            : base(account)
        {
            Root = account.Root;
            AccountType = account.AccountType;

            Gender = account.Gender;
            Birthday = account.Birthday;
            ContactEmail = account.ContactEmail;
            ContactPhone = account.ContactPhone;
            ContactFacebook = account.ContactFacebook;
            ContactSteam = account.ContactSteam;

            TotalEvents = account.TotalEvents;
            EventOffset = account.EventOffset;
            RemoteEvents = account.RemoteEvents;
            FirstName = account.FirstName;
            LastName = account.LastName;
            DisplayName = account.DisplayName;
            CustomUrl = account.CustomUrl;
            LastEvent = account.LastEvent;
            LastActive = account.LastActive;
            Avatar = account.Avatar;
            Visibility = account.Visibility;

            AwardsEnabled = account.AwardsEnabled;
            AwardsXpEnabled = account.AwardsXpEnabled;
            AwardsLevel = account.AwardsLevel;
            AwardsXp = account.AwardsXp;
        }

        public override string GetClassname()
        {
            return "UserAccount";
        }

        public static List<GabionDto> ConvertList(ICollection<UserAccount> accounts)
        {
            var models = new List<GabionDto>();

            foreach (UserAccount account in accounts)
            {
                models.Add(new UserAccountDto(account));
            }

            return models;
        }
    }
}