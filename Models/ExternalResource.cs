using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using AnimeTheme.Service.Services;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnimeTheme.Service.Models
{
    public class ExternalResource
    {
        [Key] 
        [JsonIgnore]
        public int Id { get; set; }

        public ResourceSite Site { get; set; }
        public string Link { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public virtual ICollection<Anime> Animes { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public virtual ICollection<Artist> Artists { get; set; }

        public class Config : BaseEntityTypeConfiguration<ExternalResource>
        {
        }
    }
}