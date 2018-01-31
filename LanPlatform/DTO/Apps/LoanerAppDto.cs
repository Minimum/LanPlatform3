using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.Apps;

namespace LanPlatform.DTO.Apps
{
    public class LoanerAppDto : EditableGabionDto
    {
        public long Account { get; set; }
        public long App { get; set; }

        public LoanerAppDto()
        {
            Account = 0;
            App = 0;
        }

        public LoanerAppDto(LoanerApp app)
            : base(app)
        {
            Account = app.Account;
            App = app.App;
        }

        public override string GetClassname()
        {
            return "LoanerApp";
        }

        public static List<GabionDto> ConvertList(ICollection<LoanerApp> objects)
        {
            var models = new List<GabionDto>();

            foreach (LoanerApp target in objects)
            {
                models.Add(new LoanerAppDto(target));
            }

            return models;
        }
    }
}