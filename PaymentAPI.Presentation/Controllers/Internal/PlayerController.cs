using Microsoft.AspNetCore.Mvc;
using PaymentAPI.Presentation.Models;

namespace PaymentAPI.Presentation.Controllers.Internal;

[ApiController]
[Route("[controller]")]
public class PlayerController : ControllerBase
{
    private readonly ILogger<PlayerController> _logger;

    public PlayerController(ILogger<PlayerController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get player's deposit addresses
    /// </summary>
    /// <param name="playerId">Player ID</param>
    /// <returns>List of deposit addresses</returns>
    [HttpGet("deposit-addresses/{playerId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GetPlayerDepositAddressesResponse>> GetDepositAddresses(string playerId)
    {
        try
        {
            // TODO: Implement business logic to get deposit addresses
            // Query all payment provider information: Internal DB
            // Get deposit addresses for player: External API
            return Ok(new GetPlayerDepositAddressesResponse { Data = new List<PlayerDepositAddress>() });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting deposit addresses for player {PlayerId}", playerId);
            return BadRequest();
        }
    }

    /// <summary>
    /// Request withdrawal
    /// </summary>
    /// <param name="request">Withdrawal request details</param>
    /// <returns>Success or failure response</returns>
    [HttpPost("request-withdrawl")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RequestWithdrawal([FromBody] WithdrawalRequest request)
    {
        try
        {
            // TODO: Implement business logic for withdrawal request
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing withdrawal request for player {PlayerId}", request.PlayerId);
            return BadRequest();
        }
    }
}

