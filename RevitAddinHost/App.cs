// RevitAddinHost\App.cs
using System.Globalization;
using System.Reflection;
using System.Text;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;

namespace RevitAddinHost
{
    public class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            const string tabName = "RevitAddinHotReloadDemo_v0.1.1";
            const string panelName = "HotReloadDemo_v0.1.1";

            try { application.CreateRibbonTab(tabName); } catch { }

            RibbonPanel panel = application.CreateRibbonPanel(tabName, panelName);

            string assemblyPath = Assembly.GetExecutingAssembly().Location;

            var b1 = new PushButtonData(
                "Btn1", WrapText("Button 1"), assemblyPath,
                "RevitAddinHost.Commands.Button1"
            );

            panel.AddItem(b1);
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        private static string WrapText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            return text.Replace(" ", "\n");
        }
    }
}