using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ty_mod_manager
{
    public class TyLevel
    {
        public Dictionary<string, string> LanguageNames { get; } = null;

        public float X { get; set; } = 0f;
        public float Y { get; set; } = 0f;
        public float Z { get; set; } = 0f;

        public string InputPath { get; } = null;

        public TyLevel(string path)
        {
            InputPath = path;

            LanguageNames = new Dictionary<string, string>()
            {
                { "english", "Unnamed Custom Map" },
                { "german", "Unnamed Custom Map"},
                { "spanish", "Unnamed Custom Map"},
                { "french", "Unnamed Custom Map"},
                { "italian", "Unnamed Custom Map"}
            };
        }

    }
}
