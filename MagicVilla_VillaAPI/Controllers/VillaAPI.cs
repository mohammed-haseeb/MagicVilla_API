using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
	[Route("api/VillaAPI")]
	[ApiController]
	public class VillaAPI : ControllerBase
	{
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public ActionResult<IEnumerable<VillaDTO>> GetVillas()
		{
			return VillaStore.villaList;
		}


		[HttpGet("{id:int}", Name = "GetVilla")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult<VillaDTO> GetVilla(int id)
		{
			if (id == 0)
			{
				return BadRequest();
			}

			var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
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
			if (VillaStore.villaList.FirstOrDefault(u => u.Name.ToLower() == villaDTO.Name.ToLower()) != null)
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

			villaDTO.Id = VillaStore.villaList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
			VillaStore.villaList.Add(villaDTO);

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

			var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
			if (villa == null)
			{
				return NotFound();
			}

			VillaStore.villaList.Remove(villa);
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

			var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
			villa.Name = villaDTO.Name;
			villa.Sqft = villaDTO.Sqft;
			villa.Occupancy = villaDTO.Occupancy;

			return NoContent();


		}


		[HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDTO> patchDTO)
		{
			if (patchDTO == null || id == 0)
			{
				return BadRequest();
			}

			var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
			if (villa == null)
			{
				return NotFound();
			}

			patchDTO.ApplyTo(villa, ModelState);
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			return NoContent();
		}
	}
}
