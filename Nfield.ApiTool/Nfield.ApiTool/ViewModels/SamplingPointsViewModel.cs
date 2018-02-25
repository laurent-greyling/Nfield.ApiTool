using Newtonsoft.Json;
using Nfield.ApiTool.Helper;
using Nfield.ApiTool.Models;
using Plugin.FilePicker.Abstractions;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace Nfield.ApiTool.ViewModels
{
    public class SamplingPointsViewModel : INotifyPropertyChanged
    {
        public string SurveyName { get; set; }

        private string _mocString;
        public string MocString
        {
            get { return _mocString; }
            set
            {
                if (_mocString != value)
                {
                    _mocString = value;
                    OnPropertyChanged("MocString");
                }
            }
        }

        public SamplingPointsViewModel(AccessToken token, string serverUrl, SurveyDetails surveyDetails, FileData file)
        {
            SurveyName = $"{surveyDetails.SurveyName} Sampling Points";
            if (file != null)
            {
                Task.Run(async () => await UploadSamplingPoints(token, serverUrl, surveyDetails, file));
            }            
        }

        public async Task UploadSamplingPoints(AccessToken token, string serverUrl, SurveyDetails surveyDetails, FileData file)
        {
            var url = $"{serverUrl}/v1/Surveys/{surveyDetails.SurveyId}/SamplingPoints";
            
            using (var memoryStream = new MemoryStream(file.DataArray))
            {
                using (var reader = new StreamReader(memoryStream))
                {
                    var samplingPoints = new List<SamplingPointModel>();
                    var csvFile = await reader.ReadToEndAsync();
                    var columnsLine = new StringReader(csvFile).ReadLine();
                    var delim = new[] { ',', ';' };
                    var columns = columnsLine.Split(delim);
                    var csvData = csvFile.Split('\n').Skip(1);

                    //Parallel.ForEach(csvData, async data =>
                    //{
                    //    if (string.IsNullOrEmpty(data)) return;

                    //    var columnData = data.Split(delim);
                    //    var kind = SamplingPointKind.Regular;
                    //    switch (columnData[columns.IndexOf("Preference")].ToLower())
                    //    {
                    //        case "spare":
                    //            kind = SamplingPointKind.Spare;
                    //            break;
                    //        case "spareactive":
                    //            kind = SamplingPointKind.SpareActive;
                    //            break;
                    //        case "replaced":
                    //            kind = SamplingPointKind.Replaced;
                    //            break;
                    //    }

                    //    var sample = new SamplingPointModel
                    //    {
                    //        SamplingPointId = columns.IndexOf("SamplingPointId") != -1
                    //            ? columnData[columns.IndexOf("SamplingPointId")]
                    //            : "",
                    //        Name = columns.IndexOf("Name") != -1 ? columnData[columns.IndexOf("Name")] : "",
                    //        Description =
                    //            columns.IndexOf("Description") != -1
                    //                ? columnData[columns.IndexOf("Description")]
                    //                : "",
                    //        Instruction =
                    //            columns.IndexOf("Instruction") != -1
                    //                ? columnData[columns.IndexOf("Instruction")]
                    //                : "",
                    //        FieldworkOfficeId = columns.IndexOf("FieldworkOffice") != -1
                    //            ? columnData[columns.IndexOf("FieldworkOffice")]
                    //            : "",
                    //        GroupId =
                    //            columns.IndexOf("GroupId") != -1 ? columnData[columns.IndexOf("GroupId")] : "",
                    //        Stratum =
                    //            columns.IndexOf("Stratum") != -1 ? columnData[columns.IndexOf("Stratum")] : "",
                    //        Kind = kind
                    //    };
                    //    var samplingData = JsonConvert.SerializeObject(sample);
                    //    var samplePosted = await PostSamplingPoint(samplingData, url, token);
                    //    samplingPoints.Add(samplePosted);
                    //});

                    //foreach (var data in csvData)
                    //{
                    //    if (string.IsNullOrEmpty(data)) return;

                    //    var columnData = data.Split(delim);
                    //    var kind = SamplingPointKind.Regular;
                    //    switch (columnData[columns.IndexOf("Preference")].ToLower())
                    //    {
                    //        case "spare":
                    //            kind = SamplingPointKind.Spare;
                    //            break;
                    //        case "spareactive":
                    //            kind = SamplingPointKind.SpareActive;
                    //            break;
                    //        case "replaced":
                    //            kind = SamplingPointKind.Replaced;
                    //            break;
                    //    }


                    //    var SamplingPointId = columns.IndexOf("SamplingPointId");
                    //    var Name = columns.IndexOf("Name");
                    //    var Description =
                    //            columns.IndexOf("Description");
                    //    var Instruction =
                    //            columns.IndexOf("Instruction");
                    //    var FieldworkOfficeId = columns.IndexOf("FieldworkOffice");
                    //    var GroupId =
                    //        columns.IndexOf("GroupId");
                    //    var Stratum =
                    //        columns.IndexOf("Stratum");


                    //    var sample = new SamplingPointModel
                    //    {
                    //        SamplingPointId = columns.IndexOf("SamplingPointId") != -1
                    //            ? columnData[columns.IndexOf("SamplingPointId")]
                    //            : "",
                    //        Name = columns.IndexOf("Name") != -1 ? columnData[columns.IndexOf("Name")] : "",
                    //        Description =
                    //            columns.IndexOf("Description") != -1
                    //                ? columnData[columns.IndexOf("Description")]
                    //                : "",
                    //        Instruction =
                    //            columns.IndexOf("Instruction") != -1
                    //                ? columnData[columns.IndexOf("Instruction")]
                    //                : "",
                    //        FieldworkOfficeId = columns.IndexOf("FieldworkOffice") != -1
                    //            ? columnData[columns.IndexOf("FieldworkOffice")]
                    //            : "",
                    //        GroupId =
                    //            columns.IndexOf("GroupId") != -1 ? columnData[columns.IndexOf("GroupId")] : "",
                    //        Stratum =
                    //            columns.IndexOf("Stratum") != -1 ? columnData[columns.IndexOf("Stratum")] : "",
                    //        Kind = kind
                    //    };

                    //    try
                    //    {
                    //        var samplingData = JsonConvert.SerializeObject(sample);
                    //        await PostSamplingPoint(samplingData, url, token);
                    //    }
                    //    catch (System.Exception)
                    //    {

                    //        throw;
                    //    }
                        
                    //    //samplingPoints.Add(samplePosted);
                    //}

                    var urll = $"{serverUrl}v1/Surveys/{surveyDetails.SurveyId}/SamplingPoints";
                    await GetStuff(urll, token);
                    var uploadSampleJson = JsonConvert.SerializeObject(samplingPoints);

                    MocString = uploadSampleJson;
                }
            }
        }

        public async Task PostSamplingPoint(string data, string url, AccessToken token)
        {
            var request = new RestApi().Post(url, token);

            using (var writer = new StreamWriter(await request.GetRequestStreamAsync()))
            {
                writer.Write(data);
                writer.Flush();
            }

            //using (var response = await request.GetResponseAsync())
            //{
            //    using (var reader = new StreamReader(response.GetResponseStream()))
            //    {
            //        var content = reader.ReadToEnd();
            //        return JsonConvert.DeserializeObject<SamplingPointModel>(content);
            //    }
            //}
        }

        public async Task GetStuff(string url, AccessToken token)
        {
            var request = new RestApi().Get(url, token);
            try
            {
                using (WebResponse response = await request.GetResponseAsync())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        var content = reader.ReadToEnd();
                        var SurveyCounts = JsonConvert.DeserializeObject<SamplingPointModel>(content);
                    }
                }
            }
            catch (System.Exception e)
            {
                var t = e;
                throw;
            }
            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
