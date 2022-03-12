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
    class SquareColumnsReinforcementCommandT1 : IExternalCommand
    {
        List<FamilyInstance> columnsList;
        SquareColumnsReinforcementMainForm squareColumnsReinforcementMainForm;

        public SquareColumnsReinforcementCommandT1 (List<FamilyInstance> columnsListFromMainComand, SquareColumnsReinforcementMainForm formFromMainComand)
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
                t.Start("Армирование колонны Тип1");
                foreach(FamilyInstance column in columnsList)
                {
                    ColumnProperties columnProperties = new ColumnProperties(doc, column);

                    if (columnProperties.ColumnSectionWidth != columnProperties.ColumnSectionHeight)
                    {
                        continue;
                    }

                    if (columnProperties.ColumnSectionWidth > 400 / 304.8)
                    {
                        TaskDialog.Show("Revit", "Выбранный тип армирования применим для колонн сечением не более 400 мм");
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
                    double deltaXWelding = Math.Sqrt(Math.Pow((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8), 2)
                        + Math.Pow((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8), 2));
                    double alphaWelding = Math.Asin((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8) / deltaXWelding);

                    //Если стыковка стержней в нахлест без изменения сечения колонны выше
                    if (squareColumnsReinforcementMainForm.CheckedRebarOutletsTepeName == "radioButton_Overlapping"
                        & squareColumnsReinforcementMainForm.СhangeSection == false
                        & squareColumnsReinforcementMainForm.BendIntoSlab == false)
                    {
                        //Точки для построения кривых стержня
                        XYZ rebar_p1 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z
                            - (squareColumnsReinforcementMainForm.RebarDeepeningOffset / 304.8), 6));

                        XYZ rebar_p2 = new XYZ(Math.Round(rebar_p1.X, 6), Math.Round(rebar_p1.Y, 6)
                            , Math.Round(rebar_p1.Z
                            + (squareColumnsReinforcementMainForm.RebarDeepeningOffset / 304.8)
                            + columnProperties.ColumnLength, 6));

                        XYZ rebar_p3 = new XYZ(Math.Round(rebar_p2.X
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam, 6)
                            , Math.Round(rebar_p2.Y, 6)
                            , Math.Round(rebar_p2.Z
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8), 6));

                        XYZ rebar_p4L = new XYZ(Math.Round(rebar_p3.X, 6), Math.Round(rebar_p3.Y, 6)
                            , Math.Round(rebar_p3.Z
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));

                        XYZ rebar_p4S = new XYZ(Math.Round(rebar_p3.X, 6), Math.Round(rebar_p3.Y, 6)
                            , Math.Round(rebar_p3.Z
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8), 6));

                        //Кривые стержня
                        List<Curve> mainRebarLongCurves = new List<Curve>();
                        List<Curve> mainRebarShortCurves = new List<Curve>();

                        Curve line1 = Line.CreateBound(rebar_p1, rebar_p2) as Curve;
                        mainRebarLongCurves.Add(line1);
                        mainRebarShortCurves.Add(line1);
                        Curve line2 = Line.CreateBound(rebar_p2, rebar_p3) as Curve;
                        mainRebarLongCurves.Add(line2);
                        mainRebarShortCurves.Add(line2);
                        Curve line3L = Line.CreateBound(rebar_p3, rebar_p4L) as Curve;
                        mainRebarLongCurves.Add(line3L);
                        Curve line3S = Line.CreateBound(rebar_p3, rebar_p4S) as Curve;
                        mainRebarShortCurves.Add(line3S);

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
                    }

                    //Если стыковка стержней в нахлест с загибом в плиту
                    else if (squareColumnsReinforcementMainForm.CheckedRebarOutletsTepeName == "radioButton_Overlapping"
                        & squareColumnsReinforcementMainForm.СhangeSection == false
                        & squareColumnsReinforcementMainForm.BendIntoSlab == true)
                    {
                        //Точки для построения кривых стержня
                        XYZ rebar_p1 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z - (squareColumnsReinforcementMainForm.RebarDeepeningOffset / 304.8), 6));
                        XYZ rebar_p2 = new XYZ(Math.Round(rebar_p1.X, 6)
                            , Math.Round(rebar_p1.Y, 6)
                            , Math.Round(rebar_p1.Z
                            + (squareColumnsReinforcementMainForm.RebarDeepeningOffset / 304.8)
                            + columnProperties.ColumnLength
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)
                            - (squareColumnsReinforcementMainForm.BendIntoSlabOffset / 304.8)
                            - (squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2), 6));
                        XYZ rebar_p3L = new XYZ(Math.Round(rebar_p2.X
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8)
                            - ((squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)
                            - (squareColumnsReinforcementMainForm.BendIntoSlabOffset / 304.8)
                            + (squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2)), 6)
                            , Math.Round(rebar_p2.Y, 6), Math.Round(rebar_p2.Z, 6));
                        XYZ rebar_p3S = new XYZ(Math.Round(rebar_p2.X
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8)
                            - ((squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)
                            - (squareColumnsReinforcementMainForm.BendIntoSlabOffset / 304.8)
                            + (squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2)), 6)
                            , Math.Round(rebar_p2.Y, 6), Math.Round(rebar_p2.Z, 6));

                        //Кривые стержня
                        List<Curve> mainRebarLongCurves = new List<Curve>();
                        List<Curve> mainRebarShortCurves = new List<Curve>();

                        Curve line1 = Line.CreateBound(rebar_p1, rebar_p2) as Curve;
                        mainRebarLongCurves.Add(line1);
                        mainRebarShortCurves.Add(line1);
                        Curve line2L = Line.CreateBound(rebar_p2, rebar_p3L) as Curve;
                        mainRebarLongCurves.Add(line2L);
                        Curve line2S = Line.CreateBound(rebar_p2, rebar_p3S) as Curve;
                        mainRebarShortCurves.Add(line2S);

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
                    }

                    //Если стыковка стержней в нахлест c изменением сечения колонны выше
                    else if (squareColumnsReinforcementMainForm.CheckedRebarOutletsTepeName == "radioButton_Overlapping"
                        & squareColumnsReinforcementMainForm.СhangeSection == true
                        & squareColumnsReinforcementMainForm.СhangeSectionOffset <= 50)
                    {
                        //Точки для построения кривых стержня

                        XYZ rebar_p1 = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z - (squareColumnsReinforcementMainForm.RebarDeepeningOffset / 304.8), 6));
                        XYZ rebar_p2 = new XYZ(Math.Round(rebar_p1.X, 6)
                            , Math.Round(rebar_p1.Y, 6)
                            , Math.Round(rebar_p1.Z
                            + (squareColumnsReinforcementMainForm.RebarDeepeningOffset / 304.8)
                            + columnProperties.ColumnLength
                            - ((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8) * 6
                            - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)), 6));
                        XYZ rebar_p3 = new XYZ(Math.Round(rebar_p2.X + deltaXMainRebarOverlapping, 6)
                            , Math.Round(rebar_p2.Y, 6)
                            , Math.Round(rebar_p2.Z
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)
                            + ((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8) * 6
                            - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)), 6));
                        XYZ rebar_p4L = new XYZ(Math.Round(rebar_p3.X, 6)
                            , Math.Round(rebar_p3.Y, 6)
                            , Math.Round(rebar_p3.Z + (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));
                        XYZ rebar_p4S = new XYZ(Math.Round(rebar_p3.X, 6)
                            , Math.Round(rebar_p3.Y, 6)
                            , Math.Round(rebar_p3.Z + (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8), 6));

                        //Кривые стержня
                        List<Curve> mainRebarLongCurves = new List<Curve>();
                        List<Curve> mainRebarShortCurves = new List<Curve>();

                        Curve line1 = Line.CreateBound(rebar_p1, rebar_p2) as Curve;
                        mainRebarLongCurves.Add(line1);
                        mainRebarShortCurves.Add(line1);
                        Curve line2 = Line.CreateBound(rebar_p2, rebar_p3) as Curve;
                        mainRebarLongCurves.Add(line2);
                        mainRebarShortCurves.Add(line2);
                        Curve line3L = Line.CreateBound(rebar_p3, rebar_p4L) as Curve;
                        mainRebarLongCurves.Add(line3L);
                        Curve line3S = Line.CreateBound(rebar_p3, rebar_p4S) as Curve;
                        mainRebarShortCurves.Add(line3S);

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
                        XYZ rotate2_p1 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z);
                        XYZ rotate2_p2 = new XYZ(rebar_p1.X, rebar_p1.Y, rebar_p1.Z + 1);
                        Line rotateLine2 = Line.CreateBound(rotate2_p1, rotate2_p2);
                        ElementTransformUtils.RotateElement(doc, mainRebarRightTop.Id, rotateLine2, 180 * (Math.PI / 180));
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
                        ElementTransformUtils.RotateElement(doc, mainRebarRightBottom.Id, rotateLine2, 180 * (Math.PI / 180));
                        XYZ newPlaсeMainRebarRightBottom = new XYZ(columnProperties.ColumnSectionHeight / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            - squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2
                            , -columnProperties.ColumnSectionWidth / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, mainRebarRightBottom.Id, newPlaсeMainRebarRightBottom);
                        rebarIdCollection.Add(mainRebarRightBottom.Id);
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
                        //Точки для построения кривых стержня
                        XYZ rebar_p1L = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));
                        XYZ rebar_p1S = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8), 6));
                        XYZ rebar_p2L = new XYZ(Math.Round(rebar_p1L.X, 6)
                            , Math.Round(rebar_p1L.Y, 6)
                            , Math.Round(rebar_p1L.Z + columnProperties.ColumnLength
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8), 6));
                        XYZ rebar_p2S = new XYZ(Math.Round(rebar_p1S.X, 6)
                            , Math.Round(rebar_p1S.Y, 6)
                            , Math.Round(rebar_p1S.Z + columnProperties.ColumnLength
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
                        List<Curve> mainRebarShortCurves = new List<Curve>();

                        Curve line1L = Line.CreateBound(rebar_p1L, rebar_p2L) as Curve;
                        mainRebarLongCurves.Add(line1L);
                        Curve line1S = Line.CreateBound(rebar_p1S, rebar_p2S) as Curve;
                        mainRebarShortCurves.Add(line1S);

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

                        FamilyInstance tubWelding_4 = doc.Create.NewFamilyInstance(tubWelding_p1S
                            , tubWeldingSymbol
                            , columnProperties.СolumnBaseLevel
                            , StructuralType.NonStructural);
                        tubWelding_4.LookupParameter("Диаметр стержня").Set(squareColumnsReinforcementMainForm.MainRebarTapeDiam);
                        ElementTransformUtils.MoveElement(doc, tubWelding_4.Id, newPlaсeMainRebarRightBottom);
                        rebarIdCollection.Add(tubWelding_4.Id);
                    }

                    //Если стыковка стержней на сварке с загибом в плиту
                    else if (squareColumnsReinforcementMainForm.CheckedRebarOutletsTepeName == "radioButton_Welding"
                        & squareColumnsReinforcementMainForm.СhangeSection == false
                        & squareColumnsReinforcementMainForm.TransitionToOverlap == false
                        & squareColumnsReinforcementMainForm.BendIntoSlab == true)
                    {
                        //Точки для построения кривых стержня
                        XYZ rebar_p1L = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));
                        XYZ rebar_p1S = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8), 6));

                        XYZ rebar_p2L = new XYZ(Math.Round(rebar_p1L.X, 6)
                            , Math.Round(rebar_p1L.Y, 6)
                            , Math.Round(rebar_p1L.Z - (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8)
                            + columnProperties.ColumnLength
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)
                            - (squareColumnsReinforcementMainForm.BendIntoSlabOffset / 304.8)
                            - (squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2), 6));
                        XYZ rebar_p2S = new XYZ(Math.Round(rebar_p1S.X, 6)
                            , Math.Round(rebar_p1S.Y, 6)
                            , Math.Round(rebar_p1S.Z - (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8)
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
                        XYZ rebar_p3S = new XYZ(Math.Round(rebar_p2S.X
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8)
                            - ((squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)
                            - (squareColumnsReinforcementMainForm.BendIntoSlabOffset / 304.8)
                            + (squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2)), 6)
                            , Math.Round(rebar_p2S.Y, 6)
                            , Math.Round(rebar_p2S.Z, 6));

                        //Кривые стержня
                        List<Curve> mainRebarLongCurves = new List<Curve>();
                        List<Curve> mainRebarShortCurves = new List<Curve>();

                        Curve line1L = Line.CreateBound(rebar_p1L, rebar_p2L) as Curve;
                        mainRebarLongCurves.Add(line1L);
                        Curve line1S = Line.CreateBound(rebar_p1S, rebar_p2S) as Curve;
                        mainRebarShortCurves.Add(line1S);
                        Curve line2L = Line.CreateBound(rebar_p2L, rebar_p3L) as Curve;
                        mainRebarLongCurves.Add(line2L);
                        Curve line2S = Line.CreateBound(rebar_p2S, rebar_p3S) as Curve;
                        mainRebarShortCurves.Add(line2S);


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
                    }

                    //Переход со сварки на нахлест без изменения сечения колонны
                    else if (squareColumnsReinforcementMainForm.CheckedRebarOutletsTepeName == "radioButton_Welding"
                        & squareColumnsReinforcementMainForm.СhangeSection == false
                        & squareColumnsReinforcementMainForm.TransitionToOverlap == true)
                    {
                        //Точки для построения кривых стержня
                        XYZ rebar_p1L = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));
                        XYZ rebar_p1S = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8), 6));

                        XYZ rebar_p2L = new XYZ(Math.Round(rebar_p1L.X, 6)
                            , Math.Round(rebar_p1L.Y, 6)
                            , Math.Round(rebar_p1L.Z
                            + columnProperties.ColumnLength
                            - (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));
                        XYZ rebar_p2S = new XYZ(Math.Round(rebar_p1S.X, 6)
                            , Math.Round(rebar_p1S.Y, 6)
                            , Math.Round(rebar_p1S.Z
                            + columnProperties.ColumnLength
                            - (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8), 6));

                        XYZ rebar_p3L = new XYZ(Math.Round(rebar_p2L.X
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam, 6)
                            , Math.Round(rebar_p2L.Y, 6)
                            , Math.Round(rebar_p2L.Z
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8), 6));
                        XYZ rebar_p3S = new XYZ(Math.Round(rebar_p2S.X
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam, 6)
                            , Math.Round(rebar_p2S.Y, 6)
                            , Math.Round(rebar_p2S.Z
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8), 6));

                        XYZ rebar_p4L = new XYZ(Math.Round(rebar_p3L.X, 6)
                            , Math.Round(rebar_p3L.Y, 6)
                            , Math.Round(rebar_p3L.Z
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));
                        XYZ rebar_p4S = new XYZ(Math.Round(rebar_p3S.X, 6)
                            , Math.Round(rebar_p3S.Y, 6)
                            , Math.Round(rebar_p3S.Z
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8), 6));

                        //Кривые стержня
                        List<Curve> mainRebarLongCurves = new List<Curve>();
                        List<Curve> mainRebarShortCurves = new List<Curve>();

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

                        //Точки для построения кривых стержня
                        XYZ rebar_p1L = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));
                        XYZ rebar_p1S = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8), 6));

                        XYZ rebar_p2L = new XYZ(Math.Round(rebar_p1L.X, 6)
                            , Math.Round(rebar_p1L.Y, 6)
                            , Math.Round(rebar_p1L.Z
                            + columnProperties.ColumnLength
                            - ((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8) * 6
                            - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8))
                            - (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));
                        XYZ rebar_p2S = new XYZ(Math.Round(rebar_p1S.X, 6)
                            , Math.Round(rebar_p1S.Y, 6)
                            , Math.Round(rebar_p1S.Z
                            + columnProperties.ColumnLength
                            - ((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8) * 6
                            - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8))
                            - (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8), 6));

                        XYZ rebar_p3L = new XYZ(Math.Round(rebar_p2L.X + deltaXWelding, 6)
                            , Math.Round(rebar_p2L.Y, 6)
                            , Math.Round(rebar_p2L.Z
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)
                            + ((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8) * 6
                            - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)), 6));
                        XYZ rebar_p3S = new XYZ(Math.Round(rebar_p2S.X + deltaXWelding, 6)
                            , Math.Round(rebar_p2S.Y, 6)
                            , Math.Round(rebar_p2S.Z
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)
                            + ((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8) * 6
                            - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)), 6));

                        XYZ rebar_p4L = new XYZ(Math.Round(rebar_p3L.X, 6)
                            , Math.Round(rebar_p3L.Y, 6)
                            , Math.Round(rebar_p3L.Z
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));
                        XYZ rebar_p4S = new XYZ(Math.Round(rebar_p3S.X, 6)
                            , Math.Round(rebar_p3S.Y, 6)
                            , Math.Round(rebar_p3S.Z
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
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2,
                            -columnProperties.ColumnSectionWidth / 2
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
                            , RebarHookOrientation.Right, RebarHookOrientation.Right);
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
                        XYZ newPlaсeTubWeldingMainRebarRightBottom = new XYZ(columnProperties.ColumnSectionHeight / 2
                            - squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            - squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2
                            - (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8),
                            -columnProperties.ColumnSectionWidth / 2
                            + squareColumnsReinforcementMainForm.MainRebarCoverLayer
                            + squareColumnsReinforcementMainForm.MainRebarTapeDiam / 2
                            + (squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8), 0);
                        ElementTransformUtils.MoveElement(doc, tubWeldingMainRebarRightBottom.Id, newPlaсeTubWeldingMainRebarRightBottom);
                        rebarIdCollection.Add(tubWeldingMainRebarRightBottom.Id);
                    }

                    //Переход со сварки на нахлест с изменением сечения колонны
                    else if (squareColumnsReinforcementMainForm.CheckedRebarOutletsTepeName == "radioButton_Welding"
                        & squareColumnsReinforcementMainForm.СhangeSection == true
                        & squareColumnsReinforcementMainForm.TransitionToOverlap == true
                        & squareColumnsReinforcementMainForm.СhangeSectionOffset <= 50)
                    {
                        //Точки для построения кривых стержня
                        XYZ rebar_p1L = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));
                        XYZ rebar_p1S = new XYZ(Math.Round(columnProperties.ColumnOrigin.X, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Y, 6)
                            , Math.Round(columnProperties.ColumnOrigin.Z
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8), 6));

                        XYZ rebar_p2L = new XYZ(Math.Round(rebar_p1L.X, 6)
                            , Math.Round(rebar_p1L.Y, 6)
                            , Math.Round(rebar_p1L.Z
                            + columnProperties.ColumnLength
                            - ((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8) * 6
                            - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8))
                            - (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));
                        XYZ rebar_p2S = new XYZ(Math.Round(rebar_p1S.X, 6)
                            , Math.Round(rebar_p1S.Y, 6)
                            , Math.Round(rebar_p1S.Z
                            + columnProperties.ColumnLength
                            - ((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8) * 6
                            - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8))
                            - (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8), 6));

                        XYZ rebar_p3L = new XYZ(Math.Round(rebar_p2L.X + deltaXMainRebarOverlapping, 6)
                            , Math.Round(rebar_p2L.Y, 6)
                            , Math.Round(rebar_p2L.Z
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)
                            + ((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8) * 6
                            - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)), 6));
                        XYZ rebar_p3S = new XYZ(Math.Round(rebar_p2S.X + deltaXMainRebarOverlapping, 6)
                            , Math.Round(rebar_p2S.Y, 6)
                            , Math.Round(rebar_p2S.Z
                            + (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)
                            + ((squareColumnsReinforcementMainForm.СhangeSectionOffset / 304.8) * 6
                            - (squareColumnsReinforcementMainForm.FloorThicknessAboveColumn / 304.8)), 6));

                        XYZ rebar_p4L = new XYZ(Math.Round(rebar_p3L.X, 6)
                            , Math.Round(rebar_p3L.Y, 6)
                            , Math.Round(rebar_p3L.Z
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsLongLength / 304.8), 6));
                        XYZ rebar_p4S = new XYZ(Math.Round(rebar_p3S.X, 6)
                            , Math.Round(rebar_p3S.Y, 6)
                            , Math.Round(rebar_p3S.Z
                            + (squareColumnsReinforcementMainForm.MainRebarOutletsShortLength / 304.8), 6));

                        //Кривые стержня
                        List<Curve> mainRebarLongCurves = new List<Curve>();
                        List<Curve> mainRebarShortCurves = new List<Curve>();

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
                    }
                    
                    //Хомуты
                    SquareColumnsReinforcementCommand.CreateEncircleStirrup(doc
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
