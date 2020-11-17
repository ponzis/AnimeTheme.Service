using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AnimeTheme.Service.Models
{
    public class Song
    {
        [Key] 
        [JsonIgnore]
        public int Id { get; set; }
        public string Title { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public virtual ICollection<Theme> Themes { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Artist Artist { get; set; }

        public class Config : BaseEntityTypeConfiguration<Song>
        {
        }
    }
}