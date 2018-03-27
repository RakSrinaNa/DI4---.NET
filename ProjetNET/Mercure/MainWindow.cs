using System;
using System.Collections;
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

            listView1.Sorting = System.Windows.Forms.SortOrder.Ascending;
            listView1.ColumnClick += new ColumnClickEventHandler(OnColumnClick);
            listView1.ListViewItemSorter = new ListViewItemComparer();

            listView1.Columns.Add("RefArticle");
            listView1.Columns.Add("Description");
            listView1.Columns.Add("RefSousFamille");
            listView1.Columns.Add("RefMarque");
            listView1.Columns.Add("Prix HT");
            listView1.Columns.Add("Quantité");

            for (int i = 0; i < Dt.Rows.Count; i++)
            {
                DataRow Dr = Dt.Rows[i];
                ListViewItem ListItem = new ListViewItem(Dr["RefArticle"].ToString());
                ListItem.SubItems.Add(Dr["Description"].ToString());
                ListItem.SubItems.Add(Dr["RefSousFamille"].ToString());
                ListItem.SubItems.Add(Dr["RefMarque"].ToString());
                ListItem.SubItems.Add(Dr["PrixHT"].ToString());
                ListItem.SubItems.Add(Dr["Quantite"].ToString());
                listView1.Items.Add(ListItem);
            }

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

        private void OnColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListViewItemComparer LastComparer = (ListViewItemComparer)listView1.ListViewItemSorter;
            if (LastComparer.GetCol() == e.Column)
            {
                LastComparer.Invert();
                listView1.Sort();
            }
            else
            {
                listView1.ListViewItemSorter = new ListViewItemComparer(e.Column);
            }
        }

        class ListViewItemComparer : IComparer
        {
            private int Col;
            private bool Inverted;
            public ListViewItemComparer()
            {
                Col = 0;
                Inverted = false;
            }

            public int GetCol()
            {
                return Col;
            }

            public void Invert()
            {
                Inverted = !Inverted;
            }

            public ListViewItemComparer(int ArgCol)
            {
                Col = ArgCol;
                Inverted = false;
            }

            public int Compare(object x, object y)
            {
                switch (Col)
                {
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                        return (Inverted ? -1 : 1) * (int)(double.Parse(((ListViewItem)x).SubItems[Col].Text, System.Globalization.CultureInfo.InvariantCulture) - double.Parse(((ListViewItem)y).SubItems[Col].Text, System.Globalization.CultureInfo.InvariantCulture));
                    default:
                        return (Inverted ? -1 : 1) * String.Compare(((ListViewItem)x).SubItems[Col].Text, ((ListViewItem)y).SubItems[Col].Text);
                }
            }
        }

    }
}
