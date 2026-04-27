using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Public.Application.HomeCare.Interfaces;
using Public.Domain.HomeCare.DataModels.Request.SupportTicket;
using Shared.Helpers;
using Shared.HomeCare.Resources;

namespace Public.Api.HomeCare.Controllers
{
    [Route("api/support-tickets")]
    [ApiController]
    [AllowAnonymous]
    public class SupportTicketController(ISupportTicketService supportTicketService) : ControllerBase
    {
        [HttpPost("submit")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateSupportTicketRequestModel request)
        {
            var result = await supportTicketService.CreateSupportTicketAsync(request);
            return Ok(ResponseHelper.CreateResponse(
                result,
                string.Format(Messages.CreatedSuccessfully, Messages.SupportTicket)));
        }
    }
}