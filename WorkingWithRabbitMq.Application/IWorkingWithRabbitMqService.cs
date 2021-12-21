using WorkingWithRabbitMq.Application.Model;

namespace WorkingWithRabbitMq.Application
{
    public interface IWorkingWithRabbitMqService
    {
        bool SendMessage(RabbitMqTask task);
        RabbitMqTask GetMessage();
    }
}
