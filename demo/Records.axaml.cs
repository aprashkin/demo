using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using demo.EntityModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace demo;

public partial class Records : Window
{
    public Records()
    {
        InitializeComponent();
        UpdateList();
    }

    private void UpdateList()
    {
        using (var context = new AndrianovContext())
        {

            List.ItemsSource = context.ClientServices.Include(x => x.Service).Include(x => x.Client).Select(x => new
            {
                NameService = x.Service.Title,
                FIO = x.Client.LastName + " " + x.Client.FirstName + " " + x.Client.Patronymic,
                Email = x.Client.Email,
                Phone = x.Client.Phone,
                Date = x.StartTime,
            }).ToList();

        }

    }
    public void Back(object sender, RoutedEventArgs e)
    {
        new ServiceAdminWindow().Show();
        this.Close();
    }
}