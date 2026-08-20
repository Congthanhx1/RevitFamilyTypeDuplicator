using Autodesk.Revit.UI;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace CopyFam;

public sealed class App : IExternalApplication
{
    public Result OnStartup(UIControlledApplication application)
    {
        RibbonPanel panel = application.CreateRibbonPanel("CopyFam");
        string assembly = Assembly.GetExecutingAssembly().Location;
        var data = new PushButtonData("CopyFam.Duplicate", "Copy &&\nrename", assembly, typeof(DuplicateTypesCommand).FullName);
        var button = (PushButton)panel.AddItem(data);
        button.ToolTip = "Duplicate and batch-rename Family Types. Supports English and Vietnamese.";
        button.Image = LoadImage("Resources/CopyFam-16.png");
        button.LargeImage = LoadImage("Resources/CopyFam-32.png");
        button.SetContextualHelp(new ContextualHelp(
            ContextualHelpType.Url,
            "https://github.com/Congthanhx1/RevitFamilyTypeDuplicator#usage"));
        return Result.Succeeded;
    }

    public Result OnShutdown(UIControlledApplication application) => Result.Succeeded;

    private static BitmapImage LoadImage(string resourcePath) => new(
        new Uri($"pack://application:,,,/CopyFam;component/{resourcePath}", UriKind.Absolute));
}
