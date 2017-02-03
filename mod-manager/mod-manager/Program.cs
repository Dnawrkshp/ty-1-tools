using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using TyModManager.Archive;
using TyModManager.Attribute;
using TyModManager.Element;
using TyModManager.UI;

namespace TyModManager
{
    static class Program
    {
        public static List<TyMod> Mods = new List<TyMod>();
        public static TyRKV DataPC = null;

        public static string ModDirectory = "Mods";
        public static string OutDirectory = "PC_External";
        public static string TyExecutable = "TY.exe";
        public static string TyDirectory = String.Empty;

        public static ulong ErrorCount = 0;

        public static double RVersion = 0d;
        public static double VVersion = 0d;

        public static string PortalEntry = "pos = %x, %y, %z\r\n" +
                                           "ID = %i,%l\r\n" +
                                           "standPos = -192.000, 4379.000, 4551.000\r\n" +
                                           "standYaw = -632.000\r\n" +
                                           "connectingLevel = %i,%l\r\n" +
                                           "bExitPortal = 0,false\r\n" +
                                           "bVisible = 1,true\r\n" +
                                           "// User: %a\r\n";

        private static string LogPath = "mod-manager.log";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            TyDirectory = Environment.CurrentDirectory;
            if (!File.Exists(Path.Combine(TyDirectory, TyExecutable)))
                TyDirectory = Environment.GetEnvironmentVariable("TY_1_DIR");

            if (TyDirectory == null || !File.Exists(Path.Combine(TyDirectory, TyExecutable)))
            {
                MessageBox.Show("Please place this within your Ty installation.\r\n\r\nAlternatively, set the TY_1_DIR system environment variable.");
                return;
            }

            ModDirectory = Path.Combine(TyDirectory, ModDirectory);
            OutDirectory = Path.Combine(TyDirectory, OutDirectory);
            TyExecutable = Path.Combine(TyDirectory, TyExecutable);
            LogPath = Path.Combine(TyDirectory, LogPath);

            if (File.Exists(LogPath))
                File.Delete(LogPath);

            if (!GetTyVersion())
            {
                Log("mod-manager", "Unable to determine version of Ty.exe", null, true);
                return;
            }

            DataPC = new TyRKV(Path.Combine(Program.TyDirectory, "Data_PC.rkv"));
            if (!ImportMods(Program.ModDirectory))
            {
                // to-do
                // inform user there were errors importing mods
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }

        #region Import Mods

        private static bool ImportMods(string directory)
        {
            ulong currentErrorCount = Program.ErrorCount;

            foreach (string xml in Directory.GetFiles(directory, "*.xml", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    ImportMod(xml);
                }
                catch (Exception e) { Program.Log(xml, "Failed to load", e); }
            }

            return currentErrorCount == Program.ErrorCount;
        }

        private static void ImportMod(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            XmlDocument xmldoc = new XmlDocument();
            XmlNodeList xmlnode;
            XmlNode mod;
            string name, tyversion, version, authors, description;
            TyVersion tyversionRange;

            xmldoc.Load(fs);
            xmlnode = xmldoc.GetElementsByTagName("tymod");

            for (int i = 0; i < xmlnode.Count; i++)
            {
                name = null; tyversion = null; version = null; tyversionRange = null; authors = null; description = null;

                mod = xmlnode[i];

                try { name = mod.Attributes.GetNamedItem("name").Value; } catch (Exception e) { Program.Log(path, "Invalid name attribute for tymod \"" + mod.OuterXml + "\"", e); continue; }
                try { version = mod.Attributes.GetNamedItem("version").Value; } catch { }
                try { authors = mod.Attributes.GetNamedItem("authors").Value; } catch { }
                try { description = mod.Attributes.GetNamedItem("description").Value; } catch { }
                try { tyversion = mod.Attributes.GetNamedItem("tyversion").Value; tyversionRange = new TyVersion(tyversion, name + " (" + (version ?? String.Empty) + ";" + (authors ?? String.Empty) + ")"); } catch { }

                if (name != null && name != String.Empty)
                {
                    TyMod tymod = new TyMod(name, tyversionRange, version, authors, description);
                    tymod.AddFromNode(mod);

                    Mods.Add(tymod);
                }
            }

            fs.Close();
        }

        #endregion

        #region Apply Mods

