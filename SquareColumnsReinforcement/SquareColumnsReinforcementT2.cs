﻿using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquareColumnsReinforcement
{
    public class SquareColumnsReinforcementT2
    {
        public SquareColumnsReinforcementT2(Document doc
            , FamilyInstance column
            , SquareColumnsReinforcementWPF squareColumnsReinforcementWPF)
        {
            ColumnPropertyCollector columnProperty = new ColumnPropertyCollector(doc, column);
        }
    }
}
