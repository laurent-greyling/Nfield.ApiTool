using Nfield.ApiTool.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nfield.ApiTool.Services;
using Plugin.FilePicker;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using Nfield.ApiTool.ViewModels;
using Plugin.FilePicker.Abstractions;

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
	            var d = Android.OS.Environment.DirectoryDownloads;


                var path = await CrossFilePicker.Current.PickFile();
                var downloadSampleData = await new DownloadSamplingPoints().Download(Token, ServerUrl, SurveyDetails, null, true, "Downloading...");
	            var fileData = new FileData
	            {
	                FileName = $"SamplingPoints-{SurveyDetails.SurveyName}.csv",
	                DataArray = Encoding.UTF8.GetBytes(downloadSampleData),
                    FilePath = path.FilePath
                };
	            var saveFile = await CrossFilePicker.Current.SaveFile(fileData);

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