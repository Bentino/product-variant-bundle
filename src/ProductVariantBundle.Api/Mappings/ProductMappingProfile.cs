using AutoMapper;
using ProductVariantBundle.Api.DTOs.Common;
using ProductVariantBundle.Api.DTOs.Products;
using ProductVariantBundle.Core.Entities;
using ProductVariantBundle.Core.Models;

namespace ProductVariantBundle.Api.Mappings;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        // ProductMaster mappings
        CreateMap<ProductMaster, ProductMasterDto>();
        CreateMap<CreateProductMasterDto, ProductMaster>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.Variants, opt => opt.Ignore());
        
        CreateMap<UpdateProductMasterDto, ProductMaster>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.Variants, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // ProductVariant mappings
        CreateMap<ProductVariant, ProductVariantDto>()
            .ForMember(dest => dest.SKU, opt => opt.MapFrom(src => src.SellableItem != null ? src.SellableItem.SKU : string.Empty));
        
        CreateMap<CreateProductVariantDto, ProductVariant>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.CombinationKey, opt => opt.Ignore())
            .ForMember(dest => dest.ProductMaster, opt => opt.Ignore())
            .ForMember(dest => dest.SellableItem, opt => opt.Ignore())
            .ForMember(dest => dest.OptionValues, opt => opt.Ignore());

        CreateMap<UpdateProductVariantDto, ProductVariant>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.CombinationKey, opt => opt.Ignore())
            .ForMember(dest => dest.ProductMaster, opt => opt.Ignore())
            .ForMember(dest => dest.ProductMasterId, opt => opt.Ignore())
            .ForMember(dest => dest.SellableItem, opt => opt.Ignore())
            .ForMember(dest => dest.OptionValues, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // VariantOption mappings
        CreateMap<VariantOption, VariantOptionDto>();
        CreateMap<CreateVariantOptionDto, VariantOption>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.ProductMaster, opt => opt.Ignore())
            .ForMember(dest => dest.Values, opt => opt.Ignore());

        CreateMap<UpdateVariantOptionDto, VariantOption>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.ProductMaster, opt => opt.Ignore())
            .ForMember(dest => dest.ProductMasterId, opt => opt.Ignore())
            .ForMember(dest => dest.Values, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // VariantOptionValue mappings
        CreateMap<VariantOptionValue, VariantOptionValueDto>()
            .ForMember(dest => dest.OptionName, opt => opt.MapFrom(src => src.VariantOption.Name))
            .ForMember(dest => dest.OptionSlug, opt => opt.MapFrom(src => src.VariantOption.Slug));

        CreateMap<CreateVariantOptionValueDto, VariantOptionValue>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.VariantOption, opt => opt.Ignore())
            .ForMember(dest => dest.ProductVariant, opt => opt.Ignore());

        // Filter mappings
        CreateMap<ProductFilterDto, ProductFilter>();
        
        // Pagination mappings
        CreateMap<ProductVariantBundle.Core.Models.PaginationMeta, ProductVariantBundle.Api.DTOs.Common.PaginationMeta>();
    }
}