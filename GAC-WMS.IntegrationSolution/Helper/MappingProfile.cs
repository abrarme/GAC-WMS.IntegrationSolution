using AutoMapper;
using GAC_WMS.IntegrationSolution.DTO;
using GAC_WMS.IntegrationSolution.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GAC_WMS.IntegrationSolution.Helper
{

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CustomerDto, Customer>().ReverseMap();
            CreateMap<ProductDto, Product>().ReverseMap();
        }
    }
}
