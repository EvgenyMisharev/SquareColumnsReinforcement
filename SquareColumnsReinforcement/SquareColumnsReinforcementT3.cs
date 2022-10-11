using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquareColumnsReinforcement
{
    public class SquareColumnsReinforcementT3
    {
        public SquareColumnsReinforcementT3(Document doc
            , List<FamilyInstance> columnsList
            , SquareColumnsReinforcementWPF squareColumnsReinforcementWPF)
        {
            double sectionOffset = squareColumnsReinforcementWPF.SectionChange / 304.8;
            double deltaXOverlapping = Math.Sqrt(Math.Pow((sectionOffset + squareColumnsReinforcementWPF.FirstMainBarTape.BarDiameter), 2) + Math.Pow(sectionOffset, 2));
            double alphaOverlapping = Math.Asin(sectionOffset / deltaXOverlapping);
            double deltaXWelding = Math.Sqrt(Math.Pow(sectionOffset, 2) + Math.Pow(sectionOffset, 2));
            double alphaWelding = Math.Asin(sectionOffset / deltaXWelding);

            using (Transaction t = new Transaction(doc))
            {
                t.Start("Армирование колонн - Тип 3");
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
