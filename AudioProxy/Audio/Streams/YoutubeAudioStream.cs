using NAudio.Wave;
using System;
using System.IO;

namespace AudioProxy.Audio
{
    public sealed class YoutubeAudioStream : WaveStream
    {
        private readonly Stream RawAudioStream;
        private readonly Stream WaveStream;
        private readonly Func<long> PositionGetter;

        public override long Length { get; }

        public override long Position { get => PositionGetter.Invoke(); set => throw new NotSupportedException(); }

        public override WaveFormat WaveFormat { get; }

        public YoutubeAudioStream(Stream audioStream, string container)
        {
            RawAudioStream = audioStream;

            switch (container)
            {
                case "mp3":
                    var mp3Reader = new Mp3FileReader(audioStream);
                    WaveFormat = mp3Reader.WaveFormat;
                    Length = mp3Reader.Length;
                    PositionGetter = () => mp3Reader.Position;
                    WaveStream = mp3Reader;
                    break;
                default:
                    var mediaFoundationReader = new StreamMediaFoundationReader(audioStream);
                    WaveFormat = mediaFoundationReader.WaveFormat;
                    Length = mediaFoundationReader.Length;
                    PositionGetter = () => mediaFoundationReader.Position;
                    WaveStream = mediaFoundationReader;
                    break;
                    //default:
                    //    throw new NotSupportedException($"The audio coded {container} is not supported!");
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
            => WaveStream.Read(buffer, offset, count);

        protected override void Dispose(bool disposing)
        {
            RawAudioStream.Dispose();
            WaveStream.Dispose();
            base.Dispose(disposing);
        }
    }
}
