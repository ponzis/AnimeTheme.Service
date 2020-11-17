using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnimeTheme.Service.Data;
using AnimeTheme.Service.Models;
using AnimeTheme.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AnimeTheme.Service.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("[controller]")]
    public class AnimeThemeController : ControllerBase
    {
       

        private readonly ILogger<AnimeThemeController> _logger;
        private readonly DatabaseSeeder _client;
        private readonly AnimeThemesContext _context;

        public AnimeThemeController(ILogger<AnimeThemeController> logger, AnimeThemesContext context, DatabaseSeeder client)
        {
            _logger = logger;
            _client = client;
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<Video>> Get()
        {
            return await _context.Videos.Take(10).ToListAsync();
        }
        
        [HttpGet("SeedAllDatabases")]
        public async Task<bool> SeedAllDatabasesGet()
        {
            await _client.Run();
            return true;
        }

        
        [HttpGet("Artist/{slug?}")]
        public async Task<Artist> ArtistPathGet(string slug)
        {
            var artist = await _context.Artists
                .AsSingleQuery()
                .Include(c=>c.Songs)
                .Include(c=>c.Resource)
                .FirstAsync(c => c.Slug == slug);
            return artist.ShallowClone();
        }
        

        [HttpGet("Anime/{slug?}")]
        public async Task<Anime> AnimePathGet(string slug)
        {
            var anime = await _context.Animes
                .AsSingleQuery()
                .Include(c=>c.Synonyms)
                .Include(c=>c.Resource)
                .Include(c=>c.Themes)
                .Include(c=>c.Series)
                .FirstAsync(c => c.Slug == slug);
            return anime.ShallowClone();
        }
        
        
        [HttpGet("Anime")]
        public async Task<Anime> AnimeGet(string name)
        {
            var anime = await _context.Animes
                .AsSingleQuery()
                .Include(c=>c.Synonyms)
                .Include(c=>c.Resource)
                .Include(c=>c.Themes)
                .ThenInclude(c=>c.Song)
                .Include(c=>c.Series)
                .FirstAsync(c => c.Name == name || c.Slug == name || c.Synonyms.Any(p=>p.Text == name));

            return anime.ShallowClone();
        }

        [HttpGet("Anime/Random")]
        public async Task<IEnumerable<Anime>> RandomAnimeGet(int count = 1)
        {
            var randomList = await _context.Animes
                .AsSingleQuery()
                .Include(c=>c.Synonyms)
                .Include(c=>c.Resource)
                .Include(c=>c.Themes)
                .Include(c=>c.Series)
                .OrderBy(c => Guid.NewGuid())
                .Take(count).ToListAsync();

            return randomList.ConvertAll(c=>c.ShallowClone());
        }
    }
}