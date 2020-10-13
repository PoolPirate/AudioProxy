using Common.Configuration;

namespace AudioProxy.Options
{
    public sealed class AudioOptions : Option
    {
        public int SampleRate { get; set; } = 44100;
        public int ChannelCount { get; set; } = 2;

        public int OutputDelay { get; set; } = 150;
        public int InputDelay { get; set; } = 75;

        public AudioOptions()
        {
        }
    }
}
