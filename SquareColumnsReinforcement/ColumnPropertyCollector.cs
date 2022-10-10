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
        XYZ ColumnFacingOrientation { get; }
        XYZ ColumnHandOrientation { get; }
        double ColumnSectionWidth { get; }
        double ColumnSectionHeight { get; }
        double BaseLevelElevation { get; }
        double BaseLevelOffset { get; }
        double TopLevelElevation { get; }
        double TopLevelOffset { get; }
        double ColumnLength { get; }
        XYZ ColumnBasePoint { get; }


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

            ColumnSectionWidth = Math.Round(columnProfileCurveList.FirstOrDefault(c => (c as Line).Direction.IsAlmostEqualTo(ColumnHandOrientation)).Length, 6);
            ColumnSectionHeight = Math.Round(columnProfileCurveList.FirstOrDefault(c => (c as Line).Direction.IsAlmostEqualTo(ColumnFacingOrientation)).Length, 6);

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
