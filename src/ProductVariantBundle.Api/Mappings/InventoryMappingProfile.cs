using AutoMapper;
using ProductVariantBundle.Api.DTOs.Inventory;
using ProductVariantBundle.Core.Entities;

namespace ProductVariantBundle.Api.Mappings;

public class InventoryMappingProfile : Profile
{
    public InventoryMappingProfile()
    {
        CreateMap<InventoryRecord, InventoryRecordDto>()
            .ForMember(dest => dest.WarehouseCode, opt => opt.MapFrom(src => src.Warehouse.Code))
            .ForMember(dest => dest.SKU, opt => opt.MapFrom(src => src.SellableItem.SKU));
    }
}