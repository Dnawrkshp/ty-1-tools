using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TyModManager
{
    [Serializable, XmlRoot("Config"), XmlType("Config")]
    public class Configuration
    {
        [XmlArray("EnabledMods")]
        public List<string> EnabledMods { get; set; }

        [XmlArray("LevelOrder")]
        public List<string> LevelOrder { get; set; }

        [XmlElement("Language")]
        public string Language { get; set; }

        [XmlElement("TestCommand")]
        public string TestCommand { get; set; }

        [XmlElement("TestStartOnly")]
        public bool TestStartOnly { get; set; }

        public Configuration(string xmlPath)
        {
            Configuration con = null;
            StreamReader xmlFile = null;

            // No need to try and read a non-existent file
            if (!File.Exists(xmlPath))
                goto exit;

            try
            {
                // Deserialize xml file
                XmlSerializer serializer = new XmlSerializer(typeof(Configuration));
                xmlFile = new StreamReader(xmlPath, Encoding.Unicode);
                con = (Configuration)serializer.Deserialize(xmlFile);
                xmlFile.Close();

                // Copy contents into this instance
                this.EnabledMods = con.EnabledMods;
                this.LevelOrder = con.LevelOrder;
                this.TestCommand = con.TestCommand;
                this.TestStartOnly = con.TestStartOnly;
                this.Language = con.Language;
            }
            catch (Exception e) { Program.Log(xmlPath, "Error deserializing file", e); if (xmlFile != null) { xmlFile.Close(); } }

            // Ensure both lists aren't null
            exit:  if (this.EnabledMods == null)
                this.EnabledMods = new List<string>();
            if (this.LevelOrder == null)
                this.LevelOrder = new List<string>();
            if (this.TestCommand == null)
                this.TestCommand = "-tydev";
        }

        // Default constructor for xml deserializer
        public Configuration()
        {

        }

        // Serialize instance
        public bool Save(string xmlPath)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Configuration));
                using (StreamWriter s = new StreamWriter(File.Open(xmlPath, FileMode.Create), Encoding.Unicode))
                    serializer.Serialize(s, this);
                
                return true;
            }
            catch (Exception e) { Program.Log(xmlPath, "Error serializing file", e); }

            return false;
        }
    }
}
