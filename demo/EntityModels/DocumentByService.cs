using System;
using System.Collections.Generic;

namespace demo.EntityModels;

public partial class DocumentByService
{
    public int Id { get; set; }

    public int ClientServiceId { get; set; }

    public string DocumentPath { get; set; } = null!;

    public virtual ClientService ClientService { get; set; } = null!;
}
