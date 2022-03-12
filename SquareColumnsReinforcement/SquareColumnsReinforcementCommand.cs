using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SquareColumnsReinforcement
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class SquareColumnsReinforcementCommand : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            Selection sel = commandData.Application.ActiveUIDocument.Selection;

            List<FamilyInstance> columnsList = new List<FamilyInstance>();
            columnsList = GetColumnsFromCurrentSelection(doc, sel);
            if (columnsList.Count == 0)
            {
                columnsList = GetColumnsFromSelection(doc, sel);
            }

            SquareColumnsReinforcementMainForm squareColumnsReinforcementMainForm = new SquareColumnsReinforcementMainForm(doc);
            squareColumnsReinforcementMainForm.ShowDialog();
            if (squareColumnsReinforcementMainForm.DialogResult != System.Windows.Forms.DialogResult.OK)
            {
                return Result.Cancelled;
            }
            SquareColumnsReinforcementMainWPF squareColumnsReinforcementMainWPF = new SquareColumnsReinforcementMainWPF();
            squareColumnsReinforcementMainWPF.ShowDialog();
            if (squareColumnsReinforcementMainWPF.DialogResult != true)
            {
                return Result.Cancelled;
            }
            switch (squareColumnsReinforcementMainForm.CheckedColumnReinforcementTypeName)
            {
                case "radioButton_T1":
                    SquareColumnsReinforcementCommandT1 squareColumnsReinforcementCommandT1 = new SquareColumnsReinforcementCommandT1(columnsList, squareColumnsReinforcementMainForm);
                    squareColumnsReinforcementCommandT1.Execute(commandData.Application);
                    break;
                case "radioButton_T2":
                    SquareColumnsReinforcementCommandT2 squareColumnsReinforcementCommandT2 = new SquareColumnsReinforcementCommandT2(columnsList, squareColumnsReinforcementMainForm);
                    squareColumnsReinforcementCommandT2.Execute(commandData.Application);
                    break;
                case "radioButton_T3":
                    SquareColumnsReinforcementCommandT3 squareColumnsReinforcementCommandT3 = new SquareColumnsReinforcementCommandT3(columnsList, squareColumnsReinforcementMainForm);
                    squareColumnsReinforcementCommandT3.Execute(commandData.Application);
                    break;
                case "radioButton_T4":
                    SquareColumnsReinforcementCommandT4 squareColumnsReinforcementCommandT4 = new SquareColumnsReinforcementCommandT4(columnsList, squareColumnsReinforcementMainForm);
                    squareColumnsReinforcementCommandT4.Execute(commandData.Application);
                    break;
                case "radioButton_T5":
                    SquareColumnsReinforcementCommandT5 squareColumnsReinforcementCommandT5 = new SquareColumnsReinforcementCommandT5(columnsList, squareColumnsReinforcementMainForm);
                    squareColumnsReinforcementCommandT5.Execute(commandData.Application);
                    break;
                case "radioButton_T6":
                    SquareColumnsReinforcementCommandT6 squareColumnsReinforcementCommandT6 = new SquareColumnsReinforcementCommandT6(columnsList, squareColumnsReinforcementMainForm);
                    squareColumnsReinforcementCommandT6.Execute(commandData.Application);
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
                if (doc.GetElement(columnId) is FamilyInstance && null != doc.GetElement(columnId).Category && doc.GetElement(columnId).Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_StructuralColumns))
                {
                    tempColumnsList.Add(doc.GetElement(columnId) as FamilyInstance);
                }
            }
            return tempColumnsList;
        }
        public static List<FamilyInstance> GetColumnsFromSelection(Document doc, Selection sel)
        {
            ColumnSelectionFilter columnSelFilter = new ColumnSelectionFilter();
            IList<Reference> selColumns = sel.PickObjects(ObjectType.Element, columnSelFilter, "Выберите колонны!");
            List<FamilyInstance> tempColumnsList = new List<FamilyInstance>();
            foreach (Reference columnRef in selColumns)
            {
                tempColumnsList.Add(doc.GetElement(columnRef) as FamilyInstance);
            }
            return tempColumnsList;
        }
        public static void CreateRebarGroup(Document doc
            , FamilyInstance column
            , ICollection<ElementId> rebarIdCollection
            , ColumnProperties columnProperties)
        {
            List<Group> projectGroupList = new FilteredElementCollector(doc).OfClass(typeof(Group)).Cast<Group>().ToList();
            if (projectGroupList.Any(g => g.GroupType.Name == column.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString()))
            {
                TaskDialog.Show("Revit", $"Группа с имененм \"{ column.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString()}\" уже существует!\nБыли созданы отдельные стержни колонны без группировки!");
            }
            else
            {
                Group newRebarGroup = doc.Create.NewGroup(rebarIdCollection);
                if (column.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString() == null || column.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString() == "")
                {
                    TaskDialog.Show("Revit", $"У колонны отсутствует марка!\nИмя группы по умолчанию - \"{newRebarGroup.GroupType.Name}\"");
                }
                else
                {
                    newRebarGroup.GroupType.Name = column.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString();
                }
                if (columnProperties.ColumnRotation != 0)
                {
                    ElementTransformUtils.RotateElement(doc, newRebarGroup.Id, columnProperties.ColumnRotationAxis, columnProperties.ColumnRotation);
                }
            }
        }
        public static void CreateEncircleStirrup(Document doc
            , RebarShapeCollector rebarShapeCollector
            , FamilyInstance column
            , ColumnProperties columnProperties
            , SquareColumnsReinforcementMainForm squareColumnsReinforcementMainForm
            , ICollection<ElementId> rebarIdCollection)
        {
            //Нормаль для построения хомута
            XYZ stirrupNormal = new XYZ(0, 0, 1);

            //Точки для построения кривых стержня хомута
            XYZ rebarStirrup_p1 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X
                - columnProperties.ColumnSectionHeight / 2
                + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                - squareColumnsReinforcementMainForm.MainStirrupRebarTapeDiam, 6)
                , Math.Round(columnProperties.ColumnOrigin.Y
                + columnProperties.ColumnSectionWidth / 2
                - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                + squareColumnsReinforcementMainForm.MainStirrupRebarTapeDiam / 2, 6)
                , Math.Round(columnProperties.ColumnOrigin.Z
                + (squareColumnsReinforcementMainForm.FirstBottomStirrupOffset / 304.8), 6));

            XYZ rebarStirrup_p2 = new XYZ(Math.Round(rebarStirrup_p1.X
                + columnProperties.ColumnSectionHeight
                - squareColumnsReinforcementMainForm.MainRebarCoverLayer * 2
                + squareColumnsReinforcementMainForm.MainStirrupRebarTapeDiam, 6)
                , Math.Round(rebarStirrup_p1.Y, 6)
                , Math.Round(rebarStirrup_p1.Z, 6));

            XYZ rebarStirrup_p3 = new XYZ(Math.Round(rebarStirrup_p2.X, 6)
                , Math.Round(rebarStirrup_p2.Y
                - columnProperties.ColumnSectionWidth
                + squareColumnsReinforcementMainForm.MainRebarCoverLayer * 2
                - squareColumnsReinforcementMainForm.MainStirrupRebarTapeDiam
                - squareColumnsReinforcementMainForm.MainStirrupRebarTapeDiam / 2, 6)
                , Math.Round(rebarStirrup_p2.Z, 6));

            XYZ rebarStirrup_p4 = new XYZ(Math.Round(rebarStirrup_p3.X
                - columnProperties.ColumnSectionHeight
                + squareColumnsReinforcementMainForm.MainRebarCoverLayer * 2
                - squareColumnsReinforcementMainForm.MainStirrupRebarTapeDiam, 6)
                , Math.Round(rebarStirrup_p3.Y, 6)
                , Math.Round(rebarStirrup_p3.Z, 6));

            //Кривые хомута
            List<Curve> stirrupCurves = new List<Curve>();

            Curve stirrupLine1 = Line.CreateBound(rebarStirrup_p1, rebarStirrup_p2) as Curve;
            stirrupCurves.Add(stirrupLine1);
            Curve stirrupLine2 = Line.CreateBound(rebarStirrup_p2, rebarStirrup_p3) as Curve;
            stirrupCurves.Add(stirrupLine2);
            Curve stirrupLine3 = Line.CreateBound(rebarStirrup_p3, rebarStirrup_p4) as Curve;
            stirrupCurves.Add(stirrupLine3);
            Curve stirrupLine4 = Line.CreateBound(rebarStirrup_p4, rebarStirrup_p1) as Curve;
            stirrupCurves.Add(stirrupLine4);

            //Построение нижнего хомута
            Rebar downStirrup = Rebar.CreateFromCurvesAndShape(doc
                , rebarShapeCollector.Shape51
                , squareColumnsReinforcementMainForm.MainStirrupRebarTape
                , rebarShapeCollector.SeismicRebarHook135
                , rebarShapeCollector.SeismicRebarHook135
                , column
                , stirrupNormal
                , stirrupCurves
                , RebarHookOrientation.Right
                , RebarHookOrientation.Right);

            downStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
            downStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS)
                .Set((int)(squareColumnsReinforcementMainForm.FrequentBottomStirrupPlacementHeight
                / squareColumnsReinforcementMainForm.FrequentBottomStirrupStep) + 1);
            downStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set((squareColumnsReinforcementMainForm.FrequentBottomStirrupStep / 304.8));
            rebarIdCollection.Add(downStirrup.Id);

            double heightForMiddleStirrup = columnProperties.ColumnLength
                - ((squareColumnsReinforcementMainForm.FrequentBottomStirrupPlacementHeight
                + squareColumnsReinforcementMainForm.FrequentTopStirrupPlacementHeight
                + squareColumnsReinforcementMainForm.FirstBottomStirrupOffset) / 304.8);

            int middleStirrupQuantityOfBars = (int)(heightForMiddleStirrup / (squareColumnsReinforcementMainForm.StandardStirrupStep / 304.8));
            double remainingHeight = heightForMiddleStirrup - (middleStirrupQuantityOfBars * (squareColumnsReinforcementMainForm.StandardStirrupStep / 304.8));

            //Копирование хомута для стандартного шага
            XYZ middleStirrupInstallationPoint = new XYZ(0, 0, (squareColumnsReinforcementMainForm.FrequentBottomStirrupPlacementHeight 
                + squareColumnsReinforcementMainForm.StandardStirrupStep) / 304.8);
            Rebar middleStirrup = doc.GetElement((ElementTransformUtils.CopyElement(doc, downStirrup.Id, middleStirrupInstallationPoint) as List<ElementId>).First()) as Rebar;
            middleStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
            middleStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(middleStirrupQuantityOfBars - 1);
            middleStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set((squareColumnsReinforcementMainForm.StandardStirrupStep / 304.8));
            rebarIdCollection.Add(middleStirrup.Id);

            //Копирование хомута вверх
            if (Math.Round(remainingHeight, 6) <= Math.Round((squareColumnsReinforcementMainForm.FrequentTopStirrupStep / 304.8), 6) & Math.Round(remainingHeight, 6) != 0)
            {
                XYZ topStirrupInstallationPoint = new XYZ(0, 0, columnProperties.ColumnLength - (squareColumnsReinforcementMainForm.FirstBottomStirrupOffset / 304.8) - remainingHeight);
                Rebar topStirrup = doc.GetElement((ElementTransformUtils.CopyElement(doc, downStirrup.Id, topStirrupInstallationPoint) as List<ElementId>).First()) as Rebar;
                topStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(0);
                topStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                topStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS)
                    .Set((int)(squareColumnsReinforcementMainForm.FrequentTopStirrupPlacementHeight
                    / squareColumnsReinforcementMainForm.FrequentTopStirrupStep) + 1);
                topStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set((squareColumnsReinforcementMainForm.FrequentTopStirrupStep / 304.8));
                rebarIdCollection.Add(topStirrup.Id);
            }
            else if (Math.Round(remainingHeight, 6) > Math.Round((squareColumnsReinforcementMainForm.FrequentTopStirrupStep / 304.8), 6) & Math.Round(remainingHeight, 6) != 0)
            {
                int topStirrupQuantityOfAdditionalBars = (int)(remainingHeight / (squareColumnsReinforcementMainForm.FrequentTopStirrupStep / 304.8));

                XYZ topStirrupInstallationPoint = new XYZ(0, 0, columnProperties.ColumnLength - (squareColumnsReinforcementMainForm.FirstBottomStirrupOffset / 304.8) - (remainingHeight - topStirrupQuantityOfAdditionalBars * (squareColumnsReinforcementMainForm.FrequentTopStirrupStep / 304.8)));
                Rebar topStirrup = doc.GetElement((ElementTransformUtils.CopyElement(doc, downStirrup.Id, topStirrupInstallationPoint) as List<ElementId>).First()) as Rebar;
                topStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(0);
                topStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                topStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS)
                    .Set((int)(squareColumnsReinforcementMainForm.FrequentTopStirrupPlacementHeight
                    / squareColumnsReinforcementMainForm.FrequentTopStirrupStep) + 1 + topStirrupQuantityOfAdditionalBars);
                topStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set((squareColumnsReinforcementMainForm.FrequentTopStirrupStep / 304.8));
                rebarIdCollection.Add(topStirrup.Id);
            }
            else
            {
                XYZ topStirrupInstallationPoint = new XYZ(0, 0, columnProperties.ColumnLength - (squareColumnsReinforcementMainForm.FirstBottomStirrupOffset / 304.8) - (50 / 304.8));
                Rebar topStirrup = doc.GetElement((ElementTransformUtils.CopyElement(doc, downStirrup.Id, topStirrupInstallationPoint) as List<ElementId>).First()) as Rebar;
                topStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(0);
                topStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                topStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS)
                    .Set((int)(squareColumnsReinforcementMainForm.FrequentTopStirrupPlacementHeight
                    / squareColumnsReinforcementMainForm.FrequentTopStirrupStep) + 1);
                topStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set((squareColumnsReinforcementMainForm.FrequentTopStirrupStep / 304.8));
                rebarIdCollection.Add(topStirrup.Id);
            }
        }
        public static void CreateRhombicStirrup(Document doc
            , RebarShapeCollector rebarShapeCollector
            , FamilyInstance column
            , ColumnProperties columnProperties
            , SquareColumnsReinforcementMainForm squareColumnsReinforcementMainForm
            , ICollection<ElementId> rebarIdCollection)
        {
            //Нормаль для построения хомута
            XYZ stirrupNormal = new XYZ(0, 0, 1);

            //Точки для построения кривых стержня нижнего ромбического хомута
            double stirrupDeltaXDeltaY = (squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2 
                + squareColumnsReinforcementMainForm.AdditionalStirrupRebarTapeDiam /2) 
                / Math.Sin(0.785398);
            double stirrupDeltaXDeltaY_2 = (squareColumnsReinforcementMainForm.AdditionalStirrupRebarTapeDiam / 4)
                / Math.Sin(0.785398);

            XYZ additionalRebarDownStirrup_p1 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X 
                - columnProperties.ColumnSectionWidth / 2 
                + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                + (squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam /2)
                - stirrupDeltaXDeltaY
                - stirrupDeltaXDeltaY_2, 6)
                , Math.Round(columnProperties.ColumnOrigin.Y
                - stirrupDeltaXDeltaY_2, 6)
                , Math.Round(columnProperties.ColumnOrigin.Z 
                + (squareColumnsReinforcementMainForm.FirstBottomStirrupOffset / 304.8)
                + squareColumnsReinforcementMainForm.MainStirrupRebarTapeDiam / 2 
                + squareColumnsReinforcementMainForm.AdditionalStirrupRebarTapeDiam / 2, 6));

            XYZ additionalRebarDownStirrup_p2 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X
                - stirrupDeltaXDeltaY_2, 6)
                , Math.Round(columnProperties.ColumnOrigin.Y 
                + columnProperties.ColumnSectionHeight / 2 
                - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                - (squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2)
                + stirrupDeltaXDeltaY
                - stirrupDeltaXDeltaY_2, 6)
                , Math.Round(additionalRebarDownStirrup_p1.Z, 6));

            XYZ additionalRebarDownStirrup_p3 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X 
                + columnProperties.ColumnSectionWidth / 2 
                - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                - (squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2)
                + stirrupDeltaXDeltaY, 6)
                , Math.Round(columnProperties.ColumnOrigin.Y
                - stirrupDeltaXDeltaY_2 * 2, 6)
                , Math.Round(additionalRebarDownStirrup_p2.Z, 6));

            XYZ additionalRebarDownStirrup_p4 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                , Math.Round(columnProperties.ColumnOrigin.Y 
                - columnProperties.ColumnSectionHeight / 2 
                + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                + (squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2)
                - stirrupDeltaXDeltaY
                - stirrupDeltaXDeltaY_2 * 2, 6)
                , Math.Round(additionalRebarDownStirrup_p3.Z, 6));

            //Кривые нижнего ромбического хомута
            List<Curve> additionalDownStirrupCurves = new List<Curve>();

            Curve downStirrupLine1 = Line.CreateBound(additionalRebarDownStirrup_p1, additionalRebarDownStirrup_p2) as Curve;
            additionalDownStirrupCurves.Add(downStirrupLine1);
            Curve downStirrupLine2 = Line.CreateBound(additionalRebarDownStirrup_p2, additionalRebarDownStirrup_p3) as Curve;
            additionalDownStirrupCurves.Add(downStirrupLine2);
            Curve downStirrupLine3 = Line.CreateBound(additionalRebarDownStirrup_p3, additionalRebarDownStirrup_p4) as Curve;
            additionalDownStirrupCurves.Add(downStirrupLine3);
            Curve downStirrupLine4 = Line.CreateBound(additionalRebarDownStirrup_p4, additionalRebarDownStirrup_p1) as Curve;
            additionalDownStirrupCurves.Add(downStirrupLine4);

            //Построение нижнего ромбического хомута
            Rebar downStirrup = Rebar.CreateFromCurvesAndShape(doc
                , rebarShapeCollector.Shape51
                , squareColumnsReinforcementMainForm.AdditionalStirrupRebarTape
                , rebarShapeCollector.SeismicRebarHook135
                , rebarShapeCollector.SeismicRebarHook135
                , column
                , stirrupNormal
                , additionalDownStirrupCurves
                , RebarHookOrientation.Right
                , RebarHookOrientation.Right);

            downStirrup.GetShapeDrivenAccessor()
                .SetLayoutAsNumberWithSpacing(((int)(squareColumnsReinforcementMainForm.FrequentBottomStirrupPlacementHeight
                / squareColumnsReinforcementMainForm.FrequentBottomStirrupStep) + 1)
                , (squareColumnsReinforcementMainForm.FrequentBottomStirrupStep / 304.8), true, true, true);
            rebarIdCollection.Add(downStirrup.Id);

            double heightForMiddleStirrup = columnProperties.ColumnLength
                - ((squareColumnsReinforcementMainForm.FrequentBottomStirrupPlacementHeight
                + squareColumnsReinforcementMainForm.FrequentTopStirrupPlacementHeight
                + squareColumnsReinforcementMainForm.FirstBottomStirrupOffset) / 304.8);

            int middleStirrupQuantityOfBars = (int)(heightForMiddleStirrup / (squareColumnsReinforcementMainForm.StandardStirrupStep / 304.8));
            double remainingHeight = heightForMiddleStirrup - (middleStirrupQuantityOfBars * (squareColumnsReinforcementMainForm.StandardStirrupStep / 304.8));

            //Точки для построения кривых стержня среднего ромбического хомута
            XYZ additionalRebarMiddleStirrup_p1 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X
                - columnProperties.ColumnSectionWidth / 2
                + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                + (squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2)
                - stirrupDeltaXDeltaY
                - stirrupDeltaXDeltaY_2, 6)
                , Math.Round(columnProperties.ColumnOrigin.Y
                - stirrupDeltaXDeltaY_2, 6)
                , Math.Round(columnProperties.ColumnOrigin.Z
                + (squareColumnsReinforcementMainForm.FirstBottomStirrupOffset / 304.8)
                + squareColumnsReinforcementMainForm.MainStirrupRebarTapeDiam / 2
                + squareColumnsReinforcementMainForm.AdditionalStirrupRebarTapeDiam / 2
                + ((squareColumnsReinforcementMainForm.FrequentBottomStirrupPlacementHeight
                + squareColumnsReinforcementMainForm.StandardStirrupStep) / 304.8), 6));

            XYZ additionalRebarMiddleStirrup_p2 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X
                - stirrupDeltaXDeltaY_2, 6)
                , Math.Round(columnProperties.ColumnOrigin.Y
                + columnProperties.ColumnSectionHeight / 2
                - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                - (squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2)
                + stirrupDeltaXDeltaY
                - stirrupDeltaXDeltaY_2, 6)
                , Math.Round(additionalRebarMiddleStirrup_p1.Z, 6));

            XYZ additionalRebarMiddleStirrup_p3 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X
                + columnProperties.ColumnSectionWidth / 2
                - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                - (squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2)
                + stirrupDeltaXDeltaY, 6)
                , Math.Round(columnProperties.ColumnOrigin.Y
                - stirrupDeltaXDeltaY_2 * 2, 6)
                , Math.Round(additionalRebarMiddleStirrup_p2.Z, 6));

            XYZ additionalRebarMiddleStirrup_p4 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                , Math.Round(columnProperties.ColumnOrigin.Y
                - columnProperties.ColumnSectionHeight / 2
                + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                + (squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2)
                - stirrupDeltaXDeltaY
                - stirrupDeltaXDeltaY_2 * 2, 6)
                , Math.Round(additionalRebarMiddleStirrup_p3.Z, 6));

            //Кривые среднего ромбического хомута
            List<Curve> additionalMiddleStirrupCurves = new List<Curve>();

            Curve middleStirrupLine1 = Line.CreateBound(additionalRebarMiddleStirrup_p1, additionalRebarMiddleStirrup_p2) as Curve;
            additionalMiddleStirrupCurves.Add(middleStirrupLine1);
            Curve middleStirrupLine2 = Line.CreateBound(additionalRebarMiddleStirrup_p2, additionalRebarMiddleStirrup_p3) as Curve;
            additionalMiddleStirrupCurves.Add(middleStirrupLine2);
            Curve middleStirrupLine3 = Line.CreateBound(additionalRebarMiddleStirrup_p3, additionalRebarMiddleStirrup_p4) as Curve;
            additionalMiddleStirrupCurves.Add(middleStirrupLine3);
            Curve middleStirrupLine4 = Line.CreateBound(additionalRebarMiddleStirrup_p4, additionalRebarMiddleStirrup_p1) as Curve;
            additionalMiddleStirrupCurves.Add(middleStirrupLine4);

            //Построение среднего ромбического хомута
            Rebar middleStirrup = Rebar.CreateFromCurvesAndShape(doc
                , rebarShapeCollector.Shape51
                , squareColumnsReinforcementMainForm.AdditionalStirrupRebarTape
                , rebarShapeCollector.SeismicRebarHook135
                , rebarShapeCollector.SeismicRebarHook135
                , column
                , stirrupNormal
                , additionalMiddleStirrupCurves
                , RebarHookOrientation.Right
                , RebarHookOrientation.Right);

            middleStirrup.GetShapeDrivenAccessor().SetLayoutAsNumberWithSpacing(middleStirrupQuantityOfBars - 1
                , (squareColumnsReinforcementMainForm.StandardStirrupStep / 304.8), true, true, true);
            rebarIdCollection.Add(middleStirrup.Id);


            if (Math.Round(remainingHeight, 6) <= Math.Round((squareColumnsReinforcementMainForm.FrequentTopStirrupStep / 304.8), 6) & Math.Round(remainingHeight, 6) != 0)
            {
                //Точки для построения кривых верхнего среднего ромбического хомута
                XYZ additionalRebarTopStirrup_p1 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X
                    - columnProperties.ColumnSectionWidth / 2
                    + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                    + (squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2)
                    - stirrupDeltaXDeltaY
                    - stirrupDeltaXDeltaY_2, 6)
                    , Math.Round(columnProperties.ColumnOrigin.Y
                    - stirrupDeltaXDeltaY_2, 6)
                    , Math.Round(columnProperties.ColumnOrigin.Z
                    + (squareColumnsReinforcementMainForm.FirstBottomStirrupOffset / 304.8)
                    + (columnProperties.ColumnLength 
                    - (squareColumnsReinforcementMainForm.FirstBottomStirrupOffset / 304.8) 
                    - remainingHeight)
                    + squareColumnsReinforcementMainForm.MainStirrupRebarTapeDiam / 2
                    + squareColumnsReinforcementMainForm.AdditionalStirrupRebarTapeDiam / 2, 6));

                XYZ additionalRebarTopStirrup_p2 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X
                    - stirrupDeltaXDeltaY_2, 6)
                    , Math.Round(columnProperties.ColumnOrigin.Y
                    + columnProperties.ColumnSectionHeight / 2
                    - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                    - (squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2)
                    + stirrupDeltaXDeltaY
                    - stirrupDeltaXDeltaY_2, 6)
                    , Math.Round(additionalRebarTopStirrup_p1.Z, 6));

                XYZ additionalRebarTopStirrup_p3 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X
                    + columnProperties.ColumnSectionWidth / 2
                    - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                    - (squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2)
                    + stirrupDeltaXDeltaY, 6)
                    , Math.Round(columnProperties.ColumnOrigin.Y
                    - stirrupDeltaXDeltaY_2 * 2, 6)
                    , Math.Round(additionalRebarTopStirrup_p2.Z, 6));

                XYZ additionalRebarTopStirrup_p4 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                    , Math.Round(columnProperties.ColumnOrigin.Y
                    - columnProperties.ColumnSectionHeight / 2
                    + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                    + (squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2)
                    - stirrupDeltaXDeltaY
                    - stirrupDeltaXDeltaY_2 * 2, 6)
                    , Math.Round(additionalRebarTopStirrup_p3.Z, 6));

                //Кривые среднего ромбического хомута
                List<Curve> additionalTopStirrupCurves = new List<Curve>();

                Curve topStirrupLine1 = Line.CreateBound(additionalRebarTopStirrup_p1, additionalRebarTopStirrup_p2) as Curve;
                additionalTopStirrupCurves.Add(topStirrupLine1);
                Curve topStirrupLine2 = Line.CreateBound(additionalRebarTopStirrup_p2, additionalRebarTopStirrup_p3) as Curve;
                additionalTopStirrupCurves.Add(topStirrupLine2);
                Curve topStirrupLine3 = Line.CreateBound(additionalRebarTopStirrup_p3, additionalRebarTopStirrup_p4) as Curve;
                additionalTopStirrupCurves.Add(topStirrupLine3);
                Curve topStirrupLine4 = Line.CreateBound(additionalRebarTopStirrup_p4, additionalRebarTopStirrup_p1) as Curve;
                additionalTopStirrupCurves.Add(topStirrupLine4);

                //Построение верхнего ромбического хомута
                Rebar topStirrup = Rebar.CreateFromCurvesAndShape(doc
                    , rebarShapeCollector.Shape51
                    , squareColumnsReinforcementMainForm.AdditionalStirrupRebarTape
                    , rebarShapeCollector.SeismicRebarHook135
                    , rebarShapeCollector.SeismicRebarHook135
                    , column
                    , stirrupNormal
                    , additionalTopStirrupCurves
                    , RebarHookOrientation.Right
                    , RebarHookOrientation.Right);

                topStirrup.GetShapeDrivenAccessor()
                    .SetLayoutAsNumberWithSpacing(((int)(squareColumnsReinforcementMainForm.FrequentTopStirrupPlacementHeight
                    / squareColumnsReinforcementMainForm.FrequentTopStirrupStep) + 1)
                    , (squareColumnsReinforcementMainForm.FrequentTopStirrupStep / 304.8), false, true, true);
                rebarIdCollection.Add(topStirrup.Id);
            }
            else if (Math.Round(remainingHeight, 6) > Math.Round((squareColumnsReinforcementMainForm.FrequentTopStirrupStep / 304.8), 6) & Math.Round(remainingHeight, 6) != 0)
            {
                int topStirrupQuantityOfAdditionalBars = (int)(remainingHeight / (squareColumnsReinforcementMainForm.FrequentTopStirrupStep / 304.8));
                
                //Точки для построения кривых верхнего среднего ромбического хомута
                XYZ additionalRebarTopStirrup_p1 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X
                    - columnProperties.ColumnSectionWidth / 2
                    + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                    + (squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2)
                    - stirrupDeltaXDeltaY
                    - stirrupDeltaXDeltaY_2, 6)
                    , Math.Round(columnProperties.ColumnOrigin.Y
                    - stirrupDeltaXDeltaY_2, 6)
                    , Math.Round(columnProperties.ColumnOrigin.Z
                    + (squareColumnsReinforcementMainForm.FirstBottomStirrupOffset / 304.8)
                    + (columnProperties.ColumnLength 
                    - (squareColumnsReinforcementMainForm.FirstBottomStirrupOffset / 304.8) 
                    - (remainingHeight 
                    - topStirrupQuantityOfAdditionalBars * (squareColumnsReinforcementMainForm.FrequentTopStirrupStep / 304.8)))
                    + squareColumnsReinforcementMainForm.MainStirrupRebarTapeDiam / 2
                    + squareColumnsReinforcementMainForm.AdditionalStirrupRebarTapeDiam / 2, 6));

                XYZ additionalRebarTopStirrup_p2 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X
                    - stirrupDeltaXDeltaY_2, 6)
                    , Math.Round(columnProperties.ColumnOrigin.Y
                    + columnProperties.ColumnSectionHeight / 2
                    - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                    - (squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2)
                    + stirrupDeltaXDeltaY
                    - stirrupDeltaXDeltaY_2, 6)
                    , Math.Round(additionalRebarTopStirrup_p1.Z, 6));

                XYZ additionalRebarTopStirrup_p3 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X
                    + columnProperties.ColumnSectionWidth / 2
                    - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                    - (squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2)
                    + stirrupDeltaXDeltaY, 6)
                    , Math.Round(columnProperties.ColumnOrigin.Y
                    - stirrupDeltaXDeltaY_2 * 2, 6)
                    , Math.Round(additionalRebarTopStirrup_p2.Z, 6));

                XYZ additionalRebarTopStirrup_p4 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                    , Math.Round(columnProperties.ColumnOrigin.Y
                    - columnProperties.ColumnSectionHeight / 2
                    + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                    + (squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2)
                    - stirrupDeltaXDeltaY
                    - stirrupDeltaXDeltaY_2 * 2, 6)
                    , Math.Round(additionalRebarTopStirrup_p3.Z, 6));


                //Кривые среднего ромбического хомута
                List<Curve> additionalTopStirrupCurves = new List<Curve>();

                Curve topStirrupLine1 = Line.CreateBound(additionalRebarTopStirrup_p1, additionalRebarTopStirrup_p2) as Curve;
                additionalTopStirrupCurves.Add(topStirrupLine1);
                Curve topStirrupLine2 = Line.CreateBound(additionalRebarTopStirrup_p2, additionalRebarTopStirrup_p3) as Curve;
                additionalTopStirrupCurves.Add(topStirrupLine2);
                Curve topStirrupLine3 = Line.CreateBound(additionalRebarTopStirrup_p3, additionalRebarTopStirrup_p4) as Curve;
                additionalTopStirrupCurves.Add(topStirrupLine3);
                Curve topStirrupLine4 = Line.CreateBound(additionalRebarTopStirrup_p4, additionalRebarTopStirrup_p1) as Curve;
                additionalTopStirrupCurves.Add(topStirrupLine4);

                //Построение верхнего ромбического хомута
                Rebar topStirrup = Rebar.CreateFromCurvesAndShape(doc
                    , rebarShapeCollector.Shape51
                    , squareColumnsReinforcementMainForm.AdditionalStirrupRebarTape
                    , rebarShapeCollector.SeismicRebarHook135
                    , rebarShapeCollector.SeismicRebarHook135
                    , column
                    , stirrupNormal
                    , additionalTopStirrupCurves
                    , RebarHookOrientation.Right
                    , RebarHookOrientation.Right);

                topStirrup.GetShapeDrivenAccessor()
                    .SetLayoutAsNumberWithSpacing(((int)(squareColumnsReinforcementMainForm.FrequentTopStirrupPlacementHeight
                    / squareColumnsReinforcementMainForm.FrequentTopStirrupStep) + 1 + topStirrupQuantityOfAdditionalBars)
                    , (squareColumnsReinforcementMainForm.FrequentTopStirrupStep / 304.8), false, true, true);
                rebarIdCollection.Add(topStirrup.Id);
            }
            else
            {
                //Точки для построения кривых верхнего среднего ромбического хомута
                XYZ additionalRebarTopStirrup_p1 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X
                    - columnProperties.ColumnSectionWidth / 2
                    + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                    + (squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2)
                    - stirrupDeltaXDeltaY
                    - stirrupDeltaXDeltaY_2, 6)
                    , Math.Round(columnProperties.ColumnOrigin.Y
                    - stirrupDeltaXDeltaY_2, 6)
                    , Math.Round(columnProperties.ColumnOrigin.Z
                    + (squareColumnsReinforcementMainForm.FirstBottomStirrupOffset / 304.8)
                    + (columnProperties.ColumnLength 
                    - (squareColumnsReinforcementMainForm.FirstBottomStirrupOffset / 304.8) 
                    - (50 / 304.8))
                    + squareColumnsReinforcementMainForm.MainStirrupRebarTapeDiam / 2
                    + squareColumnsReinforcementMainForm.AdditionalStirrupRebarTapeDiam / 2, 6));

                XYZ additionalRebarTopStirrup_p2 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X
                    - stirrupDeltaXDeltaY_2, 6)
                    , Math.Round(columnProperties.ColumnOrigin.Y
                    + columnProperties.ColumnSectionHeight / 2
                    - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                    - (squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2)
                    + stirrupDeltaXDeltaY
                    - stirrupDeltaXDeltaY_2, 6)
                    , Math.Round(additionalRebarTopStirrup_p1.Z, 6));

                XYZ additionalRebarTopStirrup_p3 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X
                    + columnProperties.ColumnSectionWidth / 2
                    - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                    - (squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2)
                    + stirrupDeltaXDeltaY, 6)
                    , Math.Round(columnProperties.ColumnOrigin.Y
                    - stirrupDeltaXDeltaY_2 * 2, 6)
                    , Math.Round(additionalRebarTopStirrup_p2.Z, 6));

                XYZ additionalRebarTopStirrup_p4 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                    , Math.Round(columnProperties.ColumnOrigin.Y
                    - columnProperties.ColumnSectionHeight / 2
                    + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                    + (squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2)
                    - stirrupDeltaXDeltaY
                    - stirrupDeltaXDeltaY_2 * 2, 6)
                    , Math.Round(additionalRebarTopStirrup_p3.Z, 6));

                //Кривые среднего ромбического хомута
                List<Curve> additionalTopStirrupCurves = new List<Curve>();

                Curve topStirrupLine1 = Line.CreateBound(additionalRebarTopStirrup_p1, additionalRebarTopStirrup_p2) as Curve;
                additionalTopStirrupCurves.Add(topStirrupLine1);
                Curve topStirrupLine2 = Line.CreateBound(additionalRebarTopStirrup_p2, additionalRebarTopStirrup_p3) as Curve;
                additionalTopStirrupCurves.Add(topStirrupLine2);
                Curve topStirrupLine3 = Line.CreateBound(additionalRebarTopStirrup_p3, additionalRebarTopStirrup_p4) as Curve;
                additionalTopStirrupCurves.Add(topStirrupLine3);
                Curve topStirrupLine4 = Line.CreateBound(additionalRebarTopStirrup_p4, additionalRebarTopStirrup_p1) as Curve;
                additionalTopStirrupCurves.Add(topStirrupLine4);

                //Построение верхнего ромбического хомута
                Rebar topStirrup = Rebar.CreateFromCurvesAndShape(doc
                    , rebarShapeCollector.Shape51
                    , squareColumnsReinforcementMainForm.AdditionalStirrupRebarTape
                    , rebarShapeCollector.SeismicRebarHook135
                    , rebarShapeCollector.SeismicRebarHook135
                    , column
                    , stirrupNormal
                    , additionalTopStirrupCurves
                    , RebarHookOrientation.Right
                    , RebarHookOrientation.Right);

                topStirrup.GetShapeDrivenAccessor()
                    .SetLayoutAsNumberWithSpacing(((int)(squareColumnsReinforcementMainForm.FrequentTopStirrupPlacementHeight
                    / squareColumnsReinforcementMainForm.FrequentTopStirrupStep) + 1)
                    , (squareColumnsReinforcementMainForm.FrequentTopStirrupStep / 304.8), false, true, true);
                rebarIdCollection.Add(topStirrup.Id);
            }
        }
        public static void CreateCrossoverStirrup(Document doc
            , RebarShapeCollector rebarShapeCollector
            , FamilyInstance column
            , ColumnProperties columnProperties
            , SquareColumnsReinforcementMainForm squareColumnsReinforcementMainForm
            , ICollection<ElementId> rebarIdCollection)
        {
            //Нормаль для построения хомута
            XYZ stirrupNormal = new XYZ(0, 0, 1);

            //Точки для построения кривых стержня хомута дополнительного
            XYZ additionalRebarDownStirrup_p1 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X
                - (squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                - squareColumnsReinforcementMainForm.AdditionalStirrupRebarTapeDiam, 6)
                , Math.Round(columnProperties.ColumnOrigin.Y
                + columnProperties.ColumnSectionWidth / 2
                - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                + squareColumnsReinforcementMainForm.AdditionalStirrupRebarTapeDiam / 2, 6)
                , Math.Round(columnProperties.ColumnOrigin.Z
                + (squareColumnsReinforcementMainForm.FirstBottomStirrupOffset / 304.8)
                + squareColumnsReinforcementMainForm.MainStirrupRebarTapeDiam / 2
                + squareColumnsReinforcementMainForm.AdditionalStirrupRebarTapeDiam / 2, 6));

            XYZ additionalRebarDownStirrup_p2 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X
                + (squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 6)
                , Math.Round(columnProperties.ColumnOrigin.Y
                + columnProperties.ColumnSectionWidth / 2
                - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                + squareColumnsReinforcementMainForm.AdditionalStirrupRebarTapeDiam / 2, 6)
                , Math.Round(additionalRebarDownStirrup_p1.Z, 6));

            XYZ additionalRebarDownStirrup_p3 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X
                + (squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 6)
                , Math.Round(columnProperties.ColumnOrigin.Y
                - columnProperties.ColumnSectionWidth / 2
                + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                - squareColumnsReinforcementMainForm.AdditionalStirrupRebarTapeDiam, 6)
                , Math.Round(additionalRebarDownStirrup_p2.Z, 6));

            XYZ additionalRebarDownStirrup_p4 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X
                - (squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                - squareColumnsReinforcementMainForm.AdditionalStirrupRebarTapeDiam, 6)
                , Math.Round(columnProperties.ColumnOrigin.Y
                - columnProperties.ColumnSectionWidth / 2
                + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                - squareColumnsReinforcementMainForm.AdditionalStirrupRebarTapeDiam, 6)
                , Math.Round(additionalRebarDownStirrup_p3.Z, 6));


            //Точки для построения кривых стержня хомута дополнительного 90 град
            XYZ additionalRebarDownStirrupNinetyDegrees_p1 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X
                - columnProperties.ColumnSectionHeight / 2
                + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                - squareColumnsReinforcementMainForm.AdditionalStirrupRebarTapeDiam / 2, 6)
                , Math.Round(columnProperties.ColumnOrigin.Y
                - (squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8)
                - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                - squareColumnsReinforcementMainForm.AdditionalStirrupRebarTapeDiam, 6)
                , Math.Round(columnProperties.ColumnOrigin.Z
                + (squareColumnsReinforcementMainForm.FirstBottomStirrupOffset / 304.8)
                + squareColumnsReinforcementMainForm.MainStirrupRebarTapeDiam / 2
                + squareColumnsReinforcementMainForm.AdditionalStirrupRebarTapeDiam / 2
                + squareColumnsReinforcementMainForm.AdditionalStirrupRebarTapeDiam, 6));

            XYZ additionalRebarDownStirrupNinetyDegrees_p2 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X
                - columnProperties.ColumnSectionHeight / 2
                + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                - squareColumnsReinforcementMainForm.AdditionalStirrupRebarTapeDiam / 2, 6)
                , Math.Round(columnProperties.ColumnOrigin.Y
                + (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8)
                + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 6)
                , Math.Round(additionalRebarDownStirrupNinetyDegrees_p1.Z, 6));

            XYZ additionalRebarDownStirrupNinetyDegrees_p3 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X
                + columnProperties.ColumnSectionHeight / 2
                - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                + squareColumnsReinforcementMainForm.AdditionalStirrupRebarTapeDiam, 6)
                , Math.Round(columnProperties.ColumnOrigin.Y
                + (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8)
                + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 6)
                , Math.Round(additionalRebarDownStirrupNinetyDegrees_p2.Z, 6));

            XYZ additionalRebarDownStirrupNinetyDegrees_p4 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X
                + columnProperties.ColumnSectionHeight / 2
                - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                + squareColumnsReinforcementMainForm.AdditionalStirrupRebarTapeDiam, 6)
                , Math.Round(columnProperties.ColumnOrigin.Y
                - (squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8)
                - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                - squareColumnsReinforcementMainForm.AdditionalStirrupRebarTapeDiam, 6)
                , Math.Round(additionalRebarDownStirrupNinetyDegrees_p3.Z, 6));

            //Кривые нижнего дополнительного хомута
            List<Curve> additionalDownStirrupCurves = new List<Curve>();

            Curve downStirrupLine1 = Line.CreateBound(additionalRebarDownStirrup_p1, additionalRebarDownStirrup_p2) as Curve;
            additionalDownStirrupCurves.Add(downStirrupLine1);
            Curve downStirrupLine2 = Line.CreateBound(additionalRebarDownStirrup_p2, additionalRebarDownStirrup_p3) as Curve;
            additionalDownStirrupCurves.Add(downStirrupLine2);
            Curve downStirrupLine3 = Line.CreateBound(additionalRebarDownStirrup_p3, additionalRebarDownStirrup_p4) as Curve;
            additionalDownStirrupCurves.Add(downStirrupLine3);
            Curve downStirrupLine4 = Line.CreateBound(additionalRebarDownStirrup_p4, additionalRebarDownStirrup_p1) as Curve;
            additionalDownStirrupCurves.Add(downStirrupLine4);

            //Кривые нижнего дополнительного хомута 90 град
            List<Curve> additionalDownStirrupNinetyDegreesCurves = new List<Curve>();

            Curve downStirrupNinetyDegreesLine1 = Line.CreateBound(additionalRebarDownStirrupNinetyDegrees_p1, additionalRebarDownStirrupNinetyDegrees_p2) as Curve;
            additionalDownStirrupNinetyDegreesCurves.Add(downStirrupNinetyDegreesLine1);
            Curve downStirrupNinetyDegreesLine2 = Line.CreateBound(additionalRebarDownStirrupNinetyDegrees_p2, additionalRebarDownStirrupNinetyDegrees_p3) as Curve;
            additionalDownStirrupNinetyDegreesCurves.Add(downStirrupNinetyDegreesLine2);
            Curve downStirrupNinetyDegreesLine3 = Line.CreateBound(additionalRebarDownStirrupNinetyDegrees_p3, additionalRebarDownStirrupNinetyDegrees_p4) as Curve;
            additionalDownStirrupNinetyDegreesCurves.Add(downStirrupNinetyDegreesLine3);
            Curve downStirrupNinetyDegreesLine4 = Line.CreateBound(additionalRebarDownStirrupNinetyDegrees_p4, additionalRebarDownStirrupNinetyDegrees_p1) as Curve;
            additionalDownStirrupNinetyDegreesCurves.Add(downStirrupNinetyDegreesLine4);

            //Построение нижнего хомута дополнительного
            Rebar downStirrup = Rebar.CreateFromCurvesAndShape(doc
                , rebarShapeCollector.Shape51
                , squareColumnsReinforcementMainForm.AdditionalStirrupRebarTape
                , rebarShapeCollector.SeismicRebarHook135
                , rebarShapeCollector.SeismicRebarHook135
                , column
                , stirrupNormal
                , additionalDownStirrupCurves
                , RebarHookOrientation.Right
                , RebarHookOrientation.Right);

            downStirrup.GetShapeDrivenAccessor()
                .SetLayoutAsNumberWithSpacing(((int)(squareColumnsReinforcementMainForm.FrequentBottomStirrupPlacementHeight
                / squareColumnsReinforcementMainForm.FrequentBottomStirrupStep) + 1)
                , (squareColumnsReinforcementMainForm.FrequentBottomStirrupStep / 304.8), true, true, true);
            rebarIdCollection.Add(downStirrup.Id);

            //Построение нижнего хомута дополнительного 90 град
            Rebar downStirrupNinetyDegrees = Rebar.CreateFromCurvesAndShape(doc
                , rebarShapeCollector.Shape51
                , squareColumnsReinforcementMainForm.AdditionalStirrupRebarTape
                , rebarShapeCollector.SeismicRebarHook135
                , rebarShapeCollector.SeismicRebarHook135
                , column
                , stirrupNormal
                , additionalDownStirrupNinetyDegreesCurves
                , RebarHookOrientation.Right
                , RebarHookOrientation.Right);

            downStirrupNinetyDegrees.GetShapeDrivenAccessor()
                .SetLayoutAsNumberWithSpacing(((int)(squareColumnsReinforcementMainForm.FrequentBottomStirrupPlacementHeight
                / squareColumnsReinforcementMainForm.FrequentBottomStirrupStep) + 1)
                , (squareColumnsReinforcementMainForm.FrequentBottomStirrupStep / 304.8), true, true, true);
            rebarIdCollection.Add(downStirrupNinetyDegrees.Id);

            double heightForMiddleStirrup = columnProperties.ColumnLength
                - ((squareColumnsReinforcementMainForm.FrequentBottomStirrupPlacementHeight
                + squareColumnsReinforcementMainForm.FrequentTopStirrupPlacementHeight
                 + squareColumnsReinforcementMainForm.FirstBottomStirrupOffset) / 304.8);

            int middleStirrupQuantityOfBars = (int)(heightForMiddleStirrup
                / (squareColumnsReinforcementMainForm.StandardStirrupStep / 304.8));
            double remainingHeight = heightForMiddleStirrup
                - (middleStirrupQuantityOfBars
                * (squareColumnsReinforcementMainForm.StandardStirrupStep / 304.8));

            //Копирование хомута для стандартного шага
            XYZ middleStirrupInstallationPoint = new XYZ(0, 0, (squareColumnsReinforcementMainForm.FrequentBottomStirrupPlacementHeight
                + squareColumnsReinforcementMainForm.StandardStirrupStep) / 304.8);
            Rebar middleStirrup = doc.GetElement((ElementTransformUtils.CopyElement(doc, downStirrup.Id, middleStirrupInstallationPoint) as List<ElementId>).First()) as Rebar;
            middleStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
            middleStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(middleStirrupQuantityOfBars - 1);
            middleStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set((squareColumnsReinforcementMainForm.StandardStirrupStep / 304.8));
            rebarIdCollection.Add(middleStirrup.Id);

            //Копирование хомута 90 град для стандартного шага
            Rebar middleStirrupNinetyDegrees = doc.GetElement((ElementTransformUtils.CopyElement(doc, downStirrupNinetyDegrees.Id, middleStirrupInstallationPoint) as List<ElementId>).First()) as Rebar;
            middleStirrupNinetyDegrees.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
            middleStirrupNinetyDegrees.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(middleStirrupQuantityOfBars - 1);
            middleStirrupNinetyDegrees.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set((squareColumnsReinforcementMainForm.StandardStirrupStep / 304.8));
            rebarIdCollection.Add(middleStirrupNinetyDegrees.Id);

            //Копирование хомута вверх
            if (Math.Round(remainingHeight, 6) <= Math.Round((squareColumnsReinforcementMainForm.FrequentTopStirrupStep / 304.8), 6) & Math.Round(remainingHeight, 6) != 0)
            {
                XYZ topStirrupInstallationPoint = new XYZ(0, 0, columnProperties.ColumnLength - (squareColumnsReinforcementMainForm.FirstBottomStirrupOffset / 304.8) - remainingHeight);
                Rebar topStirrup = doc.GetElement((ElementTransformUtils.CopyElement(doc, downStirrup.Id, topStirrupInstallationPoint) as List<ElementId>).First()) as Rebar;
                topStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(0);
                topStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                topStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS)
                    .Set((int)(squareColumnsReinforcementMainForm.FrequentTopStirrupPlacementHeight
                    / squareColumnsReinforcementMainForm.FrequentTopStirrupStep) + 1);
                topStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set((squareColumnsReinforcementMainForm.FrequentTopStirrupStep / 304.8));
                rebarIdCollection.Add(topStirrup.Id);

                Rebar topStirrupNinetyDegrees = doc.GetElement((ElementTransformUtils.CopyElement(doc, downStirrupNinetyDegrees.Id, topStirrupInstallationPoint) as List<ElementId>).First()) as Rebar;
                topStirrupNinetyDegrees.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(0);
                topStirrupNinetyDegrees.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                topStirrupNinetyDegrees.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS)
                    .Set((int)(squareColumnsReinforcementMainForm.FrequentTopStirrupPlacementHeight
                    / squareColumnsReinforcementMainForm.FrequentTopStirrupStep) + 1);
                topStirrupNinetyDegrees.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set((squareColumnsReinforcementMainForm.FrequentTopStirrupStep / 304.8));
                rebarIdCollection.Add(topStirrupNinetyDegrees.Id);
            }
            else if (Math.Round(remainingHeight, 6) > Math.Round((squareColumnsReinforcementMainForm.FrequentTopStirrupStep / 304.8), 6) & Math.Round(remainingHeight, 6) != 0)
            {
                int topStirrupQuantityOfAdditionalBars = (int)(remainingHeight / (squareColumnsReinforcementMainForm.FrequentTopStirrupStep / 304.8));

                XYZ topStirrupInstallationPoint = new XYZ(0, 0, columnProperties.ColumnLength - (squareColumnsReinforcementMainForm.FirstBottomStirrupOffset / 304.8) - (remainingHeight - topStirrupQuantityOfAdditionalBars * (squareColumnsReinforcementMainForm.FrequentTopStirrupStep / 304.8)));
                Rebar topStirrup = doc.GetElement((ElementTransformUtils.CopyElement(doc, downStirrup.Id, topStirrupInstallationPoint) as List<ElementId>).First()) as Rebar;
                topStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(0);
                topStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                topStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS)
                    .Set((int)(squareColumnsReinforcementMainForm.FrequentTopStirrupPlacementHeight
                    / squareColumnsReinforcementMainForm.FrequentTopStirrupStep) + 1 + topStirrupQuantityOfAdditionalBars);
                topStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set((squareColumnsReinforcementMainForm.FrequentTopStirrupStep / 304.8));
                rebarIdCollection.Add(topStirrup.Id);

                Rebar topStirrupNinetyDegrees = doc.GetElement((ElementTransformUtils.CopyElement(doc, downStirrupNinetyDegrees.Id, topStirrupInstallationPoint) as List<ElementId>).First()) as Rebar;
                topStirrupNinetyDegrees.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(0);
                topStirrupNinetyDegrees.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                topStirrupNinetyDegrees.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS)
                    .Set((int)(squareColumnsReinforcementMainForm.FrequentTopStirrupPlacementHeight
                    / squareColumnsReinforcementMainForm.FrequentTopStirrupStep) + 1 + topStirrupQuantityOfAdditionalBars);
                topStirrupNinetyDegrees.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set((squareColumnsReinforcementMainForm.FrequentTopStirrupStep / 304.8));
                rebarIdCollection.Add(topStirrupNinetyDegrees.Id);
            }
            else
            {
                XYZ topStirrupInstallationPoint = new XYZ(0, 0, columnProperties.ColumnLength - (squareColumnsReinforcementMainForm.FirstBottomStirrupOffset / 304.8) - (50 / 304.8));
                Rebar topStirrup = doc.GetElement((ElementTransformUtils.CopyElement(doc, downStirrup.Id, topStirrupInstallationPoint) as List<ElementId>).First()) as Rebar;
                topStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(0);
                topStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                topStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS)
                    .Set((int)(squareColumnsReinforcementMainForm.FrequentTopStirrupPlacementHeight
                    / squareColumnsReinforcementMainForm.FrequentTopStirrupStep) + 1);
                topStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set((squareColumnsReinforcementMainForm.FrequentTopStirrupStep / 304.8));
                rebarIdCollection.Add(topStirrup.Id);

                Rebar topStirrupNinetyDegrees = doc.GetElement((ElementTransformUtils.CopyElement(doc, downStirrupNinetyDegrees.Id, topStirrupInstallationPoint) as List<ElementId>).First()) as Rebar;
                topStirrupNinetyDegrees.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(0);
                topStirrupNinetyDegrees.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                topStirrupNinetyDegrees.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS)
                    .Set((int)(squareColumnsReinforcementMainForm.FrequentTopStirrupPlacementHeight
                    / squareColumnsReinforcementMainForm.FrequentTopStirrupStep) + 1);
                topStirrupNinetyDegrees.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set((squareColumnsReinforcementMainForm.FrequentTopStirrupStep / 304.8));
                rebarIdCollection.Add(topStirrupNinetyDegrees.Id);
            }
        }

        public static void CreateProgressiveCollapseRebar(Document doc
            , RebarShapeCollector rebarShapeCollector
            , FamilyInstance column
            , ColumnProperties columnProperties
            , SquareColumnsReinforcementMainForm squareColumnsReinforcementMainForm
            , ICollection<ElementId> rebarIdCollection)
        {
            if (squareColumnsReinforcementMainForm.ProgressiveCollapseLeftRebar)
            {
                //Нормаль для построения стержней
                XYZ progressiveCollapseRebarNormal = new XYZ(0, 1, 0);

                //Точки для построения кривых стержня
                XYZ rebar_p1 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                    , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                    , Math.Round(columnProperties.ColumnOrigin.Z
                    - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)
                    + (squareColumnsReinforcementMainForm.ProgressiveCollapseBottomOffset / 304.8)
                    + (squareColumnsReinforcementMainForm.ProgressiveCollapseRebarTypeDiam / 2), 6));

                XYZ rebar_p2 = new XYZ(Math.Round(rebar_p1.X, 6)
                    , Math.Round(rebar_p1.Y, 6)
                    , Math.Round(rebar_p1.Z
                    + (squareColumnsReinforcementMainForm.ProgressiveCollapseUpLength / 304.8)
                    - (squareColumnsReinforcementMainForm.ProgressiveCollapseRebarTypeDiam / 2), 6));

                XYZ rebar_p3 = new XYZ(Math.Round(rebar_p1.X
                    + (squareColumnsReinforcementMainForm.ProgressiveCollapseSideLength / 304.8)
                    - (squareColumnsReinforcementMainForm.ProgressiveCollapseRebarTypeDiam / 2), 6)
                    , Math.Round(rebar_p1.Y, 6)
                    , Math.Round(rebar_p1.Z, 6));

                //Кривые стержня
                List<Curve> progressiveCollapseRebarCurves = new List<Curve>();

                Curve line1 = Line.CreateBound(rebar_p2, rebar_p1) as Curve;
                progressiveCollapseRebarCurves.Add(line1);
                Curve line2 = Line.CreateBound(rebar_p1, rebar_p3) as Curve;
                progressiveCollapseRebarCurves.Add(line2);

                //Левый стержень
                Rebar progressiveCollapseRebar = Rebar.CreateFromCurvesAndShape(doc
                    , rebarShapeCollector.Shape11
                    , squareColumnsReinforcementMainForm.ProgressiveCollapseRebarType
                    , null
                    , null
                    , column
                    , progressiveCollapseRebarNormal
                    , progressiveCollapseRebarCurves
                    , RebarHookOrientation.Right
                    , RebarHookOrientation.Right);
                XYZ rotate_p1 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z);
                XYZ rotate_p2 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z + 1);
                Line rotateLine = Line.CreateBound(rotate_p1, rotate_p2);
                ElementTransformUtils.RotateElement(doc, progressiveCollapseRebar.Id, rotateLine, 180 * (Math.PI / 180));
                XYZ newPlaсeProgressiveCollapseRebar = new XYZ(-columnProperties.ColumnSectionWidth / 4, 0, 0);
                ElementTransformUtils.MoveElement(doc, progressiveCollapseRebar.Id, newPlaсeProgressiveCollapseRebar);
                rebarIdCollection.Add(progressiveCollapseRebar.Id);
            }
            if (squareColumnsReinforcementMainForm.ProgressiveCollapseRightRebar)
            {
                //Нормаль для построения стержней
                XYZ progressiveCollapseRebarNormal = new XYZ(0, 1, 0);

                //Точки для построения кривых стержня
                XYZ rebar_p1 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                    , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                    , Math.Round(columnProperties.ColumnOrigin.Z
                    - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)
                    + (squareColumnsReinforcementMainForm.ProgressiveCollapseBottomOffset / 304.8)
                    + (squareColumnsReinforcementMainForm.ProgressiveCollapseRebarTypeDiam / 2), 6));

                XYZ rebar_p2 = new XYZ(Math.Round(rebar_p1.X, 6)
                    , Math.Round(rebar_p1.Y, 6)
                    , Math.Round(rebar_p1.Z
                    + (squareColumnsReinforcementMainForm.ProgressiveCollapseUpLength / 304.8)
                    - (squareColumnsReinforcementMainForm.ProgressiveCollapseRebarTypeDiam / 2), 6));

                XYZ rebar_p3 = new XYZ(Math.Round(rebar_p1.X
                    + (squareColumnsReinforcementMainForm.ProgressiveCollapseSideLength / 304.8)
                    - (squareColumnsReinforcementMainForm.ProgressiveCollapseRebarTypeDiam / 2), 6)
                    , Math.Round(rebar_p1.Y, 6)
                    , Math.Round(rebar_p1.Z, 6));

                //Кривые стержня
                List<Curve> progressiveCollapseRebarCurves = new List<Curve>();

                Curve line1 = Line.CreateBound(rebar_p2, rebar_p1) as Curve;
                progressiveCollapseRebarCurves.Add(line1);
                Curve line2 = Line.CreateBound(rebar_p1, rebar_p3) as Curve;
                progressiveCollapseRebarCurves.Add(line2);

                //Правый стержень
                Rebar progressiveCollapseRebar = Rebar.CreateFromCurvesAndShape(doc
                    , rebarShapeCollector.Shape11
                    , squareColumnsReinforcementMainForm.ProgressiveCollapseRebarType
                    , null
                    , null
                    , column
                    , progressiveCollapseRebarNormal
                    , progressiveCollapseRebarCurves
                    , RebarHookOrientation.Right
                    , RebarHookOrientation.Right);
                XYZ newPlaсeProgressiveCollapseRebar = new XYZ(columnProperties.ColumnSectionWidth / 4, 0, 0);
                ElementTransformUtils.MoveElement(doc, progressiveCollapseRebar.Id, newPlaсeProgressiveCollapseRebar);
                rebarIdCollection.Add(progressiveCollapseRebar.Id);
            }
            if (squareColumnsReinforcementMainForm.ProgressiveCollapseDownRebar)
            {
                //Нормаль для построения стержней
                XYZ progressiveCollapseRebarNormal = new XYZ(0, 1, 0);

                //Точки для построения кривых стержня
                XYZ rebar_p1 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                    , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                    , Math.Round(columnProperties.ColumnOrigin.Z
                    - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)
                    + (squareColumnsReinforcementMainForm.ProgressiveCollapseBottomOffset / 304.8)
                    + (squareColumnsReinforcementMainForm.ProgressiveCollapseRebarTypeDiam / 2), 6));

                XYZ rebar_p2 = new XYZ(Math.Round(rebar_p1.X, 6)
                    , Math.Round(rebar_p1.Y, 6)
                    , Math.Round(rebar_p1.Z
                    + (squareColumnsReinforcementMainForm.ProgressiveCollapseUpLength / 304.8)
                    - (squareColumnsReinforcementMainForm.ProgressiveCollapseRebarTypeDiam / 2), 6));

                XYZ rebar_p3 = new XYZ(Math.Round(rebar_p1.X
                    + (squareColumnsReinforcementMainForm.ProgressiveCollapseSideLength / 304.8)
                    - (squareColumnsReinforcementMainForm.ProgressiveCollapseRebarTypeDiam / 2), 6)
                    , Math.Round(rebar_p1.Y, 6)
                    , Math.Round(rebar_p1.Z, 6));

                //Кривые стержня
                List<Curve> progressiveCollapseRebarCurves = new List<Curve>();

                Curve line1 = Line.CreateBound(rebar_p2, rebar_p1) as Curve;
                progressiveCollapseRebarCurves.Add(line1);
                Curve line2 = Line.CreateBound(rebar_p1, rebar_p3) as Curve;
                progressiveCollapseRebarCurves.Add(line2);

                //Нижний стержень
                Rebar progressiveCollapseRebar = Rebar.CreateFromCurvesAndShape(doc
                    , rebarShapeCollector.Shape11
                    , squareColumnsReinforcementMainForm.ProgressiveCollapseRebarType
                    , null
                    , null
                    , column
                    , progressiveCollapseRebarNormal
                    , progressiveCollapseRebarCurves
                    , RebarHookOrientation.Right
                    , RebarHookOrientation.Right);
                XYZ rotate_p1 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z);
                XYZ rotate_p2 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z + 1);
                Line rotateLine = Line.CreateBound(rotate_p1, rotate_p2);
                ElementTransformUtils.RotateElement(doc, progressiveCollapseRebar.Id, rotateLine, -90 * (Math.PI / 180));
                XYZ newPlaсeProgressiveCollapseRebar = new XYZ(0, -columnProperties.ColumnSectionHeight / 4, 0);
                ElementTransformUtils.MoveElement(doc, progressiveCollapseRebar.Id, newPlaсeProgressiveCollapseRebar);
                rebarIdCollection.Add(progressiveCollapseRebar.Id);
            }
            if (squareColumnsReinforcementMainForm.ProgressiveCollapseUpRebar)
            {
                //Нормаль для построения стержней
                XYZ progressiveCollapseRebarNormal = new XYZ(0, 1, 0);

                //Точки для построения кривых стержня
                XYZ rebar_p1 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                    , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                    , Math.Round(columnProperties.ColumnOrigin.Z
                    - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)
                    + (squareColumnsReinforcementMainForm.ProgressiveCollapseBottomOffset / 304.8)
                    + (squareColumnsReinforcementMainForm.ProgressiveCollapseRebarTypeDiam / 2), 6));

                XYZ rebar_p2 = new XYZ(Math.Round(rebar_p1.X, 6)
                    , Math.Round(rebar_p1.Y, 6)
                    , Math.Round(rebar_p1.Z
                    + (squareColumnsReinforcementMainForm.ProgressiveCollapseUpLength / 304.8)
                    - (squareColumnsReinforcementMainForm.ProgressiveCollapseRebarTypeDiam / 2), 6));

                XYZ rebar_p3 = new XYZ(Math.Round(rebar_p1.X
                    + (squareColumnsReinforcementMainForm.ProgressiveCollapseSideLength / 304.8)
                    - (squareColumnsReinforcementMainForm.ProgressiveCollapseRebarTypeDiam / 2), 6)
                    , Math.Round(rebar_p1.Y, 6)
                    , Math.Round(rebar_p1.Z, 6));

                //Кривые стержня
                List<Curve> progressiveCollapseRebarCurves = new List<Curve>();

                Curve line1 = Line.CreateBound(rebar_p2, rebar_p1) as Curve;
                progressiveCollapseRebarCurves.Add(line1);
                Curve line2 = Line.CreateBound(rebar_p1, rebar_p3) as Curve;
                progressiveCollapseRebarCurves.Add(line2);

                //Верхний стержень
                Rebar progressiveCollapseRebar = Rebar.CreateFromCurvesAndShape(doc
                    , rebarShapeCollector.Shape11
                    , squareColumnsReinforcementMainForm.ProgressiveCollapseRebarType
                    , null
                    , null
                    , column
                    , progressiveCollapseRebarNormal
                    , progressiveCollapseRebarCurves
                    , RebarHookOrientation.Right
                    , RebarHookOrientation.Right);
                XYZ rotate_p1 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z);
                XYZ rotate_p2 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z + 1);
                Line rotateLine = Line.CreateBound(rotate_p1, rotate_p2);
                ElementTransformUtils.RotateElement(doc, progressiveCollapseRebar.Id, rotateLine, 90 * (Math.PI / 180));
                XYZ newPlaсeProgressiveCollapseRebar = new XYZ(0, columnProperties.ColumnSectionHeight / 4, 0);
                ElementTransformUtils.MoveElement(doc, progressiveCollapseRebar.Id, newPlaсeProgressiveCollapseRebar);
                rebarIdCollection.Add(progressiveCollapseRebar.Id);
            }
        }
    }
}
