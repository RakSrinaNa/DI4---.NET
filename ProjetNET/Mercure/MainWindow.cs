using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Data.SQLite;
using System.Data.SqlClient;

namespace ProjetNET
{
    public partial class MainWindow : Form
    {
        private String XMLFilePath;

        public MainWindow()
        {
            this.FormClosing += MainWindow_FormClosing;
            InitializeComponent();
            SQLiteDataAdapter Adapter = new SQLiteDataAdapter("SELECT * FROM Articles", DBConnect.GetInstance().GetConnection());
            DataTable Dt = new DataTable();
            Adapter.Fill(Dt);
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
            DBConnect.GetInstance().Close();
        }
    }
}
