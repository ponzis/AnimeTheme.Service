using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AnimeTheme.Service.Models
{
    public class Entry
    {
        [Key] 
        [JsonIgnore]
        public int Id { get; set; }
        public int Version { get; set; }
        public string Episodes { get; set; }
        public bool Nsfw { get; set; }
        public bool Spoiler { get; set; }
        public string Notes { get; set; }
        public virtual Theme Theme { get; set; }
        public virtual ICollection<Video> Videos { get; set; }

        public class Config : BaseEntityTypeConfiguration<Entry>
        {
        }

        
    }
}