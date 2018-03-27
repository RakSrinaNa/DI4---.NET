namespace ProjetNET
{
    partial class MainWindow
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.MainStatusStrip = new System.Windows.Forms.StatusStrip();
            this.ToolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.MyMainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.FileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SelectionXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.IntegrationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listView1 = new System.Windows.Forms.ListView();
            this.MainStatusStrip.SuspendLayout();
            this.MyMainMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainStatusStrip
            // 
            this.MainStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripStatusLabel1});
            this.MainStatusStrip.Location = new System.Drawing.Point(0, 385);
            this.MainStatusStrip.Name = "MainStatusStrip";
            this.MainStatusStrip.Size = new System.Drawing.Size(506, 22);
            this.MainStatusStrip.TabIndex = 0;
            this.MainStatusStrip.Text = "statusStrip1";
            // 
            // ToolStripStatusLabel1
            // 
            this.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1";
            this.ToolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // MyMainMenuStrip
            // 
            this.MyMainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileToolStripMenuItem,
            this.IntegrationToolStripMenuItem});
            this.MyMainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.MyMainMenuStrip.Name = "MyMainMenuStrip";
            this.MyMainMenuStrip.Size = new System.Drawing.Size(506, 24);
            this.MyMainMenuStrip.TabIndex = 1;
            this.MyMainMenuStrip.Text = "menuStrip1";
            // 
            // FileToolStripMenuItem
            // 
            this.FileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SelectionXMLToolStripMenuItem});
            this.FileToolStripMenuItem.Name = "FileToolStripMenuItem";
            this.FileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.FileToolStripMenuItem.Text = "File";
            // 
            // SelectionXMLToolStripMenuItem
            // 
            this.SelectionXMLToolStripMenuItem.Name = "SelectionXMLToolStripMenuItem";
            this.SelectionXMLToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.SelectionXMLToolStripMenuItem.Text = "XML Selection";
            this.SelectionXMLToolStripMenuItem.Click += new System.EventHandler(this.SelectionXMLToolStripMenuItem_Click);
            // 
            // IntegrationToolStripMenuItem
            // 
            this.IntegrationToolStripMenuItem.Name = "IntegrationToolStripMenuItem";
            this.IntegrationToolStripMenuItem.Size = new System.Drawing.Size(77, 20);
            this.IntegrationToolStripMenuItem.Text = "Integration";
            this.IntegrationToolStripMenuItem.Click += new System.EventHandler(this.IntegrationToolStripMenuItem_Click);
            // 
            // listView1
            // 
            this.listView1.Location = new System.Drawing.Point(12, 27);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(482, 355);
            this.listView1.TabIndex = 2;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(506, 407);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.MainStatusStrip);
            this.Controls.Add(this.MyMainMenuStrip);
            this.Name = "MainWindow";
            this.Text = "Mercure";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.MainStatusStrip.ResumeLayout(false);
            this.MainStatusStrip.PerformLayout();
            this.MyMainMenuStrip.ResumeLayout(false);
            this.MyMainMenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip MainStatusStrip;
        private System.Windows.Forms.MenuStrip MyMainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem FileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SelectionXMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem IntegrationToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel ToolStripStatusLabel1;
        private System.Windows.Forms.ListView listView1;
    }
}

