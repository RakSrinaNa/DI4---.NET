using System;
using System.Collections;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;


namespace ProjetNET
{
    /// <inheritdoc />
    /// <summary>
    /// The window to display the list of families
    /// </summary>
    public partial class Families : Form
    {
        /// <inheritdoc />
        /// <summary>
        /// Constructor by default
        /// </summary>
        public Families()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterParent;

            listView1.FullRowSelect = true;
            listView1.MouseClick += OnClickFamily;

            LoadDatabase();
        }

        /// <summary>
        /// Define what to do when a click occurs on a element in the list
        /// </summary>
        /// <param name="Sender">The object sending the event</param>
        /// <param name="Event">The mouse event</param>
        private void OnClickFamily(object Sender, MouseEventArgs Event)
        {
            if (Event.Clicks == 2)
            {
                if (listView1.SelectedItems.Count == 1)
                {
                    ListViewItem Item = listView1.SelectedItems[0];
                    UpdateFamily((Family) Item.Tag);
                }
            }
            else if (Event.Button == MouseButtons.Right)
            {
                ContextMenu ContextMenu1 = new ContextMenu();

                if (listView1.SelectedItems.Count == 1)
                {
                    ListViewItem Item = listView1.SelectedItems[0];
                    MenuItem MenuAdd = new MenuItem("Add family");
                    MenuAdd.Click += (Sender2, Event2) =>
                    {
                        UpdateFamily(null);
                    };
                    MenuItem MenuMod = new MenuItem("Edit family");
                    MenuMod.Click += (Sender2, Event2) =>
                    {
                        UpdateFamily((Family) Item.Tag);
                    };
                    MenuItem MenuDel = new MenuItem("Delete family");
                    MenuDel.Click += (Sender2, Event2) =>
                    {
                        DbConnect.GetInstance().DeleteFamily(((Family) Item.Tag).Reference);
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
        /// Reload all the items in the view from the database
        /// </summary>
        private void LoadDatabase()
        {
            listView1.Columns.Clear();
            listView1.Items.Clear();
            SQLiteDataAdapter Adapter = new SQLiteDataAdapter("SELECT RefFamille, Nom FROM Familles",
                DbConnect.GetInstance().GetConnection());
            DataTable Dt = new DataTable();
            Adapter.Fill(Dt);

            listView1.KeyDown += OnListViewKeyDown;
            listView1.Sorting = SortOrder.Ascending;
            listView1.ColumnClick += OnColumnClick;
            listView1.ListViewItemSorter = new ListViewItemComparer();

            listView1.Columns.Add("RefFamille");
            listView1.Columns.Add("Nom");

            for (int I = 0; I < Dt.Rows.Count; I++)
            {
                DataRow Dr = Dt.Rows[I];
                ListViewItem ListItem = new ListViewItem(Dr["RefFamille"].ToString());
                ListItem.SubItems.Add(Dr["Nom"].ToString());
                ListItem.Tag = new Family(
                    Convert.ToInt64(Dr["RefFamille"].ToString()),
                    Dr["Nom"].ToString()
                );
                listView1.Items.Add(ListItem);
            }

            listView1.Columns[0].Width = -2;
            listView1.Columns[1].Width = -2;

            Adapter.Dispose();
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
                    UpdateFamily((Family) Item.Tag);
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
                    DbConnect.GetInstance().DeleteFamily(((Family) Item.Tag).Reference);
                }

                LoadDatabase();
            }
        }

        /// <summary>
        /// Open a window to edit the given family and update database if there are modifications
        /// </summary>
        /// <param name="Family">The family to update</param>
        private void UpdateFamily(Family Family)
        {
            AddFamily AddFamily = new AddFamily(Family);
            if (AddFamily.ShowDialog() == DialogResult.OK)
                LoadDatabase();
        }
    }
}
