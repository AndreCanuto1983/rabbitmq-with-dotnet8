using WorkingWithRabbitMq.Application.Model;

namespace WorkingWithRabbitMq.Application
{
    public interface IWorkingWithRabbitMqService
    {
        bool SendMessage(RabbitMqTask task, CancellationToken cancellationToken);
        RabbitMqTask GetMessage(CancellationToken cancellationToken);
    }
}
