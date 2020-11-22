using System;
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
    public class AnimeSeasonSeeder
    {
        private static readonly string[] YearPages =
        {
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
        
        private readonly ILogger<AnimeSeasonSeeder> _logger;
        private readonly HttpClient _httpClient;

        private readonly AnimeThemesContext _context;
        
        private readonly Random _random = new();
        
        public AnimeSeasonSeeder(ILogger<AnimeSeasonSeeder> logger, AnimeThemesContext context, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
            _context = context;
        }


        public async Task Run(float minDelay = 5, float maxDelay = 15)
        {
            _logger.LogInformation("Seeding AnimeSeasonSeeder data.");
            foreach (var yearPage in YearPages)
            {
                _logger.LogInformation($"Working on {yearPage}.");
                var ob = await _httpClient.GetJsonObjectAsync<RedditDataWrapper>(new Uri(yearPage));
                var data = HttpUtility.HtmlDecode(ob.Data.ContentMd);
                await ParseData(data);
                await Task.Delay(TimeSpan.FromSeconds(_random.Range(minDelay, maxDelay)));
            }
            await _context.SaveChangesAsync();
            _logger.LogInformation("Done Seeding AnimeSeasonSeeder data.");
        }
        
        
        private async Task ParseData(string data)
        {
            int year = default;
            AnimeSeason? season = null;
            
            var animeSeasonWikiEntries  = Regex.Matches(data ,@"^(.*)$", RegexOptions.Multiline | RegexOptions.Compiled);
            foreach (Match animeSeasonWikiEntry  in animeSeasonWikiEntries )
            {
                var wikiEntryLine = animeSeasonWikiEntry.Value;
                
                var animeSeason = Regex.Match(wikiEntryLine, @"^##(\d+).*(Fall|Summer|Spring|Winter).*(?:\r)?$");
                if (animeSeason.Success)
                {
                    var yearString = animeSeason.Groups[1].Value;
                    var seasonString = animeSeason.Groups[2].Value.ToUpper();
                    year = int.Parse(yearString);
                    if (Enum.TryParse<AnimeSeason>(seasonString, out var seasonValue))
                        season = seasonValue;
                    continue;
                }
                
                var animeName = Regex.Match(wikiEntryLine, @"###\[(.*)\]\(https\:\/\/.*\)");
                if (!animeName.Success) continue;
                var name = animeName.Groups[1].Value;
                var anime = await _context.Animes.FirstOrDefaultAsync(c => c.Name == name && c.Year == year);
                if (anime != null)
                {
                    anime.Season = season;
                }
            }
        }
    }
}
