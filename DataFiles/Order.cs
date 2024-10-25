namespace DeliveryServiceTest.DataFiles
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderId { get; set; }
        public double Weight { get; set; }
        public string District { get; set; }
        public DateTime DeliveryTime { get; set; }
    }
}
