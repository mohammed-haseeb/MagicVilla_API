using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Controllers
{
	[Route("api/VillaAPI")]
	[ApiController]
	public class VillaAPIController : ControllerBase
	{
		private readonly ApplicationDbContext _db;
		private readonly IMapper _mapper;
		private readonly ILogger<VillaAPIController> _logger;
		public VillaAPIController(ApplicationDbContext db, ILogger<VillaAPIController> logger, IMapper mapper)
		{
			_db = db;
			_logger = logger;
			_mapper = mapper;
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
		{
			//_logger.LogInformation("Getting All Villas.");
			IEnumerable<Villa> villaList = await _db.Villas.ToListAsync();
			return Ok(_mapper.Map<List<VillaDTO>>(villaList));
		}


		[HttpGet("{id:int}", Name = "GetVilla")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<VillaDTO>> GetVilla(int id)
		{
			if (id == 0)
			{
				//_logger.LogError("Invalid Id");
				return BadRequest();
			}

			var villa = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
			if (villa == null)
			{
				return NotFound();
			}

			return Ok(_mapper.Map<VillaDTO>(villa));
		}


		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody] VillaCreateDTO createDTO)
		{
			if (await _db.Villas.FirstOrDefaultAsync(u => u.Name.ToLower() == createDTO.Name.ToLower()) != null)
			{
				ModelState.AddModelError("CustomErorr", "Villa already exists!");
				return BadRequest(ModelState);
			}

			if (createDTO == null)
			{
				return BadRequest();
			}

			Villa model = _mapper.Map<Villa>(createDTO);

			await _db.Villas.AddAsync(model);
			await _db.SaveChangesAsync();

			//return Ok(villaDTO);
			return CreatedAtRoute("GetVilla", new { id = model.Id }, model);
		}


		[HttpDelete("{id:int}", Name = "DeleteVilla")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> RemoveVilla(int id)
		{
			if (id == 0)
			{
				return BadRequest();
			}

			var villa = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
			if (villa == null)
			{
				return NotFound();
			}

			_db.Villas.Remove(villa);
			await _db.SaveChangesAsync();
			return NoContent();
		}


		[HttpPut("{id:int}", Name = "UpdateVilla")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDTO updateDto)
		{
			if (updateDto.Id == 0 || id != updateDto.Id)
			{
				return BadRequest();
			}

			Villa model = _mapper.Map<Villa>(updateDto);

			_db.Villas.Update(model);
			await _db.SaveChangesAsync();

			return NoContent();
		}


		[HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
		{
			if (patchDTO == null || id == 0)
			{
				return BadRequest();
			}

			var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

			if (villa == null)
			{
				return BadRequest();
			}

			VillaUpdateDTO villaDto = _mapper.Map<VillaUpdateDTO>(villa);

			patchDTO.ApplyTo(villaDto, ModelState);

			Villa patchedVilla = _mapper.Map<Villa>(villaDto);

			_db.Villas.Update(patchedVilla);
			await _db.SaveChangesAsync();

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			return NoContent();
		}
	}
}
