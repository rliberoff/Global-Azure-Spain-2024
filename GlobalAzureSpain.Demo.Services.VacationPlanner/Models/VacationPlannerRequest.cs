using System.ComponentModel.DataAnnotations;

namespace GlobalAzureSpain.Demo.Services.VacationPlanner.Models;

public class VacationPlannerRequest
{
    [Required]
    public string Ask { get; init; } = string.Empty;
}
