using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TyModManager.Attribute
{
    public class TyVersion
    {
        private double min = -1;
        private bool min_inclusive = false;

        private double max = -1;
        private bool max_inclusive = false;

        public bool Valid { get; } = false;

        public override string ToString()
        {
            string result = min_inclusive ? "[" : "(";

            if (min >= 0)
                result += min.ToString("F");

            if (max >= 0)
                result += "," + max.ToString("F");

            return result + (max_inclusive ? "]" : ")");
        }

        public TyVersion(string tyversion, string context)
        {
            // If no tyversion, default is to support all versions
            if (tyversion == null || tyversion == String.Empty)
            {
                Valid = true;
                return;
            }

            min_inclusive = !tyversion.StartsWith("(");
            max_inclusive = !tyversion.EndsWith(")");

            string[] parts = tyversion.Split(new char[] { ',' });

            if (parts.Length == 1)
            {
                min = ParseValue(parts[0].Replace("r", String.Empty), context);
                max = min;

                if (!min_inclusive && !max_inclusive)
                    Program.Log(context, "Version \"" + tyversion + "\" will never find a match");
                else
                    Valid = true;
                return;
            }
            else if (parts.Length == 2)
            {
                min = ParseValue(parts[0].Replace("r", String.Empty), context);
                max = ParseValue(parts[1].Replace("r", String.Empty), context);
                Valid = true;
                return;
            }

            Program.Log(context, "Invalid version range \"" + tyversion + "\"");
        }

        public bool ContainsVersion(double version)
        {
            // All supported version
            if (min < 0 && max < 0)
                return true;

            // Maximum version
            if (min < 0 && (max_inclusive ? version <= max : version < max))
                return true;

            // Minimum version
            if (max < 0 && (min_inclusive ? version >= min : version > min))
                return true;

            // Exact range
            if ((min_inclusive ? version >= min : version > min) && (max_inclusive ? version <= max : version < max))
                return true;

            return false;
        }


        private bool IsIdentifier(char c)
        {
            return c == '[' || c == ']' || c == '(' || c == ')';
        }

        private double ParseValue(string value, string context)
        {
            if (value == null || value == String.Empty)
                return -1d;

            int start = IsIdentifier(value[0])?1:0;
            if (start >= value.Length)
                return -1d;

            string subString = value.Substring(start, value.Length - (IsIdentifier(value[value.Length - 1]) ? 1 : 0) - start);
            string doubleString = String.Empty;
            double result = -1d;

            start = 0;
            for (int x = 0; x < subString.Length; x++) {
                if (subString[x] == '.')
                {
                    if (start != 0)
                        continue;

                    start = 1;
                }

                doubleString += subString[x];
            }

            try { result = double.Parse(doubleString); } catch (Exception e) { Program.Log(context, "Invalid version number \"" + value + "\"", e); }

            return result;
        }
    }
}
