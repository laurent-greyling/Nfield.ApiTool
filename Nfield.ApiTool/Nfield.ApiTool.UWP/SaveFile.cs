using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Nfield.ApiTool.Interfaces;
using Nfield.ApiTool.UWP;
using Xamarin.Forms;

[assembly: Dependency(typeof(SaveFile))]
namespace Nfield.ApiTool.UWP
{
    public class SaveFile : ISave
    {
        // https://msdn.microsoft.com/en-us/library/windows/apps/xaml/hh758325.aspx
        public async Task SaveAsync(string filename, string text)
        {
            var picker = new FolderPicker {SuggestedStartLocation = PickerLocationId.Desktop};
            picker.FileTypeFilter.Add("*");

            var folder = await picker.PickSingleFolderAsync();
            StorageApplicationPermissions.FutureAccessList.Add(folder);
            var t = folder.Path;
            var sampleFile = await folder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(sampleFile, text);
        }
    }
}
