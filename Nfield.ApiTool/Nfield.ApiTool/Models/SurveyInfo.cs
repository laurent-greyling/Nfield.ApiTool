namespace Nfield.ApiTool.Models
{
    public class SurveyInfo
    {
        public string SurveyName { get; set; }
        public string Success { get; set; }
        public string ActiveLive { get; set; }
        public string ActiveTest { get; set; }
        public string Total { get; set; }
        public string PercOfTarget { get; set; }
        public string PercSuccess { get; set; }
        public string PercDrop { get; set; }
        public string PercScreen { get; set; }
        public string PercReject { get; set; }
        public string PercTotal { get; set; }
        public string Target { get; set; }
        public bool TargetVisible { get; set; } = false;

        public bool HasNoQuota { get; set; } = true;

        public SurveyCountsModel SurveyCounts { get; set; }
    }
}
