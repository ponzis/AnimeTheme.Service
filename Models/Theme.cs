using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnimeTheme.Service.Models
{
    public class Theme
    {
        [Key] 
        [JsonIgnore]
        public int Id { get; set; }
        public string Group { get; set; }
        public ThemeType Type { get; set; }
        public int? Sequence { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public virtual Anime Anime { get; set; }
 
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public virtual Song Song { get; set; }

        public class Config : BaseEntityTypeConfiguration<Theme>
        {
        }
    }
}