using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ProjetNET
{
    /// <summary>
    /// The window to display the list of brands
    /// </summary>
    public partial class Brands : Form
    {
        /// <summary>
        /// Constructor by default
        /// </summary>
        public Brands()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterParent;

            listView1.FullRowSelect = true;
            listView1.MouseClick += new MouseEventHandler(OnClickBrand);

            LoadDatabase();
        }

        /// <summary>
        /// Define what to do when a click occurs on a element in the list
        /// </summary>
        /// <param name="sender">The object sending the event</param>
        /// <param name="e">The event</param>
        private void OnClickBrand(object sender, MouseEventArgs e)
        {
            if (e.Clicks == 2)
            {
                if (listView1.SelectedItems.Count == 1)
                {
                    ListViewItem Item = listView1.SelectedItems[0];
                    UpdateBrand((Brand) Item.Tag);
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                ContextMenu ContextMenu = new ContextMenu();

                if (listView1.SelectedItems.Count == 1)
                {
                    ListViewItem Item = listView1.SelectedItems[0];
                    MenuItem MenuAdd = new MenuItem("Add brand");
                    MenuAdd.Click += new EventHandler((o, evt) =>
                    {
                        UpdateBrand(null);
                    });
                    MenuItem MenuMod = new MenuItem("Edit brand");
                    MenuMod.Click += new EventHandler((o, evt) =>
                    {
                        UpdateBrand((Brand) Item.Tag);
                    });
                    MenuItem MenuDel = new MenuItem("Delete brand");
                    MenuDel.Click += new EventHandler((o, evt) =>
                    {
                        DBConnect.GetInstance().DeleteBrand(((Brand) Item.Tag).Reference);
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
        /// Reload all the items in the view from the database
        /// </summary>
        private void LoadDatabase()
        {
            listView1.Columns.Clear();
            listView1.Items.Clear();
            SQLiteDataAdapter Adapter = new SQLiteDataAdapter("SELECT RefMarque, Nom FROM Marques",
                DBConnect.GetInstance().GetConnection());
            DataTable Dt = new DataTable();
            Adapter.Fill(Dt);

            listView1.KeyDown += new KeyEventHandler(OnListViewKeyDown);
            listView1.Sorting = System.Windows.Forms.SortOrder.Ascending;
            listView1.ColumnClick += new ColumnClickEventHandler(OnColumnClick);
            listView1.ListViewItemSorter = new ListViewItemComparer();

            listView1.Columns.Add("RefMarque");
            listView1.Columns.Add("Nom");

            for (int i = 0; i < Dt.Rows.Count; i++)
            {
                DataRow Dr = Dt.Rows[i];
                ListViewItem ListItem = new ListViewItem(Dr["RefMarque"].ToString());
                ListItem.SubItems.Add(Dr["Nom"].ToString());
                ListItem.Tag = new Brand(
                    Convert.ToInt64(Dr["RefMarque"].ToString()),
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
        /// <param name="sender">The object sending the event</param>
        /// <param name="e">The object</param>
        private void OnColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListViewItemComparer LastComparer = (ListViewItemComparer) listView1.ListViewItemSorter;
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

        /// <summary>
        /// Class to sort a list depending of a given column
        /// </summary>
        private class ListViewItemComparer : IComparer
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
                    case 0:
                        return (Inverted ? -1 : 1) *
                               (int) (long.Parse(((ListViewItem) x).SubItems[Col].Text,
                                          System.Globalization.CultureInfo.CurrentUICulture) -
                                      long.Parse(((ListViewItem) y).SubItems[Col].Text,
                                          System.Globalization.CultureInfo.CurrentUICulture));
                    default:
                        return (Inverted ? -1 : 1) * String.Compare(((ListViewItem) x).SubItems[Col].Text,
                                   ((ListViewItem) y).SubItems[Col].Text);
                }
            }
        }

        /// <summary>
        /// Define what to do when a keyboard key is pressed on an element
        /// </summary>
        /// <param name="sender">The object sending the event</param>
        /// <param name="e">The event</param>
        private void OnListViewKeyDown(object sender, KeyEventArgs e)
        {
            if ((Keys) e.KeyCode == Keys.Enter)
            {
                if (listView1.SelectedItems.Count == 1)
                {
                    ListViewItem Item = listView1.SelectedItems[0];
                    UpdateBrand((Brand) Item.Tag);
                }
            }
            else if ((Keys) e.KeyCode == Keys.F5)
            {
                LoadDatabase();
            }
            else if ((Keys) e.KeyCode == Keys.Delete)
            {
                for (int i = 0; i < listView1.SelectedItems.Count; i++)
                {
                    ListViewItem Item = listView1.SelectedItems[i];
                    DBConnect.GetInstance().DeleteBrand(((Brand) Item.Tag).Reference);
                }

                LoadDatabase();
            }
        }

        /// <summary>
        /// Open a window to edit the given brand and update database if there are modifications
        /// </summary>
        /// <param name="Family">The family to update</param>
        private void UpdateBrand(Brand Brand)
        {
            AddBrand AddBrand = new AddBrand(Brand);
            if (AddBrand.ShowDialog() == DialogResult.OK)
                LoadDatabase();
        }
    }
}