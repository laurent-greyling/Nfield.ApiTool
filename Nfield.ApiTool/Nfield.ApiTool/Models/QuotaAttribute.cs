using System.Collections.Generic;

namespace Nfield.ApiTool.Models
{
    public class QuotaAttribute
    {
        public string Name { get; set; }
        public string OdinVariable { get; set; }
        public bool IsSelectionOptional { get; set; }
        public List<QuotaLevel> Levels { get; set; }
    }
}
