using randomkiwi.Services.Http;

namespace randomkiwi.Interfaces;

public interface IHttpClientOptionFactory
{
    HttpClientOption CreateOption();
}
