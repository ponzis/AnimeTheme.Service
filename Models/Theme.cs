using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using AnimeTheme.Service.Utils;
using Microsoft.Extensions.Logging;

namespace AnimeTheme.Service.Models
{
    public class Theme
    {
        [Key] 
        [JsonIgnore]
        public int Id { get; set; }
        public ThemeType Type { get; set; }
        public int? Sequence { get; set; }
        
        public string Group { get; set; }

      
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public virtual Song Song { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public virtual ICollection<Entry> Entries { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public virtual Anime Anime { get; set; }

        public class Config : BaseEntityTypeConfiguration<Theme>
        {
        }
    }

    public class ThemeMapper : ModelMapper<Theme>
    {
        public ThemeMapper(ILogger<ThemeMapper> logger, ModelMapperFactory modelFactory, MapperData data) : base(logger, modelFactory, data)
        {
        }

        protected override Theme CreateModel(Theme model, MapperData mapper)
        {
            var result = new Theme
            {
                Id = model.Id,
                Type = model.Type,
                Sequence = model.Sequence,
                Group = model.Group,
                
            };
            return result;
        }
    }
}