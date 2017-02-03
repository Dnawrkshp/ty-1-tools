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

        public static Configuration Config = null;

        public static string ModDirectory = "Mods";
        public static string OutDirectory = "PC_External";
        public static string TyExecutable = "TY.exe";
        public static string ConfigPath = "mod-manager.config";
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

            if (File.Exists(LogPath))
                File.Delete(LogPath);

            if (!GetTyVersion())
            {
                Log("mod-manager", "Unable to determine version of Ty.exe", null, true);
                return;
            }

            // Load program config
            Config = new Configuration(ConfigPath);

            // Load Data_PC.rkv
            DataPC = new TyRKV(Path.Combine(Program.TyDirectory, "Data_PC.rkv"));

            // Import mods
            if (!ImportMods(Program.ModDirectory))
            {
                // to-do
                // inform user there were errors importing mods
            }

            // Load UI
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }

        #region Import Mods

        // Loops through root directory and read XML mods
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

        // Import individual XML mod
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

                    // Check if tymod already exists
                    if (Mods.Any(x => x.ToString() == tymod.ToString() && x.TyVersion.OverlapsVersion(tymod.TyVersion)))
                    {
                        Log(path, "TyMod with same name, version, and dependency already exists (\"" + tymod.ToString() + "\")");
                        continue;
                    }

                    // Add Elements from node
                    tymod.AddFromNode(mod);

                    // Add to list
                    Mods.Add(tymod);
                }
            }

            fs.Close();
        }

        #endregion

        #region Apply Mods

        // Apply list of mods
        public static bool ApplyMods(List<TyMod> mods)
        {
            Dictionary<string, TyGlobal> globals = new Dictionary<string, TyGlobal>();
            Dictionary<string, TyTranslation> translations = new Dictionary<string, TyTranslation>();
            List<TyLevel> levels = new List<TyLevel>();

            // Save level order
            ApplyMods_LevelOrder();

            // Delete existing directory
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

                    // Add level
                    foreach (TyLevel level in mod.Levels)
                        levels.Add(level);

                    foreach (TyModTranslate translation in mod.Translations)
                        TyMod.ApplyTranslate(translations, translation);
                }
            }

            // Apply levels
            ApplyMods_Level(levels, ref translations);

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

            // Update Config with new list of enabled mods
            Config.EnabledMods.Clear();
            foreach (TyMod mod in mods)
                Config.EnabledMods.Add(mod.ToString());
            Config.Save(Program.ConfigPath);

            return true;
        }

        private static void ApplyMods_Level(List<TyLevel> levels, ref Dictionary<string, TyTranslation> translations)
        {
            List<string> customLevels = new List<string>();
            string z1LV2, levelID;
            string[] englishTranslations = null;
            uint levelStart = 0;
            TyLevel level;

            if (levels == null || levels.Count == 0)
                return;

            // Sort least to greatest by IndexOffset
            levels.Sort((a, b) => a.IndexOffset.CompareTo(b.IndexOffset));

            // Load the potentially altered z1.lv2
            z1LV2 = File.ReadAllText(Path.Combine(Program.OutDirectory, "z1.lv2"), Encoding.GetEncoding("iso-8859-1"));

            // Offset levelIndex by index of "TEXT_LEVEL_Z1" translation entry
            englishTranslations = translations["english"].Translations.Keys.ToArray();
            while (englishTranslations[levelStart] != "TEXT_LEVEL_Z1")
                levelStart++;

            levelStart = (uint)translations["english"].Translations.Count - levelStart;

            // Apply level
            for (int x = 0; x <= levels.Max(l => l.IndexOffset); x++)
            {
                levelID = "f" + x.ToString();
                level = levels.Where(l => l.IndexOffset == x).FirstOrDefault();

                // Try to add level names to translations
                try { translations["english"].Translations.Add("TEXT_LEVEL_" + levelID, level.LanguageNames["english"]); }
                catch (Exception e)
                {
                    if (level != null) { Log(level.ToString(), "Unable to get english translation", e); }
                    translations["english"].Translations.Add("TEXT_LEVEL_" + levelID, String.Empty);
                }

                try { translations["german"].Translations.Add("TEXT_LEVEL_" + levelID, level.LanguageNames["german"]); }
                catch (Exception e)
                {
                    if (level != null) { Log(level.ToString(), "Unable to get german translation", e); }
                    translations["german"].Translations.Add("TEXT_LEVEL_" + levelID, String.Empty);
                }

                try { translations["spanish"].Translations.Add("TEXT_LEVEL_" + levelID, level.LanguageNames["spanish"]); }
                catch (Exception e)
                {
                    if (level != null) { Log(level.ToString(), "Unable to get spanish translation", e); }
                    translations["spanish"].Translations.Add("TEXT_LEVEL_" + levelID, String.Empty);
                }

                try { translations["french"].Translations.Add("TEXT_LEVEL_" + levelID, level.LanguageNames["french"]); }
                catch (Exception e)
                {
                    if (level != null) { Log(level.ToString(), "Unable to get french translation", e); }
                    translations["french"].Translations.Add("TEXT_LEVEL_" + levelID, String.Empty);
                }

                try { translations["italian"].Translations.Add("TEXT_LEVEL_" + levelID, level.LanguageNames["italian"]); }
                catch (Exception e)
                {
                    if (level != null) { Log(level.ToString(), "Unable to get italian translation", e); }
                    translations["italian"].Translations.Add("TEXT_LEVEL_" + levelID, String.Empty);
                }


                // Apply level
                if (level != null) {

                    // Find portal entries in z1.lv2
                    Match portal = Regex.Match(z1LV2, @"^name\s*PORTAL\s*$", RegexOptions.Multiline);
                    if (!portal.Success)
                    {
                        Program.Log("z1.lv2", "Unable to find portal entries", null, true);
                        return;
                    }

                    // Add level files to output directory
                    TyMod.ApplyLevel(levelID, level);

                    // Add portal for level
                    z1LV2 = z1LV2.Insert(z1LV2.IndexOf("\r\n", portal.Index) + 2, "\r\n" + Program.PortalEntry
                        .Replace("%l", levelID)
                        .Replace("%i", (x+levelStart).ToString())
                        .Replace("%x", level.X.ToString("F"))
                        .Replace("%y", level.Y.ToString("F"))
                        .Replace("%z", level.Z.ToString("F"))
                        .Replace("%a", level.Authors)
                        );

                    // Add to list of custom levels (to be used by the OpenAL32 proxy)
                    customLevels.Add((x + levelStart).ToString() + " " + levelID);
                }
            }

            // Write potentially edited zl.lv2
            File.WriteAllText(Path.Combine(Program.OutDirectory, "z1.lv2"), z1LV2, Encoding.GetEncoding("iso-8859-1"));

            // Write list of custom maps to cmaps.ini for the proxy dll to parse
            File.WriteAllText(Path.Combine(Program.OutDirectory, "cmaps.ini"), String.Join("\n", customLevels));
        }

        // Apply level order based on imported and enabled mods
        private static void ApplyMods_LevelOrder()
        {
            int x = 0;

            Config.LevelOrder.Clear();

            // Loop through once, adding all levels with an offset
            foreach (TyMod tymod in Mods)
            {
                foreach (TyLevel level in tymod.Levels)
                {
                    if (level.IndexOffset >= 0)
                    {
                        // Add empty entries to fill gap
                        while (level.IndexOffset > Config.LevelOrder.Count)
                            Config.LevelOrder.Add(String.Empty);
                        Config.LevelOrder.Add(level.ToString());
                    }
                }
            }

            // Loop through again, adding all levels with an offset
            foreach (TyMod tymod in Mods)
            {
                foreach (TyLevel level in tymod.Levels)
                {
                    if (level.IndexOffset < 0)
                    {
                        // Find empty entry and replace
                        for (x = 0; x < Config.LevelOrder.Count; x++)
                        {
                            if (Config.LevelOrder[x] == String.Empty)
                            {
                                Config.LevelOrder[x] = level.ToString();
                                break;
                            }
                        }

                        // Update level offset
                        level.IndexOffset = x;

                        // Add new entry if no empty ones exist
                        if (x == Config.LevelOrder.Count)
                            Config.LevelOrder.Add(level.ToString());
                    }
                }
            }

            Config.Save(ConfigPath);
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

        // Get version of Ty directly from the executable
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
