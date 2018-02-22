using System;
using System.Collections.Generic;
using System.Text;

namespace Nfield.ApiTool.Models
{
    public class ActionsToTake
    {
        public string Action { get; set; }
        public string Icon { get; set; }

        public string ServerUrl { get; set; }

        public SurveyDetails SurveyDetails { get; set; }

        public AccessToken AccessToken { get; set; }
    }
}
