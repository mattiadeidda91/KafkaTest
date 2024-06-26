namespace Common.Models
{
    public abstract class BaseEvent<T> where T : class
    {
        public string? ActivityId { get; set; }
        public T? Activity { get; set; }
    }
}
