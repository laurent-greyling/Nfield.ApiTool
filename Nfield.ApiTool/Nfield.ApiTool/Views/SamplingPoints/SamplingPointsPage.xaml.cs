using Nfield.ApiTool.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.FilePicker;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
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
                var sampleData = new SamplingPointsViewModel(Token, ServerUrl, SurveyDetails, fileData);

                BindingContext = sampleData;
            }
	        catch (Exception e)
	        {
	            Console.WriteLine(e);
	            throw;
	        }
	    }
	}
}