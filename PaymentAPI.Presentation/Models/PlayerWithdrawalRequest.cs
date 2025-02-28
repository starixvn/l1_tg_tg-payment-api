namespace PaymentAPI.Presentation.Models;

public class WithdrawalRequest
{
    /// <summary>
    /// The blockchain network chosen for withdrawal
    /// </summary>
    public required string Chain { get; set; }

    /// <summary>
    /// The withdrawal address
    /// </summary>
    public required string Address { get; set; }

    /// <summary>
    /// Player ID
    /// </summary>
    public required string PlayerId { get; set; }
}