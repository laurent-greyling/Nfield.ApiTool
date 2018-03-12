using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nfield.ApiTool.Helper;
using Nfield.ApiTool.Models;
using Plugin.FilePicker.Abstractions;

namespace Nfield.ApiTool.ViewModels
{
    public class SurveyScriptFragmentViewModel : INotifyPropertyChanged
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

        private string ServerUrl { get; set; }
        private string SurveyId { get; set; }
        public List<ScriptFragmentsModel> ScriptFragments { get; set; }

        public SurveyScriptFragmentViewModel(AccessToken token,
            string serverUrl,
            SurveyDetails surveyDetails,
            FileData file,
            bool isLoading = false,
            string loading = "")
        {
            Loading = loading;
            IsLoading = isLoading;
            ServerUrl = serverUrl;
            SurveyId = surveyDetails.SurveyId;
            SurveyName = $"{surveyDetails.SurveyName} Script Fragments";
            if (file != null)
            {
                Task.Run(async () =>
                {
                    await UploadScriptFragment(token, file);
                });
            }
            else
            {
                var fragUrl = $"{ServerUrl}/v1/Surveys/{SurveyId}/ScriptFragments";
                Task.Run(async () => ScriptFragments = await GetFragmentsAsync(fragUrl, token));
            }
        }

        private async Task UploadScriptFragment(AccessToken token, FileData file)
        {
            using (var memoryStream = new MemoryStream(file.DataArray))
            {
                using (var reader = new StreamReader(memoryStream))
                {
                    try
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        File.SetAttributes(file.FilePath, FileAttributes.Normal);
                        var url = $"{ServerUrl}/v1/Surveys/{SurveyId}/ScriptFragments/{fileName}";
                        await PostFragment(url, token, file.DataArray);

                        var fragUrl = $"{ServerUrl}/v1/Surveys/{SurveyId}/ScriptFragments";
                        ScriptFragments = await GetFragmentsAsync(fragUrl, token);
                    }
                    catch (Exception e)
                    {
                        IsLoading = false;
                        throw;
                    }
                }
            }
        }

        public async Task PostFragment(string url, AccessToken token, byte[] contentBytes)
        {
            var request = new RestApi().PostStream(url, token);

            using (var stream = await request.GetRequestStreamAsync())
            {
                await stream.WriteAsync(contentBytes, 0, contentBytes.Length);
            }

            using (var response = await request.GetResponseAsync())
            {
            }
        }

        public async Task<List<ScriptFragmentsModel>> GetFragmentsAsync(string url, AccessToken token)
        {
            var request = new RestApi().Get(url, token);

            using (var response = await request.GetResponseAsync())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var content = reader.ReadToEnd();
                    var fragments = JsonConvert.DeserializeObject<List<string>>(content);

                    var scriptFragments = fragments.Select(x => new ScriptFragmentsModel {FragmentName = x}).ToList();

                    IsLoading = false;
                    return scriptFragments;
                }
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
