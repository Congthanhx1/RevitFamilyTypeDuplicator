using Autodesk.Revit.UI;
using System.Reflection;

namespace CopyFam;

public sealed class App : IExternalApplication
{
    public Result OnStartup(UIControlledApplication application)
    {
        const string tab = "CopyFam";
        try { application.CreateRibbonTab(tab); } catch { }
        RibbonPanel panel = application.CreateRibbonPanel(tab, "Family Type");
        string assembly = Assembly.GetExecutingAssembly().Location;
        var data = new PushButtonData("CopyFam.Duplicate", "Copy &&\nđặt tên", assembly, typeof(DuplicateTypesCommand).FullName);
        var button = (PushButton)panel.AddItem(data);
        button.ToolTip = "Nhân bản Family Type và đặt tên hàng loạt.";
        return Result.Succeeded;
    }

    public Result OnShutdown(UIControlledApplication application) => Result.Succeeded;
}
