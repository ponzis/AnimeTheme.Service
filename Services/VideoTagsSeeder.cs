using System;
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
    public class VideoTagsSeeder
    {
        private static readonly string[] YearPages =
        {
            "https://www.reddit.com/r/AnimeThemes/wiki/misc.json",
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
            
        private readonly ILogger<VideoTagsSeeder> _logger;
        private readonly HttpClient _httpClient;

        private readonly AnimeThemesContext _context;

        private readonly Random _random = new();
        
        public VideoTagsSeeder(ILogger<VideoTagsSeeder> logger, AnimeThemesContext context, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
            _context = context;
        }


        public async Task Run(float minDelay = 5, float maxDelay = 15)
        {
            _logger.LogInformation("Seeding video_tage data.");
            foreach (var yearPage in YearPages)
            {
                _logger.LogInformation($"Working on {yearPage}.");
                var ob = await _httpClient.GetJsonObject<RedditDataWrapper>(new Uri(yearPage));
                var data = HttpUtility.HtmlDecode(ob.Data.ContentMd);
                await ParseData(data);
                await Task.Delay(TimeSpan.FromSeconds(_random.Range(minDelay, maxDelay)));
            }
            await _context.SaveChangesAsync();
            _logger.LogInformation("Done Seeding video_tage data.");
        }
        
        
        private async Task ParseData(string data)
        {
            var videoWikiEntries  = Regex.Matches(data ,@"\[Webm.*\((.*)\)\]\((https\:\/\/animethemes\.moe\/video\/.*)\)|\[Webm\]\((https\:\/\/animethemes\.moe\/video\/.*)\)", RegexOptions.Multiline | RegexOptions.Compiled);
            foreach (Match videoWikiEntry in videoWikiEntries )
            {
                var videoTagsString = videoWikiEntry.Groups[1].Value.ToUpper();
                var videoTags = Regex.Replace(videoTagsString, @"\s+", "").Split(',');
                var videoPathString = (string.IsNullOrWhiteSpace(videoWikiEntry.Groups[3].Value)
                    ? videoWikiEntry.Groups[2]
                    : videoWikiEntry.Groups[3]).Value;
            
                var videoPath = new Uri(videoPathString);
                var videoBasename = Path.GetFileName(videoPath.LocalPath);
                
                var video = await _context.Videos.FirstOrDefaultAsync(c => c.Basename == videoBasename);
                if (video == null)
                {
                    video = new Video();
                    await _context.AddAsync(video);
                }
                video.Basename = videoBasename;
                video.Filename = Path.GetFileNameWithoutExtension(videoBasename);
                
                video.Path = videoPath;
                
                video.Nc = videoTags.Contains("NC");
                video.Subbed = videoTags.Contains("SUBBED");
                video.Lyrics = videoTags.Contains("LYRICS");
                video.Uncen = videoTags.Contains("UNCEN");
                
                video.Resolution = 720;
                foreach (var videoTag in videoTags)
                {
                    if (!int.TryParse(videoTag, out int value)) continue;
                    video.Resolution = value;
                    break;
                }
                
                foreach (VideoSource sourceKey in Enum.GetValues(typeof(VideoSource)))
                {
                    if (!videoTags.Contains(sourceKey.ToString())) continue;
                    video.Source = sourceKey;
                    break;
                }
                
                var hasTransTag = videoTags.Contains(VideoOverlap.TRANS.ToString());
                var hasOverTag = videoTags.Contains(VideoOverlap.OVER.ToString());
                video.Overlap = hasTransTag switch
                {
                    true when !hasOverTag => VideoOverlap.TRANS,
                    false when hasOverTag => VideoOverlap.OVER,
                    _ => VideoOverlap.NONE
                };
            }
        }
    }
}