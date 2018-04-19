using System;
using System.Collections;
using System.Data;
using System.Windows.Forms;
using System.Data.SQLite;

namespace ProjetNET
{
    /// <inheritdoc />
    /// <summary>
    /// Main window of the application
    /// </summary>
    public partial class MainWindow : Form
    {
        public static event BottomBarEventHandler BottomBarEvent;
        public delegate void BottomBarEventHandler(string Text);
        private int _SortColumn;

        /// <summary>
        /// Update the text in the bottom bar
        /// </summary>
        /// <param name="Text">The new text to display</param>
        public static void ChangeStripText(string Text) 
        {
            if(BottomBarEvent != null)
                BottomBarEvent.Invoke(Text);
        }

        /// <inheritdoc />
        /// <summary>
        /// Default constructor of the main window
        /// </summary>
        public MainWindow()
        {
            FormClosing += MainWindow_FormClosing;
            InitializeComponent();

            BottomBarEvent += (Text) => ToolStripStatusLabel1.Text = Text;

            listView1.FullRowSelect = true;
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listView1.MouseClick += OnClickArticle;

            LoadDatabase();

            WindowState = FormWindowState.Maximized;
        }

        /// <summary>
        /// Define what to do when a click occurs on a element in the list of the articles
        /// </summary>
        /// <param name="Sender">The object sending the event</param>
        /// <param name="Event">The event</param>
        private void OnClickArticle(object Sender, MouseEventArgs Event)
        {
            if (Event.Clicks == 2)
            {
                if (listView1.SelectedItems.Count == 1)
                {
                    ListViewItem Item = listView1.SelectedItems[0];
                    UpdateArticle((Article)Item.Tag);
                }
            }
            else if (Event.Button == MouseButtons.Right)
            {
                ContextMenu ContextMenu1 = new ContextMenu();

                if (listView1.SelectedItems.Count == 1)
                {
                    //Create context menu
                    ListViewItem Item = listView1.SelectedItems[0];
                    MenuItem MenuAdd = new MenuItem("Add article");
                    MenuAdd.Click += (Sender2, Event2) =>
                    {
                        UpdateArticle(null);
                    };
                    MenuItem MenuMod = new MenuItem("Edit article");
                    MenuMod.Click += (Sender2, Event2) =>
                    {
                        UpdateArticle((Article)Item.Tag);
                    };
                    MenuItem MenuDel = new MenuItem("Delete article");
                    MenuDel.Click += (Sender2, Event2) =>
                    {
                        DbConnect.GetInstance().DeleteArticle(((Article)Item.Tag).Reference);
                        LoadDatabase();
                    };

                    ContextMenu1.MenuItems.Add(MenuAdd);
                    ContextMenu1.MenuItems.Add(MenuMod);
                    ContextMenu1.MenuItems.Add(MenuDel);
                    
                }

                MenuItem MenuRfh = new MenuItem("Refresh");
                MenuRfh.Click += (Sender2, Event2) => LoadDatabase();
                ContextMenu1.MenuItems.Add(MenuRfh);

                ContextMenu1.Show(this, Event.Location);
            }
        }

