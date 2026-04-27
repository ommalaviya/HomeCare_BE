using Admin.Application.HomeCare.Interfaces;
using Admin.Application.HomeCare.Services;
using Admin.Domain.HomeCare.DataModels.Request.Category;
using Admin.Domain.HomeCare.DataModels.Response.Category;
using Microsoft.AspNetCore.Mvc;
using Shared.Helpers;
using Shared.HomeCare.Resources;
using Microsoft.AspNetCore.Authorization;

namespace Api.HomeCare.Controllers
{
    [Route("api/categories")]
    [ApiController]
    [AllowAnonymous]
    public class CategoryController(ICategoryService categoryService) : ControllerBase
    {
        [HttpGet("get")]
        public async Task<IActionResult> GetCategoryByServiceTypeAsync(int serviceTypeId)
        {
            var result = await categoryService
                .GetCategoryByServiceTypeAsync(serviceTypeId);
            return Ok(ResponseHelper.SuccessResponse(
                result));
        }

        [HttpPost("add")]
        public async Task<ActionResult<GetCategoryResponseModel>> CreateCategoryAsync(
            [FromBody] CreateCategoryRequestModel request)
        {
            if (request == null)
                return BadRequest(Messages.InvalidRequest);

            var result = await categoryService
                .CreateCategoryAsync(request);

            return Ok(ResponseHelper.CreateResponse(
                result,
                string.Format(Messages.CreatedSuccessfully, Messages.Category)));
        }

        [HttpDelete("delete/{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var result = await categoryService.SoftDeleteCategoryAsync(id);

            return Ok(ResponseHelper.SuccessResponse(
                result,
                string.Format(Messages.RemovedSuccessfully, Messages.Category)));
        }
    }
}