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

namespace TyModManager.UI
{
    public partial class Main : Form
    {
        

        public Main()
        {
            InitializeComponent();

            Program.ApplyMods(Program.Mods);

            this.Text = "Ty The Tasmanian Tiger " + "r" + Program.RVersion.ToString("G") + "_v" + Program.VVersion.ToString("F");
        }
        
    }
}
