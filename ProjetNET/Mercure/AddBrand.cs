using System;
using System.Windows.Forms;

namespace ProjetNET
{
    /// <inheritdoc />
    /// <summary>
    /// Window to enter the data of a brand
    /// </summary>
    public partial class AddBrand : Form
    {
        /// <inheritdoc />
        /// <summary>
        ///  Initialize an empty window to add a brand
        /// </summary>
        public AddBrand()
        {
            InitializeComponent();
            Construct(null);
        }

        /// <inheritdoc />
        /// <summary>
        /// Initialize a window to modify the given brand
        /// </summary>
        /// <param name="Brand">The brand to edit. If the brand is null, no informations will be filled</param>
        public AddBrand(Brand Brand)
        {
            InitializeComponent();
            Construct(Brand);
        }

        /// <summary>
        /// Put the attributes of the given brand in the corresponding fields
        /// </summary>
        /// <param name="Brand">The brand (or null for empty window)</param>
        private void Construct(Brand Brand)
        {
            StartPosition = FormStartPosition.CenterParent;
            DialogResult = DialogResult.Cancel;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;

            if (Brand != null)
                SetBrand(Brand);
            else
                _Id = -1;
        }

        /// <summary>
        /// Return the brand created (or edited) by the window
        /// </summary>
        /// <returns>The new brand</returns>
        public Brand GetBrand()
        {
            Brand Brand = new Brand(_Id, TextBoxName.Text);
            return Brand;
        }

        /// <summary>
        /// Edit the given brand with the new data
        /// </summary>
        /// <param name="Brand">The brand to edit</param>
        public void SetBrand(Brand Brand)
        {
            TextBoxName.Text = Brand.Name;
            _Id = Brand.Reference;
        }

        /// <summary>
        /// Validate the creation (or editing)
        /// </summary>
        /// <param name="Sender">The object sending the event</param>
        /// <param name="Event">The event</param>
        private void ButtonOK_Click(object Sender, EventArgs Event)
        {
            DialogResult = DialogResult.OK;
            Brand Brand = GetBrand();
            if (Brand != null)
                DbConnect.GetInstance().UpdateOrCreateBrand(Brand);
            Close();
        }

        /// <summary>
        /// Close the window without saving the changes
        /// </summary>
        /// <param name="Sender">The object sending the event</param>
        /// <param name="Event">The event</param>
        private void ButtonCancel_Click(object Sender, EventArgs Event)
        {
            Close();
        }

        private long _Id;
    }
}
