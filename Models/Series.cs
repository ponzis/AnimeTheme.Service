using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using AnimeTheme.Service.Utils;
using Microsoft.Extensions.Logging;

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
        public virtual ICollection<Anime> Animes { get; set; }
        
        public class Config : BaseEntityTypeConfiguration<Series>
        {
        }
    }

    public class SeriesMapper : ModelMapper<Series>
    {
        public SeriesMapper(ILogger<SeriesMapper> logger, ModelMapperFactory modelFactory, MapperData data) : base(logger, modelFactory, data)
        {
        }

        protected override Series CreateModel(Series model, MapperData mapper)
        {
            var result = new Series
            {
                Id = model.Id,
                Slug = model.Slug,
                Name = model.Name,
                Animes = _modelFactory.GetAnimeMapper().Get(model.Animes)
            };
            return result;
        }
    }
}