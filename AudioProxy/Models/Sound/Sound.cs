using System;

namespace AudioProxy.Models
{
    public class Sound
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public SoundSource Source { get; set; }

        /// <summary>
        /// Absolute Path for File Source.
        /// Video Id for Youtube Source.
        /// </summary>
        public string Path { get; set; }

        public Sound(string name, SoundSource source, string path)
        {
            Id = Guid.NewGuid();
            Name = name;
            Source = source;
            Path = path;
        }
        public Sound(SoundSource source)
        {
            Source = source;
        }
        public Sound()
        {
        }
    }
}
