using System.Net;
using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
	[Route("api/VillaNumberAPI")]
	[ApiController]
	public class VillaNumberAPIController : ControllerBase
	{
		private readonly IVillaNumberRepository _dbVillaNo;
		private readonly IVillaRepository _dbVilla;
		private readonly APIResponse _response;
		private readonly IMapper _mapper;

		public VillaNumberAPIController(IVillaNumberRepository dbVillaNo, IVillaRepository dbVilla, IMapper mapper)
		{
			_dbVillaNo = dbVillaNo;
			_dbVilla = dbVilla;
			_mapper = mapper;
			this._response = new();
		}

		[HttpGet(Name = "GetVillaNumbers")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<APIResponse>> GetAllVillaNumbers()
		{
			try
			{
				IEnumerable<VillaNumber> villaNumbersList = await _dbVillaNo.GetAllAsync();
				_response.Result = _mapper.Map<List<VillaNumberDTO>>(villaNumbersList);
				_response.StatusCode = HttpStatusCode.OK;
				return Ok(_response);
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.ErrorMessages = new List<string> { ex.ToString() };
				return _response;
			}
		}

		[HttpGet("{villaNo:int}", Name = "GetVillaNumber")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<APIResponse>> GetVillaNumber(int villaNo)
		{
			try
			{
				if (villaNo == 0)
				{
					_response.StatusCode = HttpStatusCode.BadRequest;
					return BadRequest(_response);
				}

				var villaNumber = await _dbVillaNo.GetAsync(u => u.VillaNo == villaNo);

				if (villaNumber == null)
				{
					_response.StatusCode = HttpStatusCode.NotFound;
					return NotFound(_response);
				}

				_response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
				_response.StatusCode = HttpStatusCode.OK;
				return Ok(_response);
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.ErrorMessages = new List<string> { ex.ToString() };
				return _response;
			}

		}

		[HttpPost(Name = "CreateVillaNumber")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] VillaNumberCreateDTO createDto)
		{
			try
			{

				if (await _dbVillaNo.GetAsync(u => u.VillaNo == createDto.VillaNo) != null)
				{
					ModelState.AddModelError("CustomError", "Villa Number already exists!");
					return BadRequest(ModelState);
				}

				if (await _dbVilla.GetAsync(u => u.Id == createDto.VillaID) == null)
				{
					ModelState.AddModelError("CustomError", "Villa ID is Invalid!");
					return BadRequest(ModelState);
				}

				if (createDto == null)
				{
					return BadRequest(createDto);
				}

				var villaNumber = _mapper.Map<VillaNumber>(createDto);
				await _dbVillaNo.CreateAsync(villaNumber);

				_response.Result = _mapper.Map<VillaNumberCreateDTO>(villaNumber);
				_response.StatusCode = HttpStatusCode.OK;

				return CreatedAtRoute("GetVillaNumber", new { VillaNo = villaNumber.VillaNo }, _response);
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.ErrorMessages = new List<string> { ex.ToString() };
				return _response;
			}
		}


		[HttpDelete("{villaNo:int}", Name = "DeleteVillaNumber")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<APIResponse>> RemoveVillaNumber(int villaNo)
		{
			try
			{
				if (villaNo == 0)
				{
					return BadRequest();
				}

				var villaNumber = await _dbVillaNo.GetAsync(u => u.VillaNo == villaNo);

				if (villaNumber == null)
				{
					return NotFound();
				}

				await _dbVillaNo.RemoveAsync(villaNumber);

				_response.StatusCode = HttpStatusCode.NoContent;
				return Ok(_response);

			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.ErrorMessages = new List<string> { ex.ToString() };
				return _response;
			}
		}


		[HttpPut("{villaNo:int}", Name = "UpdateVillaNumber")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int villaNo, [FromBody] VillaNumberUpdateDTO updateDto)
		{
			try
			{
				if (updateDto.VillaNo == 0 || villaNo != updateDto.VillaNo)
				{
					return BadRequest();
				}


				if (await _dbVilla.GetAsync(u => u.Id == updateDto.VillaID) == null)
				{
					ModelState.AddModelError("CustomError", "Villa ID is Invalid!");
					return BadRequest(ModelState);
				}

				var villaNumber = _mapper.Map<VillaNumber>(updateDto);

				await _dbVillaNo.UpdateAsync(villaNumber);

				_response.StatusCode = HttpStatusCode.NoContent;
				return Ok(_response);
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.ErrorMessages = new List<string> { ex.ToString() };
				return _response;
			}
		}
	}
}
