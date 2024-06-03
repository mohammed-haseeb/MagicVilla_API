using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;

namespace MagicVilla_VillaAPI
{
	public class MappingConfig : Profile
	{
		public MappingConfig()
		{
			// Mapping for Villa 
			CreateMap<Villa, VillaDTO>();
			CreateMap<VillaDTO, Villa>();
			CreateMap<Villa, VillaCreateDTO>().ReverseMap();
			CreateMap<Villa, VillaUpdateDTO>().ReverseMap();


			// Mapping for Villa Numbers
			CreateMap<VillaNumber, VillaNumberDTO>().ReverseMap();
			CreateMap<VillaNumber, VillaNumberCreateDTO>().ReverseMap();
			CreateMap<VillaNumber, VillaNumberUpdateDTO>().ReverseMap();
		}
	}
}
