using System.Threading.Tasks;
using Nfield.ApiTool.Helper;
using Nfield.ApiTool.Models;

namespace Nfield.ApiTool.Services
{
    public class FragmentOperations
    {
        public async Task Delete(AccessToken token,
            string serverUrl,
            string scriptFragmentName,
            string surveyId)
        {
            var url = $"{serverUrl}/v1/Surveys/{surveyId}/ScriptFragments/{scriptFragmentName}";
            var request = new RestApi().Delete(url, token);
            
            using (var response = await request.GetResponseAsync())
            {
            }
        }

        public async Task Update(AccessToken token,
            string serverUrl,
            string scriptFragmentName,
            string surveyId,
            byte[] fragment)
        {
            var url = $"{serverUrl}/v1/Surveys/{surveyId}/ScriptFragments/{scriptFragmentName}";
            await PostFragment(url, token, fragment);
        }

        private async Task PostFragment(string url, AccessToken token, byte[] contentBytes)
        {
            var request = new RestApi().PostStream(url, token);

            using (var stream = await request.GetRequestStreamAsync())
            {
                await stream.WriteAsync(contentBytes, 0, contentBytes.Length);
            }

            using (var response = await request.GetResponseAsync())
            {
            }
        }
    }
}
