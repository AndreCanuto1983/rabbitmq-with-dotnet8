namespace WorkingWithRabbitMq.Application.Model
{
    public class RabbitMqTask
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }

        public bool IsValid()
        {
            return Id > 0 && string.IsNullOrEmpty(Name) && string.IsNullOrEmpty(Phone);
        }
    }
}
