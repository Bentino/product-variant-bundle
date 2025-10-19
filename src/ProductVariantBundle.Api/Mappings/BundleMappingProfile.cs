using AutoMapper;
using ProductVariantBundle.Api.DTOs.Bundles;
using ProductVariantBundle.Api.DTOs.Common;
using ProductVariantBundle.Core.Entities;
using ProductVariantBundle.Core.Interfaces;
using ProductVariantBundle.Core.Models;

namespace ProductVariantBundle.Api.Mappings;

public class BundleMappingProfile : Profile
{
    public BundleMappingProfile()
    {
        // ProductBundle mappings
        CreateMap<ProductBundle, ProductBundleDto>()
            .ForMember(dest => dest.SKU, opt => opt.MapFrom(src => src.SellableItem != null ? src.SellableItem.SKU : string.Empty));

        CreateMap<CreateProductBundleDto, ProductBundle>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.SellableItem, opt => opt.Ignore())
            .ForMember(dest => dest.Items, opt => opt.Ignore());

        CreateMap<UpdateProductBundleDto, ProductBundle>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.SellableItem, opt => opt.Ignore())
            .ForMember(dest => dest.Items, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // BundleItem mappings
        CreateMap<BundleItem, BundleItemDto>()
            .ForMember(dest => dest.SKU, opt => opt.MapFrom(src => src.SellableItem.SKU))
            .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => 
                src.SellableItem.Variant != null ? src.SellableItem.Variant.ProductMaster.Name :
                src.SellableItem.Bundle != null ? src.SellableItem.Bundle.Name : string.Empty));

        CreateMap<CreateBundleItemDto, BundleItem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.BundleId, opt => opt.Ignore())
            .ForMember(dest => dest.Bundle, opt => opt.Ignore())
            .ForMember(dest => dest.SellableItem, opt => opt.Ignore());

        // Bundle availability mappings
        CreateMap<BundleAvailability, BundleAvailabilityDto>();

        CreateMap<ComponentAvailability, ComponentAvailabilityDto>();

        // Filter mappings
        CreateMap<BundleFilterDto, BundleFilter>();
        
        // Pagination mappings
        CreateMap<ProductVariantBundle.Core.Models.PaginationMeta, ProductVariantBundle.Api.DTOs.Common.PaginationMeta>();
    }
}