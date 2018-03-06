using System.IO;
using Windows.Storage;
using Nfield.ApiTool.Interfaces;
using Nfield.ApiTool.UWP.SQLite;
using SQLite;
using Xamarin.Forms;

[assembly: Dependency(typeof(StorageConnectionDb))]
namespace Nfield.ApiTool.UWP.SQLite
{
    public class StorageConnectionDb : ISQLite
    {
        public SQLiteConnection GetConnection()
        {
            var fileName = "ApiToolDetails.db3";
            var path = Path.Combine(ApplicationData.Current.LocalFolder.Path, fileName);

            var connection = new SQLiteConnection(path);

            return connection;
        }
    }
}
