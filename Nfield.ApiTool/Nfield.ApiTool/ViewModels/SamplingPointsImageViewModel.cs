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
        private string _loading;
        public string Loading
        {
            get { return _loading; }
            set
            {
                if (_loading != value)
                {
                    _loading = value;
                    OnPropertyChanged("Loading");
                }
            }
        }

        private bool _isLoading = true;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged("IsLoading");
                }
            }
        }

        public SamplingPointsImageViewModel(AccessToken token,
            string serverUrl,
            SurveyDetails surveyDetails,
            FileData file,
            bool isLoading = false,
            string loading = "")
        {
            Loading = loading;
            IsLoading = isLoading;
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

                            File.SetAttributes(imagePath, FileAttributes.Normal);
                            var contentBytes = File.ReadAllBytes(imagePath);

                            var url = $"{serverUrl}/v1/Surveys/{surveyDetails.SurveyId}/SamplingPoint/{samplingPointId}/Image/{fileName}";
                            await PostSamplingPointImage(url, token, contentBytes);

                        }
                        catch (System.Exception e)
                        {
                            IsLoading = false;
                            throw;
                        }
                    }

                    IsLoading = false;
                }
            }
        }

        public async Task PostSamplingPointImage(string url, AccessToken token, byte[] contentBytes)
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
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
