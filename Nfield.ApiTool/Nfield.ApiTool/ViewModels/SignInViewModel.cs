using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nfield.ApiTool.Helper;
using Nfield.ApiTool.Models;

namespace Nfield.ApiTool.ViewModels
{
    public class SignInViewModel : INotifyPropertyChanged
    {
        public AccessToken AccessToken { get; set; }

        public SignInViewModel(SignInModel signIn, string serverUrl)
        {
            SignIn(signIn, serverUrl).Wait();
        }

        public async Task SignIn(SignInModel signIn, string serverUrl)
        {
            var data = JsonConvert.SerializeObject(signIn);

            var url = $"{serverUrl}/v1/SignIn";
            var request = new RestApi().Post(url);

            using (var writer = new StreamWriter(await request.GetRequestStreamAsync()))
            {
                writer.Write(data);
                writer.Flush();
            }

            using (var response = await request.GetResponseAsync())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var content = reader.ReadToEnd();
                    AccessToken = JsonConvert.DeserializeObject<AccessToken>(content);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
