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
using Nfield.ApiTool.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Nfield.ApiTool.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SurveysListPage : ContentPage
	{
	    public SurveysViewModel Surveys { get; set; }
	    public FavToggleDb _db = new FavToggleDb();
        private string ServerUrl { get; set; }
        private AccessToken Token { get; set; }
        public SurveysListPage (AccessToken token, string serverUrl)
		{
			InitializeComponent ();
		    Token = token;
		    ServerUrl = serverUrl;
            Task.Run(async () => 
            {
                await GetSurveys();
            });
        }

        private async Task GetSurveys()
        {
            try
            {
                Surveys = new SurveysViewModel(Token, ServerUrl);
                BindingContext = Surveys;
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

            var surveyDetails = (SurveyDetails)e.Item;
            
            await Navigation.PushAsync(new ActionsPage(Token, ServerUrl, surveyDetails));
        }

        private void SearchBar_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            SurveyList.ItemsSource = string.IsNullOrWhiteSpace(e.NewTextValue)
                ? SurveyList.ItemsSource = Surveys.Surveys
                : SurveyList.ItemsSource = Surveys.Surveys.Where(n => n.SurveyName.ToLower().Contains(e.NewTextValue.ToLower()));
        }

        private async Task Surveys_Refresh(object sender, EventArgs e)
        {
            await GetSurveys();
            SurveyList.ItemsSource = Surveys.Surveys;
            SurveyList.EndRefresh();
        }

        public void Select_As_Favourite(object sender, EventArgs e)
        {
            var fav = (Image)sender;

            var favSurvey = Surveys.Surveys.FirstOrDefault(s => s.SurveyId == fav.ClassId);

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

            var isSurveyAdded = _db.GetFavourites().FirstOrDefault(x => x.SurveId == fav.ClassId);

            if (isSurveyAdded == null || string.IsNullOrEmpty(isSurveyAdded.SurveId))
            {
                _db.AddFav(fav.ClassId, favSurvey.IsFavourite, favSurvey.Image);
            }
            else
            {
                _db.UpdateFav(fav.ClassId, favSurvey.IsFavourite, favSurvey.Image);
            }

            Surveys.Favourites();
            SurveyList.ItemsSource = Surveys.Surveys;
        }        
    }
}