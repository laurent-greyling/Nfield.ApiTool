using Nfield.ApiTool.Models;
using System;
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
		public SamplingPointsPage (AccessToken token, string serverUrl, SurveyDetails surveyDetails)
		{
			InitializeComponent ();
            Token = token;
            ServerUrl = serverUrl;
            SurveyDetails = surveyDetails;
            var sampleData = new SamplingPointsViewModel(Token, ServerUrl, SurveyDetails, null);
            BindingContext = sampleData;
        }

	    public async Task Upload_File()
	    {
            try
	        {
                var fileData = await CrossFilePicker.Current.PickFile();
                
                var sampleData = new SamplingPointsViewModel(Token, ServerUrl, SurveyDetails, fileData, true, "Uploading...");

                BindingContext = sampleData;
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
                var downloadSampleData = await new DownloadSamplingPoints().Download(Token, ServerUrl, SurveyDetails, null, true, "Downloading...");

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
    }
}