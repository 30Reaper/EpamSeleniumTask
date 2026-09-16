using RestSharp;

namespace EpamSeleniumTask.Core.API.Builders;

public class UserRequestBuilder
{
    private readonly RestRequest _request;

    public UserRequestBuilder(string resource, Method method)
    {
        _request = new RestRequest(resource, method);
    }

    public UserRequestBuilder AddHeader(string name, string value)
    {
        _request.AddHeader(name, value);
        return this;
    }

    public UserRequestBuilder AddJsonBody<T>(T body) where T : class
    {
        _request.AddJsonBody(body);
        return this;
    }

    public RestRequest Build()
    {
        return _request;
    }
}