namespace ECommerceProject.PresentationLayer.ViewModels
{
    public class AdminProductViewModel
    {
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public int CategoryId { get; set; }

        // Ana resim
        public IFormFile MainImage { get; set; }

        // Çoklu resimler
        public List<IFormFile> AdditionalImages { get; set; }

        // Beden, renk ve stok miktarları
        public List<AdminProductSizeStockViewModel> AdminProductSizeStocks { get; set; }
    }
}
