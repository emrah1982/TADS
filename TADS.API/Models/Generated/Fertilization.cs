using System;
using System.Collections.Generic;

namespace TADS.API.Models.Generated;

public partial class Fertilization
{
    public int Id { get; set; }

    public int FieldId { get; set; }

    public DateOnly Date { get; set; }

    public int FertilizerTypeId { get; set; }

    public decimal Amount { get; set; }

    public string Unit { get; set; } = null!;

    public string? Method { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public int Season { get; set; }

    public virtual Fertilizertype FertilizerType { get; set; } = null!;

    public virtual Field Field { get; set; } = null!;
}
