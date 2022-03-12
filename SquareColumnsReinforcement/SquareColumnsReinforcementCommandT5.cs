using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;

namespace SquareColumnsReinforcement
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class SquareColumnsReinforcementCommandT5 : IExternalCommand
    {
        List<FamilyInstance> columnsList;
        SquareColumnsReinforcementMainForm squareColumnsReinforcementMainForm;
        public SquareColumnsReinforcementCommandT5(List<FamilyInstance> columnsListFromMainComand, SquareColumnsReinforcementMainForm formFromMainComand)
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
                t.Start("Армирование колонны Тип5");
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
                        //Точки для построения кривых основных угловых стержней
                        XYZ rebar_p1 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z 
                            - (squareColumnsReinforcementMainForm.RebarDeepeningOffset / 304.8), 6));
                        XYZ rebar_p2 = new XYZ(Math.Round(rebar_p1.X, 6)
                            , Math.Round(rebar_p1.Y, 6)
                            , Math.Round(rebar_p1.Z 
                            + (squareColumnsReinforcementMainForm.RebarDeepeningOffset / 304.8)
                            + columnProperties.ColumnLength, 6));
                        XYZ rebar_p3 = new XYZ(Math.Round(rebar_p2.X 
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam, 6)
                            , Math.Round(rebar_p2.Y, 6)
                            , Math.Round(rebar_p2.Z 
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8), 6));
                        XYZ rebar_p4L = new XYZ(Math.Round(rebar_p3.X, 6)
                            , Math.Round(rebar_p3.Y, 6)
                            , Math.Round(rebar_p3.Z 
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));

                        //Точки для построения кривфх основных боковых стержней с двойным нахлестом
                        XYZ additionalRebar_p1 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z 
                            - (squareColumnsReinforcementMainForm.RebarDeepeningOffset / 304.8), 6));
                        XYZ additionalRebar_p2 = new XYZ(Math.Round(additionalRebar_p1.X, 6)
                            , Math.Round(additionalRebar_p1.Y, 6)
                            , Math.Round(additionalRebar_p1.Z 
                            + (squareColumnsReinforcementMainForm.RebarDeepeningOffset / 304.8)
                            + columnProperties.ColumnLength, 6));
                        XYZ additionalRebar_p3 = new XYZ(Math.Round(additionalRebar_p2.X 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam, 6)
                            , Math.Round(additionalRebar_p2.Y, 6)
                            , Math.Round(additionalRebar_p2.Z 
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8), 6));
                        XYZ additionalRebar_p4L = new XYZ(Math.Round(additionalRebar_p3.X, 6)
                            , Math.Round(additionalRebar_p3.Y, 6)
                            , Math.Round(additionalRebar_p3.Z 
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));

                        XYZ additionalRebar_p4S = new XYZ(Math.Round(additionalRebar_p3.X, 6)
                            , Math.Round(additionalRebar_p3.Y, 6)
                            , Math.Round(additionalRebar_p3.Z
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8), 6));

                        //Кривые стержня
                        List<Curve> mainRebarLongCurves = new List<Curve>();
                        List<Curve> additionalRebarLongCurves = new List<Curve>();
                        List<Curve> additionalRebarShortCurves = new List<Curve>();

                        Curve line1L = Line.CreateBound(rebar_p1, rebar_p2) as Curve;
                        mainRebarLongCurves.Add(line1L);
                        Curve line2L = Line.CreateBound(rebar_p2, rebar_p3) as Curve;
                        mainRebarLongCurves.Add(line2L);
                        Curve line3L = Line.CreateBound(rebar_p3, rebar_p4L) as Curve;
                        mainRebarLongCurves.Add(line3L);

                        Curve additionalLine1L = Line.CreateBound(additionalRebar_p1, additionalRebar_p2) as Curve;
                        additionalRebarLongCurves.Add(additionalLine1L);
                        Curve additionalLine2L = Line.CreateBound(additionalRebar_p2, additionalRebar_p3) as Curve;
                        additionalRebarLongCurves.Add(additionalLine2L);
                        Curve additionalLine3L = Line.CreateBound(additionalRebar_p3, additionalRebar_p4L) as Curve;
                        additionalRebarLongCurves.Add(additionalLine3L);

                        Curve additionalLine1S = Line.CreateBound(additionalRebar_p1, additionalRebar_p2) as Curve;
                        additionalRebarShortCurves.Add(additionalLine1S);
                        Curve additionalLine2S = Line.CreateBound(additionalRebar_p2, additionalRebar_p3) as Curve;
                        additionalRebarShortCurves.Add(additionalLine2S);
                        Curve additionalLine3S = Line.CreateBound(additionalRebar_p3, additionalRebar_p4S) as Curve;
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
                            , mainRebarLongCurves
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
                        XYZ rotate_p1 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z);
                        XYZ rotate_p2 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z + 1);
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
                            , mainRebarLongCurves
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
                        //Центральный левый нижний стержень
                        Rebar additionalRebarLeftCenterDown = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarLeftCenterDown.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarLeftCenterDown = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterDown.Id, newPlaсeAdditionalRebarLeftCenterDown);
                        rebarIdCollection.Add(additionalRebarLeftCenterDown.Id);

                        //Центральный левый верхний стержень
                        Rebar additionalRebarLeftCenterUp = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarLeftCenterUp.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarLeftCenterUp = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterUp.Id, newPlaсeAdditionalRebarLeftCenterUp);
                        rebarIdCollection.Add(additionalRebarLeftCenterUp.Id);

                        //Центральный левый стержень
                        Rebar additionalRebarLeftCenter = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarLeftCenter.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarLeftCenter = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenter.Id, newPlaсeAdditionalRebarLeftCenter);
                        rebarIdCollection.Add(additionalRebarLeftCenter.Id);

                        //Правая грань
                        //Центральный правый нижний стержень
                        Rebar additionalRebarRightCenterDown = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenterDown.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarRightCenterDown = new XYZ(+columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterDown.Id, newPlaсeAdditionalRebarRightCenterDown);
                        rebarIdCollection.Add(additionalRebarRightCenterDown.Id);

                        //Центральный правый верхний стержень
                        Rebar additionalRebarRightCenterUp = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenterUp.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarRightCenterUp = new XYZ(+columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterUp.Id, newPlaсeAdditionalRebarRightCenterUp);
                        rebarIdCollection.Add(additionalRebarRightCenterUp.Id);

                        //Центральный правый стержень
                        Rebar additionalRebarRightCenter = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenter.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarRightCenter = new XYZ(+columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenter.Id, newPlaсeAdditionalRebarRightCenter);
                        rebarIdCollection.Add(additionalRebarRightCenter.Id);

                        //Нижняя грань
                        //Центральный нижний левый стержень
                        Rebar additionalRebarBottomCenterLeft = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarBottomCenterLeft = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterLeft.Id, newPlaсeAdditionalRebarBottomCenterLeft);
                        rebarIdCollection.Add(additionalRebarBottomCenterLeft.Id);

                        //Центральный нижний правый стержень
                        Rebar additionalRebarBottomCenterRight = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarBottomCenterRight = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterRight.Id, newPlaсeAdditionalRebarBottomCenterRight);
                        rebarIdCollection.Add(additionalRebarBottomCenterRight.Id);

                        //Центральный нижний стержень
                        Rebar additionalRebarBottomCenter = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarBottomCenter = new XYZ(0, -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenter.Id, newPlaсeAdditionalRebarBottomCenter);
                        rebarIdCollection.Add(additionalRebarBottomCenter.Id);

                        //Верхняя грань
                        //Центральный верхний левый стержень
                        Rebar additionalRebarTopCenterLeft = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarTopCenterLeft = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterLeft.Id, newPlaсeAdditionalRebarTopCenterLeft);
                        rebarIdCollection.Add(additionalRebarTopCenterLeft.Id);

                        //Центральный верхний правый стержень
                        Rebar additionalRebarTopCenterRight = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarTopCenterRight = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterRight.Id, newPlaсeAdditionalRebarTopCenterRight);
                        rebarIdCollection.Add(additionalRebarTopCenterRight.Id);

                        //Центральный верхний левый стержень
                        Rebar additionalRebarTopCenter = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarTopCenter = new XYZ(0, columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenter.Id, newPlaсeAdditionalRebarTopCenter);
                        rebarIdCollection.Add(additionalRebarTopCenter.Id);
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
                        XYZ rotate1_p1 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z);
                        XYZ rotate1_p2 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z + 1);
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
                        //Центральный левый нижний стержень
                        Rebar additionalRebarLeftCenterDown = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarLeftCenterDown.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarLeftCenterDown = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterDown.Id, newPlaсeAdditionalRebarLeftCenterDown);
                        rebarIdCollection.Add(additionalRebarLeftCenterDown.Id);

                        //Центральный левый верхний стержень
                        Rebar additionalRebarLeftCenterUp = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarLeftCenterUp.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarLeftCenterUp = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterUp.Id, newPlaсeAdditionalRebarLeftCenterUp);
                        rebarIdCollection.Add(additionalRebarLeftCenterUp.Id);

                        //Центральный левый стержень
                        Rebar additionalRebarLeftCenter = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarLeftCenter.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarLeftCenter = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenter.Id, newPlaсeAdditionalRebarLeftCenter);
                        rebarIdCollection.Add(additionalRebarLeftCenter.Id);
                        
                        //Правая грань
                        //Центральный правый нижний стержень
                        Rebar additionalRebarRightCenterDown = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarRightCenterDown = new XYZ(+columnProperties.ColumnSectionHeight / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterDown.Id, newPlaсeAdditionalRebarRightCenterDown);
                        rebarIdCollection.Add(additionalRebarRightCenterDown.Id);

                        //Центральный правый верхний стержень
                        Rebar additionalRebarRightCenterUp = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarRightCenterUp = new XYZ(+columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterUp.Id, newPlaсeAdditionalRebarRightCenterUp);
                        rebarIdCollection.Add(additionalRebarRightCenterUp.Id);

                        //Центральный правый стержень
                        Rebar additionalRebarRightCenter = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarRightCenter = new XYZ(+columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenter.Id, newPlaсeAdditionalRebarRightCenter);
                        rebarIdCollection.Add(additionalRebarRightCenter.Id);

                        //Нижняя грань
                        //Центральный нижний левый стержень
                        Rebar additionalRebarBottomCenterLeft = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                       ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenterLeft.Id, rotateLine, -90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarBottomCenterLeft = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterLeft.Id, newPlaсeAdditionalRebarBottomCenterLeft);
                        rebarIdCollection.Add(additionalRebarBottomCenterLeft.Id);

                        //Центральный нижний правый стержень
                        Rebar additionalRebarBottomCenterRight = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenterRight.Id, rotateLine, -90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarBottomCenterRight = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterRight.Id, newPlaсeAdditionalRebarBottomCenterRight);
                        rebarIdCollection.Add(additionalRebarBottomCenterRight.Id);

                        //Центральный нижний стержень
                        Rebar additionalRebarBottomCenter = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenter.Id, rotateLine, -90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarBottomCenter = new XYZ(0, -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenter.Id, newPlaсeAdditionalRebarBottomCenter);
                        rebarIdCollection.Add(additionalRebarBottomCenter.Id);

                        //Верхняя грань
                        //Центральный верхний левый стержень
                        Rebar additionalRebarTopCenterLeft = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenterLeft.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarTopCenterLeft = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8), columnProperties.ColumnSectionWidth / 2 - squareColumnsReinforcementMainForm.MainRebarCoverLayer - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterLeft.Id, newPlaсeAdditionalRebarTopCenterLeft);
                        rebarIdCollection.Add(additionalRebarTopCenterLeft.Id);

                        //Центральный верхний правый стержень
                        Rebar additionalRebarTopCenterRight = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenterRight.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarTopCenterRight = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterRight.Id, newPlaсeAdditionalRebarTopCenterRight);
                        rebarIdCollection.Add(additionalRebarTopCenterRight.Id);

                        //Центральный верхний левый стержень
                        Rebar additionalRebarTopCenter = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenter.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarTopCenter = new XYZ(0, columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenter.Id, newPlaсeAdditionalRebarTopCenter);
                        rebarIdCollection.Add(additionalRebarTopCenter.Id);
                    }

                    //Если стыковка стержней в нахлест c изменением сечения колонны выше
                    else if (squareColumnsReinforcementMainForm.CheckedRebarOutletsTepeName == "radioButton_Overlapping"
                        & squareColumnsReinforcementMainForm.СhangeSection == true
                        & squareColumnsReinforcementMainForm.СhangeSectionOffset <= 50)
                    {
                        //Точки для построения кривых основных угловых удлиненных стержней
                        XYZ rebar_p1 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z
                            - (squareColumnsReinforcementMainForm.RebarDeepeningOffset / 304.8), 6));
                        XYZ rebar_p2 = new XYZ(Math.Round(rebar_p1.X, 6)
                            , Math.Round(rebar_p1.Y, 6)
                            , Math.Round(rebar_p1.Z
                            + (squareColumnsReinforcementMainForm.RebarDeepeningOffset / 304.8)
                            + columnProperties.ColumnLength
                            - ((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8) * 6
                            - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)), 6));
                        XYZ rebar_p3 = new XYZ(Math.Round(rebar_p2.X
                            + deltaXMainRebarOverlapping, 6)
                            , Math.Round(rebar_p2.Y, 6)
                            , Math.Round(rebar_p2.Z
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)
                            + ((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8) * 6
                            - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)), 6));
                        XYZ rebar_p4L = new XYZ(Math.Round(rebar_p3.X, 6)
                            , Math.Round(rebar_p3.Y, 6)
                            , Math.Round(rebar_p3.Z
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));

                        //Точки для построения кривых основных боковых удлиненных стержней
                        XYZ additionalRebar_p1 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z
                            - (squareColumnsReinforcementMainForm.RebarDeepeningOffset / 304.8), 6));
                        XYZ additionalRebar_p2 = new XYZ(Math.Round(additionalRebar_p1.X, 6)
                            , Math.Round(additionalRebar_p1.Y, 6)
                            , Math.Round(additionalRebar_p1.Z
                            + (squareColumnsReinforcementMainForm.RebarDeepeningOffset / 304.8)
                            + columnProperties.ColumnLength
                            - ((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8) * 6
                            - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)), 6));
                        XYZ additionalRebar_p3 = new XYZ(Math.Round(additionalRebar_p2.X
                            + deltaXAdditionalMainRebarOverlapping, 6)
                            , Math.Round(additionalRebar_p2.Y, 6)
                            , Math.Round(additionalRebar_p2.Z
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)
                            + ((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8) * 6
                            - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)), 6));
                        XYZ additionalRebar_p4L = new XYZ(Math.Round(additionalRebar_p3.X, 6)
                            , Math.Round(additionalRebar_p3.Y, 6)
                            , Math.Round(additionalRebar_p3.Z
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));
                        XYZ additionalRebar_p4S = new XYZ(Math.Round(additionalRebar_p3.X, 6)
                            , Math.Round(additionalRebar_p3.Y, 6)
                            , Math.Round(additionalRebar_p3.Z
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8), 6));

                        //Кривые стержня
                        List<Curve> mainRebarLongCurves = new List<Curve>();
                        List<Curve> additionalRebarLongCurves = new List<Curve>();
                        List<Curve> additionalRebarShortCurves = new List<Curve>();

                        Curve line1 = Line.CreateBound(rebar_p1, rebar_p2) as Curve;
                        mainRebarLongCurves.Add(line1);
                        Curve line2 = Line.CreateBound(rebar_p2, rebar_p3) as Curve;
                        mainRebarLongCurves.Add(line2);
                        Curve line3L = Line.CreateBound(rebar_p3, rebar_p4L) as Curve;
                        mainRebarLongCurves.Add(line3L);

                        Curve additionalLine1 = Line.CreateBound(additionalRebar_p1, additionalRebar_p2) as Curve;
                        additionalRebarLongCurves.Add(additionalLine1);
                        additionalRebarShortCurves.Add(additionalLine1);
                        Curve additionalLine2 = Line.CreateBound(additionalRebar_p2, additionalRebar_p3) as Curve;
                        additionalRebarLongCurves.Add(additionalLine2);
                        additionalRebarShortCurves.Add(additionalLine2);
                        Curve additionalLine3L = Line.CreateBound(additionalRebar_p3, additionalRebar_p4L) as Curve;
                        additionalRebarLongCurves.Add(additionalLine3L);
                        Curve additionalLine3S = Line.CreateBound(additionalRebar_p3, additionalRebar_p4S) as Curve;
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
                        XYZ rotate_p1 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z);
                        XYZ rotate_p2 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z + 1);
                        Line rotateLine = Line.CreateBound(rotate_p1, rotate_p2);
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
                            , mainRebarLongCurves
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
                            , mainRebarLongCurves
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
                        //Центральный левый нижний стержень
                        Rebar additionalRebarLeftCenterDown = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarLeftCenterDown.Id, rotateLine, alphaAdditionalMainRebarOverlapping);
                        XYZ newPlaсeAdditionalRebarLeftCenterDown = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterDown.Id, newPlaсeAdditionalRebarLeftCenterDown);
                        rebarIdCollection.Add(additionalRebarLeftCenterDown.Id);

                        //Центральный левый верхний стержень
                        Rebar additionalRebarLeftCenterUp = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarLeftCenterUp.Id, rotateLine, alphaAdditionalMainRebarOverlapping);
                        XYZ newPlaсeAdditionalRebarLeftCenterUp = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterUp.Id, newPlaсeAdditionalRebarLeftCenterUp);
                        rebarIdCollection.Add(additionalRebarLeftCenterUp.Id);

                        //Центральный левый стержень
                        Rebar additionalRebarLeftCenter = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarLeftCenter.Id, rotateLine, alphaAdditionalMainRebarOverlapping);
                        XYZ newPlaсeAdditionalRebarLeftCenter = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenter.Id, newPlaсeAdditionalRebarLeftCenter);
                        rebarIdCollection.Add(additionalRebarLeftCenter.Id);

                        //Правая грань
                        //Центральный правый нижний стержень
                        Rebar additionalRebarRightCenterDown = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenterDown.Id, rotateLine, -alphaAdditionalMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenterDown.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarRightCenterDown = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterDown.Id, newPlaсeAdditionalRebarRightCenterDown);
                        rebarIdCollection.Add(additionalRebarRightCenterDown.Id);

                        //Центральный правый верхний стержень
                        Rebar additionalRebarRightCenterUp = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenterUp.Id, rotateLine, -alphaAdditionalMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenterUp.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarRightCenterUp = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterUp.Id, newPlaсeAdditionalRebarRightCenterUp);
                        rebarIdCollection.Add(additionalRebarRightCenterUp.Id);

                        // Центральный правый стержень
                        Rebar additionalRebarRightCenter = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenter.Id, rotateLine, -alphaAdditionalMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenter.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarRightCenter = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenter.Id, newPlaсeAdditionalRebarRightCenter);
                        rebarIdCollection.Add(additionalRebarRightCenter.Id);

                        //Нижняя грань
                        //Центральный нижний левый стержень
                        Rebar additionalRebarBottomCenterLeft = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenterLeft.Id, rotateLine, -alphaAdditionalMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenterLeft.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarBottomCenterLeft = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterLeft.Id, newPlaсeAdditionalRebarBottomCenterLeft);
                        rebarIdCollection.Add(additionalRebarBottomCenterLeft.Id);

                        //Центральный нижний правый стержень
                        Rebar additionalRebarBottomCenterRight = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenterRight.Id, rotateLine, -alphaAdditionalMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenterRight.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarBottomCenterRight = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterRight.Id, newPlaсeAdditionalRebarBottomCenterRight);
                        rebarIdCollection.Add(additionalRebarBottomCenterRight.Id);

                        //Центральный нижний стержень
                        Rebar additionalRebarBottomCenter = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenter.Id, rotateLine, -alphaAdditionalMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenter.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarBottomCenter = new XYZ(0, -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenter.Id, newPlaсeAdditionalRebarBottomCenter);
                        rebarIdCollection.Add(additionalRebarBottomCenter.Id);

                        //Верхняя грань
                        //Центральный верхний левый стержень
                        Rebar additionalRebarTopCenterLeft = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenterLeft.Id, rotateLine, alphaAdditionalMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenterLeft.Id, rotateLine, -90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarTopCenterLeft = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterLeft.Id, newPlaсeAdditionalRebarTopCenterLeft);
                        rebarIdCollection.Add(additionalRebarTopCenterLeft.Id);

                        //Центральный верхний правый стержень
                        Rebar additionalRebarTopCenterRight = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenterRight.Id, rotateLine, alphaAdditionalMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenterRight.Id, rotateLine, -90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarTopCenterRight = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterRight.Id, newPlaсeAdditionalRebarTopCenterRight);
                        rebarIdCollection.Add(additionalRebarTopCenterRight.Id);

                        //Центральный верхний стержень
                        Rebar additionalRebarTopCenter = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenter.Id, rotateLine, alphaAdditionalMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenter.Id, rotateLine, -90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarTopCenter = new XYZ(0, columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenter.Id, newPlaсeAdditionalRebarTopCenter);
                        rebarIdCollection.Add(additionalRebarTopCenter.Id);
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
                            , mainRebarLongCurves
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

                        FamilyInstance tubWeldingMainRebarLeftTop = doc.Create.NewFamilyInstance(tubWelding_p1L
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

                        FamilyInstance tubWeldingMainRebarRightBottom = doc.Create.NewFamilyInstance(tubWelding_p1L
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingMainRebarRightBottom.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.MainRebarTapeDiam);
                        ElementTransformUtils.MoveElement(doc, tubWeldingMainRebarRightBottom.Id, newPlaсeMainRebarRightBottom);
                        rebarIdCollection.Add(tubWeldingMainRebarRightBottom.Id);

                        //Левая грань
                        //Центральный левый нижний стержень
                        Rebar additionalRebarLeftCenterDown = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape01
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarLeftCenterDown = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterDown.Id, newPlaсeAdditionalRebarLeftCenterDown);
                        rebarIdCollection.Add(additionalRebarLeftCenterDown.Id);

                        FamilyInstance tubWeldingAdditionalRebarLeftCenterDown = doc.Create.NewFamilyInstance(tubWelding_p1S
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarLeftCenterDown.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarLeftCenterDown.Id, newPlaсeAdditionalRebarLeftCenterDown);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarLeftCenterDown.Id);

                        //Центральный левый верхний стержень
                        Rebar additionalRebarLeftCenterUp = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape01
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right, RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarLeftCenterUp = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterUp.Id, newPlaсeAdditionalRebarLeftCenterUp);
                        rebarIdCollection.Add(additionalRebarLeftCenterUp.Id);

                        FamilyInstance tubWeldingAdditionalRebarLeftCenterUp = doc.Create.NewFamilyInstance(tubWelding_p1S
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarLeftCenterUp.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarLeftCenterUp.Id, newPlaсeAdditionalRebarLeftCenterUp);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarLeftCenterUp.Id);

                        //Центральный левый стержень
                        Rebar additionalRebarLeftCenter = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape01
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal,
                            mainRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarLeftCenter = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenter.Id, newPlaсeAdditionalRebarLeftCenter);
                        rebarIdCollection.Add(additionalRebarLeftCenter.Id);

                        FamilyInstance tubWeldingAdditionalRebarLeftCenter = doc.Create.NewFamilyInstance(tubWelding_p1L
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarLeftCenter.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarLeftCenter.Id, newPlaсeAdditionalRebarLeftCenter);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarLeftCenter.Id);

                        //Правая грань
                        //Центральный правый нижний стержень
                        Rebar additionalRebarRightCenterDown = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape01
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarRightCenterDown = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterDown.Id, newPlaсeAdditionalRebarRightCenterDown);
                        rebarIdCollection.Add(additionalRebarRightCenterDown.Id);

                        FamilyInstance tubWeldingAdditionalRebarRightCenterDown = doc.Create.NewFamilyInstance(tubWelding_p1S
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarRightCenterDown.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarRightCenterDown.Id, newPlaсeAdditionalRebarRightCenterDown);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarRightCenterDown.Id);

                        //Центральный правый верхний стержень
                        Rebar additionalRebarRightCenterUp = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape01
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarRightCenterUp = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterUp.Id, newPlaсeAdditionalRebarRightCenterUp);
                        rebarIdCollection.Add(additionalRebarRightCenterUp.Id);

                        FamilyInstance tubWeldingAdditionalRebarRightCenterUp = doc.Create.NewFamilyInstance(tubWelding_p1S
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarRightCenterUp.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarRightCenterUp.Id, newPlaсeAdditionalRebarRightCenterUp);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarRightCenterUp.Id);

                        //Центральный правый стержень
                        Rebar additionalRebarRightCenter = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape01
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarRightCenter = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenter.Id, newPlaсeAdditionalRebarRightCenter);
                        rebarIdCollection.Add(additionalRebarRightCenter.Id);

                        FamilyInstance tubWeldingAdditionalRebarRightCenter = doc.Create.NewFamilyInstance(tubWelding_p1L
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarRightCenter.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarRightCenter.Id, newPlaсeAdditionalRebarRightCenter);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarRightCenter.Id);

                        //Нижняя грань
                        //Центральный нижний левый стержень
                        Rebar additionalRebarBottomCenterLeft = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape01
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarBottomCenterLeft = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8), -columnProperties.ColumnSectionWidth / 2 + squareColumnsReinforcementMainForm.MainRebarCoverLayer + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterLeft.Id, newPlaсeAdditionalRebarBottomCenterLeft);
                        rebarIdCollection.Add(additionalRebarBottomCenterLeft.Id);

                        FamilyInstance tubWeldingAdditionalRebarBottomCenterLeft = doc.Create.NewFamilyInstance(tubWelding_p1S
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarBottomCenterLeft.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarBottomCenterLeft.Id, newPlaсeAdditionalRebarBottomCenterLeft);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarBottomCenterLeft.Id);

                        //Центральный нижний правый стержень
                        Rebar additionalRebarBottomCenterRight = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape01
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right, RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarBottomCenterRight = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8), -columnProperties.ColumnSectionWidth / 2 + squareColumnsReinforcementMainForm.MainRebarCoverLayer + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterRight.Id, newPlaсeAdditionalRebarBottomCenterRight);
                        rebarIdCollection.Add(additionalRebarBottomCenterRight.Id);

                        FamilyInstance tubWeldingAdditionalRebarBottomCenterRight = doc.Create.NewFamilyInstance(tubWelding_p1S
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarBottomCenterRight.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarBottomCenterRight.Id, newPlaсeAdditionalRebarBottomCenterRight);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarBottomCenterRight.Id);

                        //Центральный нижний стержень
                        Rebar additionalRebarBottomCenter = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape01
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarBottomCenter = new XYZ(0, -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenter.Id, newPlaсeAdditionalRebarBottomCenter);
                        rebarIdCollection.Add(additionalRebarBottomCenter.Id);

                        FamilyInstance tubWeldingAdditionalRebarBottomCenter = doc.Create.NewFamilyInstance(tubWelding_p1L
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarBottomCenter.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarBottomCenter.Id, newPlaсeAdditionalRebarBottomCenter);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarBottomCenter.Id);

                        //Верхняя грань
                        //Центральный верхний левый стержень
                        Rebar additionalRebarTopCenterLeft = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape01
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarTopCenterLeft = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8), columnProperties.ColumnSectionWidth / 2 - squareColumnsReinforcementMainForm.MainRebarCoverLayer - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterLeft.Id, newPlaсeAdditionalRebarTopCenterLeft);
                        rebarIdCollection.Add(additionalRebarTopCenterLeft.Id);

                        FamilyInstance tubWeldingAdditionalRebarTopCenterLeft = doc.Create.NewFamilyInstance(tubWelding_p1S
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarTopCenterLeft.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarTopCenterLeft.Id, newPlaсeAdditionalRebarTopCenterLeft);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarTopCenterLeft.Id);

                        //Центральный верхний правый стержень
                        Rebar additionalRebarTopCenterRight = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape01
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarTopCenterRight = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8), columnProperties.ColumnSectionWidth / 2 - squareColumnsReinforcementMainForm.MainRebarCoverLayer - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterRight.Id, newPlaсeAdditionalRebarTopCenterRight);
                        rebarIdCollection.Add(additionalRebarTopCenterRight.Id);

                        FamilyInstance tubWeldingAdditionalRebarTopCenterRight = doc.Create.NewFamilyInstance(tubWelding_p1S
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarTopCenterRight.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarTopCenterRight.Id, newPlaсeAdditionalRebarTopCenterRight);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarTopCenterRight.Id);

                        //Центральный верхний стержень
                        Rebar additionalRebarTopCenter = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape01
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarTopCenter = new XYZ(0, columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenter.Id, newPlaсeAdditionalRebarTopCenter);
                        rebarIdCollection.Add(additionalRebarTopCenter.Id);

                        FamilyInstance tubWeldingAdditionalRebarTopCenter = doc.Create.NewFamilyInstance(tubWelding_p1L
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarTopCenter.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarTopCenter.Id, newPlaсeAdditionalRebarTopCenter);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarTopCenter.Id);
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
                        List<Curve> additionalRebarLongCurves = new List<Curve>();
                        List<Curve> additionalRebarShortCurves = new List<Curve>();

                        Curve line1L = Line.CreateBound(rebar_p1L, rebar_p2L) as Curve;
                        mainRebarLongCurves.Add(line1L);
                        Curve line2L = Line.CreateBound(rebar_p2L, rebar_p3L) as Curve;
                        mainRebarLongCurves.Add(line2L);

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
                        XYZ rotate_p1 = new XYZ(rebar_p1L.X, rebar_p1L.Y, rebar_p1L.Z);
                        XYZ rotate_p2 = new XYZ(rebar_p1L.X, rebar_p1L.Y, rebar_p1L.Z + 1);
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
                        //Центральный левый нижний стержень
                        Rebar additionalRebarLeftCenterDown = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarLeftCenterDown.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarLeftCenterDown = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterDown.Id, newPlaсeAdditionalRebarLeftCenterDown);
                        rebarIdCollection.Add(additionalRebarLeftCenterDown.Id);

                        //Центральный левый верхний стержень
                        Rebar additionalRebarLeftCenterUp = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarLeftCenterUp.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarLeftCenterUp = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterUp.Id, newPlaсeAdditionalRebarLeftCenterUp);
                        rebarIdCollection.Add(additionalRebarLeftCenterUp.Id);

                        //Центральный левый стержень
                        Rebar additionalRebarLeftCenter = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarLeftCenter.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarLeftCenter = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenter.Id, newPlaсeAdditionalRebarLeftCenter);
                        rebarIdCollection.Add(additionalRebarLeftCenter.Id);

                        //Правая грань
                        //Центральный правый нижний стержень
                        Rebar additionalRebarRightCenterDown = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarRightCenterDown = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterDown.Id, newPlaсeAdditionalRebarRightCenterDown);
                        rebarIdCollection.Add(additionalRebarRightCenterDown.Id);

                        //Центральный правый верхний стержень
                        Rebar additionalRebarRightCenterUp = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarRightCenterUp = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterUp.Id, newPlaсeAdditionalRebarRightCenterUp);
                        rebarIdCollection.Add(additionalRebarRightCenterUp.Id);

                        //Центральный правый стержень
                        Rebar additionalRebarRightCenter = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarRightCenter = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenter.Id, newPlaсeAdditionalRebarRightCenter);
                        rebarIdCollection.Add(additionalRebarRightCenter.Id);

                        //Нижняя грань
                        //Центральный нижний левый стержень
                        Rebar additionalRebarBottomCenterLeft = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenterLeft.Id, rotateLine, -90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarBottomCenterLeft = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterLeft.Id, newPlaсeAdditionalRebarBottomCenterLeft);
                        rebarIdCollection.Add(additionalRebarBottomCenterLeft.Id);

                        //Центральный нижний правый стержень
                        Rebar additionalRebarBottomCenterRight = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenterRight.Id, rotateLine, -90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarBottomCenterRight = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterRight.Id, newPlaсeAdditionalRebarBottomCenterRight);
                        rebarIdCollection.Add(additionalRebarBottomCenterRight.Id);

                        //Центральный нижний стержень
                        Rebar additionalRebarBottomCenter = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenter.Id, rotateLine, -90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarBottomCenter = new XYZ(0, -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenter.Id, newPlaсeAdditionalRebarBottomCenter);
                        rebarIdCollection.Add(additionalRebarBottomCenter.Id);

                        //Верхняя грань
                        //Центральный верхний левый стержень
                        Rebar additionalRebarTopCenterLeft = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenterLeft.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarTopCenterLeft = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterLeft.Id, newPlaсeAdditionalRebarTopCenterLeft);
                        rebarIdCollection.Add(additionalRebarTopCenterLeft.Id);

                        //Центральный верхний правый стержень
                        Rebar additionalRebarTopCenterRight = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenterRight.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarTopCenterRight = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterRight.Id, newPlaсeAdditionalRebarTopCenterRight);
                        rebarIdCollection.Add(additionalRebarTopCenterRight.Id);

                        //Центральный верхний стержень
                        Rebar additionalRebarTopCenter = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape11
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenter.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarTopCenter = new XYZ(0, columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenter.Id, newPlaсeAdditionalRebarTopCenter);
                        rebarIdCollection.Add(additionalRebarTopCenter.Id);
                    }

                    //Переход со сварки на нахлест без изменения сечения колонны
                    else if (squareColumnsReinforcementMainForm.CheckedRebarOutletsTepeName == "radioButton_Welding"
                        & squareColumnsReinforcementMainForm.СhangeSection == false
                        & squareColumnsReinforcementMainForm.TransitionToOverlap == true)
                    {
                        //Точки для построения кривых основных угловых удлинненных стержней
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

                        //Точки для построения кривых основных боковых удлинненных стержней
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

                        //Точки для построения кривых основных боковых укороченных стержней
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
                        List<Curve> additionalRebarLongCurves = new List<Curve>();
                        List<Curve> additionalRebarShortCurves = new List<Curve>();

                        Curve line1L = Line.CreateBound(rebar_p1L, rebar_p2L) as Curve;
                        mainRebarLongCurves.Add(line1L);
                        Curve line2L = Line.CreateBound(rebar_p2L, rebar_p3L) as Curve;
                        mainRebarLongCurves.Add(line2L);
                        Curve line3L = Line.CreateBound(rebar_p3L, rebar_p4L) as Curve;
                        mainRebarLongCurves.Add(line3L);

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
                            , mainRebarLongCurves
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
                            , mainRebarLongCurves
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
                        //Центральный левый нижний стержень
                        Rebar additionalRebarLeftCenterDown = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarLeftCenterDown.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarLeftCenterDown = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterDown.Id, newPlaсeAdditionalRebarLeftCenterDown);
                        rebarIdCollection.Add(additionalRebarLeftCenterDown.Id);

                        //Центральный левый верхний стержень
                        Rebar additionalRebarLeftCenterUp = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarLeftCenterUp.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarLeftCenterUp = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterUp.Id, newPlaсeAdditionalRebarLeftCenterUp);
                        rebarIdCollection.Add(additionalRebarLeftCenterUp.Id);

                        //Центральный левый стержень
                        Rebar additionalRebarLeftCenter = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarLeftCenter.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarLeftCenter = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenter.Id, newPlaсeAdditionalRebarLeftCenter);
                        rebarIdCollection.Add(additionalRebarLeftCenter.Id);

                        //Правая грань
                        //Центральный правый нижний стержень
                        Rebar additionalRebarRightCenterDown = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenterDown.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarRightCenterDown = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterDown.Id, newPlaсeAdditionalRebarRightCenterDown);
                        rebarIdCollection.Add(additionalRebarRightCenterDown.Id);

                        //Центральный правый верхний стержень
                        Rebar additionalRebarRightCenterUp = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenterUp.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarRightCenterUp = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterUp.Id, newPlaсeAdditionalRebarRightCenterUp);
                        rebarIdCollection.Add(additionalRebarRightCenterUp.Id);

                        //Центральный правый стержень
                        Rebar additionalRebarRightCenter = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenter.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarRightCenter = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenter.Id, newPlaсeAdditionalRebarRightCenter);
                        rebarIdCollection.Add(additionalRebarRightCenter.Id);

                        //Нижняя грань
                        //Центральный нижний левый стержень
                        Rebar additionalRebarBottomCenterLeft = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarBottomCenterLeft = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterLeft.Id, newPlaсeAdditionalRebarBottomCenterLeft);
                        rebarIdCollection.Add(additionalRebarBottomCenterLeft.Id);

                        //Центральный нижний правый стержень
                        Rebar additionalRebarBottomCenterRight = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarBottomCenterRight = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterRight.Id, newPlaсeAdditionalRebarBottomCenterRight);
                        rebarIdCollection.Add(additionalRebarBottomCenterRight.Id);

                        //Центральный нижний стержень
                        Rebar additionalRebarBottomCenter = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarBottomCenter = new XYZ(0, -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenter.Id, newPlaсeAdditionalRebarBottomCenter);
                        rebarIdCollection.Add(additionalRebarBottomCenter.Id);

                        //Верхняя грань
                        //Центральный верхний левый стержень
                        Rebar additionalRebarTopCenterLeft = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right, RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarTopCenterLeft = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterLeft.Id, newPlaсeAdditionalRebarTopCenterLeft);
                        rebarIdCollection.Add(additionalRebarTopCenterLeft.Id);

                        //Центральный верхний правый стержень
                        Rebar additionalRebarTopCenterRight = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarTopCenterRight = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterRight.Id, newPlaсeAdditionalRebarTopCenterRight);
                        rebarIdCollection.Add(additionalRebarTopCenterRight.Id);

                        //Центральный верхний левый стержень
                        Rebar additionalRebarTopCenter = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarTopCenter = new XYZ(0, columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenter.Id, newPlaсeAdditionalRebarTopCenter);
                        rebarIdCollection.Add(additionalRebarTopCenter.Id);
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
                        XYZ rebar_p3L = new XYZ(Math.Round(rebar_p2L.X + deltaXWelding, 6)
                            , Math.Round(rebar_p2L.Y, 6)
                            , Math.Round(rebar_p2L.Z 
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8) 
                            + ((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8) * 6 
                            - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)), 6));
                        XYZ rebar_p4L = new XYZ(Math.Round(rebar_p3L.X, 6)
                            , Math.Round(rebar_p3L.Y, 6)
                            , Math.Round(rebar_p3L.Z 
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));

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
                        List<Curve> additionalRebarLongCurves = new List<Curve>();
                        List<Curve> additionalRebarShortCurves = new List<Curve>();

                        Curve line1L = Line.CreateBound(rebar_p1L, rebar_p2L) as Curve;
                        mainRebarLongCurves.Add(line1L);
                        Curve line2L = Line.CreateBound(rebar_p2L, rebar_p3L) as Curve;
                        mainRebarLongCurves.Add(line2L);
                        Curve line3L = Line.CreateBound(rebar_p3L, rebar_p4L) as Curve;
                        mainRebarLongCurves.Add(line3L);

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
                        XYZ newPlaсeTubWeldingMainRebarLeftBottom = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2 
                            + (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2 
                            + (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, tubWeldingMainRebarLeftBottom.Id, newPlaсeTubWeldingMainRebarLeftBottom);
                        rebarIdCollection.Add(tubWeldingMainRebarLeftBottom.Id);

                        //Верхний левый угол
                        Rebar mainRebarLeftTop = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.MainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarLongCurves
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

                        FamilyInstance tubWeldingMainRebarLeftTop = doc.Create.NewFamilyInstance(tubWelding_p1L
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingMainRebarLeftTop.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.MainRebarTapeDiam);
                        XYZ newPlaсeTubWeldingMainRebarLeftTop = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2 
                            + (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2 
                            - (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, tubWeldingMainRebarLeftTop.Id, newPlaсeTubWeldingMainRebarLeftTop);
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
                        XYZ newPlaсeTubWeldingMainRebarRightTop = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2 
                            - (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2 
                            - (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, tubWeldingMainRebarRightTop.Id, newPlaсeTubWeldingMainRebarRightTop);
                        rebarIdCollection.Add(tubWeldingMainRebarRightTop.Id);

                        //Нижний правый угол
                        Rebar mainRebarRightBottom = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.MainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , mainRebarLongCurves
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

                        FamilyInstance tubWeldingMainRebarRightBottom = doc.Create.NewFamilyInstance(tubWelding_p1L
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingMainRebarRightBottom.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.MainRebarTapeDiam);
                        XYZ newPlaсeTubWeldingMainRebarRightBottom = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2 
                            - (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2 
                            + (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, tubWeldingMainRebarRightBottom.Id, newPlaсeTubWeldingMainRebarRightBottom);
                        rebarIdCollection.Add(tubWeldingMainRebarRightBottom.Id);

                        //Левая грань
                        //Центральный левый нижний стержень
                        Rebar additionalRebarLeftCenterDown = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarLeftCenterDown = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterDown.Id, newPlaсeAdditionalRebarLeftCenterDown);
                        rebarIdCollection.Add(additionalRebarLeftCenterDown.Id);

                        FamilyInstance tubWeldingAdditionalRebarLeftCenterDown = doc.Create.NewFamilyInstance(tubWelding_p1S
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarLeftCenterDown.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        XYZ newPlaсeTubWeldingAdditionalRebarLeftCenterDown = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2 
                            + (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8)
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarLeftCenterDown.Id, newPlaсeTubWeldingAdditionalRebarLeftCenterDown);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarLeftCenterDown.Id);

                        //Центральный левый верхний стержень
                        Rebar additionalRebarLeftCenterUp = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarLeftCenterUp = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterUp.Id, newPlaсeAdditionalRebarLeftCenterUp);
                        rebarIdCollection.Add(additionalRebarLeftCenterUp.Id);

                        FamilyInstance tubWeldingAdditionalRebarLeftCenterUp = doc.Create.NewFamilyInstance(tubWelding_p1S
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarLeftCenterUp.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        XYZ newPlaсeTubWeldingAdditionalRebarLeftCenterUp = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2 
                            + (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8)
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarLeftCenterUp.Id, newPlaсeTubWeldingAdditionalRebarLeftCenterUp);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarLeftCenterUp.Id);

                        //Центральный левый стержень
                        Rebar additionalRebarLeftCenter = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeAdditionalRebarLeftCenter = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenter.Id, newPlaсeAdditionalRebarLeftCenter);
                        rebarIdCollection.Add(additionalRebarLeftCenter.Id);

                        FamilyInstance tubWeldingAdditionalRebarLeftCenter = doc.Create.NewFamilyInstance(tubWelding_p1L
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarLeftCenter.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        XYZ newPlaсeTubWeldingAdditionalRebarLeftCenter = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2 
                            + (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8), 0, 0);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarLeftCenter.Id, newPlaсeTubWeldingAdditionalRebarLeftCenter);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarLeftCenter.Id);

                        //Правая грань
                        //Центральный правый нижний стержень
                        Rebar additionalRebarRightCenterDown = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenterDown.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarRightCenterDown = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterDown.Id, newPlaсeAdditionalRebarRightCenterDown);
                        rebarIdCollection.Add(additionalRebarRightCenterDown.Id);

                        FamilyInstance tubWeldingAdditionalRebarRightCenterDown = doc.Create.NewFamilyInstance(tubWelding_p1S
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarRightCenterDown.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        XYZ newPlaсeTubWeldingAdditionalRebarRightCenterDown = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2 
                            - (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8)
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarRightCenterDown.Id, newPlaсeTubWeldingAdditionalRebarRightCenterDown);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarRightCenterDown.Id);

                        //Центральный правый верхний стержень
                        Rebar additionalRebarRightCenterUp = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenterUp.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarRightCenterUp = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterUp.Id, newPlaсeAdditionalRebarRightCenterUp);
                        rebarIdCollection.Add(additionalRebarRightCenterUp.Id);

                        FamilyInstance tubWeldingAdditionalRebarRightCenterUp = doc.Create.NewFamilyInstance(tubWelding_p1S
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarRightCenterUp.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        XYZ newPlaсeTubWeldingAdditionalRebarRightCenterUp = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2 
                            - (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8)
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarRightCenterUp.Id, newPlaсeTubWeldingAdditionalRebarRightCenterUp);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarRightCenterUp.Id);

                        //Центральный правый стержень
                        Rebar additionalRebarRightCenter = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenter.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarRightCenter = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenter.Id, newPlaсeAdditionalRebarRightCenter);
                        rebarIdCollection.Add(additionalRebarRightCenter.Id);

                        FamilyInstance tubWeldingAdditionalRebarRightCenter = doc.Create.NewFamilyInstance(tubWelding_p1L
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarRightCenter.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        XYZ newPlaсeTubWeldingAdditionalRebarRightCenter = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2 
                            - (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8), 0, 0);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarRightCenter.Id, newPlaсeTubWeldingAdditionalRebarRightCenter);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarRightCenter.Id);

                        //Нижняя грань
                        //Центральный нижний левый стержень
                        Rebar additionalRebarBottomCenterLeft = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenterLeft.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarBottomCenterLeft = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterLeft.Id, newPlaсeAdditionalRebarBottomCenterLeft);
                        rebarIdCollection.Add(additionalRebarBottomCenterLeft.Id);

                        FamilyInstance tubWeldingAdditionalRebarBottomCenterLeft = doc.Create.NewFamilyInstance(tubWelding_p1S
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarBottomCenterLeft.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        XYZ newPlaсeTubWeldingAdditionalRebarBottomCenterLeft = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2 
                            + (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarBottomCenterLeft.Id, newPlaсeTubWeldingAdditionalRebarBottomCenterLeft);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarBottomCenterLeft.Id);

                        //Центральный нижний правый стержень
                        Rebar additionalRebarBottomCenterRight = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenterRight.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarBottomCenterRight = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterRight.Id, newPlaсeAdditionalRebarBottomCenterRight);
                        rebarIdCollection.Add(additionalRebarBottomCenterRight.Id);

                        FamilyInstance tubWeldingAdditionalRebarBottomCenterRight = doc.Create.NewFamilyInstance(tubWelding_p1S
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarBottomCenterRight.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        XYZ newPlaсeTubWeldingAdditionalRebarBottomCenterRight = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2 
                            + (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarBottomCenterRight.Id, newPlaсeTubWeldingAdditionalRebarBottomCenterRight);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarBottomCenterRight.Id);

                        //Центральный нижний стержень
                        Rebar additionalRebarBottomCenter = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right, RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenter.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarBottomCenter = new XYZ(0, -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenter.Id, newPlaсeAdditionalRebarBottomCenter);
                        rebarIdCollection.Add(additionalRebarBottomCenter.Id);

                        FamilyInstance tubWeldingAdditionalRebarBottomCenter = doc.Create.NewFamilyInstance(tubWelding_p1L
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarBottomCenter.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        XYZ newPlaсeTubWeldingAdditionalRebarBottomCenter = new XYZ(0, -columnProperties.ColumnSectionWidth / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2 
                            + (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarBottomCenter.Id, newPlaсeTubWeldingAdditionalRebarBottomCenter);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarBottomCenter.Id);

                        //Верхняя грань
                        //Центральный верхний левый стержень
                        Rebar additionalRebarTopCenterLeft = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenterLeft.Id, rotateLine, -90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarTopCenterLeft = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterLeft.Id, newPlaсeAdditionalRebarTopCenterLeft);
                        rebarIdCollection.Add(additionalRebarTopCenterLeft.Id);

                        FamilyInstance tubWeldingAdditionalRebarTopCenterLeft = doc.Create.NewFamilyInstance(tubWelding_p1S
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarTopCenterLeft.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        XYZ newPlaсeTubWeldingAdditionalRebarTopCenterLeft = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2 
                            - (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarTopCenterLeft.Id, newPlaсeTubWeldingAdditionalRebarTopCenterLeft);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarTopCenterLeft.Id);

                        //Центральный верхний стержень
                        Rebar additionalRebarTopCenterRight = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenterRight.Id, rotateLine, -90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarTopCenterRight = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterRight.Id, newPlaсeAdditionalRebarTopCenterRight);
                        rebarIdCollection.Add(additionalRebarTopCenterRight.Id);

                        FamilyInstance tubWeldingAdditionalRebarTopCenterRight = doc.Create.NewFamilyInstance(tubWelding_p1S
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel, StructuralType.NonStructural);
                        tubWeldingAdditionalRebarTopCenterRight.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        XYZ newPlaсeTubWeldingAdditionalRebarTopCenterRight = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2 
                            - (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarTopCenterRight.Id, newPlaсeTubWeldingAdditionalRebarTopCenterRight);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarTopCenterRight.Id);

                        // Центральный верхний левый стержень
                        Rebar additionalRebarTopCenter = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenter.Id, rotateLine, -90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarTopCenter = new XYZ(0, columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenter.Id, newPlaсeAdditionalRebarTopCenter);
                        rebarIdCollection.Add(additionalRebarTopCenter.Id);

                        FamilyInstance tubWeldingAdditionalRebarTopCenter = doc.Create.NewFamilyInstance(tubWelding_p1L
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWeldingAdditionalRebarTopCenter.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam);
                        XYZ newPlaсeTubWeldingAdditionalRebarTopCenter = new XYZ(0, columnProperties.ColumnSectionWidth / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2 
                            - (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, tubWeldingAdditionalRebarTopCenter.Id, newPlaсeTubWeldingAdditionalRebarTopCenter);
                        rebarIdCollection.Add(tubWeldingAdditionalRebarTopCenter.Id);
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
                        List<Curve> additionalRebarLongCurves = new List<Curve>();
                        List<Curve> additionalRebarShortCurves = new List<Curve>();

                        //Кривые основных угловых удлиненных стержней
                        Curve line1L = Line.CreateBound(rebar_p1L, rebar_p2L) as Curve;
                        mainRebarLongCurves.Add(line1L);
                        Curve line2L = Line.CreateBound(rebar_p2L, rebar_p3L) as Curve;
                        mainRebarLongCurves.Add(line2L);
                        Curve line3L = Line.CreateBound(rebar_p3L, rebar_p4L) as Curve;
                        mainRebarLongCurves.Add(line3L);

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
                        XYZ rotate_p1 = new XYZ(rebar_p1L.X, rebar_p1L.Y, rebar_p1L.Z);
                        XYZ rotate_p2 = new XYZ(rebar_p1L.X, rebar_p1L.Y, rebar_p1L.Z + 1);
                        Line rotateLine = Line.CreateBound(rotate_p1, rotate_p2);
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
                            , mainRebarLongCurves
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
                            , mainRebarLongCurves
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
                        //Центральный левый нижний стержень
                        Rebar additionalRebarLeftCenterDown = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarLeftCenterDown.Id, rotateLine, alphaAdditionalMainRebarOverlapping);
                        XYZ newPlaсeAdditionalRebarLeftCenterDown = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterDown.Id, newPlaсeAdditionalRebarLeftCenterDown);
                        rebarIdCollection.Add(additionalRebarLeftCenterDown.Id);

                        //Центральный левый верхний стержень
                        Rebar additionalRebarLeftCenterUp = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarLeftCenterUp.Id, rotateLine, alphaAdditionalMainRebarOverlapping);
                        XYZ newPlaсeAdditionalRebarLeftCenterUp = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenterUp.Id, newPlaсeAdditionalRebarLeftCenterUp);
                        rebarIdCollection.Add(additionalRebarLeftCenterUp.Id);

                        //Центральный левый стержень
                        Rebar additionalRebarLeftCenter = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarLeftCenter.Id, rotateLine, alphaAdditionalMainRebarOverlapping);
                        XYZ newPlaсeAdditionalRebarLeftCenter = new XYZ(-columnProperties.ColumnSectionHeight / 2 
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarLeftCenter.Id, newPlaсeAdditionalRebarLeftCenter);
                        rebarIdCollection.Add(additionalRebarLeftCenter.Id);

                        //Правая грань
                        //Центральный правый нижний стержень
                        Rebar additionalRebarRightCenterDown = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenterDown.Id, rotateLine, -alphaAdditionalMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenterDown.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarRightCenterDown = new XYZ(columnProperties.ColumnSectionHeight / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , -(squareColumnsReinforcementMainForm.FirstDownRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterDown.Id, newPlaсeAdditionalRebarRightCenterDown);
                        rebarIdCollection.Add(additionalRebarRightCenterDown.Id);

                        //Центральный правый верхний стержень
                        Rebar additionalRebarRightCenterUp = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right, RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenterUp.Id, rotateLine, -alphaAdditionalMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenterUp.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarRightCenterUp = new XYZ(columnProperties.ColumnSectionHeight / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2
                            , (squareColumnsReinforcementMainForm.FirstUpRebarOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenterUp.Id, newPlaсeAdditionalRebarRightCenterUp);
                        rebarIdCollection.Add(additionalRebarRightCenterUp.Id);

                        // Центральный правый стержень
                        Rebar additionalRebarRightCenter = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenter.Id, rotateLine, -alphaAdditionalMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, additionalRebarRightCenter.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarRightCenter = new XYZ(columnProperties.ColumnSectionHeight / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarRightCenter.Id, newPlaсeAdditionalRebarRightCenter);
                        rebarIdCollection.Add(additionalRebarRightCenter.Id);

                        //Нижняя грань
                        //Центральный нижний левый стержень
                        Rebar additionalRebarBottomCenterLeft = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenterLeft.Id, rotateLine, -alphaAdditionalMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenterLeft.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarBottomCenterLeft = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterLeft.Id, newPlaсeAdditionalRebarBottomCenterLeft);
                        rebarIdCollection.Add(additionalRebarBottomCenterLeft.Id);

                        //Центральный нижний правый стержень
                        Rebar additionalRebarBottomCenterRight = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenterRight.Id, rotateLine, -alphaAdditionalMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenterRight.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarBottomCenterRight = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            , -columnProperties.ColumnSectionWidth / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenterRight.Id, newPlaсeAdditionalRebarBottomCenterRight);
                        rebarIdCollection.Add(additionalRebarBottomCenterRight.Id);

                        //Центральный нижний стержень
                        Rebar additionalRebarBottomCenter = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenter.Id, rotateLine, -alphaAdditionalMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, additionalRebarBottomCenter.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarBottomCenter = new XYZ(0, -columnProperties.ColumnSectionWidth / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            + squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarBottomCenter.Id, newPlaсeAdditionalRebarBottomCenter);
                        rebarIdCollection.Add(additionalRebarBottomCenter.Id);

                        //Верхняя грань
                        //Центральный верхний левый стержень
                        Rebar additionalRebarTopCenterLeft = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenterLeft.Id, rotateLine, alphaAdditionalMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenterLeft.Id, rotateLine, -90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarTopCenterLeft = new XYZ(-(squareColumnsReinforcementMainForm.FirstLeftRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2 
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterLeft.Id, newPlaсeAdditionalRebarTopCenterLeft);
                        rebarIdCollection.Add(additionalRebarTopCenterLeft.Id);

                        //Центральный верхний правый стержень
                        Rebar additionalRebarTopCenterRight = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarShortCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenterRight.Id, rotateLine, alphaAdditionalMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenterRight.Id, rotateLine, -90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarTopCenterRight = new XYZ((squareColumnsReinforcementMainForm.FirstRightRebarOffset / 304.8)
                            , columnProperties.ColumnSectionWidth / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer 
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenterRight.Id, newPlaсeAdditionalRebarTopCenterRight);
                        rebarIdCollection.Add(additionalRebarTopCenterRight.Id);

                        //Центральный верхний стержень
                        Rebar additionalRebarTopCenter = Rebar.CreateFromCurvesAndShape(doc
                            , rebarShapeCollector.Shape26
                            , squareColumnsReinforcementMainForm.AdditionalMainRebarTape
                            , null
                            , null
                            , column
                            , mainRebarNormal
                            , additionalRebarLongCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenter.Id, rotateLine, alphaAdditionalMainRebarOverlapping);
                        ElementTransformUtils.RotateElement(doc, additionalRebarTopCenter.Id, rotateLine, -90 * (Math.PI / 180));
                        XYZ newPlaсeAdditionalRebarTopCenter = new XYZ(0, columnProperties.ColumnSectionWidth / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            - squareColumnsReinforcementMainForm.AdditionalMainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, additionalRebarTopCenter.Id, newPlaсeAdditionalRebarTopCenter);
                        rebarIdCollection.Add(additionalRebarTopCenter.Id);
                    }

                    //Хомуты
                    SquareColumnsReinforcementCommand.CreateEncircleStirrup(doc
                        , rebarShapeCollector
                        , column
                        , columnProperties
                        , squareColumnsReinforcementMainForm
                        , rebarIdCollection);
                    SquareColumnsReinforcementCommand.CreateRhombicStirrup(doc
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
