using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace AnimeTheme.Service.Models
{
    public class ExternalResource
    {
        [Key] 
        [JsonIgnore]
        public int Id { get; set; }
        
        public string Link { get; set; }
        
        public ResourceSite Site { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public virtual ICollection<Anime> Animes { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public virtual ICollection<Artist> Artists { get; set; }
        
        public class Config : BaseEntityTypeConfiguration<ExternalResource>
        {
        }
    }

    public class ResourceMapper : ModelMapper<ExternalResource>
    {
        public ResourceMapper(ILogger<ResourceMapper> logger, ModelMapperFactory modelFactory, MapperData data) : base(logger, modelFactory, data)
        {
        }
        
        protected override ExternalResource CreateModel(ExternalResource model, MapperData mapper)
        {
            var result = new ExternalResource
            {
                Id = model.Id,
                Site = model.Site,
                Link = model.Link,
                Animes = _modelFactory.GetAnimeMapper().Get(model.Animes),
                Artists = _modelFactory.GetArtistMapper().Get(model.Artists)
            };
            return result;
        }
    }
}