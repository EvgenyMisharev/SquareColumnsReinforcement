using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SquareColumnsReinforcement
{
    public class RebarShapeCollector
    {
        public RebarShape Shape01 { get; }
        public RebarShape Shape11 { get; }
        public RebarShape Shape26 { get; }
        public RebarShape Shape51 { get; }
        public RebarShape Shape52 { get; }
        public RebarHookType SeismicRebarHook135 { get; }

        private Document doc;

        public bool CollectionResultOk = true;
        public RebarShapeCollector(Document document)
        {
            doc = document;
            Shape01 = GetShape01();
            Shape11 = GetShape11();
            Shape26 = GetShape26();
            Shape51 = GetShape51();
            SeismicRebarHook135 = GetSeismicRebar135HookType();

            if (Shape01 == null || Shape11 == null || Shape26 == null || Shape51 == null || SeismicRebarHook135 == null)
            {
                CollectionResultOk = false;
            }
        }
        private RebarShape GetShape01()
        {
            List<RebarShape> rebarShapeListFor = new FilteredElementCollector(doc)
                    .OfClass(typeof(RebarShape))
                    .Where(rs => rs.Name.ToString() == "01")
                    .Cast<RebarShape>()
                    .ToList();
            if (rebarShapeListFor.Count == 0)
            {
                TaskDialog.Show("Revit", "Форма 01 не найдена!");
                return null;
            }
            else
            {
                return rebarShapeListFor.First();
            }
        }
        private RebarShape GetShape11()
        {
            List<RebarShape> rebarShapeListFor = new FilteredElementCollector(doc)
                    .OfClass(typeof(RebarShape))
                    .Where(rs => rs.Name.ToString() == "11")
                    .Cast<RebarShape>()
                    .ToList();
            if (rebarShapeListFor.Count == 0)
            {
                TaskDialog.Show("Revit","Форма 11 не найдена!");
                return null;
            }
            else
            {
                return rebarShapeListFor.First();
            }
        }
        private RebarShape GetShape26()
        {
            List<RebarShape> rebarShapeListFor = new FilteredElementCollector(doc)
                    .OfClass(typeof(RebarShape))
                    .Where(rs => rs.Name.ToString() == "26")
                    .Cast<RebarShape>()
                    .ToList();
            if (rebarShapeListFor.Count == 0)
            {
                TaskDialog.Show("Revit", "Форма 26 не найдена!");
                return null;
            }
            else
            {
                return rebarShapeListFor.First();
            }
        }
        private RebarShape GetShape51()
        {
            List<RebarShape> rebarShapeListFor = new FilteredElementCollector(doc)
                    .OfClass(typeof(RebarShape))
                    .Where(rs => rs.Name.ToString() == "51")
                    .Cast<RebarShape>()
                    .ToList();
            if (rebarShapeListFor.Count == 0)
            {
                TaskDialog.Show("Revit", "Форма 51 не найдена!");
                return null;
            }
            else
            {
                if (rebarShapeListFor.First().get_Parameter(BuiltInParameter.REBAR_SHAPE_HOOK_STYLE).AsInteger() != 1)
                {
                    TaskDialog.Show("Revit", "Стиль формы 51 не \"Хомут / Стяжка\". Измените свойство формы перед запуском плагина!");
                    return null;
                }
                return rebarShapeListFor.First();
            }
        }

        private RebarHookType GetSeismicRebar135HookType()
        {
            List<RebarHookType> rebarHookTypeList = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarHookType))
                .Where(rs => rs.Name.ToString() == "Сейсмическая поперечная арматура - 135 градусов")
                .Cast<RebarHookType>()
                .ToList();

            if (rebarHookTypeList.Count == 0)
            {
                TaskDialog.Show("Revit", "Сейсмическая поперечная арматура - 135 градусов не найдена!");
                return null;
            }
            else
            {
                return rebarHookTypeList.First();
            }
        }


        public FamilySymbol GetTubWeldingFamilySymbol()
        {
            List<Family> familiesListForTubWelding = new FilteredElementCollector(doc)
                .OfClass(typeof(Family))
                .Cast<Family>()
                .Where(f => f.Name == "CIT_04_ВаннаДляСварки")
                .ToList();
            if (familiesListForTubWelding.Count == 0)
            {
                return null;
            }
            Family tubWeldingFamilie = familiesListForTubWelding.First();

            List<ElementId> symbolsTubWeldingIds = tubWeldingFamilie.GetFamilySymbolIds().ToList();
            ElementId firstSymbolTubWeldingId = symbolsTubWeldingIds.First();

            FamilySymbol tubWeldingSymbol = doc.GetElement(firstSymbolTubWeldingId) as FamilySymbol;
            if (tubWeldingSymbol == null)
            {
                return null;
            }
            return tubWeldingSymbol;
        }
    }
}

