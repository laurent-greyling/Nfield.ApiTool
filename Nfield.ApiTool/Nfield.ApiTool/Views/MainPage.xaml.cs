using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nfield.ApiTool.Models;
using Nfield.ApiTool.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Nfield.ApiTool.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainPage : ContentPage
	{
	    private string ServerUrl { get; set; }
	    private SignInViewModel SigninViewModel { get; set; }

	    public MainPage ()
		{
		    NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent ();
		}

	    public async Task Login_Api()
	    {
	        try
	        {
	            Loading.IsVisible = true;
	            Login.IsVisible = false;
	            Login.IsEnabled = false;

	            await Task.Run(() =>
	            {
	                var signIModel = new SignInModel
	                {
	                    UserName = UserName.Text,
	                    Domain = Domain.Text,
	                    Password = Password.Text
	                };

	                SigninViewModel = new SignInViewModel(signIModel, ServerUrl);
                });

	            await Navigation.PushAsync(new ActionsPage(SigninViewModel.AccessToken),true);

	        }
	        catch (Exception)
	        {
	            await DisplayAlert("Access Denied", "User Name or Password is Incorrect", "OK");
            }
	        finally
	        {
	            Loading.IsVisible = false;
	            Login.IsVisible = true;
	            Login.IsEnabled = true;
	        };
        }


	    private void OnPickerSelected_IndexChanged(object sender, EventArgs e)
	    {
	        var picker = (Picker)sender;
	        var selectedIndex = picker.SelectedIndex;

	        if (selectedIndex == -1) return;

	        switch (picker.Items[selectedIndex])
	        {
	            case "RC":
	                ServerUrl = "https://rc-api.niposoftware-dev.com";
	                break;
	            case "Blue":
	                ServerUrl = "https://blue-api.niposoftware-dev.com";
	                break;
	            case "Red":
	                ServerUrl = "https://red-api.niposoftware-dev.com";
	                break;
	            case "Orange":
	                ServerUrl = "https://orange-api.niposoftware-dev.com";
	                break;
	            case "White":
	                ServerUrl = "https://white-api.niposoftware-dev.com";
	                break;
	            case "Yellow":
	                ServerUrl = "https://yellow-api.niposoftware-dev.com";
	                break;
	            case "Demo":
	                ServerUrl = "https://api.nfieldmr.com";
	                break;
	            default:
	                break;
	        };
	    }
    }
}