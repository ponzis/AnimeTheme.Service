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
    public class Video
    {
        [Key] 
        [JsonIgnore]
        public int Id { get; set; }
        
        public string Basename  { get; set; }
        public string Filename { get; set; }
        public Uri Path  { get; set; }
        
        public int Resolution  { get; set; }
        
        public bool Nc { get; set; }
        
        public bool Subbed  { get; set; }
        public bool Lyrics  { get; set; }
        public bool Uncen  { get; set; }
        public VideoSource? Source  { get; set; }
        public VideoOverlap Overlap  { get; set; }
        
        public class Config : BaseEntityTypeConfiguration<Video>
        {
        }
    }

    public class VideoMapper : ModelMapper<Video>
    {
        public VideoMapper(ILogger<VideoMapper> logger, ModelMapperFactory modelFactory, MapperData data) : base(logger, modelFactory, data)
        {
        }

        protected override Video CreateModel(Video model, MapperData mapper)
        {
            var result = new Video
            {
                Id = model.Id,
                Basename = model.Basename,
                Filename = model.Filename,
                Path = model.Path,
                Resolution = model.Resolution,
                Nc = model.Nc,
                Subbed = model.Subbed,
                Lyrics = model.Lyrics,
                Uncen = model.Uncen,
                Source = model.Source,
                Overlap = model.Overlap
            };
            return result;
        }
    }
}