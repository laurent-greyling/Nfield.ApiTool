using System.Threading.Tasks;

namespace Nfield.ApiTool.Interfaces
{
    public interface ISave
    {
        Task SaveAsync(string filename, string text);
    }
}
