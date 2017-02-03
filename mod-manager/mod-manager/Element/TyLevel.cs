using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TyModManager.Element
{
    public class TyLevel
    {
        public Dictionary<string, string> LanguageNames { get; } = null;
        public string Name { get; } = null;
        public string Authors { get; } = null;

        public float X { get; set; } = 0f;
        public float Y { get; set; } = 0f;
        public float Z { get; set; } = 0f;

        public string InputPath { get; } = null;

        public int IndexOffset { get; set; } = -1;

        
        private string _version = null;

        public TyLevel(string path, string name, string version, string authors)
        {
            Name = name;
            Authors = authors;
            _version = version;
            InputPath = path;

            LanguageNames = new Dictionary<string, string>()
            {
                { "english", "Unnamed Custom Map" },
                { "german", "Unnamed Custom Map"},
                { "spanish", "Unnamed Custom Map"},
                { "french", "Unnamed Custom Map"},
                { "italian", "Unnamed Custom Map"}
            };

            // Try and read the IndexOffset from the level load order
            IndexOffset = Program.Config.LevelOrder.FindIndex(x => x == ToString());
        }

        public override string ToString()
        {
            return (Name ?? "Unnamed Level") + " (" + (_version ?? String.Empty) + ";" + (Authors ?? String.Empty) + ")";
        }
    }
}
