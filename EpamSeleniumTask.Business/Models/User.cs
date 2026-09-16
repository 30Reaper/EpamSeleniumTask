using System.Text.Json.Serialization;

namespace EpamSeleniumTask.Business.Models;

public class User
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("address")]
    public Address Address { get; set; } = new();

    [JsonPropertyName("phone")]
    public string Phone { get; set; } = string.Empty;

    [JsonPropertyName("website")]
    public string Website { get; set; } = string.Empty;

    [JsonPropertyName("company")]
    public Company Company { get; set; } = new();
}

public class Address
{
    [JsonPropertyName("street")]
    public string Street { get; set; } = string.Empty;

    [JsonPropertyName("suite")]
    public string Suite { get; set; } = string.Empty;

    [JsonPropertyName("city")]
    public string City { get; set; } = string.Empty;

    [JsonPropertyName("zipcode")]
    public string Zipcode { get; set; } = string.Empty;
}

public class Company
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("catchPhrase")]
    public string CatchPhrase { get; set; } = string.Empty;

    [JsonPropertyName("bs")]
    public string Bs { get; set; } = string.Empty;
}