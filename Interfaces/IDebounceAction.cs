namespace randomkiwi.Interfaces;

public interface IDebounceAction
{
    Task<T> ExecuteAsync<T>(Func<Task<T>> action);
    T Execute<T>(Func<T> action);
}