namespace GAC_WMS.IntegrationSolution.DTO
{
    public class SalesOrder
    {

        public class SalesOrderDto
        {
            public string OrderId { get; set; }
            public DateTime ProcessingDate { get; set; }
            public string CustomerIdentifier { get; set; }
            public string ShipmentAddress { get; set; }
            public List<SalesOrderItemDto> Items { get; set; }
        }

        public class SalesOrderItemDto
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }


    }
}