        /// <summary>
        /// Reload (or not) the database, then group the data
        /// </summary>
        /// <param name="ShouldReloadData"></param>
        private void LoadDatabase(bool ShouldReloadData = true)
        {
            if (ShouldReloadData)
            {
                listView1.Columns.Clear();
                listView1.Items.Clear();
                SQLiteDataAdapter Adapter = new SQLiteDataAdapter("SELECT Marques.RefMarque AS RefMarque, SousFamilles.RefSousFamille AS RefSousFamille, Articles.RefArticle AS RefArticle, Articles.Description AS Description, Articles.PrixHT AS PrixHT, Articles.Quantite AS Quantite, SousFamilles.Nom AS SousFamille, Familles.Nom AS Famille, Marques.Nom AS Marque FROM Articles LEFT JOIN Marques ON Articles.RefMarque = Marques.RefMarque LEFT JOIN SousFamilles ON Articles.RefSousFamille = SousFamilles.RefSousFamille LEFT JOIN Familles ON SousFamilles.RefFamille = Familles.RefFamille", DbConnect.GetInstance().GetConnection());
                DataTable Dt = new DataTable();
                Adapter.Fill(Dt);

                listView1.KeyDown += OnListViewKeyDown;
                listView1.Sorting = SortOrder.Ascending;
                listView1.ColumnClick += OnColumnClick;
                listView1.ListViewItemSorter = new ListViewItemComparer();

                //Create columns
                listView1.Columns.Add("RefArticle");
                listView1.Columns.Add("Description");
                listView1.Columns.Add("Family");
                listView1.Columns.Add("Sub Family");
                listView1.Columns.Add("Brand");
                listView1.Columns.Add("Price excluding VAT (€)");
                listView1.Columns.Add("Quantity");

                //Add items
                for (int I = 0; I < Dt.Rows.Count; I++)
                {
                    DataRow Dr = Dt.Rows[I];
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

            //Resize columns
            listView1.Columns[0].Width = -2;
            listView1.Columns[1].Width = -2;
            listView1.Columns[2].Width = -2;
            listView1.Columns[3].Width = -2;
            listView1.Columns[4].Width = -2;
            listView1.Columns[5].Width = -2;
            listView1.Columns[6].Width = -2;

            if (_SortColumn == 0 || _SortColumn == 1) //Hide or show groups
            {
                listView1.ShowGroups = false;
            }
            else
            {
                listView1.ShowGroups = true;
                foreach (ListViewItem ListItem in listView1.Items) //Set group for each item
                {
                    ListItem.Group = null;
                    ListViewGroup Group = null;
                    foreach (ListViewGroup GroupTest in listView1.Groups)
                    {
                        if (GroupTest.Header == ListItem.SubItems[_SortColumn].Text)
                        {
                            Group = GroupTest;
                            break;
                        }
                    }

                    if (Group == null)
                    {
                        Group = new ListViewGroup();
                        Group.Name = ListItem.SubItems[_SortColumn].Text;
                        Group.Header = ListItem.SubItems[_SortColumn].Text;
                        listView1.Groups.Add(Group);
                    }

                    Group = listView1.Groups[ListItem.SubItems[_SortColumn].Text];
                    Group.Items.Add(ListItem);
                    ListItem.Group = Group;
                }
            }
        }

        /// <summary>
        /// Define what to do when a keyboard key is pressed
        /// </summary>
        /// <param name="Sender">The object sending the event</param>
        /// <param name="Event">The event</param>
        private void OnListViewKeyDown(object Sender, KeyEventArgs Event)
        {
            if (Event.Handled)
                return;
            if (Event.KeyCode == Keys.Enter)
            {
                if (listView1.SelectedItems.Count == 1)
                {
                    ListViewItem Item = listView1.SelectedItems[0];
                    UpdateArticle((Article)Item.Tag);
                }
            }
            else if (Event.KeyCode == Keys.F5)
            {
                LoadDatabase();
            }
            else if (Event.KeyCode == Keys.Delete)
            {
                for (int I = 0; I < listView1.SelectedItems.Count; I++)
                {
                    ListViewItem Item = listView1.SelectedItems[I];
                    DbConnect.GetInstance().DeleteArticle(((Article)Item.Tag).Reference);
                }
                LoadDatabase();
            }
            Event.Handled = true;
        }

        /// <summary>
        /// Open a window to edit the given article
        /// </summary>
        /// <param name="Article">The article</param>
        private void UpdateArticle(Article Article)
        {
            AddArticle AddArticle = new AddArticle(Article);
            if(AddArticle.ShowDialog() == DialogResult.OK)
                LoadDatabase();
        }

        /// <summary>
        /// Open the integration window
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="Event"></param>
        private void SelectionXMLToolStripMenuItem_Click(object Sender, EventArgs Event)
        {
            Integration MyIntegration = new Integration();
            MyIntegration.ShowDialog();
            LoadDatabase();
        }

        /// <summary>
        /// Close the database when the application is closed
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="Event"></param>
        private static void MainWindow_FormClosing(object Sender, EventArgs Event)
        {
            DbConnect.GetInstance().Close();
        }

        /// <summary>
        /// Define what to do when a click occurs on a column
        /// </summary>
        /// <param name="Sender">The object sending the event</param>
        /// <param name="Event">The event</param>
        private void OnColumnClick(object Sender, ColumnClickEventArgs Event)
        {
            ListViewItemComparer LastComparer = (ListViewItemComparer)listView1.ListViewItemSorter;
            if (LastComparer.GetCol() == Event.Column) //If the column is the one already being sorted.
            {
                LastComparer.Invert();
                listView1.Sort();
            }
            else
            {
                _SortColumn = Event.Column;
                LoadDatabase(false);
                listView1.ListViewItemSorter = new ListViewItemComparer(Event.Column);
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Class to sort a list depending of a given column
        /// </summary>
        class ListViewItemComparer : IComparer
        {
            private readonly int _Col;
            private bool _Inverted;

            /// <summary>
            /// Default constructor
            /// </summary>
            public ListViewItemComparer()
            {
                _Col = 0;
                _Inverted = false;
            }

            /// <summary>
            /// Return the number of the actual sorting column
            /// </summary>
            /// <returns>The number of the actual sorting column</returns>
            public int GetCol()
            {
                return _Col;
            }

            /// <summary>
            /// Invert (asc / desc) the actual sorting
            /// </summary>
            public void Invert()
            {
                _Inverted = !_Inverted;
            }

            /// <summary>
            /// Constructor with a given column
            /// </summary>
            /// <param name="ArgCol">The sorting column</param>
            public ListViewItemComparer(int ArgCol)
            {
                _Col = ArgCol;
                _Inverted = false;
            }

            /// <inheritdoc />
            /// <summary>
            /// Compare two items in the list
            /// </summary>
            /// <param name="X">The first item</param>
            /// <param name="Y">The second item</param>
            /// <returns>result is negative if X before Y, positive if X after Y, 0 if equal</returns>
            public int Compare(object X, object Y)
            {
                switch (_Col)
                {
                    case 5: //If it's the price
                    case 6: //If it's the price
                        return (_Inverted ? -1 : 1) * (int)(double.Parse(((ListViewItem)X).SubItems[_Col].Text, System.Globalization.CultureInfo.CurrentUICulture) - double.Parse(((ListViewItem)Y).SubItems[_Col].Text, System.Globalization.CultureInfo.CurrentUICulture));
                    default:
                        // ReSharper disable once StringCompareIsCultureSpecific.1
                        return (_Inverted ? -1 : 1) * String.Compare(((ListViewItem)X).SubItems[_Col].Text, ((ListViewItem)Y).SubItems[_Col].Text);
                }
            }
        }

        /// <summary>
        /// Open the Brand window when a click occurs on the menu action 'Brand'
        /// </summary>
        /// <param name="Sender">The object sending the event</param>
        /// <param name="Event">The event</param>
        private void BrandsToolStripMenuItem_Click(object Sender, EventArgs Event)
        {
            Brands Brands = new Brands();
            Brands.ShowDialog();
            LoadDatabase();
        }

        /// <summary>
        /// Open the Family window when a click occurs on the menu action 'Family'
        /// </summary>
        /// <param name="Sender">The object sending the event</param>
        /// <param name="Event">The event</param>
        private void FamiliesToolStripMenuItem_Click(object Sender, EventArgs Event)
        {
            Families Families = new Families();
            Families.ShowDialog();
            LoadDatabase();
        }

        /// <summary>
        /// Open the Subfamily window when a click occurs on the menu action 'Subfamily'
        /// </summary>
        /// <param name="Sender">The object sending the event</param>
        /// <param name="Event">The event</param>
        private void SubFamiliesToolStripMenuItem_Click(object Sender, EventArgs Event)
        {
            SubFamilies SubFamilies = new SubFamilies();
            SubFamilies.ShowDialog();
            LoadDatabase();
        }
    }
}
