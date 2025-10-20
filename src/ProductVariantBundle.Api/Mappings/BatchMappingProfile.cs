using AutoMapper;
using ProductVariantBundle.Api.DTOs.Batch;
using ProductVariantBundle.Core.Models;

namespace ProductVariantBundle.Api.Mappings;

public class BatchMappingProfile : Profile
{
    public BatchMappingProfile()
    {
        // BatchOperationResult mappings
        CreateMap<BatchOperationResult<object>, BatchOperationResultDto<object>>();
        CreateMap<BatchItemResult<object>, BatchItemResultDto<object>>();

        // Enum mappings
        CreateMap<DTOs.Batch.ConflictResolutionStrategy, Core.Models.ConflictResolutionStrategy>().ReverseMap();

        // Batch request mappings
        CreateMap<BatchCreateVariantRequestDto, BatchCreateVariantRequest>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items.Select(item => new ProductVariantBatchItem
            {
                ProductMasterId = item.ProductMasterId,
                Price = item.Price,
                SKU = item.SKU,
                OptionValues = item.OptionValues.Where(ov => ov.VariantOptionId.HasValue).Select(ov => new VariantOptionValueBatchItem
                {
                    VariantOptionId = ov.VariantOptionId!.Value,
                    Value = ov.Value
                })
            })));

        // TODO: Fix mapping for UpdateVariantOptionValueDto - it has Id instead of VariantOptionId
        // CreateMap<BatchUpdateVariantRequestDto, BatchUpdateVariantRequest>()
        //     .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items.Select(item => new ProductVariantUpdateBatchItem
        //     {
        //         Id = item.Id,
        //         Price = item.Price,
        //         SKU = item.SKU,
        //         OptionValues = item.OptionValues != null ? item.OptionValues.Select(ov => new VariantOptionValueBatchItem
        //         {
        //             VariantOptionId = ov.VariantOptionId, // This property doesn't exist in UpdateVariantOptionValueDto
        //             Value = ov.Value
        //         }) : null
        //     })));

        CreateMap<BatchCreateBundleRequestDto, BatchCreateBundleRequest>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items.Select(item => new ProductBundleBatchItem
            {
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                SKU = item.SKU,
                Items = item.Items.Select(bi => new BundleItemBatchItem
                {
                    SellableItemId = bi.SellableItemId,
                    Quantity = bi.Quantity
                })
            })));
    }
}