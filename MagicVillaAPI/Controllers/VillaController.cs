using AutoMapper;
using MagicVillaAPI.Datos;
using MagicVillaAPI.Models;
using MagicVillaAPI.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVillaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<VillaController> _logger;
        private readonly IMapper _mapper;
       

        public VillaController(ILogger<VillaController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Villa, VillaDto>().ReverseMap();
                cfg.CreateMap<Villa, VillaCreateDto>().ReverseMap();
                cfg.CreateMap<Villa, VillaUpdateDto>().ReverseMap();
            });

            _mapper = configuration.CreateMapper();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VillaDto>>> GetVillas()
        {
            _logger.LogInformation("Obtener las Villas");
            IEnumerable<Villa> villaList = await _db.Villas.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<VillaDto>>(villaList));
        }

        [HttpGet("id:int", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VillaDto>> GetVilla(int id)
        {

            if(id == 0)
            {
                _logger.LogError("Error al traer Villa con Id" + id);
                return BadRequest();
            }

            var villaEncontrada = await _db.Villas.FirstOrDefaultAsync(villa => villa.Id == id);

            if (villaEncontrada == null)
            {
                _logger.LogError("Obtener las Villas");
                return NotFound();
            }

            return Ok(_mapper.Map<VillaDto>(villaEncontrada));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDto>> CrearVilla([FromBody] VillaCreateDto createDto)
        {

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(await _db.Villas.FirstOrDefaultAsync(villa => villa.Nombre.ToLower() == createDto.Nombre.ToLower()) != null){
                ModelState.AddModelError("NombreExiste", "La Villa con ese nombre YA existe");
                return BadRequest(ModelState);
            }

            if(createDto == null)
            {
                return BadRequest(createDto);
            }

            Villa nuevaVilla = _mapper.Map<Villa>(createDto);

            await _db.Villas.AddAsync(nuevaVilla);
            await _db.SaveChangesAsync();

            return CreatedAtRoute("GetVilla", new {id = nuevaVilla.Id}, nuevaVilla);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var villaEncontrada = await _db.Villas.FirstOrDefaultAsync(villa => villa.Id == id);

            if (villaEncontrada == null)
            {
                return NotFound();
            }

            _db.Villas.Remove(villaEncontrada);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody]VillaUpdateDto updateDto)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            Villa villaActualiza = _mapper.Map<Villa>(updateDto);

            _db.Villas.Update(villaActualiza);
            await _db.SaveChangesAsync();

            return NoContent(); 
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePatchVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDto)
        {
            if (patchDto == null || id == 0)
            {
                return BadRequest();
            }

            var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(villa => villa.Id == id);

            VillaUpdateDto villaDto = _mapper.Map<VillaUpdateDto>(villa);

            if (villa == null)
            {
                return NotFound();
            }

            patchDto.ApplyTo(villaDto, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Villa villaActualiza = _mapper.Map<Villa>(villaDto);

            _db.Villas.Update(villaActualiza);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
