using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using AnimeTheme.Service.Utils;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;

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
        public virtual ICollection<Song> Songs { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public virtual ExternalResource Resource { get; set; }
        
        public class Config : BaseEntityTypeConfiguration<Artist>
        {
            public override void Configure(EntityTypeBuilder<Artist> builder)
            {
                builder.HasIndex(u => u.Slug).IsUnique();
                base.Configure(builder);
            }
        }
    }
    
    public class ArtistMapper : ModelMapper<Artist>
    {
        public ArtistMapper(ILogger<ArtistMapper> logger, ModelMapperFactory modelFactory, MapperData data) : base(logger, modelFactory, data)
        {
        }
        
        protected override Artist CreateModel(Artist model, MapperData mapper)
        {
            var result = new Artist
            {
                Id = model.Id,
                Name = model.Name,
                Slug = model.Slug,
                Resource = _modelFactory.GetResourceMapper().Get(model.Resource),
                Songs = _modelFactory.GetSongMapper().Get(model.Songs)
            };
            return result;
        }
    }
}