using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
	[Route("api/VillaAPI")]
	[ApiController]
	public class VillaAPIController : ControllerBase
	{
		private readonly ApplicationDbContext _db;
		public VillaAPIController(ApplicationDbContext db)
		{
			_db = db;
		}

		// Custom Logging
		//private readonly ILogging _logger;
		//public VillaAPI(ILogging logger)
		//{
		//	_logger = logger;
		//}

		//private readonly ILogger<VillaDTO> _logger;

		//public VillaAPIController(ILogger<VillaDTO> logger)
		//{
		//	_logger = logger;
		//}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public ActionResult<IEnumerable<VillaDTO>> GetVillas()
		{
			//_logger.Log("Get all Villas.", "success"); (custom log)
			//_logger.LogInformation("Get all the Villas.");
			return Ok(_db.Villas.ToList());
		}


		[HttpGet("{id:int}", Name = "GetVilla")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult<VillaDTO> GetVilla(int id)
		{
			if (id == 0)
			{
				//_logger.Log("Invalid Id", "error"); (custom log)
				//_logger.LogError("Invalid Id");
				return BadRequest();
			}

			var villa = _db.Villas.FirstOrDefault(u => u.Id == id);
			if (villa == null)
			{
				return NotFound();
			}

			return Ok(villa);
		}


		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public ActionResult<VillaDTO> CreateVilla([FromBody] VillaDTO villaDTO)
		{
			if (_db.Villas.FirstOrDefault(u => u.Name.ToLower() == villaDTO.Name.ToLower()) != null)
			{
				ModelState.AddModelError("CustomErorr", "Villa already exists!");
				return BadRequest(ModelState);
			}

			if (villaDTO == null)
			{
				return BadRequest();
			}

			if (villaDTO.Id > 0)
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}

			Villa model = new()
			{
				Amenity = villaDTO.Amenity,
				Details = villaDTO.Details,
				ImageUrl = villaDTO.ImageUrl,
				Name = villaDTO.Name,
				Occupancy = villaDTO.Occupancy,
				Rate = villaDTO.Rate,
				Sqft = villaDTO.Sqft
			};

			_db.Villas.Add(model);
			_db.SaveChanges();

			//return Ok(villaDTO);
			return CreatedAtRoute("GetVilla", new { id = villaDTO.Id }, villaDTO);
		}


		[HttpDelete("{id:int}", Name = "DeleteVilla")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public IActionResult RemoveVilla(int id)
		{
			if (id == 0)
			{
				return BadRequest();
			}

			var villa = _db.Villas.FirstOrDefault(u => u.Id == id);
			if (villa == null)
			{
				return NotFound();
			}

			_db.Villas.Remove(villa);
			_db.SaveChanges();
			return NoContent();
		}


		[HttpPut("{id:int}", Name = "UpdateVilla")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public IActionResult UpdateVilla(int id, [FromBody] VillaDTO villaDTO)
		{
			if (villaDTO.Id == 0 || id != villaDTO.Id)
			{
				return BadRequest();
			}

			Villa model = new()
			{
				Amenity = villaDTO.Amenity,
				Details = villaDTO.Details,
				ImageUrl = villaDTO.ImageUrl,
				Name = villaDTO.Name,
				Occupancy = villaDTO.Occupancy,
				Rate = villaDTO.Rate,
				Sqft = villaDTO.Sqft
			};

			_db.Villas.Update(model);
			_db.SaveChanges();

			return NoContent();
		}


		[HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDTO> patchDTO)
		{
			if (patchDTO == null || id == 0)
			{
				return BadRequest();
			}

			var villa = _db.Villas.FirstOrDefault(u => u.Id == id);

			if (villa == null)
			{
				return BadRequest();
			}

			VillaDTO villaDto = new()
			{
				Amenity = villa.Amenity,
				Details = villa.Details,
				Id = villa.Id,
				ImageUrl = villa.ImageUrl,
				Name = villa.Name,
				Occupancy = villa.Occupancy,
				Rate = villa.Rate,
				Sqft = villa.Sqft
			};


			patchDTO.ApplyTo(villaDto, ModelState);

			villa.Name = villaDto.Name;
			villa.Details = villaDto.Details;
			villa.Rate = villaDto.Rate;
			villa.Sqft = villaDto.Sqft;
			villa.Occupancy = villaDto.Occupancy;
			villa.ImageUrl = villaDto.ImageUrl;
			villa.Amenity = villaDto.Amenity;


			_db.Villas.Update(villa);
			_db.SaveChanges();

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			return NoContent();
		}
	}
}
