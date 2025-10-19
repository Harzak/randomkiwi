namespace randomkiwi.Repositories;

public sealed class UserPreferenceRepository : IUserPreferenceRepository
{
    private readonly IJsonStorage<UserPreferenceModel> _jsonStorage;

    public UserPreferenceRepository(IJsonStorage<UserPreferenceModel> jsonStorage)
    {
        _jsonStorage = jsonStorage ?? throw new ArgumentNullException(nameof(jsonStorage));
    }

    public async Task<OperationResult<UserPreferenceModel>> LoadAsync()
    {
        return await _jsonStorage.LoadAsync().ConfigureAwait(false);
    }

    public async Task SaveAsync(UserPreferenceModel userPreference)
    {
        ArgumentNullException.ThrowIfNull(userPreference, nameof(userPreference));

        await _jsonStorage.SaveAsync(userPreference).ConfigureAwait(false);
    }
}