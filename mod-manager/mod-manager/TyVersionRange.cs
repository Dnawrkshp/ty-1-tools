using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ty_mod_manager
{
    public class TyVersionRange
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

        public TyVersionRange(string versionRange)
        {
            // If no tyversion, default is to support all versions
            if (versionRange == null || versionRange == String.Empty)
            {
                Valid = true;
                return;
            }

            min_inclusive = !versionRange.StartsWith("(");
            max_inclusive = !versionRange.EndsWith(")");

            string[] parts = versionRange.Split(new char[] { ',' });

            if (parts.Length == 1)
            {
                min = ParseValue(parts[0].Replace("r", ""));
                max = min;

                if (!min_inclusive && !max_inclusive)
                    Program.Log("Version \"" + versionRange + "\" will never find a match");
                else
                    Valid = true;
                return;
            }
            else if (parts.Length == 2)
            {
                min = ParseValue(parts[0].Replace("r", ""));
                max = ParseValue(parts[1].Replace("r", ""));
                Valid = true;
                return;
            }

            Program.Log("Invalid version range \"" + versionRange + "\"");
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

        private double ParseValue(string value)
        {
            if (value == null || value == String.Empty)
                return -1d;

            int start = IsIdentifier(value[0])?1:0;
            if (start >= value.Length)
                return -1d;

            string subString = value.Substring(start, value.Length - (IsIdentifier(value[value.Length - 1]) ? 1 : 0) - start);
            string doubleString = "";
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

            try { result = double.Parse(doubleString); } catch (Exception e) { Program.Log("Invalid version number \"" + value + "\"", e); }

            return result;
        }
    }
}
