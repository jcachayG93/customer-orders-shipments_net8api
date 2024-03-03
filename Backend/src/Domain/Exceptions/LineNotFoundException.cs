namespace Domain.Exceptions;

public class LineNotFoundException : DomainException
{
    public LineNotFoundException(
        Guid salesOrderId, Guid lineId) : base(
        $"SalesOrder with Id: {salesOrderId} does not have a line with Id: {lineId}")
    {
        
    }
}