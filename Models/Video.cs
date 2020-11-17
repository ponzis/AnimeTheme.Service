using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AnimeTheme.Service.Models
{
    public class Video
    {
        [Key] 
        [JsonIgnore]
        public int Id { get; set; }
        public bool Nc { get; set; }
        public int Resolution  { get; set; }
        public bool Subbed  { get; set; }
        public bool Lyrics  { get; set; }
        public bool Uncen  { get; set; }
        public VideoSource Source  { get; set; }
        public VideoOverlap Overlap  { get; set; }
        public string Basename  { get; set; }
        public string Filename { get; set; }
        public Uri Path  { get; set; }
        
        public class Config : BaseEntityTypeConfiguration<Video>
        {
        }
    }
}