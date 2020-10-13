using System;
using System.Collections.Generic;

namespace AudioProxy.Models
{
    public class Profile
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<ProfileSound> Sounds { get; set; } = new List<ProfileSound>();

        public Profile(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
            Sounds = new List<ProfileSound>();
        }
        public Profile()
        {
        }
    }
}
