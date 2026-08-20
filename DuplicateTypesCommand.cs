using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CopyFam;

[Transaction(TransactionMode.Manual)]
public sealed class DuplicateTypesCommand : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        Document doc = commandData.Application.ActiveUIDocument.Document;
        var symbols = new FilteredElementCollector(doc)
            .OfClass(typeof(FamilySymbol)).Cast<FamilySymbol>()
            .Where(x => x.Family is not null)
            .OrderBy(x => x.Family.Name).ThenBy(x => x.Name)
            .ToList();

        if (symbols.Count == 0)
        {
            TaskDialog.Show("CopyFam", "Dự án không có Family Type để sao chép.");
            return Result.Cancelled;
        }

        ElementId? selectedTypeId = GetSelectedTypeId(commandData.Application.ActiveUIDocument);
        var window = new DuplicateTypesWindow(symbols, selectedTypeId);
        if (window.ShowDialog() != true) return Result.Cancelled;

        FamilySymbol source = window.SelectedSymbol!;
        HashSet<string> existing = source.Family.GetFamilySymbolIds()
            .Select(id => doc.GetElement(id)?.Name ?? "")
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var created = new List<string>();
        var skipped = new List<string>();

        using var transaction = new Transaction(doc, "Copy và đặt tên Family Type");
        transaction.Start();
        try
        {
            for (int i = window.StartNumber; i <= window.EndNumber; i++)
            {
                string name = $"{window.NamePrefix}{i}";
                if (existing.Contains(name)) { skipped.Add(name); continue; }
                source.Duplicate(name);
                existing.Add(name);
                created.Add(name);
            }
            transaction.Commit();
        }
        catch (Exception ex)
        {
            if (transaction.HasStarted()) transaction.RollBack();
            message = ex.Message;
            return Result.Failed;
        }

        string summary = window.IsEnglish
            ? $"Created {created.Count} Types."
            : $"Đã tạo {created.Count} Type.";
        if (skipped.Count > 0)
            summary += window.IsEnglish
                ? $"\nSkipped {skipped.Count} existing names."
                : $"\nBỏ qua {skipped.Count} tên đã tồn tại.";
        TaskDialog.Show("CopyFam", summary);
        return Result.Succeeded;
    }

    private static ElementId? GetSelectedTypeId(UIDocument uiDoc)
    {
        Element? selected = uiDoc.Selection.GetElementIds().Select(uiDoc.Document.GetElement).FirstOrDefault();
        return selected switch
        {
            FamilyInstance instance => instance.Symbol.Id,
            FamilySymbol symbol => symbol.Id,
            _ => null
        };
    }
}
