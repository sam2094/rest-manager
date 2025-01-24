using Application.DTOs.Responses;
using AutoMapper;
using Domain.Models;

namespace API.Mapping;

public class ModelMapper : Profile
{
    public ModelMapper()
    {
        CreateMap<Table, TableDto>().ReverseMap();
    }
}
