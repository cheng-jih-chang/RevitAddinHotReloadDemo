using Autodesk.Revit.UI;

namespace RevitLogic.Features.Button1
{
    public class Button1Service
    {
        public string Execute(UIApplication uiapp)
        {
            if (uiapp == null) return "uiapp is null";
            // TODO: 套管生成主邏輯
            return "BUTTON1 confirmed successful execution";
        }
    }
}
