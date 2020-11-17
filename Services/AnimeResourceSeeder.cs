using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using AnimeTheme.Service.Data;
using AnimeTheme.Service.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AnimeTheme.Service.Services
{
    public class AnimeResourceSeeder
    {
        private static readonly string[] YearPages =
        {
            "https://www.reddit.com/r/AnimeThemes/wiki/60s.json",
            "https://www.reddit.com/r/AnimeThemes/wiki/70s.json",
            "https://www.reddit.com/r/AnimeThemes/wiki/80s.json",
            "https://www.reddit.com/r/AnimeThemes/wiki/90s.json",
            "https://www.reddit.com/r/AnimeThemes/wiki/2000.json",
            "https://www.reddit.com/r/AnimeThemes/wiki/2001.json",
            "https://www.reddit.com/r/AnimeThemes/wiki/2002.json",
            "https://www.reddit.com/r/AnimeThemes/wiki/2003.json",
            "https://www.reddit.com/r/AnimeThemes/wiki/2004.json",
            "https://www.reddit.com/r/AnimeThemes/wiki/2005.json",
            "https://www.reddit.com/r/AnimeThemes/wiki/2006.json",
            "https://www.reddit.com/r/AnimeThemes/wiki/2007.json",
            "https://www.reddit.com/r/AnimeThemes/wiki/2008.json",
            "https://www.reddit.com/r/AnimeThemes/wiki/2009.json",
            "https://www.reddit.com/r/AnimeThemes/wiki/2010.json",
            "https://www.reddit.com/r/AnimeThemes/wiki/2011.json",
            "https://www.reddit.com/r/AnimeThemes/wiki/2012.json",
            "https://www.reddit.com/r/AnimeThemes/wiki/2013.json",
            "https://www.reddit.com/r/AnimeThemes/wiki/2014.json",
            "https://www.reddit.com/r/AnimeThemes/wiki/2015.json",
            "https://www.reddit.com/r/AnimeThemes/wiki/2016.json",
            "https://www.reddit.com/r/AnimeThemes/wiki/2017.json",
            "https://www.reddit.com/r/AnimeThemes/wiki/2018.json",
            "https://www.reddit.com/r/AnimeThemes/wiki/2019.json",
            "https://www.reddit.com/r/AnimeThemes/wiki/2020.json"
        };
        
        private readonly ILogger<AnimeResourceSeeder> _logger;
        private readonly HttpClient _httpClient;

        private readonly AnimeThemesContext _context;
        
        private readonly Random _random = new();
        
        public AnimeResourceSeeder(ILogger<AnimeResourceSeeder> logger, AnimeThemesContext context, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
            _context = context;
        }


        public async Task Run(float minDelay = 5, float maxDelay = 15)
        {
            _logger.LogInformation("Seeding AnimeResourceSeeder data.");
            foreach (var yearPage in YearPages)
            {
                _logger.LogInformation($"Working on {yearPage}.");
                var ob = await _httpClient.GetJsonObject<RedditDataWrapper>(new Uri(yearPage));
                var data = HttpUtility.HtmlDecode(ob.Data.ContentMd);
                await ParseData(data);
                await Task.Delay(TimeSpan.FromSeconds(_random.Range(minDelay, maxDelay)));
            }
            await _context.SaveChangesAsync();
            _logger.LogInformation("Done Seeding AnimeResourceSeeder data.");
        }


        private async Task ParseData(string data)
        {
            var animeResourceWikiEntries =
                Regex.Matches(data, @"###\[(.*)\]\((https\:\/\/.*)\)", RegexOptions.Multiline);
            foreach (Match animeResourceWikiEntry in animeResourceWikiEntries)
            {
                var animeName = animeResourceWikiEntry.Groups[1].Value;
                var resourceLink = animeResourceWikiEntry.Groups[2].Value;

                var resource = await _context.Resources.FirstOrDefaultAsync(c => c.Link == resourceLink);
                if (resource == null)
                {
                    resource = new ExternalResource();
                    await _context.AddAsync(resource);
                }

                resource.Site = ResourceSiteExtensions.ValueOf(resourceLink);
                resource.Link = resourceLink;
                var resourceAnime = await _context.Animes.FirstOrDefaultAsync(c => c.Name == animeName);
                if (resourceAnime != null)
                {
                    resourceAnime.Resource = resource;
                }
            }
        }
    }
}