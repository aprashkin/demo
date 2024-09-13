using System;
using System.Collections.Generic;

namespace demo.EntityModels;

public partial class Service
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public decimal Cost { get; set; }

    public int DurationInSeconds { get; set; }

    public string? Description { get; set; }

    public double? Discount { get; set; }

    public string? MainImagePath { get; set; }

    public Avalonia.Media.Imaging.Bitmap? Photokartochka
    {
        get
        {
            try
            {
                return new Avalonia.Media.Imaging.Bitmap(AppDomain.CurrentDomain.BaseDirectory + "" + MainImagePath);
            }
            catch { return null; }
        }

    }

    public virtual ICollection<ClientService> ClientServices { get; set; } = new List<ClientService>();

    public virtual ICollection<ServicePhoto> ServicePhotos { get; set; } = new List<ServicePhoto>();
}
