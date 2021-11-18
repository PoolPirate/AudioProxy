using AudioProxy.Audio;
using Common.Services;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using System.IO;
using YoutubeExplode.Converter;
using AudioProxy.Options;
using YoutubeExplode.Videos;
using NAudio.Wave;
using System.Linq;

namespace AudioProxy.Services
{
    /// <summary>
    /// Used to created IAudioStream instances for playing youtube video sound.
    /// This uses YoutubeExplode internally to load the data.
    /// </summary>
    public sealed class YoutubeAudioService : Service
    {
        private const long YoutubeSampleSize = 24;

        private const string FFMPEGPath = "ffmpeg.exe";

        [Inject]
        private readonly YoutubeClient YoutubeClient;
        [Inject]
        private readonly GeneralOptions GeneralOptions;

        public async Task<bool> IsValidVideoUrl(string url)
        {
            try
            {
                var video = await YoutubeClient.Videos.GetAsync(url);
                return video != null;
            }
            catch
            {
                return false;
            }
        }

        public async ValueTask<WaveStream> GetAudioStreamAsync(VideoId videoId)
        {
            string audioFilePath = GetFileNameForVideoId(videoId);

            if (!File.Exists(audioFilePath))
            {
                await DownloadYoutubeVideoAsync(videoId, audioFilePath);
            }

            return new Mp3FileReader(audioFilePath);
        }

        private async Task DownloadYoutubeVideoAsync(VideoId videoId, string audioFilePath)
        {
            var video = await YoutubeClient.Videos.GetAsync(videoId);
            var manifest = await YoutubeClient.Videos.Streams.GetManifestAsync(video.Id);
            var conversionRequest = MakeConversionRequest(audioFilePath);

            Logger.LogInformation($"Downloading Youtube Video : {video.Title}");

            await YoutubeClient.Videos.DownloadAsync(new IStreamInfo[] { manifest.GetAudioOnlyStreams().GetWithHighestBitrate() }, conversionRequest);
        }

        private ConversionRequest MakeConversionRequest(string outputPath)
        {
            return new ConversionRequest(FFMPEGPath, outputPath,
                new ConversionFormat("mp3"), ConversionPreset.Medium);
        }

        private string GetFileNameForVideoId(VideoId videoId)
        {
            return Path.Combine(GeneralOptions.YoutubeCacheLocation, videoId.Value + ".mp3");
        }
    }
}
