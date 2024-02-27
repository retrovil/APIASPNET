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

        public VillaController(ILogger<VillaController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;

        }

        [HttpGet]
        public ActionResult<IEnumerable<VillaDto>> GetVillas()
        {
            _logger.LogInformation("Obtener las Villas");
            return Ok(_db.Villas.ToList());
        }

        [HttpGet("id:int", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<VillaDto> GetVilla(int id)
        {

            if(id == 0)
            {
                _logger.LogError("Error al traer Villa con Id" + id);
                return BadRequest();
            }

            var villaEncontrada = _db.Villas.FirstOrDefault(villa => villa.Id == id);

            if (villaEncontrada == null)
            {
                _logger.LogError("Obtener las Villas");
                return NotFound();
            }

            return Ok(villaEncontrada);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDto> CrearVilla([FromBody] VillaDto villaDto)
        {

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(_db.Villas.FirstOrDefault(villa => villa.Nombre.ToLower() == villaDto.Nombre.ToLower()) != null){
                ModelState.AddModelError("NombreExiste", "La Villa con ese nombre YA existe");
                return BadRequest(ModelState);
            }

            if(villaDto == null)
            {
                return BadRequest(villaDto);
            }

            if(villaDto.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            Villa nuevaVilla = new()
            {
                Nombre = villaDto.Nombre,
                Id = villaDto.Id,
                Detalles = villaDto.Detalles,
                ImagenUrl = villaDto.ImagenUrl,
                Amenidad = villaDto.Amenidad,
                Tarifa = villaDto.Tarifa,
                Ocupantes = villaDto.Ocupantes,
                MetrosCuadrados = villaDto.MetrosCuadrados
            };

            _db.Villas.Add(nuevaVilla);
            _db.SaveChanges();

            return CreatedAtRoute("GetVilla", new {id = villaDto.Id}, villaDto);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var villaEncontrada = _db.Villas.FirstOrDefault(villa => villa.Id == id);

            if (villaEncontrada == null)
            {
                return NotFound();
            }

            _db.Villas.Remove(villaEncontrada);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateVilla(int id, [FromBody]VillaDto villaDto)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            Villa villaActualiza = new()
            {
                Id = villaDto.Id,
                Nombre = villaDto.Nombre,
                Detalles = villaDto.Detalles,
                ImagenUrl = villaDto.ImagenUrl,
                Amenidad = villaDto.Amenidad,
                Tarifa = villaDto.Tarifa,
                Ocupantes = villaDto.Ocupantes,
                MetrosCuadrados = villaDto.MetrosCuadrados
            };

            _db.Villas.Update(villaActualiza);
            _db.SaveChanges();

            return NoContent(); 
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdatePatchVilla(int id, JsonPatchDocument<VillaDto> patchDto)
        {
            if (patchDto == null || id == 0)
            {
                return BadRequest();
            }

            var villa = _db.Villas.AsNoTracking().FirstOrDefault(villa => villa.Id == id);

            VillaDto villaDto = new()
            {
                Nombre = villa.Nombre,
                Id = villa.Id,
                Detalles = villa.Detalles,
                ImagenUrl = villa.ImagenUrl,
                Amenidad = villa.Amenidad,
                Tarifa = villa.Tarifa,
                Ocupantes = villa.Ocupantes,
                MetrosCuadrados = villa.MetrosCuadrados
            };

            if (villa == null)
            {
                return NotFound();
            }

            patchDto.ApplyTo(villaDto, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Villa villaActualiza = new()
            {
                Nombre = villaDto.Nombre,
                Id = villaDto.Id,
                Detalles = villaDto.Detalles,
                ImagenUrl = villaDto.ImagenUrl,
                Amenidad = villaDto.Amenidad,
                Tarifa = villaDto.Tarifa,
                Ocupantes = villaDto.Ocupantes,
                MetrosCuadrados = villaDto.MetrosCuadrados
            };

            _db.Villas.Update(villaActualiza);
            _db.SaveChanges();

            return NoContent();
        }
    }
}
