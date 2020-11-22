using System.Threading.Tasks;
using AnimeTheme.Service.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AnimeTheme.Service.Services.Seeders
{
    public class DatabaseSeeder
    {
        private readonly ILogger<DatabaseSeeder> _logger;
        
        private readonly VideoTagsSeeder _videoTagsSeeder;
        private readonly AnimeSeeder _animeSeeder;
        private readonly AnimeResourceSeeder _animeResourceSeeder;
        private readonly AnimeSeasonSeeder _animeSeasonSeeder;
        private readonly AnimeThemeSeeder _animeThemeSeeder;
        private readonly ArtistSeeder _artistSeeder;
        private readonly SeriesSeeder _seriesSeeder;
        private readonly ArtistSongSeeder _artistSongSeeder;

        private readonly AnimeThemesContext _context;

        private readonly float _minDelay = 1f;
        private readonly float _maxDelay = 1.5f;
        
        public DatabaseSeeder(ILogger<DatabaseSeeder> logger,
            AnimeThemesContext context,
            VideoTagsSeeder videoTagsSeeder, 
            AnimeSeeder animeSeeder, 
            AnimeResourceSeeder animeResourceSeeder, 
            AnimeSeasonSeeder animeSeasonSeeder, 
            AnimeThemeSeeder animeThemeSeeder, 
            ArtistSeeder artistSeeder, 
            SeriesSeeder seriesSeeder, 
            ArtistSongSeeder artistSongSeeder)
        {
            _logger = logger;
            _context = context;
            
            _videoTagsSeeder = videoTagsSeeder;
            _animeSeeder = animeSeeder;
            _animeResourceSeeder = animeResourceSeeder;
            _animeSeasonSeeder = animeSeasonSeeder;
            _animeThemeSeeder = animeThemeSeeder;
            _artistSeeder = artistSeeder;
            _seriesSeeder = seriesSeeder;
            _artistSongSeeder = artistSongSeeder;
        }
        
        public async Task Run()
        {
            _logger.LogInformation("Seeding all databases.");
            
            await _videoTagsSeeder.Run(_minDelay, _maxDelay);
            await _animeSeeder.Run();
            await _animeResourceSeeder.Run(_minDelay, _maxDelay);
            await _animeSeasonSeeder.Run(_minDelay, _maxDelay);
            await _animeThemeSeeder.Run(_minDelay, _maxDelay);
            await _artistSeeder.Run(_minDelay, _maxDelay);
            await _seriesSeeder.Run(_minDelay, _maxDelay);
            await _artistSongSeeder.Run(_minDelay, _maxDelay);

            _logger.LogInformation("Done Seeding all databases.");
        }
    }

    public static class DatabaseSeederExtensions
    {
        public static IServiceCollection AddDatabaseSeeder(this IServiceCollection services)
        {
            services.AddTransient<DatabaseSeeder>();
            
            services.AddTransient<VideoTagsSeeder>();
            services.AddTransient<AnimeSeeder>();
            services.AddTransient<AnimeResourceSeeder>();
            services.AddTransient<AnimeSeasonSeeder>();
            services.AddTransient<AnimeThemeSeeder>();
            services.AddTransient<ArtistSeeder>();
            services.AddTransient<ArtistSongSeeder>();
            services.AddTransient<SeriesSeeder>();
            return services;
        }
    }
}