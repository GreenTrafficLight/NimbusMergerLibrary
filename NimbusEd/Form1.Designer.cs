namespace NimbusEd
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            MenuStrip = new MenuStrip();
            MenuStripFile = new ToolStripMenuItem();
            MSFileOpen = new ToolStripMenuItem();
            MSFileSave = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            MSFileExit = new ToolStripMenuItem();
            DataTablesTreeView = new TreeView();
            propertyGrid1 = new PropertyGrid();
            MenuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // MenuStrip
            // 
            MenuStrip.Items.AddRange(new ToolStripItem[] { MenuStripFile });
            MenuStrip.Location = new Point(0, 0);
            MenuStrip.Name = "MenuStrip";
            MenuStrip.Size = new Size(892, 24);
            MenuStrip.TabIndex = 0;
            MenuStrip.Text = "menuStrip1";
            // 
            // MenuStripFile
            // 
            MenuStripFile.DropDownItems.AddRange(new ToolStripItem[] { MSFileOpen, MSFileSave, toolStripSeparator1, MSFileExit });
            MenuStripFile.Name = "MenuStripFile";
            MenuStripFile.Size = new Size(37, 20);
            MenuStripFile.Text = "File";
            // 
            // MSFileOpen
            // 
            MSFileOpen.Name = "MSFileOpen";
            MSFileOpen.Size = new Size(103, 22);
            MSFileOpen.Text = "Open";
            MSFileOpen.Click += MSFileOpen_Click;
            // 
            // MSFileSave
            // 
            MSFileSave.Name = "MSFileSave";
            MSFileSave.Size = new Size(103, 22);
            MSFileSave.Text = "Save";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(100, 6);
            // 
            // MSFileExit
            // 
            MSFileExit.Name = "MSFileExit";
            MSFileExit.Size = new Size(103, 22);
            MSFileExit.Text = "Exit";
            // 
            // DataTablesTreeView
            // 
            DataTablesTreeView.Location = new Point(12, 27);
            DataTablesTreeView.Name = "DataTablesTreeView";
            DataTablesTreeView.Size = new Size(242, 481);
            DataTablesTreeView.TabIndex = 1;
            DataTablesTreeView.AfterSelect += DataTablesTreeView_AfterSelect;
            // 
            // propertyGrid1
            // 
            propertyGrid1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            propertyGrid1.HelpVisible = false;
            propertyGrid1.Location = new Point(260, 27);
            propertyGrid1.Name = "propertyGrid1";
            propertyGrid1.Size = new Size(620, 316);
            propertyGrid1.TabIndex = 2;
            propertyGrid1.ToolbarVisible = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(892, 520);
            Controls.Add(propertyGrid1);
            Controls.Add(DataTablesTreeView);
            Controls.Add(MenuStrip);
            MainMenuStrip = MenuStrip;
            Name = "Form1";
            Text = "NimbusEd";
            MenuStrip.ResumeLayout(false);
            MenuStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip MenuStrip;
        private ToolStripMenuItem MenuStripFile;
        private ToolStripMenuItem MSFileOpen;
        private ToolStripMenuItem MSFileSave;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem MSFileExit;
        private TreeView DataTablesTreeView;
        private PropertyGrid propertyGrid1;
    }
}
