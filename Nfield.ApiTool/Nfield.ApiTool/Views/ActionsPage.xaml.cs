using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Nfield.ApiTool.Models;
using Nfield.ApiTool.Views.SamplingPoints;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Nfield.ApiTool.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ActionsPage : ContentPage
	{
        public ObservableCollection<ActionsToTake> Actions { get; set; }

        public string ActionsTitle { get; set; }

        public ActionsPage (AccessToken token, string serverUrl, SurveyDetails surveyDetails)
		{
			InitializeComponent();
            ActionsTitle = $"{surveyDetails.SurveyName} Actions";
            
            Actions = new ObservableCollection<ActionsToTake>
		    {
		        new ActionsToTake
		        {
		            Action = "Addresses",
                    Icon = "map.png",
                    ServerUrl = serverUrl,
                    SurveyDetails = surveyDetails,
                    AccessToken = token
		        },
		        new ActionsToTake
		        {
		            Action = "Interviewers",
                    Icon = "meeting.png",
                    ServerUrl = serverUrl,
                    SurveyDetails = surveyDetails,
                    AccessToken = token

                },
		        new ActionsToTake
		        {
		            Action = "Sampling Points",
                    Icon = "network.png",
                    ServerUrl = serverUrl,
                    SurveyDetails = surveyDetails,
                    AccessToken = token
		        },
		        new ActionsToTake
		        {
		            Action = "Survey Data Overview",
                    Icon = "analytics.png",
                    ServerUrl = serverUrl,
                    SurveyDetails = surveyDetails,
                    AccessToken = token
		        },
		        new ActionsToTake
		        {
		            Action = "Survey Settings",
                    Icon = "survey.png",
                    ServerUrl = serverUrl,
                    SurveyDetails = surveyDetails,
                    AccessToken = token
		        },
                new ActionsToTake
                {
                    Action = "Survey Statistics",
                    Icon = "barchart.png",
                    ServerUrl = serverUrl,
                    SurveyDetails = surveyDetails,
                    AccessToken = token
                },
            };

		    if (surveyDetails.SurveyType != SurveyType.EuroBarometer.ToString())
		    {
		        var index = Actions.Single(c => c.Action == "Addresses");
		        Actions.Remove(index);
		    }

		    if (surveyDetails.SurveyType == SurveyType.OnlineBasic.ToString())
		    {
		        var index = Actions.Single(c => c.Action == "Interviewers");
		        Actions.Remove(index);
		        index = Actions.Single(c => c.Action == "Survey Data Overview");
		        Actions.Remove(index);
            }

		    if (surveyDetails.SurveyType != SurveyType.Advanced.ToString() && surveyDetails.SurveyType != SurveyType.EuroBarometer.ToString())
		    {
		        var index = Actions.Single(c => c.Action == "Sampling Points");
		        Actions.Remove(index);
            }

            BindingContext = this;

		}

        public async Task Select_Action(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            var surveyDetails = (ActionsToTake)e.Item;

            switch (surveyDetails.Action)
            {
                case "Survey Statistics":
                    await Navigation.PushAsync(new SurveyStatisticsPage(surveyDetails.AccessToken, surveyDetails.ServerUrl, surveyDetails.SurveyDetails));
                    break;
                case "Sampling Points":
                    await Navigation.PushAsync(new SamplingPointsPage(surveyDetails.AccessToken, surveyDetails.ServerUrl, surveyDetails.SurveyDetails));
                    break;
                default:
                    break;
            }
        }

    }
}