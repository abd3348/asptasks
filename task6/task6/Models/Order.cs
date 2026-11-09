namespace task6.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int CustomerName { get; set; }
        public int OrderDate { get; set; }
        public int TotalPrice { get; set; }
        public int Status { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
