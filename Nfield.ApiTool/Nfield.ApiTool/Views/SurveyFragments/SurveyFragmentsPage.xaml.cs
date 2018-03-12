using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nfield.ApiTool.Models;
using Nfield.ApiTool.ViewModels;
using Plugin.FilePicker;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Nfield.ApiTool.Views.SurveyFragments
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SurveyFragmentsPage : ContentPage
	{
	    private AccessToken Token { get; set; }
	    private string ServerUrl { get; set; }
	    private SurveyDetails SurveyDetails { get; set; }

        public SurveyScriptFragmentViewModel ScriptFragments { get; set; }

        
        public SurveyFragmentsPage (AccessToken token, string serverUrl, SurveyDetails surveyDetails)
		{
			InitializeComponent ();
		    Token = token;
		    ServerUrl = serverUrl;
		    SurveyDetails = surveyDetails;
		    ScriptFragments = new SurveyScriptFragmentViewModel(token, serverUrl, surveyDetails, null);
		    BindingContext = ScriptFragments;
        }

	    public async Task Upload_File()
	    {
	        try
	        {
	            var fileData = await CrossFilePicker.Current.PickFile();
	            ScriptFragments = new SurveyScriptFragmentViewModel(Token, ServerUrl, SurveyDetails, fileData, true, "Uploading...");

	            BindingContext = ScriptFragments;
	        }
	        catch (Exception e)
	        {
	            await DisplayAlert("Error", $"Something went wrong uploading interviewers {e.Message}", "Ok");
	            Console.WriteLine(e);
	            throw;
	        }
	    }

	    private void SearchBar_OnTextChanged(object sender, TextChangedEventArgs e)
	    {
	        try
	        {
	            FragmentList.ItemsSource = string.IsNullOrWhiteSpace(e.NewTextValue)
	                ? FragmentList.ItemsSource = ScriptFragments.ScriptFragments
	                : FragmentList.ItemsSource = ScriptFragments.ScriptFragments.Where(n =>
	                {
	                    var fragmentName = string.IsNullOrEmpty(n.FragmentName) ? "" : n.FragmentName.ToLower();

	                    return fragmentName.Contains(e.NewTextValue.ToLower());
	                });

	        }
	        catch (Exception)
	        {
	            FragmentList.ItemsSource = null;

	        }
	    }

	    private async Task Select_Fragment(object sender, ItemTappedEventArgs e)
	    {
	        if (e.Item == null)
	            return;

	        var scriptFragment = (ScriptFragmentsModel)e.Item;

	        await Navigation.PushAsync(new ScriptFragmentPage(Token,ServerUrl, scriptFragment.FragmentName,SurveyDetails));
	    }
    }
}