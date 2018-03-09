using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nfield.ApiTool.Helper;
using Nfield.ApiTool.Models;
using Plugin.FilePicker.Abstractions;

namespace Nfield.ApiTool.Services
{
    public class DownloadSamplingPoints
    {
        public async Task<string> Download(AccessToken token,
            string serverUrl,
            SurveyDetails surveyDetails,
            FileData file)
        {
            var officesUrl = $"{serverUrl}/v1/Offices";
            var offices = await GetFieldWorkOfficeIdAsync(officesUrl, token);

            var samplingUrl = $"{serverUrl}/v1/Surveys/{surveyDetails.SurveyId}/SamplingPoints";
            var samplingPoints = await Retrieve(samplingUrl, token);

            var sampleCsv = new StringBuilder();
            var headers =
                $"SamplingPointId,Name,Description,Instruction,FieldworkOffice,GroupId,Preference,Stratum{Environment.NewLine}";
            sampleCsv.Append(headers);

            foreach (var samplingPoint in samplingPoints)
            {
                var office = offices.FirstOrDefault(x => x.OfficeId == samplingPoint.FieldworkOfficeId)?.OfficeName;
                var kind = SamplingPointKind.Regular.ToString();
                switch (samplingPoint.Kind)
                {
                    case SamplingPointKind.Spare:
                        kind = SamplingPointKind.Spare.ToString();
                        break;
                    case SamplingPointKind.SpareActive:
                        kind = SamplingPointKind.SpareActive.ToString();
                        break;
                    case SamplingPointKind.Replaced:
                        kind = SamplingPointKind.Replaced.ToString();
                        break;
                }

                var row = $"{samplingPoint.SamplingPointId},{samplingPoint.Name},{samplingPoint.Description},{samplingPoint.Instruction},{office},{samplingPoint.GroupId},{kind},{samplingPoint.Stratum}{Environment.NewLine}";
                sampleCsv.Append(row);
            }
            
            return sampleCsv.ToString();
        }

        public async Task<List<SamplingPointModel>> Retrieve(string url, AccessToken token)
        {
            var request = new RestApi().Get(url, token);

            using (var response = await request.GetResponseAsync())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var content = reader.ReadToEnd();
                    return JsonConvert.DeserializeObject<List<SamplingPointModel>>(content);
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
    }
}
