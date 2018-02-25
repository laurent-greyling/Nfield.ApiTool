using ReadExcel.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadExcel
{
    interface IExcelReader
    {
        List<SamplingPointModel> GetExcelSamplePointData(string path); 
    }
}
