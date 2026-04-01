using Autodesk.Revit.UI;
using RevitLogic.Features.Button1;

namespace RevitLogic.EntryPoints
{
    public static class Button1Entry
    {
        public static string Run(UIApplication uiapp)
        {
            return new Button1Service().Execute(uiapp);
        }
    }
}
