using System.Collections.Generic;
using System.Text.Json.Serialization;


namespace AnimeTheme.Service.Models
{
    public class SearchResult
    {
        [JsonPropertyName("Anime")] private ICollection<Anime> Animes { get; set; }
        [JsonPropertyName("Artists")] private ICollection<Artist> Artists { get; set; }
        [JsonPropertyName("entries")] private ICollection<Entry> Entries { get; set; }
        [JsonPropertyName("series")] private ICollection<Series> Series { get; set; }
        [JsonPropertyName("songs")] private ICollection<Song> Songs { get; set; }
        [JsonPropertyName("synonyms")] private ICollection<Synonym> Synonyms { get; set; }
        [JsonPropertyName("themes")] private ICollection<Theme> Themes { get; set; }
        [JsonPropertyName("videos")] private ICollection<Video> Videos { get; set; }
    }

    public class SearchResultPage : Page
    {
        
    }
    public class Page 
    {

    }
    
    public class AnnouncementPage : Page
    {
        
    }

    public class Announcement
    {
        private string Content { get; set; }
    }

    
    
    public class Year
    {
        [JsonPropertyName("winter")] private ICollection<Anime> Winter { get; set; }
        [JsonPropertyName("spring")] private ICollection<Anime> Spring { get; set; }
        [JsonPropertyName("summer")] private ICollection<Anime> Summer { get; set; }
        [JsonPropertyName("fall")] private ICollection<Anime> Fall { get; set; }
    }
    
}