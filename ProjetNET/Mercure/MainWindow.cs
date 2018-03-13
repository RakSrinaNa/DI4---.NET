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
        private DBConnect DB;

        public MainWindow()
        {
            this.FormClosing += MainWindow_FormClosing;
            InitializeComponent();
            DBConnect.GetInstance().LoadXml(Parser.ParseXML("C:\\Users\\Administrateur\\Desktop\\DI4---.NET\\DI4 Plateformes logicielles Net - Sujet TP\\DI4 Plateformes logicielles Net - Sujet TP\\Mercure.xml"));
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {

        }


        private void SelectionXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {

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
