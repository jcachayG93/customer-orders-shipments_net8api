using FluentValidation;

namespace WebApi.Features.Orders.OrdersAddRemoveLines;

public class OrdersAddRemoveLinesCommandValidator
: AbstractValidator<OrdersAddRemoveLinesCommand>
{
    public OrdersAddRemoveLinesCommandValidator()
    {
        RuleForEach(e => e.Lines)
            .Must(e => e.Quantity > 0)
            .WithMessage("The SalesOrder Line Quantity must be greater than zero.")
            .Must(e => e.UnitPrice >= 0M)
            .WithMessage("The SalesOrder Line Unit Price must be positive.");

        RuleFor(e => e.Lines)
            .Must(AllLineProductNamesAreUnique)
            .WithMessage("Each SalesOrder line must have a unique product name.");
    }

    private bool AllLineProductNamesAreUnique(ICollection<OrdersAddRemoveLinesCommand.OrderLineDto> lines)
    {
        var groupByName = lines.GroupBy(l =>
            l.Product.Trim().ToUpper());

        return groupByName.Count() == lines.Count();
    }
}