using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TyModManager.Element
{
    public class TyGlobal
    {
        public string Name { get; } = null;
        public string Contents { get; } = null;


        public List<TyGlobalItem> Items { get; } = null;

        public TyGlobal(string name, string globalContents)
        {
            Name = name;
            Contents = globalContents;

            Items = new List<TyGlobalItem>();

            ParseContents(globalContents);
        }

        public override string ToString()
        {
            string result = String.Empty;

            foreach (TyGlobalItem item in Items)
                result += "\r\n\r\n" + item.ToString(this, "    ");

            return result.Trim();
        }


        private string ParseContents_GetContext(string lastContext, string contents, int index)
        {
            int x = index - 2, lastComment = -1;

            while (x >= 0)
            {
                if (contents[x] == '\n' && contents[x + 1] != '/' && !(contents[x + 1] == '\r' || contents[x + 1] == '\n'))
                {
                    if (lastComment < 0)
                        return lastContext;

                    return contents.Substring(lastComment, index - lastComment).Trim().ToLower();
                }
                else if (contents[x] == '/' && contents[x + 1] == '/')
                    lastComment = x;

                x--;
            }

            if (lastContext == null)
                return contents.Substring(0, index).Trim().ToLower();
            else
                return lastContext;
        }

        // Parses the global file
        private void ParseContents(string contents)
        {
            string context = null;
            MatchCollection matches = Regex.Matches(contents, @"^\b(\s*)name(.*?)$", RegexOptions.Multiline);

            if (matches.Count == 0)
                return;

            Items.Add(new TyGlobalItem(null, contents.Substring(0, matches[0].Index), context, 0));

            for (int x = 0; x < matches.Count; x++)
            {
                // get context
                context = ParseContents_GetContext(context, contents, matches[x].Index);

                ParseContents_Match(contents,
                    matches[x],
                    x == (matches.Count - 1) ? contents.Length - matches[x].Index : matches[x+1].Index - matches[x].Index, context);
            }
        }

        private void ParseContents_Match(string contents, Match match, int length, string context)
        {
            string[] lines = contents.Substring(match.Index, length).Split(new char[] { '\n' });
            string[] words = null;
            int indent;
            TyGlobalItem root = null, parent = null;
            List<TyGlobalItem> lastItems = new List<TyGlobalItem>();

            foreach (string line in lines)
            {
                if (lastItems.Count > 0)
                {
                    indent = ParseContents_GetLineInfo(line.Replace("\r", String.Empty), ref lastItems, out words);
                    parent = lastItems[lastItems.Count - 1];

                    if (words.Length == 0)
                    {
                        //parent.SubItems.Add(new TyGlobalItem(null, null, indent));
                    }
                    else if (words.Length == 1)
                    {
                        parent.SubItems.Add(new TyGlobalItem(null, words[0], context, indent));
                        lastItems.Add(parent.SubItems[parent.SubItems.Count-1]);
                    }
                    else if (words[1] == "=")
                    {
                        parent.SubItems.Add(new TyGlobalItem(words[0], String.Join(" ", words, 2, words.Length - 2), context, indent, true));
                        lastItems.Add(parent.SubItems[parent.SubItems.Count - 1]);
                    }
                    else
                    {
                        parent.SubItems.Add(new TyGlobalItem(words[0], String.Join(" ", words, 1, words.Length - 1), context, indent));
                        lastItems.Add(parent.SubItems[parent.SubItems.Count - 1]);
                    }
                }
                else if (line.StartsWith("name "))
                {
                    root = new TyGlobalItem("name", line.Substring(5).Trim(), context, 0);
                    lastItems.Add(root);
                }
            }

            if (root.Key != null && root.Value != null)
                Items.Add(root);
        }

        private int ParseContents_GetLineInfo(string line, ref List<TyGlobalItem> hierarchy, out string[] words)
        {
            int indent = 0, x = 0;

            // Get indent of line
            while (x < line.Length)
            {
                if (line[x] == ' ')
                    indent++;
                else if (line[x] == '\t')
                    indent += 4;
                else
                    break;
                x++;
            }

            // Get words
            words = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            // Set remove all subelements that are of same indent or more
            for (x = hierarchy.Count - 1; x > 0; x--)
                if (indent <= hierarchy[x].Indents)
                    hierarchy.RemoveAt(x);

            return indent;
        }
    }

    public class TyGlobalItem
    {
        public string Key { get; } = null;
        public string Value { get; set; } = null;
        public List<TyGlobalItem> SubItems { get; } = null;
        public bool EqualSign { get; } = false;
        public int Indents { get; } = 0;
        public string Context { get; } = null;

        public TyGlobalItem(string key, string value, string context, int indents, bool equalSign = false)
        {
            Key = key;
            Value = value;
            Context = context;
            EqualSign = equalSign;
            Indents = indents;

            SubItems = new List<TyGlobalItem>();
        }

        public string ToString(TyGlobal global, string indent)
        {
            string result = (Key == null ? String.Empty : Key + " ") + (EqualSign ? "= " : String.Empty) + (Value ?? String.Empty).Trim();
            result = result.PadLeft((Indents - indent.Length) + 2 + result.Length, ' ');

            if (indent.Length > 0 && Key != null && Value != null && Key.ToLower() == "name" && Value.ToLower() != "setup")
                indent = "";

            foreach (TyGlobalItem item in SubItems)
                result += "\r\n" + indent + item.ToString(global, indent + "  ");

            if (indent.Length == 2 && global.Name.ToLower() == "global.sound")
                result += "\r\n";

            return result;
        }
    }
}
