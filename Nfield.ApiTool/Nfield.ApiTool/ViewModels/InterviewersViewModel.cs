using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nfield.ApiTool.Helper;
using Nfield.ApiTool.Models;
using Plugin.FilePicker.Abstractions;
using Xamarin.Forms.Internals;

namespace Nfield.ApiTool.ViewModels
{
    public class InterviewersViewModel : INotifyPropertyChanged
    {
        public string SurveyName { get; set; }

        private string _loading;
        public string Loading
        {
            get { return _loading; }
            set
            {
                if (_loading != value)
                {
                    _loading = value;
                    OnPropertyChanged("Loading");
                }
            }
        }

        private bool _isLoading = true;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged("IsLoading");
                }
            }
        }

        private List<InterviewerModel> _interviewers;
        public List<InterviewerModel> Interviewers
        {
            get { return _interviewers; }
            set
            {
                if (_interviewers != value)
                {
                    _interviewers = value;
                    OnPropertyChanged("Interviewers");
                }
            }
        }

        private string ServerUrl { get; set; }
        private string SurveyId { get; set; }

        public InterviewersViewModel(AccessToken token,
            string serverUrl,
            SurveyDetails surveyDetails,
            FileData file,
            bool isLoading = false,
            string loading = "")
        {
            ServerUrl = serverUrl;
            SurveyId = surveyDetails.SurveyId;
            Loading = loading;
            IsLoading = isLoading;
            SurveyName = $"{surveyDetails.SurveyName} Interviewers";

            if (file != null)
            {
                Task.Run(async () =>
                {
                    await UploadInterviewers(token, file);
                });
            }
            else
            {
                var url = $"{ServerUrl}/v1/Interviewers";
                Task.Run(async () =>
                {
                    Interviewers = await GetInterviewersAsync(url, token);
                });
            }
        }

        private async Task UploadInterviewers(AccessToken token, FileData file)
        {
            var url = $"{ServerUrl}/v1/Interviewers";

            using (var memoryStream = new MemoryStream(file.DataArray))
            {
                using (var reader = new StreamReader(memoryStream))
                {
                    var csvFile = await reader.ReadToEndAsync();
                    var columnsLine = new StringReader(csvFile).ReadLine();
                    var delim = new[] { ',', ';' };
                    var columns = columnsLine.Split(delim);
                    var csvData = csvFile.Split('\n').Skip(1);

                    foreach (var row in csvData)
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(row)) continue;

                            var columnData = row.Split(delim);
                            var isSupervisor = false;

                            if (columns.IndexOf("IsSupervisor") != -1)
                            {
                                var supervisorValue = columnData[columns.IndexOf("IsSupervisor")];
                                supervisorValue = supervisorValue.Replace("\r", "");

                                if (supervisorValue.ToLower() == "true" ||
                                    supervisorValue.ToLower() == "yes")
                                {
                                    isSupervisor = true;
                                }
                            }
                            var interviewer = new InterviewerModel
                            {
                                IsSupervisor = isSupervisor,
                                Username = columns.IndexOf("UserName") != -1 ? columnData[columns.IndexOf("UserName")] : "",
                                FirstName = columns.IndexOf("FirstName") != -1 ? columnData[columns.IndexOf("FirstName")] : "",
                                LastName = columns.IndexOf("LastName") != -1 ? columnData[columns.IndexOf("LastName")] : "",
                                EmailAddress = columns.IndexOf("EmailAddress") != -1 ? columnData[columns.IndexOf("EmailAddress")] : "",
                                TelephoneNumber = columns.IndexOf("TelephoneNumber") != -1 ? columnData[columns.IndexOf("TelephoneNumber")] : "",
                                Password = columns.IndexOf("Password") != -1 ? columnData[columns.IndexOf("Password")] : "",
                                ClientInterviewerId = columns.IndexOf("InterviewerId") != -1 ? InterviewerIdConvert(columnData[columns.IndexOf("InterviewerId")]) : "",
                            };

                            var interviewerData = JsonConvert.SerializeObject(interviewer);
                            await PostInterviewers(interviewerData, url, token);
                        }
                        catch (Exception e)
                        {
                            IsLoading = false;
                            throw;
                        }
                    }

                    Interviewers = await GetInterviewersAsync(url, token);
                }
            }
        }

        private async Task PostInterviewers(string data, string url, AccessToken token)
        {
            var request = new RestApi().Post(url, token);

            using (var writer = new StreamWriter(await request.GetRequestStreamAsync()))
            {
                writer.Write(data);
                writer.Flush();
            }

            using (var response = await request.GetResponseAsync())
            {
            }
        }

        public async Task<List<InterviewerModel>> GetInterviewersAsync(string url, AccessToken token)
        {
            var request = new RestApi().Get(url, token);

            using (var response = await request.GetResponseAsync())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var content = reader.ReadToEnd();
                    var interviewrs = JsonConvert.DeserializeObject<List<InterviewerModel>>(content);

                    IsLoading = false;
                    return interviewrs;
                }
            }

        }

        private string InterviewerIdConvert(string interviewerId)
        {
            do
            {
                interviewerId = interviewerId.Insert(0, "0");
            } while (interviewerId.Length != 8);

            return interviewerId;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
