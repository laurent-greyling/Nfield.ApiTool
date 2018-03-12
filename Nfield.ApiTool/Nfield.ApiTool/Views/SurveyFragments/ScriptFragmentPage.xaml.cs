using System.Text;
using System.Threading.Tasks;
using Nfield.ApiTool.Models;
using Nfield.ApiTool.Services;
using Nfield.ApiTool.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Nfield.ApiTool.Views.SurveyFragments
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ScriptFragmentPage : ContentPage
	{
        public ScriptFragmentUpdateViewModel ScriptFragment { get; set; }
        private SurveyDetails SurveyDetails { get; set; }
	    private AccessToken Token { get; set; }
        private string ServerUrl { get; set; }
	    private string FragmentNme { get; set; }
	    private string SurveyId { get; set; }
        public ScriptFragmentPage (AccessToken token,
            string serverUrl,
            string scriptFragmentName, SurveyDetails surveyDetails)
		{
			InitializeComponent ();
		    Token = token;
		    ServerUrl = serverUrl;
		    FragmentNme = scriptFragmentName;
		    SurveyId = surveyDetails.SurveyId;
		    SurveyDetails = surveyDetails;

            ScriptFragment = new ScriptFragmentUpdateViewModel(token,serverUrl,scriptFragmentName, surveyDetails.SurveyId, surveyDetails.SurveyName);

		    BindingContext = ScriptFragment;
		}

	    public async Task Delete_Fragment()
	    {
	        await new FragmentOperations().Delete(Token, ServerUrl, FragmentNme, SurveyId);
	        await Navigation.PushAsync(new SurveyFragmentsPage(Token,ServerUrl, SurveyDetails));
	    }

	    public async Task Update_Fragment()
	    {
	        var fragName = FragmentName.Text;
	        var content = Encoding.UTF8.GetBytes(Fragment.Text);

	        await new FragmentOperations().Update(Token, ServerUrl, fragName, SurveyId, content);
	        await Navigation.PushAsync(new SurveyFragmentsPage(Token, ServerUrl, SurveyDetails));
        }
	}
}