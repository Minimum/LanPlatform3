using System;
using LanPlatform.Database;

namespace LanPlatform.Accounts
{
    public class AccountEditField : DatabaseObject
    {
        public long Action { get; set; }
        public String FieldName { get; set; }
        public String OldValue { get; set; }
        public String NewValue { get; set; }

        public AccountEditField()
        {
            Action = 0;
            FieldName = "";
            OldValue = "";
            NewValue = "";
        }

        public AccountEditField(String fieldName, object oldValue, object newValue)
        {
            FieldName = fieldName;
            OldValue = oldValue.ToString();
            NewValue = newValue.ToString();
        }

        public AccountEditField(String fieldName, String oldValue, String newValue)
        {
            FieldName = fieldName;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}