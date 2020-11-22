using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnimeTheme.Service.Data;
using AnimeTheme.Service.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AnimeTheme.Service.Services
{
    public interface IAnimeThemeService
    {
        Task<List<Video>> GetVideosAsync(int count = 5);
        Task<Artist> GetArtistBySlugAsync(string slug);
        Task<Anime> GetAnimeBySlugAsync(string slug);
        Task<Anime> GetAnimeByNameAsync(string name);
        Task<List<Anime>> GetRandomAnimesAsync(int count);
    }
    public class AnimeThemeService : IAnimeThemeService
    {
        private ILogger<AnimeThemeService> _logger;
        private AnimeThemesContext _context;


        public AnimeThemeService(ILogger<AnimeThemeService> logger, AnimeThemesContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<Anime> GetAnimeByNameAsync(string name)
        {
            var requestedExpands = new[] {"Themes", "Synonyms"};
            var anime = await FullAnimeIncludeQuery(requestedExpands)
                .FirstAsync(a => a.Name == name || a.Slug == name || a.Synonyms.Any(p => p.Text == name));
            return anime;
        }

        public async Task<List<Video>> GetVideosAsync(int count = 5)
        {
            return await _context.Videos.Take(count).ToListAsync();
        }

        public async Task<Anime> GetAnimeBySlugAsync(string slug)
        {
            var requestedExpands = new[] {"Themes", "Synonyms"};
            var anime = await FullAnimeIncludeQuery(requestedExpands)
                .FirstAsync(c => c.Slug == slug);
            return anime;
        }
        

        public async Task<Anime> GetDirect(string name)
        {
            var anime = await _context.Animes.FromSqlRaw("SELECT * FROM Animes WHERE Name like {0}", name).FirstAsync();
            return anime;
        }

        public async Task<Artist> GetArtistBySlugAsync(string slug)
        {
            var artist = await FullArtistIncludeQuery().FirstAsync(c => c.Slug == slug);
            
            return artist;
        }

        private readonly Random _random = new();
        public async Task<List<Anime>> GetRandomAnimesAsync(int count)
        {
            var ids = await _context.Animes.Select(a=>a.Id).ToListAsync();
            var selected = ids.Shuffle(_random).Take(count);
            return await _context.Animes.Where(a=>selected.Contains(a.Id)).ToListAsync();
        }
        private IQueryable<Artist> FullArtistIncludeQuery()
        {
            return _context.Artists.AsNoTracking();
        }
        
        private IQueryable<Anime> FullAnimeIncludeQuery(string[] requestedExpands)
        {
            var q = _context.Animes.AsSingleQuery();
            foreach (var rq in requestedExpands)
            {
                q = q.Include(rq);
            }
            return q;
        }
    }

    public static class ListExtensions
    {
        public static IList<T> Shuffle<T>(this IList<T> list, Random random)
        {
            for (var i = list.Count; i > 1; i--)
            {
                var j = random.Next(0, i);
                (list[i - 1], list[j]) = (list[j], list[i - 1]);
            }
            return list;
        }
    }
}