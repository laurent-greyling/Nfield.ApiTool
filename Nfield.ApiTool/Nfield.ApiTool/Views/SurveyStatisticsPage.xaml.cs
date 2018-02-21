using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Nfield.ApiTool.Models;
using Nfield.ApiTool.Helper;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Nfield.ApiTool.ViewModels;

namespace Nfield.ApiTool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SurveyStatisticsPage : TabbedPage
    {
        public SurveyStatisticsViewModel SurveyCounts { get; set; }
        private AccessToken Token { get; set; }
        private string ServerUrl { get; set; }
        private string SurveyId { get; set; }
        private SurveyDetails SurveyDetails { get; set; }

        public SurveyStatisticsPage (AccessToken token, string serverUrl, SurveyDetails surveyDetails)
        {
            InitializeComponent();
            Token = token;
            ServerUrl = serverUrl;
            SurveyId = surveyDetails.SurveyId;
            SurveyDetails = surveyDetails;
            Task.Run(async () =>
            {
                await GetCounts();
            });            
        }
        
        private async Task GetCounts()
        {
            try
            {
                SurveyCounts = new SurveyStatisticsViewModel(Token, ServerUrl, SurveyId, SurveyDetails);
                if (SurveyCounts.HasQuota)
                {
                    NoQuota.IsVisible = false;
                }
                BindingContext = SurveyCounts;
            }
            catch (Exception)
            {
                await DisplayAlert("Oeps", $"No data for {SurveyCounts.SurveyInfo[0].SurveyName}", "Ok");
            }
        }       

        private async Task Stats_Refresh(object sender, EventArgs e)
        {
            await GetCounts();
            SurveyStats.EndRefresh();
        }

        public void Handle_QuotaItemTapped(object sender, SelectedItemChangedEventArgs e)
        {
            var selected = ((ListView)sender).SelectedItem as QuotaLevel;

            DisplayAlert($"Extra Info {selected.Name}", $@"Dropped Out: {selected.DroppedOutCount}
Screened Out: {selected.UnsuccessfulCount}
Rejeceted: {selected.RejectedCount}", "Ok");

            ((ListView)sender).SelectedItem = null;
        }

        public void Handle_ItemTapped(object sender, SelectedItemChangedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }

        private void Tapped_Search()
        {
            if (!SearchBarQuota.IsVisible)
            {
                CurrentPage = QuotaTab;
                SearchBarQuota.IsVisible = true;
                SearchBarQuota.Focus();
            }
            else
            {
                SearchBarQuota.IsVisible = false;
            }
        }

        private void SearchBar_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            QuotaList.ItemsSource = string.IsNullOrWhiteSpace(e.NewTextValue)
                ? QuotaList.ItemsSource = SurveyCounts.QuotaGroup
                : QuotaList.ItemsSource = SurveyCounts.QuotaGroup.Where(x =>
                x.Any(n =>
                n.Name.ToLowerInvariant().Contains(e.NewTextValue.ToLowerInvariant()))
                || x.LevelName.ToLowerInvariant().Contains(e.NewTextValue.ToLowerInvariant()));
        }

        private async Task Quota_Refresh(object sender, EventArgs e)
        {
            await GetCounts();
            QuotaList.EndRefresh();
        }
    }
}