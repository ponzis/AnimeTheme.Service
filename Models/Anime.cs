using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;

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
        
        public string Synopsis { get; set; }

        public Uri Cover { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public virtual ICollection<Synonym> Synonyms { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public virtual ICollection<Theme> Themes { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public virtual Series Series { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public virtual ExternalResource Resource { get; set; }
        
        public class Config : BaseEntityTypeConfiguration<Anime>
        {
            public override void Configure(EntityTypeBuilder<Anime> builder)
            {
                builder.HasIndex(u => u.Slug).IsUnique();
                base.Configure(builder);
            }
        }
    }
    
    public class AnimeMapper : ModelMapper<Anime>
    {
        public AnimeMapper(ILogger<AnimeMapper> logger, ModelMapperFactory modelFactory, MapperData data) : base(logger, modelFactory, data)
        {
        }
        
        protected override Anime CreateModel(Anime model, MapperData mapper)
        {
            var result = new Anime
            {
                Id = model.Id,
                Name = model.Name,
                Slug = model.Slug,
                Year = model.Year,
                Season = model.Season,
                Cover = model.Cover,
                Synopsis = model.Synopsis,
                Synonyms = _modelFactory.GetSynonymMapper().Get(model.Synonyms),
                Resource = _modelFactory.GetResourceMapper().Get(model.Resource),
                Series = _modelFactory.GetSeriesMapper().Get(model.Series),
                Themes = _modelFactory.GetThemeMapper().Get(model.Themes)
            };
            return result;
        }
    }
}