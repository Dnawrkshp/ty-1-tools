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

        public Configuration(string xmlPath)
        {
            Configuration con = null;
            FileStream xmlFile = null;

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Configuration));
                xmlFile = new FileStream(xmlPath, FileMode.Open);
                con = (Configuration)serializer.Deserialize(xmlFile);
                xmlFile.Close();

                this.EnabledMods = con.EnabledMods;
                this.LevelOrder = con.LevelOrder;
            }
            catch (Exception e) { Program.Log(xmlPath, "Error deserializing file", e); if (xmlFile != null) { xmlFile.Close(); } }

            if (this.EnabledMods == null)
                this.EnabledMods = new List<string>();
            if (this.LevelOrder == null)
                this.LevelOrder = new List<string>();
        }

        public Configuration()
        {

        }

        public bool Save(string xmlPath)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Configuration));
                using (Stream s = File.Open(xmlPath, FileMode.Create))
                    serializer.Serialize(s, this);
                
                return true;
            }
            catch (Exception e) { Program.Log(xmlPath, "Error serializing file", e); }

            return false;
        }
    }
}
