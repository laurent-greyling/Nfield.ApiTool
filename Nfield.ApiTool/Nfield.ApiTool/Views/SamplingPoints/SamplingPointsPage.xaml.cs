using Nfield.ApiTool.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Nfield.ApiTool.Views.SamplingPoints
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SamplingPointsPage : ContentPage
	{
        public string SurveyName { get; set; }
		public SamplingPointsPage (AccessToken token, string serverUrl, SurveyDetails surveyDetails)
		{
			InitializeComponent ();
            SurveyName = $"{surveyDetails.SurveyName} Sampling Points";


            BindingContext = this;

        }
	}
}