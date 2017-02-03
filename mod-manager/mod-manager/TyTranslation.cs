using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ty_mod_manager
{
    public class TyTranslation
    {

        public string Name { get; } = null;

        public Dictionary<string, string> Translations { get; set; } = null;

        public TyTranslation(string name, string contents)
        {
            Name = name;
            Translations = new Dictionary<string, string>();

            ParseContents(contents);
        }

        public override string ToString()
        {
            string result = "[]\r\n";

            foreach (string key in Translations.Keys)
                result += key + " " + Translations[key] + "\r\n";

            return result;
        }


        private void ParseContents(string contents)
        {
            string context = "[]";
            string[] lines = contents.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            string[] words;

            foreach (string line in lines)
            {
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    context = line;
                    continue;
                }

                if (context != "[]" && context != "[PC]")
                    continue;

                words = line.Split(new char[] { ' ' }, 2);
                if (words.Length == 0)
                    continue;

                if (words.Length == 1 && words[0] != String.Empty)
                    Translations.Add(words[0], String.Empty);
                else if (words[0] != String.Empty)
                    Translations.Add(words[0], words[1]);
            }
        }


    }
}
