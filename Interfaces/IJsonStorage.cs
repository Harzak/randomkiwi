using randomkiwi.Utilities.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace randomkiwi.Interfaces;

/// <summary>
/// Defines a contract for managing JSON-based storage of data.
/// </summary>
public interface IJsonStorage<T>
{
    string FilePath { get; }
    bool Exists { get; }

    /// <summary>
    /// Asynchronously loads data of type <typeparamref name="T"/> from a JSON file.
    /// </summary>
    Task SaveAsync(T data);

    /// <summary>
    /// Asynchronously saves the specified data to a file in JSON format.
    /// </summary>
    Task<OperationResult<T>> LoadAsync();

    /// <summary>
    /// Deletes the current json storage.
    /// </summary>
    void Delete();
}