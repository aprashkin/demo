using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using demo.EntityModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace demo;

public partial class ServiceWindow : Window
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

    public ServiceWindow()
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


    public class ServicePresenter() : Service
    {
        public int ServiceID { get => this.Id; }
        public string ServiceName { get => this.Title; }
        public string SeviceCostPerSeconds
        {
            get =>
                (this.Discount != 0 ?
                String.Format("{0:0.00} рублей за {1} минут", this.Cost - this.Cost * Convert.ToDecimal(this.Discount), this.DurationInSeconds)
                :
                String.Format("{0:0.00} рублей за {1} минут", this.Cost, this.DurationInSeconds)
                );
        }
        public Bitmap ServiceImage { get => GetBitmap(this.MainImagePath); }
        public decimal? OldCost { get => (this.Discount != 0 ? this.Cost : null); }
        public string? ServiceDiscount
        {
            get =>
                (this.Discount != 0 ?
                String.Format("* скидка {0}%", Discount * 100)
                : String.Empty
                );
        }

        private Bitmap GetBitmap(string fileName)
        {
            try
            {
                return new Bitmap($"Assets\\{fileName}");
            }
            catch (Exception ex)
            {
                return new Bitmap("");

            }
        }
    }
}