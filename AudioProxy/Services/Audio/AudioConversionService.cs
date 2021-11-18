using Common.Services;
using NAudio.Wave;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace AudioProxy.Services
{
    /// <summary>
    /// A service used to convert audio streams to playable codecs using FFMPEG.
    /// </summary>
    public sealed class AudioConversionService : Service
    {
        private const string FFMPEGPath = "ffmpeg.exe";
        private const string FFMPEGOptions = ""; 

        protected override ValueTask InitializeAsync()
        {
            if (!File.Exists(FFMPEGPath))
            {
                throw new InvalidOperationException("Please add ffmpeg.exe to the root directory of this application!");
            }

            return ValueTask.CompletedTask;
        }


    }
}
