using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MelodyMantra.Models
{
    public class Album
    {
        public int AlbumId { get; set; }
        public string AlbumName { get; set; }
        public string Image { get; set; }
        public int Price { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Genre { get; set; }
        public string ArtistName { get; set; }
    }

}


