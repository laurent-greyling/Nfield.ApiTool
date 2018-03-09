namespace Nfield.ApiTool.Models
{
    public class InterviewerModel
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string TelephoneNumber { get; set; }
        public string Password { get; set; }
        public string ClientInterviewerId { get; set; }
        public bool IsSupervisor { get; set; }
    }
}
