using DroneService.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace DroneService.Application.Contracts.Fields;

public class CreateFieldModel
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "Příspěvek musí mít nějaký text!")]
    [MaxLength(Field.Metadata.ContentLenght)]
    public string Name { get; set; } = null!;
    public double Area { get; set; }
    public string CurrentCrops { get; set; } = null!;
    public int ArcGisId { get; set; }
    public int AtticBlock { get; set; }
    public string BlockType { get; set; } = null!;
    public string Municipality { get; set; } = null!;
}
