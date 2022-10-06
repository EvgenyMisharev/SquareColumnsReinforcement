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

            //Список типов для выбора арматуры
            List<RebarBarType> rebarBarTypesList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .OrderBy(rbt => rbt.Name, new AlphanumComparatorFastString())
                .ToList();

            //Список типов защитных слоев
            List<RebarCoverType> rebarCoverTypesList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarCoverType))
                .Cast<RebarCoverType>()
                .OrderBy(rct => rct.Name, new AlphanumComparatorFastString())
                .ToList();

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

            List<Family> rebarConnectionsList = new FilteredElementCollector(doc)
                .OfClass(typeof(Family))
                .Cast<Family>()
                .Where(f => f.FamilyCategoryId.IntegerValue == (int)BuiltInCategory.OST_Rebar)
                .Where(f => f.GetFamilySymbolIds().Count != 0)
                .Where(f => doc.GetElement(f.GetFamilySymbolIds().First()).get_Parameter(BuiltInParameter.ALL_MODEL_MODEL) != null)
                .Where(f => doc.GetElement(f.GetFamilySymbolIds().First()).get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).AsString() == "Соединение арматуры")
                .OrderBy(f => f.Name, new AlphanumComparatorFastString())
                .ToList();

            SquareColumnsReinforcementWPF squareColumnsReinforcementWPF = new SquareColumnsReinforcementWPF(rebarBarTypesList
                , rebarCoverTypesList
                , rebarShapeList
                , rebarHookTypeList
                , rebarConnectionsList);
            squareColumnsReinforcementWPF.ShowDialog();
            if (squareColumnsReinforcementWPF.DialogResult != true)
            {
                return Result.Cancelled;
            }

            return Result.Succeeded;
        }
    }
}
