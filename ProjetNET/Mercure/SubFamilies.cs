using System;
using System.Collections;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;

namespace ProjetNET
{
    /// <inheritdoc />
    /// <summary>
    /// The window to display the list of subfamilies
    /// </summary>
    public partial class SubFamilies : Form
    {
        private int _SortColumn;

        /// <inheritdoc />
        /// <summary>
        /// Constructor by default
        /// </summary>
        public SubFamilies()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterParent;

            listView1.FullRowSelect = true;
            listView1.MouseClick += OnClickSubFamily;

            LoadDatabase();
        }

        /// <summary>
        /// Define what to do when a click occurs on a element in the list
        /// </summary>
        /// <param name="Sender">The object sending the event</param>
        /// <param name="Event">The mouse event</param>
        private void OnClickSubFamily(object Sender, MouseEventArgs Event)
        {
            if (Event.Clicks == 2)
            {
                if (listView1.SelectedItems.Count == 1)
                {
                    ListViewItem Item = listView1.SelectedItems[0];
                    UpdateSubFamily((SubFamily) Item.Tag);
                }
            }
            else if (Event.Button == MouseButtons.Right)
            {
                ContextMenu ContextMenu1 = new ContextMenu();

                if (listView1.SelectedItems.Count == 1)
                {
                    ListViewItem Item = listView1.SelectedItems[0];
                    MenuItem MenuAdd = new MenuItem("Add sub family");
                    MenuAdd.Click += (Sender2, Event2) => { UpdateSubFamily(null); };
                    MenuItem MenuMod = new MenuItem("Edit sub family");
                    MenuMod.Click += (Sender2, Event2) => { UpdateSubFamily((SubFamily) Item.Tag); };
                    MenuItem MenuDel = new MenuItem("Delete sub family");
                    MenuDel.Click += (Sender2, Event2) =>
                    {
                        DbConnect.GetInstance().DeleteSubFamily(((SubFamily) Item.Tag).Reference);
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
                SQLiteDataAdapter Adapter = new SQLiteDataAdapter(
                    "SELECT RefSousFamille, RefFamille, Nom FROM SousFamilles",
                    DbConnect.GetInstance().GetConnection());
                DataTable Dt = new DataTable();
                Adapter.Fill(Dt);

                listView1.KeyDown += OnListViewKeyDown;
                listView1.Sorting = SortOrder.Ascending;
                listView1.ColumnClick += OnColumnClick;
                listView1.ListViewItemSorter = new ListViewItemComparer();

                listView1.Columns.Add("RefSousFamille");
                listView1.Columns.Add("RefFamille");
                listView1.Columns.Add("Nom");

                for (int I = 0; I < Dt.Rows.Count; I++)
                {
                    DataRow Dr = Dt.Rows[I];
                    ListViewItem ListItem = new ListViewItem(Dr["RefSousFamille"].ToString());
                    ListItem.SubItems.Add(Dr["RefFamille"].ToString());
                    ListItem.SubItems.Add(Dr["Nom"].ToString());
                    ListItem.Tag = new SubFamily(
                        Convert.ToInt64(Dr["RefSousFamille"].ToString()),
                        Convert.ToInt64(Dr["RefFamille"].ToString()),
                        Dr["Nom"].ToString()
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

            if (_SortColumn == 0 || _SortColumn == 2)
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
        /// Sort the list depending of the clicked column
        /// </summary>
        /// <param name="Sender">The object sending the event</param>
        /// <param name="Event">The event</param>
        private void OnColumnClick(object Sender, ColumnClickEventArgs Event)
        {
            ListViewItemComparer LastComparer = (ListViewItemComparer) listView1.ListViewItemSorter;
            if (LastComparer.GetCol() == Event.Column)
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
        private class ListViewItemComparer : IComparer
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
                    case 0:
                    case 1:
                        return (_Inverted ? -1 : 1) *
                               (int) (long.Parse(((ListViewItem) X).SubItems[_Col].Text,
                                          System.Globalization.CultureInfo.CurrentUICulture) -
                                      long.Parse(((ListViewItem) Y).SubItems[_Col].Text,
                                          System.Globalization.CultureInfo.CurrentUICulture));
                    default:
                        // ReSharper disable once StringCompareIsCultureSpecific.1
                        return (_Inverted ? -1 : 1) * String.Compare(((ListViewItem) X).SubItems[_Col].Text,
                                   ((ListViewItem) Y).SubItems[_Col].Text);
                }
            }
        }

        /// <summary>
        /// Define what to do when a keyboard key is pressed on an element
        /// </summary>
        /// <param name="Sender">The object sending the event</param>
        /// <param name="Event">The event</param>
        private void OnListViewKeyDown(object Sender, KeyEventArgs Event)
        {
            if (Event.KeyCode == Keys.Enter)
            {
                if (listView1.SelectedItems.Count == 1)
                {
                    ListViewItem Item = listView1.SelectedItems[0];
                    UpdateSubFamily((SubFamily) Item.Tag);
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
                    DbConnect.GetInstance().DeleteSubFamily(((SubFamily) Item.Tag).Reference);
                }

                LoadDatabase();
            }
        }

        /// <summary>
        /// Open a window to edit the given subfamily and update database if there are modifications
        /// </summary>
        /// <param name="SubFamily">The subfamily to update</param>
        private void UpdateSubFamily(SubFamily SubFamily)
        {
            AddSubFamily AddSubFamily = new AddSubFamily(SubFamily);
            if (AddSubFamily.ShowDialog() == DialogResult.OK)
                LoadDatabase();
        }
    }
}