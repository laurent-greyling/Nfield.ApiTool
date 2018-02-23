using Nfield.ApiTool.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.FilePicker;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Nfield.ApiTool.Views.SamplingPoints
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SamplingPointsPage : ContentPage
	{
        public string SurveyName { get; set; }
		public SamplingPointsPage (AccessToken token, string serverUrl, SurveyDetails surveyDetails)
		{
			InitializeComponent ();
            SurveyName = $"{surveyDetails.SurveyName} Sampling Points";


            BindingContext = this;

        }

	    public async Task Upload_File()
	    {
	        try
	        {
                var fileData = await CrossFilePicker.Current.PickFile();
	            //var delim = new[] { ',', ';' };
                //var data = await Task.Run(() => File.ReadAllLines(fileData.FilePath));


                // var columnsLine = await new StringReader(fileData.FilePath).ReadToEndAsync();
                using (var memoryStream = new MemoryStream(fileData.DataArray))
                {
                    using (var reader = new StreamReader(memoryStream))
                    {
                        var samplingPoints = new List<SamplingPointModel>();
                        var csvFile = await reader.ReadToEndAsync();
                        var columnsLine = new StringReader(csvFile).ReadLine();
                        var delim = new[] { ',', ';' };
                        var columns = columnsLine.Split(delim);
                        var csvData = csvFile.Split('\n').Skip(1);
                        
                        foreach (var data in csvData)
                        {
                            if (string.IsNullOrEmpty(data)) continue;

                            var columnData = data.Split(delim);
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
                                FieldworkOfficeId = columns.IndexOf("FieldworkOfficeId") != -1
                                    ? columnData[columns.IndexOf("FieldworkOfficeId")]
                                    : "",
                                GroupId =
                                    columns.IndexOf("GroupId") != -1 ? columnData[columns.IndexOf("GroupId")] : "",
                                Stratum =
                                    columns.IndexOf("Stratum") != -1 ? columnData[columns.IndexOf("Stratum")] : "",
                                Kind = kind
                            };

                            samplingPoints.Add(sample);
                        }

                        var uploadSampleJson = JsonConvert.SerializeObject(samplingPoints);
                    }
                }
            }
	        catch (Exception e)
	        {
	            Console.WriteLine(e);
	            throw;
	        }
	    }
	}
}