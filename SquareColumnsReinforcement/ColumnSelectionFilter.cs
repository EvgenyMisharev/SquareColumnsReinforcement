using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace SquareColumnsReinforcement
{
    class ColumnSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return elem is FamilyInstance && null != elem.Category && elem.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_StructuralColumns);
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
