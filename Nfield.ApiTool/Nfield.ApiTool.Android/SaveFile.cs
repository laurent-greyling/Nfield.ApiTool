using Nfield.ApiTool.Droid;
using Nfield.ApiTool.Interfaces;
using Xamarin.Forms;
using System.IO;
using System.Threading.Tasks;
using Environment = System.Environment;

[assembly: Dependency(typeof(SaveFile))]
namespace Nfield.ApiTool.Droid
{
    public class SaveFile : ISave
    {
        public async Task SaveAsync(string filename, string text)
        {
            await Task.Run(() => {
                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var filePath = Path.Combine(documentsPath, filename);
                File.WriteAllText(filePath, text);
            });
        }
    }
}