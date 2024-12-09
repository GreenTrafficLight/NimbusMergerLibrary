using NimbusMergerLibrary.Mergers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NimbusMergerGUI
{
    public partial class Launcher : Form
    {
        private string _gameDir = "";
        private string _modsDir = "";
        private string _exportDir = "";

        public string GameDir
        {
            get { return _gameDir; }
            set
            {
                if (_gameDir != value)
                {
                    _gameDir = value;
                    LauncherTextBoxGameDir.Text = value;
                }
            }
        }

        public string ModsDir
        {
            get { return _modsDir; }
            set
            {
                if (_modsDir != value)
                {
                    _modsDir = value;
                    LauncherTextBoxModsDir.Text = value;
                }
            }
        }

        public string ExportDir
        {
            get { return _exportDir; }
            set
            {
                if (_exportDir != value)
                {
                    _exportDir = value;
                    LauncherTextBoxExportDir.Text = value;
                }

            }
        }

        public Launcher()
        {
            InitializeComponent();

            ExportDir = Directory.GetCurrentDirectory();
        }

        #region Button Events

        private void LauncherButtonGameDir_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    GameDir = fbd.SelectedPath;
                    if (Directory.Exists(GameDir + "\\~mods"))
                    {
                        ModsDir = GameDir + "\\~mods";
                    }
                }
            }
        }

        private void LauncherButtonModsDir_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    ModsDir = fbd.SelectedPath;
                }
            }
        }

        private void LauncherButtonExportDir_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    ExportDir = fbd.SelectedPath;
                }
            }
        }

        private void LauncherButtonOk_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(GameDir) && Directory.Exists(ModsDir) && Directory.Exists(ExportDir))
            {
                DialogResult = DialogResult.OK;

                NimbusMerger nimbusMerger = new NimbusMerger(GameDir, ModsDir);

                nimbusMerger.Initialize();

                ExportDir += "\\" + new string('~', nimbusMerger.TildeCount) + "export\\" + "export_P";

                nimbusMerger.MergeLocalization();
                nimbusMerger.MergeDataTables();

                nimbusMerger.WriteMergedLocalization(ExportDir);
                nimbusMerger.WriteMergedDataTables(ExportDir);
                nimbusMerger.WritePak(Directory.GetCurrentDirectory() + "\\" + "UnrealPak-Batch-No-Compression.bat", ExportDir);

                Close();
            }
        }

        private void LauncherButtonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        #endregion

        #region TextBox Events

        private void LauncherTextBoxGameDir_TextChanged(object sender, EventArgs e)
        {
            GameDir = LauncherTextBoxGameDir.Text;
            if (Directory.Exists(GameDir + "\\~mods") && string.IsNullOrEmpty(LauncherTextBoxModsDir.Text))
            {
                ModsDir = GameDir + "\\~mods";
                ExportDir = GameDir + "\\~mods";
            }
        }

        private void LauncherTextBoxModsDir_TextChanged(object sender, EventArgs e)
        {
            ModsDir = LauncherTextBoxModsDir.Text;
        }

        private void LauncherTextBoxExportDir_TextChanged(object sender, EventArgs e)
        {
            ExportDir = LauncherTextBoxExportDir.Text;
        }

        #endregion
    }
}
