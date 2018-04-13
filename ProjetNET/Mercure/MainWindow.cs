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

        public MainWindow()
        {
            this.FormClosing += MainWindow_FormClosing;
            InitializeComponent();

            listView1.FullRowSelect = true;
            listView1.MouseClick += new MouseEventHandler(OnClickArticle);

            LoadDatabase();
        }

        private void OnClickArticle(object sender, MouseEventArgs e)
        {
            if (e.Clicks == 2)
            {
                if (listView1.SelectedItems.Count == 1)
                {
                    ListViewItem Item = listView1.SelectedItems[0];
                    UpdateArticle((Article)Item.Tag);
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                ContextMenu ContextMenu = new ContextMenu();

                if (listView1.SelectedItems.Count == 1)
                {
                    ListViewItem Item = listView1.SelectedItems[0];
                    MenuItem MenuAdd = new MenuItem("Add article");
                    MenuAdd.Click += new EventHandler((o, evt) =>
                    {
                        AddArticle AddArticle = new AddArticle();
                        AddArticle.ShowDialog();
                        LoadDatabase();
                    });
                    MenuItem MenuMod = new MenuItem("Edit article");
                    MenuMod.Click += new EventHandler((o, evt) =>
                    {
                        AddArticle AddArticle = new AddArticle((Article)Item.Tag);
                        AddArticle.ShowDialog();
                        LoadDatabase();
                    });
                    MenuItem MenuDel = new MenuItem("Delete article");
                    MenuDel.Click += new EventHandler((o, evt) =>
                    {
                        DBConnect.GetInstance().DeleteArticle(((Article)Item.Tag).Reference);
                        LoadDatabase();
                    });

                    ContextMenu.MenuItems.Add(MenuAdd);
                    ContextMenu.MenuItems.Add(MenuMod);
                    ContextMenu.MenuItems.Add(MenuDel);
                    
                }

                MenuItem MenuRfh = new MenuItem("Refresh");
                MenuRfh.Click += new EventHandler((o, evt) => LoadDatabase());
                ContextMenu.MenuItems.Add(MenuRfh);

                ContextMenu.Show(this, e.Location);
            }
        }

        private void LoadDatabase()
        {
            listView1.Columns.Clear();
            listView1.Items.Clear();
            SQLiteDataAdapter Adapter = new SQLiteDataAdapter("SELECT Marques.RefMarque AS RefMarque, SousFamilles.RefSousFamille AS RefSousFamille, Articles.RefArticle AS RefArticle, Articles.Description AS Description, Articles.PrixHT AS PrixHT, Articles.Quantite AS Quantite, SousFamilles.Nom AS SousFamille, Familles.Nom AS Famille, Marques.Nom AS Marque FROM Articles LEFT JOIN Marques ON Articles.RefMarque = Marques.RefMarque LEFT JOIN SousFamilles ON Articles.RefSousFamille = SousFamilles.RefSousFamille LEFT JOIN Familles ON SousFamilles.RefFamille = Familles.RefFamille", DBConnect.GetInstance().GetConnection());
            DataTable Dt = new DataTable();
            Adapter.Fill(Dt);

            listView1.KeyDown += new KeyEventHandler(OnListViewKeyDown);
            listView1.Sorting = System.Windows.Forms.SortOrder.Ascending;
            listView1.ColumnClick += new ColumnClickEventHandler(OnColumnClick);
            listView1.ListViewItemSorter = new ListViewItemComparer();

            listView1.Columns.Add("RefArticle");
            listView1.Columns.Add("Description");
            listView1.Columns.Add("Family");
            listView1.Columns.Add("Sub Family");
            listView1.Columns.Add("Brand");
            listView1.Columns.Add("Price excluding VAT (€)");
            listView1.Columns.Add("Quantity");

            for (int i = 0; i < Dt.Rows.Count; i++)
            {
                DataRow Dr = Dt.Rows[i];
                ListViewItem ListItem = new ListViewItem(Dr["RefArticle"].ToString());
                ListItem.SubItems.Add(Dr["Description"].ToString());
                ListItem.SubItems.Add(Dr["Famille"].ToString());
                ListItem.SubItems.Add(Dr["SousFamille"].ToString());
                ListItem.SubItems.Add(Dr["Marque"].ToString());
                ListItem.SubItems.Add(Dr["PrixHT"].ToString());
                ListItem.SubItems.Add(Dr["Quantite"].ToString());
                ListItem.Tag = new Article(
                    Dr["RefArticle"].ToString(),
                    Dr["Description"].ToString(),
                    Convert.ToInt64(Dr["RefSousFamille"].ToString()),
                    Convert.ToInt64(Dr["RefMarque"].ToString()),
                    double.Parse(Dr["PrixHT"].ToString()),
                    Convert.ToInt64(Dr["Quantite"].ToString())
                );
                listView1.Items.Add(ListItem);
            }

            Adapter.Dispose();
        }

        private void OnListViewKeyDown(object sender, KeyEventArgs e)
        {
            if ((Keys)e.KeyCode == Keys.Enter)
            {
                if (listView1.SelectedItems.Count == 1)
                {
                    ListViewItem Item = listView1.SelectedItems[0];
                    UpdateArticle((Article)Item.Tag);
                }
            }
            else if ((Keys)e.KeyCode == Keys.F5)
            {
                LoadDatabase();
            }
            else if ((Keys)e.KeyCode == Keys.Delete)
            {
                for (int i = 0; i < listView1.SelectedItems.Count; i++)
                {
                    ListViewItem Item = listView1.SelectedItems[i];
                    DBConnect.GetInstance().DeleteArticle(((Article)Item.Tag).Reference);
                }
                LoadDatabase();
            }
        }

        private void UpdateArticle(Article article)
        {
            AddArticle AddArticle = new AddArticle(article);
            AddArticle.ShowDialog();
            LoadDatabase();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {

        }

        private void SelectionXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Integration MyIntegration = new Integration();
            MyIntegration.ShowDialog();
            LoadDatabase();
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
                    case 5:
                        return (Inverted ? -1 : 1) * (int)(double.Parse(((ListViewItem)x).SubItems[Col].Text, System.Globalization.CultureInfo.CurrentUICulture) - double.Parse(((ListViewItem)y).SubItems[Col].Text, System.Globalization.CultureInfo.CurrentUICulture));
                    default:
                        return (Inverted ? -1 : 1) * String.Compare(((ListViewItem)x).SubItems[Col].Text, ((ListViewItem)y).SubItems[Col].Text);
                }
            }
        }

    }
}
