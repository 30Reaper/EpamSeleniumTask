using RestSharp;

namespace EpamSeleniumTask.Core.API;

public class BaseClient
{
    private readonly RestClient _client;

    public BaseClient(string baseUrl)
    {
        _client = new RestClient(baseUrl);
    }

    public async Task<RestResponse> ExecuteAsync(RestRequest request)
    {
        return await _client.ExecuteAsync(request);
    }
}