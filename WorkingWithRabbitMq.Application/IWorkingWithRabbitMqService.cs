using WorkingWithRabbitMq.Application.Model;

namespace WorkingWithRabbitMq.Application
{
    public interface IWorkingWithRabbitMqService
    {
        bool SendMessage(RabbitMqTask task, CancellationToken cancellationToken);
        Task<RabbitMqTask> GetMessage(CancellationToken cancellationToken);
    }
}
