using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PWApplication.MobileAppService.Data;
using PWApplication.MobileAppService.Models;

namespace PWApplication.MobileAppService.Controllers
{
    public abstract class GenericController<TEntity> : ControllerBase where TEntity : BaseEntitySimpleModel
    {
        protected readonly AppDbContext AppDbContext;

        public GenericController(AppDbContext dbContext)
        {
            AppDbContext = dbContext;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public virtual IEnumerable<TEntity> List()
        {
            return AppDbContext.Set<TEntity>().ToList();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<IActionResult> GetEntity([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = await AppDbContext.Set<TEntity>().FindAsync(new Guid(id));

            if (entity == null)
            {
                return NotFound();
            }

            return Ok(entity);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public virtual async Task<IActionResult> Create([FromBody] TEntity entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            AppDbContext.Set<TEntity>().Add(entity);
            await AppDbContext.SaveChangesAsync();

            return CreatedAtAction("GetEntity", new { id = entity.Id }, entity);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public virtual async Task<IActionResult> Edit([FromRoute] string id, [FromBody] TEntity entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Guid idGuid = new Guid(id);
            if (idGuid != entity.Id)
            {
                return BadRequest();
            }

            AppDbContext.Entry(entity).State = EntityState.Modified;

            try
            {
                await AppDbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EntityExists(idGuid))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<IActionResult> Delete([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TEntity entity = await AppDbContext.Set<TEntity>().FindAsync(new Guid(id));

            if (entity == null)
                return NotFound();

            AppDbContext.Set<TEntity>().Remove(entity);
            await AppDbContext.SaveChangesAsync();

            return Ok(entity);
        }

        private bool EntityExists(Guid id)
        {
            return AppDbContext.Set<TEntity>().Any(e => e.Id == id);
        }
    }
}
