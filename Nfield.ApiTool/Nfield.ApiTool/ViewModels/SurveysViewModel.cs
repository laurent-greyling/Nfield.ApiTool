using Newtonsoft.Json;
using Nfield.ApiTool.Helper;
using Nfield.ApiTool.Models;
using Nfield.ApiTool.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Nfield.ApiTool.ViewModels
{
    public class SurveysViewModel : INotifyPropertyChanged
    {

        public FavToggleDb _db = new FavToggleDb();
        private ObservableCollection<SurveyDetails> _surveys;
        public ObservableCollection<SurveyDetails> Surveys
        {
            get { return _surveys; }
            set {
                if (_surveys != value)
                {
                    _surveys = value;
                    OnPropertyChanged("Surveys");
                }
            }
        }

        public SurveysViewModel(AccessToken token, string serverUrl)
        {
            Task.Run(async ()=> await RetrieveSurveysAsync(token, serverUrl));
        }

        private async Task RetrieveSurveysAsync(AccessToken token, string serverUrl)
        {
            var url = $"{serverUrl}/v1/Surveys";
            var request = new RestApi().Get(url, token);

            using (WebResponse response = await request.GetResponseAsync())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    var content = reader.ReadToEnd();
                    Surveys = JsonConvert.DeserializeObject<ObservableCollection<SurveyDetails>>(content);
                }
            }

            foreach (var item in Surveys.ToList())
            {
                item.Icon = "ic_android_black_24dp.png";

                if (item.SurveyType == "OnlineBasic")
                {
                    item.Icon = "ic_online_24dp.png";
                }
            }

            Favourites();
        }

        public void Favourites()
        {
            foreach (var survey in Surveys)
            {
                var localSurvey = _db.GetFavourites().FirstOrDefault(x => x.SurveId == survey.SurveyId);

                if (localSurvey == null || string.IsNullOrEmpty(localSurvey.SurveId) || !localSurvey.IsFavourite)
                {
                    survey.IsFavourite = false;
                    survey.Image = "unselect.png";
                }
                else
                {
                    survey.IsFavourite = true;
                    survey.Image = "select.png";
                }
            }

            var ordered = from survey in Surveys
                          orderby !survey.IsFavourite
                          select new SurveyDetails()
                          {
                              SurveyId = survey.SurveyId,
                              SurveyName = survey.SurveyName,
                              ClientName = survey.ClientName,
                              SurveyType = survey.SurveyType,
                              Description = survey.Description,
                              QuestionnaireMD5 = survey.QuestionnaireMD5,
                              InterviewerInstruction = survey.InterviewerInstruction,
                              Icon = survey.Icon,
                              Image = survey.Image,
                              IsFavourite = survey.IsFavourite,
                              SuccessFulCount = survey.SuccessFulCount
                          };

            Surveys = new ObservableCollection<SurveyDetails>(ordered);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
