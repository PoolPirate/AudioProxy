using AudioProxy.Options;
using Common.Services;

namespace AudioProxy.Services
{
    public sealed class AudioService : Service
    {
        [Inject] private readonly AudioOptions AudioOptions;

        public int GetSampleRate()
        {
            lock (AudioOptions)
            {
                return AudioOptions.SampleRate;
            }
        }
        public int GetChannelCount()
        {
            lock (AudioOptions)
            {
                return AudioOptions.ChannelCount;
            }
        }
        public int GetInputDelay()
        {
            lock (AudioOptions)
            {
                return AudioOptions.InputDelay;
            }
        }
        public int GetOutputDelay()
        {
            lock (AudioOptions)
            {
                return AudioOptions.OutputDelay;
            }
        }

        public void UpdateSampleRate(int sampleRate)
        {
            lock (AudioOptions)
            {
                AudioOptions.SampleRate = sampleRate;
            }
        }
        public void UpdateChannelCount(int channelCount)
        {
            lock (AudioOptions)
            {
                AudioOptions.ChannelCount = channelCount;
            }
        }
        public void UpdateInputDelay(int inputDelay)
        {
            lock (AudioOptions)
            {
                AudioOptions.InputDelay = inputDelay;
            }
        }
        public void UpdateOutputDelay(int outputDelay)
        {
            lock (AudioOptions)
            {
                AudioOptions.OutputDelay = outputDelay;
            }
        }
    }
}
