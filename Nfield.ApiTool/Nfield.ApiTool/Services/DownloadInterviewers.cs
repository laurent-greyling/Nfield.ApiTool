using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nfield.ApiTool.Helper;
using Nfield.ApiTool.Models;
using Plugin.FilePicker.Abstractions;

namespace Nfield.ApiTool.Services
{
    public class DownloadInterviewers
    {
        public async Task<string> Download(AccessToken token,
            string serverUrl,
            SurveyDetails surveyDetails,
            FileData file)
        {
            var url = $"{serverUrl}/v1/Interviewers";
            var interviewers = await Retrieve(url, token);

            var interviewerCsv = new StringBuilder();
            var headers =
                $"InterviewerId,UserName,FirstName,LastName,EmailAddress,TelephoneNumber,LastPasswordChangeTime,ClientInterviewerId,SuccessfulCount,UnsuccessfulCount,DroppedOutCount,RejectedCount,LastSyncDate,IsFullSynced,IsLastSyncSuccessful,IsSupervisor{Environment.NewLine}";
            interviewerCsv.Append(headers);

            foreach (var interviewer in interviewers)
            {
                var row = $"{interviewer.InterviewerId},{interviewer.UserName},{interviewer.FirstName},{interviewer.LastName},{interviewer.EmailAddress},{interviewer.TelephoneNumber},{interviewer.LastPasswordChangeTime},{interviewer.ClientInterviewerId},{interviewer.SuccessfulCount},{interviewer.UnsuccessfulCount},{interviewer.DroppedOutCount},{interviewer.RejectedCount},{interviewer.LastSyncDate},{interviewer.IsFullSynced},{interviewer.IsLastSyncSuccessful},{interviewer.IsSupervisor}{Environment.NewLine}";
                interviewerCsv.Append(row);
            }

            return interviewerCsv.ToString();
        }

        public async Task<List<Interviewer>> Retrieve(string url, AccessToken token)
        {
            var request = new RestApi().Get(url, token);

            using (var response = await request.GetResponseAsync())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var content = reader.ReadToEnd();
                    return JsonConvert.DeserializeObject<List<Interviewer>>(content);
                }
            }
        }
    }
}
