using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace SquareColumnsReinforcement
{
    class ColumnSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is FamilyInstance
                && null != elem.Category 
                && elem.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_StructuralColumns))
            {
                return true;
            }
            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
