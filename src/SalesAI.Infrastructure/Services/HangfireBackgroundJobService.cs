using Hangfire;
using SalesAI.Application.Common.Interfaces;

namespace SalesAI.Infrastructure.Services;

public class HangfireBackgroundJobService : IBackgroundJobService
{
    private readonly IBackgroundJobClient _backgroundJobClient;

    public HangfireBackgroundJobService(IBackgroundJobClient backgroundJobClient)
    {
        _backgroundJobClient = backgroundJobClient;
    }

    public string Enqueue(System.Linq.Expressions.Expression<Action> methodCall)
    {
        return _backgroundJobClient.Enqueue(methodCall);
    }

    public string Enqueue<T>(System.Linq.Expressions.Expression<Action<T>> methodCall)
    {
        return _backgroundJobClient.Enqueue<T>(methodCall);
    }

    public string Enqueue<T>(System.Linq.Expressions.Expression<Func<T, Task>> methodCall)
    {
        return _backgroundJobClient.Enqueue<T>(methodCall);
    }
}
