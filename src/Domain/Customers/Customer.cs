namespace Domain.Customers;

/// <summary>
/// A customer
/// </summary>
public class Customer
{
    /*
     * See, Customer has Orders, so one can assume a customer is an aggregate. That makes sense but,
     * when the number of child entities (a customer may have many orders), I prefer to keep them separate
     * for performance (an aggregate should be loaded complete each time)
     * So, this is a compromise.
     */
}