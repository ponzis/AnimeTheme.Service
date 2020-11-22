using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace AnimeTheme.Service.Models
{
    public class Synonym
    {
        [Key] 
        [JsonIgnore]
        public int Id { get; set; }

        public string Text { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public virtual Anime Anime { get; set; }
        
        public class Config : BaseEntityTypeConfiguration<Synonym>
        {
        }
    }
    
    public class SynonymMapper : ModelMapper<Synonym>
    {
        public SynonymMapper(ILogger<SynonymMapper> logger, ModelMapperFactory modelFactory, MapperData data) : base(logger, modelFactory, data)
        {
        }
        
        protected override Synonym CreateModel(Synonym model, MapperData mapper)
        {
            var result = new Synonym
            {
                Id = model.Id,
                Text = model.Text,
                Anime = _modelFactory.GetAnimeMapper().Get(model.Anime)
            };
            return result;
        }
    }
}