namespace OrderFlow.Domain.Enums
{
    public enum OrderStatusEnum
    {
        New = 0,
        Processing = 1,
        Sent = 2,
        Completed = 3,
        Cancelled = 4,
        Refunded = 5
    }
}
