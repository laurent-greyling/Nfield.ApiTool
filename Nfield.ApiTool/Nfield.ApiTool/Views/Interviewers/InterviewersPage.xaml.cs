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
	    public InterviewersViewModel Interviewers { get; set; }

	    public InterviewersPage (AccessToken token, string serverUrl, SurveyDetails surveyDetails)
		{
			InitializeComponent ();
		    Token = token;
		    ServerUrl = serverUrl;
		    SurveyDetails = surveyDetails;
		    Interviewers = new InterviewersViewModel(Token, ServerUrl, SurveyDetails, null);
		    BindingContext = Interviewers;
        }

	    public async Task Upload_File()
	    {
	        try
	        {
	            var fileData = await CrossFilePicker.Current.PickFile();

	            Interviewers = new InterviewersViewModel(Token, ServerUrl, SurveyDetails, fileData, true, "Uploading...");

	            BindingContext = Interviewers;
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

	    private void SearchBar_OnTextChanged(object sender, TextChangedEventArgs e)
	    {
	        try
	        {

	            InterviewersList.ItemsSource = string.IsNullOrWhiteSpace(e.NewTextValue)
	                ? InterviewersList.ItemsSource = Interviewers.Interviewers
	                : InterviewersList.ItemsSource = Interviewers.Interviewers.Where(n =>
	                    n.Username.ToLower().Contains(e.NewTextValue.ToLower()) ||
	                    n.FirstName.ToLower().Contains(e.NewTextValue.ToLower()) ||
	                    n.LastName.ToLower().Contains(e.NewTextValue.ToLower()) ||
	                    n.ClientInterviewerId.ToLower().Contains(e.NewTextValue.ToLower()) ||
	                    n.EmailAddress.ToLower().Contains(e.NewTextValue.ToLower()));

	        }
	        catch (Exception)
	        {
	            InterviewersList.ItemsSource = null;

	        }
	    }
    }
}