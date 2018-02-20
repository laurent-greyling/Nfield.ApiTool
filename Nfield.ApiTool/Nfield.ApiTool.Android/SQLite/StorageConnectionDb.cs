using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
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