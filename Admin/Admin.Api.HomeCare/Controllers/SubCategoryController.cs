using Admin.Application.HomeCare.Interfaces;
using Admin.Domain.HomeCare.DataModels.Request.SubCategory;
using Admin.Domain.HomeCare.DataModels.Response.SubCategory;
using Microsoft.AspNetCore.Mvc;
using Shared.Helpers;
using Shared.HomeCare.Resources;
using Microsoft.AspNetCore.Authorization;

namespace Api.HomeCare.Controllers
{
    [Route("api/subcategories")]
    [ApiController]
    [AllowAnonymous]
    public class SubCategoryController(ISubCategoryService subCategoryService)
        : ControllerBase
    {
        [HttpGet("get")]
        public async Task<IActionResult> GetSubCategoryByCategoryAsync(int categoryId)
        {
            var result = await subCategoryService
                .GetSubCategoryByCategoryAsync(categoryId);

            return Ok(ResponseHelper.SuccessResponse(
               result));
        } 

        [HttpPost("add")]
        public async Task<ActionResult<GetSubCategoryResponseModel>> CreateSubCategoryAsync(
            [FromBody] CreateSubCategoryRequestModel request)
        {
            if (request == null)
                return BadRequest(Messages.InvalidRequest);

            var result = await subCategoryService
                .CreateSubCategoryAsync(request);

            return Ok(ResponseHelper.CreateResponse(
                result,
                string.Format(Messages.CreatedSuccessfully, Messages.SubCategory)));
        }

        [HttpDelete("delete/{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var result = await subCategoryService.SoftDeleteSubCategoryAsync(id);
            return Ok(ResponseHelper.SuccessResponse(
                result,
                string.Format(Messages.RemovedSuccessfully, Messages.SubCategory)));
        }
    }
}

