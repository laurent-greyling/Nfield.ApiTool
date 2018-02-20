using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nfield.ApiTool.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Nfield.ApiTool.Views
{
    public class ActionsToTake
    {
        public string Action { get; set; }
        public string Icon { get; set; } 
    }
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ActionsPage : ContentPage
	{
        public ObservableCollection<ActionsToTake> Actions { get; set; }

        public ActionsPage (AccessToken token, string serverUrl, SurveyDetails surveyDetails)
		{
			InitializeComponent();

		    Actions = new ObservableCollection<ActionsToTake>
		    {
		        new ActionsToTake {Action = "Addresses", Icon = "map.png"},
		        new ActionsToTake {Action = "Interviewers", Icon = "meeting.png"},
		        new ActionsToTake {Action = "Sampling Points", Icon = "network.png"},
		        new ActionsToTake {Action = "Survey Data", Icon = "analytics.png"},
		        new ActionsToTake {Action = "Survey Settings", Icon = "survey.png"}
		    };

		    BindingContext = this;

		}
	}
}