using MagicVilla_Web.Models.Dto;

namespace MagicVilla_Web.Services.IServices
{
	public interface IVillaService
	{
		Task<T> GetAllAsync<T>();
		Task<T> GetAsync<T>(int id);
		Task<T> CreateAsync<T>(VillaCreateDTO createDto);
		Task<T> UpdateAsync<T>(VillaUpdateDTO updateDto);
		Task<T> DeleteAsync<T>(int id);
	}
}
