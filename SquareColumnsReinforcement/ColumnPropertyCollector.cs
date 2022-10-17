using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace SquareColumnsReinforcement
{
    class ColumnPropertyCollector
    {
        public XYZ ColumnFacingOrientation { get; }
        public XYZ ColumnHandOrientation { get; }
        public double ColumnSectionWidth { get; }
        public double ColumnSectionHeight { get; }
        public Level BaseLevel { get; }
        public double BaseLevelElevation { get; }
        public double BaseLevelOffset { get; }
        public double TopLevelElevation { get; }
        public double TopLevelOffset { get; }
        public double ColumnLength { get; }
        public XYZ ColumnBasePoint { get; }


        public ColumnPropertyCollector(Document doc, FamilyInstance column)
        {
            //Ориентация колонны
            ColumnFacingOrientation = column.FacingOrientation;
            ColumnHandOrientation = column.HandOrientation;

            //Размеры сечения колонны
            CurveArray columnProfileCurveArray = column.GetSweptProfile().GetSweptProfile().Curves;
            List<Curve> columnProfileCurveList = new List<Curve>();
            foreach(Curve curve in columnProfileCurveArray)
            {
                columnProfileCurveList.Add(curve);
            }

            ColumnSectionWidth = Math.Round(columnProfileCurveList.FirstOrDefault(c => (c as Line).Direction.IsAlmostEqualTo(new XYZ(0, 1, 0))).Length, 6);
            ColumnSectionHeight = Math.Round(columnProfileCurveList.FirstOrDefault(c => (c as Line).Direction.IsAlmostEqualTo(new XYZ(1, 0, 0))).Length, 6);

            //Базовый уровень
            BaseLevel = doc.GetElement(column.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_PARAM).AsElementId()) as Level;
            //Отметка базового уровеня
            BaseLevelElevation = Math.Round((doc.GetElement(column.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_PARAM).AsElementId()) as Level).Elevation, 6);
            //Смещение снизу
            BaseLevelOffset = Math.Round(column.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM).AsDouble(), 6);

            //Отметка уровня сверху
            TopLevelElevation = Math.Round((doc.GetElement(column.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).AsElementId()) as Level).Elevation, 6);
            //Смещение сверху
            TopLevelOffset = Math.Round(column.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM).AsDouble(), 6);

            //Нижняя точка геометрии колонны
            XYZ columnOriginPoint = (column.Location as LocationPoint).Point;
            ColumnBasePoint = new XYZ(columnOriginPoint.X, columnOriginPoint.Y, BaseLevelElevation + BaseLevelOffset);

            //Длина колонны
            ColumnLength = TopLevelElevation + TopLevelOffset - (BaseLevelElevation + BaseLevelOffset);
        }
    }
}
