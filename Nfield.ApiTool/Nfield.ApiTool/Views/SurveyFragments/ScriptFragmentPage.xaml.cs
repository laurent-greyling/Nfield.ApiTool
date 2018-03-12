using Nfield.ApiTool.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Nfield.ApiTool.Views.SurveyFragments
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ScriptFragmentPage : ContentPage
	{

        public ScriptFragmentPage (AccessToken token,
            string serverUrl,
            string scriptFragmentName,
            string surveyId)
		{
			InitializeComponent ();
        }
	}
}