using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnimeTheme.Service.Models;
using AnimeTheme.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AnimeTheme.Service.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("[controller]")]
    public class AnimeThemeController : ControllerBase
    {
       

        private readonly ILogger<AnimeThemeController> _logger;
        private readonly IAnimeThemeService _animeThemeService;
        private readonly AnimeMapper _animeMapper;
        private readonly ArtistMapper _artistMapper;

        public AnimeThemeController(ILogger<AnimeThemeController> logger, IAnimeThemeService animeThemeService, ModelMapperFactory mapperFactory)
        {
            _logger = logger;
            _animeThemeService = animeThemeService;
            
            _artistMapper = mapperFactory.GetArtistMapper();
            _animeMapper = mapperFactory.GetAnimeMapper();
        }

        [HttpGet]
        public async Task<IEnumerable<Video>> Get()
        {
            return await _animeThemeService.GetVideosAsync();
        }

        [HttpGet("Artist/{slug?}")]
        public async Task<Artist> ArtistPathGet(string slug)
        {
            var artist = await _animeThemeService.GetArtistBySlugAsync(slug);
            return _artistMapper.Get(artist);
        }
        

        [HttpGet("Anime/{slug?}")]
        public async Task<Anime> AnimePathGet(string slug)
        {
            var anime = await _animeThemeService.GetAnimeBySlugAsync(slug);
            return _animeMapper.Get(anime);
        }
        
        
        [HttpGet("Anime")]
        public async Task<Anime> AnimeGet(string name)
        {
            var anime = await _animeThemeService.GetAnimeByNameAsync(name);
            return _animeMapper.Get(anime);
        }

        [HttpGet("Anime/Random")]
        public async Task<IEnumerable<Anime>> RandomAnimeGet(int count = 1)
        {
            var randomList = await _animeThemeService.GetRandomAnimesAsync(count);
            return _animeMapper.Get(randomList);
        }
    }
}