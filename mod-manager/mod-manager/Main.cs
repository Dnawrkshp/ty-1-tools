using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace ty_mod_manager
{
    public partial class Main : Form
    {
        List<TyMod> Mods = new List<TyMod>();
        TyRKV DataPC = null;

        public Main()
        {
            InitializeComponent();

            DataPC = new TyRKV(Path.Combine(Program.TyDirectory, "Data_PC.rkv"));

            if (!ImportMods(Program.ModDirectory))
            {
                // to-do
                // inform user there were errors importing mods
            }

            ApplyMods();

            this.Text = "Ty The Tasmanian Tiger " + "r" + Program.RVersion.ToString("G") + "_v" + Program.VVersion.ToString("F");
        }

        #region Import and Apply Mods

        public bool ApplyMods()
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
            foreach (TyMod mod in Mods)
            {
                if (mod.Valid && (mod.VersionRange == null || mod.VersionRange.ContainsVersion(Program.RVersion)))
                {
                    foreach (TyMod.TyModImport import in mod.Imports)
                        TyMod.ApplyImport(import);
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
            foreach (TyMod mod in Mods)
            {
                if (mod.Valid && (mod.VersionRange == null || mod.VersionRange.ContainsVersion(Program.RVersion)))
                {
                    // Apply global edit
                    foreach (TyModEdit edit in mod.Edits)
                    {
                        if (!globals.ContainsKey(edit.Source))
                        {
                            TyRKV.FileEntry file = DataPC.FileEntries.Where(f => f.FileName == edit.Source || f.FullPath == edit.Source).FirstOrDefault();
                            if (file.FileName == null)
                            {
                                Program.Log("Unable to find global source file \"" + edit.Source + "\"");
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
                        catch (Exception e) { Program.Log("Unable to add language translations for level \"" + level.InputPath + "\"", e, true); return false; }

                        // Add to z1.lv2
                        Match portal = Regex.Match(z1LV2, @"^name\s*PORTAL\s*$", RegexOptions.Multiline);

                        if (!portal.Success)
                        {
                            Program.Log("Unable to find portal entries in z1.lv2", null, true);
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

        public bool ImportMods(string directory)
        {
            ulong currentErrorCount = Program.ErrorCount;

            foreach (string xml in Directory.GetFiles(directory, "*.xml", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    ImportMod(xml);
                }
                catch (Exception e) { Program.Log("Failed to load \"" + xml + "\"", e); }
            }

            return currentErrorCount == Program.ErrorCount;
        }

        public void ImportMod(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            XmlDocument xmldoc = new XmlDocument();
            XmlNodeList xmlnode;
            XmlNode mod;
            string name, tyversion, version, authors, description;
            TyVersionRange versionRange;

            xmldoc.Load(fs);
            xmlnode = xmldoc.GetElementsByTagName("tymod");

            for (int i = 0; i < xmlnode.Count; i++)
            {
                name = null; tyversion = null;  version = null; versionRange = null; authors = null; description = null;

                mod = xmlnode[i];

                try { name = mod.Attributes.GetNamedItem("name").Value; } catch (Exception e) { Program.Log("Invalid name attribute for tymod \"" + mod.OuterXml + "\"", e); continue; }
                try { tyversion = mod.Attributes.GetNamedItem("tyversion").Value; versionRange = new TyVersionRange(tyversion); } catch (Exception e) { }
                try { version = mod.Attributes.GetNamedItem("version").Value; } catch (Exception e) { Program.Log("", e); }
                try { authors = mod.Attributes.GetNamedItem("authors").Value; } catch (Exception e) { Program.Log("", e); }
                try { description = mod.Attributes.GetNamedItem("description").Value; } catch (Exception e) { Program.Log("", e); }

                if (name != null && name != String.Empty)
                {
                    TyMod tymod = new TyMod(name, versionRange, version, authors, description);
                    tymod.AddFromNode(mod);

                    Mods.Add(tymod);
                }
            }

            fs.Close();
        }

        #endregion

    }
}
