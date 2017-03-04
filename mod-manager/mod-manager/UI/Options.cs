using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TyModManager.Localization;

namespace TyModManager.UI
{
    public partial class Options : Form, ILocale
    {
        private string _lang = String.Empty;
        private bool _userInput = true;

        public Options()
        {
            _userInput = false;

            InitializeComponent();

            this.Icon = Properties.Resources.mod_manager;

            this.cbLanguage.Items.Clear();
            foreach (Locale lang in Program.Locales)
                this.cbLanguage.Items.Add(lang.Native);

            _lang = Program.Config.Language;

            this.tbTestArgs.Text = Program.Config.TestCommand;
            this.cbStartOnly.Checked = Program.Config.TestStartOnly;
            this.cbLanguage.SelectedItem = Program.Locales.Where(l => l.Name == Program.Config.Language).FirstOrDefault().Native;

            Localize();

            _userInput = true;
        }

        public void Localize()
        {
            // Apply text
            this.Text = Locale.Language.Options.Title.Replace("%%", string.Format(Locale.Language.Culture, "v{0:0.00}", Program.AppVersion));
            this.gbTestArgs.Text = Locale.Language.Options.OnTestContainer.TestArgs.Text;
            this.gbOnTest.Text = Locale.Language.Options.OnTestContainer.Text;
            this.cbStartOnly.Text = Locale.Language.Options.OnTestContainer.StartOnly.Text;
            this.gbLanguage.Text = Locale.Language.Options.LanguageContainer.Text;
            this.btOkay.Text = Locale.Language.Okay;
            this.btCancel.Text = Locale.Language.Cancel;

            // Apply tooltip
            this.toolTip.SetToolTip(this.gbTestArgs, Locale.Language.Options.OnTestContainer.TestArgs.Tooltip);
            this.toolTip.SetToolTip(this.tbTestArgs, Locale.Language.Options.OnTestContainer.TestArgs.Tooltip);
            this.toolTip.SetToolTip(this.gbOnTest, Locale.Language.Options.OnTestContainer.Tooltip);
            this.toolTip.SetToolTip(this.cbStartOnly, Locale.Language.Options.OnTestContainer.StartOnly.Tooltip);
            this.toolTip.SetToolTip(this.gbLanguage, Locale.Language.Options.LanguageContainer.Tooltip);

            // Apply font
            this.Font = new Font(Locale.GetFontRegular(), 8.75f, FontStyle.Regular, GraphicsUnit.Point);
            this.gbTestArgs.Font = this.Font;
            this.gbOnTest.Font = this.Font;
            this.cbStartOnly.Font = this.Font;
            this.gbLanguage.Font = this.Font;
            this.btOkay.Font = this.Font;
            this.btCancel.Font = this.Font;
        }

        private void BtCancel_Click(object sender, EventArgs e)
        {
            // Set language back
            Locale.ChangeLocale(_lang);
            this.Close();
        }

        private void BtOkay_Click(object sender, EventArgs e)
        {
            Program.Config.TestCommand = tbTestArgs.Text;
            Program.Config.TestStartOnly = cbStartOnly.Checked;
            Program.Config.Save(Program.ConfigPath);
            this.Close();
        }

        private void CbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_userInput && cbLanguage.SelectedIndex >= 0)
                Locale.ChangeLocale(Program.Locales[cbLanguage.SelectedIndex].Name);
        }
    }
}
