namespace WorkingWithRabbitMq.Application.Model
{
    public class RabbitMqTask
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }

        public bool IsValid()
        {
            return Id > 0 && string.IsNullOrEmpty(Description) && Priority > 0;
        }
    }
}
