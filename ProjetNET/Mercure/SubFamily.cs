using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjetNET
{
    public class SubFamily
    {
        public long Reference { get; set; }
        public long FamilyReference { get; set; }
        public String Name { get; set; }

        public SubFamily(long Reference, long FamilyReference, string Name)
        {
            this.Reference = Reference;
            this.FamilyReference = FamilyReference;
            this.Name = Name;
        }
    }
}
