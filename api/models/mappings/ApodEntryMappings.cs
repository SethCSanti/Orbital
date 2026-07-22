using Orbital.Api.Models.DTOs;
using Orbital.Api.Models.Entities;
namespace Orbital.Api.Models.Mappings;

public static class ApodEntryMappings
{
    public static ApodEntryDto ToDto(this ApodEntry entity)
    {
        // your mapping code goes here
        return new ApodEntryDto
        {
            Date = entity.Date,
            Title = entity.Title,
            Explanation = entity.Explanation,
            Url = entity.Url,
            MediaType = entity.MediaType,
            HdUrl = entity.HdUrl,
            Copyright = entity.Copyright
        };
    }
}