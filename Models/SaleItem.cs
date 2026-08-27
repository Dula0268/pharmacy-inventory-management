namespace PharmacyInventory.Models
{
    public class SaleItem
    {
        public long Id { get; set; }

        public long SaleId { get; set; }
        public Sale? Sale { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public int Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}
