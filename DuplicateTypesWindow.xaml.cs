using Autodesk.Revit.DB;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace CopyFam;

public partial class DuplicateTypesWindow : Window
{
    private readonly List<SymbolItem> _items;
    private readonly SymbolItem? _initialItem;
    private bool _loading;
    public FamilySymbol? SelectedSymbol { get; private set; }
    public string NamePrefix { get; private set; } = "";
    public int StartNumber { get; private set; }
    public int EndNumber { get; private set; }
    public bool IsEnglish => (LanguageCombo?.SelectedItem as ComboBoxItem)?.Tag?.ToString() == "en";

    public DuplicateTypesWindow(IEnumerable<FamilySymbol> symbols, ElementId? selectedTypeId)
    {
        InitializeComponent();
        _items = symbols.Select(s => new SymbolItem(s)).ToList();
        _initialItem = _items.FirstOrDefault(x => selectedTypeId is not null && x.Symbol.Id == selectedTypeId);
        SelectedFamilyOnlyBox.IsEnabled = _initialItem is not null;
        SelectedFamilyOnlyBox.IsChecked = _initialItem is not null;
        ReloadFilters();
        PrefixBox.TextChanged += (_, _) => UpdatePreview();
        StartBox.TextChanged += (_, _) => UpdatePreview();
        EndBox.TextChanged += (_, _) => UpdatePreview();
    }

    private IEnumerable<SymbolItem> FilteredItems() =>
        SelectedFamilyOnlyBox.IsChecked == true && _initialItem is not null
            ? _items.Where(x => x.FamilyName == _initialItem.FamilyName && x.CategoryName == _initialItem.CategoryName)
            : _items;

    private void ReloadFilters()
    {
        _loading = true;
        string? oldCategory = CategoryCombo.SelectedItem as string;
        var categories = FilteredItems().Select(x => x.CategoryName).Distinct().OrderBy(x => x).ToList();
        CategoryCombo.ItemsSource = categories;
        CategoryCombo.SelectedItem = categories.Contains(oldCategory ?? "") ? oldCategory : _initialItem?.CategoryName ?? categories.FirstOrDefault();
        _loading = false;
        ReloadFamilies();
    }

    private void ReloadFamilies()
    {
        string? category = CategoryCombo.SelectedItem as string;
        if (category is null) return;
        _loading = true;
        string? oldFamily = FamilyCombo.SelectedItem as string;
        var families = FilteredItems().Where(x => x.CategoryName == category).Select(x => x.FamilyName).Distinct().OrderBy(x => x).ToList();
        FamilyCombo.ItemsSource = families;
        FamilyCombo.SelectedItem = families.Contains(oldFamily ?? "") ? oldFamily : _initialItem?.FamilyName ?? families.FirstOrDefault();
        _loading = false;
        ReloadTypes();
    }

    private void ReloadTypes()
    {
        string? category = CategoryCombo.SelectedItem as string;
        string? family = FamilyCombo.SelectedItem as string;
        var types = FilteredItems().Where(x => x.CategoryName == category && x.FamilyName == family).OrderBy(x => x.TypeName).ToList();
        SourceCombo.ItemsSource = types;
        SourceCombo.SelectedItem = types.FirstOrDefault(x => _initialItem is not null && x.Symbol.Id == _initialItem.Symbol.Id) ?? types.FirstOrDefault();
    }

    private void FilterChanged(object sender, RoutedEventArgs e) { if (IsLoaded) ReloadFilters(); }
    private void CategoryChanged(object sender, SelectionChangedEventArgs e) { if (!_loading) ReloadFamilies(); }
    private void FamilyChanged(object sender, SelectionChangedEventArgs e) { if (!_loading) ReloadTypes(); }

    private void SourceChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SourceCombo.SelectedItem is not SymbolItem item) return;
        var match = Regex.Match(item.Symbol.Name, @"^(.*?)(\d+)$");
        PrefixBox.Text = match.Success ? match.Groups[1].Value : item.Symbol.Name + " ";
        if (match.Success) StartBox.Text = match.Groups[2].Value;
        UpdatePreview();
    }

    private void UpdatePreview()
    {
        if (PreviewText is null) return;
        PreviewText.Text = int.TryParse(StartBox?.Text, out int start) && int.TryParse(EndBox?.Text, out int end) && end >= start
            ? $"{(IsEnglish ? "Preview" : "Xem trước")}: {PrefixBox?.Text}{start} … {PrefixBox?.Text}{end} ({end - start + 1} Type)"
            : (IsEnglish ? "Enter a valid number range." : "Nhập dải số hợp lệ.");
    }

    private void LanguageChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!IsInitialized || LanguageLabel is null) return;
        bool en = IsEnglish;
        Title = en ? "Copy and rename Family Types" : "Copy và đặt tên Family Type";
        LanguageLabel.Text = en ? "Language" : "Ngôn ngữ";
        FilterHeading.Text = en ? "Family Type filters" : "Bộ lọc Family Type";
        SelectedFamilyOnlyBox.Content = en ? "Show only the selected element's Family" : "Chỉ hiện Family của đối tượng đang chọn";
        CategoryLabel.Text = "Category";
        FamilyLabel.Text = "Family";
        SourceLabel.Text = en ? "Source Type" : "Type nguồn";
        PrefixLabel.Text = en ? "Name prefix" : "Tên gốc";
        StartLabel.Text = en ? "Start number" : "Từ số";
        EndLabel.Text = en ? "End number" : "Đến số";
        CancelButton.Content = en ? "Cancel" : "Hủy";
        CreateButton.Content = en ? "Create Types" : "Tạo Type";
        UpdatePreview();
    }

    private void CreateClicked(object sender, RoutedEventArgs e)
    {
        if (SourceCombo.SelectedItem is not SymbolItem item || string.IsNullOrWhiteSpace(PrefixBox.Text) ||
            !int.TryParse(StartBox.Text, out int start) || !int.TryParse(EndBox.Text, out int end) || start < 0 || end < start || end - start > 999)
        {
            MessageBox.Show(IsEnglish
                ? "Enter a name prefix and a valid range (maximum 1,000 Types)."
                : "Vui lòng nhập tên gốc và dải số hợp lệ (tối đa 1000 Type).",
                "CopyFam", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        SelectedSymbol = item.Symbol;
        NamePrefix = PrefixBox.Text;
        StartNumber = start;
        EndNumber = end;
        DialogResult = true;
    }

    private sealed record SymbolItem(FamilySymbol Symbol)
    {
        public string CategoryName => Symbol.Category?.Name ?? "Không có Category";
        public string FamilyName => Symbol.Family.Name;
        public string TypeName => Symbol.Name;
    }
}
