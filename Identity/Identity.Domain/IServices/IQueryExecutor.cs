namespace Identity.Domain.IServices;

public interface IQueryExecutor
{
    Task<IEnumerable<TResponse>> QueryAsync<TResponse>(string rawQuery) where TResponse : class;
}