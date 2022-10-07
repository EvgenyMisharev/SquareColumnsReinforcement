using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SquareColumnsReinforcement
{
    class SquareColumnsReinforcementSettingsT3
    {
        public string Form01Name { get; set; }
        public string Form26Name { get; set; }
        public string Form11Name { get; set; }
        public string Form51Name { get; set; }
        public string RebarHookTypeForStirrupName { get; set; }

        public string FirstMainBarTapeName { get; set; }
        public string SecondMainBarTapeName { get; set; }
        public string FirstStirrupBarTapeName { get; set; }
        public string SecondStirrupBarTapeName { get; set; }
        public string ColumnRebarCoverTypeName { get; set; }

        public string RebarConnectionOptionName { get; set; }
        public bool DeepenRebarChecked { get; set; }
        public double DeepenRebar { get; set; }

        public bool OverlapTransitionChecked { get; set; }

        public double FirstRebarOutletsLength { get; set; }
        public double SecondRebarOutletsLength { get; set; }
        public double StandardStirrupStep { get; set; }
        public double FrequentButtomStirrupStep { get; set; }
        public double FrequentButtomStirrupPlacementHeight { get; set; }
        public double FrequentTopStirrupStep { get; set; }
        public double FrequentTopStirrupPlacementHeight { get; set; }
        public double FloorThickness { get; set; }
        public double FirstStirrupButtomOffset { get; set; }

        public bool BendInSlabChecked { get; set; }
        public double BendInSlab { get; set; }

        public bool SectionChangeChecked { get; set; }
        public double SectionChange { get; set; }

        public bool ProgressiveCollapseBarIntoSlabChecked { get; set; }
        public string ProgressiveCollapseBarTapeName { get; set; }
        public double ProgressiveCollapseColumnCenterOffset { get; set; }
        public double ProgressiveCollapseUpLength { get; set; }
        public double ProgressiveCollapseBottomIndent { get; set; }
        public double ProgressiveCollapseSideLength { get; set; }

        public string MechanicalConnectionOptionName { get; set; }
        public string WeldedConnectionFamilyName { get; set; }
        public string CouplingConnectionFamilyName { get; set; }

        public SquareColumnsReinforcementSettingsT3 GetSettings()
        {
            SquareColumnsReinforcementSettingsT3 squareColumnsReinforcementSettingsT3 = null;
            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "SquareColumnsReinforcementSettingsT3.xml";
            string assemblyPath = assemblyPathAll.Replace("SquareColumnsReinforcement.dll", fileName);

            if (File.Exists(assemblyPath))
            {
                using (FileStream fs = new FileStream(assemblyPath, FileMode.Open))
                {
                    XmlSerializer xSer = new XmlSerializer(typeof(SquareColumnsReinforcementSettingsT3));
                    squareColumnsReinforcementSettingsT3 = xSer.Deserialize(fs) as SquareColumnsReinforcementSettingsT3;
                    fs.Close();
                }
            }
            else
            {
                squareColumnsReinforcementSettingsT3 = null;
            }

            return squareColumnsReinforcementSettingsT3;
        }

        public void SaveSettings()
        {
            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "SquareColumnsReinforcementSettingsT3.xml";
            string assemblyPath = assemblyPathAll.Replace("SquareColumnsReinforcement.dll", fileName);

            if (File.Exists(assemblyPath))
            {
                File.Delete(assemblyPath);
            }

            using (FileStream fs = new FileStream(assemblyPath, FileMode.Create))
            {
                XmlSerializer xSer = new XmlSerializer(typeof(SquareColumnsReinforcementSettingsT3));
                xSer.Serialize(fs, this);
                fs.Close();
            }
        }
    }
}
