namespace NimbusMergerGUI
{
    partial class Launcher
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            LauncherLabelGameDir = new Label();
            LauncherTextBoxGameDir = new TextBox();
            LauncherLabelModsDir = new Label();
            LauncherTextBoxModsDir = new TextBox();
            LauncherLabelExportDir = new Label();
            LauncherTextBoxExportDir = new TextBox();
            LauncherButtonGameDir = new Button();
            LauncherButtonModsDir = new Button();
            LauncherButtonExportDir = new Button();
            LauncherButtonCancel = new Button();
            LauncherButtonOk = new Button();
            SuspendLayout();
            // 
            // LauncherLabelGameDir
            // 
            LauncherLabelGameDir.AutoSize = true;
            LauncherLabelGameDir.Location = new Point(12, 9);
            LauncherLabelGameDir.Name = "LauncherLabelGameDir";
            LauncherLabelGameDir.Size = new Size(108, 15);
            LauncherLabelGameDir.TabIndex = 0;
            LauncherLabelGameDir.Text = "Path to game files :";
            // 
            // LauncherTextBoxGameDir
            // 
            LauncherTextBoxGameDir.Location = new Point(12, 27);
            LauncherTextBoxGameDir.Name = "LauncherTextBoxGameDir";
            LauncherTextBoxGameDir.Size = new Size(530, 23);
            LauncherTextBoxGameDir.TabIndex = 1;
            LauncherTextBoxGameDir.TextChanged += LauncherTextBoxGameDir_TextChanged;
            // 
            // LauncherLabelModsDir
            // 
            LauncherLabelModsDir.AutoSize = true;
            LauncherLabelModsDir.Location = new Point(12, 53);
            LauncherLabelModsDir.Name = "LauncherLabelModsDir";
            LauncherLabelModsDir.Size = new Size(118, 15);
            LauncherLabelModsDir.TabIndex = 2;
            LauncherLabelModsDir.Text = "Path to mods folder :";
            // 
            // LauncherTextBoxModsDir
            // 
            LauncherTextBoxModsDir.Location = new Point(12, 71);
            LauncherTextBoxModsDir.Name = "LauncherTextBoxModsDir";
            LauncherTextBoxModsDir.Size = new Size(530, 23);
            LauncherTextBoxModsDir.TabIndex = 3;
            LauncherTextBoxModsDir.TextChanged += LauncherTextBoxModsDir_TextChanged;
            // 
            // LauncherLabelExportDir
            // 
            LauncherLabelExportDir.AutoSize = true;
            LauncherLabelExportDir.Location = new Point(12, 97);
            LauncherLabelExportDir.Name = "LauncherLabelExportDir";
            LauncherLabelExportDir.Size = new Size(74, 15);
            LauncherLabelExportDir.TabIndex = 4;
            LauncherLabelExportDir.Text = "Export Path :";
            // 
            // LauncherTextBoxExportDir
            // 
            LauncherTextBoxExportDir.Location = new Point(12, 115);
            LauncherTextBoxExportDir.Name = "LauncherTextBoxExportDir";
            LauncherTextBoxExportDir.Size = new Size(530, 23);
            LauncherTextBoxExportDir.TabIndex = 5;
            LauncherTextBoxExportDir.TextChanged += LauncherTextBoxExportDir_TextChanged;
            // 
            // LauncherButtonGameDir
            // 
            LauncherButtonGameDir.Location = new Point(548, 27);
            LauncherButtonGameDir.Name = "LauncherButtonGameDir";
            LauncherButtonGameDir.Size = new Size(28, 23);
            LauncherButtonGameDir.TabIndex = 6;
            LauncherButtonGameDir.Text = "...";
            LauncherButtonGameDir.UseVisualStyleBackColor = true;
            LauncherButtonGameDir.Click += LauncherButtonGameDir_Click;
            // 
            // LauncherButtonModsDir
            // 
            LauncherButtonModsDir.Location = new Point(548, 71);
            LauncherButtonModsDir.Name = "LauncherButtonModsDir";
            LauncherButtonModsDir.Size = new Size(28, 23);
            LauncherButtonModsDir.TabIndex = 7;
            LauncherButtonModsDir.Text = "...";
            LauncherButtonModsDir.UseVisualStyleBackColor = true;
            LauncherButtonModsDir.Click += LauncherButtonModsDir_Click;
            // 
            // LauncherButtonExportDir
            // 
            LauncherButtonExportDir.Location = new Point(548, 114);
            LauncherButtonExportDir.Name = "LauncherButtonExportDir";
            LauncherButtonExportDir.Size = new Size(28, 23);
            LauncherButtonExportDir.TabIndex = 8;
            LauncherButtonExportDir.Text = "...";
            LauncherButtonExportDir.UseVisualStyleBackColor = true;
            LauncherButtonExportDir.Click += LauncherButtonExportDir_Click;
            // 
            // LauncherButtonCancel
            // 
            LauncherButtonCancel.Location = new Point(476, 156);
            LauncherButtonCancel.Name = "LauncherButtonCancel";
            LauncherButtonCancel.Size = new Size(100, 42);
            LauncherButtonCancel.TabIndex = 9;
            LauncherButtonCancel.Text = "Cancel";
            LauncherButtonCancel.UseVisualStyleBackColor = true;
            LauncherButtonCancel.Click += LauncherButtonCancel_Click;
            // 
            // LauncherButtonOk
            // 
            LauncherButtonOk.Location = new Point(370, 156);
            LauncherButtonOk.Name = "LauncherButtonOk";
            LauncherButtonOk.Size = new Size(100, 42);
            LauncherButtonOk.TabIndex = 10;
            LauncherButtonOk.Text = "OK";
            LauncherButtonOk.UseVisualStyleBackColor = true;
            LauncherButtonOk.Click += LauncherButtonOk_Click;
            // 
            // Launcher
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(589, 210);
            Controls.Add(LauncherButtonOk);
            Controls.Add(LauncherButtonCancel);
            Controls.Add(LauncherButtonExportDir);
            Controls.Add(LauncherButtonModsDir);
            Controls.Add(LauncherButtonGameDir);
            Controls.Add(LauncherTextBoxExportDir);
            Controls.Add(LauncherLabelExportDir);
            Controls.Add(LauncherTextBoxModsDir);
            Controls.Add(LauncherLabelModsDir);
            Controls.Add(LauncherTextBoxGameDir);
            Controls.Add(LauncherLabelGameDir);
            Name = "Launcher";
            Text = "Launcher";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label LauncherLabelGameDir;
        private TextBox LauncherTextBoxGameDir;
        private Label LauncherLabelModsDir;
        private TextBox LauncherTextBoxModsDir;
        private Label LauncherLabelExportDir;
        private TextBox LauncherTextBoxExportDir;
        private Button LauncherButtonGameDir;
        private Button LauncherButtonModsDir;
        private Button LauncherButtonExportDir;
        private Button LauncherButtonCancel;
        private Button LauncherButtonOk;
    }
}