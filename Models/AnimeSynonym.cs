using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AnimeTheme.Service.Models
{
    public class AnimeSynonym
    {
        [Key] 
        [JsonIgnore]
        public int Id { get; set; }

        public string Text { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Anime Anime { get; set; }

        public class Config : BaseEntityTypeConfiguration<AnimeSynonym>
        {
         
            
        }
    }
}