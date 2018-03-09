using System;

namespace Nfield.ApiTool.Models
{
    public class Interviewer
    {
        public string InterviewerId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string TelephoneNumber { get; set; }
        public DateTime? LastPasswordChangeTime { get; set; }
        public string ClientInterviewerId { get; set; }
        public int SuccessfulCount { get; set; }
        public int UnsuccessfulCount { get; set; }
        public int DroppedOutCount { get; set; }
        public int RejectedCount { get; set; }
        public DateTime? LastSyncDate { get; set; }
        public bool IsFullSynced { get; set; }
        public bool IsLastSyncSuccessful { get; set; }
        public bool IsSupervisor { get; set; }

    }
}
