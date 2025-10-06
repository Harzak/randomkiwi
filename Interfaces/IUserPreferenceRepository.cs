using randomkiwi.Utilities.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace randomkiwi.Interfaces;

public interface IUserPreferenceRepository
{
    Task<OperationResult<UserPreferenceModel>> LoadAsync();
    Task SaveAsync(UserPreferenceModel userPreference);
}