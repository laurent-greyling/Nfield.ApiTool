using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nfield.ApiTool.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Nfield.ApiTool.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ActionsPage : ContentPage
	{
	    public string Token { get; set; }

	    public ActionsPage (AccessToken token)
		{
			InitializeComponent();
		    Token = token.AuthenticationToken;

		    BindingContext = this;
		}
	}
}