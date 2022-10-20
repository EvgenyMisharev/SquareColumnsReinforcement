using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SquareColumnsReinforcement
{
    public class SquareColumnsReinforcementT4 : IExternalCommand
    {
        public Result Execute(UIApplication uiapp
            , Document doc
            , List<FamilyInstance> columnsList
            , SquareColumnsReinforcementWPF squareColumnsReinforcementWPF)
        {

#if R2019 || R2020 || R2021 || R2022
            RebarBarType firstMainBarTape = squareColumnsReinforcementWPF.FirstMainBarTape;
            double firstMainBarDiam = firstMainBarTape.BarDiameter;
            RebarBarType secondMainBarTape = squareColumnsReinforcementWPF.SecondMainBarTape;
            double secondMainBarDiam = secondMainBarTape.BarDiameter;
            RebarBarType firstStirrupBarTape = squareColumnsReinforcementWPF.FirstStirrupBarTape;
            double firstStirrupBarDiam = firstStirrupBarTape.BarDiameter;
            RebarBarType secondStirrupBarTape = squareColumnsReinforcementWPF.SecondStirrupBarTape;
            double secondStirrupBarDiam = secondStirrupBarTape.BarDiameter;
            RebarHookType rebarHookTypeForStirrup = squareColumnsReinforcementWPF.RebarHookTypeForStirrup;
#else
            RebarBarType firstMainBarTape = squareColumnsReinforcementWPF.FirstMainBarTape;
            double firstMainBarDiam = firstMainBarTape.BarNominalDiameter;
            RebarBarType secondMainBarTape = squareColumnsReinforcementWPF.SecondMainBarTape;
            double secondMainBarDiam = secondMainBarTape.BarNominalDiameter;
            RebarBarType firstStirrupBarTape = squareColumnsReinforcementWPF.FirstStirrupBarTape;
            double firstStirrupBarDiam = firstStirrupBarTape.BarNominalDiameter;
            RebarBarType secondStirrupBarTape = squareColumnsReinforcementWPF.SecondStirrupBarTape;
            double secondStirrupBarDiam = secondStirrupBarTape.BarNominalDiameter;
            RebarHookType rebarHookTypeForStirrup = squareColumnsReinforcementWPF.RebarHookTypeForStirrup;
#endif
            RebarShape form01 = squareColumnsReinforcementWPF.Form01;
            RebarShape form26 = squareColumnsReinforcementWPF.Form26;
            RebarShape form11 = squareColumnsReinforcementWPF.Form11;
            RebarShape form51 = squareColumnsReinforcementWPF.Form51;

            double firstTopRebarOffset = squareColumnsReinforcementWPF.FirstTopRebarOffset / 304.8;
            double firstLowerRebarOffset = squareColumnsReinforcementWPF.FirstLowerRebarOffset / 304.8;
            double firstLeftRebarOffset = squareColumnsReinforcementWPF.FirstLeftRebarOffset / 304.8;
            double firstRightRebarOffset = squareColumnsReinforcementWPF.FirstRightRebarOffset / 304.8;

            double firstRebarOutletsLength = squareColumnsReinforcementWPF.FirstRebarOutletsLength / 304.8;
            double secondRebarOutletsLength = squareColumnsReinforcementWPF.SecondRebarOutletsLength / 304.8;
            double firstStirrupButtomOffset = squareColumnsReinforcementWPF.FirstStirrupButtomOffset / 304.8;

            double frequentButtomStirrupPlacementHeight = squareColumnsReinforcementWPF.FrequentButtomStirrupPlacementHeight / 304.8;
            double frequentButtomStirrupStep = squareColumnsReinforcementWPF.FrequentButtomStirrupStep / 304.8;
            double standardStirrupStep = squareColumnsReinforcementWPF.StandardStirrupStep / 304.8;

            double frequentTopStirrupPlacementHeight = squareColumnsReinforcementWPF.FrequentTopStirrupPlacementHeight / 304.8;
            double frequentTopStirrupStep = squareColumnsReinforcementWPF.FrequentTopStirrupStep / 304.8;

            double deepenRebar = squareColumnsReinforcementWPF.DeepenRebar / 304.8;
            double floorThickness = squareColumnsReinforcementWPF.FloorThickness / 304.8;
            double bendInSlab = squareColumnsReinforcementWPF.BendInSlab / 304.8;

            RebarCoverType columnRebarCoverType = squareColumnsReinforcementWPF.ColumnRebarCoverType;
            double coverDistance = columnRebarCoverType.CoverDistance;

            double sectionOffset = squareColumnsReinforcementWPF.SectionChange / 304.8;
            double deltaXOverlapping = Math.Sqrt(Math.Pow((sectionOffset + firstMainBarDiam), 2) + Math.Pow(sectionOffset, 2));
            double alphaOverlapping = Math.Asin(sectionOffset / deltaXOverlapping);

            double deltaXSecondOverlapping = Math.Sqrt(Math.Pow(sectionOffset, 2) + Math.Pow(secondMainBarDiam, 2));
            double alphaSecondOverlapping = Math.Asin(secondMainBarDiam / deltaXSecondOverlapping);

            double deltaXWelding = Math.Sqrt(Math.Pow(sectionOffset, 2) + Math.Pow(sectionOffset, 2));
            double alphaWelding = Math.Asin(sectionOffset / deltaXWelding);

            Guid diamGuid = new Guid("9b679ab7-ea2e-49ce-90ab-0549d5aa36ff");

            FamilySymbol firstMechanicalConnectionFamilySymbol = null;
            FamilySymbol secondMechanicalConnectionFamilySymbol = null;
            if (squareColumnsReinforcementWPF.RebarConnectionOptionName == "radioButton_Mechanical")
            {
                if (squareColumnsReinforcementWPF.MechanicalConnectionOptionName == "radioButton_WeldedConnection")
                {
                    List<ElementId> weldedConnectionElementIds = squareColumnsReinforcementWPF.WeldedConnectionFamily.GetFamilySymbolIds().ToList();
                    List<FamilySymbol> weldedConnectionFamilySymbolList = new List<FamilySymbol>();
                    foreach (ElementId id in weldedConnectionElementIds)
                    {
                        weldedConnectionFamilySymbolList.Add(doc.GetElement(id) as FamilySymbol);
                    }
                    weldedConnectionFamilySymbolList = weldedConnectionFamilySymbolList.OrderBy(fs => fs.get_Parameter(diamGuid).AsDouble()).ToList();
                    foreach (FamilySymbol fs in weldedConnectionFamilySymbolList)
                    {
                        if (Math.Round(fs.get_Parameter(diamGuid).AsDouble(), 6) == Math.Round(firstMainBarDiam, 6))
                        {
                            firstMechanicalConnectionFamilySymbol = fs;
                            break;
                        }
                    }
                    foreach (FamilySymbol fs in weldedConnectionFamilySymbolList)
                    {
                        if (Math.Round(fs.get_Parameter(diamGuid).AsDouble(), 6) == Math.Round(secondMainBarDiam, 6))
                        {
                            secondMechanicalConnectionFamilySymbol = fs;
                            break;
                        }
                    }
                }
                else
                {
                    List<ElementId> couplingConnectionElementIds = squareColumnsReinforcementWPF.CouplingConnectionFamily.GetFamilySymbolIds().ToList();
                    List<FamilySymbol> couplingConnectionFamilySymbolList = new List<FamilySymbol>();
                    foreach (ElementId id in couplingConnectionElementIds)
                    {
                        couplingConnectionFamilySymbolList.Add(doc.GetElement(id) as FamilySymbol);
                    }
                    couplingConnectionFamilySymbolList = couplingConnectionFamilySymbolList.OrderBy(fs => fs.get_Parameter(diamGuid).AsDouble()).ToList();
                    foreach (FamilySymbol fs in couplingConnectionFamilySymbolList)
                    {
                        if (Math.Round(fs.get_Parameter(diamGuid).AsDouble(), 6) == Math.Round(firstMainBarDiam, 6))
                        {
                            firstMechanicalConnectionFamilySymbol = fs;
                            break;
                        }
                    }
                    foreach (FamilySymbol fs in couplingConnectionFamilySymbolList)
                    {
                        if (Math.Round(fs.get_Parameter(diamGuid).AsDouble(), 6) == Math.Round(secondMainBarDiam, 6))
                        {
                            secondMechanicalConnectionFamilySymbol = fs;
                            break;
                        }
                    }
                }
            }

            using (Transaction t = new Transaction(doc))
            {
                t.Start("Армирование колонн - Тип 4");
                if (squareColumnsReinforcementWPF.RebarConnectionOptionName == "radioButton_Mechanical")
                {
                    if (firstMechanicalConnectionFamilySymbol == null)
                    {
                        if (squareColumnsReinforcementWPF.MechanicalConnectionOptionName == "radioButton_WeldedConnection")
                        {
                            TaskDialog.Show("Revit", $"Для угловых стержней не найден подходящий тип соединения {squareColumnsReinforcementWPF.WeldedConnectionFamily.Name}! Уваличте диаметр стержней или измените тип соединения!");
                            return Result.Cancelled;
                        }
                        else
                        {
                            TaskDialog.Show("Revit", $"Для угловых стержней не найден подходящий тип соединения {squareColumnsReinforcementWPF.CouplingConnectionFamily.Name}! Уваличте диаметр стержней или измените тип соединения!");
                            return Result.Cancelled;
                        }
                    }
                    else if (secondMechanicalConnectionFamilySymbol == null)
                    {
                        if (squareColumnsReinforcementWPF.MechanicalConnectionOptionName == "radioButton_WeldedConnection")
                        {
                            TaskDialog.Show("Revit", $"Для боковых стержней не найден подходящий тип соединения {squareColumnsReinforcementWPF.WeldedConnectionFamily.Name}! Уваличте диаметр стержней или измените тип соединения!");
                            return Result.Cancelled;
                        }
                        else
                        {
                            TaskDialog.Show("Revit", $"Для боковых стержней не найден подходящий тип соединения {squareColumnsReinforcementWPF.CouplingConnectionFamily.Name}! Уваличте диаметр стержней или измените тип соединения!");
                            return Result.Cancelled;
                        }
                    }
                    firstMechanicalConnectionFamilySymbol.Activate();
                    secondMechanicalConnectionFamilySymbol.Activate();
                }

                foreach (FamilyInstance column in columnsList)
                {
                    ColumnPropertyCollector columnProperty = new ColumnPropertyCollector(doc, column);
                    column.get_Parameter(BuiltInParameter.CLEAR_COVER_OTHER).Set(columnRebarCoverType.Id);

                    XYZ rotateBase_p1 = columnProperty.ColumnBasePoint;
                    XYZ rotateBase_p2 = new XYZ(rotateBase_p1.X, rotateBase_p1.Y, rotateBase_p1.Z + 1);
                    Line rotateLineBase = Line.CreateBound(rotateBase_p1, rotateBase_p2);

                    //Если стыковка стержней в нахлест без изменения сечения колонны выше
                    if (squareColumnsReinforcementWPF.RebarConnectionOptionName == "radioButton_Overlap"
                        && squareColumnsReinforcementWPF.SectionChangeChecked == false
                        && squareColumnsReinforcementWPF.BendInSlabChecked == false)
                    {
                        //Точки для построения кривых основных угловых стержней удлиненных
                        XYZ rebar_p1L = null;
                        XYZ rebar_p2L = null;
                        if (squareColumnsReinforcementWPF.DeepenRebarChecked)
                        {
                            rebar_p1L = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), Math.Round(columnProperty.ColumnBasePoint.Z - deepenRebar, 6));
                            rebar_p2L = new XYZ(Math.Round(rebar_p1L.X, 6), Math.Round(rebar_p1L.Y, 6), Math.Round(rebar_p1L.Z + deepenRebar + columnProperty.ColumnLength, 6));
                        }
                        else
                        {
                            rebar_p1L = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), Math.Round(columnProperty.ColumnBasePoint.Z, 6));
                            rebar_p2L = new XYZ(Math.Round(rebar_p1L.X, 6), Math.Round(rebar_p1L.Y, 6), Math.Round(rebar_p1L.Z + columnProperty.ColumnLength, 6));
                        }
                        XYZ rebar_p3L = new XYZ(Math.Round(rebar_p2L.X + firstMainBarDiam, 6), Math.Round(rebar_p2L.Y, 6), Math.Round(rebar_p2L.Z + floorThickness, 6));
                        XYZ rebar_p4L = new XYZ(Math.Round(rebar_p3L.X, 6), Math.Round(rebar_p3L.Y, 6), Math.Round(rebar_p3L.Z + firstRebarOutletsLength, 6));

                        //Точки для построения кривых основных угловых стержней укороченных
                        XYZ rebar_p1S = null;
                        XYZ rebar_p2S = null;
                        if (squareColumnsReinforcementWPF.DeepenRebarChecked)
                        {
                            rebar_p1S = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), Math.Round(columnProperty.ColumnBasePoint.Z - deepenRebar, 6));
                            rebar_p2S = new XYZ(Math.Round(rebar_p1S.X, 6), Math.Round(rebar_p1S.Y, 6), Math.Round(rebar_p1S.Z + deepenRebar + columnProperty.ColumnLength, 6));
                        }
                        else
                        {
                            rebar_p1S = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), Math.Round(columnProperty.ColumnBasePoint.Z, 6));
                            rebar_p2S = new XYZ(Math.Round(rebar_p1S.X, 6), Math.Round(rebar_p1S.Y, 6), Math.Round(rebar_p1S.Z + columnProperty.ColumnLength, 6));
                        }
                        XYZ rebar_p3S = new XYZ(Math.Round(rebar_p2S.X + firstMainBarDiam, 6), Math.Round(rebar_p2S.Y, 6), Math.Round(rebar_p2S.Z + floorThickness, 6));
                        XYZ rebar_p4S = new XYZ(Math.Round(rebar_p3S.X, 6), Math.Round(rebar_p3S.Y, 6), Math.Round(rebar_p3S.Z + secondRebarOutletsLength, 6));

                        //Точки для построения кривых основных боковых стержней удлиненных
                        XYZ rebar_p1AL = null;
                        XYZ rebar_p2AL = null;
                        if (squareColumnsReinforcementWPF.DeepenRebarChecked)
                        {
                            rebar_p1AL = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), Math.Round(columnProperty.ColumnBasePoint.Z - deepenRebar, 6));
                            rebar_p2AL = new XYZ(Math.Round(rebar_p1AL.X, 6), Math.Round(rebar_p1AL.Y, 6), Math.Round(rebar_p1AL.Z + deepenRebar + columnProperty.ColumnLength, 6));
                        }
                        else
                        {
                            rebar_p1AL = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), Math.Round(columnProperty.ColumnBasePoint.Z, 6));
                            rebar_p2AL = new XYZ(Math.Round(rebar_p1AL.X, 6), Math.Round(rebar_p1AL.Y, 6), Math.Round(rebar_p1AL.Z + columnProperty.ColumnLength, 6));
                        }
                        XYZ rebar_p3AL = new XYZ(Math.Round(rebar_p2AL.X + secondMainBarDiam, 6), Math.Round(rebar_p2AL.Y, 6), Math.Round(rebar_p2AL.Z + floorThickness, 6));
                        XYZ rebar_p4AL = new XYZ(Math.Round(rebar_p3AL.X, 6), Math.Round(rebar_p3AL.Y, 6), Math.Round(rebar_p3AL.Z + firstRebarOutletsLength, 6));

                        //Точки для построения кривых основных боковых стержней укороченных
                        XYZ rebar_p1AS = null;
                        XYZ rebar_p2AS = null;
                        if (squareColumnsReinforcementWPF.DeepenRebarChecked)
                        {
                            rebar_p1AS = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), Math.Round(columnProperty.ColumnBasePoint.Z - deepenRebar, 6));
                            rebar_p2AS = new XYZ(Math.Round(rebar_p1AS.X, 6), Math.Round(rebar_p1AS.Y, 6), Math.Round(rebar_p1AS.Z + deepenRebar + columnProperty.ColumnLength, 6));
                        }
                        else
                        {
                            rebar_p1AS = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), Math.Round(columnProperty.ColumnBasePoint.Z, 6));
                            rebar_p2AS = new XYZ(Math.Round(rebar_p1AS.X, 6), Math.Round(rebar_p1AS.Y, 6), Math.Round(rebar_p1AS.Z + columnProperty.ColumnLength, 6));
                        }
                        XYZ rebar_p3AS = new XYZ(Math.Round(rebar_p2AS.X + secondMainBarDiam, 6), Math.Round(rebar_p2AS.Y, 6), Math.Round(rebar_p2AS.Z + floorThickness, 6));
                        XYZ rebar_p4AS = new XYZ(Math.Round(rebar_p3AS.X, 6), Math.Round(rebar_p3AS.Y, 6), Math.Round(rebar_p3AS.Z + secondRebarOutletsLength, 6));

                        //Кривые основных угловых стержней удлиненных
                        List<Curve> mainFirstRebarCurvesL = new List<Curve>();

                        Curve line1L = Line.CreateBound(rebar_p1L, rebar_p2L) as Curve;
                        mainFirstRebarCurvesL.Add(line1L);
                        Curve line2L = Line.CreateBound(rebar_p2L, rebar_p3L) as Curve;
                        mainFirstRebarCurvesL.Add(line2L);
                        Curve line3L = Line.CreateBound(rebar_p3L, rebar_p4L) as Curve;
                        mainFirstRebarCurvesL.Add(line3L);

                        //Кривые основных угловых стержней укороченных
                        List<Curve> mainFirstRebarCurvesS = new List<Curve>();

                        Curve line1S = Line.CreateBound(rebar_p1S, rebar_p2S) as Curve;
                        mainFirstRebarCurvesS.Add(line1S);
                        Curve line2S = Line.CreateBound(rebar_p2S, rebar_p3S) as Curve;
                        mainFirstRebarCurvesS.Add(line2L);
                        Curve line3S = Line.CreateBound(rebar_p3S, rebar_p4S) as Curve;
                        mainFirstRebarCurvesS.Add(line3S);

                        //Кривые основных боковых стержней c двойным нахлестом
                        List<Curve> mainSecondRebarCurvesL = new List<Curve>();

                        Curve line1AL = Line.CreateBound(rebar_p1AL, rebar_p2AL) as Curve;
                        mainSecondRebarCurvesL.Add(line1AL);
                        Curve line2AL = Line.CreateBound(rebar_p2AL, rebar_p3AL) as Curve;
                        mainSecondRebarCurvesL.Add(line2AL);
                        Curve line3AL = Line.CreateBound(rebar_p3AL, rebar_p4AL) as Curve;
                        mainSecondRebarCurvesL.Add(line3AL);

                        //Кривые основных боковых стержней c одинарным нахлестом
                        List<Curve> mainSecondRebarCurvesS = new List<Curve>();

                        Curve line1AS = Line.CreateBound(rebar_p1AS, rebar_p2AS) as Curve;
                        mainSecondRebarCurvesS.Add(line1AS);
                        Curve line2AS = Line.CreateBound(rebar_p2AS, rebar_p3AS) as Curve;
                        mainSecondRebarCurvesS.Add(line2AS);
                        Curve line3AS = Line.CreateBound(rebar_p3AS, rebar_p4AS) as Curve;
                        mainSecondRebarCurvesS.Add(line3AS);

                        //Нижний левый угол
                        Rebar columnMainRebar_1 = null;
                        try
                        {
                            columnMainRebar_1 = Rebar.CreateFromCurvesAndShape(doc
                                , form26
                                , firstMainBarTape
                                , null
                                , null
                                , column
                                , new XYZ(0, 1, 0)
                                , mainFirstRebarCurvesL
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);
                        }
                        catch
                        {
                            TaskDialog.Show("Revit", "Не удалось создать Z-образный стержень! Возможно выбран некорректный тип формы 26!");
                            return Result.Cancelled;
                        }

                        if (columnMainRebar_1 == null)
                        {
                            TaskDialog.Show("Revit", "Не удалось создать Z-образный стержень! Возможно выбран некорректный тип формы 26!");
                            return Result.Cancelled;
                        }

                        XYZ newPlaсeСolumnMainRebar_1 = new XYZ(-columnProperty.ColumnSectionHeight / 2
                            + coverDistance
                            + firstMainBarDiam / 2
                            , -columnProperty.ColumnSectionWidth / 2
                            + coverDistance
                            + firstMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1.Id, newPlaсeСolumnMainRebar_1);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Верхний левый угол
                        Rebar columnMainRebar_2 = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , firstMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainFirstRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnRebar_2 = new XYZ(-columnProperty.ColumnSectionHeight / 2
                            + coverDistance
                            + firstMainBarDiam / 2
                            , columnProperty.ColumnSectionWidth / 2
                            - coverDistance
                            - firstMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2.Id, newPlaсeСolumnRebar_2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Верхний правый угол
                        Rebar columnMainRebar_3 = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , firstMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainFirstRebarCurvesL
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ rotate1_p1 = new XYZ(rebar_p1L.X, rebar_p1L.Y, rebar_p1L.Z);
                        XYZ rotate1_p2 = new XYZ(rebar_p1L.X, rebar_p1L.Y, rebar_p1L.Z + 1);
                        Line rotateLine = Line.CreateBound(rotate1_p1, rotate1_p2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_3 = new XYZ(columnProperty.ColumnSectionHeight / 2
                            - coverDistance
                            - firstMainBarDiam / 2
                            , columnProperty.ColumnSectionWidth / 2
                            - coverDistance
                            - firstMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3.Id, newPlaсeСolumnRebar_3);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Нижний правый угол
                        Rebar columnMainRebar_4 = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , firstMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainFirstRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_4 = new XYZ(columnProperty.ColumnSectionHeight / 2
                            - coverDistance
                            - firstMainBarDiam / 2
                            , -columnProperty.ColumnSectionWidth / 2
                            + coverDistance
                            + firstMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4.Id, newPlaсeСolumnRebar_4);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Центральный левый нижний стержень
                        Rebar columnMainRebar_1A = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1A.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeСolumnMainRebar_1A = new XYZ(-columnProperty.ColumnSectionHeight / 2 + coverDistance + secondMainBarDiam / 2, -firstLowerRebarOffset, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1A.Id, newPlaсeСolumnMainRebar_1A);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1A.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Центральный левый верхний стержень
                        Rebar columnMainRebar_1B = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesL
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1B.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeСolumnMainRebar_1B = new XYZ(-columnProperty.ColumnSectionHeight / 2 + coverDistance + secondMainBarDiam / 2, firstTopRebarOffset, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1B.Id, newPlaсeСolumnMainRebar_1B);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1B.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);


                        //Центральный правый нижний стержень
                        Rebar columnMainRebar_2A = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesL
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2A.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeСolumnMainRebar_2A = new XYZ(+columnProperty.ColumnSectionHeight / 2 - coverDistance - secondMainBarDiam / 2, -firstLowerRebarOffset, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2A.Id, newPlaсeСolumnMainRebar_2A);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2A.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Центральный правый верхний стержень
                        Rebar columnMainRebar_2B = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2B.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeСolumnMainRebar_2B = new XYZ(+columnProperty.ColumnSectionHeight / 2 - coverDistance - secondMainBarDiam / 2, firstTopRebarOffset, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2B.Id, newPlaсeСolumnMainRebar_2B);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2B.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);


                        //Центральный нижний левый стержень
                        Rebar columnMainRebar_3A = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebar_3A = new XYZ(-firstLeftRebarOffset, -columnProperty.ColumnSectionWidth / 2 + coverDistance + secondMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3A.Id, newPlaсeСolumnMainRebar_3A);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3A.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Центральный нижний правый стержень
                        Rebar columnMainRebar_3B = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesL
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebar_3B = new XYZ(firstRightRebarOffset, -columnProperty.ColumnSectionWidth / 2 + coverDistance + secondMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3B.Id, newPlaсeСolumnMainRebar_3B);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3B.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);


                        //Центральный верхний левый стержень
                        Rebar columnMainRebar_4A = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesL
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebar_4A = new XYZ(-firstLeftRebarOffset, columnProperty.ColumnSectionWidth / 2 - coverDistance - secondMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4A.Id, newPlaсeСolumnMainRebar_4A);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4A.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Центральный верхний правый стержень
                        Rebar columnMainRebar_4B = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebar_4B = new XYZ(firstRightRebarOffset, columnProperty.ColumnSectionWidth / 2 - coverDistance - secondMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4B.Id, newPlaсeСolumnMainRebar_4B);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4B.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);
                    }

                    //Если стыковка стержней в нахлест загиб в плиту
                    else if (squareColumnsReinforcementWPF.RebarConnectionOptionName == "radioButton_Overlap"
                        && squareColumnsReinforcementWPF.SectionChangeChecked == false
                        && squareColumnsReinforcementWPF.BendInSlabChecked == true)
                    {
                        //Точки для построения кривых основных угловых стержней
                        XYZ rebar_p1L = null;
                        XYZ rebar_p2L = null;
                        if (squareColumnsReinforcementWPF.DeepenRebarChecked)
                        {
                            rebar_p1L = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), Math.Round(columnProperty.ColumnBasePoint.Z - deepenRebar, 6));
                            rebar_p2L = new XYZ(Math.Round(rebar_p1L.X, 6), Math.Round(rebar_p1L.Y, 6), Math.Round(rebar_p1L.Z + deepenRebar + columnProperty.ColumnLength + floorThickness - bendInSlab - firstMainBarDiam / 2, 6));
                        }
                        else
                        {
                            rebar_p1L = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), Math.Round(columnProperty.ColumnBasePoint.Z, 6));
                            rebar_p2L = new XYZ(Math.Round(rebar_p1L.X, 6), Math.Round(rebar_p1L.Y, 6), Math.Round(rebar_p1L.Z + columnProperty.ColumnLength + floorThickness - bendInSlab - firstMainBarDiam / 2, 6));
                        }
                        XYZ rebar_p3L = new XYZ(Math.Round(rebar_p2L.X + firstRebarOutletsLength - (floorThickness - bendInSlab - firstMainBarDiam / 2), 6), Math.Round(rebar_p2L.Y, 6), Math.Round(rebar_p2L.Z, 6));

                        //Точки для построения кривых основных боковых стержней
                        XYZ rebar_p1S = null;
                        XYZ rebar_p2S = null;
                        if (squareColumnsReinforcementWPF.DeepenRebarChecked)
                        {
                            rebar_p1S = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), Math.Round(columnProperty.ColumnBasePoint.Z - deepenRebar, 6));
                            rebar_p2S = new XYZ(Math.Round(rebar_p1S.X, 6), Math.Round(rebar_p1S.Y, 6), Math.Round(rebar_p1S.Z + deepenRebar + columnProperty.ColumnLength + floorThickness - bendInSlab - secondMainBarDiam / 2, 6));
                        }
                        else
                        {
                            rebar_p1S = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), Math.Round(columnProperty.ColumnBasePoint.Z, 6));
                            rebar_p2S = new XYZ(Math.Round(rebar_p1S.X, 6), Math.Round(rebar_p1S.Y, 6), Math.Round(rebar_p1S.Z + columnProperty.ColumnLength + floorThickness - bendInSlab - secondMainBarDiam / 2, 6));
                        }
                        XYZ rebar_p3S = new XYZ(Math.Round(rebar_p2S.X + secondRebarOutletsLength - (floorThickness - bendInSlab - secondMainBarDiam / 2), 6), Math.Round(rebar_p2S.Y, 6), Math.Round(rebar_p2S.Z, 6));

                        //Кривые основных угловых стержней
                        List<Curve> mainFirstRebarCurvesL = new List<Curve>();

                        Curve line1L = Line.CreateBound(rebar_p1L, rebar_p2L) as Curve;
                        mainFirstRebarCurvesL.Add(line1L);
                        Curve line2L = Line.CreateBound(rebar_p2L, rebar_p3L) as Curve;
                        mainFirstRebarCurvesL.Add(line2L);

                        //Кривые основных боковых стержней
                        List<Curve> mainRebarCurvesS = new List<Curve>();

                        Curve line1S = Line.CreateBound(rebar_p1S, rebar_p2S) as Curve;
                        mainRebarCurvesS.Add(line1S);
                        Curve line2S = Line.CreateBound(rebar_p2S, rebar_p3S) as Curve;
                        mainRebarCurvesS.Add(line2S);

                        //Нижний левый угол
                        Rebar columnMainRebar_1 = null;
                        try
                        {
                            columnMainRebar_1 = Rebar.CreateFromCurvesAndShape(doc
                                , form11
                                , firstMainBarTape
                                , null
                                , null
                                , column
                                , new XYZ(0, 1, 0)
                                , mainFirstRebarCurvesL
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);
                        }
                        catch
                        {
                            TaskDialog.Show("Revit", "Не удалось создать Г-образный стержень! Возможно выбран некорректный тип формы 11!");
                            return Result.Cancelled;
                        }

                        if (columnMainRebar_1 == null)
                        {
                            TaskDialog.Show("Revit", "Не удалось создать Г-образный стержень! Возможно выбран некорректный тип формы 11!");
                            return Result.Cancelled;
                        }

                        XYZ rotate1_p1 = new XYZ(rebar_p1L.X, rebar_p1L.Y, rebar_p1L.Z);
                        XYZ rotate1_p2 = new XYZ(rebar_p1L.X, rebar_p1L.Y, rebar_p1L.Z + 1);
                        Line rotateLine = Line.CreateBound(rotate1_p1, rotate1_p2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1.Id, rotateLine, 180 * (Math.PI / 180));

                        XYZ newPlaсeСolumnMainRebar_1 = new XYZ(-columnProperty.ColumnSectionHeight / 2
                            + coverDistance
                            + firstMainBarDiam / 2
                            , -columnProperty.ColumnSectionWidth / 2
                            + coverDistance
                            + firstMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1.Id, newPlaсeСolumnMainRebar_1);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Верхний левый угол
                        Rebar columnMainRebar_2 = Rebar.CreateFromCurvesAndShape(doc
                            , form11
                            , firstMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainFirstRebarCurvesL
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_2 = new XYZ(-columnProperty.ColumnSectionHeight / 2
                            + coverDistance
                            + firstMainBarDiam / 2
                            , columnProperty.ColumnSectionWidth / 2
                            - coverDistance
                            - firstMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2.Id, newPlaсeСolumnRebar_2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Верхний правый угол
                        Rebar columnMainRebar_3 = Rebar.CreateFromCurvesAndShape(doc
                            , form11
                            , firstMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainFirstRebarCurvesL
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        XYZ newPlaсeСolumnRebar_3 = new XYZ(columnProperty.ColumnSectionHeight / 2
                            - coverDistance
                            - firstMainBarDiam / 2
                            , columnProperty.ColumnSectionWidth / 2
                            - coverDistance
                            - firstMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3.Id, newPlaсeСolumnRebar_3);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Нижний правый угол
                        Rebar columnMainRebar_4 = Rebar.CreateFromCurvesAndShape(doc
                            , form11
                            , firstMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainFirstRebarCurvesL
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        XYZ newPlaсeСolumnRebar_4 = new XYZ(columnProperty.ColumnSectionHeight / 2
                            - coverDistance
                            - firstMainBarDiam / 2
                            , -columnProperty.ColumnSectionWidth / 2
                            + coverDistance
                            + firstMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4.Id, newPlaсeСolumnRebar_4);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);


                        //Центральный левый нижний стержень
                        Rebar columnMainRebar_1A = Rebar.CreateFromCurvesAndShape(doc
                            , form11
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1A.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnMainRebar_1A = new XYZ(-columnProperty.ColumnSectionHeight / 2 + coverDistance + secondMainBarDiam / 2, -firstLowerRebarOffset, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1A.Id, newPlaсeСolumnMainRebar_1A);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1A.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Центральный левый верхний стержень
                        Rebar columnMainRebar_1B = Rebar.CreateFromCurvesAndShape(doc
                            , form11
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1B.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnMainRebar_1B = new XYZ(-columnProperty.ColumnSectionHeight / 2 + coverDistance + secondMainBarDiam / 2, firstTopRebarOffset, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1B.Id, newPlaсeСolumnMainRebar_1B);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1B.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);


                        //Центральный правый нижний стержень
                        Rebar columnMainRebar_2A = Rebar.CreateFromCurvesAndShape(doc
                            , form11
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        XYZ newPlaсeСolumnMainRebar_2A = new XYZ(+columnProperty.ColumnSectionHeight / 2 - coverDistance - secondMainBarDiam / 2, -firstLowerRebarOffset, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2A.Id, newPlaсeСolumnMainRebar_2A);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2A.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Центральный правый верхний стержень
                        Rebar columnMainRebar_2B = Rebar.CreateFromCurvesAndShape(doc
                            , form11
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        XYZ newPlaсeСolumnMainRebar_2B = new XYZ(+columnProperty.ColumnSectionHeight / 2 - coverDistance - secondMainBarDiam / 2, firstTopRebarOffset, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2B.Id, newPlaсeСolumnMainRebar_2B);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2B.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);


                        //Центральный нижний левый стержень
                        Rebar columnMainRebar_3A = Rebar.CreateFromCurvesAndShape(doc
                            , form11
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3A.Id, rotateLine, -90 * (Math.PI / 180));
                        XYZ newPlaсeСolumnMainRebar_3A = new XYZ(-firstLeftRebarOffset, -columnProperty.ColumnSectionWidth / 2 + coverDistance + secondMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3A.Id, newPlaсeСolumnMainRebar_3A);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3A.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Центральный нижний правый стержень
                        Rebar columnMainRebar_3B = Rebar.CreateFromCurvesAndShape(doc
                            , form11
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3B.Id, rotateLine, -90 * (Math.PI / 180));
                        XYZ newPlaсeСolumnMainRebar_3B = new XYZ(firstRightRebarOffset, -columnProperty.ColumnSectionWidth / 2 + coverDistance + secondMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3B.Id, newPlaсeСolumnMainRebar_3B);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3B.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);


                        //Центральный верхний левый стержень
                        Rebar columnMainRebar_4A = Rebar.CreateFromCurvesAndShape(doc
                            , form11
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4A.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeСolumnMainRebar_4A = new XYZ(-firstLeftRebarOffset, columnProperty.ColumnSectionWidth / 2 - coverDistance - secondMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4A.Id, newPlaсeСolumnMainRebar_4A);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4A.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Центральный верхний правый стержень
                        Rebar columnMainRebar_4B = Rebar.CreateFromCurvesAndShape(doc
                            , form11
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4B.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeСolumnMainRebar_4B = new XYZ(firstRightRebarOffset, columnProperty.ColumnSectionWidth / 2 - coverDistance - secondMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4B.Id, newPlaсeСolumnMainRebar_4B);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4B.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);
                    }

                    //Если стыковка стержней на сварке без изменения сечения колонны выше
                    else if (squareColumnsReinforcementWPF.RebarConnectionOptionName == "radioButton_Mechanical"
                        && squareColumnsReinforcementWPF.OverlapTransitionChecked == false
                        && squareColumnsReinforcementWPF.SectionChangeChecked == false
                        && squareColumnsReinforcementWPF.BendInSlabChecked == false)
                    {
                        //Точки для построения кривых удлиненных стержней 
                        XYZ rebar_p1L = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), Math.Round(columnProperty.ColumnBasePoint.Z + firstRebarOutletsLength, 6));
                        XYZ rebar_p2L = new XYZ(Math.Round(rebar_p1L.X, 6), Math.Round(rebar_p1L.Y, 6), Math.Round(rebar_p1L.Z + columnProperty.ColumnLength + floorThickness, 6));

                        //Точки для построения кривых укороченных стержней
                        XYZ rebar_p1S = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), Math.Round(columnProperty.ColumnBasePoint.Z + secondRebarOutletsLength, 6));
                        XYZ rebar_p2S = new XYZ(Math.Round(rebar_p1S.X, 6), Math.Round(rebar_p1S.Y, 6), Math.Round(rebar_p1S.Z + columnProperty.ColumnLength + floorThickness, 6));

                        XYZ mechanicalConnection_p0L = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), columnProperty.ColumnLength + floorThickness + firstRebarOutletsLength + columnProperty.BaseLevelOffset);
                        XYZ mechanicalConnection_p0S = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), columnProperty.ColumnLength + floorThickness + secondRebarOutletsLength + columnProperty.BaseLevelOffset);

                        //Кривые стержня удлиненного
                        List<Curve> mainRebarCurvesL = new List<Curve>();

                        Curve line1L = Line.CreateBound(rebar_p1L, rebar_p2L) as Curve;
                        mainRebarCurvesL.Add(line1L);

                        //Кривые стержня укороченного
                        List<Curve> mainRebarCurvesS = new List<Curve>();

                        Curve line1S = Line.CreateBound(rebar_p1S, rebar_p2S) as Curve;
                        mainRebarCurvesS.Add(line1S);

                        //Нижний левый угол
                        Rebar columnMainRebar_1 = null;
                        try
                        {
                            columnMainRebar_1 = Rebar.CreateFromCurvesAndShape(doc
                                , form01
                                , firstMainBarTape
                                , null
                                , null
                                , column
                                , new XYZ(0, 1, 0)
                                , mainRebarCurvesL
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);
                        }
                        catch
                        {
                            TaskDialog.Show("Revit", "Не удалось создать прямой стержень! Возможно выбран некорректный тип формы 01!");
                            return Result.Cancelled;
                        }

                        if (columnMainRebar_1 == null)
                        {
                            TaskDialog.Show("Revit", "Не удалось создать прямой стержень! Возможно выбран некорректный тип формы 01!");
                            return Result.Cancelled;
                        }

                        XYZ newPlaсeСolumnMainRebar_1 = new XYZ(-columnProperty.ColumnSectionHeight / 2
                            + coverDistance
                            + firstMainBarDiam / 2
                            , -columnProperty.ColumnSectionWidth / 2
                            + coverDistance
                            + firstMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1.Id, newPlaсeСolumnMainRebar_1);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        FamilyInstance mechanicalConnection_1 = doc.Create.NewFamilyInstance(mechanicalConnection_p0L
                            , firstMechanicalConnectionFamilySymbol
                            , columnProperty.BaseLevel
                            , StructuralType.NonStructural);
                        ElementTransformUtils.MoveElement(doc, mechanicalConnection_1.Id, newPlaсeСolumnMainRebar_1);
                        ElementTransformUtils.RotateElement(doc, mechanicalConnection_1.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Верхний левый угол
                        Rebar columnMainRebar_2 = Rebar.CreateFromCurvesAndShape(doc
                            , form01
                            , firstMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebar_2 = new XYZ(-columnProperty.ColumnSectionHeight / 2
                            + coverDistance
                            + firstMainBarDiam / 2
                            , columnProperty.ColumnSectionWidth / 2
                            - coverDistance
                            - firstMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2.Id, newPlaсeСolumnMainRebar_2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        FamilyInstance mechanicalConnection_2 = doc.Create.NewFamilyInstance(mechanicalConnection_p0S
                            , firstMechanicalConnectionFamilySymbol
                            , columnProperty.BaseLevel
                            , StructuralType.NonStructural);
                        ElementTransformUtils.MoveElement(doc, mechanicalConnection_2.Id, newPlaсeСolumnMainRebar_2);
                        ElementTransformUtils.RotateElement(doc, mechanicalConnection_2.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Верхний правый угол
                        Rebar columnMainRebar_3 = Rebar.CreateFromCurvesAndShape(doc
                            , form01
                            , firstMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainRebarCurvesL
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebar_3 = new XYZ(columnProperty.ColumnSectionHeight / 2
                            - coverDistance
                            - firstMainBarDiam / 2
                            , columnProperty.ColumnSectionWidth / 2
                            - coverDistance
                            - firstMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3.Id, newPlaсeСolumnMainRebar_3);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        FamilyInstance mechanicalConnection_3 = doc.Create.NewFamilyInstance(mechanicalConnection_p0L
                            , firstMechanicalConnectionFamilySymbol
                            , columnProperty.BaseLevel
                            , StructuralType.NonStructural);
                        ElementTransformUtils.MoveElement(doc, mechanicalConnection_3.Id, newPlaсeСolumnMainRebar_3);
                        ElementTransformUtils.RotateElement(doc, mechanicalConnection_3.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Нижний правый угол
                        Rebar columnMainRebar_4 = Rebar.CreateFromCurvesAndShape(doc
                            , form01
                            , firstMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebar_4 = new XYZ(columnProperty.ColumnSectionHeight / 2
                            - coverDistance
                            - firstMainBarDiam / 2
                            , -columnProperty.ColumnSectionWidth / 2
                            + coverDistance
                            + firstMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4.Id, newPlaсeСolumnMainRebar_4);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        FamilyInstance mechanicalConnection_4 = doc.Create.NewFamilyInstance(mechanicalConnection_p0S
                            , firstMechanicalConnectionFamilySymbol
                            , columnProperty.BaseLevel
                            , StructuralType.NonStructural);
                        ElementTransformUtils.MoveElement(doc, mechanicalConnection_4.Id, newPlaсeСolumnMainRebar_4);
                        ElementTransformUtils.RotateElement(doc, mechanicalConnection_4.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Центральный левый нижний стержень
                        Rebar columnMainRebar_1A = Rebar.CreateFromCurvesAndShape(doc
                            , form01
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebar_1A = new XYZ(-columnProperty.ColumnSectionHeight / 2
                            + coverDistance
                            + secondMainBarDiam / 2
                            , -firstLowerRebarOffset, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1A.Id, newPlaсeСolumnMainRebar_1A);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1A.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        FamilyInstance mechanicalConnection_1A = doc.Create.NewFamilyInstance(mechanicalConnection_p0S
                            , secondMechanicalConnectionFamilySymbol
                            , columnProperty.BaseLevel
                            , StructuralType.NonStructural);
                        ElementTransformUtils.MoveElement(doc, mechanicalConnection_1A.Id, newPlaсeСolumnMainRebar_1A);
                        ElementTransformUtils.RotateElement(doc, mechanicalConnection_1A.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Центральный левый верхний стержень
                        Rebar columnMainRebar_1B = Rebar.CreateFromCurvesAndShape(doc
                            , form01
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainRebarCurvesL
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebar_1B = new XYZ(-columnProperty.ColumnSectionHeight / 2 + coverDistance + secondMainBarDiam / 2, firstTopRebarOffset, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1B.Id, newPlaсeСolumnMainRebar_1B);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1B.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        FamilyInstance mechanicalConnection_1B = doc.Create.NewFamilyInstance(mechanicalConnection_p0L
                            , secondMechanicalConnectionFamilySymbol
                            , columnProperty.BaseLevel
                            , StructuralType.NonStructural);
                        ElementTransformUtils.MoveElement(doc, mechanicalConnection_1B.Id, newPlaсeСolumnMainRebar_1B);
                        ElementTransformUtils.RotateElement(doc, mechanicalConnection_1B.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);


                        //Центральный правый нижний стержень
                        Rebar columnMainRebar_2A = Rebar.CreateFromCurvesAndShape(doc
                            , form01
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainRebarCurvesL
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebar_2A = new XYZ(columnProperty.ColumnSectionHeight / 2
                            - coverDistance
                            - secondMainBarDiam / 2
                            , -firstLowerRebarOffset, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2A.Id, newPlaсeСolumnMainRebar_2A);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2A.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        FamilyInstance mechanicalConnection_2A = doc.Create.NewFamilyInstance(mechanicalConnection_p0L
                            , secondMechanicalConnectionFamilySymbol
                            , columnProperty.BaseLevel
                            , StructuralType.NonStructural);
                        ElementTransformUtils.MoveElement(doc, mechanicalConnection_2A.Id, newPlaсeСolumnMainRebar_2A);
                        ElementTransformUtils.RotateElement(doc, mechanicalConnection_2A.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Центральный правый верхний стержень
                        Rebar columnMainRebar_2B = Rebar.CreateFromCurvesAndShape(doc
                            , form01
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebar_2B = new XYZ(columnProperty.ColumnSectionHeight / 2 - coverDistance - secondMainBarDiam / 2, firstTopRebarOffset, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2B.Id, newPlaсeСolumnMainRebar_2B);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2B.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        FamilyInstance mechanicalConnection_2B = doc.Create.NewFamilyInstance(mechanicalConnection_p0S
                            , secondMechanicalConnectionFamilySymbol
                            , columnProperty.BaseLevel
                            , StructuralType.NonStructural);
                        ElementTransformUtils.MoveElement(doc, mechanicalConnection_2B.Id, newPlaсeСolumnMainRebar_2B);
                        ElementTransformUtils.RotateElement(doc, mechanicalConnection_2B.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);


                        //Центральный нижний левый стержень
                        Rebar columnMainRebar_3A = Rebar.CreateFromCurvesAndShape(doc
                            , form01
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebar_3A = new XYZ(-firstLeftRebarOffset
                            , -columnProperty.ColumnSectionWidth / 2
                            + coverDistance
                            + secondMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3A.Id, newPlaсeСolumnMainRebar_3A);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3A.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        FamilyInstance mechanicalConnection_3A = doc.Create.NewFamilyInstance(mechanicalConnection_p0S
                            , secondMechanicalConnectionFamilySymbol
                            , columnProperty.BaseLevel
                            , StructuralType.NonStructural);
                        ElementTransformUtils.MoveElement(doc, mechanicalConnection_3A.Id, newPlaсeСolumnMainRebar_3A);
                        ElementTransformUtils.RotateElement(doc, mechanicalConnection_3A.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Центральный нижний правый стержень
                        Rebar columnMainRebar_3B = Rebar.CreateFromCurvesAndShape(doc
                            , form01
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainRebarCurvesL
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebar_3B = new XYZ(firstRightRebarOffset, -columnProperty.ColumnSectionWidth / 2 + coverDistance + secondMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3B.Id, newPlaсeСolumnMainRebar_3B);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3B.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        FamilyInstance mechanicalConnection_3B = doc.Create.NewFamilyInstance(mechanicalConnection_p0L
                            , secondMechanicalConnectionFamilySymbol
                            , columnProperty.BaseLevel
                            , StructuralType.NonStructural);
                        ElementTransformUtils.MoveElement(doc, mechanicalConnection_3B.Id, newPlaсeСolumnMainRebar_3B);
                        ElementTransformUtils.RotateElement(doc, mechanicalConnection_3B.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);


                        //Центральный верхний левый стержень
                        Rebar columnMainRebar_4A = Rebar.CreateFromCurvesAndShape(doc
                            , form01
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainRebarCurvesL
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebar_4A = new XYZ(-firstLeftRebarOffset, columnProperty.ColumnSectionWidth / 2 - coverDistance - secondMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4A.Id, newPlaсeСolumnMainRebar_4A);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4A.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        FamilyInstance mechanicalConnection_4A = doc.Create.NewFamilyInstance(mechanicalConnection_p0L
                            , secondMechanicalConnectionFamilySymbol
                            , columnProperty.BaseLevel
                            , StructuralType.NonStructural);
                        ElementTransformUtils.MoveElement(doc, mechanicalConnection_4A.Id, newPlaсeСolumnMainRebar_4A);
                        ElementTransformUtils.RotateElement(doc, mechanicalConnection_4A.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Центральный верхний правый стержень
                        Rebar columnMainRebar_4B = Rebar.CreateFromCurvesAndShape(doc
                            , form01
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebar_4B = new XYZ(firstRightRebarOffset, columnProperty.ColumnSectionWidth / 2 - coverDistance - secondMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4B.Id, newPlaсeСolumnMainRebar_4B);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4B.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        FamilyInstance mechanicalConnection_4B = doc.Create.NewFamilyInstance(mechanicalConnection_p0S
                            , secondMechanicalConnectionFamilySymbol
                            , columnProperty.BaseLevel
                            , StructuralType.NonStructural);
                        ElementTransformUtils.MoveElement(doc, mechanicalConnection_4B.Id, newPlaсeСolumnMainRebar_4B);
                        ElementTransformUtils.RotateElement(doc, mechanicalConnection_4B.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);
                    }

                    //Если стыковка стержней на сварке загиб в плиту
                    else if (squareColumnsReinforcementWPF.RebarConnectionOptionName == "radioButton_Mechanical"
                        && squareColumnsReinforcementWPF.OverlapTransitionChecked == false
                        && squareColumnsReinforcementWPF.SectionChangeChecked == false
                        && squareColumnsReinforcementWPF.BendInSlabChecked == true)
                    {
                        //Точки для построения кривых удлиненных стержней (начало и конец удлиненные)
                        XYZ rebar_p1L = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), Math.Round(columnProperty.ColumnBasePoint.Z + firstRebarOutletsLength, 6));
                        XYZ rebar_p2L = new XYZ(Math.Round(rebar_p1L.X, 6), Math.Round(rebar_p1L.Y, 6), Math.Round(rebar_p1L.Z - firstRebarOutletsLength + columnProperty.ColumnLength + floorThickness - bendInSlab - firstMainBarDiam / 2, 6));
                        XYZ rebar_p3L = new XYZ(Math.Round(rebar_p2L.X + firstRebarOutletsLength - (floorThickness - bendInSlab - firstMainBarDiam / 2), 6), Math.Round(rebar_p2L.Y, 6), Math.Round(rebar_p2L.Z, 6));

                        //Точки для построения кривых удлиненных стержней (начало укороченное и конец удлиненный)
                        XYZ rebar_p1S = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), Math.Round(columnProperty.ColumnBasePoint.Z + secondRebarOutletsLength, 6));
                        XYZ rebar_p2S = new XYZ(Math.Round(rebar_p1S.X, 6), Math.Round(rebar_p1S.Y, 6), Math.Round(rebar_p1S.Z - secondRebarOutletsLength + columnProperty.ColumnLength + floorThickness - bendInSlab - firstMainBarDiam / 2, 6));
                        XYZ rebar_p3S = new XYZ(Math.Round(rebar_p2S.X + firstRebarOutletsLength - (floorThickness - bendInSlab - firstMainBarDiam / 2), 6), Math.Round(rebar_p2S.Y, 6), Math.Round(rebar_p2S.Z, 6));

                        //Точки для построения кривых укороченных стержней (начало и конец укороченные)
                        XYZ rebar_p1AS = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), Math.Round(columnProperty.ColumnBasePoint.Z + secondRebarOutletsLength, 6));
                        XYZ rebar_p2AS = new XYZ(Math.Round(rebar_p1AS.X, 6), Math.Round(rebar_p1AS.Y, 6), Math.Round(rebar_p1AS.Z - secondRebarOutletsLength + columnProperty.ColumnLength + floorThickness - bendInSlab - secondMainBarDiam / 2, 6));
                        XYZ rebar_p3AS = new XYZ(Math.Round(rebar_p2AS.X + secondRebarOutletsLength - (floorThickness - bendInSlab - secondMainBarDiam / 2), 6), Math.Round(rebar_p2AS.Y, 6), Math.Round(rebar_p2AS.Z, 6));

                        //Точки для построения кривых укороченных стержней (начало удлиненное и конец укороченный)
                        XYZ rebar_p1AL = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), Math.Round(columnProperty.ColumnBasePoint.Z + firstRebarOutletsLength, 6));
                        XYZ rebar_p2AL = new XYZ(Math.Round(rebar_p1AL.X, 6), Math.Round(rebar_p1AL.Y, 6), Math.Round(rebar_p1AL.Z - firstRebarOutletsLength + columnProperty.ColumnLength + floorThickness - bendInSlab - secondMainBarDiam / 2, 6));
                        XYZ rebar_p3AL = new XYZ(Math.Round(rebar_p2AL.X + secondRebarOutletsLength - (floorThickness - bendInSlab - secondMainBarDiam / 2), 6), Math.Round(rebar_p2AL.Y, 6), Math.Round(rebar_p2AL.Z, 6));

                        //Кривые основных угловых стержней (начало и конец удлиненные)
                        List<Curve> mainFirstRebarCurvesL = new List<Curve>();

                        Curve line1L = Line.CreateBound(rebar_p1L, rebar_p2L) as Curve;
                        mainFirstRebarCurvesL.Add(line1L);
                        Curve line2L = Line.CreateBound(rebar_p2L, rebar_p3L) as Curve;
                        mainFirstRebarCurvesL.Add(line2L);

                        //Кривые основных угловых стержней (начало укороченное и конец удлиненный)
                        List<Curve> mainFirstRebarCurvesS = new List<Curve>();

                        Curve line1S = Line.CreateBound(rebar_p1S, rebar_p2S) as Curve;
                        mainFirstRebarCurvesS.Add(line1S);
                        Curve line2S = Line.CreateBound(rebar_p2S, rebar_p3S) as Curve;
                        mainFirstRebarCurvesS.Add(line2S);

                        //Кривые основных боковых стержней (начало и конец укороченные)
                        List<Curve> mainSecondRebarCurvesS = new List<Curve>();

                        Curve line1AS = Line.CreateBound(rebar_p1AS, rebar_p2AS) as Curve;
                        mainSecondRebarCurvesS.Add(line1AS);
                        Curve line2AS = Line.CreateBound(rebar_p2AS, rebar_p3AS) as Curve;
                        mainSecondRebarCurvesS.Add(line2AS);

                        //Кривые основных боковых стержней (начало удлиненное и конец укороченный)
                        List<Curve> mainSecondRebarCurvesL = new List<Curve>();

                        Curve line1AL = Line.CreateBound(rebar_p1AL, rebar_p2AL) as Curve;
                        mainSecondRebarCurvesL.Add(line1AL);
                        Curve line2AL = Line.CreateBound(rebar_p2AL, rebar_p3AL) as Curve;
                        mainSecondRebarCurvesL.Add(line2AL);

                        //Нижний левый угол
                        Rebar columnMainRebar_1 = null;
                        try
                        {
                            columnMainRebar_1 = Rebar.CreateFromCurvesAndShape(doc
                                , form11
                                , firstMainBarTape
                                , null
                                , null
                                , column
                                , new XYZ(0, 1, 0)
                                , mainFirstRebarCurvesL
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);
                        }
                        catch
                        {
                            TaskDialog.Show("Revit", "Не удалось создать Г-образный стержень! Возможно выбран некорректный тип формы 11!");
                            return Result.Cancelled;
                        }

                        if (columnMainRebar_1 == null)
                        {
                            TaskDialog.Show("Revit", "Не удалось создать Г-образный стержень! Возможно выбран некорректный тип формы 11!");
                            return Result.Cancelled;
                        }

                        XYZ rotate1_p1 = new XYZ(rebar_p1L.X, rebar_p1L.Y, rebar_p1L.Z);
                        XYZ rotate1_p2 = new XYZ(rebar_p1L.X, rebar_p1L.Y, rebar_p1L.Z + 1);
                        Line rotateLine = Line.CreateBound(rotate1_p1, rotate1_p2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1.Id, rotateLine, 180 * (Math.PI / 180));

                        XYZ newPlaсeСolumnMainRebar_1 = new XYZ(-columnProperty.ColumnSectionHeight / 2
                            + coverDistance
                            + firstMainBarDiam / 2
                            , -columnProperty.ColumnSectionWidth / 2
                            + coverDistance
                            + firstMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1.Id, newPlaсeСolumnMainRebar_1);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Верхний левый угол
                        Rebar columnMainRebar_2 = Rebar.CreateFromCurvesAndShape(doc
                            , form11
                            , firstMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainFirstRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_2 = new XYZ(-columnProperty.ColumnSectionHeight / 2
                            + coverDistance
                            + firstMainBarDiam / 2
                            , columnProperty.ColumnSectionWidth / 2
                            - coverDistance
                            - firstMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2.Id, newPlaсeСolumnRebar_2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Верхний правый угол
                        Rebar columnMainRebar_3 = Rebar.CreateFromCurvesAndShape(doc
                            , form11
                            , firstMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainFirstRebarCurvesL
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        XYZ newPlaсeСolumnRebar_3 = new XYZ(columnProperty.ColumnSectionHeight / 2
                            - coverDistance
                            - firstMainBarDiam / 2
                            , columnProperty.ColumnSectionWidth / 2
                            - coverDistance
                            - firstMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3.Id, newPlaсeСolumnRebar_3);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Нижний правый угол
                        Rebar columnMainRebar_4 = Rebar.CreateFromCurvesAndShape(doc
                            , form11
                            , firstMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainFirstRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        XYZ newPlaсeСolumnRebar_4 = new XYZ(columnProperty.ColumnSectionHeight / 2
                            - coverDistance
                            - firstMainBarDiam / 2
                            , -columnProperty.ColumnSectionWidth / 2
                            + coverDistance
                            + firstMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4.Id, newPlaсeСolumnRebar_4);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Центральный левый нижний стержень
                        Rebar columnMainRebar_1A = Rebar.CreateFromCurvesAndShape(doc
                            , form11
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1A.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnMainRebar_1A = new XYZ(-columnProperty.ColumnSectionHeight / 2
                            + coverDistance
                            , -firstLowerRebarOffset, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1A.Id, newPlaсeСolumnMainRebar_1A);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1A.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Центральный левый верхний стержень
                        Rebar columnMainRebar_1B = Rebar.CreateFromCurvesAndShape(doc
                            , form11
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesL
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1B.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnMainRebar_1B = new XYZ(-columnProperty.ColumnSectionHeight / 2
                            + coverDistance
                            + secondMainBarDiam / 2
                            , firstTopRebarOffset, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1B.Id, newPlaсeСolumnMainRebar_1B);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1B.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);


                        //Центральный правый нижний стержень
                        Rebar columnMainRebar_2A = Rebar.CreateFromCurvesAndShape(doc
                            , form11
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesL
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        XYZ newPlaсeСolumnMainRebar_2A = new XYZ(+columnProperty.ColumnSectionHeight / 2
                            - coverDistance
                            - secondMainBarDiam / 2
                            , -firstLowerRebarOffset, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2A.Id, newPlaсeСolumnMainRebar_2A);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2A.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Центральный правый верхний стержень
                        Rebar columnMainRebar_2B = Rebar.CreateFromCurvesAndShape(doc
                            , form11
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        XYZ newPlaсeСolumnMainRebar_2B = new XYZ(+columnProperty.ColumnSectionHeight / 2
                            - coverDistance
                            - secondMainBarDiam / 2
                            , firstTopRebarOffset, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2B.Id, newPlaсeСolumnMainRebar_2B);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2B.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);


                        //Центральный нижний левый стержень
                        Rebar columnMainRebar_3A = Rebar.CreateFromCurvesAndShape(doc
                            , form11
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3A.Id, rotateLine, -90 * (Math.PI / 180));
                        XYZ newPlaсeСolumnMainRebar_3A = new XYZ(-firstLeftRebarOffset
                            , -columnProperty.ColumnSectionWidth / 2
                            + coverDistance
                            + secondMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3A.Id, newPlaсeСolumnMainRebar_3A);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3A.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Центральный нижний правый стержень
                        Rebar columnMainRebar_3B = Rebar.CreateFromCurvesAndShape(doc
                            , form11
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesL
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3B.Id, rotateLine, -90 * (Math.PI / 180));
                        XYZ newPlaсeСolumnMainRebar_3B = new XYZ(firstRightRebarOffset
                            , -columnProperty.ColumnSectionWidth / 2
                            + coverDistance
                            + secondMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3B.Id, newPlaсeСolumnMainRebar_3B);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3B.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);


                        //Центральный верхний левый стержень
                        Rebar columnMainRebar_4A = Rebar.CreateFromCurvesAndShape(doc
                            , form11
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesL
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4A.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeСolumnMainRebar_4A = new XYZ(-firstLeftRebarOffset
                            , columnProperty.ColumnSectionWidth / 2
                            - coverDistance
                            - secondMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4A.Id, newPlaсeСolumnMainRebar_4A);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4A.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Центральный верхний правый стержень
                        Rebar columnMainRebar_4B = Rebar.CreateFromCurvesAndShape(doc
                            , form11
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4B.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeСolumnMainRebar_4B = new XYZ(firstRightRebarOffset
                            , columnProperty.ColumnSectionWidth / 2
                            - coverDistance
                            - secondMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4B.Id, newPlaсeСolumnMainRebar_4B);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4B.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);
                    }

                    //Если переход со сварки в нахлест без изменения сечения колонны выше
                    else if (squareColumnsReinforcementWPF.RebarConnectionOptionName == "radioButton_Mechanical"
                        && squareColumnsReinforcementWPF.OverlapTransitionChecked == true
                        && squareColumnsReinforcementWPF.SectionChangeChecked == false)
                    {
                        //Точки для построения кривых основных угловых стержней с двойным нахлестом
                        XYZ rebar_p1L = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), Math.Round(columnProperty.ColumnBasePoint.Z + firstRebarOutletsLength, 6));
                        XYZ rebar_p2L = new XYZ(Math.Round(rebar_p1L.X, 6), Math.Round(rebar_p1L.Y, 6), Math.Round(rebar_p1L.Z + columnProperty.ColumnLength - firstRebarOutletsLength, 6));
                        XYZ rebar_p3L = new XYZ(Math.Round(rebar_p2L.X + firstMainBarDiam, 6), Math.Round(rebar_p2L.Y, 6), Math.Round(rebar_p2L.Z + floorThickness, 6));
                        XYZ rebar_p4L = new XYZ(Math.Round(rebar_p3L.X, 6), Math.Round(rebar_p3L.Y, 6), Math.Round(rebar_p3L.Z + firstRebarOutletsLength, 6));

                        //Точки для построения кривых основных угловых стержней с одинарным нахлестом
                        XYZ rebar_p1S = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), Math.Round(columnProperty.ColumnBasePoint.Z + secondRebarOutletsLength, 6));
                        XYZ rebar_p2S = new XYZ(Math.Round(rebar_p1S.X, 6), Math.Round(rebar_p1S.Y, 6), Math.Round(rebar_p1S.Z + columnProperty.ColumnLength - secondRebarOutletsLength, 6));
                        XYZ rebar_p3S = new XYZ(Math.Round(rebar_p2S.X + firstMainBarDiam, 6), Math.Round(rebar_p2S.Y, 6), Math.Round(rebar_p2S.Z + floorThickness, 6));
                        XYZ rebar_p4S = new XYZ(Math.Round(rebar_p3S.X, 6), Math.Round(rebar_p3S.Y, 6), Math.Round(rebar_p3S.Z + secondRebarOutletsLength, 6));

                        //Точки для построения кривых основных боковых стержней с двойным нахлестом
                        XYZ rebar_p1AL = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), Math.Round(columnProperty.ColumnBasePoint.Z + firstRebarOutletsLength, 6));
                        XYZ rebar_p2AL = new XYZ(Math.Round(rebar_p1AL.X, 6), Math.Round(rebar_p1AL.Y, 6), Math.Round(rebar_p1AL.Z + columnProperty.ColumnLength - firstRebarOutletsLength, 6));
                        XYZ rebar_p3AL = new XYZ(Math.Round(rebar_p2AL.X + secondMainBarDiam, 6), Math.Round(rebar_p2AL.Y, 6), Math.Round(rebar_p2AL.Z + floorThickness, 6));
                        XYZ rebar_p4AL = new XYZ(Math.Round(rebar_p3AL.X, 6), Math.Round(rebar_p3AL.Y, 6), Math.Round(rebar_p3AL.Z + firstRebarOutletsLength, 6));

                        //Точки для построения кривых основных боковых стержней с одинарным нахлестом
                        XYZ rebar_p1AS = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), Math.Round(columnProperty.ColumnBasePoint.Z + secondRebarOutletsLength, 6));
                        XYZ rebar_p2AS = new XYZ(Math.Round(rebar_p1AS.X, 6), Math.Round(rebar_p1AS.Y, 6), Math.Round(rebar_p1AS.Z + columnProperty.ColumnLength - secondRebarOutletsLength, 6));
                        XYZ rebar_p3AS = new XYZ(Math.Round(rebar_p2AS.X + secondMainBarDiam, 6), Math.Round(rebar_p2AS.Y, 6), Math.Round(rebar_p2AS.Z + floorThickness, 6));
                        XYZ rebar_p4AS = new XYZ(Math.Round(rebar_p3AS.X, 6), Math.Round(rebar_p3AS.Y, 6), Math.Round(rebar_p3AS.Z + secondRebarOutletsLength, 6));

                        //Кривые основных угловых стержней с двойным нахлестом
                        List<Curve> mainFirstRebarCurvesL = new List<Curve>();

                        Curve line1L = Line.CreateBound(rebar_p1L, rebar_p2L) as Curve;
                        mainFirstRebarCurvesL.Add(line1L);
                        Curve line2L = Line.CreateBound(rebar_p2L, rebar_p3L) as Curve;
                        mainFirstRebarCurvesL.Add(line2L);
                        Curve line3L = Line.CreateBound(rebar_p3L, rebar_p4L) as Curve;
                        mainFirstRebarCurvesL.Add(line3L);

                        //Кривые основных угловых стержней с одинарным нахлестом
                        List<Curve> mainFirstRebarCurvesS = new List<Curve>();

                        Curve line1S = Line.CreateBound(rebar_p1S, rebar_p2S) as Curve;
                        mainFirstRebarCurvesS.Add(line1S);
                        Curve line2S = Line.CreateBound(rebar_p2S, rebar_p3S) as Curve;
                        mainFirstRebarCurvesS.Add(line2L);
                        Curve line3S = Line.CreateBound(rebar_p3S, rebar_p4S) as Curve;
                        mainFirstRebarCurvesS.Add(line3S);

                        //Кривые основных боковых стержней c двойным нахлестом
                        List<Curve> mainSecondRebarCurvesL = new List<Curve>();

                        Curve line1AL = Line.CreateBound(rebar_p1AL, rebar_p2AL) as Curve;
                        mainSecondRebarCurvesL.Add(line1AL);
                        Curve line2AL = Line.CreateBound(rebar_p2AL, rebar_p3AL) as Curve;
                        mainSecondRebarCurvesL.Add(line2AL);
                        Curve line3AL = Line.CreateBound(rebar_p3AL, rebar_p4AL) as Curve;
                        mainSecondRebarCurvesL.Add(line3AL);

                        //Кривые основных боковых стержней c одинарным нахлестом
                        List<Curve> mainSecondRebarCurvesS = new List<Curve>();

                        Curve line1AS = Line.CreateBound(rebar_p1AS, rebar_p2AS) as Curve;
                        mainSecondRebarCurvesS.Add(line1AS);
                        Curve line2AS = Line.CreateBound(rebar_p2AS, rebar_p3AS) as Curve;
                        mainSecondRebarCurvesS.Add(line2AS);
                        Curve line3AS = Line.CreateBound(rebar_p3AS, rebar_p4AS) as Curve;
                        mainSecondRebarCurvesS.Add(line3AS);

                        //Нижний левый угол
                        Rebar columnMainRebar_1 = null;
                        try
                        {
                            columnMainRebar_1 = Rebar.CreateFromCurvesAndShape(doc
                                , form26
                                , firstMainBarTape
                                , null
                                , null
                                , column
                                , new XYZ(0, 1, 0)
                                , mainFirstRebarCurvesL
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);
                        }
                        catch
                        {
                            TaskDialog.Show("Revit", "Не удалось создать Z-образный стержень! Возможно выбран некорректный тип формы 26!");
                            return Result.Cancelled;
                        }

                        if (columnMainRebar_1 == null)
                        {
                            TaskDialog.Show("Revit", "Не удалось создать Z-образный стержень! Возможно выбран некорректный тип формы 26!");
                            return Result.Cancelled;
                        }

                        XYZ newPlaсeСolumnMainRebar_1 = new XYZ(-columnProperty.ColumnSectionHeight / 2
                            + coverDistance
                            + firstMainBarDiam / 2
                            , -columnProperty.ColumnSectionWidth / 2
                            + coverDistance
                            + firstMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1.Id, newPlaсeСolumnMainRebar_1);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Верхний левый угол
                        Rebar columnMainRebar_2 = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , firstMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainFirstRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnRebar_2 = new XYZ(-columnProperty.ColumnSectionHeight / 2
                            + coverDistance
                            + firstMainBarDiam / 2
                            , columnProperty.ColumnSectionWidth / 2
                            - coverDistance
                            - firstMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2.Id, newPlaсeСolumnRebar_2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Верхний правый угол
                        Rebar columnMainRebar_3 = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , firstMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainFirstRebarCurvesL
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ rotate1_p1 = new XYZ(rebar_p1L.X, rebar_p1L.Y, rebar_p1L.Z);
                        XYZ rotate1_p2 = new XYZ(rebar_p1L.X, rebar_p1L.Y, rebar_p1L.Z + 1);
                        Line rotateLine = Line.CreateBound(rotate1_p1, rotate1_p2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_3 = new XYZ(columnProperty.ColumnSectionHeight / 2
                            - coverDistance
                            - firstMainBarDiam / 2
                            , columnProperty.ColumnSectionWidth / 2
                            - coverDistance
                            - firstMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3.Id, newPlaсeСolumnRebar_3);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Нижний правый угол
                        Rebar columnMainRebar_4 = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , firstMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainFirstRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4.Id, rotateLine, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_4 = new XYZ(columnProperty.ColumnSectionHeight / 2
                            - coverDistance
                            - firstMainBarDiam / 2
                            , -columnProperty.ColumnSectionWidth / 2
                            + coverDistance
                            + firstMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4.Id, newPlaсeСolumnRebar_4);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);


                        //Центральный левый нижний стержень
                        Rebar columnMainRebar_1A = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1A.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeСolumnMainRebar_1A = new XYZ(-columnProperty.ColumnSectionHeight / 2
                            + coverDistance
                            + secondMainBarDiam / 2
                            , -firstLowerRebarOffset, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1A.Id, newPlaсeСolumnMainRebar_1A);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1A.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Центральный левый верхний стержень
                        Rebar columnMainRebar_1B = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesL
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1B.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeСolumnMainRebar_1B = new XYZ(-columnProperty.ColumnSectionHeight / 2
                            + coverDistance
                            + secondMainBarDiam / 2
                            , firstTopRebarOffset, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1B.Id, newPlaсeСolumnMainRebar_1B);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1B.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);


                        //Центральный правый нижний стержень
                        Rebar columnMainRebar_2A = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesL
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2A.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeСolumnMainRebar_2A = new XYZ(+columnProperty.ColumnSectionHeight / 2
                            - coverDistance
                            - secondMainBarDiam / 2
                            , -firstLowerRebarOffset, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2A.Id, newPlaсeСolumnMainRebar_2A);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2A.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Центральный правый верхний стержень
                        Rebar columnMainRebar_2B = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2B.Id, rotateLine, 90 * (Math.PI / 180));
                        XYZ newPlaсeСolumnMainRebar_2B = new XYZ(columnProperty.ColumnSectionHeight / 2 - coverDistance - secondMainBarDiam / 2, firstTopRebarOffset, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2B.Id, newPlaсeСolumnMainRebar_2B);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2B.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);


                        //Центральный нижний левый стержень
                        Rebar columnMainRebar_3A = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebar_3A = new XYZ(-firstLeftRebarOffset, -columnProperty.ColumnSectionWidth / 2 + coverDistance + secondMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3A.Id, newPlaсeСolumnMainRebar_3A);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3A.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Центральный нижний правый стержень
                        Rebar columnMainRebar_3B = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesL
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebar_3B = new XYZ(firstRightRebarOffset, -columnProperty.ColumnSectionWidth / 2 + coverDistance + secondMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3B.Id, newPlaсeСolumnMainRebar_3B);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3B.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);


                        //Центральный верхний левый стержень
                        Rebar columnMainRebar_4A = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesL
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebar_4A = new XYZ(-firstLeftRebarOffset, columnProperty.ColumnSectionWidth / 2 - coverDistance - secondMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4A.Id, newPlaсeСolumnMainRebar_4A);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4A.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Центральный верхний правый стержень
                        Rebar columnMainRebar_4B = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebar_4B = new XYZ(firstRightRebarOffset, columnProperty.ColumnSectionWidth / 2 - coverDistance - secondMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4B.Id, newPlaсeСolumnMainRebar_4B);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4B.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);
                    }

                    //Если стыковка стержней в нахлест c изменением сечения колонны выше
                    else if (squareColumnsReinforcementWPF.RebarConnectionOptionName == "radioButton_Overlap"
                        && squareColumnsReinforcementWPF.SectionChangeChecked == true)
                    {
                        //Точки для построения кривых основных угловых удлиненных стержней
                        XYZ rebar_p1L = null;
                        XYZ rebar_p2L = null;
                        if (squareColumnsReinforcementWPF.DeepenRebarChecked)
                        {
                            rebar_p1L = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), Math.Round(columnProperty.ColumnBasePoint.Z - deepenRebar, 6));
                            rebar_p2L = new XYZ(Math.Round(rebar_p1L.X, 6), Math.Round(rebar_p1L.Y, 6), Math.Round(rebar_p1L.Z + deepenRebar + columnProperty.ColumnLength - (sectionOffset * 6 - floorThickness), 6));
                        }
                        else
                        {
                            rebar_p1L = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), Math.Round(columnProperty.ColumnBasePoint.Z, 6));
                            rebar_p2L = new XYZ(Math.Round(rebar_p1L.X, 6), Math.Round(rebar_p1L.Y, 6), Math.Round(rebar_p1L.Z + columnProperty.ColumnLength - (sectionOffset * 6 - floorThickness), 6));
                        }
                        XYZ rebar_p3L = new XYZ(Math.Round(rebar_p2L.X + deltaXOverlapping, 6), Math.Round(rebar_p2L.Y, 6), Math.Round(rebar_p2L.Z + floorThickness + (sectionOffset * 6 - floorThickness), 6));
                        XYZ rebar_p4L = new XYZ(Math.Round(rebar_p3L.X, 6), Math.Round(rebar_p3L.Y, 6), Math.Round(rebar_p3L.Z + firstRebarOutletsLength, 6));

                        //Точки для построения кривых основных угловых укороченных стержней
                        XYZ rebar_p1S = null;
                        XYZ rebar_p2S = null;
                        if (squareColumnsReinforcementWPF.DeepenRebarChecked)
                        {
                            rebar_p1S = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), Math.Round(columnProperty.ColumnBasePoint.Z - deepenRebar, 6));
                            rebar_p2S = new XYZ(Math.Round(rebar_p1S.X, 6), Math.Round(rebar_p1S.Y, 6), Math.Round(rebar_p1S.Z + deepenRebar + columnProperty.ColumnLength - (sectionOffset * 6 - floorThickness), 6));
                        }
                        else
                        {
                            rebar_p1S = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), Math.Round(columnProperty.ColumnBasePoint.Z, 6));
                            rebar_p2S = new XYZ(Math.Round(rebar_p1S.X, 6), Math.Round(rebar_p1S.Y, 6), Math.Round(rebar_p1S.Z + columnProperty.ColumnLength - (sectionOffset * 6 - floorThickness), 6));
                        }
                        XYZ rebar_p3S = new XYZ(Math.Round(rebar_p2S.X + deltaXOverlapping, 6), Math.Round(rebar_p2S.Y, 6), Math.Round(rebar_p2S.Z + floorThickness + (sectionOffset * 6 - floorThickness), 6));
                        XYZ rebar_p4S = new XYZ(Math.Round(rebar_p3S.X, 6), Math.Round(rebar_p3S.Y, 6), Math.Round(rebar_p3S.Z + secondRebarOutletsLength, 6));

                        //Точки для построения кривых основных боковых удлиненных стержней
                        XYZ rebar_p1AL = null;
                        XYZ rebar_p2AL = null;
                        if (squareColumnsReinforcementWPF.DeepenRebarChecked)
                        {
                            rebar_p1AL = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), Math.Round(columnProperty.ColumnBasePoint.Z - deepenRebar, 6));
                            rebar_p2AL = new XYZ(Math.Round(rebar_p1AL.X, 6), Math.Round(rebar_p1AL.Y, 6), Math.Round(rebar_p1AL.Z + deepenRebar + columnProperty.ColumnLength - (sectionOffset * 6 - floorThickness), 6));
                        }
                        else
                        {
                            rebar_p1AL = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), Math.Round(columnProperty.ColumnBasePoint.Z, 6));
                            rebar_p2AL = new XYZ(Math.Round(rebar_p1AL.X, 6), Math.Round(rebar_p1AL.Y, 6), Math.Round(rebar_p1AL.Z + columnProperty.ColumnLength - (sectionOffset * 6 - floorThickness), 6));
                        }
                        XYZ rebar_p3AL = new XYZ(Math.Round(rebar_p2AL.X + deltaXSecondOverlapping, 6), Math.Round(rebar_p2AL.Y, 6), Math.Round(rebar_p2AL.Z + floorThickness + (sectionOffset * 6 - floorThickness), 6));
                        XYZ rebar_p4AL = new XYZ(Math.Round(rebar_p3AL.X, 6), Math.Round(rebar_p3AL.Y, 6), Math.Round(rebar_p3AL.Z + firstRebarOutletsLength, 6));

                        //Точки для построения кривых основных боковых укороченных стержней
                        XYZ rebar_p1AS = null;
                        XYZ rebar_p2AS = null;
                        if (squareColumnsReinforcementWPF.DeepenRebarChecked)
                        {
                            rebar_p1AS = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), Math.Round(columnProperty.ColumnBasePoint.Z - deepenRebar, 6));
                            rebar_p2AS = new XYZ(Math.Round(rebar_p1AS.X, 6), Math.Round(rebar_p1AS.Y, 6), Math.Round(rebar_p1AS.Z + deepenRebar + columnProperty.ColumnLength - (sectionOffset * 6 - floorThickness), 6));
                        }
                        else
                        {
                            rebar_p1AS = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), Math.Round(columnProperty.ColumnBasePoint.Z, 6));
                            rebar_p2AS = new XYZ(Math.Round(rebar_p1AS.X, 6), Math.Round(rebar_p1AS.Y, 6), Math.Round(rebar_p1AS.Z + columnProperty.ColumnLength - (sectionOffset * 6 - floorThickness), 6));
                        }
                        XYZ rebar_p3AS = new XYZ(Math.Round(rebar_p2AS.X + deltaXSecondOverlapping, 6), Math.Round(rebar_p2AS.Y, 6), Math.Round(rebar_p2AS.Z + floorThickness + (sectionOffset * 6 - floorThickness), 6));
                        XYZ rebar_p4AS = new XYZ(Math.Round(rebar_p3AS.X, 6), Math.Round(rebar_p3AS.Y, 6), Math.Round(rebar_p3AS.Z + secondRebarOutletsLength, 6));


                        //Кривые основных угловых удлиненных стержней
                        List<Curve> mainFirstRebarCurvesL = new List<Curve>();

                        Curve line1L = Line.CreateBound(rebar_p1L, rebar_p2L) as Curve;
                        mainFirstRebarCurvesL.Add(line1L);
                        Curve line2L = Line.CreateBound(rebar_p2L, rebar_p3L) as Curve;
                        mainFirstRebarCurvesL.Add(line2L);
                        Curve line3L = Line.CreateBound(rebar_p3L, rebar_p4L) as Curve;
                        mainFirstRebarCurvesL.Add(line3L);

                        //Кривые основных угловых укороченных стержней
                        List<Curve> mainFirstRebarCurvesS = new List<Curve>();

                        Curve line1S = Line.CreateBound(rebar_p1S, rebar_p2S) as Curve;
                        mainFirstRebarCurvesS.Add(line1S);
                        Curve line2S = Line.CreateBound(rebar_p2S, rebar_p3S) as Curve;
                        mainFirstRebarCurvesS.Add(line2S);
                        Curve line3S = Line.CreateBound(rebar_p3S, rebar_p4S) as Curve;
                        mainFirstRebarCurvesS.Add(line3S);

                        //Кривые основных боковых удлиненных стержней
                        List<Curve> mainSecondRebarCurvesL = new List<Curve>();

                        Curve line1AL = Line.CreateBound(rebar_p1AL, rebar_p2AL) as Curve;
                        mainSecondRebarCurvesL.Add(line1AL);
                        Curve line2AL = Line.CreateBound(rebar_p2AL, rebar_p3AL) as Curve;
                        mainSecondRebarCurvesL.Add(line2AL);
                        Curve line3AL = Line.CreateBound(rebar_p3AL, rebar_p4AL) as Curve;
                        mainSecondRebarCurvesL.Add(line3AL);

                        //Кривые основных боковых укороченных стержней
                        List<Curve> mainSecondRebarCurvesS = new List<Curve>();

                        Curve line1AS = Line.CreateBound(rebar_p1AS, rebar_p2AS) as Curve;
                        mainSecondRebarCurvesS.Add(line1AS);
                        Curve line2AS = Line.CreateBound(rebar_p2AS, rebar_p3AS) as Curve;
                        mainSecondRebarCurvesS.Add(line2AS);
                        Curve line3AS = Line.CreateBound(rebar_p3AS, rebar_p4AS) as Curve;
                        mainSecondRebarCurvesS.Add(line3AS);

                        //Нижний левый угол
                        Rebar columnMainRebar_1 = null;
                        try
                        {
                            columnMainRebar_1 = Rebar.CreateFromCurvesAndShape(doc
                                , form26
                                , firstMainBarTape
                                , null
                                , null
                                , column
                                , new XYZ(0, 1, 0)
                                , mainFirstRebarCurvesL
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);
                        }
                        catch
                        {
                            TaskDialog.Show("Revit", "Не удалось создать Z-образный стержень! Возможно выбран некорректный тип формы 26!");
                            return Result.Cancelled;
                        }

                        if (columnMainRebar_1 == null)
                        {
                            TaskDialog.Show("Revit", "Не удалось создать Z-образный стержень! Возможно выбран некорректный тип формы 26!");
                            return Result.Cancelled;
                        }

                        XYZ rotate1_p1 = new XYZ(rebar_p1L.X, rebar_p1L.Y, rebar_p1L.Z);
                        XYZ rotate1_p2 = new XYZ(rebar_p1L.X, rebar_p1L.Y, rebar_p1L.Z + 1);
                        Line rotateLine = Line.CreateBound(rotate1_p1, rotate1_p2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1.Id, rotateLine, alphaOverlapping);
                        XYZ newPlaсeСolumnMainRebar_1 = new XYZ(-columnProperty.ColumnSectionHeight / 2
                            + coverDistance
                            + firstMainBarDiam / 2
                            , -columnProperty.ColumnSectionWidth / 2
                            + coverDistance
                            + firstMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1.Id, newPlaсeСolumnMainRebar_1);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Верхний левый угол
                        Rebar columnMainRebar_2 = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , firstMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainFirstRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2.Id, rotateLine, -alphaOverlapping);
                        XYZ newPlaсeСolumnRebar_2 = new XYZ(-columnProperty.ColumnSectionHeight / 2
                            + coverDistance
                            + firstMainBarDiam / 2
                            , columnProperty.ColumnSectionWidth / 2
                            - coverDistance
                            - firstMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2.Id, newPlaсeСolumnRebar_2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Верхний правый угол
                        Rebar columnMainRebar_3 = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , firstMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainFirstRebarCurvesL
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3.Id, rotateLine, alphaOverlapping);
                        XYZ rotate2_p1 = new XYZ(rebar_p1L.X, rebar_p1L.Y, rebar_p1L.Z);
                        XYZ rotate2_p2 = new XYZ(rebar_p1L.X, rebar_p1L.Y, rebar_p1L.Z + 1);
                        Line rotateLine2 = Line.CreateBound(rotate2_p1, rotate2_p2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3.Id, rotateLine2, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_3 = new XYZ(columnProperty.ColumnSectionHeight / 2
                            - coverDistance
                            - firstMainBarDiam / 2
                            , columnProperty.ColumnSectionWidth / 2
                            - coverDistance
                            - firstMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3.Id, newPlaсeСolumnRebar_3);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Нижний правый угол
                        Rebar columnMainRebar_4 = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , firstMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainFirstRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4.Id, rotateLine, -alphaOverlapping);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4.Id, rotateLine2, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_4 = new XYZ(columnProperty.ColumnSectionHeight / 2
                            - coverDistance
                            - firstMainBarDiam / 2
                            , -columnProperty.ColumnSectionWidth / 2
                            + coverDistance
                            + firstMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4.Id, newPlaсeСolumnRebar_4);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Центральный левый нижний стержень
                        Rebar columnMainRebar_1A = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1A.Id, rotateLine, alphaSecondOverlapping);
                        XYZ newPlaсeСolumnMainRebar_1A = new XYZ(-columnProperty.ColumnSectionHeight / 2 + coverDistance + secondMainBarDiam / 2, -firstLowerRebarOffset, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1A.Id, newPlaсeСolumnMainRebar_1A);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1A.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Центральный левый верхний стержень
                        Rebar columnMainRebar_1B = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesL
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1B.Id, rotateLine, alphaSecondOverlapping);
                        XYZ newPlaсeСolumnMainRebar_1B = new XYZ(-columnProperty.ColumnSectionHeight / 2 + coverDistance + secondMainBarDiam / 2, firstTopRebarOffset, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1B.Id, newPlaсeСolumnMainRebar_1B);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1B.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);


                        //Центральный правый нижний стержень
                        Rebar columnMainRebar_2A = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesL
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2A.Id, rotateLine, -alphaSecondOverlapping);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2A.Id, rotateLine2, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_2A = new XYZ(columnProperty.ColumnSectionHeight / 2 - coverDistance - secondMainBarDiam / 2, -firstLowerRebarOffset, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2A.Id, newPlaсeСolumnRebar_2A);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2A.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Центральный правый верхний стержень
                        Rebar columnMainRebar_2B = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2B.Id, rotateLine, -alphaSecondOverlapping);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2B.Id, rotateLine2, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_2B = new XYZ(columnProperty.ColumnSectionHeight / 2 - coverDistance - secondMainBarDiam / 2, firstTopRebarOffset, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2B.Id, newPlaсeСolumnRebar_2B);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2B.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);


                        //Центральный нижний левый стержень
                        Rebar columnMainRebar_3A = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3A.Id, rotateLine, -alphaSecondOverlapping);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3A.Id, rotateLine2, 90 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_3A = new XYZ(-firstLeftRebarOffset, -columnProperty.ColumnSectionWidth / 2 + coverDistance + secondMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3A.Id, newPlaсeСolumnRebar_3A);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3A.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Центральный нижний правый стержень
                        Rebar columnMainRebar_3B = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesL
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3B.Id, rotateLine, -alphaSecondOverlapping);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3B.Id, rotateLine2, 90 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_3B = new XYZ(firstRightRebarOffset, -columnProperty.ColumnSectionWidth / 2 + coverDistance + secondMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3B.Id, newPlaсeСolumnRebar_3B);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3B.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);


                        //Центральный верхний левый стержень
                        Rebar columnMainRebar_4A = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesL
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4A.Id, rotateLine, alphaSecondOverlapping);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4A.Id, rotateLine2, -90 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_4A = new XYZ(-firstLeftRebarOffset, columnProperty.ColumnSectionWidth / 2 - coverDistance - secondMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4A.Id, newPlaсeСolumnRebar_4A);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4A.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Центральный верхний правый стержень
                        Rebar columnMainRebar_4B = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4B.Id, rotateLine, alphaSecondOverlapping);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4B.Id, rotateLine2, -90 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_4B = new XYZ(firstRightRebarOffset, columnProperty.ColumnSectionWidth / 2 - coverDistance - secondMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4B.Id, newPlaсeСolumnRebar_4B);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4B.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);
                    }

                    //Если стыковка стержней на сварке с изменением сечения колонны выше
                    else if (squareColumnsReinforcementWPF.RebarConnectionOptionName == "radioButton_Mechanical"
                        && squareColumnsReinforcementWPF.OverlapTransitionChecked == false
                        && squareColumnsReinforcementWPF.SectionChangeChecked == true)
                    {
                        XYZ mechanicalConnection_p0L = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), columnProperty.ColumnLength + floorThickness + firstRebarOutletsLength + columnProperty.BaseLevelOffset);
                        XYZ mechanicalConnection_p0S = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), columnProperty.ColumnLength + floorThickness + secondRebarOutletsLength + columnProperty.BaseLevelOffset);

                        //Точки для построения кривых основных удлиненных угловых стержней
                        XYZ rebar_p1L = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), Math.Round(columnProperty.ColumnBasePoint.Z + firstRebarOutletsLength, 6));
                        XYZ rebar_p2L = new XYZ(Math.Round(rebar_p1L.X, 6), Math.Round(rebar_p1L.Y, 6), Math.Round(rebar_p1L.Z + columnProperty.ColumnLength - (sectionOffset * 6 - floorThickness) - firstRebarOutletsLength, 6));
                        XYZ rebar_p3L = new XYZ(Math.Round(rebar_p2L.X + deltaXWelding, 6), Math.Round(rebar_p2L.Y, 6), Math.Round(rebar_p2L.Z + floorThickness + (sectionOffset * 6 - floorThickness), 6));
                        XYZ rebar_p4L = new XYZ(Math.Round(rebar_p3L.X, 6), Math.Round(rebar_p3L.Y, 6), Math.Round(rebar_p3L.Z + firstRebarOutletsLength, 6));

                        //Точки для построения кривых основных укороченных угловых стержней
                        XYZ rebar_p1S = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), Math.Round(columnProperty.ColumnBasePoint.Z + secondRebarOutletsLength, 6));
                        XYZ rebar_p2S = new XYZ(Math.Round(rebar_p1S.X, 6), Math.Round(rebar_p1S.Y, 6), Math.Round(rebar_p1S.Z + columnProperty.ColumnLength - (sectionOffset * 6 - floorThickness) - secondRebarOutletsLength, 6));
                        XYZ rebar_p3S = new XYZ(Math.Round(rebar_p2S.X + deltaXWelding, 6), Math.Round(rebar_p2S.Y, 6), Math.Round(rebar_p2S.Z + floorThickness + (sectionOffset * 6 - floorThickness), 6));
                        XYZ rebar_p4S = new XYZ(Math.Round(rebar_p3S.X, 6), Math.Round(rebar_p3S.Y, 6), Math.Round(rebar_p3S.Z + secondRebarOutletsLength, 6));

                        //Точки для построения кривых основных боковых удлиненных стержней
                        XYZ rebar_p1AL = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), Math.Round(columnProperty.ColumnBasePoint.Z + firstRebarOutletsLength, 6));
                        XYZ rebar_p2AL = new XYZ(Math.Round(rebar_p1AL.X, 6), Math.Round(rebar_p1AL.Y, 6), Math.Round(rebar_p1AL.Z + columnProperty.ColumnLength - (sectionOffset * 6 - floorThickness) - firstRebarOutletsLength, 6));
                        XYZ rebar_p3AL = new XYZ(Math.Round(rebar_p2AL.X + sectionOffset, 6), Math.Round(rebar_p2AL.Y, 6), Math.Round(rebar_p2AL.Z + floorThickness + (sectionOffset * 6 - floorThickness), 6));
                        XYZ rebar_p4AL = new XYZ(Math.Round(rebar_p3AL.X, 6), Math.Round(rebar_p3AL.Y, 6), Math.Round(rebar_p3AL.Z + firstRebarOutletsLength, 6));

                        //Точки для построения кривых основных боковых укороченных стержней
                        XYZ rebar_p1AS = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), Math.Round(columnProperty.ColumnBasePoint.Z + secondRebarOutletsLength, 6));
                        XYZ rebar_p2AS = new XYZ(Math.Round(rebar_p1AS.X, 6), Math.Round(rebar_p1AS.Y, 6), Math.Round(rebar_p1AS.Z + columnProperty.ColumnLength - (sectionOffset * 6 - floorThickness) - secondRebarOutletsLength, 6));
                        XYZ rebar_p3AS = new XYZ(Math.Round(rebar_p2AS.X + sectionOffset, 6), Math.Round(rebar_p2AS.Y, 6), Math.Round(rebar_p2AS.Z + floorThickness + (sectionOffset * 6 - floorThickness), 6));
                        XYZ rebar_p4AS = new XYZ(Math.Round(rebar_p3AS.X, 6), Math.Round(rebar_p3AS.Y, 6), Math.Round(rebar_p3AS.Z + secondRebarOutletsLength, 6));

                        //Кривые основных угловых удлиненных стержней
                        List<Curve> mainFirstRebarCurvesL = new List<Curve>();

                        Curve line1L = Line.CreateBound(rebar_p1L, rebar_p2L) as Curve;
                        mainFirstRebarCurvesL.Add(line1L);
                        Curve line2L = Line.CreateBound(rebar_p2L, rebar_p3L) as Curve;
                        mainFirstRebarCurvesL.Add(line2L);
                        Curve line3L = Line.CreateBound(rebar_p3L, rebar_p4L) as Curve;
                        mainFirstRebarCurvesL.Add(line3L);

                        //Кривые основных угловых укороченных стержней
                        List<Curve> mainFirstRebarCurvesS = new List<Curve>();

                        Curve line1S = Line.CreateBound(rebar_p1S, rebar_p2S) as Curve;
                        mainFirstRebarCurvesS.Add(line1S);
                        Curve line2S = Line.CreateBound(rebar_p2S, rebar_p3S) as Curve;
                        mainFirstRebarCurvesS.Add(line2S);
                        Curve line3S = Line.CreateBound(rebar_p3S, rebar_p4S) as Curve;
                        mainFirstRebarCurvesS.Add(line3S);

                        //Кривые основных боковых удлиненных стержней
                        List<Curve> mainSecondRebarCurvesL = new List<Curve>();

                        Curve line1AL = Line.CreateBound(rebar_p1AL, rebar_p2AL) as Curve;
                        mainSecondRebarCurvesL.Add(line1AL);
                        Curve line2AL = Line.CreateBound(rebar_p2AL, rebar_p3AL) as Curve;
                        mainSecondRebarCurvesL.Add(line2AL);
                        Curve line3AL = Line.CreateBound(rebar_p3AL, rebar_p4AL) as Curve;
                        mainSecondRebarCurvesL.Add(line3AL);

                        //Кривые основных боковых укороченных стержней
                        List<Curve> mainSecondRebarCurvesS = new List<Curve>();

                        Curve line1AS = Line.CreateBound(rebar_p1AS, rebar_p2AS) as Curve;
                        mainSecondRebarCurvesS.Add(line1AS);
                        Curve line2AS = Line.CreateBound(rebar_p2AS, rebar_p3AS) as Curve;
                        mainSecondRebarCurvesS.Add(line2AS);
                        Curve line4AS = Line.CreateBound(rebar_p3AS, rebar_p4AS) as Curve;
                        mainSecondRebarCurvesS.Add(line4AS);

                        //Нижний левый угол
                        Rebar columnMainRebar_1 = null;
                        try
                        {
                            columnMainRebar_1 = Rebar.CreateFromCurvesAndShape(doc
                                , form26
                                , firstMainBarTape
                                , null
                                , null
                                , column
                                , new XYZ(0, 1, 0)
                                , mainFirstRebarCurvesL
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);
                        }
                        catch
                        {
                            TaskDialog.Show("Revit", "Не удалось создать Z-образный стержень! Возможно выбран некорректный тип формы 26!");
                            return Result.Cancelled;
                        }

                        if (columnMainRebar_1 == null)
                        {
                            TaskDialog.Show("Revit", "Не удалось создать Z-образный стержень! Возможно выбран некорректный тип формы 26!");
                            return Result.Cancelled;
                        }

                        XYZ rotate1_p1 = new XYZ(rebar_p1L.X, rebar_p1L.Y, rebar_p1L.Z);
                        XYZ rotate1_p2 = new XYZ(rebar_p1L.X, rebar_p1L.Y, rebar_p1L.Z + 1);
                        Line rotateLine = Line.CreateBound(rotate1_p1, rotate1_p2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1.Id, rotateLine, alphaWelding);
                        XYZ newPlaсeСolumnMainRebar_1 = new XYZ(-columnProperty.ColumnSectionHeight / 2
                            + coverDistance
                            + firstMainBarDiam / 2
                            , -columnProperty.ColumnSectionWidth / 2
                            + coverDistance
                            + firstMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1.Id, newPlaсeСolumnMainRebar_1);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        FamilyInstance mechanicalConnection_1 = doc.Create.NewFamilyInstance(mechanicalConnection_p0L
                            , firstMechanicalConnectionFamilySymbol
                            , columnProperty.BaseLevel
                            , StructuralType.NonStructural);
                        XYZ newPlaсemechanicalConnection_1 = new XYZ(-columnProperty.ColumnSectionHeight / 2
                            + coverDistance
                            + firstMainBarDiam / 2
                            + sectionOffset
                            , -columnProperty.ColumnSectionWidth / 2
                            + coverDistance
                            + firstMainBarDiam / 2
                            + sectionOffset, 0);
                        ElementTransformUtils.MoveElement(doc, mechanicalConnection_1.Id, newPlaсemechanicalConnection_1);
                        ElementTransformUtils.RotateElement(doc, mechanicalConnection_1.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Верхний левый угол
                        Rebar columnMainRebar_2 = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , firstMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainFirstRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2.Id, rotateLine, -alphaWelding);
                        XYZ newPlaсeСolumnMainRebar_2 = new XYZ(-columnProperty.ColumnSectionHeight / 2 + coverDistance + firstMainBarDiam / 2, columnProperty.ColumnSectionWidth / 2 - coverDistance - firstMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2.Id, newPlaсeСolumnMainRebar_2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        FamilyInstance mechanicalConnection_2 = doc.Create.NewFamilyInstance(mechanicalConnection_p0S
                            , firstMechanicalConnectionFamilySymbol
                            , columnProperty.BaseLevel
                            , StructuralType.NonStructural);
                        XYZ newPlaсemechanicalConnection_2 = new XYZ(-columnProperty.ColumnSectionHeight / 2
                            + coverDistance
                            + firstMainBarDiam / 2
                            + sectionOffset
                            , columnProperty.ColumnSectionWidth / 2
                            - coverDistance
                            - firstMainBarDiam / 2
                            - sectionOffset, 0);
                        ElementTransformUtils.MoveElement(doc, mechanicalConnection_2.Id, newPlaсemechanicalConnection_2);
                        ElementTransformUtils.RotateElement(doc, mechanicalConnection_2.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Верхний правый угол
                        Rebar columnMainRebar_3 = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , firstMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainFirstRebarCurvesL
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3.Id, rotateLine, alphaWelding);
                        XYZ rotate2_p1 = new XYZ(rebar_p1L.X, rebar_p1L.Y, rebar_p1L.Z);
                        XYZ rotate2_p2 = new XYZ(rebar_p1L.X, rebar_p1L.Y, rebar_p1L.Z + 1);
                        Line rotateLine2 = Line.CreateBound(rotate2_p1, rotate2_p2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3.Id, rotateLine2, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnMainRebar_3 = new XYZ(columnProperty.ColumnSectionHeight / 2
                            - coverDistance
                            - firstMainBarDiam / 2
                            , columnProperty.ColumnSectionWidth / 2
                            - coverDistance
                            - firstMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3.Id, newPlaсeСolumnMainRebar_3);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        FamilyInstance mechanicalConnection_3 = doc.Create.NewFamilyInstance(mechanicalConnection_p0L
                            , firstMechanicalConnectionFamilySymbol
                            , columnProperty.BaseLevel
                            , StructuralType.NonStructural);
                        XYZ newPlaсemechanicalConnection_3 = new XYZ(columnProperty.ColumnSectionHeight / 2
                            - coverDistance
                            - firstMainBarDiam / 2
                            - sectionOffset
                            , columnProperty.ColumnSectionWidth / 2
                            - coverDistance
                            - firstMainBarDiam / 2
                            - sectionOffset, 0);
                        ElementTransformUtils.MoveElement(doc, mechanicalConnection_3.Id, newPlaсemechanicalConnection_3);
                        ElementTransformUtils.RotateElement(doc, mechanicalConnection_3.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Нижний правый угол
                        Rebar columnMainRebar_4 = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , firstMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainFirstRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4.Id, rotateLine, -alphaWelding);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4.Id, rotateLine2, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnMainRebar_4 = new XYZ(columnProperty.ColumnSectionHeight / 2
                            - coverDistance
                            - firstMainBarDiam / 2
                            , -columnProperty.ColumnSectionWidth / 2
                            + coverDistance
                            + firstMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4.Id, newPlaсeСolumnMainRebar_4);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        FamilyInstance mechanicalConnection_4 = doc.Create.NewFamilyInstance(mechanicalConnection_p0S
                            , firstMechanicalConnectionFamilySymbol
                            , columnProperty.BaseLevel
                            , StructuralType.NonStructural);
                        XYZ newPlaсemechanicalConnection_4 = new XYZ(columnProperty.ColumnSectionHeight / 2 - coverDistance - firstMainBarDiam / 2 - sectionOffset, -columnProperty.ColumnSectionWidth / 2 + coverDistance + firstMainBarDiam / 2 + sectionOffset, 0);
                        ElementTransformUtils.MoveElement(doc, mechanicalConnection_4.Id, newPlaсemechanicalConnection_4);
                        ElementTransformUtils.RotateElement(doc, mechanicalConnection_4.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);


                        //Центральный левый нижний стержень
                        Rebar columnMainRebar_1A = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebar_1A = new XYZ(-columnProperty.ColumnSectionHeight / 2 + coverDistance + secondMainBarDiam / 2, -firstLowerRebarOffset, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1A.Id, newPlaсeСolumnMainRebar_1A);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1A.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        FamilyInstance mechanicalConnection_1A = doc.Create.NewFamilyInstance(mechanicalConnection_p0S
                            , secondMechanicalConnectionFamilySymbol
                            , columnProperty.BaseLevel
                            , StructuralType.NonStructural);
                        XYZ newPlaсemechanicalConnection_1A = new XYZ(-columnProperty.ColumnSectionHeight / 2 + coverDistance + secondMainBarDiam / 2 + sectionOffset, -firstLowerRebarOffset, 0);
                        ElementTransformUtils.MoveElement(doc, mechanicalConnection_1A.Id, newPlaсemechanicalConnection_1A);
                        ElementTransformUtils.RotateElement(doc, mechanicalConnection_1A.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Центральный левый верхний стержень
                        Rebar columnMainRebar_1B = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesL
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        XYZ newPlaсeСolumnMainRebar_1B = new XYZ(-columnProperty.ColumnSectionHeight / 2 + coverDistance + secondMainBarDiam / 2, firstTopRebarOffset, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1B.Id, newPlaсeСolumnMainRebar_1B);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1B.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        FamilyInstance mechanicalConnection_1B = doc.Create.NewFamilyInstance(mechanicalConnection_p0L
                            , secondMechanicalConnectionFamilySymbol
                            , columnProperty.BaseLevel
                            , StructuralType.NonStructural);
                        XYZ newPlaсemechanicalConnection_1B = new XYZ(-columnProperty.ColumnSectionHeight / 2 + coverDistance + secondMainBarDiam / 2 + sectionOffset, firstTopRebarOffset, 0);
                        ElementTransformUtils.MoveElement(doc, mechanicalConnection_1B.Id, newPlaсemechanicalConnection_1B);
                        ElementTransformUtils.RotateElement(doc, mechanicalConnection_1B.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);


                        //Центральный правый нижний стержень
                        Rebar columnMainRebar_2A = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesL
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2A.Id, rotateLine2, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_2A = new XYZ(columnProperty.ColumnSectionHeight / 2 - coverDistance - secondMainBarDiam / 2, -firstLowerRebarOffset, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2A.Id, newPlaсeСolumnRebar_2A);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2A.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        FamilyInstance mechanicalConnection_2A = doc.Create.NewFamilyInstance(mechanicalConnection_p0L
                            , secondMechanicalConnectionFamilySymbol
                            , columnProperty.BaseLevel
                            , StructuralType.NonStructural);
                        XYZ newPlaсemechanicalConnection_2A = new XYZ(columnProperty.ColumnSectionHeight / 2 - coverDistance - secondMainBarDiam / 2 - sectionOffset, -firstLowerRebarOffset, 0);
                        ElementTransformUtils.MoveElement(doc, mechanicalConnection_2A.Id, newPlaсemechanicalConnection_2A);
                        ElementTransformUtils.RotateElement(doc, mechanicalConnection_2A.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Центральный правый верхний стержень
                        Rebar columnMainRebar_2B = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2B.Id, rotateLine2, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_2B = new XYZ(columnProperty.ColumnSectionHeight / 2 - coverDistance - secondMainBarDiam / 2, firstTopRebarOffset, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2B.Id, newPlaсeСolumnRebar_2B);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2B.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        FamilyInstance mechanicalConnection_2B = doc.Create.NewFamilyInstance(mechanicalConnection_p0S
                            , secondMechanicalConnectionFamilySymbol
                            , columnProperty.BaseLevel
                            , StructuralType.NonStructural);
                        XYZ newPlaсemechanicalConnection_2B = new XYZ(columnProperty.ColumnSectionHeight / 2 - coverDistance - secondMainBarDiam / 2 - sectionOffset, firstTopRebarOffset, 0);
                        ElementTransformUtils.MoveElement(doc, mechanicalConnection_2B.Id, newPlaсemechanicalConnection_2B);
                        ElementTransformUtils.RotateElement(doc, mechanicalConnection_2B.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Центральный нижний левый стержень
                        Rebar columnMainRebar_3A = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3A.Id, rotateLine2, 90 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_3A = new XYZ(-firstLeftRebarOffset, -columnProperty.ColumnSectionWidth / 2 + coverDistance + secondMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3A.Id, newPlaсeСolumnRebar_3A);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3A.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        FamilyInstance mechanicalConnection_3A = doc.Create.NewFamilyInstance(mechanicalConnection_p0S
                            , secondMechanicalConnectionFamilySymbol
                            , columnProperty.BaseLevel
                            , StructuralType.NonStructural);
                        XYZ newPlaсemechanicalConnection_3A = new XYZ(-firstLeftRebarOffset, -columnProperty.ColumnSectionWidth / 2 + coverDistance + secondMainBarDiam / 2 + sectionOffset, 0);
                        ElementTransformUtils.MoveElement(doc, mechanicalConnection_3A.Id, newPlaсemechanicalConnection_3A);
                        ElementTransformUtils.RotateElement(doc, mechanicalConnection_3A.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Центральный нижний правый стержень
                        Rebar columnMainRebar_3B = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesL
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3B.Id, rotateLine2, 90 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_3B = new XYZ(firstRightRebarOffset, -columnProperty.ColumnSectionWidth / 2 + coverDistance + secondMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3B.Id, newPlaсeСolumnRebar_3B);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3B.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        FamilyInstance mechanicalConnection_3B = doc.Create.NewFamilyInstance(mechanicalConnection_p0L
                            , secondMechanicalConnectionFamilySymbol
                            , columnProperty.BaseLevel
                            , StructuralType.NonStructural);
                        XYZ newPlaсemechanicalConnection_3B = new XYZ(firstRightRebarOffset, -columnProperty.ColumnSectionWidth / 2 + coverDistance + secondMainBarDiam / 2 + sectionOffset, 0);
                        ElementTransformUtils.MoveElement(doc, mechanicalConnection_3B.Id, newPlaсemechanicalConnection_3B);
                        ElementTransformUtils.RotateElement(doc, mechanicalConnection_3B.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);


                        //Центральный верхний левый стержень
                        Rebar columnMainRebar_4A = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesL
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4A.Id, rotateLine2, -90 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_4A = new XYZ(-firstLeftRebarOffset, columnProperty.ColumnSectionWidth / 2 - coverDistance - secondMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4A.Id, newPlaсeСolumnRebar_4A);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4A.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        FamilyInstance mechanicalConnection_4A = doc.Create.NewFamilyInstance(mechanicalConnection_p0L
                            , secondMechanicalConnectionFamilySymbol
                            , columnProperty.BaseLevel
                            , StructuralType.NonStructural);
                        XYZ newPlaсemechanicalConnection_4A = new XYZ(-firstLeftRebarOffset, columnProperty.ColumnSectionWidth / 2 - coverDistance - secondMainBarDiam / 2 - sectionOffset, 0);
                        ElementTransformUtils.MoveElement(doc, mechanicalConnection_4A.Id, newPlaсemechanicalConnection_4A);
                        ElementTransformUtils.RotateElement(doc, mechanicalConnection_4A.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Центральный верхний стержень
                        Rebar columnMainRebar_4B = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4B.Id, rotateLine2, -90 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_4B = new XYZ(firstRightRebarOffset, columnProperty.ColumnSectionWidth / 2 - coverDistance - secondMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4B.Id, newPlaсeСolumnRebar_4B);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4B.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        FamilyInstance mechanicalConnection_4B = doc.Create.NewFamilyInstance(mechanicalConnection_p0S
                            , secondMechanicalConnectionFamilySymbol
                            , columnProperty.BaseLevel
                            , StructuralType.NonStructural);
                        XYZ newPlaсemechanicalConnection_4B = new XYZ(firstRightRebarOffset, columnProperty.ColumnSectionWidth / 2 - coverDistance - secondMainBarDiam / 2 - sectionOffset, 0);
                        ElementTransformUtils.MoveElement(doc, mechanicalConnection_4B.Id, newPlaсemechanicalConnection_4B);
                        ElementTransformUtils.RotateElement(doc, mechanicalConnection_4B.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);
                    }

                    //Если переход со сварки в нахлест c изменением сечения колонны выше
                    else if (squareColumnsReinforcementWPF.RebarConnectionOptionName == "radioButton_Mechanical"
                        && squareColumnsReinforcementWPF.OverlapTransitionChecked == true
                        && squareColumnsReinforcementWPF.SectionChangeChecked == true)
                    {

                        //Точки для построения кривых основных угловых удлиненных стержней
                        XYZ rebar_p1L = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), Math.Round(columnProperty.ColumnBasePoint.Z + firstRebarOutletsLength, 6));
                        XYZ rebar_p2L = new XYZ(Math.Round(rebar_p1L.X, 6), Math.Round(rebar_p1L.Y, 6), Math.Round(rebar_p1L.Z + columnProperty.ColumnLength - (sectionOffset * 6 - floorThickness) - firstRebarOutletsLength, 6));
                        XYZ rebar_p3L = new XYZ(Math.Round(rebar_p2L.X + deltaXOverlapping, 6), Math.Round(rebar_p2L.Y, 6), Math.Round(rebar_p2L.Z + floorThickness + (sectionOffset * 6 - floorThickness), 6));
                        XYZ rebar_p4L = new XYZ(Math.Round(rebar_p3L.X, 6), Math.Round(rebar_p3L.Y, 6), Math.Round(rebar_p3L.Z + firstRebarOutletsLength, 6));

                        //Точки для построения кривых основных угловых укороченных стержней
                        XYZ rebar_p1S = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), Math.Round(columnProperty.ColumnBasePoint.Z + secondRebarOutletsLength, 6));
                        XYZ rebar_p2S = new XYZ(Math.Round(rebar_p1S.X, 6), Math.Round(rebar_p1S.Y, 6), Math.Round(rebar_p1S.Z + columnProperty.ColumnLength - (sectionOffset * 6 - floorThickness) - secondRebarOutletsLength, 6));
                        XYZ rebar_p3S = new XYZ(Math.Round(rebar_p2S.X + deltaXOverlapping, 6), Math.Round(rebar_p2S.Y, 6), Math.Round(rebar_p2S.Z + floorThickness + (sectionOffset * 6 - floorThickness), 6));
                        XYZ rebar_p4S = new XYZ(Math.Round(rebar_p3S.X, 6), Math.Round(rebar_p3S.Y, 6), Math.Round(rebar_p3S.Z + secondRebarOutletsLength, 6));

                        //Точки для построения кривых основных боковых удлиненных стержней
                        XYZ rebar_p1AL = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), Math.Round(columnProperty.ColumnBasePoint.Z + firstRebarOutletsLength, 6));
                        XYZ rebar_p2AL = new XYZ(Math.Round(rebar_p1AL.X, 6), Math.Round(rebar_p1AL.Y, 6), Math.Round(rebar_p1AL.Z + columnProperty.ColumnLength - (sectionOffset * 6 - floorThickness) - firstRebarOutletsLength, 6));
                        XYZ rebar_p3AL = new XYZ(Math.Round(rebar_p2AL.X + deltaXSecondOverlapping, 6), Math.Round(rebar_p2AL.Y, 6), Math.Round(rebar_p2AL.Z + floorThickness + (sectionOffset * 6 - floorThickness), 6));
                        XYZ rebar_p4AL = new XYZ(Math.Round(rebar_p3AL.X, 6), Math.Round(rebar_p3AL.Y, 6), Math.Round(rebar_p3AL.Z + firstRebarOutletsLength, 6));

                        //Точки для построения кривых основных боковых укороченных стержней
                        XYZ rebar_p1AS = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), Math.Round(columnProperty.ColumnBasePoint.Z + secondRebarOutletsLength, 6));
                        XYZ rebar_p2AS = new XYZ(Math.Round(rebar_p1AS.X, 6), Math.Round(rebar_p1AS.Y, 6), Math.Round(rebar_p1AS.Z + columnProperty.ColumnLength - (sectionOffset * 6 - floorThickness) - secondRebarOutletsLength, 6));
                        XYZ rebar_p3AS = new XYZ(Math.Round(rebar_p2AS.X + deltaXSecondOverlapping, 6), Math.Round(rebar_p2AS.Y, 6), Math.Round(rebar_p2AS.Z + floorThickness + (sectionOffset * 6 - floorThickness), 6));
                        XYZ rebar_p4AS = new XYZ(Math.Round(rebar_p3AS.X, 6), Math.Round(rebar_p3AS.Y, 6), Math.Round(rebar_p3AS.Z + secondRebarOutletsLength, 6));


                        //Кривые основных угловых удлиненных стержней
                        List<Curve> mainFirstRebarCurvesL = new List<Curve>();

                        Curve line1L = Line.CreateBound(rebar_p1L, rebar_p2L) as Curve;
                        mainFirstRebarCurvesL.Add(line1L);
                        Curve line2L = Line.CreateBound(rebar_p2L, rebar_p3L) as Curve;
                        mainFirstRebarCurvesL.Add(line2L);
                        Curve line3L = Line.CreateBound(rebar_p3L, rebar_p4L) as Curve;
                        mainFirstRebarCurvesL.Add(line3L);

                        //Кривые основных угловых укороченных стержней
                        List<Curve> mainFirstRebarCurvesS = new List<Curve>();

                        Curve line1S = Line.CreateBound(rebar_p1S, rebar_p2S) as Curve;
                        mainFirstRebarCurvesS.Add(line1S);
                        Curve line2S = Line.CreateBound(rebar_p2S, rebar_p3S) as Curve;
                        mainFirstRebarCurvesS.Add(line2S);
                        Curve line3S = Line.CreateBound(rebar_p3S, rebar_p4S) as Curve;
                        mainFirstRebarCurvesS.Add(line3S);

                        //Кривые основных боковых удлиненных стержней
                        List<Curve> mainSecondRebarCurvesL = new List<Curve>();

                        Curve line1AL = Line.CreateBound(rebar_p1AL, rebar_p2AL) as Curve;
                        mainSecondRebarCurvesL.Add(line1AL);
                        Curve line2AL = Line.CreateBound(rebar_p2AL, rebar_p3AL) as Curve;
                        mainSecondRebarCurvesL.Add(line2AL);
                        Curve line3AL = Line.CreateBound(rebar_p3AL, rebar_p4AL) as Curve;
                        mainSecondRebarCurvesL.Add(line3AL);

                        //Кривые основных боковых укороченных стержней
                        List<Curve> mainSecondRebarCurvesS = new List<Curve>();

                        Curve line1AS = Line.CreateBound(rebar_p1AS, rebar_p2AS) as Curve;
                        mainSecondRebarCurvesS.Add(line1AS);
                        Curve line2AS = Line.CreateBound(rebar_p2AS, rebar_p3AS) as Curve;
                        mainSecondRebarCurvesS.Add(line2AS);
                        Curve line3AS = Line.CreateBound(rebar_p3AS, rebar_p4AS) as Curve;
                        mainSecondRebarCurvesS.Add(line3AS);

                        //Нижний левый угол
                        Rebar columnMainRebar_1 = null;
                        try
                        {
                            columnMainRebar_1 = Rebar.CreateFromCurvesAndShape(doc
                                , form26
                                , firstMainBarTape
                                , null
                                , null
                                , column
                                , new XYZ(0, 1, 0)
                                , mainFirstRebarCurvesL
                                , RebarHookOrientation.Right
                                , RebarHookOrientation.Right);
                        }
                        catch
                        {
                            TaskDialog.Show("Revit", "Не удалось создать Z-образный стержень! Возможно выбран некорректный тип формы 26!");
                            return Result.Cancelled;
                        }

                        if (columnMainRebar_1 == null)
                        {
                            TaskDialog.Show("Revit", "Не удалось создать Z-образный стержень! Возможно выбран некорректный тип формы 26!");
                            return Result.Cancelled;
                        }

                        XYZ rotate1_p1 = new XYZ(rebar_p1L.X, rebar_p1L.Y, rebar_p1L.Z);
                        XYZ rotate1_p2 = new XYZ(rebar_p1L.X, rebar_p1L.Y, rebar_p1L.Z + 1);
                        Line rotateLine = Line.CreateBound(rotate1_p1, rotate1_p2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1.Id, rotateLine, alphaOverlapping);
                        XYZ newPlaсeСolumnMainRebar_1 = new XYZ(-columnProperty.ColumnSectionHeight / 2
                            + coverDistance
                            + firstMainBarDiam / 2
                            , -columnProperty.ColumnSectionWidth / 2
                            + coverDistance
                            + firstMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1.Id, newPlaсeСolumnMainRebar_1);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Верхний левый угол
                        Rebar columnMainRebar_2 = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , firstMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainFirstRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2.Id, rotateLine, -alphaOverlapping);
                        XYZ newPlaсeСolumnRebar_2 = new XYZ(-columnProperty.ColumnSectionHeight / 2
                            + coverDistance
                            + firstMainBarDiam / 2
                            , columnProperty.ColumnSectionWidth / 2
                            - coverDistance
                            - firstMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2.Id, newPlaсeСolumnRebar_2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Верхний правый угол
                        Rebar columnMainRebar_3 = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , firstMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainFirstRebarCurvesL
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3.Id, rotateLine, alphaOverlapping);
                        XYZ rotate2_p1 = new XYZ(rebar_p1L.X, rebar_p1L.Y, rebar_p1L.Z);
                        XYZ rotate2_p2 = new XYZ(rebar_p1L.X, rebar_p1L.Y, rebar_p1L.Z + 1);
                        Line rotateLine2 = Line.CreateBound(rotate2_p1, rotate2_p2);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3.Id, rotateLine2, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_3 = new XYZ(columnProperty.ColumnSectionHeight / 2
                            - coverDistance
                            - firstMainBarDiam / 2
                            , columnProperty.ColumnSectionWidth / 2
                            - coverDistance
                            - firstMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3.Id, newPlaсeСolumnRebar_3);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Нижний правый угол
                        Rebar columnMainRebar_4 = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , firstMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainFirstRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4.Id, rotateLine, -alphaOverlapping);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4.Id, rotateLine2, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_4 = new XYZ(columnProperty.ColumnSectionHeight / 2
                            - coverDistance
                            - firstMainBarDiam / 2
                            , -columnProperty.ColumnSectionWidth / 2
                            + coverDistance
                            + firstMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4.Id, newPlaсeСolumnRebar_4);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Центральный левый нижний стержень
                        Rebar columnMainRebar_1A = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1A.Id, rotateLine, alphaSecondOverlapping);
                        XYZ newPlaсeСolumnMainRebar_1A = new XYZ(-columnProperty.ColumnSectionHeight / 2 + coverDistance + secondMainBarDiam / 2, -firstLowerRebarOffset, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1A.Id, newPlaсeСolumnMainRebar_1A);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1A.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Центральный левый верхний стержень
                        Rebar columnMainRebar_1B = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesL
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1B.Id, rotateLine, alphaSecondOverlapping);
                        XYZ newPlaсeСolumnMainRebar_1B = new XYZ(-columnProperty.ColumnSectionHeight / 2 + coverDistance + secondMainBarDiam / 2, firstTopRebarOffset, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_1B.Id, newPlaсeСolumnMainRebar_1B);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_1B.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Центральный правый нижний стержень
                        Rebar columnMainRebar_2A = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesL
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2A.Id, rotateLine, -alphaSecondOverlapping);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2A.Id, rotateLine2, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_2A = new XYZ(columnProperty.ColumnSectionHeight / 2 - coverDistance - secondMainBarDiam / 2, -firstLowerRebarOffset, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2A.Id, newPlaсeСolumnRebar_2A);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2A.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Центральный правый верхний стержень
                        Rebar columnMainRebar_2B = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2B.Id, rotateLine, -alphaSecondOverlapping);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2B.Id, rotateLine2, 180 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_2B = new XYZ(columnProperty.ColumnSectionHeight / 2 - coverDistance - secondMainBarDiam / 2, firstTopRebarOffset, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_2B.Id, newPlaсeСolumnRebar_2B);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_2B.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Центральный нижний левый стержень
                        Rebar columnMainRebar_3A = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3A.Id, rotateLine, -alphaSecondOverlapping);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3A.Id, rotateLine2, 90 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_3A = new XYZ(-firstLeftRebarOffset, -columnProperty.ColumnSectionWidth / 2 + coverDistance + secondMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3A.Id, newPlaсeСolumnRebar_3A);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3A.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Центральный нижний правый стержень
                        Rebar columnMainRebar_3B = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesL
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3B.Id, rotateLine, -alphaSecondOverlapping);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3B.Id, rotateLine2, 90 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_3B = new XYZ(firstRightRebarOffset, -columnProperty.ColumnSectionWidth / 2 + coverDistance + secondMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_3B.Id, newPlaсeСolumnRebar_3B);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_3B.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Центральный верхний левый стержень
                        Rebar columnMainRebar_4A = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesL
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4A.Id, rotateLine, alphaSecondOverlapping);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4A.Id, rotateLine2, -90 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_4A = new XYZ(-firstLeftRebarOffset, columnProperty.ColumnSectionWidth / 2 - coverDistance - secondMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4A.Id, newPlaсeСolumnRebar_4A);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4A.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Центральный верхний правый стержень
                        Rebar columnMainRebar_4B = Rebar.CreateFromCurvesAndShape(doc
                            , form26
                            , secondMainBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , mainSecondRebarCurvesS
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4B.Id, rotateLine, alphaSecondOverlapping);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4B.Id, rotateLine2, -90 * (Math.PI / 180));
                        XYZ newPlaсeСolumnRebar_4B = new XYZ(firstRightRebarOffset, columnProperty.ColumnSectionWidth / 2 - coverDistance - secondMainBarDiam / 2, 0);
                        ElementTransformUtils.MoveElement(doc, columnMainRebar_4B.Id, newPlaсeСolumnRebar_4B);
                        ElementTransformUtils.RotateElement(doc, columnMainRebar_4B.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);
                    }

                    //Хомуты
                    //Точки для построения кривых стержня хомута
                    XYZ firstStirrup_p1 = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X - columnProperty.ColumnSectionHeight / 2 + coverDistance - firstStirrupBarDiam, 6)
                        , Math.Round(columnProperty.ColumnBasePoint.Y + columnProperty.ColumnSectionWidth / 2 - coverDistance + firstStirrupBarDiam / 2, 6)
                        , Math.Round(columnProperty.ColumnBasePoint.Z + firstStirrupButtomOffset, 6));

                    XYZ firstStirrup_p2 = new XYZ(Math.Round(firstStirrup_p1.X + columnProperty.ColumnSectionHeight - coverDistance * 2 + firstStirrupBarDiam, 6)
                        , Math.Round(firstStirrup_p1.Y, 6)
                        , Math.Round(firstStirrup_p1.Z, 6));

                    XYZ firstStirrup_p3 = new XYZ(Math.Round(firstStirrup_p2.X, 6)
                        , Math.Round(firstStirrup_p2.Y - columnProperty.ColumnSectionWidth + coverDistance * 2 - firstStirrupBarDiam - firstStirrupBarDiam / 2, 6)
                        , Math.Round(firstStirrup_p2.Z, 6));

                    XYZ firstStirrup_p4 = new XYZ(Math.Round(firstStirrup_p3.X - columnProperty.ColumnSectionHeight + coverDistance * 2 - firstStirrupBarDiam, 6)
                        , Math.Round(firstStirrup_p3.Y, 6)
                        , Math.Round(firstStirrup_p3.Z, 6));

                    //Кривые хомута
                    List<Curve> firstStirrupCurves = new List<Curve>();

                    Curve firstStirrup_line1 = Line.CreateBound(firstStirrup_p1, firstStirrup_p2) as Curve;
                    firstStirrupCurves.Add(firstStirrup_line1);
                    Curve firstStirrup_line2 = Line.CreateBound(firstStirrup_p2, firstStirrup_p3) as Curve;
                    firstStirrupCurves.Add(firstStirrup_line2);
                    Curve firstStirrup_line3 = Line.CreateBound(firstStirrup_p3, firstStirrup_p4) as Curve;
                    firstStirrupCurves.Add(firstStirrup_line3);
                    Curve firstStirrup_line4 = Line.CreateBound(firstStirrup_p4, firstStirrup_p1) as Curve;
                    firstStirrupCurves.Add(firstStirrup_line4);

                    //Построение нижнего хомута
                    Rebar buttomStirrup = null;
                    try
                    {
                        buttomStirrup = Rebar.CreateFromCurvesAndShape(doc
                            , form51
                            , firstStirrupBarTape
                            , rebarHookTypeForStirrup
                            , rebarHookTypeForStirrup
                            , column
                            , new XYZ(0, 0, 1)
                            , firstStirrupCurves
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);
                    }
                    catch
                    {
                        TaskDialog.Show("Revit", "Не удалось создать хомут! Возможно выбран некорректный тип формы 51 или отгиб арматуры не соответствует хомуту!");
                        return Result.Cancelled;
                    }

                    if (buttomStirrup == null)
                    {
                        TaskDialog.Show("Revit", "Не удалось создать хомут! Возможно выбран некорректный тип формы 51 или отгиб арматуры не соответствует хомуту!");
                        return Result.Cancelled;
                    }

                    ElementTransformUtils.RotateElement(doc, buttomStirrup.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                    int buttomStirrupQuantity = (int)(frequentButtomStirrupPlacementHeight / frequentButtomStirrupStep) + 1;
                    buttomStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                    buttomStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(buttomStirrupQuantity + 1);
                    buttomStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(frequentButtomStirrupStep);

                    //Копирование хомута
                    XYZ mediumStirrupInstallationPoint = new XYZ(0, 0, frequentButtomStirrupPlacementHeight + standardStirrupStep);
                    List<ElementId> mediumStirrupIdList = ElementTransformUtils.CopyElement(doc, buttomStirrup.Id, mediumStirrupInstallationPoint) as List<ElementId>;
                    Rebar mediumStirrup = doc.GetElement(mediumStirrupIdList.First()) as Rebar;

                    //Высота размещения хомутов со стандартным шагом
                    double mediumStirrupPlacementHeight = columnProperty.ColumnLength - frequentButtomStirrupPlacementHeight - frequentTopStirrupPlacementHeight - firstStirrupButtomOffset - 50 / 304.8;
                    int mediumStirrupQuantity = (int)(mediumStirrupPlacementHeight / standardStirrupStep);
                    mediumStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                    mediumStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(mediumStirrupQuantity);
                    mediumStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(standardStirrupStep);

                    //Копирование хомута последний
                    double topStirrupPlacementHeight = columnProperty.ColumnLength - firstStirrupButtomOffset - 50 / 304.8;
                    int topStirrupQuantity = 0;
                    if ((int)(frequentTopStirrupPlacementHeight / frequentTopStirrupStep) == 1)
                    {
                        topStirrupQuantity = 1;
                    }
                    else
                    {
                        topStirrupQuantity = (int)(frequentTopStirrupPlacementHeight / frequentTopStirrupStep) + 1;
                    }

                    XYZ topStirrupInstallationPoint = new XYZ(0, 0, topStirrupPlacementHeight);
                    List<ElementId> topStirrupIdList = ElementTransformUtils.CopyElement(doc, buttomStirrup.Id, topStirrupInstallationPoint) as List<ElementId>;
                    Rebar topStirrup = doc.GetElement(topStirrupIdList.First()) as Rebar;
                    topStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                    topStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(topStirrupQuantity);
                    topStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(frequentTopStirrupStep);
                    topStirrup.GetShapeDrivenAccessor().BarsOnNormalSide = false;


                    //Точки для построения кривых стержня хомута дополнительного
                    XYZ secondStirrup_p1 = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X - firstLeftRebarOffset - secondMainBarDiam / 2 - secondStirrupBarDiam, 6)
                        , Math.Round(columnProperty.ColumnBasePoint.Y + columnProperty.ColumnSectionWidth / 2 - coverDistance + secondStirrupBarDiam / 2, 6)
                        , Math.Round(columnProperty.ColumnBasePoint.Z + firstStirrupButtomOffset + firstStirrupBarDiam / 2 + secondStirrupBarDiam / 2, 6));

                    XYZ secondStirrup_p2 = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X + firstRightRebarOffset + secondMainBarDiam / 2, 6)
                        , Math.Round(columnProperty.ColumnBasePoint.Y + columnProperty.ColumnSectionWidth / 2 - coverDistance + secondStirrupBarDiam / 2, 6)
                        , Math.Round(secondStirrup_p1.Z, 6));

                    XYZ secondStirrup_p3 = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X + firstRightRebarOffset + secondMainBarDiam / 2, 6)
                        , Math.Round(columnProperty.ColumnBasePoint.Y - columnProperty.ColumnSectionWidth / 2 + coverDistance - secondStirrupBarDiam, 6)
                        , Math.Round(secondStirrup_p2.Z, 6));

                    XYZ secondStirrup_p4 = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X - firstLeftRebarOffset - secondMainBarDiam / 2 - secondStirrupBarDiam, 6)
                        , Math.Round(columnProperty.ColumnBasePoint.Y - columnProperty.ColumnSectionWidth / 2 + coverDistance - secondStirrupBarDiam, 6)
                        , Math.Round(secondStirrup_p3.Z, 6));


                    //Точки для построения кривых стержня хомута дополнительного 90 град
                    XYZ thirdStirrup_p1 = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X - columnProperty.ColumnSectionHeight / 2 + coverDistance - secondStirrupBarDiam / 2, 6)
                        , Math.Round(columnProperty.ColumnBasePoint.Y - firstLowerRebarOffset - secondMainBarDiam / 2 - secondStirrupBarDiam, 6)
                        , Math.Round(columnProperty.ColumnBasePoint.Z + firstStirrupButtomOffset + firstStirrupBarDiam / 2 + secondStirrupBarDiam / 2 + secondStirrupBarDiam, 6));

                    XYZ thirdStirrup_p2 = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X - columnProperty.ColumnSectionHeight / 2 + coverDistance - secondStirrupBarDiam / 2, 6)
                        , Math.Round(columnProperty.ColumnBasePoint.Y + firstTopRebarOffset + secondMainBarDiam / 2, 6)
                        , Math.Round(thirdStirrup_p1.Z, 6));

                    XYZ thirdStirrup_p3 = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X + columnProperty.ColumnSectionHeight / 2 - coverDistance + secondStirrupBarDiam, 6)
                        , Math.Round(columnProperty.ColumnBasePoint.Y + firstTopRebarOffset + secondMainBarDiam / 2, 6)
                        , Math.Round(thirdStirrup_p2.Z, 6));

                    XYZ thirdStirrup_p4 = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X + columnProperty.ColumnSectionHeight / 2 - coverDistance + secondStirrupBarDiam, 6)
                        , Math.Round(columnProperty.ColumnBasePoint.Y - firstLowerRebarOffset - secondMainBarDiam / 2 - secondStirrupBarDiam, 6)
                        , Math.Round(thirdStirrup_p3.Z, 6));

                    //Кривые хомута дополнительного
                    List<Curve> secondStirrupCurves = new List<Curve>();

                    Curve secondStirrup_line1 = Line.CreateBound(secondStirrup_p1, secondStirrup_p2) as Curve;
                    secondStirrupCurves.Add(secondStirrup_line1);
                    Curve secondStirrup_line2 = Line.CreateBound(secondStirrup_p2, secondStirrup_p3) as Curve;
                    secondStirrupCurves.Add(secondStirrup_line2);
                    Curve secondStirrup_line3 = Line.CreateBound(secondStirrup_p3, secondStirrup_p4) as Curve;
                    secondStirrupCurves.Add(secondStirrup_line3);
                    Curve secondStirrup_line4 = Line.CreateBound(secondStirrup_p4, secondStirrup_p1) as Curve;
                    secondStirrupCurves.Add(secondStirrup_line4);

                    //Кривые хомута дополнительного 90 град
                    List<Curve> thirdStirrupCurves = new List<Curve>();

                    Curve thirdStirrup_line1 = Line.CreateBound(thirdStirrup_p1, thirdStirrup_p2) as Curve;
                    thirdStirrupCurves.Add(thirdStirrup_line1);
                    Curve thirdStirrup_line2 = Line.CreateBound(thirdStirrup_p2, thirdStirrup_p3) as Curve;
                    thirdStirrupCurves.Add(thirdStirrup_line2);
                    Curve thirdStirrup_line3 = Line.CreateBound(thirdStirrup_p3, thirdStirrup_p4) as Curve;
                    thirdStirrupCurves.Add(thirdStirrup_line3);
                    Curve thirdStirrup_line4 = Line.CreateBound(thirdStirrup_p4, thirdStirrup_p1) as Curve;
                    thirdStirrupCurves.Add(thirdStirrup_line4);

                    //Построение нижнего хомута дополнительного
                    Rebar buttomSecondStirrup = Rebar.CreateFromCurvesAndShape(doc
                        , form51
                        , secondStirrupBarTape
                        , rebarHookTypeForStirrup
                        , rebarHookTypeForStirrup
                        , column
                        , new XYZ(0, 0, 1)
                        , secondStirrupCurves
                        , RebarHookOrientation.Right
                        , RebarHookOrientation.Right);
                    ElementTransformUtils.RotateElement(doc, buttomSecondStirrup.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                    buttomSecondStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                    buttomSecondStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(buttomStirrupQuantity + 1);
                    buttomSecondStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(frequentButtomStirrupStep);

                    //Построение нижнего хомута дополнительного 90 град
                    Rebar buttomThirdStirrup = Rebar.CreateFromCurvesAndShape(doc
                        , form51
                        , secondStirrupBarTape
                        , rebarHookTypeForStirrup
                        , rebarHookTypeForStirrup
                        , column
                        , new XYZ(0, 0, 1)
                        , thirdStirrupCurves
                        , RebarHookOrientation.Right
                        , RebarHookOrientation.Right);
                    ElementTransformUtils.RotateElement(doc, buttomThirdStirrup.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                    buttomThirdStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                    buttomThirdStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(buttomStirrupQuantity + 1);
                    buttomThirdStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(frequentButtomStirrupStep);

                    //Копирование хомута дополнительного
                    XYZ mediumSecondStirrupInstallationPoint = new XYZ(0, 0, frequentButtomStirrupPlacementHeight + standardStirrupStep);
                    List<ElementId> mediumSecondStirrupIdList = ElementTransformUtils.CopyElement(doc, buttomSecondStirrup.Id, mediumSecondStirrupInstallationPoint) as List<ElementId>;
                    Rebar mediumSecondStirrup = doc.GetElement(mediumSecondStirrupIdList.First()) as Rebar;

                    //Высота размещения хомутов со стандартным шагом
                    double mediumSecondStirrupPlacementHeight = columnProperty.ColumnLength - frequentButtomStirrupPlacementHeight - frequentTopStirrupPlacementHeight - firstStirrupButtomOffset - 50 / 304.8;
                    int mediumSecondStirrupQuantity = (int)(mediumSecondStirrupPlacementHeight / standardStirrupStep);
                    mediumSecondStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                    mediumSecondStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(mediumSecondStirrupQuantity);
                    mediumSecondStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(standardStirrupStep);


                    //Копирование хомута дополнительного 90 град
                    XYZ mediumThirdStirrupInstallationPoint = new XYZ(0, 0, frequentButtomStirrupPlacementHeight + standardStirrupStep);
                    List<ElementId> mediumThirdStirrupIdList = ElementTransformUtils.CopyElement(doc, buttomThirdStirrup.Id, mediumThirdStirrupInstallationPoint) as List<ElementId>;
                    Rebar mediumThirdStirrup = doc.GetElement(mediumThirdStirrupIdList.First()) as Rebar;

                    //Высота размещения хомутов со стандартным шагом
                    double mediumThirdStirrupPlacementHeight = columnProperty.ColumnLength - frequentButtomStirrupPlacementHeight - frequentTopStirrupPlacementHeight - firstStirrupButtomOffset - 50 / 304.8;
                    int mediumThirdStirrupQuantity = (int)(mediumThirdStirrupPlacementHeight / standardStirrupStep);
                    mediumThirdStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                    mediumThirdStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(mediumThirdStirrupQuantity);
                    mediumThirdStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(standardStirrupStep);

                    //Копирование хомута последний
                    XYZ topSecondStirrupInstallationPoint = new XYZ(0, 0, topStirrupPlacementHeight);
                    List<ElementId> topSecondStirrupIdList = ElementTransformUtils.CopyElement(doc, buttomSecondStirrup.Id, topSecondStirrupInstallationPoint) as List<ElementId>;
                    Rebar topSecondStirrup = doc.GetElement(topSecondStirrupIdList.First()) as Rebar;
                    topSecondStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                    topSecondStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(topStirrupQuantity);
                    topSecondStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(frequentTopStirrupStep);
                    topSecondStirrup.GetShapeDrivenAccessor().BarsOnNormalSide = false;

                    //Копирование хомута последний
                    XYZ topThirdStirrupInstallationPoint = new XYZ(0, 0, topStirrupPlacementHeight);
                    List<ElementId> topThirdStirrupIdList = ElementTransformUtils.CopyElement(doc, buttomThirdStirrup.Id, topThirdStirrupInstallationPoint) as List<ElementId>;
                    Rebar topThirdStirrup = doc.GetElement(topThirdStirrupIdList.First()) as Rebar;
                    topThirdStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set(3);
                    topThirdStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(topStirrupQuantity);
                    topThirdStirrup.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(frequentTopStirrupStep);
                    topThirdStirrup.GetShapeDrivenAccessor().BarsOnNormalSide = false;

                    if (squareColumnsReinforcementWPF.ProgressiveCollapseBarIntoSlabChecked)
                    {
                        RebarBarType progressiveCollapseBarTape = squareColumnsReinforcementWPF.ProgressiveCollapseBarTape;
#if R2019 || R2020 || R2021 || R2022
                        double progressiveCollapseBarDiam = progressiveCollapseBarTape.BarDiameter;
#else
                        double progressiveCollapseBarDiam = progressiveCollapseBarTape.BarNominalDiameter;
#endif

                        double progressiveCollapseBottomIndent = squareColumnsReinforcementWPF.ProgressiveCollapseBottomIndent / 304.8;
                        double progressiveCollapseUpLength = squareColumnsReinforcementWPF.ProgressiveCollapseUpLength / 304.8;
                        double progressiveCollapseSideLength = squareColumnsReinforcementWPF.ProgressiveCollapseSideLength / 304.8;
                        double progressiveCollapseColumnCenterOffset = squareColumnsReinforcementWPF.ProgressiveCollapseColumnCenterOffset / 304.8;

                        //Точки для построения кривых стержня
                        XYZ rebar_p1PC = new XYZ(Math.Round(columnProperty.ColumnBasePoint.X, 6), Math.Round(columnProperty.ColumnBasePoint.Y, 6), Math.Round(columnProperty.ColumnBasePoint.Z + progressiveCollapseUpLength - progressiveCollapseBottomIndent, 6));
                        XYZ rebar_p2PC = new XYZ(Math.Round(rebar_p1PC.X, 6), Math.Round(rebar_p1PC.Y, 6), Math.Round(rebar_p1PC.Z - progressiveCollapseUpLength + progressiveCollapseBarDiam / 2, 6));
                        XYZ rebar_p3PC = new XYZ(Math.Round(rebar_p2PC.X + progressiveCollapseSideLength - progressiveCollapseBarDiam / 2, 6), Math.Round(rebar_p2PC.Y, 6), Math.Round(rebar_p2PC.Z, 6));

                        //Кривые стержня
                        List<Curve> rebarCurvesPC = new List<Curve>();

                        Curve line1PC = Line.CreateBound(rebar_p1PC, rebar_p2PC) as Curve;
                        rebarCurvesPC.Add(line1PC);
                        Curve line2PC = Line.CreateBound(rebar_p2PC, rebar_p3PC) as Curve;
                        rebarCurvesPC.Add(line2PC);

                        //Правый
                        Rebar rebarPC_1 = Rebar.CreateFromCurvesAndShape(doc
                            , form11
                            , progressiveCollapseBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , rebarCurvesPC
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        if (rebarPC_1 == null)
                        {
                            TaskDialog.Show("Revit", "Не удалось создать дополнительный Г-образный стержень в основании колонны! Возможно выбран некорректный тип формы 11!");
                            return Result.Cancelled;
                        }

                        XYZ newPlaсeRebarPC_1 = new XYZ(progressiveCollapseColumnCenterOffset, 0, 0);
                        ElementTransformUtils.MoveElement(doc, rebarPC_1.Id, newPlaсeRebarPC_1);
                        ElementTransformUtils.RotateElement(doc, rebarPC_1.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Левый
                        Rebar rebarPC_2 = Rebar.CreateFromCurvesAndShape(doc
                            , form11
                            , progressiveCollapseBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , rebarCurvesPC
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        XYZ rotate1_p1 = new XYZ(rebar_p1PC.X, rebar_p1PC.Y, rebar_p1PC.Z);
                        XYZ rotate1_p2 = new XYZ(rebar_p1PC.X, rebar_p1PC.Y, rebar_p1PC.Z + 1);
                        Line rotateLine = Line.CreateBound(rotate1_p1, rotate1_p2);
                        ElementTransformUtils.RotateElement(doc, rebarPC_2.Id, rotateLine, 180 * (Math.PI / 180));

                        XYZ newPlaсeRebarPC_2 = new XYZ(-progressiveCollapseColumnCenterOffset, 0, 0);
                        ElementTransformUtils.MoveElement(doc, rebarPC_2.Id, newPlaсeRebarPC_2);
                        ElementTransformUtils.RotateElement(doc, rebarPC_2.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Верхний
                        Rebar rebarPC_3 = Rebar.CreateFromCurvesAndShape(doc
                            , form11
                            , progressiveCollapseBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , rebarCurvesPC
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        ElementTransformUtils.RotateElement(doc, rebarPC_3.Id, rotateLine, 90 * (Math.PI / 180));

                        XYZ newPlaсeRebarPC_3 = new XYZ(0, progressiveCollapseColumnCenterOffset, 0);
                        ElementTransformUtils.MoveElement(doc, rebarPC_3.Id, newPlaсeRebarPC_3);
                        ElementTransformUtils.RotateElement(doc, rebarPC_3.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);

                        //Нижний
                        Rebar rebarPC_4 = Rebar.CreateFromCurvesAndShape(doc
                            , form11
                            , progressiveCollapseBarTape
                            , null
                            , null
                            , column
                            , new XYZ(0, 1, 0)
                            , rebarCurvesPC
                            , RebarHookOrientation.Right
                            , RebarHookOrientation.Right);

                        ElementTransformUtils.RotateElement(doc, rebarPC_4.Id, rotateLine, -90 * (Math.PI / 180));

                        XYZ newPlaсeRebarPC_4 = new XYZ(0, -progressiveCollapseColumnCenterOffset, 0);
                        ElementTransformUtils.MoveElement(doc, rebarPC_4.Id, newPlaсeRebarPC_4);
                        ElementTransformUtils.RotateElement(doc, rebarPC_4.Id, rotateLineBase, (column.Location as LocationPoint).Rotation);
                    }
                }
                t.Commit();
            }
            return Result.Succeeded;
        }
        
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            throw new NotImplementedException();
        }
    }
}
