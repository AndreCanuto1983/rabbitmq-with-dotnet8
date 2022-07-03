using WorkingWithRabbitMq.Application.Model;

namespace WorkingWithRabbitMq.Infra.Interface
{
    public interface IWorkingWithRabbitMqService
    {
        void SendMessage(RabbitMqTask task);
        RabbitMqTask GetMessage();
    }
}
