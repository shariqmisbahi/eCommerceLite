using System.ComponentModel.DataAnnotations;

namespace eCommerceLite.Model
{

    public class ProductEmbeddings
    {
        [Key]
        public int productEmbeddingId { get; set; }

        public string productName { get; set; }
        public string productDescription { get; set; }
        public string productDimension { get; set; }
        public decimal productPrice { get; set; }
    }

}
