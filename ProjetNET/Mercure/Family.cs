namespace ProjetNET
{
    /// <summary>
    /// Modeling class for a family
    /// </summary>
    public class Family
    {
        public long Reference { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Constructor from all the data
        /// </summary>
        /// <param name="Reference">The reference of the family</param>
        /// <param name="Name">The name of the family</param>
        public Family(long Reference, string Name)
        {
            this.Reference = Reference;
            this.Name = Name;
        }
    }
}
