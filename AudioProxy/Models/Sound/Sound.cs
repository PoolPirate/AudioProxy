using System;

namespace AudioProxy.Models
{
    public class Sound
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public SoundSource Source { get; set; }
        public string Path { get; set; }

        public Sound(string name, SoundSource source, string path)
        {
            Id = Guid.NewGuid();
            Name = name;
            Source = source;
            Path = path;
        }
        public Sound()
        {
        }
    }
}
