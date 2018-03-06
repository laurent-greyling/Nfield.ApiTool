using System.IO;
using Nfield.ApiTool.Droid.SQLite;
using Nfield.ApiTool.Interfaces;
using SQLite;
using Xamarin.Forms;

[assembly: Dependency(typeof(StorageConnectionDb))]
namespace Nfield.ApiTool.Droid.SQLite
{
    public class StorageConnectionDb : ISQLite
    {
        public SQLiteConnection GetConnection()
        {
            var fileName = "ApiToolDetails.db3";
            var documentPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var path = Path.Combine(documentPath, fileName);

            var connection = new SQLiteConnection(path);

            return connection;
        }
    }
}