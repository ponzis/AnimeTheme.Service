using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using AnimeTheme.Service.Data;
using AnimeTheme.Service.Models;
using AnimeTheme.Service.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AnimeTheme.Service.Services
{
    public class AnimeSeeder
    {
        private static readonly string Page = "https://old.reddit.com/r/AnimeThemes/wiki/anime_index.json";
        
        private readonly ILogger<AnimeSeeder> _logger;
        private readonly HttpClient _httpClient;

        private readonly AnimeThemesContext _context;
        
        public AnimeSeeder(ILogger<AnimeSeeder> logger, AnimeThemesContext context, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
            _context = context;
        }


        public async Task Run()
        {
            _logger.LogInformation("Seeding AnimeSeeder data.");

            var slugs = await _context.Animes.ToListAsync();
            _slugs.UnionWith(slugs);
            
            _logger.LogInformation($"Working on {Page}.");
            var ob = await _httpClient.GetJsonObject<RedditDataWrapper>(new Uri(Page));
            var data = HttpUtility.HtmlDecode(ob.Data.ContentMd);
            await ParseData(data);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Done Seeding AnimeSeeder data.");
        }

        private readonly HashSet<Anime> _slugs = new();
        
        private async Task ParseData(string data)
        {
            var animeWikiEntries  = Regex.Matches(data ,@"\[(.*)\s\((.*)\)\]\(\/r\/AnimeThemes\/wiki\/.*\)", RegexOptions.Multiline | RegexOptions.Compiled);
            foreach (Match animeWikiEntry  in animeWikiEntries )
            {
                var animeName = animeWikiEntry.Groups[1].Value;
                var animeYear = animeWikiEntry.Groups[2].Value;
                
                if (animeYear.EndsWith('s')) animeYear = $"{19}{animeYear.TrimEnd('s')}";
                var year = int.Parse(animeYear);
                
                var anime = await _context.Animes.FirstOrDefaultAsync(c => c.Name == animeName && c.Year == year);
                if (anime == null)
                {
                    anime = new Anime();
                    await _context.AddAsync(anime);
                }
                
                var slug = anime.Slug ?? await GenerateSlug(animeName, animeYear);
                _slugs.Add(anime);
                
                anime.Name = animeName;
                anime.Slug = slug;
                anime.Year = year;
            }
        }

        private async Task<string> GenerateSlug(string animeName, string animeYear, string symbol = "_")
        {
            var slug = animeName.GenerateSlug(symbol);
            if (_slugs.Any(c => c.Slug == slug))
            {
                slug = $"{animeName} {animeYear}".GenerateSlug(symbol);
                if (_slugs.Any(c => c.Slug == slug))
                {
                    var count = _slugs.Select(o => o.Name == animeName).Count() + 1;
                    slug = $"{animeName} {animeYear} {count}".GenerateSlug(symbol);
                }
            }
            return slug;
        }
    }
}