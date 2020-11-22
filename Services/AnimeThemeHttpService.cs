using System;
using System.Net.Http;
using AnimeTheme.Service.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AnimeTheme.Service.Services
{
    
    public interface IAnimeThemeHttpClient
    {
        public string ServiceName { get; }

        public SearchResult Search(string q, int limit, string[] fields);
        public SearchResultPage Anime(string[] include, int year, string[] sort, int pageSize, int pageNumber, string[] fields);
        public Anime Anime(string slug, string[] include, string[] fields);
        
        public int[] Year();
        public Year Year(string slug, string[] include, string[] fields);
        
        public AnnouncementPage Announcement(string[] sort, int pageSize, int pageNumber, string[] fields);
        
    }

    public class AnimeThemeHttpClientOptions
    {
        public Uri BaseAddress { get; init; } = new(@"https://animethemes.dev/api");
        public string ServiceName { get; init; }  = "animethemes.moe";
    }
    public class AnimeThemeHttpClient
    {
        private readonly ILogger<AnimeThemeHttpClient> _logger;
        private readonly HttpClient _httpClient;

       
        private AnimeThemeHttpClientOptions _options;

        AnimeThemeHttpClient(ILogger<AnimeThemeHttpClient> logger, HttpClient httpClient, IOptions<AnimeThemeHttpClientOptions> options)
        {
            _logger = logger;
            _options = options.Value;
            _httpClient = httpClient;
            _httpClient.BaseAddress = _options.BaseAddress;

        }
    }
}