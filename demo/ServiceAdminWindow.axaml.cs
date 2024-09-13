using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using static demo.ServiceWindow;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using demo.EntityModels;
using System.Linq;

namespace demo;

public partial class ServiceAdminWindow : Window
{
    public List<ServicePresenter> sourceList { get; set; }
    public ObservableCollection<ServicePresenter> displayList { get; }

    private string _searchWord = string.Empty;

    private string[] _sortingSource = new string[3] { "Отсутствуют", "По возврастанию", "По убыванию" };
    private Dictionary<string, (int, int)> _dicountFilter = new Dictionary<string, (int, int)> {
        {"Все", (0, 0) },
        { "0-5", (0,5)},
        { "15-30", (5,15)},
        { "30-70", (30,70)},
        { "70-100", (70,100)},
    };

    public ServiceAdminWindow()
    {
        InitializeComponent();
        using (var dbContext = new AndrianovContext())
        {

            sourceList = dbContext.Services.Select(service =>
            new ServicePresenter
            {
                Title = service.Title,
                Discount = service.Discount,
                DurationInSeconds = service.DurationInSeconds,
                Id = service.Id,
                Cost = service.Cost,
                MainImagePath = service.MainImagePath,
            }
            ).ToList();
        }
        displayList = new ObservableCollection<ServicePresenter>(sourceList);
        SortingComboBox.ItemsSource = _sortingSource;
        ServiceListBox.ItemsSource = displayList;
        FilteringComboBox.ItemsSource = _dicountFilter.Keys;
        StatisticTextBlock.Text = String.Format("{0} из {1}", displayList.Count, sourceList.Count);


    }

    private void DisplayService()
    {

        var displayServiceList = sourceList;
        if (FilteringComboBox.SelectedIndex > 0) displayServiceList = displayServiceList.Where(service =>
        FilterByDiscount(service, FilteringComboBox.SelectionBoxItem.ToString())).ToList();
        if (!String.IsNullOrEmpty(_searchWord)) displayServiceList = displayServiceList.Where(service => SearchByWord(service, _searchWord)).ToList();
        if (SortingComboBox.SelectedIndex > 0) displayServiceList = SortingComboBox.SelectedIndex == 1 ?
                displayServiceList.OrderBy(service => service.Cost).ToList() :
                displayServiceList.OrderByDescending(service => service.Cost).ToList();
        displayList.Clear();
        foreach (var service in displayServiceList) { displayList.Add(service); }
        StatisticTextBlock.Text = String.Format("{0} из {1}", displayServiceList.Count, sourceList.Count);
    }


    private bool SearchByWord(ServicePresenter service, string word)
    {
        if (service.Title.Contains(word, StringComparison.CurrentCultureIgnoreCase)) return true;
        if (String.IsNullOrEmpty(service.Description)) return false;
        if (service.Description.Contains(word, StringComparison.CurrentCultureIgnoreCase)) return true;
        return false;
    }
    private bool FilterByDiscount(ServicePresenter servicePresenter, string key)
    {
        (int left, int right) = _dicountFilter[key];
        double discount = servicePresenter.Discount != null ? Convert.ToDouble(servicePresenter.Discount) * 100 : 0;
        return left <= discount && discount < right;
    }

    private void SearchTextBox_TextChanged(object? sender, Avalonia.Controls.TextChangedEventArgs e)
    {
        _searchWord = SearchTextBox.Text.ToString();
        DisplayService();
    }


    private void FilteringComboBox_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
    {
        DisplayService();
    }

    private void SortingComboBox_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
    {
        DisplayService();
    }

    private void EditButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var serviceID = Convert.ToInt32((sender as Button).Tag);
        var service = displayList.First(s => s.Id == serviceID);

        new AddEditWindow(service).Show(); Close();

    }

    private void RemoveButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var serviceID = Convert.ToInt32((sender as Button).Tag);
        using (var dbContext = new AndrianovContext())
        {
            var service = dbContext.Services.FirstOrDefault(service => service.Id == serviceID);
            var serviceLocal = displayList.First(service => service.Id == serviceID);
            if (service == null) return;

            if (dbContext.ClientServices.Any(item => item.ServiceId == serviceID))
            {
                Error.Text = "Не удалось удалить услугу с ID {serviceID}, так как она связана с клиентами.";
                return;
            }

            using var transaction = dbContext.Database.BeginTransaction();
            try
            {
                var servicePhotos = dbContext.ServicePhotos.Where(item => item.ServiceId == serviceID).ToList();
                if (servicePhotos.Any())
                {
                    dbContext.RemoveRange(servicePhotos);
                }

                dbContext.Services.Remove(service);
                displayList.Remove(serviceLocal);

                dbContext.SaveChanges();
                transaction.Commit();
                Error.Text = "Услуга с ID {serviceID} успешно удалена.";
            }
            catch (Exception ex)
            {
                Error.Text = "Ошибка при удалении услуги: {ex.Message}";
                transaction.Rollback();
            }
        }
    }


    private void Button_Click_1(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        new Records().Show(); Close();
    }

    private void Button_Click_2(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        new AddEditWindow().Show(); Close();
    }
}