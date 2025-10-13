namespace randomkiwi.Interfaces;

public interface IUserPreferenceRepository
{
    Task<OperationResult<UserPreferenceModel>> LoadAsync();
    Task SaveAsync(UserPreferenceModel userPreference);
}