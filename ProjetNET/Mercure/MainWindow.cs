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
    /// <summary>
    /// Main window of the application
    /// </summary>
    public partial class MainWindow : Form
    {
        public static event BottomBarEventHandler BottomBarEvent;
        public delegate void BottomBarEventHandler(String Text);
        private int SortColumn;

        /// <summary>
        /// Update the text in the bottom bar
        /// </summary>
        /// <param name="Text">The new text to display</param>
        public static void ChangeStripText(String Text) 
        {
            if(BottomBarEvent != null)
                BottomBarEvent.Invoke(Text);
        }

        /// <summary>
        /// Default constructor of the main window
        /// </summary>
        public MainWindow()
        {
            this.FormClosing += MainWindow_FormClosing;
            InitializeComponent();

            BottomBarEvent += (Text) => ToolStripStatusLabel1.Text = Text;

            listView1.FullRowSelect = true;
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listView1.MouseClick += new MouseEventHandler(OnClickArticle);

            LoadDatabase();

            this.WindowState = FormWindowState.Maximized;
        }

        /// <summary>
        /// Define what to do when a click occurs on a element in the list of the articles
        /// </summary>
        /// <param name="sender">The object sending the event</param>
        /// <param name="e">The event</param>
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
                        UpdateArticle(null);
                    });
                    MenuItem MenuMod = new MenuItem("Edit article");
                    MenuMod.Click += new EventHandler((o, evt) =>
                    {
                       UpdateArticle((Article)Item.Tag);
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

        /// <summary>
        /// Reload all the data in the main list
        /// </summary>
        private void LoadDatabase()
        {
            LoadDatabase(true);
        }

        /// <summary>
        /// Reload (or not) the database, then group the data
        /// </summary>
        /// <param name="ShouldReloadData"></param>
        private void LoadDatabase(bool ShouldReloadData)
        {
            if (ShouldReloadData)
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
            else
            {
                listView1.Groups.Clear();
            }

            listView1.Columns[0].Width = -2;
            listView1.Columns[1].Width = -2;
            listView1.Columns[2].Width = -2;
            listView1.Columns[3].Width = -2;
            listView1.Columns[4].Width = -2;
            listView1.Columns[5].Width = -2;
            listView1.Columns[6].Width = -2;

            if (SortColumn == 0 || SortColumn == 1)
            {
                listView1.ShowGroups = false;
            }
            else
            {
                listView1.ShowGroups = true;
                foreach (ListViewItem ListItem in listView1.Items)
                {
                    ListItem.Group = null;
                    ListViewGroup Group = null;
                    foreach (ListViewGroup GroupTest in listView1.Groups)
                    {
                        if (GroupTest.Header == ListItem.SubItems[SortColumn].Text)
                        {
                            Group = GroupTest;
                            break;
                        }
                    }

                    if (Group == null)
                    {
                        Group = new ListViewGroup();
                        Group.Name = ListItem.SubItems[SortColumn].Text;
                        Group.Header = ListItem.SubItems[SortColumn].Text;
                        listView1.Groups.Add(Group);
                    }

                    Group = listView1.Groups[ListItem.SubItems[SortColumn].Text];
                    Group.Items.Add(ListItem);
                    ListItem.Group = Group;
                }
            }
        }

        /// <summary>
        /// Define what to do when a keyboard key is pressed
        /// </summary>
        /// <param name="sender">The object sending the event</param>
        /// <param name="e">The event</param>
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

        /// <summary>
        /// Open a window to edit the given article
        /// </summary>
        /// <param name="article">The article</param>
        private void UpdateArticle(Article article)
        {
            AddArticle AddArticle = new AddArticle(article);
            if(AddArticle.ShowDialog() == DialogResult.OK)
                LoadDatabase();
        }

        //TODO
        private void MainWindow_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Open the integration window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectionXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Integration MyIntegration = new Integration();
            MyIntegration.ShowDialog();
            LoadDatabase();
        }

        /// <summary>
        /// Close the database when the application is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_FormClosing(object sender, EventArgs e)
        {
            DBConnect.GetInstance().Close();
        }

        /// <summary>
        /// Define what to do when a click occurs on a column
        /// </summary>
        /// <param name="sender">The object sending the event</param>
        /// <param name="e">The event</param>
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
                SortColumn = e.Column;
                LoadDatabase(false);
                listView1.ListViewItemSorter = new ListViewItemComparer(e.Column);
            }
        }

        /// <summary>
        /// Class to sort a list depending of a given column
        /// </summary>
        class ListViewItemComparer : IComparer
        {
            private int Col;
            private bool Inverted;

            /// <summary>
            /// Default constructor
            /// </summary>
            public ListViewItemComparer()
            {
                Col = 0;
                Inverted = false;
            }

            /// <summary>
            /// Return the number of the actual sorting column
            /// </summary>
            /// <returns>The number of the actual sorting column</returns>
            public int GetCol()
            {
                return Col;
            }

            /// <summary>
            /// Invert (asc / desc) the actual sorting
            /// </summary>
            public void Invert()
            {
                Inverted = !Inverted;
            }

            /// <summary>
            /// Constructor with a given column
            /// </summary>
            /// <param name="ArgCol">The sorting column</param>
            public ListViewItemComparer(int ArgCol)
            {
                Col = ArgCol;
                Inverted = false;
            }

            /// <summary>
            /// Compare two items in the list
            /// </summary>
            /// <param name="x">The first item</param>
            /// <param name="y">The second item</param>
            /// <returns>result is negative if X before Y, positive if X after Y, 0 if equal</returns>
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

        /// <summary>
        /// Open the Brand window when a click occurs on the menu action 'Brand'
        /// </summary>
        /// <param name="sender">The object sending the event</param>
        /// <param name="e">The event</param>
        private void BrandsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Brands Brands = new Brands();
            Brands.ShowDialog();
            LoadDatabase();
        }

        /// <summary>
        /// Open the Family window when a click occurs on the menu action 'Family'
        /// </summary>
        /// <param name="sender">The object sending the event</param>
        /// <param name="e">The event</param>
        private void FamiliesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Families Families = new Families();
            Families.ShowDialog();
            LoadDatabase();
        }

        /// <summary>
        /// Open the Subfamily window when a click occurs on the menu action 'Subfamily'
        /// </summary>
        /// <param name="sender">The object sending the event</param>
        /// <param name="e">The event</param>
        private void SubFamiliesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SubFamilies SubFamilies = new SubFamilies();
            SubFamilies.ShowDialog();
            LoadDatabase();
        }
    }
}
