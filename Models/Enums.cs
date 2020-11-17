using System;
using System.Text.Json.Serialization;

namespace AnimeTheme.Service.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AnimeSeason
    {
        FALL, 
        SUMMER,
        SPRING,
        WINTER
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ThemeType
    {
        OP,
        ED
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum InvitationStatus
    {
        OPEN,
        CLOSED
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum VideoOverlap
    {
        NONE,
        TRANS,
        OVER
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum VideoSource
    {
        WEB,
        RAW,
        BD,
        DVD,
        VHS
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ResourceSite
    {
        OFFICIAL_SITE,
        TWITTER,
        ANIDB,
        ANILIST,
        ANIME_PLANET,
        ANN,
        KITSU,
        MAL,
        WIKI,
        
        NONE
    }

    public static class ResourceSiteExtensions
    {
        public static string GetDomain(this ResourceSite site)
        {
            return site switch
            {
                ResourceSite.TWITTER => "twitter.com",
                ResourceSite.ANIDB => "anidb.net",
                ResourceSite.ANILIST => "anilist.co",
                ResourceSite.ANIME_PLANET => "anime-planet.com",
                ResourceSite.ANN => "animenewsnetwork.com",
                ResourceSite.KITSU => "kitsu.io",
                ResourceSite.MAL => "myanimelist.net",
               ResourceSite.WIKI =>  "wikipedia.org",
                _ => null
            };
        }

        public static ResourceSite ValueOf(string link)
        {
            var parsedHost = new Uri(link);
            foreach (var value in Enum.GetValues<ResourceSite>())
            {
                if (parsedHost.Host == value.GetDomain())
                {
                    return value;
                }
            }
            return ResourceSite.NONE;
        }
    }
    
}