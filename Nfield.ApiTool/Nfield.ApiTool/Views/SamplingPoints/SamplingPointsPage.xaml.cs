using Nfield.ApiTool.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Nfield.ApiTool.Interfaces;
using Nfield.ApiTool.Services;
using Plugin.FilePicker;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Nfield.ApiTool.ViewModels;

namespace Nfield.ApiTool.Views.SamplingPoints
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SamplingPointsPage : ContentPage
	{
        private AccessToken Token { get; set; }
        private string ServerUrl { get; set; }

        private SurveyDetails SurveyDetails { get; set; }

	    private SamplingPointsViewModel SampleData { get; set; }

	    public SamplingPointsPage (AccessToken token, string serverUrl, SurveyDetails surveyDetails)
		{
			InitializeComponent ();
            Token = token;
            ServerUrl = serverUrl;
            SurveyDetails = surveyDetails;
		    SampleData = new SamplingPointsViewModel(Token, ServerUrl, SurveyDetails, null);
            BindingContext = SampleData;
        }

	    public async Task Upload_File()
	    {
            try
	        {
                var fileData = await CrossFilePicker.Current.PickFile();

	            SampleData = new SamplingPointsViewModel(Token, ServerUrl, SurveyDetails, fileData, true, "Uploading...");

                BindingContext = SampleData;
            }
	        catch (Exception e)
	        {
	            await DisplayAlert("Error", $"Something went wrong uploading samplingpoint {e.Message}", "Ok");
	            Console.WriteLine(e);
	            throw;
	        }
	    }

	    public async Task Download_File()
	    {
	        try
	        {
                var downloadSampleData = await new DownloadSamplingPoints().Download(Token, ServerUrl, SurveyDetails, null);

	            await DependencyService.Get<ISave>().SaveAsync($"SamplingPoints-{SurveyDetails.SurveyName}.csv", downloadSampleData);
            }
	        catch (Exception e)
	        {
	            await DisplayAlert("Error", $"Something went wrong downloading samplingpoint {e.Message}", "Ok");
	            Console.WriteLine(e);
	            throw;
	        }
	    }

        public async Task Upload_Image()
        {
            try
            {
                var fileData = await CrossFilePicker.Current.PickFile();
                
                var uploadSamplingPointImage = new SamplingPointsImageViewModel(Token, ServerUrl, SurveyDetails, fileData, true, "Uploading Image...");
                
            }
            catch (Exception e)
            {
                await DisplayAlert("Error", $"Something went wrong downloading samplingpoint {e.Message}", "Ok");
                Console.WriteLine(e);
                throw;
            }
        }

	    private void SearchBar_OnTextChanged(object sender, TextChangedEventArgs e)
	    {
	        try
	        {

	            SamplingPointList.ItemsSource = string.IsNullOrWhiteSpace(e.NewTextValue)
	                ? SamplingPointList.ItemsSource = SampleData.SamplingPoints
	                : SamplingPointList.ItemsSource = SampleData.SamplingPoints.Where(n =>
	                {
	                    var samplingPointId = string.IsNullOrEmpty(n.SamplingPointId) ? "" : n.SamplingPointId.ToLower();
	                    var name = string.IsNullOrEmpty(n.Name) ? "" : n.Name.ToLower();
	                    var fieldworkOfficeId = string.IsNullOrEmpty(n.FieldworkOfficeId) ? "" : n.FieldworkOfficeId.ToLower();
	                    var groupId = string.IsNullOrEmpty(n.GroupId) ? "" : n.GroupId.ToLower();

	                    return samplingPointId.Contains(e.NewTextValue.ToLower()) ||
	                           name.Contains(e.NewTextValue.ToLower()) ||
	                           fieldworkOfficeId.Contains(e.NewTextValue.ToLower()) ||
	                           groupId.Contains(e.NewTextValue.ToLower());
	                });

	        }
	        catch (Exception)
	        {
	            SamplingPointList.ItemsSource = null;

	        }
	    }
    }
}