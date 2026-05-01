namespace DevEval.Application.Sales.Dtos
{
    public class SaleItemDto
    {
        public required int ProductId { get; set; }
        public required int Quantity { get; set; }
        public required decimal UnitPrice { get; set; }
        public required decimal Discount { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
