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
    public partial class Families : Form
    {
        public Families()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterParent;

            listView1.FullRowSelect = true;
            listView1.MouseClick += new MouseEventHandler(OnClickFamily);

            LoadDatabase();
        }

        private void OnClickFamily(object sender, MouseEventArgs e)
        {
            if (e.Clicks == 2)
            {
                if (listView1.SelectedItems.Count == 1)
                {
                    ListViewItem Item = listView1.SelectedItems[0];
                    UpdateFamily((Family) Item.Tag);
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                ContextMenu ContextMenu = new ContextMenu();

                if (listView1.SelectedItems.Count == 1)
                {
                    ListViewItem Item = listView1.SelectedItems[0];
                    MenuItem MenuAdd = new MenuItem("Add family");
                    MenuAdd.Click += new EventHandler((o, evt) =>
                    {
                        UpdateFamily(null);
                    });
                    MenuItem MenuMod = new MenuItem("Edit family");
                    MenuMod.Click += new EventHandler((o, evt) =>
                    {
                        UpdateFamily((Family) Item.Tag);
                    });
                    MenuItem MenuDel = new MenuItem("Delete family");
                    MenuDel.Click += new EventHandler((o, evt) =>
                    {
                        DBConnect.GetInstance().DeleteFamily(((Family) Item.Tag).Reference);
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
            SQLiteDataAdapter Adapter = new SQLiteDataAdapter("SELECT RefFamille, Nom FROM Familles",
                DBConnect.GetInstance().GetConnection());
            DataTable Dt = new DataTable();
            Adapter.Fill(Dt);

            listView1.KeyDown += new KeyEventHandler(OnListViewKeyDown);
            listView1.Sorting = System.Windows.Forms.SortOrder.Ascending;
            listView1.ColumnClick += new ColumnClickEventHandler(OnColumnClick);
            listView1.ListViewItemSorter = new ListViewItemComparer();

            listView1.Columns.Add("RefFamille");
            listView1.Columns.Add("Nom");

            for (int i = 0; i < Dt.Rows.Count; i++)
            {
                DataRow Dr = Dt.Rows[i];
                ListViewItem ListItem = new ListViewItem(Dr["RefFamille"].ToString());
                ListItem.SubItems.Add(Dr["Nom"].ToString());
                ListItem.Tag = new Family(
                    Convert.ToInt64(Dr["RefFamille"].ToString()),
                    Dr["Nom"].ToString()
                );
                listView1.Items.Add(ListItem);
            }

            Adapter.Dispose();
        }

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

        private class ListViewItemComparer : IComparer
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

        private void OnListViewKeyDown(object sender, KeyEventArgs e)
        {
            if ((Keys) e.KeyCode == Keys.Enter)
            {
                if (listView1.SelectedItems.Count == 1)
                {
                    ListViewItem Item = listView1.SelectedItems[0];
                    UpdateFamily((Family) Item.Tag);
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
                    DBConnect.GetInstance().DeleteFamily(((Family) Item.Tag).Reference);
                }

                LoadDatabase();
            }
        }

        private void UpdateFamily(Family Family)
        {
            AddFamily AddFamily = new AddFamily(Family);
            AddFamily.ShowDialog();
            LoadDatabase();
        }
    }
}
