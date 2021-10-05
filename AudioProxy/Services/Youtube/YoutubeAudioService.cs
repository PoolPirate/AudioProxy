using AudioProxy.Audio;
using Common.Services;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace AudioProxy.Services
{
    /// <summary>
    /// Used to created IAudioStream instances for playing youtube video sound.
    /// This uses YoutubeExplode internally to load the data.
    /// </summary>
    public sealed class YoutubeAudioService : Service
    {
        private const long YoutubeSampleSize = 24;

        [Inject]
        private readonly YoutubeClient YoutubeClient;

        public async Task<IAudioStream> GetAudioStreamAsync(string url)
        {
            var video = await YoutubeClient.Videos.GetAsync(url);
            var manifest = await YoutubeClient.Videos.Streams.GetManifestAsync(url);

            var streamInfo = manifest.GetAudioOnlyStreams().TryGetWithHighestBitrate();

            if (streamInfo is null)
            {
                return null;
            }

            Logger.LogDebug($"Loading youtube stream: {video.Title} {streamInfo.Bitrate.BitsPerSecond / YoutubeSampleSize} Samples / sec from {streamInfo.Url}");
            var stream = await YoutubeClient.Videos.Streams.GetAsync(streamInfo);

            return null;
            //return new RawAudioStream(stream, streamInfo.Bitrate.BitsPerSecond / YoutubeSampleSize); 
        }
    }
}
