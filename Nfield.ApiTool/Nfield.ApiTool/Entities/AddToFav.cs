using SQLite;

namespace Nfield.ApiTool.Entities
{
    public class AddToFav
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Unique]
        public string SurveId { get; set; }
        public bool IsFavourite { get; set; }

        public string Icon { get; set; }
        public AddToFav()
        {

        }
    }
}
