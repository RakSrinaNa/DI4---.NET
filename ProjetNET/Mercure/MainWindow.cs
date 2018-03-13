using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace ProjetNET
{
    public partial class MainWindow : Form
    {
        private String XMLFilePath;
        private DBConnect DB;

        public MainWindow()
        {
            this.FormClosing += MainWindow_FormClosing;
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {

        }


        private void SelectionXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Integration MyIntegration = new Integration();
            MyIntegration.ShowDialog();
        }

        private void IntegrationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (XMLFilePath != null)
            {
               
            }
            else
            {
                ToolStripStatusLabel1.Text = "Please select a file bro/biatch!";
            }
        }

        private void MainWindow_FormClosing(object sender, EventArgs e)
        {
            DB.Close();
        }
    }
}
