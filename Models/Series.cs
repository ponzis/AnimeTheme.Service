using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AnimeTheme.Service.Models
{
    public class Series
    {
        [Key] 
        [JsonIgnore]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ICollection<Anime> Animes { get; set; }
        
        public class Config : BaseEntityTypeConfiguration<Series>
        {
        }
    }
}