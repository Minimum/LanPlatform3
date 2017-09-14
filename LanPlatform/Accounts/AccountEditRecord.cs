using System;
using System.Collections.Generic;
using LanPlatform.Database;

namespace LanPlatform.Accounts
{
    public class AccountEditRecord : DatabaseObject
    {
        public long Account { get; set; }
        public long Editor { get; set; }
        public long Version { get; set; }
        public long Time { get; set; }

        public String Description { get; set; }

        public List<AccountEditField> Fields { get; set; }

        public AccountEditRecord()
        {
            Account = 0;
            Editor = 0;
            Version = 0;

            Description = "";

            Fields = new List<AccountEditField>();
        }

        public void AddField(String fieldName, object oldValue, object newValue)
        {
            AddField(fieldName, oldValue.ToString(), newValue.ToString());

            return;
        }

        public void AddField(String fieldName, String oldValue, String newValue)
        {
            AccountEditField field = new AccountEditField();

            field.FieldName = fieldName;
            field.OldValue = oldValue;
            field.NewValue = newValue;

            Fields.Add(field);

            return;
        }
    }
}