using System;
using System.Linq;
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
    public class ArtistSongSeeder
    {
        private static readonly string Page = "https://old.reddit.com/r/AnimeThemes/wiki/artist.json";
        
        private readonly ILogger<ArtistSongSeeder> _logger;
        private readonly HttpClient _httpClient;

        private readonly AnimeThemesContext _context;
        
        private readonly Random _random = new();
        
        public ArtistSongSeeder(ILogger<ArtistSongSeeder> logger, AnimeThemesContext context, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
            _context = context;
        }
        
        public async Task Run(float minDelay = 5, float maxDelay = 15)
        {
            _logger.LogInformation("Seeding ArtistSongSeeder data.");

            _logger.LogInformation($"Working on {Page}.");
            var ob = await _httpClient.GetJsonObjectAsync<RedditDataWrapper>(new Uri(Page));
            var data = HttpUtility.HtmlDecode(ob.Data.ContentMd);
            await ParseData(data, minDelay, maxDelay);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Done Seeding ArtistSongSeeder data.");
           
        }
        
        
        private async Task ParseData(string data, float minDelay, float maxDelay)
        {

            
            var artistWikiEntries  = Regex.Matches(data ,@"\[(.*)\]\((\/r\/AnimeThemes\/wiki\/artist\/(.*))\)", RegexOptions.Multiline);
            foreach (Match artistWikiEntry  in artistWikiEntries )
            {
                var artistName  = artistWikiEntry.Groups[1].Value;
                var artistLinkString = artistWikiEntry.Groups[2].Value;
                var artistLink  = $"https://old.reddit.com{artistLinkString}.json";
                
                var artist = await _context.Artists.Include(c=>c.Songs).FirstOrDefaultAsync(c => c.Name == artistName);
                if(artist == null) continue;
                await Task.Delay(TimeSpan.FromSeconds(_random.Range(minDelay, maxDelay)));
                
                var artistSongWikiJson  = await _httpClient.GetJsonObjectAsync<RedditDataWrapper>(new Uri(artistLink));
                var artistSongWikiJsonData = HttpUtility.HtmlDecode(artistSongWikiJson.Data.ContentMd);
                var artistSongWikiEntries = Regex.Matches(artistSongWikiJsonData, @"^(.*)$", RegexOptions.Multiline | RegexOptions.Compiled);
                
                Anime anime = null;
                foreach (Match artistSongWikiEntry in artistSongWikiEntries)
                {
                    var wikiEntryLine = artistSongWikiEntry.Groups[0].Value;

                    var animeName = Regex.Match(wikiEntryLine, @"^###\[(.*)\]\(https\:\/\/.*\)(?:\r)?$");
                    if (animeName.Success)
                    {
                        var name = animeName.Groups[1].Value;
                        anime = await _context.Animes.FirstOrDefaultAsync(c=>c.Name == name);
                        continue;
                    }

                    if (anime != null)
                    {
                        var themeMatch = Regex.Match(wikiEntryLine, @"^(OP|ED)(\d*)(?:\sV(\d*))?.*\""(.*)\""(?:\sby\s(.*))?\|\[Webm.*\]\(https\:\/\/animethemes\.moe\/video\/(.*)\)\|(.*)\|(.*)(?:\r)?$");
                        if (themeMatch.Success)
                        {
                            var themeType = Enum.Parse<ThemeType>(themeMatch.Groups[1].Value.ToUpper());
                            var sequence = themeMatch.Groups[2].Value;
                            var version = themeMatch.Groups[3].Value;
                            if (!int.TryParse(version, out var value) || value == 1)
                            {
                                var query = _context.Themes.Where(c => c.Type == themeType && c.Anime.Id == anime.Id);
                                query = int.TryParse(sequence, out var sequenceValue) ? query.Where(c=>c.Sequence == sequenceValue) : query.Where(c=>c.Sequence == null);
                                
                                var theme  = await query.Include(c=>c.Song).FirstOrDefaultAsync();
                                if (theme == null) continue;
                                var song = theme.Song;
                                artist.Songs.Add(song);
                            }
                        }
                    }
                }
                await _context.SaveChangesAsync();
            }
        }
    }
}