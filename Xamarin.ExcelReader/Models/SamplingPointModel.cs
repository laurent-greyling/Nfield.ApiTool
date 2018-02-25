
namespace Xamarin.ExcelReader.Models
{
    public class SamplingPointModel
    {
        public string SamplingPointId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Instruction { get; set; }
        public string FieldworkOfficeId { get; set; }
        public string GroupId { get; set; }
        public string Stratum { get; set; }
        public SamplingPointKind Kind { get; set; }
    }
}
