using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AnimeTheme.Service.Models
{
    public abstract class ModelMapper
    {
        
    }
    public abstract class ModelMapper<T> : ModelMapper
    {
        private readonly MapperData _data;
        private readonly ILogger<ModelMapper<T>> _logger;
        protected readonly ModelMapperFactory _modelFactory;

        protected ModelMapper(ILogger<ModelMapper<T>> logger, ModelMapperFactory modelFactory, MapperData data)
        {
            _data = data;
            _modelFactory = modelFactory;
            _logger = logger;
        }
        

        protected abstract T CreateModel(T model, MapperData mapper);

        public T Get(T model)
        {
            if (model == null || _data.Contains(model)) return default;
            _data.Add(model);
            return CreateModel(model, _data);
        }
        
        public static string NewPath(string path, string input)
        {
            return string.Join('.', path, input.ToLower());
        }

        
        public ICollection<T> Get(ICollection<T> models)
        {
            return models.Any(k=> _data.Contains(k)) ? null : models.Select(Get).ToList().NullIfEmpty();
        }
        
        protected bool IsAllowedField(string[] fields, string path, string name)
        {
            if (fields == null) return true;
            path = NewPath(path, name);
            return ((IList) fields).Contains(path);
        } 
        
    }

    public static class AbstractFactoryExtensions
    {
        public static ICollection<T> NullIfEmpty<T>(this ICollection<T> collection)
        {
            return collection.IsNullOrEmpty() ? null : collection;
        }
        
        public static IServiceCollection AddModelMappers(this IServiceCollection services)
        {
            services.AddTransient<ModelMapperFactory>();
            services.AddTransient<MapperData>();
            return services;
        }
    }

    public class ModelMapperFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly MapperData _data;
        
        public ModelMapperFactory(ILoggerFactory loggerFactory, MapperData data)
        {
            _loggerFactory = loggerFactory;
            _data = data;
        }
        
        public SynonymMapper GetSynonymMapper()
        {
            var logger = _loggerFactory.CreateLogger<SynonymMapper>();
            return new SynonymMapper(logger, this, _data);
        }

        public AnimeMapper GetAnimeMapper()
        {
            var logger = _loggerFactory.CreateLogger<AnimeMapper>();
            return new AnimeMapper(logger, this, _data);
        }

        public ResourceMapper GetResourceMapper()
        {
            var logger = _loggerFactory.CreateLogger<ResourceMapper>();
            return new ResourceMapper(logger, this, _data);
        }

        public ArtistMapper GetArtistMapper()
        {
            var logger = _loggerFactory.CreateLogger<ArtistMapper>();
            return new ArtistMapper(logger, this, _data);
        }

        public SeriesMapper GetSeriesMapper()
        {
            var logger = _loggerFactory.CreateLogger<SeriesMapper>();
            return new SeriesMapper(logger, this, _data);
        }

        public ThemeMapper GetThemeMapper()
        {
            var logger = _loggerFactory.CreateLogger<ThemeMapper>();
            return new ThemeMapper(logger, this, _data);
        }

        public SongMapper GetSongMapper()
        {
            var logger = _loggerFactory.CreateLogger<SongMapper>();
            return new SongMapper(logger, this, _data);
        }

        public VideoMapper GetVideoMapper()
        {
            var logger = _loggerFactory.CreateLogger<VideoMapper>();
            return new VideoMapper(logger, this, _data);
        }
    }
    

    public class MapperData
    {
        private readonly ICollection<object> _visited;

        public MapperData()
        {
            _visited = new HashSet<object>();
        }

        public bool Contains<T>(T model)
        {
           return _visited.Contains(model);
        }

        public void Add<T>(T model)
        {
            _visited.Add(model);
        }
    }
}