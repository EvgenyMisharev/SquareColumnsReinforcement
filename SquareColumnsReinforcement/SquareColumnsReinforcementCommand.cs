using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
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
            //Получение доступа к Selection
            Selection sel = commandData.Application.ActiveUIDocument.Selection;

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

            List<FamilyInstance> columnsList = GetColumnsFromCurrentSelection(doc, sel);
            if (columnsList.Count == 0)
            {
                ColumnSelectionFilter columnSelFilter = new ColumnSelectionFilter();
                IList<Reference> selColumnsReferenceList = null;
                try
                {
                    selColumnsReferenceList = sel.PickObjects(ObjectType.Element, columnSelFilter, "Выберите несущие колонны!");
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                {
                    return Result.Cancelled;
                }

                foreach (Reference columnRef in selColumnsReferenceList)
                {
                    columnsList.Add(doc.GetElement(columnRef) as FamilyInstance);
                }
            }

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

            switch (squareColumnsReinforcementWPF.SelectedReinforcementTypeButtonName)
            {
                case "button_Type1":
                    SquareColumnsReinforcementT1 squareColumnsReinforcementT1 = new SquareColumnsReinforcementT1(doc
                        , columnsList
                        , squareColumnsReinforcementWPF);
                    break;

                case "button_Type2":
                    SquareColumnsReinforcementT2 squareColumnsReinforcementT2 = new SquareColumnsReinforcementT2(doc
                        , columnsList
                        , squareColumnsReinforcementWPF);
                    break;

                case "button_Type3":
                    SquareColumnsReinforcementT3 squareColumnsReinforcementT3 = new SquareColumnsReinforcementT3(doc
                        , columnsList
                        , squareColumnsReinforcementWPF);
                    break;

                case "button_Type4":
                    SquareColumnsReinforcementT4 squareColumnsReinforcementT4 = new SquareColumnsReinforcementT4(doc
                        , columnsList
                        , squareColumnsReinforcementWPF);
                    break;

                case "button_Type5":
                    SquareColumnsReinforcementT5 squareColumnsReinforcementT5 = new SquareColumnsReinforcementT5(doc
                        , columnsList
                        , squareColumnsReinforcementWPF);
                    break;

                case "button_Type6":
                    SquareColumnsReinforcementT6 squareColumnsReinforcementT6 = new SquareColumnsReinforcementT6(doc
                        , columnsList
                        , squareColumnsReinforcementWPF);
                    break;
            }

            return Result.Succeeded;
        }

        private static List<FamilyInstance> GetColumnsFromCurrentSelection(Document doc, Selection sel)
        {
            ICollection<ElementId> selectedIds = sel.GetElementIds();
            List<FamilyInstance> tempColumnsList = new List<FamilyInstance>();
            foreach (ElementId columnId in selectedIds)
            {
                if (doc.GetElement(columnId) is FamilyInstance
                    && null != doc.GetElement(columnId).Category
                    && doc.GetElement(columnId).Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_StructuralColumns))
                {
                    tempColumnsList.Add(doc.GetElement(columnId) as FamilyInstance);
                }
            }
            return tempColumnsList;
        }
    }
}
