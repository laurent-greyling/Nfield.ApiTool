
using System.Collections.Generic;

namespace Nfield.ApiTool.Models
{
    public class QuotaLevel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int? Target { get; set; }
        public int? GrossTarget { get; set; }
        public int? SuccessfulCount { get; set; }
        public bool QuotaCounted { get; set; }
        public int? UnsuccessfulCount { get; set; }
        public int? DroppedOutCount { get; set; }
        public int? RejectedCount { get; set; }
        public int? Total { get; set; }
        public int? Assigned { get; set; }
        public int? TotalSuccessful { get; set; }
        public List<QuotaAttribute> Attributes { get; set; }
    }
}
