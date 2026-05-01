using DevEval.Application.Sales.Commands;
using DevEval.Application.Sales.Dtos;
using DevEval.Domain.Entities.Sale;

namespace DevEval.Application.Common.Mappings;

internal static class SaleMappings
{
    public static Sale ToEntity(this CreateSaleCommand command)
    {
        var sale = new Sale(
            command.SaleNumber,
            command.CustomerId,
            command.CustomerName,
            command.BranchId,
            command.BranchName);

        sale.UpdateSaleDate(command.SaleDate == default ? DateTime.UtcNow : command.SaleDate);
        sale.Items = command.Items.Select(item => item.ToEntity()).ToList();

        return sale;
    }

    public static void ApplyTo(this UpdateSaleCommand command, Sale sale)
    {
        sale.UpdateSaleNumber(command.SaleNumber);
        sale.UpdateSaleDate(command.SaleDate);
        sale.UpdateCustomer(command.CustomerId, command.CustomerName);
        sale.UpdateBranch(command.BranchId, command.BranchName);
        sale.Items = command.Items.Select(item => item.ToEntity()).ToList();
    }

    public static SaleDto ToDto(this Sale sale)
    {
        return new SaleDto
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            SaleDate = sale.SaleDate,
            TotalAmount = sale.TotalAmount,
            IsCancelled = sale.IsCancelled,
            Items = sale.Items.Select(item => item.ToDto()).ToList()
        };
    }

    public static SaleItem ToEntity(this SaleItemDto dto)
    {
        return new SaleItem(dto.ProductId, dto.Quantity, dto.UnitPrice, dto.Discount);
    }

    public static SaleItemDto ToDto(this SaleItem item)
    {
        return new SaleItemDto
        {
            ProductId = item.ProductId,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            Discount = item.Discount,
            TotalPrice = item.TotalPrice
        };
    }
}
