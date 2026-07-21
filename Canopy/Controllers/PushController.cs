using Canopy.Models;
using Canopy.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Canopy.Controllers
{
    [Authorize]
    [Route("api/push")]
    [ApiController]
    public class PushController : ControllerBase
    {
        private readonly IPushSubscriptionRepository _repo;
        private readonly IConfiguration _config;

        public PushController(IPushSubscriptionRepository repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }

        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(claim?.Value ?? throw new UnauthorizedAccessException());
        }

        [HttpGet("public-key")]
        public IActionResult GetPublicKey() =>
            Ok(new { publicKey = _config["VapidKeys:PublicKey"] });

        [HttpPost("subscribe")]
        public IActionResult Subscribe([FromBody] PushSubscribeRequest req)
        {
            _repo.Save(new PushSubscription
            {
                UserId = GetUserId(),
                Endpoint = req.Endpoint,
                P256dh = req.Keys.P256dh,
                Auth = req.Keys.Auth
            });
            return Ok();
        }

        [HttpPost("unsubscribe")]
        public IActionResult Unsubscribe([FromBody] PushUnsubscribeRequest req)
        {
            _repo.DeleteByEndpoint(req.Endpoint);
            return Ok();
        }
    }

    public record PushSubscribeRequest(string Endpoint, PushKeys Keys);
    public record PushKeys(string P256dh, string Auth);
    public record PushUnsubscribeRequest(string Endpoint);
}
