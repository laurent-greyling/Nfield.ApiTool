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

namespace Nfield.ApiTool.ViewModels
{
    public class ScriptFragmentUpdateViewModel : INotifyPropertyChanged
    {
        public string SurveyName { get; set; }
        public string FragmentName { get; set; }

        private string _fragment;
        public string Fragment
        {
            get { return _fragment; }
            set
            {
                if (_fragment != value)
                {
                    _fragment = value;
                    OnPropertyChanged("Fragment");
                }
            }
        }
        public ScriptFragmentUpdateViewModel(
            AccessToken token,
            string serverUrl,
            string scriptFragmentName,
            string surveyId,
            string surveyName)
        {
            SurveyName = $"{surveyName} Script Fragments";
            FragmentName = scriptFragmentName;

            var fragUrl = $"{serverUrl}/v1/Surveys/{surveyId}/ScriptFragments/{scriptFragmentName}";
            Task.Run(async () =>
            {
                Fragment = await GetFragmentAsync(fragUrl,token);
            });
        }

        public async Task<string> GetFragmentAsync(string url, AccessToken token)
        {
            var request = new RestApi().GetText(url, token);

            using (var response = await request.GetResponseAsync())
            {
                using (var reader = new StreamReader(response.GetResponseStream(), Encoding.Unicode))
                {
                    var content = reader.ReadToEnd();
                    
                    return content;
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
