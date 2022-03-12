using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;

namespace SquareColumnsReinforcement
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class SquareColumnsReinforcementCommandT6 : IExternalCommand
    {
        List<FamilyInstance> columnsList;
        SquareColumnsReinforcementMainForm squareColumnsReinforcementMainForm;
        public SquareColumnsReinforcementCommandT6(List<FamilyInstance> columnsListFromMainComand, SquareColumnsReinforcementMainForm formFromMainComand)
        {
            columnsList = columnsListFromMainComand;
            squareColumnsReinforcementMainForm = formFromMainComand;
        }
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            return Execute(commandData.Application);
        }
        public Result Execute(UIApplication uiapp)
        {
            Document doc = uiapp.ActiveUIDocument.Document;
            RebarShapeCollector rebarShapeCollector = new RebarShapeCollector(doc);
            if (!rebarShapeCollector.CollectionResultOk)
            {
                return Result.Cancelled;
            }

            using (Transaction t = new Transaction(doc))
            {
                t.Start("Армирование колонны Тип6");
                foreach (FamilyInstance column in columnsList)
                {
                    ColumnProperties columnProperties = new ColumnProperties(doc, column);

                    if (columnProperties.ColumnSectionWidth != columnProperties.ColumnSectionHeight)
                    {
                        continue;
                    }

                    //Защитный слой колонны
                    column.get_Parameter(BuiltInParameter.CLEAR_COVER_OTHER).Set(squareColumnsReinforcementMainForm.RebarCoverType.Id);

                    //Универсальная коллекция для формирования группы армирования колонны
                    ICollection<ElementId> rebarIdCollection = new List<ElementId>();

                    //Нормаль для построения стержней основной арматуры
                    XYZ mainRebarNormal = new XYZ(0, 1, 0);

                    //Дополнительные расчеты при изменении сечения колонны выше
                    double deltaXMainRebarOverlapping = Math.Sqrt(Math.Pow(((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8)
                        + squareColumnsReinforcementMainForm.MainRebarTapeDiam), 2)
                        + Math.Pow((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8), 2));
                    double alphaMainRebarOverlapping = Math.Asin((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8) / deltaXMainRebarOverlapping);

                    double deltaXAdditionalMainRebarOverlapping = Math.Sqrt(Math.Pow((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8), 2)
                        + Math.Pow(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam, 2));
                    double alphaAdditionalMainRebarOverlapping = Math.Asin(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / deltaXAdditionalMainRebarOverlapping);


                    double deltaXWelding = Math.Sqrt(Math.Pow((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8), 2)
                        + Math.Pow((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8), 2));
                    double alphaWelding = Math.Asin((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8) / deltaXWelding);

                    //Если стыковка стержней в нахлест без изменения сечения колонны выше
                    if (squareColumnsReinforcementMainForm.CheckedRebarOutletsTepeName == "radioButton_Overlapping"
                        & squareColumnsReinforcementMainForm.СhangeSection == false
                        & squareColumnsReinforcementMainForm.BendIntoSlab == false)
                    {
                        //Точки для построения кривых основных угловых стержней удлиненных
                        XYZ rebar_p1L = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z 
                            - (squareColumnsReinforcementMainForm.RebarDeepeningOffset / 304.8), 6));
                        XYZ rebar_p2L = new XYZ(Math.Round(rebar_p1L.X, 6)
                            , Math.Round(rebar_p1L.Y, 6)
                            , Math.Round(rebar_p1L.Z 
                            + (squareColumnsReinforcementMainForm.RebarDeepeningOffset / 304.8) 
                            + columnProperties.ColumnLength, 6));
                        XYZ rebar_p3L = new XYZ(Math.Round(rebar_p2L.X 
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam, 6)
                            , Math.Round(rebar_p2L.Y, 6)
                            , Math.Round(rebar_p2L.Z 
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8), 6));
                        XYZ rebar_p4L = new XYZ(Math.Round(rebar_p3L.X, 6)
                            , Math.Round(rebar_p3L.Y, 6)
                            , Math.Round(rebar_p3L.Z 
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));

                        //Точки для построения кривых основных угловых стержней укороченных
                        XYZ rebar_p1S = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z 
                            - (squareColumnsReinforcementMainForm.RebarDeepeningOffset / 304.8), 6));
                        XYZ rebar_p2S = new XYZ(Math.Round(rebar_p1S.X, 6)
                            , Math.Round(rebar_p1S.Y, 6)
                            , Math.Round(rebar_p1S.Z
                            + (squareColumnsReinforcementMainForm.RebarDeepeningOffset / 304.8) 
                            + columnProperties.ColumnLength, 6));
                        XYZ rebar_p3S = new XYZ(Math.Round(rebar_p2S.X 
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam, 6)
                            , Math.Round(rebar_p2S.Y, 6)
                            , Math.Round(rebar_p2S.Z
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8), 6));
                        XYZ rebar_p4S = new XYZ(Math.Round(rebar_p3S.X, 6)
                            , Math.Round(rebar_p3S.Y, 6)
                            , Math.Round(rebar_p3S.Z 
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8), 6));

                        //Точки для построения кривых основных боковых стержней удлиненных
                        XYZ additionalRebar_p1L = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z 
                            - (squareColumnsReinforcementMainForm.RebarDeepeningOffset / 304.8), 6));
                        XYZ additionalRebar_p2L = new XYZ(Math.Round(additionalRebar_p1L.X, 6)
                            , Math.Round(additionalRebar_p1L.Y, 6)
                            , Math.Round(additionalRebar_p1L.Z 
                            + (squareColumnsReinforcementMainForm.RebarDeepeningOffset / 304.8)
                            + columnProperties.ColumnLength, 6));
                        XYZ additionalRebar_p3L = new XYZ(Math.Round(additionalRebar_p2L.X
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam, 6)
                            , Math.Round(additionalRebar_p2L.Y, 6)
                            , Math.Round(additionalRebar_p2L.Z 
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8), 6));
                        XYZ additionalRebar_p4L = new XYZ(Math.Round(additionalRebar_p3L.X, 6)
                            , Math.Round(additionalRebar_p3L.Y, 6)
                            , Math.Round(additionalRebar_p3L.Z 
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));

                        //Точки для построения кривых основных боковых стержней укороченных
                        XYZ additionalRebar_p1S = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z 
                            - (squareColumnsReinforcementMainForm.RebarDeepeningOffset / 304.8), 6));
                        XYZ additionalRebar_p2S = new XYZ(Math.Round(additionalRebar_p1S.X, 6)
                            , Math.Round(additionalRebar_p1S.Y, 6)
                            , Math.Round(additionalRebar_p1S.Z
                            + (squareColumnsReinforcementMainForm.RebarDeepeningOffset / 304.8)
                            + columnProperties.ColumnLength, 6));
                        XYZ additionalRebar_p3S = new XYZ(Math.Round(additionalRebar_p2S.X
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam, 6)
                            , Math.Round(additionalRebar_p2S.Y, 6)
                            , Math.Round(additionalRebar_p2S.Z 
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8), 6));
                        XYZ additionalRebar_p4S = new XYZ(Math.Round(additionalRebar_p3S.X, 6)
                            , Math.Round(additionalRebar_p3S.Y, 6)
                            , Math.Round(additionalRebar_p3S.Z 
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8), 6));

                        //Кривые стержня
                        List<Curve> mainRebarLongCurves = new List<Curve>();
                        List<Curve> mainRebarShortCurves = new List<Curve>();
                        List<Curve> additionalRebarLongCurves = new List<Curve>();
                        List<Curve> additionalRebarShortCurves = new List<Curve>();

                        Curve line1L = Line.CreateBound(rebar_p1L, rebar_p2L) as Curve;
                        mainRebarLongCurves.Add(line1L);
                        Curve line2L = Line.CreateBound(rebar_p2L, rebar_p3L) as Curve;
                        mainRebarLongCurves.Add(line2L);
                        Curve line3L = Line.CreateBound(rebar_p3L, rebar_p4L) as Curve;
                        mainRebarLongCurves.Add(line3L);

                        Curve line1S = Line.CreateBound(rebar_p1S, rebar_p2S) as Curve;
                        mainRebarShortCurves.Add(line1S);
                        Curve line2S = Line.CreateBound(rebar_p2S, rebar_p3S) as Curve;
                        mainRebarShortCurves.Add(line2S);
                        Curve line3S = Line.CreateBound(rebar_p3S, rebar_p4S) as Curve;
                        mainRebarShortCurves.Add(line3S);

                        Curve additionalLine1L = Line.CreateBound(additionalRebar_p1L, additionalRebar_p2L) as Curve;
                        additionalRebarLongCurves.Add(additionalLine1L);
                        Curve additionalLine2L = Line.CreateBound(additionalRebar_p2L, additionalRebar_p3L) as Curve;
                        additionalRebarLongCurves.Add(additionalLine2L);
                        Curve additionalLine3L = Line.CreateBound(additionalRebar_p3L, additionalRebar_p4L) as Curve;
                        additionalRebarLongCurves.Add(additionalLine3L);

                        Curve additionalLine1S = Line.CreateBound(additionalRebar_p1S, additionalRebar_p2S) as Curve;
                        additionalRebarShortCurves.Add(additionalLine1S);
                        Curve additionalLine2S = Line.CreateBound(additionalRebar_p2S, additionalRebar_p3S) as Curve;
                        additionalRebarShortCurves.Add(additionalLine2S);
                        Curve additionalLine3S = Line.CreateBound(additionalRebar_p3S, additionalRebar_p4S) as Curve;
                        additionalRebarShortCurves.Add(additionalLine3S);

                        //Нижний левый угол
                        Rebar mainRebarLeftBottom = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.MainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeMainRebarLeftBottom = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, mainRebarLeftBottom.Id, newPlaсeMainRebarLeftBottom);
                        rebarIdCollection.Add(mainRebarLeftBottom.Id);

                        //Верхний левый угол
                        Rebar mainRebarLeftTop = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.MainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeMainRebarLeftTop = new XYZ(-columnProperties.ColumnSectionHeight / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            - squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, mainRebarLeftTop.Id, newPlaсeMainRebarLeftTop);
                        rebarIdCollection.Add(mainRebarLeftTop.Id);

                        //Верхний правый угол
                        Rebar mainRebarRightTop = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.MainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ rotate_p1 = new XYZ(rebar_p1L.X, rebar_p1L.Y, rebar_p1L.Z);
                        XYZ rotate_p2 = new XYZ(rebar_p1L.X, rebar_p1L.Y, rebar_p1L.Z + 1);
                        Line rotateLine = Line.CreateBound(rotate_p1, rotate_p2);
                        ElementTransformUtils.RotateElement(doc, mainRebarRightTop.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeMainRebarRightTop = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2
                            , columnProperties.ColumnSectionWidth / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, mainRebarRightTop.Id, newPlaсeMainRebarRightTop);
                        rebarIdCollection.Add(mainRebarRightTop.Id);

                        //Нижний правый угол
                        Rebar mainRebarRightBottom = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.MainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, mainRebarRightBottom.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeMainRebarRightBottom = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, mainRebarRightBottom.Id, newPlaсeMainRebarRightBottom);
                        rebarIdCollection.Add(mainRebarRightBottom.Id);


                        //Центральный левый нижний стержень Первый
                        Rebar additionalRebarLeftCenterDownFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarLeftCenterDownFirst.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarLeftCenterDownFirst = new XYZ(-columnProperties.ColumnSectionHeight / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterDownFirst.Id, newPlaсeAdditionalRebarLeftCenterDownFirst);
                        rebarIdCollection.Add(additionalRebarLeftCenterDownFirst.Id);

                        //Центральный левый верхний стержень Первый
                        Rebar additionalRebarLeftCenterUpFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarLeftCenterUpFirst.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarLeftCenterUpFirst = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterUpFirst.Id, newPlaсeAdditionalRebarLeftCenterUpFirst);
                        rebarIdCollection.Add(additionalRebarLeftCenterUpFirst.Id);

                        //Центральный левый нижний стержень Второй
                        Rebar additionalRebarLeftCenterDownSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarLeftCenterDownSecond.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarLeftCenterDownSecond = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8)
                            - (squareColumnsReinforcementMainForm.SecondDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterDownSecond.Id, newPlaсeAdditionalRebarLeftCenterDownSecond);
                        rebarIdCollection.Add(additionalRebarLeftCenterDownSecond.Id);

                        //Центральный левый верхний стержень Второй
                        Rebar additionalRebarLeftCenterUpSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarLeftCenterUpSecond.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarLeftCenterUpSecond = new XYZ(-columnProperties.ColumnSectionHeight / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8) 
                            + (squareColumnsReinforcementMainForm.SecondUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterUpSecond.Id, newPlaсeAdditionalRebarLeftCenterUpSecond);
                        rebarIdCollection.Add(additionalRebarLeftCenterUpSecond.Id);

                        //Правая грань
                        //Центральный правый нижний стержень Первый
                        Rebar additionalRebarRightCenterDownFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenterDownFirst.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarRightCenterDownFirst = new XYZ(+columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterDownFirst.Id, newPlaсeAdditionalRebarRightCenterDownFirst);
                        rebarIdCollection.Add(additionalRebarRightCenterDownFirst.Id);

                        //Центральный правый верхний стержень Первый
                        Rebar additionalRebarRightCenterUpFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenterUpFirst.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarRightCenterUpFirst = new XYZ(+columnProperties.ColumnSectionHeight / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterUpFirst.Id, newPlaсeAdditionalRebarRightCenterUpFirst);
                        rebarIdCollection.Add(additionalRebarRightCenterUpFirst.Id);

                        //Центральный правый нижний стержень Второй
                        Rebar additionalRebarRightCenterDownSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenterDownSecond.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarRightCenterDownSecond = new XYZ(+columnProperties.ColumnSectionHeight / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8)
                            - (squareColumnsReinforcementMainForm.SecondDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterDownSecond.Id, newPlaсeAdditionalRebarRightCenterDownSecond);
                        rebarIdCollection.Add(additionalRebarRightCenterDownSecond.Id);

                        //Центральный правый верхний стержень Второй
                        Rebar additionalRebarRightCenterUpSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenterUpSecond.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarRightCenterUpSecond = new XYZ(+columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8) 
                            + (squareColumnsReinforcementMainForm.SecondUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterUpSecond.Id, newPlaсeAdditionalRebarRightCenterUpSecond);
                        rebarIdCollection.Add(additionalRebarRightCenterUpSecond.Id);

                        //Нижняя грань
                        //Центральный нижний левый стержень Первый
                        Rebar additionalRebarBottomCenterLeftFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarBottomCenterLeftFirst = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterLeftFirst.Id, newPlaсeAdditionalRebarBottomCenterLeftFirst);
                        rebarIdCollection.Add(additionalRebarBottomCenterLeftFirst.Id);

                        //Центральный нижний правый стержень Первый
                        Rebar additionalRebarBottomCenterRightFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarBottomCenterRightFirst = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterRightFirst.Id, newPlaсeAdditionalRebarBottomCenterRightFirst);
                        rebarIdCollection.Add(additionalRebarBottomCenterRightFirst.Id);

                        //Центральный нижний левый стержень Второй
                        Rebar additionalRebarBottomCenterLeftSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarBottomCenterLeftSecond = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                            - (squareColumnsReinforcementMainForm.SecondLeftRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterLeftSecond.Id, newPlaсeAdditionalRebarBottomCenterLeftSecond);
                        rebarIdCollection.Add(additionalRebarBottomCenterLeftSecond.Id);

                        //Центральный нижний правый стержень Второй
                        Rebar additionalRebarBottomCenterRightSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarBottomCenterRightSecond = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8) 
                            + (squareColumnsReinforcementMainForm.SecondRightRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterRightSecond.Id, newPlaсeAdditionalRebarBottomCenterRightSecond);
                        rebarIdCollection.Add(additionalRebarBottomCenterRightSecond.Id);

                        //Верхняя грань
                        //Центральный верхний левый стержень Первый
                        Rebar additionalRebarTopCenterLeftFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarTopCenterLeftFirst = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterLeftFirst.Id, newPlaсeAdditionalRebarTopCenterLeftFirst);
                        rebarIdCollection.Add(additionalRebarTopCenterLeftFirst.Id);

                        //Центральный верхний правый стержень Первый
                        Rebar additionalRebarTopCenterRightFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarTopCenterRightFirst = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterRightFirst.Id, newPlaсeAdditionalRebarTopCenterRightFirst);
                        rebarIdCollection.Add(additionalRebarTopCenterRightFirst.Id);

                        //Центральный верхний левый стержень Второй
                        Rebar additionalRebarTopCenterLeftSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarTopCenterLeftSecond = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                            - (squareColumnsReinforcementMainForm.SecondLeftRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterLeftSecond.Id, newPlaсeAdditionalRebarTopCenterLeftSecond);
                        rebarIdCollection.Add(additionalRebarTopCenterLeftSecond.Id);

                        //Центральный верхний правый стержень Второй
                        Rebar additionalRebarTopCenterRightSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarTopCenterRightSecond = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            + (squareColumnsReinforcementMainForm.SecondRightRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterRightSecond.Id, newPlaсeAdditionalRebarTopCenterRightSecond);
                        rebarIdCollection.Add(additionalRebarTopCenterRightSecond.Id);
                    }

                    //Если стыковка стержней в нахлест с загибом в плиту
                    else if (squareColumnsReinforcementMainForm.CheckedRebarOutletsTepeName == "radioButton_Overlapping"
                        & squareColumnsReinforcementMainForm.СhangeSection == false
                        & squareColumnsReinforcementMainForm.BendIntoSlab == true)
                    {
                        //Точки для построения кривых основных угловых стержней
                        XYZ rebar_p1 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z 
                            - (squareColumnsReinforcementMainForm.RebarDeepeningOffset / 304.8), 6));
                        XYZ rebar_p2 = new XYZ(Math.Round(rebar_p1.X, 6)
                            , Math.Round(rebar_p1.Y, 6)
                            , Math.Round(rebar_p1.Z 
                            + (squareColumnsReinforcementMainForm.RebarDeepeningOffset / 304.8)
                            + columnProperties.ColumnLength
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)
                            - (squareColumnsReinforcementMainForm.BendIntoSlabOffset / 304.8)
                            - (squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2), 6));
                        XYZ rebar_p3 = new XYZ(Math.Round(rebar_p2.X
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8) 
                            - ((squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)
                            - (squareColumnsReinforcementMainForm.BendIntoSlabOffset / 304.8)
                            + (squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2)), 6)
                            , Math.Round(rebar_p2.Y, 6)
                            , Math.Round(rebar_p2.Z, 6));

                        //Точки для построения кривых основных боковых стержней
                        XYZ additionalRebar_p1 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z 
                            - (squareColumnsReinforcementMainForm.RebarDeepeningOffset / 304.8), 6));
                        XYZ additionalRebar_p2 = new XYZ(Math.Round(additionalRebar_p1.X, 6)
                            , Math.Round(additionalRebar_p1.Y, 6)
                            , Math.Round(additionalRebar_p1.Z 
                            + (squareColumnsReinforcementMainForm.RebarDeepeningOffset / 304.8)
                            + columnProperties.ColumnLength 
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)
                            - (squareColumnsReinforcementMainForm.BendIntoSlabOffset / 304.8)
                            - (squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2), 6));
                        XYZ additionalRebar_p3 = new XYZ(Math.Round(additionalRebar_p2.X 
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8)
                            - ((squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)
                            - (squareColumnsReinforcementMainForm.BendIntoSlabOffset / 304.8)
                            + (squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2)), 6)
                            , Math.Round(additionalRebar_p2.Y, 6)
                            , Math.Round(additionalRebar_p2.Z, 6));

                        //Кривые стержня
                        List<Curve> mainRebarLongCurves = new List<Curve>();
                        List<Curve> additionalRebarShortCurves = new List<Curve>();

                        Curve line1L = Line.CreateBound(rebar_p1, rebar_p2) as Curve;
                        mainRebarLongCurves.Add(line1L);
                        Curve line2L = Line.CreateBound(rebar_p2, rebar_p3) as Curve;
                        mainRebarLongCurves.Add(line2L);

                        Curve additionalLine1S = Line.CreateBound(additionalRebar_p1, additionalRebar_p2) as Curve;
                        additionalRebarShortCurves.Add(additionalLine1S);
                        Curve additionalLine2S = Line.CreateBound(additionalRebar_p2, additionalRebar_p3) as Curve;
                        additionalRebarShortCurves.Add(additionalLine2S);

                        //Нижний левый угол
                        Rebar mainRebarLeftBottom = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.MainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ rotate_p1 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z);
                        XYZ rotate_p2 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z + 1);
                        Line rotateLine = Line.CreateBound(rotate_p1, rotate_p2);
                        ElementTransformUtils.RotateElement(doc, mainRebarLeftBottom.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeMainRebarLeftBottom = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, mainRebarLeftBottom.Id, newPlaсeMainRebarLeftBottom);
                        rebarIdCollection.Add(mainRebarLeftBottom.Id);

                        //Верхний левый угол
                        Rebar mainRebarLeftTop = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.MainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        ElementTransformUtils.RotateElement(doc, mainRebarLeftTop.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeMainRebarLeftTop = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            - squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, mainRebarLeftTop.Id, newPlaсeMainRebarLeftTop);
                        rebarIdCollection.Add(mainRebarLeftTop.Id);

                        //Верхний правый угол
                        Rebar mainRebarRightTop = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.MainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        XYZ newPlaсeMainRebarRightTop = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, mainRebarRightTop.Id, newPlaсeMainRebarRightTop);
                        rebarIdCollection.Add(mainRebarRightTop.Id);

                        //Нижний правый угол
                        Rebar mainRebarRightBottom = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.MainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        XYZ newPlaсeMainRebarRightBottom = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            - squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2
                            , -columnProperties.ColumnSectionWidth / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, mainRebarRightBottom.Id, newPlaсeMainRebarRightBottom);
                        rebarIdCollection.Add(mainRebarRightBottom.Id);

                        //Левая грань
                        //Центральный левый нижний стержень Первый
                        Rebar additionalRebarLeftCenterDownFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        ElementTransformUtils.RotateElement(doc, additionalRebarLeftCenterDownFirst.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarLeftCenterDownFirst = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterDownFirst.Id, newPlaсeAdditionalRebarLeftCenterDownFirst);
                        rebarIdCollection.Add(additionalRebarLeftCenterDownFirst.Id);

                        //Центральный левый верхний стержень Первый
                        Rebar additionalRebarLeftCenterUpFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarLeftCenterUpFirst.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarLeftCenterUpFirst = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterUpFirst.Id, newPlaсeAdditionalRebarLeftCenterUpFirst);
                        rebarIdCollection.Add(additionalRebarLeftCenterUpFirst.Id);

                        //Центральный левый нижний стержень Второй
                        Rebar additionalRebarLeftCenterDownSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarLeftCenterDownSecond.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarLeftCenterDownSecond = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8)
                            - (squareColumnsReinforcementMainForm.SecondDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterDownSecond.Id, newPlaсeAdditionalRebarLeftCenterDownSecond);
                        rebarIdCollection.Add(additionalRebarLeftCenterDownSecond.Id);

                        //Центральный левый верхний стержень Второй
                        Rebar additionalRebarLeftCenterUpSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarLeftCenterUpSecond.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarLeftCenterUpSecond = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8)
                            + (squareColumnsReinforcementMainForm.SecondUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterUpSecond.Id, newPlaсeAdditionalRebarLeftCenterUpSecond);
                        rebarIdCollection.Add(additionalRebarLeftCenterUpSecond.Id);

                        //Правая грань
                        //Центральный правый нижний стержень Первый
                        Rebar additionalRebarRightCenterDownFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        XYZ newPlaсeAdditionalRebarRightCenterDownFirst = new XYZ(+columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterDownFirst.Id, newPlaсeAdditionalRebarRightCenterDownFirst);
                        rebarIdCollection.Add(additionalRebarRightCenterDownFirst.Id);

                        //Центральный правый верхний стержень Первый
                        Rebar additionalRebarRightCenterUpFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        XYZ newPlaсeAdditionalRebarRightCenterUpFirst = new XYZ(+columnProperties.ColumnSectionHeight / 2 - squareColumnsReinforcementMainForm.MainRebarCoverLayer - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterUpFirst.Id, newPlaсeAdditionalRebarRightCenterUpFirst);
                        rebarIdCollection.Add(additionalRebarRightCenterUpFirst.Id);

                        //Центральный правый нижний стержень Второй
                        Rebar additionalRebarRightCenterDownSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        XYZ newPlaсeAdditionalRebarRightCenterDownSecond = new XYZ(+columnProperties.ColumnSectionHeight / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8)
                            - (squareColumnsReinforcementMainForm.SecondDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterDownSecond.Id, newPlaсeAdditionalRebarRightCenterDownSecond);
                        rebarIdCollection.Add(additionalRebarRightCenterDownSecond.Id);

                        //Центральный правый верхний стержень Второй
                        Rebar additionalRebarRightCenterUpSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        XYZ newPlaсeAdditionalRebarRightCenterUpSecond = new XYZ(+columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8) 
                            + (squareColumnsReinforcementMainForm.SecondUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterUpSecond.Id, newPlaсeAdditionalRebarRightCenterUpSecond);
                        rebarIdCollection.Add(additionalRebarRightCenterUpSecond.Id);

                        //Нижняя грань
                        //Центральный нижний левый стержень Первый
                        Rebar additionalRebarBottomCenterLeftFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenterLeftFirst.Id, rotateLine, -90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarBottomCenterLeftFirst = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterLeftFirst.Id, newPlaсeAdditionalRebarBottomCenterLeftFirst);
                        rebarIdCollection.Add(additionalRebarBottomCenterLeftFirst.Id);

                        //Центральный нижний правый стержень Первый
                        Rebar additionalRebarBottomCenterRightFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenterRightFirst.Id, rotateLine, -90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarBottomCenterRightFirst = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterRightFirst.Id, newPlaсeAdditionalRebarBottomCenterRightFirst);
                        rebarIdCollection.Add(additionalRebarBottomCenterRightFirst.Id);

                        //Центральный нижний левый стержень Второй
                        Rebar additionalRebarBottomCenterLeftSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenterLeftSecond.Id, rotateLine, -90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarBottomCenterLeftSecond = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                            - (squareColumnsReinforcementMainForm.SecondLeftRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterLeftSecond.Id, newPlaсeAdditionalRebarBottomCenterLeftSecond);
                        rebarIdCollection.Add(additionalRebarBottomCenterLeftSecond.Id);

                        //Центральный нижний правый стержень Второй
                        Rebar additionalRebarBottomCenterRightSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenterRightSecond.Id, rotateLine, -90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarBottomCenterRightSecond = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8) 
                            + (squareColumnsReinforcementMainForm.SecondRightRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterRightSecond.Id, newPlaсeAdditionalRebarBottomCenterRightSecond);
                        rebarIdCollection.Add(additionalRebarBottomCenterRightSecond.Id);

                        //Верхняя грань
                        //Центральный верхний левый стержень Первый
                        Rebar additionalRebarTopCenterLeftFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenterLeftFirst.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarTopCenterLeftFirst = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterLeftFirst.Id, newPlaсeAdditionalRebarTopCenterLeftFirst);
                        rebarIdCollection.Add(additionalRebarTopCenterLeftFirst.Id);

                        //Центральный верхний правый стержень Первый
                        Rebar additionalRebarTopCenterRightFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenterRightFirst.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarTopCenterRightFirst = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterRightFirst.Id, newPlaсeAdditionalRebarTopCenterRightFirst);
                        rebarIdCollection.Add(additionalRebarTopCenterRightFirst.Id);

                        //Центральный верхний левый стержень Второй
                        Rebar additionalRebarTopCenterLeftSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenterLeftSecond.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarTopCenterLeftSecond = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                            - (squareColumnsReinforcementMainForm.SecondLeftRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterLeftSecond.Id, newPlaсeAdditionalRebarTopCenterLeftSecond);
                        rebarIdCollection.Add(additionalRebarTopCenterLeftSecond.Id);

                        //Центральный верхний правый стержень Второй
                        Rebar additionalRebarTopCenterRightSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenterRightSecond.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarTopCenterRightSecond = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8) 
                            + (squareColumnsReinforcementMainForm.SecondRightRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterRightSecond.Id, newPlaсeAdditionalRebarTopCenterRightSecond);
                        rebarIdCollection.Add(additionalRebarTopCenterRightSecond.Id);
                    }

                    //Если стыковка стержней в нахлест c изменением сечения колонны выше
                    else if (squareColumnsReinforcementMainForm.CheckedRebarOutletsTepeName == "radioButton_Overlapping"
                        & squareColumnsReinforcementMainForm.СhangeSection == true
                        & squareColumnsReinforcementMainForm.СhangeSectionOffset <= 50)
                    {
                        //Точки для построения кривых основных угловых удлиненных стержней
                        XYZ rebar_p1L = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z 
                            - (squareColumnsReinforcementMainForm.RebarDeepeningOffset / 304.8), 6));
                        XYZ rebar_p2L = new XYZ(Math.Round(rebar_p1L.X, 6)
                            , Math.Round(rebar_p1L.Y, 6)
                            , Math.Round(rebar_p1L.Z 
                            + (squareColumnsReinforcementMainForm.RebarDeepeningOffset / 304.8) 
                            + columnProperties.ColumnLength 
                            - ((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8) * 6 
                            - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)), 6));
                        XYZ rebar_p3L = new XYZ(Math.Round(rebar_p2L.X 
                            + deltaXMainRebarOverlapping, 6)
                            , Math.Round(rebar_p2L.Y, 6)
                            , Math.Round(rebar_p2L.Z 
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8) 
                            + ((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8) * 6 
                            - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)), 6));
                        XYZ rebar_p4L = new XYZ(Math.Round(rebar_p3L.X, 6)
                            , Math.Round(rebar_p3L.Y, 6)
                            , Math.Round(rebar_p3L.Z 
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));

                        //Точки для построения кривых основных угловых укороченных стержней
                        XYZ rebar_p1S = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z
                            - (squareColumnsReinforcementMainForm.RebarDeepeningOffset / 304.8), 6));
                        XYZ rebar_p2S = new XYZ(Math.Round(rebar_p1S.X, 6)
                            , Math.Round(rebar_p1S.Y, 6)
                            , Math.Round(rebar_p1S.Z
                            + (squareColumnsReinforcementMainForm.RebarDeepeningOffset / 304.8) 
                            + columnProperties.ColumnLength 
                            - ((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8) * 6 
                            - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)), 6));
                        XYZ rebar_p3S = new XYZ(Math.Round(rebar_p2S.X 
                            + deltaXMainRebarOverlapping, 6)
                            , Math.Round(rebar_p2S.Y, 6)
                            , Math.Round(rebar_p2S.Z 
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8) 
                            + ((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8) * 6 
                            - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)), 6));
                        XYZ rebar_p4S = new XYZ(Math.Round(rebar_p3S.X, 6)
                            , Math.Round(rebar_p3S.Y, 6)
                            , Math.Round(rebar_p3S.Z 
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8), 6));

                        //Точки для построения кривых основных боковых удлиненных стержней
                        XYZ additionalRebar_p1L = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z 
                            - (squareColumnsReinforcementMainForm.RebarDeepeningOffset / 304.8), 6));
                        XYZ additionalRebar_p2L = new XYZ(Math.Round(additionalRebar_p1L.X, 6)
                            , Math.Round(additionalRebar_p1L.Y, 6)
                            , Math.Round(additionalRebar_p1L.Z
                            + (squareColumnsReinforcementMainForm.RebarDeepeningOffset / 304.8)
                            + columnProperties.ColumnLength
                            - ((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8) * 6
                            - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)), 6));
                        XYZ additionalRebar_p3L = new XYZ(Math.Round(additionalRebar_p2L.X
                            + deltaXAdditionalMainRebarOverlapping, 6)
                            , Math.Round(additionalRebar_p2L.Y, 6)
                            , Math.Round(additionalRebar_p2L.Z 
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8) 
                            + ((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8) * 6
                            - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)), 6));
                        XYZ additionalRebar_p4L = new XYZ(Math.Round(additionalRebar_p3L.X, 6)
                            , Math.Round(additionalRebar_p3L.Y, 6)
                            , Math.Round(additionalRebar_p3L.Z 
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));

                        //Точки для построения кривых основных боковых укороченных стержней
                        XYZ additionalRebar_p1S = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z
                            - (squareColumnsReinforcementMainForm.RebarDeepeningOffset / 304.8), 6));
                        XYZ additionalRebar_p2S = new XYZ(Math.Round(additionalRebar_p1S.X, 6)
                            , Math.Round(additionalRebar_p1S.Y, 6)
                            , Math.Round(additionalRebar_p1S.Z
                            + (squareColumnsReinforcementMainForm.RebarDeepeningOffset / 304.8)
                            + columnProperties.ColumnLength
                            - ((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8) * 6
                            - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)), 6));
                        XYZ additionalRebar_p3S = new XYZ(Math.Round(additionalRebar_p2S.X 
                            + deltaXAdditionalMainRebarOverlapping, 6)
                            , Math.Round(additionalRebar_p2S.Y, 6)
                            , Math.Round(additionalRebar_p2S.Z
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8) 
                            + ((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8) * 6 
                            - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)), 6));
                        XYZ additionalRebar_p4S = new XYZ(Math.Round(additionalRebar_p3S.X, 6)
                            , Math.Round(additionalRebar_p3S.Y, 6)
                            , Math.Round(additionalRebar_p3S.Z 
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8), 6));

                        //Кривые стержня
                        List<Curve> mainRebarLongCurves = new List<Curve>();
                        List<Curve> mainRebarShortCurves = new List<Curve>();
                        List<Curve> additionalRebarLongCurves = new List<Curve>();
                        List<Curve> additionalRebarShortCurves = new List<Curve>();

                        Curve line1L = Line.CreateBound(rebar_p1L, rebar_p2L) as Curve;
                        mainRebarLongCurves.Add(line1L);
                        Curve line2L = Line.CreateBound(rebar_p2L, rebar_p3L) as Curve;
                        mainRebarLongCurves.Add(line2L);
                        Curve line3L = Line.CreateBound(rebar_p3L, rebar_p4L) as Curve;
                        mainRebarLongCurves.Add(line3L);

                        Curve line1S = Line.CreateBound(rebar_p1S, rebar_p2S) as Curve;
                        mainRebarShortCurves.Add(line1S);
                        Curve line2S = Line.CreateBound(rebar_p2S, rebar_p3S) as Curve;
                        mainRebarShortCurves.Add(line2S);
                        Curve line3S = Line.CreateBound(rebar_p3S, rebar_p4S) as Curve;
                        mainRebarShortCurves.Add(line3S);

                        Curve additionalLine1L = Line.CreateBound(additionalRebar_p1L, additionalRebar_p2L) as Curve;
                        additionalRebarLongCurves.Add(additionalLine1L);
                        Curve additionalLine2L = Line.CreateBound(additionalRebar_p2L, additionalRebar_p3L) as Curve;
                        additionalRebarLongCurves.Add(additionalLine2L);
                        Curve additionalLine3L = Line.CreateBound(additionalRebar_p3L, additionalRebar_p4L) as Curve;
                        additionalRebarLongCurves.Add(additionalLine3L);

                        Curve additionalLine1S = Line.CreateBound(additionalRebar_p1S, additionalRebar_p2S) as Curve;
                        additionalRebarShortCurves.Add(additionalLine1S);
                        Curve additionalLine2S = Line.CreateBound(additionalRebar_p2S, additionalRebar_p3S) as Curve;
                        additionalRebarShortCurves.Add(additionalLine2S);
                        Curve additionalLine3S = Line.CreateBound(additionalRebar_p3S, additionalRebar_p4S) as Curve;
                        additionalRebarShortCurves.Add(additionalLine3S);

                        //Нижний левый угол
                        Rebar mainRebarLeftBottom = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.MainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ rotate1_p1 = new XYZ(rebar_p1L.X, rebar_p1L.Y, rebar_p1L.Z);
                        XYZ rotate1_p2 = new XYZ(rebar_p1L.X, rebar_p1L.Y, rebar_p1L.Z + 1);
                        Line rotateLine = Line.CreateBound(rotate1_p1, rotate1_p2);
                        ElementTransformUtils.RotateElement(doc, mainRebarLeftBottom.Id, rotateLine, deltaXMainRebarOverlapping);
                        XYZ newPlaсeMainRebarLeftBottom = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, mainRebarLeftBottom.Id, newPlaсeMainRebarLeftBottom);
                        rebarIdCollection.Add(mainRebarLeftBottom.Id);

                        //Верхний левый угол
                        Rebar mainRebarLeftTop = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.MainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, mainRebarLeftTop.Id, rotateLine, -deltaXMainRebarOverlapping);
                        XYZ newPlaсeMainRebarLeftTop = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2
                            , columnProperties.ColumnSectionWidth / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, mainRebarLeftTop.Id, newPlaсeMainRebarLeftTop);
                        rebarIdCollection.Add(mainRebarLeftTop.Id);

                        //Верхний правый угол
                        Rebar mainRebarRightTop = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.MainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, mainRebarRightTop.Id, rotateLine, deltaXMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, mainRebarRightTop.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeMainRebarRightTop = new XYZ(columnProperties.ColumnSectionHeight / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            - squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, mainRebarRightTop.Id, newPlaсeMainRebarRightTop);
                        rebarIdCollection.Add(mainRebarRightTop.Id);

                        //Нижний правый угол
                        Rebar mainRebarRightBottom = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.MainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, mainRebarRightBottom.Id, rotateLine, -deltaXMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, mainRebarRightBottom.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeMainRebarRightBottom = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2
                            , -columnProperties.ColumnSectionWidth / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, mainRebarRightBottom.Id, newPlaсeMainRebarRightBottom);
                        rebarIdCollection.Add(mainRebarRightBottom.Id);

                        //Нижний левый угол
                        //Центральный левый нижний стержень Первый
                        Rebar additionalRebarLeftCenterDownFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarLeftCenterDownFirst.Id, rotateLine, alphaAdditionalMainRebarOverlapping);
                        XYZ newPlaсeAdditionalRebarLeftCenterDownFirst = new XYZ(-columnProperties.ColumnSectionHeight / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterDownFirst.Id, newPlaсeAdditionalRebarLeftCenterDownFirst);
                        rebarIdCollection.Add(additionalRebarLeftCenterDownFirst.Id);

                        //Центральный левый верхний стержень Первый
                        Rebar additionalRebarLeftCenterUpFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarLeftCenterUpFirst.Id, rotateLine, alphaAdditionalMainRebarOverlapping);
                        XYZ newPlaсeAdditionalRebarLeftCenterUpFirst = new XYZ(-columnProperties.ColumnSectionHeight / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterUpFirst.Id, newPlaсeAdditionalRebarLeftCenterUpFirst);
                        rebarIdCollection.Add(additionalRebarLeftCenterUpFirst.Id);

                        //Центральный левый нижний стержень Второй
                        Rebar additionalRebarLeftCenterDownSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarLeftCenterDownSecond.Id, rotateLine, alphaAdditionalMainRebarOverlapping);
                        XYZ newPlaсeAdditionalRebarLeftCenterDownSecond = new XYZ(-columnProperties.ColumnSectionHeight / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8) 
                            - (squareColumnsReinforcementMainForm.SecondDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterDownSecond.Id, newPlaсeAdditionalRebarLeftCenterDownSecond);
                        rebarIdCollection.Add(additionalRebarLeftCenterDownSecond.Id);

                        //Центральный левый верхний стержень Второй
                        Rebar additionalRebarLeftCenterUpSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarLeftCenterUpSecond.Id, rotateLine, alphaAdditionalMainRebarOverlapping);
                        XYZ newPlaсeAdditionalRebarLeftCenterUpSecond = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8) 
                            + (squareColumnsReinforcementMainForm.SecondUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterUpSecond.Id, newPlaсeAdditionalRebarLeftCenterUpSecond);
                        rebarIdCollection.Add(additionalRebarLeftCenterUpSecond.Id);

                        //Правая грань
                        //Центральный правый нижний стержень Первый
                        Rebar additionalRebarRightCenterDownFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenterDownFirst.Id, rotateLine, -alphaAdditionalMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenterDownFirst.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarRightCenterDownFirst = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterDownFirst.Id, newPlaсeAdditionalRebarRightCenterDownFirst);
                        rebarIdCollection.Add(additionalRebarRightCenterDownFirst.Id);

                        //Центральный правый верхний стержень Первый
                        Rebar additionalRebarRightCenterUpFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenterUpFirst.Id, rotateLine, -alphaAdditionalMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenterUpFirst.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarRightCenterUpFirst = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterUpFirst.Id, newPlaсeAdditionalRebarRightCenterUpFirst);
                        rebarIdCollection.Add(additionalRebarRightCenterUpFirst.Id);

                        //Центральный правый нижний стержень Второй
                        Rebar additionalRebarRightCenterDownSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenterDownSecond.Id, rotateLine, -alphaAdditionalMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenterDownSecond.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarRightCenterDownSecond = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8) 
                            - (squareColumnsReinforcementMainForm.SecondDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterDownSecond.Id, newPlaсeAdditionalRebarRightCenterDownSecond);
                        rebarIdCollection.Add(additionalRebarRightCenterDownSecond.Id);

                        //Центральный правый верхний стержень Второй
                        Rebar additionalRebarRightCenterUpSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenterUpSecond.Id, rotateLine, -alphaAdditionalMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenterUpSecond.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarRightCenterUpSecond = new XYZ(columnProperties.ColumnSectionHeight / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8) 
                            + (squareColumnsReinforcementMainForm.SecondUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterUpSecond.Id, newPlaсeAdditionalRebarRightCenterUpSecond);
                        rebarIdCollection.Add(additionalRebarRightCenterUpSecond.Id);

                        //Нижняя грань
                        //Центральный нижний левый стержень Первый
                        Rebar additionalRebarBottomCenterLeftFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenterLeftFirst.Id, rotateLine, -alphaAdditionalMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenterLeftFirst.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarBottomCenterLeftFirst = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterLeftFirst.Id, newPlaсeAdditionalRebarBottomCenterLeftFirst);
                        rebarIdCollection.Add(additionalRebarBottomCenterLeftFirst.Id);

                        //Центральный нижний правый стержень Первый
                        Rebar additionalRebarBottomCenterRightFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenterRightFirst.Id, rotateLine, -alphaAdditionalMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenterRightFirst.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarBottomCenterRightFirst = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterRightFirst.Id, newPlaсeAdditionalRebarBottomCenterRightFirst);
                        rebarIdCollection.Add(additionalRebarBottomCenterRightFirst.Id);

                        //Центральный нижний левый стержень Второй
                        Rebar additionalRebarBottomCenterLeftSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenterLeftSecond.Id, rotateLine, -alphaAdditionalMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenterLeftSecond.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarBottomCenterLeftSecond = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                            - (squareColumnsReinforcementMainForm.SecondLeftRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterLeftSecond.Id, newPlaсeAdditionalRebarBottomCenterLeftSecond);
                        rebarIdCollection.Add(additionalRebarBottomCenterLeftSecond.Id);

                        //Центральный нижний правый стержень Второй
                        Rebar additionalRebarBottomCenterRightSecond = Rebar.CreateFromCurvesAndShape(doc, rebarShapeCollector.Shape26, squareColumnsReinforcementMainForm.AdditionalMainRebarTape, null, null, column, mainRebarNormal, additionalRebarLongCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenterRightSecond.Id, rotateLine, -alphaAdditionalMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenterRightSecond.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarBottomCenterRightSecond = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            + (squareColumnsReinforcementMainForm.SecondRightRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterRightSecond.Id, newPlaсeAdditionalRebarBottomCenterRightSecond);
                        rebarIdCollection.Add(additionalRebarBottomCenterRightSecond.Id);

                        //Верхняя грань
                        //Центральный верхний левый стержень Первый
                        Rebar additionalRebarTopCenterLeftFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenterLeftFirst.Id, rotateLine, alphaAdditionalMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenterLeftFirst.Id, rotateLine, -90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarTopCenterLeftFirst = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterLeftFirst.Id, newPlaсeAdditionalRebarTopCenterLeftFirst);
                        rebarIdCollection.Add(additionalRebarTopCenterLeftFirst.Id);

                        //Центральный верхний правый стержень Первый
                        Rebar additionalRebarTopCenterRightFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenterRightFirst.Id, rotateLine, alphaAdditionalMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenterRightFirst.Id, rotateLine, -90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarTopCenterRightFirst = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterRightFirst.Id, newPlaсeAdditionalRebarTopCenterRightFirst);
                        rebarIdCollection.Add(additionalRebarTopCenterRightFirst.Id);

                        //Центральный верхний левый стержень Второй
                        Rebar additionalRebarTopCenterLeftSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenterLeftSecond.Id, rotateLine, alphaAdditionalMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenterLeftSecond.Id, rotateLine, -90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarTopCenterLeftSecond = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8) 
                            - (squareColumnsReinforcementMainForm.SecondLeftRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterLeftSecond.Id, newPlaсeAdditionalRebarTopCenterLeftSecond);
                        rebarIdCollection.Add(additionalRebarTopCenterLeftSecond.Id);

                        //Центральный верхний правый стержень Второй
                        Rebar additionalRebarTopCenterRightSecond = Rebar.CreateFromCurvesAndShape(doc, rebarShapeCollector.Shape26, squareColumnsReinforcementMainForm.AdditionalMainRebarTape, null, null, column, mainRebarNormal, additionalRebarShortCurves, RebarHookOrientation.Right, RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenterRightSecond.Id, rotateLine, alphaAdditionalMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenterRightSecond.Id, rotateLine, -90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarTopCenterRightSecond = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            + (squareColumnsReinforcementMainForm.SecondRightRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterRightSecond.Id, newPlaсeAdditionalRebarTopCenterRightSecond);
                        rebarIdCollection.Add(additionalRebarTopCenterRightSecond.Id);
                    }

                    //Если стыковка стержней на сварке без изменения сечения колонны выше
                    else if (squareColumnsReinforcementMainForm.CheckedRebarOutletsTepeName == "radioButton_Welding"
                        & squareColumnsReinforcementMainForm.СhangeSection == false
                        & squareColumnsReinforcementMainForm.TransitionToOverlap == false
                        & squareColumnsReinforcementMainForm.BendIntoSlab == false)
                    {
                        FamilySymbol tubWeldingSymbol = rebarShapeCollector.GetTubWeldingFamilySymbol();
                        if (tubWeldingSymbol == null)
                        {
                            TaskDialog.Show("Revit", "Семейство CIT_04_ВаннаДляСварки не найдено");
                            return Result.Cancelled;
                        }

                        //Точки для построения кривых удлиненных стержней 
                        XYZ rebar_p1 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));
                        XYZ rebar_p2 = new XYZ(Math.Round(rebar_p1.X, 6)
                            , Math.Round(rebar_p1.Y, 6)
                            , Math.Round(rebar_p1.Z 
                            + columnProperties.ColumnLength  
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8), 6));

                        //Точки для построения кривых укороченных стержней
                        XYZ additionalRebar_p1 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8), 6));
                        XYZ additionalRebar_p2 = new XYZ(Math.Round(additionalRebar_p1.X, 6)
                            , Math.Round(additionalRebar_p1.Y, 6)
                            , Math.Round(additionalRebar_p1.Z 
                            + columnProperties.ColumnLength  
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8), 6));

                        XYZ tubWelding_p1L = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , columnProperties.ColumnLength
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8)
                            + columnProperties.СolumnBaseLevelOffset);
                        XYZ tubWelding_p1S = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , columnProperties.ColumnLength
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8)
                            + columnProperties.СolumnBaseLevelOffset);

                        //Кривые стержня
                        List<Curve> mainRebarLongCurves = new List<Curve>();
                        List<Curve> additionalRebarShortCurves = new List<Curve>();

                        Curve line1L = Line.CreateBound(rebar_p1, rebar_p2) as Curve;
                        mainRebarLongCurves.Add(line1L);

                        Curve additionalLine1S = Line.CreateBound(additionalRebar_p1, additionalRebar_p2) as Curve;
                        additionalRebarShortCurves.Add(additionalLine1S);

                        //Нижний левый угол
                        Rebar mainRebarLeftBottom = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape01
                            , squareColumnsReinforcementMainForm.MainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeMainRebarLeftBottom = new XYZ(-columnProperties.ColumnSectionHeight / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2
                            , -columnProperties.ColumnSectionWidth / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, mainRebarLeftBottom.Id, newPlaсeMainRebarLeftBottom);
                        rebarIdCollection.Add(mainRebarLeftBottom.Id);

                        FamilyInstance tubWeldingMainRebarLeftBottom = doc.Create.NewFamilyInstance(tubWelding_p1L
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingMainRebarLeftBottom.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.MainRebarTapeDiam);
                        ElementTransformUtils.MoveElement(doc, tubWeldingMainRebarLeftBottom.Id, newPlaсeMainRebarLeftBottom);
                        rebarIdCollection.Add(tubWeldingMainRebarLeftBottom.Id);

                        //Верхний левый угол
                        Rebar mainRebarLeftTop = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape01
                            , squareColumnsReinforcementMainForm.MainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeMainRebarLeftTop = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, mainRebarLeftTop.Id, newPlaсeMainRebarLeftTop);
                        rebarIdCollection.Add(mainRebarLeftTop.Id);

                        FamilyInstance tubWeldingMainRebarLeftTop = doc.Create.NewFamilyInstance(tubWelding_p1S
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingMainRebarLeftTop.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.MainRebarTapeDiam);
                        ElementTransformUtils.MoveElement(doc, tubWeldingMainRebarLeftTop.Id, newPlaсeMainRebarLeftTop);
                        rebarIdCollection.Add(tubWeldingMainRebarLeftTop.Id);

                        //Верхний правый угол
                        Rebar mainRebarRightTop = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape01
                            , squareColumnsReinforcementMainForm.MainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeMainRebarRightTop = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            - squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, mainRebarRightTop.Id, newPlaсeMainRebarRightTop);
                        rebarIdCollection.Add(mainRebarRightTop.Id);

                        FamilyInstance tubWeldingMainRebarRightTop = doc.Create.NewFamilyInstance(tubWelding_p1L
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingMainRebarRightTop.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.MainRebarTapeDiam);
                        ElementTransformUtils.MoveElement(doc, tubWeldingMainRebarRightTop.Id, newPlaсeMainRebarRightTop);
                        rebarIdCollection.Add(tubWeldingMainRebarRightTop.Id);

                        //Нижний правый угол
                        Rebar mainRebarRightBottom = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape01
                            , squareColumnsReinforcementMainForm.MainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeMainRebarRightBottom = new XYZ(columnProperties.ColumnSectionHeight / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            - squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2
                            , -columnProperties.ColumnSectionWidth / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, mainRebarRightBottom.Id, newPlaсeMainRebarRightBottom);
                        rebarIdCollection.Add(mainRebarRightBottom.Id);

                        FamilyInstance tubWeldingMainRebarRightBottom = doc.Create.NewFamilyInstance(tubWelding_p1S
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingMainRebarRightBottom.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.MainRebarTapeDiam);
                        ElementTransformUtils.MoveElement(doc, tubWeldingMainRebarRightBottom.Id, newPlaсeMainRebarRightBottom);
                        rebarIdCollection.Add(tubWeldingMainRebarRightBottom.Id);

                        //Левая грань
                        //Центральный левый нижний стержень Первый
                        Rebar additionalRebarLeftCenterDownFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape01
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarLeftCenterDownFirst = new XYZ(-columnProperties.ColumnSectionHeight / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterDownFirst.Id, newPlaсeAdditionalRebarLeftCenterDownFirst);
                        rebarIdCollection.Add(additionalRebarLeftCenterDownFirst.Id);

                        FamilyInstance tubWeldingAdditionalRebarLeftCenterDownFirst = doc.Create.NewFamilyInstance(tubWelding_p1L
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarLeftCenterDownFirst.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarLeftCenterDownFirst.Id, newPlaсeAdditionalRebarLeftCenterDownFirst);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarLeftCenterDownFirst.Id);

                        //Центральный левый верхний стержень Первый
                        Rebar additionalRebarLeftCenterUpFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape01
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarLeftCenterUpFirst = new XYZ(-columnProperties.ColumnSectionHeight / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterUpFirst.Id, newPlaсeAdditionalRebarLeftCenterUpFirst);
                        rebarIdCollection.Add(additionalRebarLeftCenterUpFirst.Id);

                        FamilyInstance tubWeldingAdditionalRebarLeftCenterUpFirst = doc.Create.NewFamilyInstance(tubWelding_p1S
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarLeftCenterUpFirst.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarLeftCenterUpFirst.Id, newPlaсeAdditionalRebarLeftCenterUpFirst);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarLeftCenterUpFirst.Id);

                        //Центральный левый нижний стержень Второй
                        Rebar additionalRebarLeftCenterDownSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape01
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarLeftCenterDownSecond = new XYZ(-columnProperties.ColumnSectionHeight / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8)
                            - (squareColumnsReinforcementMainForm.SecondDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterDownSecond.Id, newPlaсeAdditionalRebarLeftCenterDownSecond);
                        rebarIdCollection.Add(additionalRebarLeftCenterDownSecond.Id);

                        FamilyInstance tubWeldingAdditionalRebarLeftCenterDownSecond = doc.Create.NewFamilyInstance(tubWelding_p1S
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarLeftCenterDownSecond.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarLeftCenterDownSecond.Id, newPlaсeAdditionalRebarLeftCenterDownSecond);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarLeftCenterDownSecond.Id);

                        //Центральный левый верхний стержень Второй
                        Rebar additionalRebarLeftCenterUpSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape01
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarLeftCenterUpSecond = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8)
                            + (squareColumnsReinforcementMainForm.SecondUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterUpSecond.Id, newPlaсeAdditionalRebarLeftCenterUpSecond);
                        rebarIdCollection.Add(additionalRebarLeftCenterUpSecond.Id);

                        FamilyInstance tubWeldingAdditionalRebarLeftCenterUpSecond = doc.Create.NewFamilyInstance(tubWelding_p1L
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarLeftCenterUpSecond.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarLeftCenterUpSecond.Id, newPlaсeAdditionalRebarLeftCenterUpSecond);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarLeftCenterUpSecond.Id);

                        //Правая грань
                        //Центральный правый нижний стержень Первый
                        Rebar additionalRebarRightCenterDownFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape01
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarRightCenterDownFirst = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterDownFirst.Id, newPlaсeAdditionalRebarRightCenterDownFirst);
                        rebarIdCollection.Add(additionalRebarRightCenterDownFirst.Id);

                        FamilyInstance tubWeldingAdditionalRebarRightCenterDownFirst = doc.Create.NewFamilyInstance(tubWelding_p1S
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarRightCenterDownFirst.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarRightCenterDownFirst.Id, newPlaсeAdditionalRebarRightCenterDownFirst);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarRightCenterDownFirst.Id);

                        //Центральный правый верхний стержень Первый
                        Rebar additionalRebarRightCenterUpFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape01
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarRightCenterUpFirst = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterUpFirst.Id, newPlaсeAdditionalRebarRightCenterUpFirst);
                        rebarIdCollection.Add(additionalRebarRightCenterUpFirst.Id);

                        FamilyInstance tubWeldingAdditionalRebarRightCenterUpFirst = doc.Create.NewFamilyInstance(tubWelding_p1L
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarRightCenterUpFirst.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarRightCenterUpFirst.Id, newPlaсeAdditionalRebarRightCenterUpFirst);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarRightCenterUpFirst.Id);

                        //Центральный правый нижний стержень Второй
                        Rebar additionalRebarRightCenterDownSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape01
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarRightCenterDownSecond = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8) 
                            - (squareColumnsReinforcementMainForm.SecondDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterDownSecond.Id, newPlaсeAdditionalRebarRightCenterDownSecond);
                        rebarIdCollection.Add(additionalRebarRightCenterDownSecond.Id);

                        FamilyInstance tubWeldingAdditionalRebarRightCenterDownSecond = doc.Create.NewFamilyInstance(tubWelding_p1L
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarRightCenterDownSecond.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarRightCenterDownSecond.Id, newPlaсeAdditionalRebarRightCenterDownSecond);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarRightCenterDownSecond.Id);

                        //Центральный правый верхний стержень Второй
                        Rebar additionalRebarRightCenterUpSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape01
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarRightCenterUpSecond = new XYZ(columnProperties.ColumnSectionHeight / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8)
                            + (squareColumnsReinforcementMainForm.SecondUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterUpSecond.Id, newPlaсeAdditionalRebarRightCenterUpSecond);
                        rebarIdCollection.Add(additionalRebarRightCenterUpSecond.Id);

                        FamilyInstance tubWeldingAdditionalRebarRightCenterUpSecond = doc.Create.NewFamilyInstance(tubWelding_p1S, tubWeldingSymbol, columnProperties.СolumnBaseLevel, StructuralType.NonStructural);
                        tubWeldingAdditionalRebarRightCenterUpSecond.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarRightCenterUpSecond.Id, newPlaсeAdditionalRebarRightCenterUpSecond);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarRightCenterUpSecond.Id);

                        //Нижняя грань
                        //Центральный нижний левый стержень Первый
                        Rebar additionalRebarBottomCenterLeftFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape01
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarBottomCenterLeftFirst = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterLeftFirst.Id, newPlaсeAdditionalRebarBottomCenterLeftFirst);
                        rebarIdCollection.Add(additionalRebarBottomCenterLeftFirst.Id);

                        FamilyInstance tubWeldingAdditionalRebarBottomCenterLeftFirst = doc.Create.NewFamilyInstance(tubWelding_p1L
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarBottomCenterLeftFirst.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarBottomCenterLeftFirst.Id, newPlaсeAdditionalRebarBottomCenterLeftFirst);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarBottomCenterLeftFirst.Id);

                        //Центральный нижний правый стержень Первый
                        Rebar additionalRebarBottomCenterRightFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape01
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarBottomCenterRightFirst = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterRightFirst.Id, newPlaсeAdditionalRebarBottomCenterRightFirst);
                        rebarIdCollection.Add(additionalRebarBottomCenterRightFirst.Id);

                        FamilyInstance tubWeldingAdditionalRebarBottomCenterRightFirst = doc.Create.NewFamilyInstance(tubWelding_p1S, tubWeldingSymbol, columnProperties.СolumnBaseLevel, StructuralType.NonStructural);
                        tubWeldingAdditionalRebarBottomCenterRightFirst.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarBottomCenterRightFirst.Id, newPlaсeAdditionalRebarBottomCenterRightFirst);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarBottomCenterRightFirst.Id);

                        //Центральный нижний левый стержень Второй
                        Rebar additionalRebarBottomCenterLeftSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape01
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarBottomCenterLeftSecond = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8) 
                            - (squareColumnsReinforcementMainForm.SecondLeftRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterLeftSecond.Id, newPlaсeAdditionalRebarBottomCenterLeftSecond);
                        rebarIdCollection.Add(additionalRebarBottomCenterLeftSecond.Id);

                        FamilyInstance tubWeldingAdditionalRebarBottomCenterLeftSecond = doc.Create.NewFamilyInstance(tubWelding_p1S, tubWeldingSymbol, columnProperties.СolumnBaseLevel, StructuralType.NonStructural);
                        tubWeldingAdditionalRebarBottomCenterLeftSecond.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarBottomCenterLeftSecond.Id, newPlaсeAdditionalRebarBottomCenterLeftSecond);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarBottomCenterLeftSecond.Id);

                        //Центральный нижний правый стержень Второй
                        Rebar additionalRebarBottomCenterRightSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape01
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarBottomCenterRightSecond = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            + (squareColumnsReinforcementMainForm.SecondRightRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterRightSecond.Id, newPlaсeAdditionalRebarBottomCenterRightSecond);
                        rebarIdCollection.Add(additionalRebarBottomCenterRightSecond.Id);

                        FamilyInstance tubWeldingAdditionalRebarBottomCenterRightSecond = doc.Create.NewFamilyInstance(tubWelding_p1L
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarBottomCenterRightSecond.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarBottomCenterRightSecond.Id, newPlaсeAdditionalRebarBottomCenterRightSecond);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarBottomCenterRightSecond.Id);

                        //Верхняя грань
                        //Центральный верхний левый стержень Первый
                        Rebar additionalRebarTopCenterLeftFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape01
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarTopCenterLeftFirst = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterLeftFirst.Id, newPlaсeAdditionalRebarTopCenterLeftFirst);
                        rebarIdCollection.Add(additionalRebarTopCenterLeftFirst.Id);

                        FamilyInstance tubWeldingAdditionalRebarTopCenterLeftFirst = doc.Create.NewFamilyInstance(tubWelding_p1S
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarTopCenterLeftFirst.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarTopCenterLeftFirst.Id, newPlaсeAdditionalRebarTopCenterLeftFirst);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarTopCenterLeftFirst.Id);

                        //Центральный верхний правый стержень Первый
                        Rebar additionalRebarTopCenterRightFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape01
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarTopCenterRightFirst = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterRightFirst.Id, newPlaсeAdditionalRebarTopCenterRightFirst);
                        rebarIdCollection.Add(additionalRebarTopCenterRightFirst.Id);

                        FamilyInstance tubWeldingAdditionalRebarTopCenterRightFirst = doc.Create.NewFamilyInstance(tubWelding_p1L
                            , tubWeldingSymbol, columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarTopCenterRightFirst.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarTopCenterRightFirst.Id, newPlaсeAdditionalRebarTopCenterRightFirst);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarTopCenterRightFirst.Id);

                        //Центральный верхний левый стержень Второй
                        Rebar additionalRebarTopCenterLeftSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape01
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarTopCenterLeftSecond = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                            - (squareColumnsReinforcementMainForm.SecondLeftRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterLeftSecond.Id, newPlaсeAdditionalRebarTopCenterLeftSecond);
                        rebarIdCollection.Add(additionalRebarTopCenterLeftSecond.Id);

                        FamilyInstance tubWeldingAdditionalRebarTopCenterLeftSecond = doc.Create.NewFamilyInstance(tubWelding_p1L
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarTopCenterLeftSecond.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarTopCenterLeftSecond.Id, newPlaсeAdditionalRebarTopCenterLeftSecond);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarTopCenterLeftSecond.Id);

                        //Центральный верхний правый стержень Второй
                        Rebar additionalRebarTopCenterRightSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape01
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarTopCenterRightSecond = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8) 
                            + (squareColumnsReinforcementMainForm.SecondRightRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterRightSecond.Id, newPlaсeAdditionalRebarTopCenterRightSecond);
                        rebarIdCollection.Add(additionalRebarTopCenterRightSecond.Id);

                        FamilyInstance tubWeldingAdditionalRebarTopCenterRightSecond = doc.Create.NewFamilyInstance(tubWelding_p1S
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarTopCenterRightSecond.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarTopCenterRightSecond.Id, newPlaсeAdditionalRebarTopCenterRightSecond);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarTopCenterRightSecond.Id);
                    }

                    //Если стыковка стержней на сварке с загибом в плиту
                    else if (squareColumnsReinforcementMainForm.CheckedRebarOutletsTepeName == "radioButton_Welding"
                        & squareColumnsReinforcementMainForm.СhangeSection == false
                        & squareColumnsReinforcementMainForm.TransitionToOverlap == false
                        & squareColumnsReinforcementMainForm.BendIntoSlab == true)
                    {
                        //Точки для построения кривых удлиненных стержней (начало и конец удлиненные)
                        XYZ rebar_p1L = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z 
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));
                        XYZ rebar_p2L = new XYZ(Math.Round(rebar_p1L.X, 6)
                            , Math.Round(rebar_p1L.Y, 6)
                            , Math.Round(rebar_p1L.Z 
                            - (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8) 
                            + columnProperties.ColumnLength 
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8) 
                            - (squareColumnsReinforcementMainForm.BendIntoSlabOffset / 304.8)
                            - (squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2), 6));
                        XYZ rebar_p3L = new XYZ(Math.Round(rebar_p2L.X 
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8) 
                            - ((squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)
                            - (squareColumnsReinforcementMainForm.BendIntoSlabOffset / 304.8)
                            + (squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2)), 6)
                            , Math.Round(rebar_p2L.Y, 6)
                            , Math.Round(rebar_p2L.Z, 6));

                        //Точки для построения кривых удлиненных стержней (начало укороченное и конец удлиненный)
                        XYZ rebar_p1S = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z 
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8), 6));
                        XYZ rebar_p2S = new XYZ(Math.Round(rebar_p1S.X, 6)
                            , Math.Round(rebar_p1S.Y, 6)
                            , Math.Round(rebar_p1S.Z 
                            - (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8) 
                            + columnProperties.ColumnLength 
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)
                            - (squareColumnsReinforcementMainForm.BendIntoSlabOffset / 304.8)
                            - (squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2), 6));
                        XYZ rebar_p3S = new XYZ(Math.Round(rebar_p2S.X 
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8) 
                            - ((squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)
                            - (squareColumnsReinforcementMainForm.BendIntoSlabOffset / 304.8)
                            + (squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2)), 6)
                            , Math.Round(rebar_p2S.Y, 6)
                            , Math.Round(rebar_p2S.Z, 6));

                        //Точки для построения кривых укороченных стержней (начало удлиненное и конец укороченный)
                        XYZ additionalRebar_p1L = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));
                        XYZ additionalRebar_p2L = new XYZ(Math.Round(additionalRebar_p1L.X, 6)
                            , Math.Round(additionalRebar_p1L.Y, 6)
                            , Math.Round(additionalRebar_p1L.Z
                            - (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8)
                            + columnProperties.ColumnLength
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)
                            - (squareColumnsReinforcementMainForm.BendIntoSlabOffset / 304.8)
                            - (squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2), 6));
                        XYZ additionalRebar_p3L = new XYZ(Math.Round(additionalRebar_p2L.X
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8)
                            - ((squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)
                            - (squareColumnsReinforcementMainForm.BendIntoSlabOffset / 304.8)
                            + (squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2)), 6)
                            , Math.Round(additionalRebar_p2L.Y, 6)
                            , Math.Round(additionalRebar_p2L.Z, 6));

                        //Точки для построения кривых укороченных стержней (начало и конец укороченные)
                        XYZ additionalRebar_p1S = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z 
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8), 6));
                        XYZ additionalRebar_p2S = new XYZ(Math.Round(additionalRebar_p1S.X, 6)
                            , Math.Round(additionalRebar_p1S.Y, 6)
                            , Math.Round(additionalRebar_p1S.Z 
                            - (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8) 
                            + columnProperties.ColumnLength 
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)
                            - (squareColumnsReinforcementMainForm.BendIntoSlabOffset / 304.8)
                            - (squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2), 6));
                        XYZ additionalRebar_p3S = new XYZ(Math.Round(additionalRebar_p2S.X 
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8) 
                            - ((squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)
                            - (squareColumnsReinforcementMainForm.BendIntoSlabOffset / 304.8)
                            + (squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2)), 6)
                            , Math.Round(additionalRebar_p2S.Y, 6)
                            , Math.Round(additionalRebar_p2S.Z, 6));

                        //Кривые стержня
                        List<Curve> mainRebarLongCurves = new List<Curve>();
                        List<Curve> mainRebarShortCurves = new List<Curve>();
                        List<Curve> additionalRebarLongCurves = new List<Curve>();
                        List<Curve> additionalRebarShortCurves = new List<Curve>();

                        Curve line1L = Line.CreateBound(rebar_p1L, rebar_p2L) as Curve;
                        mainRebarLongCurves.Add(line1L);
                        Curve line2L = Line.CreateBound(rebar_p2L, rebar_p3L) as Curve;
                        mainRebarLongCurves.Add(line2L);

                        Curve line1S = Line.CreateBound(rebar_p1S, rebar_p2S) as Curve;
                        mainRebarShortCurves.Add(line1S);
                        Curve line2S = Line.CreateBound(rebar_p2S, rebar_p3S) as Curve;
                        mainRebarShortCurves.Add(line2S);

                        Curve additionalLine1L = Line.CreateBound(additionalRebar_p1L, additionalRebar_p2L) as Curve;
                        additionalRebarLongCurves.Add(additionalLine1L);
                        Curve additionalLine2L = Line.CreateBound(additionalRebar_p2L, additionalRebar_p3L) as Curve;
                        additionalRebarLongCurves.Add(additionalLine2L);

                        Curve additionalLine1S = Line.CreateBound(additionalRebar_p1S, additionalRebar_p2S) as Curve;
                        additionalRebarShortCurves.Add(additionalLine1S);
                        Curve additionalLine2S = Line.CreateBound(additionalRebar_p2S, additionalRebar_p3S) as Curve;
                        additionalRebarShortCurves.Add(additionalLine2S);

                        //Нижний левый угол
                        Rebar mainRebarLeftBottom = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.MainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ rotate1_p1 = new XYZ(rebar_p1L.X, rebar_p1L.Y, rebar_p1L.Z);
                        XYZ rotate1_p2 = new XYZ(rebar_p1L.X, rebar_p1L.Y, rebar_p1L.Z + 1);
                        Line rotateLine = Line.CreateBound(rotate1_p1, rotate1_p2);
                        ElementTransformUtils.RotateElement(doc, mainRebarLeftBottom.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeMainRebarLeftBottom = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, mainRebarLeftBottom.Id, newPlaсeMainRebarLeftBottom);
                        rebarIdCollection.Add(mainRebarLeftBottom.Id);

                        //Верхний левый угол
                        Rebar mainRebarLeftTop = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.MainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, mainRebarLeftTop.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeMainRebarLeftTop = new XYZ(-columnProperties.ColumnSectionHeight / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2
                            , columnProperties.ColumnSectionWidth / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, mainRebarLeftTop.Id, newPlaсeMainRebarLeftTop);
                        rebarIdCollection.Add(mainRebarLeftTop.Id);

                        //Верхний правый угол
                        Rebar mainRebarRightTop = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.MainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeMainRebarRightTop = new XYZ(columnProperties.ColumnSectionHeight / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2
                            , columnProperties.ColumnSectionWidth / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, mainRebarRightTop.Id, newPlaсeMainRebarRightTop);
                        rebarIdCollection.Add(mainRebarRightTop.Id);

                        //Нижний правый угол
                        Rebar mainRebarRightBottom = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.MainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeMainRebarRightBottom = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, mainRebarRightBottom.Id, newPlaсeMainRebarRightBottom);
                        rebarIdCollection.Add(mainRebarRightBottom.Id);

                        //Левая грань
                        //Центральный левый нижний стержень Первый
                        Rebar additionalRebarLeftCenterDownFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarLeftCenterDownFirst.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarLeftCenterDownFirst = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterDownFirst.Id, newPlaсeAdditionalRebarLeftCenterDownFirst);
                        rebarIdCollection.Add(additionalRebarLeftCenterDownFirst.Id);

                        //Центральный левый верхний стержень Первый
                        Rebar additionalRebarLeftCenterUpFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarLeftCenterUpFirst.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarLeftCenterUpFirst = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterUpFirst.Id, newPlaсeAdditionalRebarLeftCenterUpFirst);
                        rebarIdCollection.Add(additionalRebarLeftCenterUpFirst.Id);

                        //Центральный левый нижний стержень Второй
                        Rebar additionalRebarLeftCenterDownSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarLeftCenterDownSecond.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarLeftCenterDownSecond = new XYZ(-columnProperties.ColumnSectionHeight / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8) 
                            - (squareColumnsReinforcementMainForm.SecondDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterDownSecond.Id, newPlaсeAdditionalRebarLeftCenterDownSecond);
                        rebarIdCollection.Add(additionalRebarLeftCenterDownSecond.Id);

                        //Центральный левый верхний стержень Второй
                        Rebar additionalRebarLeftCenterUpSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarLeftCenterUpSecond.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarLeftCenterUpSecond = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8) 
                            + (squareColumnsReinforcementMainForm.SecondUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterUpSecond.Id, newPlaсeAdditionalRebarLeftCenterUpSecond);
                        rebarIdCollection.Add(additionalRebarLeftCenterUpSecond.Id);

                        //Правая грань
                        //Центральный правый нижний стержень Первый
                        Rebar additionalRebarRightCenterDownFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarRightCenterDownFirst = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterDownFirst.Id, newPlaсeAdditionalRebarRightCenterDownFirst);
                        rebarIdCollection.Add(additionalRebarRightCenterDownFirst.Id);

                        //Центральный правый верхний стержень Первый
                        Rebar additionalRebarRightCenterUpFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarRightCenterUpFirst = new XYZ(columnProperties.ColumnSectionHeight / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterUpFirst.Id, newPlaсeAdditionalRebarRightCenterUpFirst);
                        rebarIdCollection.Add(additionalRebarRightCenterUpFirst.Id);

                        //Центральный правый нижний стержень Второй
                        Rebar additionalRebarRightCenterDownSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarRightCenterDownSecond = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8) 
                            - (squareColumnsReinforcementMainForm.SecondDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterDownSecond.Id, newPlaсeAdditionalRebarRightCenterDownSecond);
                        rebarIdCollection.Add(additionalRebarRightCenterDownSecond.Id);

                        //Центральный правый верхний стержень Второй
                        Rebar additionalRebarRightCenterUpSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarRightCenterUpSecond = new XYZ(columnProperties.ColumnSectionHeight / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8) 
                            + (squareColumnsReinforcementMainForm.SecondUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterUpSecond.Id, newPlaсeAdditionalRebarRightCenterUpSecond);
                        rebarIdCollection.Add(additionalRebarRightCenterUpSecond.Id);

                        //Нижняя грань
                        //Центральный нижний левый стержень Первый
                        Rebar additionalRebarBottomCenterLeftFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenterLeftFirst.Id, rotateLine, -90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarBottomCenterLeftFirst = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterLeftFirst.Id, newPlaсeAdditionalRebarBottomCenterLeftFirst);
                        rebarIdCollection.Add(additionalRebarBottomCenterLeftFirst.Id);

                        //Центральный нижний правый стержень Первый
                        Rebar additionalRebarBottomCenterRightFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenterRightFirst.Id, rotateLine, -90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarBottomCenterRightFirst = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterRightFirst.Id, newPlaсeAdditionalRebarBottomCenterRightFirst);
                        rebarIdCollection.Add(additionalRebarBottomCenterRightFirst.Id);

                        //Центральный нижний левый стержень Второй
                        Rebar additionalRebarBottomCenterLeftSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenterLeftSecond.Id, rotateLine, -90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarBottomCenterLeftSecond = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8) 
                            - (squareColumnsReinforcementMainForm.SecondLeftRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterLeftSecond.Id, newPlaсeAdditionalRebarBottomCenterLeftSecond);
                        rebarIdCollection.Add(additionalRebarBottomCenterLeftSecond.Id);

                        //Центральный нижний правый стержень Второй
                        Rebar additionalRebarBottomCenterRightSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenterRightSecond.Id, rotateLine, -90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarBottomCenterRightSecond = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            + (squareColumnsReinforcementMainForm.SecondRightRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterRightSecond.Id, newPlaсeAdditionalRebarBottomCenterRightSecond);
                        rebarIdCollection.Add(additionalRebarBottomCenterRightSecond.Id);

                        //Верхняя грань
                        //Центральный верхний левый стержень Первый
                        Rebar additionalRebarTopCenterLeftFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenterLeftFirst.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarTopCenterLeftFirst = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterLeftFirst.Id, newPlaсeAdditionalRebarTopCenterLeftFirst);
                        rebarIdCollection.Add(additionalRebarTopCenterLeftFirst.Id);

                        //Центральный верхний правый стержень Первый
                        Rebar additionalRebarTopCenterRightFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenterRightFirst.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarTopCenterRightFirst = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterRightFirst.Id, newPlaсeAdditionalRebarTopCenterRightFirst);
                        rebarIdCollection.Add(additionalRebarTopCenterRightFirst.Id);

                        //Центральный верхний левый стержень Второй
                        Rebar additionalRebarTopCenterLeftSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenterLeftSecond.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarTopCenterLeftSecond = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8) 
                            - (squareColumnsReinforcementMainForm.SecondLeftRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterLeftSecond.Id, newPlaсeAdditionalRebarTopCenterLeftSecond);
                        rebarIdCollection.Add(additionalRebarTopCenterLeftSecond.Id);

                        //Центральный верхний правый стержень Второй
                        Rebar additionalRebarTopCenterRightSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right, RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenterRightSecond.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarTopCenterRightSecond = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8) 
                            + (squareColumnsReinforcementMainForm.SecondRightRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterRightSecond.Id, newPlaсeAdditionalRebarTopCenterRightSecond);
                        rebarIdCollection.Add(additionalRebarTopCenterRightSecond.Id);
                    }

                    //Переход со сварки на нахлест без изменения сечения колонны
                    else if (squareColumnsReinforcementMainForm.CheckedRebarOutletsTepeName == "radioButton_Welding"
                        & squareColumnsReinforcementMainForm.СhangeSection == false
                        & squareColumnsReinforcementMainForm.TransitionToOverlap == true)
                    {
                        //Точки для построения кривых основных угловых стержней удлиненных
                        XYZ rebar_p1L = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z 
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));
                        XYZ rebar_p2L = new XYZ(Math.Round(rebar_p1L.X, 6)
                            , Math.Round(rebar_p1L.Y, 6)
                            , Math.Round(rebar_p1L.Z 
                            + columnProperties.ColumnLength 
                            - (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));
                        XYZ rebar_p3L = new XYZ(Math.Round(rebar_p2L.X 
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam, 6)
                            , Math.Round(rebar_p2L.Y, 6)
                            , Math.Round(rebar_p2L.Z 
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8), 6));
                        XYZ rebar_p4L = new XYZ(Math.Round(rebar_p3L.X, 6)
                            , Math.Round(rebar_p3L.Y, 6)
                            , Math.Round(rebar_p3L.Z 
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));

                        //Точки для построения кривых основных угловых стержней укороченных
                        XYZ rebar_p1S = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z 
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8), 6));
                        XYZ rebar_p2S = new XYZ(Math.Round(rebar_p1S.X, 6)
                            , Math.Round(rebar_p1S.Y, 6)
                            , Math.Round(rebar_p1S.Z 
                            + columnProperties.ColumnLength 
                            - (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8), 6));
                        XYZ rebar_p3S = new XYZ(Math.Round(rebar_p2S.X 
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam, 6)
                            , Math.Round(rebar_p2S.Y, 6)
                            , Math.Round(rebar_p2S.Z
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8), 6));
                        XYZ rebar_p4S = new XYZ(Math.Round(rebar_p3S.X, 6)
                            , Math.Round(rebar_p3S.Y, 6)
                            , Math.Round(rebar_p3S.Z
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8), 6));

                        //Точки для построения кривфх основных боковых стержней удлиненныхм
                        XYZ additionalRebar_p1L = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z 
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));
                        XYZ additionalRebar_p2L = new XYZ(Math.Round(additionalRebar_p1L.X, 6)
                            , Math.Round(additionalRebar_p1L.Y, 6)
                            , Math.Round(additionalRebar_p1L.Z 
                            + columnProperties.ColumnLength 
                            - (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));
                        XYZ additionalRebar_p3L = new XYZ(Math.Round(additionalRebar_p2L.X 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam, 6)
                            , Math.Round(additionalRebar_p2L.Y, 6)
                            , Math.Round(additionalRebar_p2L.Z
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8), 6));
                        XYZ additionalRebar_p4L = new XYZ(Math.Round(additionalRebar_p3L.X, 6)
                            , Math.Round(additionalRebar_p3L.Y, 6)
                            , Math.Round(additionalRebar_p3L.Z 
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));

                        //Точки для построения кривфх основных боковых стержней укороченных
                        XYZ additionalRebar_p1S = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z 
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8), 6));
                        XYZ additionalRebar_p2S = new XYZ(Math.Round(additionalRebar_p1S.X, 6)
                            , Math.Round(additionalRebar_p1S.Y, 6)
                            , Math.Round(additionalRebar_p1S.Z 
                            + columnProperties.ColumnLength 
                            - (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8), 6));
                        XYZ additionalRebar_p3S = new XYZ(Math.Round(additionalRebar_p2S.X
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam, 6)
                            , Math.Round(additionalRebar_p2S.Y, 6)
                            , Math.Round(additionalRebar_p2S.Z 
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8), 6));
                        XYZ additionalRebar_p4S = new XYZ(Math.Round(additionalRebar_p3S.X, 6)
                            , Math.Round(additionalRebar_p3S.Y, 6)
                            , Math.Round(additionalRebar_p3S.Z 
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8), 6));

                        //Кривые стержня
                        List<Curve> mainRebarLongCurves = new List<Curve>();
                        List<Curve> mainRebarShortCurves = new List<Curve>();
                        List<Curve> additionalRebarLongCurves = new List<Curve>();
                        List<Curve> additionalRebarShortCurves = new List<Curve>();

                        Curve line1L = Line.CreateBound(rebar_p1L, rebar_p2L) as Curve;
                        mainRebarLongCurves.Add(line1L);
                        Curve line2L = Line.CreateBound(rebar_p2L, rebar_p3L) as Curve;
                        mainRebarLongCurves.Add(line2L);
                        Curve line3L = Line.CreateBound(rebar_p3L, rebar_p4L) as Curve;
                        mainRebarLongCurves.Add(line3L);

                        Curve line1S = Line.CreateBound(rebar_p1S, rebar_p2S) as Curve;
                        mainRebarShortCurves.Add(line1S);
                        Curve line2S = Line.CreateBound(rebar_p2S, rebar_p3S) as Curve;
                        mainRebarShortCurves.Add(line2L);
                        Curve line3S = Line.CreateBound(rebar_p3S, rebar_p4S) as Curve;
                        mainRebarShortCurves.Add(line3S);

                        Curve additionalLine1L = Line.CreateBound(additionalRebar_p1L, additionalRebar_p2L) as Curve;
                        additionalRebarLongCurves.Add(additionalLine1L);
                        Curve additionalLine2L = Line.CreateBound(additionalRebar_p2L, additionalRebar_p3L) as Curve;
                        additionalRebarLongCurves.Add(additionalLine2L);
                        Curve additionalLine3L = Line.CreateBound(additionalRebar_p3L, additionalRebar_p4L) as Curve;
                        additionalRebarLongCurves.Add(additionalLine3L);

                        Curve additionalLine1S = Line.CreateBound(additionalRebar_p1S, additionalRebar_p2S) as Curve;
                        additionalRebarShortCurves.Add(additionalLine1S);
                        Curve additionalLine2S = Line.CreateBound(additionalRebar_p2S, additionalRebar_p3S) as Curve;
                        additionalRebarShortCurves.Add(additionalLine2S);
                        Curve additionalLine3S = Line.CreateBound(additionalRebar_p3S, additionalRebar_p4S) as Curve;
                        additionalRebarShortCurves.Add(additionalLine3S);

                        //Нижний левый угол
                        Rebar mainRebarLeftBottom = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.MainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeMainRebarLeftBottom = new XYZ(-columnProperties.ColumnSectionHeight / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, mainRebarLeftBottom.Id, newPlaсeMainRebarLeftBottom);
                        rebarIdCollection.Add(mainRebarLeftBottom.Id);

                        //Верхний левый угол
                        Rebar mainRebarLeftTop = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.MainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeMainRebarLeftTop = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, mainRebarLeftTop.Id, newPlaсeMainRebarLeftTop);
                        rebarIdCollection.Add(mainRebarLeftTop.Id);

                        //Верхний правый угол
                        Rebar mainRebarRightTop = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.MainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ rotate1_p1 = new XYZ(rebar_p1L.X, rebar_p1L.Y, rebar_p1L.Z);
                        XYZ rotate1_p2 = new XYZ(rebar_p1L.X, rebar_p1L.Y, rebar_p1L.Z + 1);
                        Line rotateLine = Line.CreateBound(rotate1_p1, rotate1_p2);
                        ElementTransformUtils.RotateElement(doc, mainRebarRightTop.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeMainRebarRightTop = new XYZ(columnProperties.ColumnSectionHeight / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2
                            , columnProperties.ColumnSectionWidth / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            - squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, mainRebarRightTop.Id, newPlaсeMainRebarRightTop);
                        rebarIdCollection.Add(mainRebarRightTop.Id);

                        //Нижний правый угол
                        Rebar mainRebarRightBottom = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.MainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, mainRebarRightBottom.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeMainRebarRightBottom = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, mainRebarRightBottom.Id, newPlaсeMainRebarRightBottom);
                        rebarIdCollection.Add(mainRebarRightBottom.Id);

                        //Левая грань
                        //Центральный левый нижний стержень Первый
                        Rebar additionalRebarLeftCenterDownFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarLeftCenterDownFirst.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarLeftCenterDownFirst = new XYZ(-columnProperties.ColumnSectionHeight / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterDownFirst.Id, newPlaсeAdditionalRebarLeftCenterDownFirst);
                        rebarIdCollection.Add(additionalRebarLeftCenterDownFirst.Id);

                        //Центральный левый верхний стержень Первый
                        Rebar additionalRebarLeftCenterUpFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarLeftCenterUpFirst.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarLeftCenterUpFirst = new XYZ(-columnProperties.ColumnSectionHeight / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterUpFirst.Id, newPlaсeAdditionalRebarLeftCenterUpFirst);
                        rebarIdCollection.Add(additionalRebarLeftCenterUpFirst.Id);

                        //Центральный левый нижний стержень Второй
                        Rebar additionalRebarLeftCenterDownSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarLeftCenterDownSecond.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarLeftCenterDownSecond = new XYZ(-columnProperties.ColumnSectionHeight / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8)
                            - (squareColumnsReinforcementMainForm.SecondDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterDownSecond.Id, newPlaсeAdditionalRebarLeftCenterDownSecond);
                        rebarIdCollection.Add(additionalRebarLeftCenterDownSecond.Id);

                        //Центральный левый верхний стержень Второй
                        Rebar additionalRebarLeftCenterUpSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarLeftCenterUpSecond.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarLeftCenterUpSecond = new XYZ(-columnProperties.ColumnSectionHeight / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8) 
                            + (squareColumnsReinforcementMainForm.SecondUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterUpSecond.Id, newPlaсeAdditionalRebarLeftCenterUpSecond);
                        rebarIdCollection.Add(additionalRebarLeftCenterUpSecond.Id);

                        //Правая грань
                        //Центральный правый нижний стержень Первый
                        Rebar additionalRebarRightCenterDownFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenterDownFirst.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarRightCenterDownFirst = new XYZ(+columnProperties.ColumnSectionHeight / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterDownFirst.Id, newPlaсeAdditionalRebarRightCenterDownFirst);
                        rebarIdCollection.Add(additionalRebarRightCenterDownFirst.Id);

                        //Центральный правый верхний стержень Первый
                        Rebar additionalRebarRightCenterUpFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenterUpFirst.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarRightCenterUpFirst = new XYZ(+columnProperties.ColumnSectionHeight / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterUpFirst.Id, newPlaсeAdditionalRebarRightCenterUpFirst);
                        rebarIdCollection.Add(additionalRebarRightCenterUpFirst.Id);

                        //Центральный правый нижний стержень Второй
                        Rebar additionalRebarRightCenterDownSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenterDownSecond.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarRightCenterDownSecond = new XYZ(+columnProperties.ColumnSectionHeight / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8)
                            - (squareColumnsReinforcementMainForm.SecondDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterDownSecond.Id, newPlaсeAdditionalRebarRightCenterDownSecond);
                        rebarIdCollection.Add(additionalRebarRightCenterDownSecond.Id);

                        //Центральный правый верхний стержень Второй
                        Rebar additionalRebarRightCenterUpSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenterUpSecond.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarRightCenterUpSecond = new XYZ(+columnProperties.ColumnSectionHeight / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8) 
                            + (squareColumnsReinforcementMainForm.SecondUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterUpSecond.Id, newPlaсeAdditionalRebarRightCenterUpSecond);
                        rebarIdCollection.Add(additionalRebarRightCenterUpSecond.Id);

                        //Нижняя грань
                        //Центральный нижний левый стержень Первый
                        Rebar additionalRebarBottomCenterLeftFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarBottomCenterLeftFirst = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterLeftFirst.Id, newPlaсeAdditionalRebarBottomCenterLeftFirst);
                        rebarIdCollection.Add(additionalRebarBottomCenterLeftFirst.Id);

                        //Центральный нижний правый стержень Первый
                        Rebar additionalRebarBottomCenterRightFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarBottomCenterRightFirst = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterRightFirst.Id, newPlaсeAdditionalRebarBottomCenterRightFirst);
                        rebarIdCollection.Add(additionalRebarBottomCenterRightFirst.Id);

                        //Центральный нижний левый стержень Второй
                        Rebar additionalRebarBottomCenterLeftSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarBottomCenterLeftSecond = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8) 
                            - (squareColumnsReinforcementMainForm.SecondLeftRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterLeftSecond.Id, newPlaсeAdditionalRebarBottomCenterLeftSecond);
                        rebarIdCollection.Add(additionalRebarBottomCenterLeftSecond.Id);

                        //Центральный нижний правый стержень Второй
                        Rebar additionalRebarBottomCenterRightSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right, RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarBottomCenterRightSecond = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8) 
                            + (squareColumnsReinforcementMainForm.SecondRightRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterRightSecond.Id, newPlaсeAdditionalRebarBottomCenterRightSecond);
                        rebarIdCollection.Add(additionalRebarBottomCenterRightSecond.Id);

                        //Верхняя грань
                        //Центральный верхний левый стержень Первый
                        Rebar additionalRebarTopCenterLeftFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarTopCenterLeftFirst = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterLeftFirst.Id, newPlaсeAdditionalRebarTopCenterLeftFirst);
                        rebarIdCollection.Add(additionalRebarTopCenterLeftFirst.Id);

                        //Центральный верхний правый стержень Первый
                        Rebar additionalRebarTopCenterRightFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarTopCenterRightFirst = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterRightFirst.Id, newPlaсeAdditionalRebarTopCenterRightFirst);
                        rebarIdCollection.Add(additionalRebarTopCenterRightFirst.Id);

                        //Центральный верхний левый стержень Второй
                        Rebar additionalRebarTopCenterLeftSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarTopCenterLeftSecond = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                            - (squareColumnsReinforcementMainForm.SecondLeftRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterLeftSecond.Id, newPlaсeAdditionalRebarTopCenterLeftSecond);
                        rebarIdCollection.Add(additionalRebarTopCenterLeftSecond.Id);

                        //Центральный верхний правый стержень Второй
                        Rebar additionalRebarTopCenterRightSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarTopCenterRightSecond = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8) 
                            + (squareColumnsReinforcementMainForm.SecondRightRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterRightSecond.Id, newPlaсeAdditionalRebarTopCenterRightSecond);
                        rebarIdCollection.Add(additionalRebarTopCenterRightSecond.Id);
                    }

                    //Если стыковка стержней на сварке с изменением сечения колонны выше
                    else if (squareColumnsReinforcementMainForm.CheckedRebarOutletsTepeName == "radioButton_Welding"
                        & squareColumnsReinforcementMainForm.СhangeSection == true
                        & squareColumnsReinforcementMainForm.TransitionToOverlap == false
                        & squareColumnsReinforcementMainForm.СhangeSectionOffset <= 50)
                    {
                        FamilySymbol tubWeldingSymbol = rebarShapeCollector.GetTubWeldingFamilySymbol();
                        if (tubWeldingSymbol == null)
                        {
                            TaskDialog.Show("Revit", "Семейство CIT_04_ВаннаДляСварки не найдено");
                            return Result.Cancelled;
                        }

                        //Точки для построения кривых основных удлиненных угловых стержней
                        XYZ rebar_p1L = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));
                        XYZ rebar_p2L = new XYZ(Math.Round(rebar_p1L.X, 6)
                            , Math.Round(rebar_p1L.Y, 6)
                            , Math.Round(rebar_p1L.Z 
                            + columnProperties.ColumnLength
                            - ((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8) * 6 
                            - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8))
                            - (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));
                        XYZ rebar_p3L = new XYZ(Math.Round(rebar_p2L.X 
                            + deltaXWelding, 6)
                            , Math.Round(rebar_p2L.Y, 6)
                            , Math.Round(rebar_p2L.Z 
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8) 
                            + ((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8) * 6 
                            - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)), 6));
                        XYZ rebar_p4L = new XYZ(Math.Round(rebar_p3L.X, 6)
                            , Math.Round(rebar_p3L.Y, 6)
                            , Math.Round(rebar_p3L.Z 
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));

                        //Точки для построения кривых основных укороченных угловых стержней
                        XYZ rebar_p1S = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z 
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8), 6));
                        XYZ rebar_p2S = new XYZ(Math.Round(rebar_p1S.X, 6)
                            , Math.Round(rebar_p1S.Y, 6)
                            , Math.Round(rebar_p1S.Z 
                            + columnProperties.ColumnLength
                            - ((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8) * 6 
                            - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)) 
                            - (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8), 6));
                        XYZ rebar_p3S = new XYZ(Math.Round(rebar_p2S.X + deltaXWelding, 6)
                            , Math.Round(rebar_p2S.Y, 6)
                            , Math.Round(rebar_p2S.Z 
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8) 
                            + ((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8) * 6 
                            - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)), 6));
                        XYZ rebar_p4S = new XYZ(Math.Round(rebar_p3S.X, 6)
                            , Math.Round(rebar_p3S.Y, 6)
                            , Math.Round(rebar_p3S.Z 
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8), 6));

                        //Точки для построения кривых основных боковых удлиненных стержней
                        XYZ additionalRebar_p1L = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z 
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));
                        XYZ additionalRebar_p2L = new XYZ(Math.Round(additionalRebar_p1L.X, 6)
                            , Math.Round(additionalRebar_p1L.Y, 6)
                            , Math.Round(additionalRebar_p1L.Z 
                            + columnProperties.ColumnLength 
                            - ((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8) * 6 
                            - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)) 
                            - (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));
                        XYZ additionalRebar_p3L = new XYZ(Math.Round(additionalRebar_p2L.X 
                            + (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8), 6)
                            , Math.Round(additionalRebar_p2L.Y, 6)
                            , Math.Round(additionalRebar_p2L.Z
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8) 
                            + ((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8) * 6 
                            - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)), 6));
                        XYZ additionalRebar_p4L = new XYZ(Math.Round(additionalRebar_p3L.X, 6)
                            , Math.Round(additionalRebar_p3L.Y, 6)
                            , Math.Round(additionalRebar_p3L.Z 
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));

                        //Точки для построения кривых основных боковых укороченных стержней
                        XYZ additionalRebar_p1S = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z 
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8), 6));
                        XYZ additionalRebar_p2S = new XYZ(Math.Round(additionalRebar_p1S.X, 6)
                            , Math.Round(additionalRebar_p1S.Y, 6)
                            , Math.Round(additionalRebar_p1S.Z 
                            + columnProperties.ColumnLength 
                            - ((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8) * 6 
                            - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)) 
                            - (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8), 6));
                        XYZ additionalRebar_p3S = new XYZ(Math.Round(additionalRebar_p2S.X 
                            + (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8), 6)
                            , Math.Round(additionalRebar_p2S.Y, 6)
                            , Math.Round(additionalRebar_p2S.Z
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8) 
                            + ((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8) * 6 
                            - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)), 6));
                        XYZ additionalRebar_p4S = new XYZ(Math.Round(additionalRebar_p3S.X, 6)
                            , Math.Round(additionalRebar_p3S.Y, 6)
                            , Math.Round(additionalRebar_p3S.Z 
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8), 6));

                        XYZ tubWelding_p1L = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , columnProperties.ColumnLength
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8)
                            + columnProperties.СolumnBaseLevelOffset);
                        XYZ tubWelding_p1S = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , columnProperties.ColumnLength
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8)
                            + columnProperties.СolumnBaseLevelOffset);

                        //Кривые стержня
                        List<Curve> mainRebarLongCurves = new List<Curve>();
                        List<Curve> mainRebarShortCurves = new List<Curve>();
                        List<Curve> additionalRebarLongCurves = new List<Curve>();
                        List<Curve> additionalRebarShortCurves = new List<Curve>();

                        Curve line1L = Line.CreateBound(rebar_p1L, rebar_p2L) as Curve;
                        mainRebarLongCurves.Add(line1L);
                        Curve line2L = Line.CreateBound(rebar_p2L, rebar_p3L) as Curve;
                        mainRebarLongCurves.Add(line2L);
                        Curve line3L = Line.CreateBound(rebar_p3L, rebar_p4L) as Curve;
                        mainRebarLongCurves.Add(line3L);

                        Curve line1S = Line.CreateBound(rebar_p1S, rebar_p2S) as Curve;
                        mainRebarShortCurves.Add(line1S);
                        Curve line2S = Line.CreateBound(rebar_p2S, rebar_p3S) as Curve;
                        mainRebarShortCurves.Add(line2S);
                        Curve line3S = Line.CreateBound(rebar_p3S, rebar_p4S) as Curve;
                        mainRebarShortCurves.Add(line3S);

                        Curve additionalLine1L = Line.CreateBound(additionalRebar_p1L, additionalRebar_p2L) as Curve;
                        additionalRebarLongCurves.Add(additionalLine1L);
                        Curve additionalLine2L = Line.CreateBound(additionalRebar_p2L, additionalRebar_p3L) as Curve;
                        additionalRebarLongCurves.Add(additionalLine2L);
                        Curve additionalLine3L = Line.CreateBound(additionalRebar_p3L, additionalRebar_p4L) as Curve;
                        additionalRebarLongCurves.Add(additionalLine3L);

                        Curve additionalLine1S = Line.CreateBound(additionalRebar_p1S, additionalRebar_p2S) as Curve;
                        additionalRebarShortCurves.Add(additionalLine1S);
                        Curve additionalLine2S = Line.CreateBound(additionalRebar_p2S, additionalRebar_p3S) as Curve;
                        additionalRebarShortCurves.Add(additionalLine2S);
                        Curve additionalLine3S = Line.CreateBound(additionalRebar_p3S, additionalRebar_p4S) as Curve;
                        additionalRebarShortCurves.Add(additionalLine3S);

                        //Нижний левый угол
                        Rebar mainRebarLeftBottom = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.MainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ rotate_p1 = new XYZ(rebar_p1L.X, rebar_p1L.Y, rebar_p1L.Z);
                        XYZ rotate_p2 = new XYZ(rebar_p1L.X, rebar_p1L.Y, rebar_p1L.Z + 1);
                        Line rotateLine = Line.CreateBound(rotate_p1, rotate_p2);
                        ElementTransformUtils.RotateElement(doc, mainRebarLeftBottom.Id, rotateLine, alphaWelding);
                        XYZ newPlaсeMainRebarLeftBottom = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, mainRebarLeftBottom.Id, newPlaсeMainRebarLeftBottom);
                        rebarIdCollection.Add(mainRebarLeftBottom.Id);

                        FamilyInstance tubWeldingMainRebarLeftBottom = doc.Create.NewFamilyInstance(tubWelding_p1L
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingMainRebarLeftBottom.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.MainRebarTapeDiam);
                        XYZ newPlaсetubWeldingMainRebarLeftBottom = new XYZ(-columnProperties.ColumnSectionHeight / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2
                            + (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2
                            + (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, tubWeldingMainRebarLeftBottom.Id, newPlaсetubWeldingMainRebarLeftBottom);
                        rebarIdCollection.Add(tubWeldingMainRebarLeftBottom.Id);

                        //Верхний левый угол
                        Rebar mainRebarLeftTop = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.MainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, mainRebarLeftTop.Id, rotateLine, -alphaWelding);
                        XYZ newPlaсeMainRebarLeftTop = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, mainRebarLeftTop.Id, newPlaсeMainRebarLeftTop);
                        rebarIdCollection.Add(mainRebarLeftTop.Id);

                        FamilyInstance tubWeldingMainRebarLeftTop = doc.Create.NewFamilyInstance(tubWelding_p1S
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingMainRebarLeftTop.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.MainRebarTapeDiam);
                        XYZ newPlaсetubWeldingMainRebarLeftTop = new XYZ(-columnProperties.ColumnSectionHeight / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2 
                            + (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2 
                            - (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, tubWeldingMainRebarLeftTop.Id, newPlaсetubWeldingMainRebarLeftTop);
                        rebarIdCollection.Add(tubWeldingMainRebarLeftTop.Id);

                        //Верхний правый угол
                        Rebar mainRebarRightTop = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.MainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, mainRebarRightTop.Id, rotateLine, alphaWelding);
                        ElementTransformUtils.RotateElement(doc, mainRebarRightTop.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeMainRebarRightTop = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, mainRebarRightTop.Id, newPlaсeMainRebarRightTop);
                        rebarIdCollection.Add(mainRebarRightTop.Id);

                        FamilyInstance tubWeldingMainRebarRightTop = doc.Create.NewFamilyInstance(tubWelding_p1L
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingMainRebarRightTop.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.MainRebarTapeDiam);
                        XYZ newPlaсetubWeldingMainRebarRightTop = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2
                            - (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            - squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2 
                            - (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, tubWeldingMainRebarRightTop.Id, newPlaсetubWeldingMainRebarRightTop);
                        rebarIdCollection.Add(tubWeldingMainRebarRightTop.Id);

                        //Нижний правый угол
                        Rebar mainRebarRightBottom = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.MainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, mainRebarRightBottom.Id, rotateLine, -alphaWelding);
                        ElementTransformUtils.RotateElement(doc, mainRebarRightBottom.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeMainRebarRightBottom = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            - squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, mainRebarRightBottom.Id, newPlaсeMainRebarRightBottom);
                        rebarIdCollection.Add(mainRebarRightBottom.Id);

                        FamilyInstance tubWeldingMainRebarRightBottom = doc.Create.NewFamilyInstance(tubWelding_p1S
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingMainRebarRightBottom.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.MainRebarTapeDiam);
                        XYZ newPlaсetubWeldingMainRebarRightBottom = new XYZ(columnProperties.ColumnSectionHeight / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2
                            - (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2
                            + (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, tubWeldingMainRebarRightBottom.Id, newPlaсetubWeldingMainRebarRightBottom);
                        rebarIdCollection.Add(tubWeldingMainRebarRightBottom.Id);

                        //Левая грань
                        //Центральный левый нижний стержень Первый
                        Rebar additionalRebarLeftCenterDownFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarLeftCenterDownFirst = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterDownFirst.Id, newPlaсeAdditionalRebarLeftCenterDownFirst);
                        rebarIdCollection.Add(additionalRebarLeftCenterDownFirst.Id);

                        FamilyInstance tubWeldingAdditionalRebarLeftCenterDownFirst = doc.Create.NewFamilyInstance(tubWelding_p1L
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarLeftCenterDownFirst.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        XYZ newPlaсetubWeldingAdditionalRebarLeftCenterDownFirst = new XYZ(-columnProperties.ColumnSectionHeight / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            + (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8)
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarLeftCenterDownFirst.Id, newPlaсetubWeldingAdditionalRebarLeftCenterDownFirst);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarLeftCenterDownFirst.Id);

                        //Центральный левый верхний стержень Первый
                        Rebar additionalRebarLeftCenterUpFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarLeftCenterUpFirst = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterUpFirst.Id, newPlaсeAdditionalRebarLeftCenterUpFirst);
                        rebarIdCollection.Add(additionalRebarLeftCenterUpFirst.Id);

                        FamilyInstance tubWeldingAdditionalRebarLeftCenterUpFirst = doc.Create.NewFamilyInstance(tubWelding_p1S
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarLeftCenterUpFirst.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        XYZ newPlaсetubWeldingAdditionalRebarLeftCenterUpFirst = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2 
                            + (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8)
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarLeftCenterUpFirst.Id, newPlaсetubWeldingAdditionalRebarLeftCenterUpFirst);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarLeftCenterUpFirst.Id);

                        //Центральный левый нижний стержень Второй
                        Rebar additionalRebarLeftCenterDownSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebarAdditionalRebarLeftCenterDownSecond = new XYZ(-columnProperties.ColumnSectionHeight / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8) 
                            - (squareColumnsReinforcementMainForm.SecondDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterDownSecond.Id, newPlaсeСolumnMainRebarAdditionalRebarLeftCenterDownSecond);
                        rebarIdCollection.Add(additionalRebarLeftCenterDownSecond.Id);

                        FamilyInstance tubWeldingAdditionalRebarLeftCenterDownSecond = doc.Create.NewFamilyInstance(tubWelding_p1S
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarLeftCenterDownSecond.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        XYZ newPlaсeTubWeldingAdditionalRebarLeftCenterDownSecond = new XYZ(-columnProperties.ColumnSectionHeight / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            + (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8)
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8) 
                            - (squareColumnsReinforcementMainForm.SecondDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarLeftCenterDownSecond.Id, newPlaсeTubWeldingAdditionalRebarLeftCenterDownSecond);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarLeftCenterDownSecond.Id);

                        //Центральный левый верхний стержень Второй
                        Rebar additionalRebarLeftCenterUpSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebarAdditionalRebarLeftCenterUpSecond = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8) 
                            + (squareColumnsReinforcementMainForm.SecondUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterUpSecond.Id, newPlaсeСolumnMainRebarAdditionalRebarLeftCenterUpSecond);
                        rebarIdCollection.Add(additionalRebarLeftCenterUpSecond.Id);

                        FamilyInstance tubWeldingAdditionalRebarLeftCenterUpSecond = doc.Create.NewFamilyInstance(tubWelding_p1L
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarLeftCenterUpSecond.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        XYZ newPlaсetubWeldingAdditionalRebarLeftCenterUpSecond = new XYZ(-columnProperties.ColumnSectionHeight / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2 
                            + (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8)
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8) 
                            + (squareColumnsReinforcementMainForm.SecondUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarLeftCenterUpSecond.Id, newPlaсetubWeldingAdditionalRebarLeftCenterUpSecond);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarLeftCenterUpSecond.Id);

                        //Правая грань
                        //Центральный правый нижний стержень Первый
                        Rebar additionalRebarRightCenterDownFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenterDownFirst.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarRightCenterDownFirst = new XYZ(columnProperties.ColumnSectionHeight / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterDownFirst.Id, newPlaсeAdditionalRebarRightCenterDownFirst);
                        rebarIdCollection.Add(additionalRebarRightCenterDownFirst.Id);

                        FamilyInstance tubWeldingAdditionalRebarRightCenterDownFirst = doc.Create.NewFamilyInstance(tubWelding_p1S
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarRightCenterDownFirst.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        XYZ newPlaсetubWeldingAdditionalRebarRightCenterDownFirst = new XYZ(columnProperties.ColumnSectionHeight / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2 
                            - (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8)
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarRightCenterDownFirst.Id, newPlaсetubWeldingAdditionalRebarRightCenterDownFirst);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarRightCenterDownFirst.Id);

                        //Центральный правый верхний стержень Первый
                        Rebar additionalRebarRightCenterUpFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenterUpFirst.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarRightCenterUpFirst = new XYZ(columnProperties.ColumnSectionHeight / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterUpFirst.Id, newPlaсeAdditionalRebarRightCenterUpFirst);
                        rebarIdCollection.Add(additionalRebarRightCenterUpFirst.Id);

                        FamilyInstance tubWeldingAdditionalRebarRightCenterUpFirst = doc.Create.NewFamilyInstance(tubWelding_p1L
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarRightCenterUpFirst.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        XYZ newPlaсetubWeldingAdditionalRebarRightCenterUpFirst = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            - (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8)
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarRightCenterUpFirst.Id, newPlaсetubWeldingAdditionalRebarRightCenterUpFirst);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarRightCenterUpFirst.Id);

                        //Центральный правый нижний стержень Второй
                        Rebar additionalRebarRightCenterDownSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenterDownSecond.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarRightCenterDownSecond = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8)
                            - (squareColumnsReinforcementMainForm.SecondDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterDownSecond.Id, newPlaсeAdditionalRebarRightCenterDownSecond);
                        rebarIdCollection.Add(additionalRebarRightCenterDownSecond.Id);

                        FamilyInstance tubWeldingAdditionalRebarRightCenterDownSecond = doc.Create.NewFamilyInstance(tubWelding_p1L
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel, StructuralType.NonStructural);
                        tubWeldingAdditionalRebarRightCenterDownSecond.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        XYZ newPlaсetubWeldingAdditionalRebarRightCenterDownSecond = new XYZ(columnProperties.ColumnSectionHeight / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            - (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8)
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8)
                            - (squareColumnsReinforcementMainForm.SecondDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarRightCenterDownSecond.Id, newPlaсetubWeldingAdditionalRebarRightCenterDownSecond);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarRightCenterDownSecond.Id);

                        //Центральный правый верхний стержень Второй
                        Rebar additionalRebarRightCenterUpSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenterUpSecond.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarRightCenterUpSecond = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8) 
                            + (squareColumnsReinforcementMainForm.SecondUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterUpSecond.Id, newPlaсeAdditionalRebarRightCenterUpSecond);
                        rebarIdCollection.Add(additionalRebarRightCenterUpSecond.Id);

                        FamilyInstance tubWeldingAdditionalRebarRightCenterUpSecond = doc.Create.NewFamilyInstance(tubWelding_p1S
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarRightCenterUpSecond.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        XYZ newPlaсetubWeldingAdditionalRebarRightCenterUpSecond = new XYZ(columnProperties.ColumnSectionHeight / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            - (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8)
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8)
                            + (squareColumnsReinforcementMainForm.SecondUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarRightCenterUpSecond.Id, newPlaсetubWeldingAdditionalRebarRightCenterUpSecond);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarRightCenterUpSecond.Id);

                        //Нижняя грань
                        //Центральный нижний левый стержень Первый
                        Rebar additionalRebarBottomCenterLeftFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenterLeftFirst.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarBottomCenterLeftFirst = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterLeftFirst.Id, newPlaсeAdditionalRebarBottomCenterLeftFirst);
                        rebarIdCollection.Add(additionalRebarBottomCenterLeftFirst.Id);

                        FamilyInstance tubWeldingAdditionalRebarBottomCenterLeftFirst = doc.Create.NewFamilyInstance(tubWelding_p1L
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarBottomCenterLeftFirst.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        XYZ newPlaсetubWeldingAdditionalRebarBottomCenterLeftFirst = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            + (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarBottomCenterLeftFirst.Id, newPlaсetubWeldingAdditionalRebarBottomCenterLeftFirst);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarBottomCenterLeftFirst.Id);

                        //Центральный нижний правый стержень Первый
                        Rebar additionalRebarBottomCenterRightFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenterRightFirst.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarBottomCenterRightFirst = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterRightFirst.Id, newPlaсeAdditionalRebarBottomCenterRightFirst);
                        rebarIdCollection.Add(additionalRebarBottomCenterRightFirst.Id);

                        FamilyInstance tubWeldingAdditionalRebarBottomCenterRightFirst = doc.Create.NewFamilyInstance(tubWelding_p1S
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarBottomCenterRightFirst.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        XYZ newPlaсetubWeldingAdditionalRebarBottomCenterRightFirst = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            + (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarBottomCenterRightFirst.Id, newPlaсetubWeldingAdditionalRebarBottomCenterRightFirst);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarBottomCenterRightFirst.Id);

                        //Центральный нижний левый стержень Второй
                        Rebar additionalRebarBottomCenterLeftSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenterLeftSecond.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarBottomCenterLeftSecond = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8) 
                            - (squareColumnsReinforcementMainForm.SecondLeftRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterLeftSecond.Id, newPlaсeAdditionalRebarBottomCenterLeftSecond);
                        rebarIdCollection.Add(additionalRebarBottomCenterLeftSecond.Id);

                        FamilyInstance tubWeldingAdditionalRebarBottomCenterLeftSecond = doc.Create.NewFamilyInstance(tubWelding_p1S
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarBottomCenterLeftSecond.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        XYZ newPlaсetubWeldingAdditionalRebarBottomCenterLeftSecond = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8) 
                            - (squareColumnsReinforcementMainForm.SecondLeftRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            + (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarBottomCenterLeftSecond.Id, newPlaсetubWeldingAdditionalRebarBottomCenterLeftSecond);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarBottomCenterLeftSecond.Id);

                        //Центральный нижний правый стержень Второй
                        Rebar additionalRebarBottomCenterRightSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenterRightSecond.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarBottomCenterRightSecond = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            + (squareColumnsReinforcementMainForm.SecondRightRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterRightSecond.Id, newPlaсeAdditionalRebarBottomCenterRightSecond);
                        rebarIdCollection.Add(additionalRebarBottomCenterRightSecond.Id);

                        FamilyInstance tubWeldingAdditionalRebarBottomCenterRightSecond = doc.Create.NewFamilyInstance(tubWelding_p1L
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarBottomCenterRightSecond.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        XYZ newPlaсetubWeldingAdditionalRebarBottomCenterRightSecond = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            + (squareColumnsReinforcementMainForm.SecondRightRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2 
                            + (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarBottomCenterRightSecond.Id, newPlaсetubWeldingAdditionalRebarBottomCenterRightSecond);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarBottomCenterRightSecond.Id);

                        //Верхняя грань
                        //Центральный верхний левый стержень Первый
                        Rebar additionalRebarTopCenterLeftFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenterLeftFirst.Id, rotateLine, -90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarTopCenterLeftFirst = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterLeftFirst.Id, newPlaсeAdditionalRebarTopCenterLeftFirst);
                        rebarIdCollection.Add(additionalRebarTopCenterLeftFirst.Id);

                        FamilyInstance tubWeldingAdditionalRebarTopCenterLeftFirst = doc.Create.NewFamilyInstance(tubWelding_p1S
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarTopCenterLeftFirst.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        XYZ newPlaсetubWeldingAdditionalRebarTopCenterLeftFirst = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2 
                            - (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarTopCenterLeftFirst.Id, newPlaсetubWeldingAdditionalRebarTopCenterLeftFirst);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarTopCenterLeftFirst.Id);

                        //Центральный верхний стержень Первый
                        Rebar additionalRebarTopCenterRightFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenterRightFirst.Id, rotateLine, -90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarTopCenterRightFirst = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterRightFirst.Id, newPlaсeAdditionalRebarTopCenterRightFirst);
                        rebarIdCollection.Add(additionalRebarTopCenterRightFirst.Id);

                        FamilyInstance tubWeldingAdditionalRebarTopCenterRightFirst = doc.Create.NewFamilyInstance(tubWelding_p1L
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarTopCenterRightFirst.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        XYZ newPlaсeTubWeldingAdditionalRebarTopCenterRightFirst = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2 
                            - (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarTopCenterRightFirst.Id, newPlaсeTubWeldingAdditionalRebarTopCenterRightFirst);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarTopCenterRightFirst.Id);

                        //Центральный верхний левый стержень Второй
                        Rebar additionalRebarTopCenterLeftSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenterLeftSecond.Id, rotateLine, -90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarTopCenterLeftSecond = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8) 
                            - (squareColumnsReinforcementMainForm.SecondLeftRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterLeftSecond.Id, newPlaсeAdditionalRebarTopCenterLeftSecond);
                        rebarIdCollection.Add(additionalRebarTopCenterLeftSecond.Id);

                        FamilyInstance tubWeldingAdditionalRebarTopCenterLeftSecond = doc.Create.NewFamilyInstance(tubWelding_p1L
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarTopCenterLeftSecond.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        XYZ newPlaсetubWeldingAdditionalRebarTopCenterLeftSecond = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8) 
                            - (squareColumnsReinforcementMainForm.SecondLeftRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2 
                            - (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarTopCenterLeftSecond.Id, newPlaсetubWeldingAdditionalRebarTopCenterLeftSecond);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarTopCenterLeftSecond.Id);

                        //Центральный верхний стержень Второй
                        Rebar additionalRebarTopCenterRightSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenterRightSecond.Id, rotateLine, -90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarTopCenterRightSecond = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8) 
                            + (squareColumnsReinforcementMainForm.SecondRightRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterRightSecond.Id, newPlaсeAdditionalRebarTopCenterRightSecond);
                        rebarIdCollection.Add(additionalRebarTopCenterRightSecond.Id);

                        FamilyInstance tubWeldingAdditionalRebarTopCenterRightSecond = doc.Create.NewFamilyInstance(tubWelding_p1S
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarTopCenterRightSecond.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        XYZ newPlaсetubWeldingAdditionalRebarTopCenterRightSecond = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8) 
                            + (squareColumnsReinforcementMainForm.SecondRightRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2 
                            - (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarTopCenterRightSecond.Id, newPlaсetubWeldingAdditionalRebarTopCenterRightSecond);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarTopCenterRightSecond.Id);
                    }

                    //Переход со сварки на нахлест с изменением сечения колонны
                    else if (squareColumnsReinforcementMainForm.CheckedRebarOutletsTepeName == "radioButton_Welding"
                        & squareColumnsReinforcementMainForm.СhangeSection == true
                        & squareColumnsReinforcementMainForm.TransitionToOverlap == true
                        & squareColumnsReinforcementMainForm.СhangeSectionOffset <= 50)
                    {
                        //Точки для построения кривых основных угловых удлиненных стержней
                        XYZ rebar_p1L = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z 
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));
                        XYZ rebar_p2L = new XYZ(Math.Round(rebar_p1L.X, 6)
                            , Math.Round(rebar_p1L.Y, 6)
                            , Math.Round(rebar_p1L.Z 
                            + columnProperties.ColumnLength
                            - ((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8) * 6 
                            - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8))
                            - (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));
                        XYZ rebar_p3L = new XYZ(Math.Round(rebar_p2L.X
                            + deltaXMainRebarOverlapping, 6)
                            , Math.Round(rebar_p2L.Y, 6)
                            , Math.Round(rebar_p2L.Z 
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8) 
                            + ((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8) * 6 
                            - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)), 6));
                        XYZ rebar_p4L = new XYZ(Math.Round(rebar_p3L.X, 6)
                            , Math.Round(rebar_p3L.Y, 6)
                            , Math.Round(rebar_p3L.Z
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));

                        //Точки для построения кривых основных угловых укороченных стержней
                        XYZ rebar_p1S = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8), 6));
                        XYZ rebar_p2S = new XYZ(Math.Round(rebar_p1S.X, 6)
                            , Math.Round(rebar_p1S.Y, 6)
                            , Math.Round(rebar_p1S.Z
                            + columnProperties.ColumnLength 
                            - ((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8) * 6
                            - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)) 
                            - (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8), 6));
                        XYZ rebar_p3S = new XYZ(Math.Round(rebar_p2S.X 
                            + deltaXMainRebarOverlapping, 6)
                            , Math.Round(rebar_p2S.Y, 6)
                            , Math.Round(rebar_p2S.Z 
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)
                            + ((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8) * 6 
                            - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)), 6));
                        XYZ rebar_p4S = new XYZ(Math.Round(rebar_p3S.X, 6)
                            , Math.Round(rebar_p3S.Y, 6)
                            , Math.Round(rebar_p3S.Z 
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8), 6));

                        //Точки для построения кривых основных боковых удлиненных стержней
                        XYZ additionalRebar_p1L = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z 
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));
                        XYZ additionalRebar_p2L = new XYZ(Math.Round(additionalRebar_p1L.X, 6)
                            , Math.Round(additionalRebar_p1L.Y, 6)
                            , Math.Round(additionalRebar_p1L.Z 
                            + columnProperties.ColumnLength
                            - ((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8) * 6
                            - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8))
                            - (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));
                        XYZ additionalRebar_p3L = new XYZ(Math.Round(additionalRebar_p2L.X 
                            + deltaXAdditionalMainRebarOverlapping, 6)
                            , Math.Round(additionalRebar_p2L.Y, 6)
                            , Math.Round(additionalRebar_p2L.Z 
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8) 
                            + ((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8) * 6 
                            - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)), 6));
                        XYZ additionalRebar_p4L = new XYZ(Math.Round(additionalRebar_p3L.X, 6)
                            , Math.Round(additionalRebar_p3L.Y, 6)
                            , Math.Round(additionalRebar_p3L.Z 
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));

                        //Точки для построения кривых основных боковых укороченных стержней
                        XYZ additionalRebar_p1S = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z 
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8), 6));
                        XYZ additionalRebar_p2S = new XYZ(Math.Round(additionalRebar_p1S.X, 6)
                            , Math.Round(additionalRebar_p1S.Y, 6)
                            , Math.Round(additionalRebar_p1S.Z 
                            + columnProperties.ColumnLength
                            - ((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8) * 6 
                            - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)) 
                            - (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8), 6));
                        XYZ additionalRebar_p3S = new XYZ(Math.Round(additionalRebar_p2S.X 
                            + deltaXAdditionalMainRebarOverlapping, 6)
                            , Math.Round(additionalRebar_p2S.Y, 6)
                            , Math.Round(additionalRebar_p2S.Z 
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)
                            + ((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8) * 6 
                            - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)), 6));
                        XYZ additionalRebar_p4S = new XYZ(Math.Round(additionalRebar_p3S.X, 6)
                            , Math.Round(additionalRebar_p3S.Y, 6)
                            , Math.Round(additionalRebar_p3S.Z 
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8), 6));

                        //Кривые стержня
                        List<Curve> mainRebarLongCurves = new List<Curve>();
                        List<Curve> mainRebarShortCurves = new List<Curve>();
                        List<Curve> additionalRebarLongCurves = new List<Curve>();
                        List<Curve> additionalRebarShortCurves = new List<Curve>();

                        //Кривые основных угловых удлиненных стержней
                        Curve line1L = Line.CreateBound(rebar_p1L, rebar_p2L) as Curve;
                        mainRebarLongCurves.Add(line1L);
                        Curve line2L = Line.CreateBound(rebar_p2L, rebar_p3L) as Curve;
                        mainRebarLongCurves.Add(line2L);
                        Curve line3L = Line.CreateBound(rebar_p3L, rebar_p4L) as Curve;
                        mainRebarLongCurves.Add(line3L);

                        //Кривые основных угловых укороченных стержней
                        Curve line1S = Line.CreateBound(rebar_p1S, rebar_p2S) as Curve;
                        mainRebarShortCurves.Add(line1S);
                        Curve line2S = Line.CreateBound(rebar_p2S, rebar_p3S) as Curve;
                        mainRebarShortCurves.Add(line2S);
                        Curve line3S = Line.CreateBound(rebar_p3S, rebar_p4S) as Curve;
                        mainRebarShortCurves.Add(line3S);

                        //Кривые основных боковых удлиненных стержней
                        Curve additionalLine1L = Line.CreateBound(additionalRebar_p1L, additionalRebar_p2L) as Curve;
                        additionalRebarLongCurves.Add(additionalLine1L);
                        Curve additionalLine2L = Line.CreateBound(additionalRebar_p2L, additionalRebar_p3L) as Curve;
                        additionalRebarLongCurves.Add(additionalLine2L);
                        Curve additionalLine3L = Line.CreateBound(additionalRebar_p3L, additionalRebar_p4L) as Curve;
                        additionalRebarLongCurves.Add(additionalLine3L);

                        //Кривые основных боковых укороченных стержней
                        Curve additionalLine1S = Line.CreateBound(additionalRebar_p1S, additionalRebar_p2S) as Curve;
                        additionalRebarShortCurves.Add(additionalLine1S);
                        Curve additionalLine2S = Line.CreateBound(additionalRebar_p2S, additionalRebar_p3S) as Curve;
                        additionalRebarShortCurves.Add(additionalLine2S);
                        Curve additionalLine3S = Line.CreateBound(additionalRebar_p3S, additionalRebar_p4S) as Curve;
                        additionalRebarShortCurves.Add(additionalLine3S);

                        //Нижний левый угол
                        Rebar mainRebarLeftBottom = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.MainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ rotate1_p1 = new XYZ(rebar_p1L.X, rebar_p1L.Y, rebar_p1L.Z);
                        XYZ rotate1_p2 = new XYZ(rebar_p1L.X, rebar_p1L.Y, rebar_p1L.Z + 1);
                        Line rotateLine = Line.CreateBound(rotate1_p1, rotate1_p2);
                        ElementTransformUtils.RotateElement(doc, mainRebarLeftBottom.Id, rotateLine, alphaMainRebarOverlapping);
                        XYZ newPlaсeMainRebarLeftBottom = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, mainRebarLeftBottom.Id, newPlaсeMainRebarLeftBottom);
                        rebarIdCollection.Add(mainRebarLeftBottom.Id);

                        //Верхний левый угол
                        Rebar mainRebarLeftTop = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.MainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, mainRebarLeftTop.Id, rotateLine, -alphaMainRebarOverlapping);
                        XYZ newPlaсeMainRebarLeftTop = new XYZ(-columnProperties.ColumnSectionHeight / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, mainRebarLeftTop.Id, newPlaсeMainRebarLeftTop);
                        rebarIdCollection.Add(mainRebarLeftTop.Id);

                        //Верхний правый угол
                        Rebar mainRebarRightTop = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.MainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, mainRebarRightTop.Id, rotateLine, alphaMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, mainRebarRightTop.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeMainRebarRightTop = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, mainRebarRightTop.Id, newPlaсeMainRebarRightTop);
                        rebarIdCollection.Add(mainRebarRightTop.Id);

                        //Нижний правый угол
                        Rebar mainRebarRightBottom = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.MainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, mainRebarRightBottom.Id, rotateLine, -alphaMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, mainRebarRightBottom.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeMainRebarRightBottom = new XYZ(columnProperties.ColumnSectionHeight / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, mainRebarRightBottom.Id, newPlaсeMainRebarRightBottom);
                        rebarIdCollection.Add(mainRebarRightBottom.Id);

                        //Левая грань
                        //Центральный левый нижний стержень Первый
                        Rebar additionalRebarLeftCenterDownFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarLeftCenterDownFirst.Id, rotateLine, alphaAdditionalMainRebarOverlapping);
                        XYZ newPlaсeAdditionalRebarLeftCenterDownFirst = new XYZ(-columnProperties.ColumnSectionHeight / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam  / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterDownFirst.Id, newPlaсeAdditionalRebarLeftCenterDownFirst);
                        rebarIdCollection.Add(additionalRebarLeftCenterDownFirst.Id);

                        //Центральный левый верхний стержень Первый
                        Rebar additionalRebarLeftCenterUpFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarLeftCenterUpFirst.Id, rotateLine, alphaAdditionalMainRebarOverlapping);
                        XYZ newPlaсeAdditionalRebarLeftCenterUpFirst = new XYZ(-columnProperties.ColumnSectionHeight / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam  / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterUpFirst.Id, newPlaсeAdditionalRebarLeftCenterUpFirst);
                        rebarIdCollection.Add(additionalRebarLeftCenterUpFirst.Id);

                        //Центральный левый нижний стержень Второй
                        Rebar additionalRebarLeftCenterDownSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarLeftCenterDownSecond.Id, rotateLine, alphaAdditionalMainRebarOverlapping);
                        XYZ newPlaсeAdditionalRebarLeftCenterDownSecond = new XYZ(-columnProperties.ColumnSectionHeight / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam  / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8) 
                            - (squareColumnsReinforcementMainForm.SecondDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterDownSecond.Id, newPlaсeAdditionalRebarLeftCenterDownSecond);
                        rebarIdCollection.Add(additionalRebarLeftCenterDownSecond.Id);

                        //Центральный левый верхний стержень Второй
                        Rebar additionalRebarLeftCenterUpSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarLeftCenterUpSecond.Id, rotateLine, alphaAdditionalMainRebarOverlapping);
                        XYZ newPlaсeAdditionalRebarLeftCenterUpSecond = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam  / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8) 
                            + (squareColumnsReinforcementMainForm.SecondUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterUpSecond.Id, newPlaсeAdditionalRebarLeftCenterUpSecond);
                        rebarIdCollection.Add(additionalRebarLeftCenterUpSecond.Id);

                        //Правая грань
                        //Центральный правый нижний стержень Первый
                        Rebar additionalRebarRightCenterDownFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenterDownFirst.Id, rotateLine, -alphaAdditionalMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenterDownFirst.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarRightCenterDownFirst = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam  / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterDownFirst.Id, newPlaсeAdditionalRebarRightCenterDownFirst);
                        rebarIdCollection.Add(additionalRebarRightCenterDownFirst.Id);

                        //Центральный правый верхний стержень Первый
                        Rebar additionalRebarRightCenterUpFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenterUpFirst.Id, rotateLine, -alphaAdditionalMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenterUpFirst.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarRightCenterUpFirst = new XYZ(columnProperties.ColumnSectionHeight / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam  / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterUpFirst.Id, newPlaсeAdditionalRebarRightCenterUpFirst);
                        rebarIdCollection.Add(additionalRebarRightCenterUpFirst.Id);

                        //Центральный правый нижний стержень Второй
                        Rebar additionalRebarRightCenterDownSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenterDownSecond.Id, rotateLine, -alphaAdditionalMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenterDownSecond.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarRightCenterDownSecond = new XYZ(columnProperties.ColumnSectionHeight / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam  / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8) 
                            - (squareColumnsReinforcementMainForm.SecondDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterDownSecond.Id, newPlaсeAdditionalRebarRightCenterDownSecond);
                        rebarIdCollection.Add(additionalRebarRightCenterDownSecond.Id);

                        //Центральный правый верхний стержень Второй
                        Rebar additionalRebarRightCenterUpSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenterUpSecond.Id, rotateLine, -alphaAdditionalMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenterUpSecond.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarRightCenterUpSecond = new XYZ(columnProperties.ColumnSectionHeight / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam  / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8) 
                            + (squareColumnsReinforcementMainForm.SecondUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterUpSecond.Id, newPlaсeAdditionalRebarRightCenterUpSecond);
                        rebarIdCollection.Add(additionalRebarRightCenterUpSecond.Id);

                        //Нижняя грань
                        //Центральный нижний левый стержень Первый
                        Rebar additionalRebarBottomCenterLeftFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenterLeftFirst.Id, rotateLine, -alphaAdditionalMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenterLeftFirst.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarBottomCenterLeftFirst = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam  / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterLeftFirst.Id, newPlaсeAdditionalRebarBottomCenterLeftFirst);
                        rebarIdCollection.Add(additionalRebarBottomCenterLeftFirst.Id);

                        //Центральный нижний правый стержень Первый
                        Rebar additionalRebarBottomCenterRightFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenterRightFirst.Id, rotateLine, -alphaAdditionalMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenterRightFirst.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarBottomCenterRightFirst = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam  / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterRightFirst.Id, newPlaсeAdditionalRebarBottomCenterRightFirst);
                        rebarIdCollection.Add(additionalRebarBottomCenterRightFirst.Id);

                        //Центральный нижний левый стержень Второй
                        Rebar additionalRebarBottomCenterLeftSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenterLeftSecond.Id, rotateLine, -alphaAdditionalMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenterLeftSecond.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarBottomCenterLeftSecond = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                            - (squareColumnsReinforcementMainForm.SecondLeftRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam  / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterLeftSecond.Id, newPlaсeAdditionalRebarBottomCenterLeftSecond);
                        rebarIdCollection.Add(additionalRebarBottomCenterLeftSecond.Id);

                        //Центральный нижний правый стержень Второй
                        Rebar additionalRebarBottomCenterRightSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenterRightSecond.Id, rotateLine, -alphaAdditionalMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenterRightSecond.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarBottomCenterRightSecond = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            + (squareColumnsReinforcementMainForm.SecondRightRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam  / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterRightSecond.Id, newPlaсeAdditionalRebarBottomCenterRightSecond);
                        rebarIdCollection.Add(additionalRebarBottomCenterRightSecond.Id);

                        //Верхняя грань
                        //Центральный верхний левый стержень Первый
                        Rebar additionalRebarTopCenterLeftFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenterLeftFirst.Id, rotateLine, alphaAdditionalMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenterLeftFirst.Id, rotateLine, -90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarTopCenterLeftFirst = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam  / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterLeftFirst.Id, newPlaсeAdditionalRebarTopCenterLeftFirst);
                        rebarIdCollection.Add(additionalRebarTopCenterLeftFirst.Id);

                        //Центральный верхний правый стержень Первый
                        Rebar additionalRebarTopCenterRightFirst = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenterRightFirst.Id, rotateLine, alphaAdditionalMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenterRightFirst.Id, rotateLine, -90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarTopCenterRightFirst = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam  / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterRightFirst.Id, newPlaсeAdditionalRebarTopCenterRightFirst);
                        rebarIdCollection.Add(additionalRebarTopCenterRightFirst.Id);

                        //Центральный верхний левый стержень Второй
                        Rebar additionalRebarTopCenterLeftSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenterLeftSecond.Id, rotateLine, alphaAdditionalMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenterLeftSecond.Id, rotateLine, -90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarTopCenterLeftSecond = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8) 
                            - (squareColumnsReinforcementMainForm.SecondLeftRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam  / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterLeftSecond.Id, newPlaсeAdditionalRebarTopCenterLeftSecond);
                        rebarIdCollection.Add(additionalRebarTopCenterLeftSecond.Id);

                        //Центральный верхний правый стержень Второй
                        Rebar additionalRebarTopCenterRightSecond = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenterRightSecond.Id, rotateLine, alphaAdditionalMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenterRightSecond.Id, rotateLine, -90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarTopCenterRightSecond = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            + (squareColumnsReinforcementMainForm.SecondRightRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam  / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterRightSecond.Id, newPlaсeAdditionalRebarTopCenterRightSecond);
                        rebarIdCollection.Add(additionalRebarTopCenterRightSecond.Id);
                    }

                    //Хомуты
                    SquareColumnsReinforcementCommand.CreateEncircleStirrup(doc
                        , rebarShapeCollector
                        , column
                        , columnProperties
                        , squareColumnsReinforcementMainForm
                        , rebarIdCollection);
                    SquareColumnsReinforcementCommand.CreateCrossoverStirrup(doc
                        , rebarShapeCollector
                        , column
                        , columnProperties
                        , squareColumnsReinforcementMainForm
                        , rebarIdCollection);

                    //Прогрессирующее обрушение
                    if (squareColumnsReinforcementMainForm.ProgressiveCollapseAddRebar)
                    {
                        SquareColumnsReinforcementCommand.CreateProgressiveCollapseRebar(doc
                            , rebarShapeCollector
                            , column
                            , columnProperties
                            , squareColumnsReinforcementMainForm
                            , rebarIdCollection);
                    }

                    //Группирование стержней
                    SquareColumnsReinforcementCommand.CreateRebarGroup(doc
                        , column
                        , rebarIdCollection
                        , columnProperties);
                }
                //Удаление предупреждения о редактировании группы вне редактора
                FailureHandlingOptions failureHandlingOptions = t.GetFailureHandlingOptions();
                failureHandlingOptions.SetFailuresPreprocessor(new GroupWarningSwallower());
                t.SetFailureHandlingOptions(failureHandlingOptions);
                t.Commit();
            }
            return Result.Succeeded;
        }
    }
}
