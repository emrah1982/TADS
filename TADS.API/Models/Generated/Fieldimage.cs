using System;
using System.Collections.Generic;

namespace TADS.API.Models.Generated;

public partial class Fieldimage
{
    public int Id { get; set; }

    public int FieldId { get; set; }

    public string ImagePath { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime UploadDate { get; set; }

    public string ImageType { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual Field Field { get; set; } = null!;
}
