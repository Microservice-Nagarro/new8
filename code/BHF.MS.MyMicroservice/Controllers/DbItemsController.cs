using BHF.MS.new8.Database.Models.DbItem;
using Microsoft.AspNetCore.Mvc;
using BHF.MS.new8.Database.Services;

namespace BHF.MS.new8.Controllers
{
    /// <summary>
    /// Sample REST API controller.
    /// This should be treated as a sample, how a typical REST controller should look like and behave.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class DbItemsController(IDbItemService dbItemService) : ControllerBase
    {
        /// <summary>
        /// Consider adding paging for such endpoint
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetDbItems()
        {
            return Ok(await dbItemService.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DbItemDto>> GetDbItem(Guid id)
        {
            var dbItem = await dbItemService.GetById(id);

            if (dbItem == null)
            {
                return NotFound();
            }

            return dbItem;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDbItem(Guid id, DbItemDto dbItemDto)
        {
            if (id != dbItemDto.Id)
            {
                return BadRequest();
            }

            var success = await dbItemService.Update(dbItemDto);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<CreatedAtActionResult> PostDbItem(DbItemCreateDto dbItemDto)
        {
            var result = await dbItemService.Add(dbItemDto);

            return CreatedAtAction(nameof(GetDbItem), new { id = result.Id }, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDbItem(Guid id)
        {
            var success = await dbItemService.Delete(id);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}