        public static bool ApplyMods(List<TyMod> mods)
        {
            Dictionary<string, TyGlobal> globals = new Dictionary<string, TyGlobal>();
            Dictionary<string, TyTranslation> translations = new Dictionary<string, TyTranslation>();
            List<string> customLevels = new List<string>();
            string[] englishTranslations = null;
            string z1LV2 = null;
            uint levelIndex = 0, levelStart = 0;

            if (Directory.Exists(Program.OutDirectory))
                Directory.Delete(Program.OutDirectory, true);

            // Wait until directory is deleted
            while (Directory.Exists(Program.OutDirectory))
                System.Threading.Thread.Sleep(10);

            // Create new PC_External directory
            Directory.CreateDirectory(Program.OutDirectory);

            // Import all translation files
            translations.Add("english", new TyTranslation("Translations.English.txt", DataPC.ExtractText(DataPC.FileEntries.Where(f => f.FileName == "Translations.English.txt").FirstOrDefault())));
            translations.Add("german", new TyTranslation("Translations.German.txt", DataPC.ExtractText(DataPC.FileEntries.Where(f => f.FileName == "Translations.German.txt").FirstOrDefault())));
            translations.Add("spanish", new TyTranslation("Translations.Spanish.txt", DataPC.ExtractText(DataPC.FileEntries.Where(f => f.FileName == "Translations.Spanish.txt").FirstOrDefault())));
            translations.Add("french", new TyTranslation("Translations.French.txt", DataPC.ExtractText(DataPC.FileEntries.Where(f => f.FileName == "Translations.French.txt").FirstOrDefault())));
            translations.Add("italian", new TyTranslation("Translations.Italian.txt", DataPC.ExtractText(DataPC.FileEntries.Where(f => f.FileName == "Translations.Italian.txt").FirstOrDefault())));

            // Import z1.lv2
            // We do this to later add portals for any added custom maps
            // If we load this file from the OutDirectory instead from the RKV AND we load user imports before user levels...
            // We can support any direct user modification to the z1.lv2
            File.WriteAllText(Path.Combine(Program.OutDirectory, "z1.lv2"), DataPC.ExtractText(DataPC.FileEntries.Where(f => f.FileName == "z1.lv2").FirstOrDefault()), Encoding.GetEncoding("iso-8859-1"));

            // Apply all imports first
            foreach (TyMod mod in mods)
            {
                if (mod.Enabled && mod.Valid && (mod.TyVersion == null || mod.TyVersion.ContainsVersion(Program.RVersion)))
                {
                    foreach (TyModImport import in mod.Imports)
                        TyMod.ApplyImport(mod, import);
                }
            }

            // Load the potentially altered z1.lv2
            z1LV2 = File.ReadAllText(Path.Combine(Program.OutDirectory, "z1.lv2"), Encoding.GetEncoding("iso-8859-1"));


            // Offset levelIndex by index of "TEXT_LEVEL_Z1" translation entry
            englishTranslations = translations["english"].Translations.Keys.ToArray();
            levelIndex = 0;
            while (englishTranslations[levelIndex] != "TEXT_LEVEL_Z1")
                levelIndex++;

            levelStart = (uint)translations["english"].Translations.Count - levelIndex;
            levelIndex = levelStart;

            // Apply everything else
            foreach (TyMod mod in mods)
            {
                if (mod.Enabled && mod.Valid && (mod.TyVersion == null || mod.TyVersion.ContainsVersion(Program.RVersion)))
                {
                    // Apply global edit
                    foreach (TyModEdit edit in mod.Edits)
                    {
                        if (!globals.ContainsKey(edit.Source))
                        {
                            TyRKV.FileEntry file = DataPC.FileEntries.Where(f => f.FileName == edit.Source || f.FullPath == edit.Source).FirstOrDefault();
                            if (file.FileName == null)
                            {
                                Program.Log(mod.ToString(), "Unable to find global source file \"" + edit.Source + "\"");
                                continue;
                            }

                            globals.Add(edit.Source, new TyGlobal(edit.Source, DataPC.ExtractText(file)));
                        }

                        TyMod.ApplyGlobal(globals[edit.Source], edit);
                    }

                    // Apply level
                    foreach (TyLevel level in mod.Levels)
                    {
                        string levelID = "f" + (levelIndex - levelStart).ToString();
                        TyMod.ApplyLevel(levelID, level);

                        // Add language definitions
                        try
                        {
                            translations["english"].Translations.Add("TEXT_LEVEL_" + levelID, level.LanguageNames["english"]);
                            translations["german"].Translations.Add("TEXT_LEVEL_" + levelID, level.LanguageNames["german"]);
                            translations["spanish"].Translations.Add("TEXT_LEVEL_" + levelID, level.LanguageNames["spanish"]);
                            translations["french"].Translations.Add("TEXT_LEVEL_" + levelID, level.LanguageNames["french"]);
                            translations["italian"].Translations.Add("TEXT_LEVEL_" + levelID, level.LanguageNames["italian"]);
                        }
                        catch (Exception e) { Program.Log(mod.ToString(), "Unable to add language translations for level \"" + level.InputPath + "\"", e, true); return false; }

                        // Add to z1.lv2
                        Match portal = Regex.Match(z1LV2, @"^name\s*PORTAL\s*$", RegexOptions.Multiline);

                        if (!portal.Success)
                        {
                            Program.Log("z1.lv2", "Unable to find portal entries", null, true);
                            return false;
                        }
                        z1LV2 = z1LV2.Insert(z1LV2.IndexOf("\r\n", portal.Index) + 2, "\r\n" + Program.PortalEntry
                            .Replace("%l", levelID)
                            .Replace("%i", levelIndex.ToString())
                            .Replace("%x", level.X.ToString("F"))
                            .Replace("%y", level.Y.ToString("F"))
                            .Replace("%z", level.Z.ToString("F"))
                            .Replace("%a", mod.Authors)
                            );

                        customLevels.Add(levelIndex.ToString() + " " + levelID);

                        levelIndex++;
                    }

                    foreach (TyModTranslate translation in mod.Translations)
                        TyMod.ApplyTranslate(translations, translation);
                }
            }

            // Write all edited global files
            foreach (string key in globals.Keys)
            {
                FileInfo file = new FileInfo(Path.Combine(Program.OutDirectory, key));
                if (!file.Directory.Exists)
                    Directory.CreateDirectory(file.Directory.FullName);
                File.WriteAllText(file.FullName, globals[key].ToString(), Encoding.GetEncoding("iso-8859-1"));
            }

            // Write all edited translation files
            foreach (string key in translations.Keys)
            {
                FileInfo file = new FileInfo(Path.Combine(Program.OutDirectory, "Translations." + (char.ToUpper(key[0]) + key.Substring(1)) + ".txt"));
                if (!file.Directory.Exists)
                    Directory.CreateDirectory(file.Directory.FullName);
                File.WriteAllText(file.FullName, translations[key].ToString(), Encoding.GetEncoding("iso-8859-1"));
            }

            // Write potentially edited zl.lv2
            File.WriteAllText(Path.Combine(Program.OutDirectory, "z1.lv2"), z1LV2, Encoding.GetEncoding("iso-8859-1"));

            // Write list of custom maps to cmaps.ini for the proxy dll to parse
            File.WriteAllText(Path.Combine(Program.OutDirectory, "cmaps.ini"), String.Join("\n", customLevels));

            return true;
        }

