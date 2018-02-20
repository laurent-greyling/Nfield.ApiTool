using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nfield.ApiTool.Helper;
using Nfield.ApiTool.Models;
using Nfield.ApiTool.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Nfield.ApiTool.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SurveysListPage : ContentPage
	{
	    public ObservableCollection<SurveyDetails> Surveys { get; set; }
	    public FavToggleDb _db = new FavToggleDb();
        private string ServerUrl { get; set; }
        private AccessToken Token { get; set; }
        public SurveysListPage (AccessToken token, string serverUrl)
		{
			InitializeComponent ();
		    Token = token;
		    ServerUrl = serverUrl;
		    GetSurveys().Wait();
		    Favourites();

		    BindingContext = this;
        }

        private async Task GetSurveys()
        {
            try
            {
                var url = $"{ServerUrl}/v1/Surveys";
                var request = new RestApi().Get(url, Token);

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
            }
            catch (Exception)
            {
                await DisplayAlert("No Surveys", "Could not retrieve survey information", "OK");
            }
        }

        private async Task Show_Options(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            var surveyDetails = e.Item as SurveyDetails;

            var action = await DisplayActionSheet($"{surveyDetails.SurveyName}", "Cancel", null, "Survey Preview", "Survey Statistics");

            switch (action)
            {
                case "Survey Statistics":
                    await Navigation.PushAsync(new ActionsPage(Token, ServerUrl, surveyDetails));
                    break;
                case "Survey Preview":
                    await Navigation.PushAsync(new ActionsPage(Token, ServerUrl, surveyDetails));
                    break;
                default:
                    break;
            }
        }

        private void SearchBar_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            SurveyList.ItemsSource = string.IsNullOrWhiteSpace(e.NewTextValue)
                ? SurveyList.ItemsSource = Surveys
                : SurveyList.ItemsSource = Surveys.Where(n => n.SurveyName.Contains(e.NewTextValue));
        }

        private async Task Surveys_Refresh(object sender, EventArgs e)
        {
            await GetSurveys();
            SurveyList.ItemsSource = Surveys;
            SurveyList.EndRefresh();
        }

        public void Select_As_Favourite(object sender, EventArgs e)
        {
            var fav = sender as Button;

            var favSurvey = Surveys.FirstOrDefault(s => s.SurveyId == fav.Text);

            if (favSurvey.Image == "unselect.png")
            {
                favSurvey.Image = "selected.png";
                favSurvey.IsFavourite = true;
            }
            else
            {
                favSurvey.Image = "unselect.png";
                favSurvey.IsFavourite = false;
            }

            var isSurveyAdded = _db.GetFavourites().FirstOrDefault(x => x.SurveId == fav.Text);

            if (isSurveyAdded == null || string.IsNullOrEmpty(isSurveyAdded.SurveId))
            {
                _db.AddFav(fav.Text, favSurvey.IsFavourite, favSurvey.Image);
            }
            else
            {
                _db.UpdateFav(fav.Text, favSurvey.IsFavourite, favSurvey.Image);
            }

            Favourites();
            SurveyList.ItemsSource = Surveys;
        }

        private void Favourites()
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
    }
}