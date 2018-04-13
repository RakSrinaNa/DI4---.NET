using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace ProjetNET
{
    public class Family
    {
        public long Reference { get; set; }
        public String Name { get; set; }

        public Family(long Reference, string Name)
        {
            this.Reference = Reference;
            this.Name = Name;
        }
    }
}
