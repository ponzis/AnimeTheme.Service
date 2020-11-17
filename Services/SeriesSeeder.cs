using System;
using System.Collections.Generic;
using System.Linq;
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
    public class SeriesSeeder
    {
        private static readonly string Page = "https://old.reddit.com/r/AnimeThemes/wiki/series.json";
        
        private readonly ILogger<ArtistSeeder> _logger;
        private readonly HttpClient _httpClient;

        private readonly AnimeThemesContext _context;
        
        private readonly Random _random = new();
        
        public SeriesSeeder(ILogger<ArtistSeeder> logger, AnimeThemesContext context, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
            _context = context;
        }


        public async Task Run(float minDelay = 5, float maxDelay = 15)
        {
            _logger.LogInformation("Seeding ArtistSeeder data.");

            _logger.LogInformation($"Working on {Page}.");
            var ob = await _httpClient.GetJsonObject<RedditDataWrapper>(new Uri(Page));
            var data = HttpUtility.HtmlDecode(ob.Data.ContentMd);
            await ParseData(data, minDelay, maxDelay);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Done Seeding ArtistSeeder data.");
        }


        private async Task ParseData(string data, float minDelay, float maxDelay)
        {
            var seriesWikiEntries  = Regex.Matches(data , @"\[(.*)\]\((\/r\/AnimeThemes\/wiki\/series\/(.*))\)", RegexOptions.Multiline | RegexOptions.Compiled);
            foreach (Match seriesWikiEntry in seriesWikiEntries)
            {
                var seriesName = seriesWikiEntry.Groups[1].Value;
                var seriesFileName = seriesWikiEntry.Groups[2].Value;
                var seriesLink = $"https://old.reddit.com{seriesFileName}.json";
                var seriesSlug = seriesWikiEntry.Groups[3].Value;
                
                    var series = await _context.Series.FirstOrDefaultAsync(c => c.Name == seriesName);
                    if (series == null)
                    {
                        series = new Series
                        {
                            Animes = new List<Anime>()
                        };
                        await _context.AddAsync(series);
                    }

                    series.Name = seriesName;
                    series.Slug = seriesSlug;

                    await Task.Delay(TimeSpan.FromSeconds(_random.Range(minDelay, maxDelay)));

                    var seriesAnimeWikiJson = await _httpClient.GetJsonObject<RedditDataWrapper>(new Uri(seriesLink));
                    var seriesAnimeWikiJsonData = HttpUtility.HtmlDecode(seriesAnimeWikiJson.Data.ContentMd);
                    var seriesAnimeWikiEntries = Regex.Matches(seriesAnimeWikiJsonData, 
                        @"###\[(.*)\]\(https\:\/\/.*\)", RegexOptions.Multiline);

                    var seriesAnimeNames = seriesAnimeWikiEntries.Select(p => p.Groups[1].Value);
                    var animeSeries = await _context.Animes.Where(c => seriesAnimeNames.Contains(c.Name)).ToListAsync();
                    if (animeSeries == null) continue;
                    foreach (var anime in animeSeries)
                    {
                        anime.Series = series;
                    }
            }
        }
    }
}