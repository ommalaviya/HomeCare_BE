using Admin.Application.HomeCare.Interfaces;
using Admin.Domain.HomeCare.DataModels.Request.SupportTicket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Helpers;

namespace Admin.Api.HomeCare.Controllers
{
    [Route("api/support-tickets")]
    [ApiController]
    [Authorize]
    public class SupportTicketController(ISupportTicketService supportTicketService) : ControllerBase
    {
        [HttpGet("list")]
        public async Task<IActionResult> GetAllAsync([FromQuery] FilterSupportTicketRequestModel filter)
        {
            var result = await supportTicketService.GetAllSupportTicketsAsync(filter);
            return Ok(ResponseHelper.SuccessResponse(result));
        }
    }
}