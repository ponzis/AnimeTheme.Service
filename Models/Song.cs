using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using AnimeTheme.Service.Utils;
using Microsoft.Extensions.Logging;

namespace AnimeTheme.Service.Models
{
    public class Song
    {
        [Key] 
        [JsonIgnore]
        public int Id { get; set; }
        public string Title { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public virtual ICollection<Artist> Artist { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public virtual ICollection<Theme> Themes { get; set; }

        public class Config : BaseEntityTypeConfiguration<Song>
        {
        }
    }

    public class SongMapper : ModelMapper<Song>
    {
        public SongMapper(ILogger<SongMapper> logger, ModelMapperFactory modelFactory, MapperData data) : base(logger, modelFactory, data)
        {
        }

        protected override Song CreateModel(Song model, MapperData mapper)
        {
            var result = new Song
            {
                Id = model.Id,
                Title = model.Title,
                Artist = _modelFactory.GetArtistMapper().Get(model.Artist),
                Themes = _modelFactory.GetThemeMapper().Get(model.Themes)
            };
            return result;
        }
    }
}