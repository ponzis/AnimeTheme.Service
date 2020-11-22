using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AnimeTheme.Service.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AnimeTheme.Service.Data
{
    public class AnimeThemesContext : DbContext
    {
        private readonly ILogger<AnimeThemesContext> _logger;
        public DbSet<Video> Videos { get; init; }  
        
        public DbSet<Anime> Animes { get; init; }  
        
        public DbSet<ExternalResource> Resources { get; init; }

        public DbSet<Synonym> Synonyms { get; init; }
        
        public DbSet<Theme> Themes { get; init; }
        
        public DbSet<Song> Songs { get; init; }
        
        public DbSet<Entry> Entries { get; init; }
        public  DbSet<Artist> Artists { get; set; }
        public DbSet<Series> Series { get; set; }


        public AnimeThemesContext(DbContextOptions<AnimeThemesContext> options, ILogger<AnimeThemesContext> logger)
            : base(options)
        {
            _logger = logger;
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new Video.Config());
            modelBuilder.ApplyConfiguration(new Anime.Config());
            modelBuilder.ApplyConfiguration(new ExternalResource.Config());
            modelBuilder.ApplyConfiguration(new Synonym.Config());
            modelBuilder.ApplyConfiguration(new Theme.Config());
            modelBuilder.ApplyConfiguration(new Entry.Config());
            modelBuilder.ApplyConfiguration(new Song.Config());
            modelBuilder.ApplyConfiguration(new Artist.Config());
            modelBuilder.ApplyConfiguration(new Series.Config());
            base.OnModelCreating(modelBuilder);
        }
        
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateDateTime();
            return await base.SaveChangesAsync(true, cancellationToken);
        }
        
        public override int SaveChanges()
        {
            UpdateDateTime();
            return base.SaveChanges();
        }

        private void UpdateDateTime()
        {
            foreach(var entry in ChangeTracker.Entries()
                .Where(e => e.Properties.Any(p => p.Metadata.Name == "UpdateDateTime") && e.State == EntityState.Modified))
            {
                entry.Property("UpdateDateTime").CurrentValue = DateTime.Now;
            }
        }
    }
}