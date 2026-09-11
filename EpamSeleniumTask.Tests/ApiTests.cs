using EpamSeleniumTask.Business.Models;
using EpamSeleniumTask.Core.API;
using EpamSeleniumTask.Core.API.Builders;
using Microsoft.Extensions.Logging;
using RestSharp;
using NUnit.Framework;

namespace EpamSeleniumTask.Tests;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class ApiTests : TestBase
{
    private const string BaseUrl = "https://jsonplaceholder.typicode.com";

    private readonly ILogger<ApiTests> _logger;

    public ApiTests()
    {
        _logger = LoggerFactory.CreateLogger<ApiTests>();
    }

    [Category("API")]
    [Test]
    public async Task GetUsers_ShouldReturnUsersSuccessfully()
    {
        _logger.LogInformation("Starting GET /users test");

        var client = new BaseClient(BaseUrl);

        var request = new UserRequestBuilder("/users", Method.Get)
            .AddHeader("Accept", "application/json")
            .Build();

        _logger.LogInformation("Sending GET request to /users");

        var response = await client.ExecuteAsync(request);

        _logger.LogInformation(
            "Received response from /users. StatusCode={StatusCode}",
            (int)response.StatusCode);

        Assert.AreEqual(200, (int)response.StatusCode);
        Assert.IsTrue(response.IsSuccessful);
        Assert.IsFalse(string.IsNullOrEmpty(response.Content));

        _logger.LogInformation("Response status and content validation passed");

        var users = System.Text.Json.JsonSerializer.Deserialize<List<User>>(response.Content!);

        Assert.IsNotNull(users);
        Assert.IsNotEmpty(users);

        _logger.LogInformation("Deserialized {UserCount} users", users!.Count);

        foreach (var user in users)
        {
            Assert.Greater(user.Id, 0);
            Assert.IsFalse(string.IsNullOrWhiteSpace(user.Name));
            Assert.IsFalse(string.IsNullOrWhiteSpace(user.Username));
            Assert.IsFalse(string.IsNullOrWhiteSpace(user.Email));
            Assert.IsNotNull(user.Address);
            Assert.IsFalse(string.IsNullOrWhiteSpace(user.Phone));
            Assert.IsFalse(string.IsNullOrWhiteSpace(user.Website));
            Assert.IsNotNull(user.Company);
        }

        _logger.LogInformation("All user fields validation passed");
    }

    [Category("API")]
    [Test]
    public async Task GetUsers_ShouldReturnJsonContentType()
    {
        _logger.LogInformation("Starting GET /users content type test");

        var client = new BaseClient(BaseUrl);

        var request = new UserRequestBuilder("/users", Method.Get)
            .AddHeader("Accept", "application/json")
            .Build();

        _logger.LogInformation("Sending GET request to /users");

        var response = await client.ExecuteAsync(request);

        _logger.LogInformation(
            "Received response. StatusCode={StatusCode}, ContentType={ContentType}",
            (int)response.StatusCode,
            response.ContentType);

        Assert.AreEqual(200, (int)response.StatusCode);
        Assert.IsTrue(response.IsSuccessful);

        var contentType = response.ContentType;

        Assert.IsNotNull(contentType);
        Assert.AreEqual("application/json", contentType);

        _logger.LogInformation("Content-Type validation passed");
    }

    [Category("API")]
    [Test]
    public async Task GetUsers_ShouldReturnTenUniqueUsers()
    {
        _logger.LogInformation("Starting GET /users users count and uniqueness test");

        var client = new BaseClient(BaseUrl);

        var request = new UserRequestBuilder("/users", Method.Get)
            .AddHeader("Accept", "application/json")
            .Build();

        _logger.LogInformation("Sending GET request to /users");

        var response = await client.ExecuteAsync(request);

        _logger.LogInformation(
            "Received response. StatusCode={StatusCode}",
            (int)response.StatusCode);

        Assert.AreEqual(200, (int)response.StatusCode);
        Assert.IsTrue(response.IsSuccessful);

        var users = System.Text.Json.JsonSerializer.Deserialize<List<User>>(response.Content!);

        Assert.IsNotNull(users);
        Assert.AreEqual(10, users!.Count);

        _logger.LogInformation("Validated that response contains exactly {UserCount} users", users.Count);

        var userIds = users.Select(user => user.Id).ToList();

        Assert.AreEqual(userIds.Count, userIds.Distinct().Count());

        _logger.LogInformation("Validated that all user IDs are unique");

        foreach (var user in users)
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(user.Name));
            Assert.IsFalse(string.IsNullOrWhiteSpace(user.Username));

            Assert.IsNotNull(user.Company);
            Assert.IsFalse(string.IsNullOrWhiteSpace(user.Company.Name));
        }

        _logger.LogInformation("Validated Name, Username and Company.Name for all users");
    }

    [Category("API")]
    [Test]
    public async Task CreateUser_ShouldReturnCreatedUser()
    {
        _logger.LogInformation("Starting POST /users test");

        var client = new BaseClient(BaseUrl);

        var requestBody = new
        {
            name = "Roman Leshchuk",
            username = "roman"
        };

        _logger.LogInformation(
            "Preparing POST request with Name={Name}, Username={Username}",
            requestBody.name,
            requestBody.username);

        var request = new UserRequestBuilder("/users", Method.Post)
            .AddHeader("Accept", "application/json")
            .AddJsonBody(requestBody)
            .Build();

        _logger.LogInformation("Sending POST request to /users");

        var response = await client.ExecuteAsync(request);

        _logger.LogInformation(
            "Received response from POST /users. StatusCode={StatusCode}",
            (int)response.StatusCode);

        Assert.AreEqual(201, (int)response.StatusCode);
        Assert.IsTrue(response.IsSuccessful);
        Assert.IsFalse(string.IsNullOrEmpty(response.Content));

        _logger.LogInformation("Response status and content validation passed");

        var createdUser = System.Text.Json.JsonSerializer
            .Deserialize<User>(response.Content!);

        Assert.IsNotNull(createdUser);
        Assert.Greater(createdUser!.Id, 0);

        _logger.LogInformation(
            "User successfully created with ID={UserId}",
            createdUser.Id);
    }

    [Category("API")]
    [Test]
    public async Task GetInvalidEndpoint_ShouldReturnNotFound()
    {
        _logger.LogInformation("Starting invalid endpoint test");

        var client = new BaseClient(BaseUrl);

        var request = new UserRequestBuilder("/invalidendpoint", Method.Get)
            .AddHeader("Accept", "application/json")
            .Build();

        _logger.LogInformation("Sending GET request to /invalidendpoint");

        var response = await client.ExecuteAsync(request);

        _logger.LogInformation(
            "Received response from invalid endpoint. StatusCode={StatusCode}",
            (int)response.StatusCode);

        Assert.AreEqual(404, (int)response.StatusCode);
        Assert.False(response.IsSuccessful);

        _logger.LogInformation("404 Not Found validation passed");
    }
}