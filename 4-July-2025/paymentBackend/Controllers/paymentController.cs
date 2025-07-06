using Microsoft.AspNetCore.Mvc;
using Razorpay.Api;


[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IConfiguration _config;

    public PaymentController(IConfiguration config)
    {
        _config = config;
    }

    [HttpPost("create-order")]
    public IActionResult CreateOrder([FromBody] CreateOrderRequest request)
    {
        var key = _config["Razorpay:Key"];
        var secret = _config["Razorpay:Secret"];

        RazorpayClient client = new RazorpayClient(key, secret);

        var options = new Dictionary<string, object>
        {
            { "amount", request.Amount * 100 },
            { "currency", "INR" },
             { "receipt", $"receipt_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}" },
            { "payment_capture", 1 }
        };

        Order order = client.Order.Create(options);

        return Ok(new
        {
            orderId = order["id"].ToString(),
            amount = order["amount"],
            currency = order["currency"]
        });
    }
}

