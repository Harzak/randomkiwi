using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace randomkiwi.Interfaces;

public interface IDebounceAction
{
    Task<T> ExecuteAsync<T>(Func<Task<T>> action);
    T Execute<T>(Func<T> action);
}