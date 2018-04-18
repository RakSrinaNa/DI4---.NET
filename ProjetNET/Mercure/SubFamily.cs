using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjetNET
{
    /// <summary>
    /// Modeling class for a subfamily
    /// </summary>
    public class SubFamily
    {
        public long Reference { get; set; }
        public long FamilyReference { get; set; }
        public String Name { get; set; }

        /// <summary>
        /// Constructor from all the data
        /// </summary>
        /// <param name="Reference">The reference of the subfamily</param>
        /// <param name="FamilyReference">The reference of the family of the subfamily</param>
        /// <param name="Name">The name of the subfamily</param>
        public SubFamily(long Reference, long FamilyReference, string Name)
        {
            this.Reference = Reference;
            this.FamilyReference = FamilyReference;
            this.Name = Name;
        }
    }
}
