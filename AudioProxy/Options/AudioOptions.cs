using Common.Configuration;
using NAudio.Wave;

namespace AudioProxy.Options
{
    public sealed class AudioOptions : Option
    {
        public int SampleRate { get; set; } = 48000;
        public int ChannelCount { get; set; } = 2;

        public int OutputDelay { get; set; } = 150;
        public int InputDelay { get; set; } = 75;

        public ushort ResamplingQuality { get; set; } = 60;

        public AudioOptions()
        {
        }
    }
}
