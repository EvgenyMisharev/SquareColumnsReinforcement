using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquareColumnsReinforcement
{

    class ColumnProperties
    {
        private Level columnBaseLevel;
        private double columnBaseLevelElevation;
        private double columnTopLevelElevation;
        private double columnBaseLevelOffset;
        private double columnTopLevelOffset;
        private double columnLength;
        private double columnSectionWidth;
        private double columnSectionHeight;
        private XYZ columnOrigin;
        private double columnRotation;
        private Line columnRotationAxis;


        public Level СolumnBaseLevel => columnBaseLevel;
        /// <summary>  
        ///  Отметка базового уровня колонны 
        /// </summary>
        public double СolumnBaseLevelElevation => columnBaseLevelElevation;
        /// <summary>  
        ///  Отметка верхнего уровня колонны
        /// </summary>
        public double СolumnTopLevelElevation => columnTopLevelElevation;
        /// <summary>  
        ///  Смещение снизу
        /// </summary>
        public double СolumnBaseLevelOffset => columnBaseLevelOffset;
        /// <summary>  
        /// Смещение сверху
        /// </summary>
        public double СolumnTopLevelOffset => columnTopLevelOffset;
        /// <summary>  
        ///  Длина колонны
        /// </summary>
        public double ColumnLength => columnLength;
        /// <summary>  
        ///  Ширина сечения колонны
        /// </summary>
        public double ColumnSectionWidth => columnSectionWidth;
        /// <summary>  
        ///  Высота сечения колонны
        /// </summary>
        public double ColumnSectionHeight => columnSectionHeight;
        /// <summary>  
        ///  Нижняя центральная точка колонны
        /// </summary>
        public XYZ ColumnOrigin => columnOrigin;
        /// <summary>  
        ///  Угол поворота колонны
        /// </summary>
        public double ColumnRotation => columnRotation;
        /// <summary>  
        /// Ось вращения колонны
        /// </summary>
        public Line ColumnRotationAxis => columnRotationAxis;

        public ColumnProperties(Document doc, FamilyInstance column)
        {
            ElementId baseLevelId = column.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_PARAM).AsElementId();
            columnBaseLevel = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .Where(lv => lv.Id == baseLevelId)
                .Cast<Level>()
                .ToList()
                .First();
            columnBaseLevelElevation = Math.Round(columnBaseLevel.Elevation, 6);

            ElementId topLevelId = column.get_Parameter(BuiltInParameter.SCHEDULE_TOP_LEVEL_PARAM).AsElementId();
            Level topLevel = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .Where(lv => lv.Id == topLevelId)
                .Cast<Level>()
                .ToList()
                .First();
            columnTopLevelElevation = Math.Round(topLevel.Elevation, 6);

            columnBaseLevelOffset = Math.Round(column.get_Parameter(BuiltInParameter.SCHEDULE_BASE_LEVEL_OFFSET_PARAM).AsDouble(), 6);
            columnTopLevelOffset = Math.Round(column.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM).AsDouble(), 6);

            columnLength = ((columnTopLevelElevation + columnTopLevelOffset) - (columnBaseLevelElevation + columnBaseLevelOffset));
            columnSectionWidth = column.Symbol.LookupParameter("Рзм.Ширина").AsDouble();
            columnSectionHeight = column.Symbol.LookupParameter("Рзм.Высота").AsDouble();

            LocationPoint columnOriginLocationPoint = column.Location as LocationPoint;
            XYZ columnOriginBase = columnOriginLocationPoint.Point;
            columnOrigin = new XYZ(columnOriginBase.X, columnOriginBase.Y, columnBaseLevelElevation + columnBaseLevelOffset);

            columnRotation = columnOriginLocationPoint.Rotation;

            XYZ rotationPoint1 = new XYZ(columnOrigin.X, columnOrigin.Y, columnOrigin.Z);
            XYZ rotationPoint2 = new XYZ(columnOrigin.X, columnOrigin.Y, columnOrigin.Z + 1);
            columnRotationAxis = Line.CreateBound(rotationPoint1, rotationPoint2);
        }








    }
}
