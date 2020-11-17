using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnimeTheme.Service.Models
{
    public class Anime
    {
        [Key] 
        [JsonIgnore]
        public int Id { get; set; }
        public string Name { get; set; }
        
        public string Slug { get; set; }
        
        public int Year { get; set; }

        
        public AnimeSeason? Season { get; set; }

        public virtual ICollection<AnimeSynonym> Synonyms { get; set; }
        public virtual  ExternalResource Resource { get; set; }
        
        public virtual Series Series { get; set; }
        public virtual ICollection<Theme> Themes { get; set; }

        public class Config : BaseEntityTypeConfiguration<Anime>
        {
            public override void Configure(EntityTypeBuilder<Anime> builder)
            {
                builder.HasIndex(u => u.Slug).IsUnique();
                base.Configure(builder);
            }
        }
    }

    public static class AnimeExtensions
    {
        public static Anime ShallowClone(this Anime anime)
        {
            var themes = anime.Themes == null ? null : (from theme in anime.Themes
                select new Theme
                {
                    Group = theme.Group,
                    Sequence = theme.Sequence, 
                    Type = theme.Type
                }).ToList();
            
            var series = anime.Series == null ? null : new Series
            {
                Slug = anime.Series.Slug,
                Name = anime.Series.Name,
            }; 
            
            var resource = anime.Resource == null ? null : new ExternalResource
            {
                Site = anime.Resource.Site,
                Link = anime.Resource.Link
            };
            
            var result = new Anime
            {
                Name = anime.Name,
                Slug = anime.Slug,
                Year = anime.Year,
                Season = anime.Season,
                Synonyms = anime.Synonyms,
                Resource = resource,
                Series = series,
                Themes = themes
            };
            return result;
        }
    }
}