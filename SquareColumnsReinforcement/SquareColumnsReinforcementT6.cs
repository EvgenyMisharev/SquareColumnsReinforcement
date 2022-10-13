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
            , List<FamilyInstance> columnsList
            , SquareColumnsReinforcementWPF squareColumnsReinforcementWPF)
        {


            using (Transaction t = new Transaction(doc))
            {
                t.Start("Армирование колонн - Тип 6");
                foreach (FamilyInstance column in columnsList)
                {
                    ColumnPropertyCollector columnProperty = new ColumnPropertyCollector(doc, column);

                    if (squareColumnsReinforcementWPF.RebarConnectionOptionName == "radioButton_Overlap"
                        & squareColumnsReinforcementWPF.SectionChangeChecked == false
                        & squareColumnsReinforcementWPF.BendInSlabChecked == false)
                    {

                    }
                }
                t.Commit();
            }
        }
    }
}
