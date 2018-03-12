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
	                {
	                    var userName = string.IsNullOrEmpty(n.Username) ? "" : n.Username.ToLower();
	                    var firstName = string.IsNullOrEmpty(n.FirstName) ? "" : n.FirstName.ToLower();
	                    var lastName = string.IsNullOrEmpty(n.LastName) ? "" : n.LastName.ToLower();
	                    var clientInterviewerId = string.IsNullOrEmpty(n.ClientInterviewerId) ? "" : n.ClientInterviewerId.ToLower();
	                    var emailAddress = string.IsNullOrEmpty(n.EmailAddress) ? "" : n.EmailAddress.ToLower();

                        return userName.Contains(e.NewTextValue.ToLower()) ||
                               firstName.Contains(e.NewTextValue.ToLower()) ||
                               lastName.Contains(e.NewTextValue.ToLower()) ||
                               clientInterviewerId.Contains(e.NewTextValue.ToLower()) ||
                               emailAddress.Contains(e.NewTextValue.ToLower());
	                });

	        }
	        catch (Exception)
	        {
	            InterviewersList.ItemsSource = null;

	        }
	    }
    }
}