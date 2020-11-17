using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AnimeTheme.Service.Services
{
    public static class HttpExtension
    {
        public static async Task<T> GetJsonObject<T>(this HttpClient client, Uri uri,
            CancellationToken cancellationToken = default)
        {
            var resp = await client.GetAsync(uri, cancellationToken);
            if (!resp.IsSuccessStatusCode) throw new HttpRequestException("Request could not be conducted.");
            await using var stream = await resp.Content.ReadAsStreamAsync(cancellationToken);
            var result = await JsonSerializer.DeserializeAsync<T>(stream, cancellationToken: cancellationToken);
            return result;
        }

        public static double Range(this Random random, double min, double max)
        {
            return random.NextDouble() * (max - min) + min;
        }
    }
}