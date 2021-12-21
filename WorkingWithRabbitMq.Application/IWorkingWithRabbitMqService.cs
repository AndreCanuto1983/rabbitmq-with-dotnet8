using WorkingWithRabbitMq.Application.Model;

namespace WorkingWithRabbitMq.Application
{
    public interface IWorkingWithRabbitMqService
    {
        void SendMessage(RabbitMqTask task);
        RabbitMqTask GetMessage();
    }
}
