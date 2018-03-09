using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nfield.ApiTool.Interfaces;
using Nfield.ApiTool.Models;
using Nfield.ApiTool.Services;
using Nfield.ApiTool.ViewModels;
using Plugin.FilePicker;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Nfield.ApiTool.Views.Interviewers
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class InterviewersPage : ContentPage
	{
	    private AccessToken Token { get; set; }
	    private string ServerUrl { get; set; }
	    private SurveyDetails SurveyDetails { get; set; }
        public InterviewersPage (AccessToken token, string serverUrl, SurveyDetails surveyDetails)
		{
			InitializeComponent ();
		    Token = token;
		    ServerUrl = serverUrl;
		    SurveyDetails = surveyDetails;
		    var interviewerData = new InterviewersViewModel(Token, ServerUrl, SurveyDetails, null);
		    BindingContext = interviewerData;
        }

	    public async Task Upload_File()
	    {
	        try
	        {
	            var fileData = await CrossFilePicker.Current.PickFile();

	            var interviewerData = new InterviewersViewModel(Token, ServerUrl, SurveyDetails, fileData, true, "Uploading...");

	            BindingContext = interviewerData;
	        }
	        catch (Exception e)
	        {
	            await DisplayAlert("Error", $"Something went wrong uploading interviewers {e.Message}", "Ok");
	            Console.WriteLine(e);
	            throw;
	        }
	    }

	    public async Task Download_File()
	    {
	        try
	        {
	            var downloadInterviewerData = await new DownloadInterviewers().Download(Token, ServerUrl, SurveyDetails, null);

	            await DependencyService.Get<ISave>().SaveAsync($"Interviewers-{SurveyDetails.SurveyName}.csv", downloadInterviewerData);
	        }
	        catch (Exception e)
	        {
	            await DisplayAlert("Error", $"Something went wrong downloading interviewers data {e.Message}", "Ok");
	            Console.WriteLine(e);
	            throw;
	        }
	    }
    }
}