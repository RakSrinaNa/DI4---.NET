using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjetNET
{
    public class Article
    {
        public Article(string RefArticle, string Description, long RefSubFamily, long RefBrand, double Price, long Quantity)
        {
            this.Reference = RefArticle;
            this.Description = Description;
            this.Brand = RefBrand;
            this.SubFamily = RefSubFamily;
            this.Price = Price;
            this.Quantity = Quantity;
        }

        public string Reference { get; set; }
        public string Description { get; set; }
        public long Brand { get; set; }
        public long Quantity { get;set; }
        public long SubFamily { get; set; }
        public double Price { get; set; }
    }
}
