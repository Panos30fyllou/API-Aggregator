using ApiAggregator.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace ApiAggregator.Models.Requests;

public class AggregationRequest
{
	[Required]
	public string SearchQuery { get; set; }
	public string? City { get; set; }
	public SortBy? SortBy { get; set; }
}