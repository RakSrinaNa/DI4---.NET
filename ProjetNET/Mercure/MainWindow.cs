using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ProjetNET
{
    public partial class MainWindow : Form
    {
        private String XMLFilePath;

        public MainWindow()
        {
            InitializeComponent();
            Integration MyIntegration = new Integration();
            MyIntegration.Show();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {

        }


        private void SelectionXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {

            Integration MyIntegration = new Integration();
            MyIntegration.Show();

            OpenFileDialog MyFileDialog = new OpenFileDialog();
            MyFileDialog.Filter = "XML Files|*.xml";
            DialogResult HasClickedOK = MyFileDialog.ShowDialog();
            if (HasClickedOK == DialogResult.OK)
            {
                XMLFilePath = MyFileDialog.FileName;
                ToolStripStatusLabel1.Text = "";
            }
        }

        private void IntegrationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (XMLFilePath != null)
            {
                ToolStripProgressBar1.Value = 0;
                while (ToolStripProgressBar1.Value < 100)
                {
                    ToolStripProgressBar1.Value++;
                    System.Threading.Thread.Sleep(10);
                }
            }
            else
            {
                ToolStripStatusLabel1.Text = "Please select a file bro/biatch!";
            }
        }
    }
}
