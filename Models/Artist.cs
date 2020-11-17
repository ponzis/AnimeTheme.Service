using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnimeTheme.Service.Models
{
    public class Artist
    {
        [Key] 
        [JsonIgnore]
        public int Id { get; set; }
        
        public string Name { get; set; }
        public string Slug { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public virtual ExternalResource Resource { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public virtual  ICollection<Song> Songs { get; set; }

        public class Config : BaseEntityTypeConfiguration<Artist>
        {
            public override void Configure(EntityTypeBuilder<Artist> builder)
            {
                builder.HasIndex(u => u.Slug).IsUnique();
                base.Configure(builder);
            }
        }
    }
    
    public static class ArtistExtensions
    {
        public static Artist ShallowClone(this Artist artist)
        {
            var songs = artist.Songs == null ? null : (from song in artist.Songs
                select new Song
                {
                    Title = song.Title
                }).ToList();
            
            var resource = artist.Resource == null ? null : new ExternalResource
            {
                Site = artist.Resource.Site,
                Link = artist.Resource.Link
            };
            
            var result = new Artist
            {
                Name = artist.Name,
                Slug = artist.Slug,
                Resource = resource,
                Songs = songs
            };
            return result;
        }
    }
}