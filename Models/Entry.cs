using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace AnimeTheme.Service.Models
{
    public class Entry
    {
        [Key] 
        [JsonIgnore]
        public int Id { get; set; }
        public int Version { get; set; }
        public string Episodes { get; set; }
        public bool Nsfw { get; set; }
        public bool Spoiler { get; set; }
        public string Notes { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public virtual ICollection<Video> Videos { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public virtual Theme Theme { get; set; }
        
        public class Config : BaseEntityTypeConfiguration<Entry>
        {
        }
    }
    public class EntryMapper : ModelMapper<Entry>
    {
        public EntryMapper(ILogger<EntryMapper> logger, ModelMapperFactory modelFactory, MapperData data) : base(logger, modelFactory, data)
        {
        }

        protected override Entry CreateModel(Entry model, MapperData mapper)
        {
            var result = new Entry
            {
                Id = model.Id,
                Version = model.Version,
                Episodes = model.Episodes,
                Nsfw = model.Nsfw,
                Spoiler = model.Spoiler,
                Notes = model.Notes,
                Videos = _modelFactory.GetVideoMapper().Get(model.Videos),
                Theme = _modelFactory.GetThemeMapper().Get(model.Theme),
            };
            return result;
        }
    }
}