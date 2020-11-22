using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using AnimeTheme.Service.Data;
using AnimeTheme.Service.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AnimeTheme.Service.Services.Seeders
{
    public class ArtistSeeder
    {
        private static readonly string Page = "https://old.reddit.com/r/AnimeThemes/wiki/artist.json";
        
        private readonly ILogger<ArtistSeeder> _logger;
        private readonly HttpClient _httpClient;

        private readonly AnimeThemesContext _context;
        
        private readonly Random _random = new();
        
        public ArtistSeeder(ILogger<ArtistSeeder> logger, AnimeThemesContext context, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
            _context = context;
        }


        public async Task Run(float minDelay = 5, float maxDelay = 15)
        {
            _logger.LogInformation("Seeding ArtistSeeder data.");

            _logger.LogInformation($"Working on {Page}.");
            var ob = await _httpClient.GetJsonObjectAsync<RedditDataWrapper>(new Uri(Page));
            var data = HttpUtility.HtmlDecode(ob.Data.ContentMd);
            await ParseData(data, minDelay, maxDelay);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Done Seeding ArtistSeeder data.");
        }
        
        
        private async Task ParseData(string data, float minDelay, float maxDelay)
        {

            var artistWikiContentMd  = Regex.Matches(data ,@"\[(.*)\]\((\/r\/AnimeThemes\/wiki\/artist\/(.*))\)", RegexOptions.Multiline | RegexOptions.Compiled);
            foreach (Match artistWikiEntry  in artistWikiContentMd )
            {
                var artist_name  = artistWikiEntry.Groups[1].Value;
                var artist_link  = $"https://old.reddit.com{artistWikiEntry.Groups[2].Value}.json";
                var artist_slug = artistWikiEntry.Groups[3].Value;
                
                var artist = await _context.Artists.FirstOrDefaultAsync(c => c.Name == artist_name);
                if (artist == null)
                {
                    artist = new Artist();
                    await _context.AddAsync(artist);
                }
                artist.Name = artist_name;
                artist.Slug = artist_slug;
                
                await Task.Delay(TimeSpan.FromSeconds(_random.Range(minDelay, maxDelay)));
                var artistResourceWikiJson = await _httpClient.GetJsonObjectAsync<RedditDataWrapper>(new Uri(artist_link));
                var artistResourceWikiJsonData = HttpUtility.HtmlDecode(artistResourceWikiJson.Data.ContentMd);
                var artistResourceEntry = Regex.Match(artistResourceWikiJsonData, @"##\[.*\]\((https\:\/\/.*)\)", RegexOptions.Multiline);
                var artistResourceLink = artistResourceEntry.Groups[1].Value;

                var resource = await _context.Resources.Include(c => c.Artists)
                    .FirstOrDefaultAsync(c => c.Link == artistResourceLink);
                if(resource == null)
                {
                    resource = new ExternalResource
                    {
                        Site = ResourceSiteExtensions.ValueOf(artistResourceLink),
                        Link = artistResourceLink,
                        Artists = new List<Artist>()
                    };
                    await _context.AddAsync(resource);
                }
                resource.Artists.Add(artist);
            }
        }
    }
}