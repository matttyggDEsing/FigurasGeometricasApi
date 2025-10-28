using FigurasGeometricasApi.DTOs;
using FigurasGeometricasApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FigurasGeometricasApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FigurasController : ControllerBase
    {
        private readonly IFiguraService _service;
        private readonly ILogger<FigurasController> _logger;

        public FigurasController(IFiguraService service, ILogger<FigurasController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FiguraReadDto>>> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id:int}", Name = "GetFiguraById")]
        public async Task<ActionResult<FiguraReadDto>> GetById(int id)
        {
            var f = await _service.GetByIdAsync(id);
            if (f == null) return NotFound();
            return Ok(f);
        }

        [HttpPost]
        public async Task<ActionResult<FiguraReadDto>> Create([FromBody] FiguraCreateDto dto)
        {
            try
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtRoute("GetFiguraById", new { id = created.Id }, created);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Creación inválida");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] FiguraUpdateDto dto)
        {
            try
            {
                var ok = await _service.UpdateAsync(id, dto);
                if (!ok) return NotFound();
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpGet("totales")]
        public async Task<ActionResult> Totales()
        {
            var (area, perim) = await _service.TotalesAsync();
            return Ok(new { area, perimetro = perim });
        }
    }
}
