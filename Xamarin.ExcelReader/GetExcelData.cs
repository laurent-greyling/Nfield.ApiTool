using System.Collections.Generic;
using Xamarin.ExcelReader.Models;
using Excel = Microsoft.Office.Interop.Excel;

namespace Xamarin.ExcelReader
{
    public class GetExcelData
    {
        private Excel.Application xlApp;
        private Excel.Workbook xlWorkBook;
        private Excel.Range range;

        private Excel.Worksheet Worksheet(string path)
        {
            xlApp = new Excel.ApplicationClass();
            xlWorkBook = xlApp.Workbooks.Open(path,
                0,
                true,
                5,
                "",
                "",
                true,
                Excel.XlPlatform.xlWindows,
                "\t",
                false,
                false,
                0,
                true,
                1,
                0);

            return (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
        }

        public List<SamplingPointModel> GetExcelSamplePointData(string path)
        {
            var xlWorkSheet = Worksheet(path);
            var rngHeader = xlWorkSheet.Rows[1] as Excel.Range;

            var samplingPointIdIndex = rngHeader.Find(What: "SamplingPointId", LookIn: Excel.XlFindLookIn.xlValues,
            LookAt: Excel.XlLookAt.xlPart, SearchOrder: Excel.XlSearchOrder.xlByColumns);
            var nameIndex = rngHeader.Find(What: "Name", LookIn: Excel.XlFindLookIn.xlValues,
            LookAt: Excel.XlLookAt.xlPart, SearchOrder: Excel.XlSearchOrder.xlByColumns);
            var descriptionIndex = rngHeader.Find(What: "Description", LookIn: Excel.XlFindLookIn.xlValues,
            LookAt: Excel.XlLookAt.xlPart, SearchOrder: Excel.XlSearchOrder.xlByColumns);
            var instructionIndex = rngHeader.Find(What: "Instruction", LookIn: Excel.XlFindLookIn.xlValues,
            LookAt: Excel.XlLookAt.xlPart, SearchOrder: Excel.XlSearchOrder.xlByColumns);
            var groupIdIndex = rngHeader.Find(What: "GroupId", LookIn: Excel.XlFindLookIn.xlValues,
            LookAt: Excel.XlLookAt.xlPart, SearchOrder: Excel.XlSearchOrder.xlByColumns);
            var fieldWorkOfficeIndex = rngHeader.Find(What: "FieldworkOffice", LookIn: Excel.XlFindLookIn.xlValues,
            LookAt: Excel.XlLookAt.xlPart, SearchOrder: Excel.XlSearchOrder.xlByColumns);
            var stratumIndex = rngHeader.Find(What: "Stratum", LookIn: Excel.XlFindLookIn.xlValues,
            LookAt: Excel.XlLookAt.xlPart, SearchOrder: Excel.XlSearchOrder.xlByColumns);
            var PreferenceIndex = rngHeader.Find(What: "Preference", LookIn: Excel.XlFindLookIn.xlValues,
            LookAt: Excel.XlLookAt.xlPart, SearchOrder: Excel.XlSearchOrder.xlByColumns);

            range = xlWorkSheet.UsedRange;

            var sampleDataItems = new List<SamplingPointModel>();

            for (var spRowCnt = 2; spRowCnt <= range.Rows.Count; spRowCnt++)
            {
                var samplingPointId = samplingPointIdIndex != null ? xlWorkSheet.Cells[spRowCnt, samplingPointIdIndex.Column] as Excel.Range : null;
                var name = nameIndex != null ? xlWorkSheet.Cells[spRowCnt, nameIndex.Column] as Excel.Range : null;
                var description = descriptionIndex != null ? xlWorkSheet.Cells[spRowCnt, descriptionIndex.Column] as Excel.Range : null;
                var instruction = instructionIndex != null ? xlWorkSheet.Cells[spRowCnt, instructionIndex.Column] as Excel.Range : null;
                var groupId = groupIdIndex != null ? xlWorkSheet.Cells[spRowCnt, groupIdIndex.Column] as Excel.Range : null;
                var fieldWorkOffice = fieldWorkOfficeIndex != null ? xlWorkSheet.Cells[spRowCnt, fieldWorkOfficeIndex.Column] as Excel.Range : null;
                var stratum = stratumIndex != null ? xlWorkSheet.Cells[spRowCnt, stratumIndex.Column] as Excel.Range : null;
                var preference = PreferenceIndex != null ? xlWorkSheet.Cells[spRowCnt, PreferenceIndex.Column] as Excel.Range : null;

                var knd = preference != null ? preference.Value?.ToString() : "";
                var kind = SamplingPointKind.Regular;

                if (knd != null)
                {
                    switch (knd.ToLower())
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
                        default:
                            kind = SamplingPointKind.Regular;
                            break;
                    }
                }

                var samplingPoint = new SamplingPointModel
                {
                    SamplingPointId = samplingPointId != null ? samplingPointId.Value?.ToString() : "",
                    Name = name != null ? name.Value?.ToString() : "",
                    Description = description != null ? description.Value?.ToString() : "",
                    Instruction = instruction != null ? instruction.Value?.ToString() : "",
                    GroupId = groupId != null ? groupId.Value?.ToString() : "",
                    FieldworkOfficeId = fieldWorkOffice != null ? fieldWorkOffice.Value?.ToString() : "",
                    Stratum = stratum != null ? stratum.Value?.ToString() : "",
                    Kind = kind
                };

                sampleDataItems.Add(samplingPoint);
            }

            CloseExcel();

            return sampleDataItems;
        }

        private void CloseExcel()
        {
            xlWorkBook.Close(true, null, null);
            xlApp.Quit();
        }
    }
}
