namespace SalesAI.Application.Common.Interfaces;

public interface IBackgroundJobService
{
    string Enqueue(System.Linq.Expressions.Expression<Action> methodCall);
    string Enqueue<T>(System.Linq.Expressions.Expression<Action<T>> methodCall);
    string Enqueue<T>(System.Linq.Expressions.Expression<Func<T, Task>> methodCall);
}
