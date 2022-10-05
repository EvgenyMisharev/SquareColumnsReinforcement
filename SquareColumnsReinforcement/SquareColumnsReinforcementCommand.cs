using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquareColumnsReinforcement
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class SquareColumnsReinforcementCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Получение текущего документа
            Document doc = commandData.Application.ActiveUIDocument.Document;

            //Формы для формы
            List<RebarShape> rebarShapeList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarShape))
                .Cast<RebarShape>()
                .OrderBy(rs => rs.Name, new AlphanumComparatorFastString())
                .ToList();
            List<RebarHookType> rebarHookTypeList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarHookType))
                .OrderBy(rht => rht.Name, new AlphanumComparatorFastString())
                .Cast<RebarHookType>()
                .ToList();

            SquareColumnsReinforcementWPF squareColumnsReinforcementWPF = new SquareColumnsReinforcementWPF(rebarShapeList, rebarHookTypeList);
            squareColumnsReinforcementWPF.ShowDialog();
            if (squareColumnsReinforcementWPF.DialogResult != true)
            {
                return Result.Cancelled;
            }

            return Result.Succeeded;
        }
    }
}
