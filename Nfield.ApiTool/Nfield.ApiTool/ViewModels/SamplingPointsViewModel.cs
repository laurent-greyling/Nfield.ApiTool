﻿using Newtonsoft.Json;
using Nfield.ApiTool.Helper;
using Nfield.ApiTool.Models;
using Plugin.FilePicker.Abstractions;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace Nfield.ApiTool.ViewModels
{
    public class SamplingPointsViewModel : INotifyPropertyChanged
    {
        public string SurveyName { get; set; }

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
        

        private List<SamplingPointModel> _samplingPoints;
        public List<SamplingPointModel> SamplingPoints
        {
            get { return _samplingPoints; }
            set
            {
                if (_samplingPoints != value)
                {
                    _samplingPoints = value;
                    OnPropertyChanged("SamplingPoints");
                }
            }
        }

        private string ServerUrl { get; set; }
        private string SurveyId { get; set; }

        public SamplingPointsViewModel(AccessToken token,
            string serverUrl,
            SurveyDetails surveyDetails,
            FileData file,
            bool isLoading= false,
            string loading = "")
        {
            ServerUrl = serverUrl;
            SurveyId = surveyDetails.SurveyId;
            Loading = loading;
            IsLoading = isLoading;
            SurveyName = $"{surveyDetails.SurveyName} Sampling Points";
            if (file != null)
            {
                Task.Run(async () =>
                {
                    await UploadSamplingPoints(token, file);
                });
            }
            else
            {
                var samplingUrl = $"{serverUrl}/v1/Surveys/{SurveyId}/SamplingPoints";
                Task.Run(async () => SamplingPoints = await GetSamplingPointsAsync(samplingUrl, token));
            }
        }

        private async Task UploadSamplingPoints(AccessToken token, FileData file)
        {
            var url = $"{ServerUrl}/v1/Surveys/{SurveyId}/SamplingPoints";
            var officesUrl = $"{ServerUrl}/v1/Offices";
            var offices = await GetFieldWorkOfficeIdAsync(officesUrl, token);

            using (var memoryStream = new MemoryStream(file.DataArray))
            {
                using (var reader = new StreamReader(memoryStream))
                {
                    var csvFile = await reader.ReadToEndAsync();
                    var columnsLine = new StringReader(csvFile).ReadLine();
                    var delim = new[] { ',', ';' };
                    var columns = columnsLine.Split(delim);
                    var csvData = csvFile.Split('\n').Skip(1);
                    
                    foreach (var row in csvData)
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(row)) continue;

                            var columnData = row.Split(delim);
                            var kind = SamplingPointKind.Regular;
                            switch (columnData[columns.IndexOf("Preference")].ToLower())
                            {
                                case "spare":
                                    kind = SamplingPointKind.Spare;
                                    break;
                                case "spareactive":
                                    kind = SamplingPointKind.SpareActive;
                                    break;
                                case "replaced":
                                    kind = SamplingPointKind.Replaced;
                                    break;
                            }

                            var officeId = string.Empty;
                            if (columns.IndexOf("FieldworkOffice") != -1)
                            {
                                officeId = offices.FirstOrDefault(x =>
                                    x.OfficeName == columnData[columns.IndexOf("FieldworkOffice")])?.OfficeId;
                            }

                            var sample = new SamplingPointModel
                            {
                                SamplingPointId = columns.IndexOf("SamplingPointId") != -1
                                    ? columnData[columns.IndexOf("SamplingPointId")]
                                    : "",
                                Name = columns.IndexOf("Name") != -1 ? columnData[columns.IndexOf("Name")] : "",
                                Description =
                                    columns.IndexOf("Description") != -1
                                        ? columnData[columns.IndexOf("Description")]
                                        : "",
                                Instruction =
                                    columns.IndexOf("Instruction") != -1
                                        ? columnData[columns.IndexOf("Instruction")]
                                        : "",
                                FieldworkOfficeId = officeId,
                                GroupId =
                                    columns.IndexOf("GroupId") != -1 ? columnData[columns.IndexOf("GroupId")] : null,
                                Stratum =
                                    columns.IndexOf("Stratum") != -1 ? columnData[columns.IndexOf("Stratum")] : null,
                                Kind = kind
                            };
                            var samplingData = JsonConvert.SerializeObject(sample);
                            await PostSamplingPoint(samplingData, url, token);
                        }
                        catch (System.Exception e)
                        {
                            IsLoading = false;
                            throw;
                        }
                    }

                    var samplingUrl = $"{ServerUrl}/v1/Surveys/{SurveyId}/SamplingPoints";
                    SamplingPoints = await GetSamplingPointsAsync(samplingUrl, token);

                }
            }
        }

        private async Task PostSamplingPoint(string data, string url, AccessToken token)
        {
            var request = new RestApi().Post(url, token);

            using (var writer = new StreamWriter(await request.GetRequestStreamAsync()))
            {
                writer.Write(data);
                writer.Flush();
            }

            using (var response = await request.GetResponseAsync())
            {
            }
        }

        public async Task<List<SamplingPointModel>> GetSamplingPointsAsync(string url, AccessToken token)
        {
            var request = new RestApi().Get(url, token);

            using (var response = await request.GetResponseAsync())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var content = reader.ReadToEnd();
                    var samplingPoints = JsonConvert.DeserializeObject<List<SamplingPointModel>>(content);

                    IsLoading = false;
                    return samplingPoints;
                }
            }

        }

        public async Task<List<FieldworkOfficeModel>> GetFieldWorkOfficeIdAsync(string url, AccessToken token)
        {
            var request = new RestApi().Get(url, token);
            using (var response = await request.GetResponseAsync())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var content = reader.ReadToEnd();
                    return JsonConvert.DeserializeObject<List<FieldworkOfficeModel>>(content);
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
