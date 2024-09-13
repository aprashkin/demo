using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using demo.EntityModels;
using System.Linq;
using static demo.ServiceWindow;

namespace demo;

public partial class AddEditWindow : Window
{
    private ServicePresenter _service;
    private bool _isEditMode;

    public AddEditWindow() : this(null)
    {
    }

    public AddEditWindow(ServicePresenter? service = null)
    {
        InitializeComponent();

        if (service != null)
        {
            _isEditMode = true;
            _service = service;
            LoadServiceData();
        }
        else
        {
            _isEditMode = false;
            _service = new ServicePresenter();
        }
    }

    private void LoadServiceData()
    {
        ServiceNameTextBox.Text = _service.Title;
        ServiceCostTextBox.Text = _service.Cost.ToString();
        ServiceDurationTextBox.Text = _service.DurationInSeconds.ToString();
        ServiceDiscountTextBox.Text = _service.Discount.ToString();
        ServiceDescriptionTextBox.Text = _service.Description;

        if (!string.IsNullOrEmpty(_service.MainImagePath))
        {
            ServiceImagePreview.Source = new Bitmap(_service.MainImagePath);
        }
    }



    public void OnButtonClick(object sender, RoutedEventArgs args)
    {
        OpenFileDialog dialog = new OpenFileDialog();
        dialog.Filters.Add(new FileDialogFilter { Name = "Images", Extensions = { "png", "jpg", "jpeg" } });
        var result = dialog.ShowAsync(this);

        result.ContinueWith(t =>
        {
            if (t.Result.Length > 0)
            {
                _service.MainImagePath = t.Result[0];
                ServiceImagePreview.Source = new Bitmap(_service.MainImagePath);
            }
        });
    }


    public void OnButtonClick1(object sender, RoutedEventArgs args)
    {
        if (string.IsNullOrEmpty(ServiceNameTextBox.Text) ||
            !decimal.TryParse(ServiceCostTextBox.Text, out var cost) ||
            !int.TryParse(ServiceDurationTextBox.Text, out var duration) ||
            duration <= 0 || duration > 14400)
        {
            Error.Text = "Проверьте правильность введённых данных.";
            return;
        }

        _service.Title = ServiceNameTextBox.Text;
        _service.Cost = cost;
        _service.DurationInSeconds = duration;
        _service.Discount = int.Parse(ServiceDiscountTextBox.Text);
        _service.Description = ServiceDescriptionTextBox.Text;

        if (_isEditMode)
        {
            UpdateService(_service);
        }
        else
        {
            AddNewService(_service);
        }

        new ServiceAdminWindow().Show(); Close();
    }

    public void OnButtonClick2(object sender, RoutedEventArgs args)
    {
        new ServiceAdminWindow().Show(); Close();
    }

    private void AddNewService(ServicePresenter service)
    {
        using (var dbContext = new AndrianovContext())
        {
            var maxId = dbContext.Services.Max(s => (int?)s.Id) ?? 0;

            dbContext.Services.Add(new Service
            {
                Id = maxId + 1,
                Title = service.Title,
                Cost = service.Cost,
                DurationInSeconds = service.DurationInSeconds,
                Discount = service.Discount,
                Description = service.Description,
                MainImagePath = service.MainImagePath
            });

            dbContext.SaveChanges();
        }
    }

    private void UpdateService(ServicePresenter service)
    {
        using (var dbContext = new AndrianovContext())
        {
            var existingService = dbContext.Services.FirstOrDefault(s => s.Id == service.Id);
            if (existingService != null)
            {
                existingService.Title = service.Title;
                existingService.Cost = service.Cost;
                existingService.DurationInSeconds = service.DurationInSeconds;
                existingService.Discount = service.Discount;
                existingService.Description = service.Description;
                existingService.MainImagePath = service.MainImagePath;
                dbContext.SaveChanges();
            }
        }
    }
}
