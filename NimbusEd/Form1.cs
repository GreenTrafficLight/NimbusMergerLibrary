using CUE4Parse.Encryption.Aes;
using CUE4Parse.UE4.Versions;
using NimbusMergerLibrary.FileProvider;
using NimbusMergerLibrary.PropertyGridExtensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using UAssetAPI.ExportTypes;
using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.PropertyTypes.Structs;

namespace NimbusEd
{
    public partial class Form1 : Form
    {
        private NimbusFileProvider _gameProvider { get; set; }

        public Form1()
        {
            InitializeComponent();
        }

        #region Menu Strip Controls

        private void MSFileOpen_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    string pakFolder = $"{fbd.SelectedPath}\\Game\\Content\\Paks";

                    _gameProvider = new NimbusFileProvider(pakFolder, SearchOption.TopDirectoryOnly, true, new VersionContainer(EGame.GAME_AceCombat7));
                    _gameProvider.Initialize();
                    _gameProvider.SubmitKey(new(0U), new FAesKey("68747470733a2f2f616365372e616365636f6d6261742e6a702f737065636961"));

                    LoadTreeView();
                }
            }
        }

        #endregion

        #region Loading

        private void LoadTreeView()
        {
            DataTablesTreeView.BeginUpdate();

            DataTablesTreeView.Nodes.Clear();

            // Load game data tables
            var gamePlayerPlaneDataTable = _gameProvider.GetUasset("Nimbus/Content/Blueprint/Information/PlayerPlaneDataTable.uasset", true);

            DataTableExport dataTable = (DataTableExport)gamePlayerPlaneDataTable.Exports[0];
            UDataTable gameTable = dataTable.Table;
            List<StructPropertyData> gameRows = gameTable.Data;

            for (int i = 0; i < gameRows.Count; i++)
            {
                var row = gameRows[i];
                IntPropertyData planeID = (IntPropertyData)row["PlaneID"];
                StrPropertyData planeStringID = (StrPropertyData)row["PlaneStringID"];
                TreeNode treeNode = new TreeNode(planeStringID.Value.ToString());
                treeNode.Tag = planeID.ToString();
                DataTablesTreeView.Nodes.Add(treeNode);
            }

            DataTablesTreeView.EndUpdate();
        }

        private void DataTablesTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var test = e.Node.Tag as string;

            // Load game data tables
            var gamePlayerPlaneDataTable = _gameProvider.GetUasset("Nimbus/Content/Blueprint/Information/PlayerPlaneDataTable.uasset", true);

            DataTableExport dataTable = (DataTableExport)gamePlayerPlaneDataTable.Exports[0];
            UDataTable gameTable = dataTable.Table;
            List<StructPropertyData> gameRows = gameTable.Data;

            for (int i = 0; i < gameRows.Count; i++)
            {
                StructPropertyData row = gameRows[i];
                IntPropertyData planeID = (IntPropertyData)row["PlaneID"];
                if (planeID.ToString() == test)
                {
                    StructPropertyCollection structPropertyCollection = new StructPropertyCollection();
                    //StructProperties dataTableRow = new StructProperties(row);
                    foreach (var propertyData in row.Value)
                    {
                        structPropertyCollection.Add(propertyData);
                    }
                    propertyGrid1.SelectedObject = structPropertyCollection;

                    break;
                }
            }

        }

        #endregion
    }
}
