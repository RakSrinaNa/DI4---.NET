namespace ProjetNET
{
    /// <summary>
    /// Modeling class for a brand
    /// </summary>
    public class Brand
    {
        public long Reference { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Constructor from all the data
        /// </summary>
        /// <param name="Reference">The reference of the brand</param>
        /// <param name="Name">The name of the brand</param>
        public Brand(long Reference, string Name)
        {
            this.Reference = Reference;
            this.Name = Name;
        }
    }
}
