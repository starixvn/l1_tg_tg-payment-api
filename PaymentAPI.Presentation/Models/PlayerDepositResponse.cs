using System.Text.Json.Serialization;
namespace PaymentAPI.Presentation.Models;

public class PlayerDepositAddress
{
    public required string Chain { get; set; }
    public required string Address { get; set; }
}

public class GetPlayerDepositAddressesResponse
{
    [JsonPropertyName("data")]
    public required List<PlayerDepositAddress> Data { get; set; }
}