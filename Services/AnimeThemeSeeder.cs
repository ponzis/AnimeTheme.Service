
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
    public class AnimeThemeSeeder
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
            "https://www.reddit.com/r/AnimeThemes/wiki/2020.json",
        };

        private readonly ILogger<AnimeThemeSeeder> _logger;
        private readonly HttpClient _httpClient;
        private readonly AnimeThemesContext _context;

        private readonly Random _random = new();

        public AnimeThemeSeeder(ILogger<AnimeThemeSeeder> logger, AnimeThemesContext context, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
            _context = context;
        }

        public async Task Run(float minDelay = 5, float maxDelay = 15)
        {
            _logger.LogInformation("Seeding AnimeThemeSeeder data.");
            foreach (var yearPage in YearPages)
            {
                _logger.LogInformation($"Working on {yearPage}.");
                var ob = await _httpClient.GetJsonObject<RedditDataWrapper>(new Uri(yearPage));
                var data = HttpUtility.HtmlDecode(ob.Data.ContentMd);
                await ParseData(data);
                await Task.Delay(TimeSpan.FromSeconds(_random.Range(minDelay, maxDelay)));
            }
            await _context.SaveChangesAsync();
            _logger.LogInformation("Done Seeding AnimeThemeSeeder data.");
        }
        
        private async Task ParseData(string data)
        {
            int? year = null;
            AnimeSeason? season = null;
            
            string group = null;
            
            Anime anime = null;
            Theme theme = null;
            Entry entry = null;
            
            var animeThemeWikiEntries = Regex.Matches(data, @"^(.*)$", RegexOptions.Multiline | RegexOptions.Compiled);
            foreach (Match animeThemeWikiEntry in animeThemeWikiEntries)
            {
                var wikiEntryLine = animeThemeWikiEntry.Value;
                
                var animeSeason = Regex.Match(wikiEntryLine, @"^##(\d+).*(Fall|Summer|Spring|Winter).*(?:\r)?$");
                if (animeSeason.Success)
                {
                    year = int.Parse(animeSeason.Groups[1].Value);
                    if (Enum.TryParse<AnimeSeason>(animeSeason.Groups[2].Value.ToUpper(), out var seasonValue))
                        season = seasonValue;
                    continue;
                }

                var animeName = Regex.Match(wikiEntryLine, @"^###\[(.*)\]\(https\:\/\/.*\)(?:\r)?$");
                if (animeName.Success)
                {
                    var name = animeName.Groups[1].Value;
                    var matchingAnime = _context.Animes.Where(c => c.Name == name);
                    if (year != null) matchingAnime = matchingAnime.Where(c => c.Year == year);
                    if (season != null) matchingAnime = matchingAnime.Where(c => c.Season == season);
                    
                    anime = await matchingAnime.AsSingleQuery().Include(p => p.Synonyms).Include(p => p.Themes).FirstOrDefaultAsync();
                    group = null;
                    theme = null;
                    entry = null;
                    continue;
                }

                if (anime != null)
                {
                    var synonymLine = Regex.Match(wikiEntryLine, @"^\*\*(.*)\*\*(?:\r)?$");
                    if (synonymLine.Success)
                    {
                        var synonyms = synonymLine.Groups[1].Value;
                        var synonymList = Regex.Matches(synonyms, @"(\?|""([^""]+)""|([^,]+))(?:, )?");
                        foreach (Match synonym in synonymList)
                        {
                            var synonymText = synonym.Groups[1].Value;
                            if (anime.Synonyms.Any(c => c.Text == synonymText)) continue;
                            var animeSynonym = new AnimeSynonym {Text = synonymText};
                            anime.Synonyms.Add(animeSynonym);
                        }
                    }
 
                    var groupName = Regex.Match(wikiEntryLine, @"^([a-zA-Z0-9- ]+)(?:\r)?$");
                    if (groupName.Success)
                    {
                        var groupText = groupName.Groups[1].Value.Trim();
                        if (!string.IsNullOrWhiteSpace(groupText))
                        {
                            group = groupText;
                        }
                        theme = null;
                        entry = null;
                        continue;
                    }

                    var themeMatch = Regex.Match(wikiEntryLine,
                        @"^(OP|ED)(\d*)(?:\sV(\d*))?.*\""(.*)\"".*\|\[Webm.*\]\(https\:\/\/animethemes\.moe\/video\/(.*)\)\|(.*)\|(.*)(?:\r)?$");
                    if (themeMatch.Success)
                    {
                        var themeType = themeMatch.Groups[1].Value;
                        var sequence = themeMatch.Groups[2].Value;
                        var version = themeMatch.Groups[3].Value;
                        var songTitle = themeMatch.Groups[4].Value;
                        var videoBasename = themeMatch.Groups[5].Value;
                        var episodes = themeMatch.Groups[6].Value;
                        var notes = themeMatch.Groups[7].Value.Trim();

                        if (!int.TryParse(version, out var value) || value == 1)
                        {

                            var song = await _context.Songs.FirstOrDefaultAsync(c => c.Title == songTitle);
                            if (song == null)
                            {
                                song = new Song {Title = songTitle};
                                await _context.AddAsync(song);
                            }

                            theme = await _context.Themes.FirstOrDefaultAsync(c=>c.Song.Id == song.Id && c.Anime.Id == anime.Id);
                            if (theme == null)
                            {
                                theme = new Theme();
                                await _context.AddAsync(theme); 
                            }

                            theme.Group = group;
                            theme.Type = Enum.Parse<ThemeType>(themeType.ToUpper());
                            if (int.TryParse(sequence, out var sequenceValue))
                            {
                                theme.Sequence = sequenceValue;
                            }

                            theme.Anime = anime;
                            theme.Song = song;

                            entry = await create_entry(version, episodes, notes, theme);
                            await attach_video_to_entry(videoBasename, entry);
                        }

                        if (theme != null && int.TryParse(version, out var value2) && value2 > 1)
                        {
                            entry = await create_entry(version, episodes, notes, theme);
                            await attach_video_to_entry(videoBasename, entry);
                        }
                        continue;
                    }
                }

                if (entry != null)
                {
                    var videoName = Regex.Match(wikiEntryLine, @"^\|\|\[Webm.*\]\(https\:\/\/animethemes\.moe\/video\/(.*)\)\|\|(?:\r)?$");
                    if (videoName.Success)
                    {
                        var videoBasename = videoName.Groups[1].Value;
                        await attach_video_to_entry(videoBasename, entry);
                    }
                }
            }
        }

        private async Task attach_video_to_entry(string videoBasename, Entry entry)
        {
            var video = await _context.Videos.FirstOrDefaultAsync(c=>c.Basename == videoBasename);
            if (video != null)
            {
                entry.Videos.Add(video); 
            }
        }

        private async Task<Entry> create_entry(string version, string episodes, string notes, Theme theme)
        {
            var entry = await _context.Entries.Include(p => p.Videos).FirstOrDefaultAsync(c => c.Theme.Id == theme.Id);
            if (entry == null)
            {
                entry = new Entry()
                {
                    Videos = new List<Video>()
                };
                await _context.AddAsync(entry);
            }
            if (int.TryParse(version, out var value))
            {
                entry.Version = value;
            }

            entry.Episodes = episodes;
            
            if (notes.ToUpper().Contains("NSFW")) {
                entry.Nsfw = true;
            }
            
            if (notes.ToUpper().Contains("SPOILER")) {
                entry.Spoiler = true;
            }
            
            entry.Notes = Regex.Replace(notes, @"(?:NSFW)?(?:,\s)?(?:Spoiler)?", "");
            entry.Theme = theme;
            
            return entry;
        }
    }
}