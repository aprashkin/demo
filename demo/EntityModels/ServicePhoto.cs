using System;
using System.Collections.Generic;

namespace demo.EntityModels;

public partial class ServicePhoto
{
    public int Id { get; set; }

    public int ServiceId { get; set; }

    public string PhotoPath { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;
}
