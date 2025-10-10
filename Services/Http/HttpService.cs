using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using randomkiwi.Utilities.Results;
using System.Globalization;
using System.Net.Http.Headers;

namespace randomkiwi.Services.Http;

/// <summary>
/// Provides HTTP service functionality with support for GET and POST operations, authentication headers, and response handling.
/// </summary>
public class HttpService : IHttpService, IDisposable
{
    /// <summary>
    /// Gets the name identifier for this HTTP service instance.
    /// </summary>
    public string Name { get; private set; }

    private bool _disposedValue;
    private readonly ILogger<HttpService> _logger;
    private readonly IHttpClientOptionFactory _httpClientOptionFactory;
    private readonly IDateTimeFacade _timeProvider;
    private readonly HttpClient _httpClient;
    private string _user_agent;

    private HttpService(
        HttpClient httpClient,
        IDateTimeFacade timeProvider,
        IHttpClientOptionFactory httpClientOptionFactory,
        ILogger<HttpService> logger)
    {
        _httpClient = httpClient;
        _timeProvider = timeProvider;
        _httpClientOptionFactory = httpClientOptionFactory;
        _logger = logger;
        _user_agent = string.Empty;
        this.Name = string.Empty;
    }

    public HttpService(IHttpClientFactory httpClientFactory,
        IDateTimeFacade timeProvider,
        IHttpClientOptionFactory httpClientOptionFactory,
        ILogger<HttpService> logger)

        : this(httpClientFactory?.CreateClient(HttpClientConsts.HTTPCLIENT_NAME_DEFAULT) ?? throw new ArgumentNullException(nameof(httpClientFactory)),
              timeProvider,
                httpClientOptionFactory,
              logger)
    {

    }

    public void Initialize()
    {
        HttpClientOption option = _httpClientOptionFactory.CreateOption();
        this.Name = option.Name;
        _user_agent = option.UserAgent;
        _httpClient.BaseAddress = option.BaseAddress;

        this.SetHeader();
    }

    #region GET Methods
    /// <summary>
    /// Asynchronously performs an HTTP GET request to the specified endpoint.
    /// </summary>
    public virtual async Task<OperationResult<string>> GetAsync(string endpoint, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(endpoint);

        if (!ToValidUri(endpoint, out Uri? relativeUri) || relativeUri == null)
        {
            return new OperationResult<string>().WithError("Invalid URI");
        }

        return await GetAsync(relativeUri, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously performs an HTTP GET request to the specified URI endpoint.
    /// </summary>
    public virtual async Task<OperationResult<string>> GetAsync(Uri endpoint, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(endpoint);

        HttpResponseMessage? response = await SendGetRequestAsync(endpoint, cancellationToken).ConfigureAwait(false);

        if (response is null)
        {
            return new OperationResult<string>().WithError("Unknown error");
        }
        return await Read(response, cancellationToken).ConfigureAwait(false);
    }

    private async Task<HttpResponseMessage?> SendGetRequestAsync(Uri relativeUri, CancellationToken cancellationToken)
    {
        SetHeader();

        try
        {
            return await _httpClient.GetAsync(relativeUri, cancellationToken).ConfigureAwait(false);
        }
        catch (InvalidOperationException ex)
        {
            HttpClientLogs.InvalidRequest(_logger, relativeUri.ToString(), ex);
        }
        catch (HttpRequestException ex)
        {
            HttpClientLogs.NetworkIssue(_logger, relativeUri.ToString(), ex);
            //retry
        }
        catch (TaskCanceledException ex)
        {
            HttpClientLogs.RequestCanceled(_logger, relativeUri.ToString(), ex);
        }
        catch (Exception)
        {
            throw;
        }
        return null;
    }
    #endregion

    #region POST Methods
    /// <summary>
    /// Asynchronously performs an HTTP POST request to the specified endpoint with content.
    /// </summary>
    public virtual async Task<OperationResult<string>> PostAsync(string endpoint, HttpContent content, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(endpoint);

        if (!ToValidUri(endpoint, out Uri? relativeUri) ||relativeUri == null)
        {
            return new OperationResult<string>().WithError("Invalid URI");
        }

        HttpResponseMessage? response = await SendPostRequestAsync(relativeUri, content, cancellationToken).ConfigureAwait(false);

        if (response is null)
        {
            return new OperationResult<string>().WithError("Unknown error");
        }

        return await Read(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously performs an HTTP POST request to the specified URI endpoint with content.
    /// </summary>
    public virtual async Task<OperationResult<string>> PostAsync(Uri endpoint, HttpContent content, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(endpoint);

        HttpResponseMessage? response = await SendPostRequestAsync(endpoint, content, cancellationToken).ConfigureAwait(false);

        if (response is null)
        {
            return new OperationResult<string>().WithError("Unknown error");
        }

        return await Read(response, cancellationToken).ConfigureAwait(false);
    }

    private async Task<HttpResponseMessage?> SendPostRequestAsync(Uri absoluteUri, HttpContent content, CancellationToken cancellationToken)
    {
        SetHeader();

        try
        {
            return await _httpClient.PostAsync(absoluteUri, content, cancellationToken).ConfigureAwait(false);
        }
        catch (InvalidOperationException ex)
        {
            HttpClientLogs.InvalidRequest(_logger, absoluteUri.AbsoluteUri, ex);
        }
        catch (HttpRequestException ex)
        {
            HttpClientLogs.NetworkIssue(_logger, absoluteUri.AbsoluteUri, ex);
            //retry
        }
        catch (TaskCanceledException ex)
        {
            HttpClientLogs.RequestCanceled(_logger, absoluteUri.AbsoluteUri, ex);
        }
        catch (Exception)
        {
            throw;
        }

        return null;
    }
    #endregion

    #region HTTP Response
    private async Task<OperationResult<string>> Read(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        return new OperationResult<string>()
        {
            IsSuccess = response.IsSuccessStatusCode,
            Content = response.Content != null
                        ? await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false)
                        : string.Empty,
            ErrorMessage = response.StatusCode.ToString(),
            ErrorCode = ((int)response.StatusCode).ToString(CultureInfo.CurrentCulture)
        };
    }
    #endregion

    #region HTTP Request header
    protected virtual void SetHeader()
    {
        SetUserAgent(_user_agent);
        SetDate(_timeProvider.DateTimeUTCNow());
    }

    /// <summary>
    /// Sets the authorization header for HTTP requests with the specified scheme and parameter.
    /// </summary>
    public virtual void SetAuthorization(string scheme, string parameter)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, parameter);
    }

    private void SetUserAgent(string name)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            if (_httpClient.DefaultRequestHeaders.Contains(HeaderNames.UserAgent))
            {
                _httpClient.DefaultRequestHeaders.Remove(HeaderNames.UserAgent);
            }
            _httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, name);
        }
    }

    private void SetDate(DateTime date)
    {
        _httpClient.DefaultRequestHeaders.Date = date;
    }
    #endregion

    private bool ToValidUri(string endpoint, out Uri? relativeUri)
    {
        if (!string.IsNullOrWhiteSpace(endpoint?.Trim()))
        {
            if (Uri.IsWellFormedUriString(endpoint, UriKind.RelativeOrAbsolute))
            {
                return Uri.TryCreate(endpoint, UriKind.RelativeOrAbsolute, out relativeUri);
            }
        }
        relativeUri = null;
        return false;
    }

    ~HttpService() => Dispose(false);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {

            }

            _httpClient?.Dispose();
            _disposedValue = true;
        }
    }
}
