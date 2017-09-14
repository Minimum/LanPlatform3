using System;

namespace LanPlatform.Models.Requests
{
    public class EditAccountRequest
    {
        public long Id { get; set; }

        public String FieldName { get; set; }
        public String Data { get; set; }

        public EditAccountRequest()
        {
            Id = -1;

            FieldName = "";
            Data = "";
        }
    }
}