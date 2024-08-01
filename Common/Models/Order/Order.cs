namespace Common.Models.Order
{
    public class Order : BaseDto
    {
        public string? Description { get; set; }

        public double Price { get; set; }

    }
}
