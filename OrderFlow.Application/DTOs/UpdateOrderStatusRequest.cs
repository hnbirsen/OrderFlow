namespace OrderFlow.Application.DTOs
{
    public class UpdateOrderStatusRequest
    {
        public int OrderId { get; set; }
        public int NewStatus { get; set; }
    }
}
