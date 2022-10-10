using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquareColumnsReinforcement
{
    public class SquareColumnsReinforcementT6
    {
        public SquareColumnsReinforcementT6(Document doc
            , FamilyInstance column
            , SquareColumnsReinforcementWPF squareColumnsReinforcementWPF)
        {
            ColumnPropertyCollector columnProperty = new ColumnPropertyCollector(doc, column);
        }
    }
}
