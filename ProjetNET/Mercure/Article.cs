using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjetNET
{
    public class Article
    {
        public Article()
        {
        }

        public string Reference
        {
            get
            {
                return Reference;
            }
            set
            {
                Reference = value;
            }
        }
        public string Description
        {
            get
            {
                return Description;
            }
            set
            {
                Description = value;
            }
        }
        public int Brand
        {
            get
            {
                return Brand;
            }
            set
            {
                Brand = value;
            }
        }
        public int Family
        {
            get
            {
                return Family;
            }
            set
            {
                Family = value;
            }
        }
        public int SubFamily
        {
            get
            {
                return SubFamily;
            }
            set
            {
                SubFamily = value;
            }
        }
        public float Price
        {
            get
            {
                return Price;
            }
            set
            {
                Price = value;
            }
        }
    }
}
