using Newtonsoft.Json;
using Nfield.ApiTool.Helper;
using Nfield.ApiTool.Models;
using Plugin.FilePicker.Abstractions;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace Nfield.ApiTool.ViewModels
{
    public class SamplingPointsImageViewModel : INotifyPropertyChanged
    {
        

        public SamplingPointsImageViewModel(AccessToken token,
            string serverUrl,
            SurveyDetails surveyDetails,
            FileData file)
        {
            if (file != null)
            {
                Task.Run(async () =>
                {
                    await UploadSamplingPointsImage(token, serverUrl, surveyDetails, file);
                });
            }
        }

        private async Task UploadSamplingPointsImage(AccessToken token, string serverUrl, SurveyDetails surveyDetails, FileData file)
        {
            using (var memoryStream = new MemoryStream(file.DataArray))
            {
                using (var reader = new StreamReader(memoryStream))
                {
                    var csvFile = await reader.ReadToEndAsync();
                    var columnsLine = new StringReader(csvFile).ReadLine();
                    var delim = new[] { ',', ';' };
                    var columns = columnsLine.Split(delim);
                    var csvData = csvFile.Split('\n').Skip(1);

                    foreach (var data in csvData)
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(data)) continue;

                            var columnData = data.Split(delim);
                            var samplingPointId = columns.IndexOf("SamplingPointId") != -1
                                    ? columnData[columns.IndexOf("SamplingPointId")]
                                    : "";

                            var imagePath = columns.IndexOf("ImagePath") != -1
                                    ? columnData[columns.IndexOf("ImagePath")]
                                    : "";

                            if (string.IsNullOrEmpty(samplingPointId) || string.IsNullOrEmpty(imagePath)) continue;

                            var fileName = Path.GetFileName(imagePath);
                            var content = $@"{Path.GetDirectoryName(imagePath)}\";

                            var url = $"{serverUrl}/v1/Surveys/{surveyDetails.SurveyId}/SamplingPoint/{samplingPointId}/Image/{fileName}";
                            await PostSamplingPointImage(url, token, imagePath);

                        }
                        catch (System.Exception e)
                        {
                            var t = e;
                            throw;
                        }
                    }
                    
                }
            }
        }

        public async Task PostSamplingPointImage(string url, AccessToken token, string contentType)
        {
            var request = new RestApi().Post(url, token, contentType);

            var s = "uploadfile=true&file=" + contentType;
            using (var writer = new StreamWriter(await request.GetRequestStreamAsync()))
            {
                writer.Write(s);
                writer.Flush();
            }
            using (var response = await request.GetResponseAsync())
            {                
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
