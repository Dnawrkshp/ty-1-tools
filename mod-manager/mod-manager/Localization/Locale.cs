using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace TyModManager.Localization
{
    [Serializable, XmlRoot("locale")]
    public class Locale
    {

        #region XML Schema

        public struct TooltipTextElement
        {
            [XmlAttribute("tooltip")]
            public string Tooltip { get; set; }

            [XmlText]
            public string Text { get; set; }
        }

        public struct TooltipElement
        {
            [XmlAttribute("tooltip")]
            public string Tooltip { get; set; }
        }

        public struct ModsColumnElement
        {
            [XmlElement("enabled")]
            public TooltipTextElement Enabled { get; set; }

            [XmlElement("name")]
            public TooltipTextElement Name { get; set; }
        }

        public struct ModsContainerElement
        {
            [XmlAttribute("tooltip")]
            public string Tooltip { get; set; }

            [XmlAttribute("text")]
            public string Text { get; set; }

            [XmlElement("columns")]
            public ModsColumnElement Columns { get; set; }
        }

        public struct LevelColumnElement
        {
            [XmlElement("id")]
            public TooltipTextElement ID { get; set; }

            [XmlElement("level")]
            public TooltipTextElement Level { get; set; }

            [XmlElement("u")]
            public TooltipTextElement U { get; set; }

            [XmlElement("d")]
            public TooltipTextElement D { get; set; }
        }

        public struct LevelContainerElement
        {
            [XmlAttribute("tooltip")]
            public string Tooltip { get; set; }

            [XmlAttribute("text")]
            public string Text { get; set; }

            [XmlElement("columns")]
            public LevelColumnElement Columns { get; set; }
        }

        public struct OnTestContainerElement
        {
            [XmlAttribute("tooltip")]
            public string Tooltip { get; set; }

            [XmlAttribute("text")]
            public string Text { get; set; }

            [XmlElement("test-args")]
            public TooltipTextElement TestArgs { get; set; }

            [XmlElement("start-only")]
            public TooltipTextElement StartOnly { get; set; }
        }
        
        public struct MainForm
        {
            [XmlElement("play")]
            public TooltipTextElement Play { get; set; }

            [XmlElement("test")]
            public TooltipTextElement Test { get; set; }

            [XmlElement("mods")]
            public TooltipTextElement Mods { get; set; }

            [XmlElement("options")]
            public TooltipTextElement Options { get; set; }

            [XmlElement("exit")]
            public string Exit { get; set; }

            [XmlElement("wiki")]
            public TooltipElement Wiki { get; set; }

            [XmlElement("folder")]
            public TooltipElement Folder { get; set; }

            [XmlElement("github")]
            public TooltipElement Github { get; set; }
        }
        
        public struct ModsForm
        {
            [XmlElement("title")]
            public string Title { get; set; }

            [XmlElement("empty-level")]
            public string EmptyLevel { get; set; }

            [XmlElement("incompatible-mod")]
            public string IncompatibleMod { get; set; }

            [XmlElement("mods-container")]
            public ModsContainerElement ModsContainer { get; set; }

            [XmlElement("level-container")]
            public LevelContainerElement LevelContainer { get; set; }

            [XmlElement("description-container")]
            public TooltipElement DescriptionContainer { get; set; }
        }
        
        public struct OptionsForm
        {
            [XmlElement("title")]
            public string Title { get; set; }

            [XmlElement("on-test-container")]
            public OnTestContainerElement OnTestContainer { get; set; }

            [XmlElement("language-container")]
            public TooltipTextElement LanguageContainer { get; set; }
        }



        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("native")]
        public string Native { get; set; }

        [XmlAttribute("font")]
        public string Font { get; set; }

        [XmlAttribute("font-mono")]
        public string FontMono { get; set; }


        [XmlArray("fonts")]
        public List<string> Fonts { get; set; }

        [XmlElement("okay")]
        public string Okay { get; set; }

        [XmlElement("cancel")]
        public string Cancel { get; set; }

        [XmlElement("main")]
        public MainForm Main { get; set; }

        [XmlElement("mods")]
        public ModsForm Mods { get; set; }

        [XmlElement("options")]
        public OptionsForm Options { get; set; }

        #endregion

        #region Management

        public static Locale Language { get; set; } = null;

        public static void ChangeLocale(string language)
        {
            Language = Program.Locales.Where(l => l.Name == language).FirstOrDefault();

            if (Language == null)
                Language = Program.Locales[0];

            Program.Config.Language = Language.Name;

            // Enumerate all open forms
            foreach (Form f in Application.OpenForms)
                if (f.GetType().GetInterface("ILocale") != null)
                    (f as ILocale).Localize();
        }

        public static FontFamily GetFontRegular()
        {
            FontFamily family = Program.FontCollection.Families.Where(f => f.Name == Language.Font).FirstOrDefault();
            if (family == null)
                family = FontFamily.Families.Where(f => f.Name == Language.Font).FirstOrDefault();

            return family ?? FontFamily.GenericSerif;
        }

        public static FontFamily GetFontMono()
        {
            FontFamily family = Program.FontCollection.Families.Where(f => f.Name == Language.FontMono).FirstOrDefault();
            if (family == null)
                family = FontFamily.Families.Where(f => f.Name == Language.FontMono).FirstOrDefault();

            return family ?? FontFamily.GenericMonospace;
        }

        #endregion

        #region Serialization

        public static Locale Load(string path)
        {
            try
            {
                // Deserialize
                XmlSerializer serializer = new XmlSerializer(typeof(Locale));
                using (StreamReader sr = new StreamReader(path, Encoding.Unicode))
                    return serializer.Deserialize(sr) as Locale;
            }
            catch (Exception e) { Program.Log(path, "Error deserializing file", e); }

            return null;
        }

        public void Save(string path)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Locale));
                using (StreamWriter s = new StreamWriter(File.Open(path, FileMode.Create), Encoding.Unicode))
                    serializer.Serialize(s, this);
            }
            catch (Exception e) { Program.Log(path, "Error serializing file", e); }
        }

        public bool Valid()
        {
            if (Name == null || Native == null || Font == null || FontMono == null)
                return false;

            if (Okay == null || Cancel == null)
                return false;

            if (Main.Exit == null || Main.Play.Text == null || Main.Test.Text == null || Main.Mods.Text == null || Main.Options.Text == null)
                return false;

            if (Mods.Title == null || Mods.EmptyLevel == null || Mods.IncompatibleMod == null ||
                Mods.LevelContainer.Text == null || Mods.LevelContainer.Columns.ID.Text == null || Mods.LevelContainer.Columns.Level.Text == null || Mods.LevelContainer.Columns.U.Text == null || Mods.LevelContainer.Columns.D.Text == null ||
                Mods.ModsContainer.Text == null || Mods.ModsContainer.Columns.Enabled.Text == null || Mods.ModsContainer.Columns.Name.Text == null)
                return false;

            if (Options.Title == null || Options.OnTestContainer.Text == null || Options.OnTestContainer.StartOnly.Text == null || Options.OnTestContainer.TestArgs.Text == null ||
                Options.LanguageContainer.Text == null)
                return false;

            return true;
        }

        #endregion

    }
}
