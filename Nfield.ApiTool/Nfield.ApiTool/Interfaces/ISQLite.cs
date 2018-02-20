using SQLite;

namespace Nfield.ApiTool.Interfaces
{
    public interface ISQLite
    {
        SQLiteConnection GetConnection();
    }
}