        #endregion

        #region Log

        public static void Log(string context, string line, Exception e = null, bool show = false)
        {
            if (context != null && context != String.Empty)
                line = context + ": " + line;

            ErrorCount++;
            File.AppendAllText(LogPath, line + "\r\n" + e?.ToString() + "\r\n\r\n\r\n");

            if (show)
                MessageBox.Show(line, e?.ToString());
        }

        #endregion

        #region Get TyVersion

        private static bool GetTyVersion()
        {
            byte[] tyExe = File.ReadAllBytes(TyExecutable);

            if (tyExe == null || tyExe.Length == 0)
            {
                MessageBox.Show("Unable to open \"" + TyExecutable + "\" to parse version");
                return false;
            }

            // Loop through bytes searching for a pattern of rX_vX where X is a numeric value of variable length
            for (int x = 0; x < (tyExe.Length - 7); x++)
                if (GetTyVersion_Match(ref tyExe, x))
                    return true;

            RVersion = 0d;
            VVersion = 0d;
            return false;
        }

        private static bool GetTyVersion_Match(ref byte[] tyExe, int index)
        {
            int start = index, element0 = 0, element1 = 0;

            try
            {
                if (tyExe[index++] != 'r')
                    return false;

                // Grab length of the number following
                element0 = index;
                while ((tyExe[index] >= 0x30 && tyExe[index] <= 0x39) || tyExe[index] == '.')
                    index++;

                // Ensure the length of the number is greater than 0
                if ((index - element0) == 0)
                    return false;

                // Parse value into RVersion
                RVersion = double.Parse(Encoding.ASCII.GetString(tyExe, element0, index - element0));

                // Ensure the next two characters are "_v"
                if (!(tyExe[index++] == '_' && tyExe[index++] == 'v'))
                    return false;

                // Grab length of the number following
                element1 = index;
                while ((tyExe[index] >= 0x30 && tyExe[index] <= 0x39) || tyExe[index] == '.')
                    index++;

                // Ensure the length of the number is greater than 0
                if ((index - element1) == 0)
                    return false;

                // Parse value into VVersion
                VVersion = double.Parse(Encoding.ASCII.GetString(tyExe, element1, index - element1));
            }
            catch { return false; }

            return true;
        }

        #endregion

    }
}
