using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ty_mod_manager
{
    static class Program
    {
        public static string ModDirectory = "Mods";
        public static string OutDirectory = "PC_External";
        public static string TyExecutable = "TY.exe";
        public static string TyDirectory = "";

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

            if (!GetTyVersion())
                return;


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }

        public static void Log(string line, Exception e = null, bool show = false)
        {
            if (show)
            {
                MessageBox.Show(line, e?.ToString());
                return;
            }
        }

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
    }
}
