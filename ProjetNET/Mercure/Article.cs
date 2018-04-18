using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjetNET
{
    /// <summary>
    /// Modeling class for an article
    /// </summary>
    public class Article
    {
        /// <summary>
        /// Constructor from all the data
        /// </summary>
        /// <param name="RefArticle">The reference of the article</param>
        /// <param name="Description">The description of the article</param>
        /// <param name="RefSubFamily">The reference of the subfamily of the article</param>
        /// <param name="RefBrand">The reference of the brand of the article</param>
        /// <param name="Price">The price of the article</param>
        /// <param name="Quantity">The quantity of the article</param>
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
