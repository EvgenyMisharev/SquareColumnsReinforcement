using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SquareColumnsReinforcement
{
    public class SquareColumnsReinforcementSettings
    {
        public string MainRebarTape { get; set; }
        public string AdditionalMainRebarTape { get; set; }
        public string MainStirrupRebarTape { get; set; }
        public string AdditionalMainStirrupRebarTape { get; set; }
        public string RebarCoverType { get; set; }
        public string ProgressiveCollapseRebarType { get; set; }

        public string MainRebarOutletsLongLength { get; set; }
        public string MainRebarOutletsShortLength { get; set; }
        public string FloorThicknessAboveColumn { get; set; }
        public string StandardStirrupStep { get; set; }
        public string FrequentTopStirrupStep { get; set; }
        public string FrequentBottomStirrupStep { get; set; }
        public string FrequentTopStirrupPlacementHeight { get; set; }
        public string FrequentBottomStirrupPlacementHeight { get; set; }
        public string FirstBottomStirrupOffset { get; set; }


        public string FirstUpRebarOffset { get; set; }
        public string SecondUpRebarOffset { get; set; }
        public string FirstDownRebarOffset { get; set; }
        public string SecondDownRebarOffset { get; set; }

        public string FirstLeftRebarOffset { get; set; }
        public string SecondLeftRebarOffset { get; set; }
        public string FirstRightRebarOffset { get; set; }
        public string SecondRightRebarOffset { get; set; }

        public string ProgressiveCollapseUpLength { get; set; }
        public string ProgressiveCollapseSideLength { get; set; }
        public string ProgressiveCollapseBottomOffset { get; set; }

        public static SquareColumnsReinforcementSettings GetSettingsT1()
        {
            SquareColumnsReinforcementSettings squareColumnsReinforcementSettingsT1 = null;
            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "SquareColumnsReinforcementSettingsT1.xml";
            string assemblyPath = assemblyPathAll.Replace("SquareColumnsReinforcement.dll", fileName);

            if (File.Exists(assemblyPath))
            {
                using (FileStream fs = new FileStream(assemblyPath, FileMode.Open))
                {
                    XmlSerializer xSer = new XmlSerializer(typeof(SquareColumnsReinforcementSettings));
                    squareColumnsReinforcementSettingsT1 = xSer.Deserialize(fs) as SquareColumnsReinforcementSettings;
                    fs.Close();
                }
            }
            else
            {
                squareColumnsReinforcementSettingsT1 = new SquareColumnsReinforcementSettings();
            }

            return squareColumnsReinforcementSettingsT1;
        }
        public static SquareColumnsReinforcementSettings GetSettingsT2()
        {
            SquareColumnsReinforcementSettings squareColumnsReinforcementSettingsT2 = null;
            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "SquareColumnsReinforcementSettingsT2.xml";
            string assemblyPath = assemblyPathAll.Replace("SquareColumnsReinforcement.dll", fileName);

            if (File.Exists(assemblyPath))
            {
                using (FileStream fs = new FileStream(assemblyPath, FileMode.Open))
                {
                    XmlSerializer xSer = new XmlSerializer(typeof(SquareColumnsReinforcementSettings));
                    squareColumnsReinforcementSettingsT2 = xSer.Deserialize(fs) as SquareColumnsReinforcementSettings;
                    fs.Close();
                }
            }
            else
            {
                squareColumnsReinforcementSettingsT2 = new SquareColumnsReinforcementSettings();
            }

            return squareColumnsReinforcementSettingsT2;
        }
        public static SquareColumnsReinforcementSettings GetSettingsT3()
        {
            SquareColumnsReinforcementSettings squareColumnsReinforcementSettingsT3 = null;
            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "SquareColumnsReinforcementSettingsT3.xml";
            string assemblyPath = assemblyPathAll.Replace("SquareColumnsReinforcement.dll", fileName);

            if (File.Exists(assemblyPath))
            {
                using (FileStream fs = new FileStream(assemblyPath, FileMode.Open))
                {
                    XmlSerializer xSer = new XmlSerializer(typeof(SquareColumnsReinforcementSettings));
                    squareColumnsReinforcementSettingsT3 = xSer.Deserialize(fs) as SquareColumnsReinforcementSettings;
                    fs.Close();
                }
            }
            else
            {
                squareColumnsReinforcementSettingsT3 = new SquareColumnsReinforcementSettings();
            }

            return squareColumnsReinforcementSettingsT3;
        }
        public static SquareColumnsReinforcementSettings GetSettingsT4()
        {
            SquareColumnsReinforcementSettings squareColumnsReinforcementSettingsT4 = null;
            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "SquareColumnsReinforcementSettingsT4.xml";
            string assemblyPath = assemblyPathAll.Replace("SquareColumnsReinforcement.dll", fileName);

            if (File.Exists(assemblyPath))
            {
                using (FileStream fs = new FileStream(assemblyPath, FileMode.Open))
                {
                    XmlSerializer xSer = new XmlSerializer(typeof(SquareColumnsReinforcementSettings));
                    squareColumnsReinforcementSettingsT4 = xSer.Deserialize(fs) as SquareColumnsReinforcementSettings;
                    fs.Close();
                }
            }
            else
            {
                squareColumnsReinforcementSettingsT4 = new SquareColumnsReinforcementSettings();
            }

            return squareColumnsReinforcementSettingsT4;
        }
        public static SquareColumnsReinforcementSettings GetSettingsT5()
        {
            SquareColumnsReinforcementSettings squareColumnsReinforcementSettingsT5 = null;
            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "SquareColumnsReinforcementSettingsT5.xml";
            string assemblyPath = assemblyPathAll.Replace("SquareColumnsReinforcement.dll", fileName);

            if (File.Exists(assemblyPath))
            {
                using (FileStream fs = new FileStream(assemblyPath, FileMode.Open))
                {
                    XmlSerializer xSer = new XmlSerializer(typeof(SquareColumnsReinforcementSettings));
                    squareColumnsReinforcementSettingsT5 = xSer.Deserialize(fs) as SquareColumnsReinforcementSettings;
                    fs.Close();
                }
            }
            else
            {
                squareColumnsReinforcementSettingsT5 = new SquareColumnsReinforcementSettings();
            }

            return squareColumnsReinforcementSettingsT5;
        }
        public static SquareColumnsReinforcementSettings GetSettingsT6()
        {
            SquareColumnsReinforcementSettings squareColumnsReinforcementSettingsT6 = null;
            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "SquareColumnsReinforcementSettingsT6.xml";
            string assemblyPath = assemblyPathAll.Replace("SquareColumnsReinforcement.dll", fileName);

            if (File.Exists(assemblyPath))
            {
                using (FileStream fs = new FileStream(assemblyPath, FileMode.Open))
                {
                    XmlSerializer xSer = new XmlSerializer(typeof(SquareColumnsReinforcementSettings));
                    squareColumnsReinforcementSettingsT6 = xSer.Deserialize(fs) as SquareColumnsReinforcementSettings;
                    fs.Close();
                }
            }
            else
            {
                squareColumnsReinforcementSettingsT6 = new SquareColumnsReinforcementSettings();
            }

            return squareColumnsReinforcementSettingsT6;
        }

        public void SaveSettingsT1()
        {
            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "SquareColumnsReinforcementSettingsT1.xml";
            string assemblyPath = assemblyPathAll.Replace("SquareColumnsReinforcement.dll", fileName);

            if (File.Exists(assemblyPath))
            {
                File.Delete(assemblyPath);
            }

            using (FileStream fs = new FileStream(assemblyPath, FileMode.Create))
            {
                XmlSerializer xSer = new XmlSerializer(typeof(SquareColumnsReinforcementSettings));
                xSer.Serialize(fs, this);
                fs.Close();
            }
        }
        public void SaveSettingsT2()
        {
            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "SquareColumnsReinforcementSettingsT2.xml";
            string assemblyPath = assemblyPathAll.Replace("SquareColumnsReinforcement.dll", fileName);

            if (File.Exists(assemblyPath))
            {
                File.Delete(assemblyPath);
            }

            using (FileStream fs = new FileStream(assemblyPath, FileMode.Create))
            {
                XmlSerializer xSer = new XmlSerializer(typeof(SquareColumnsReinforcementSettings));
                xSer.Serialize(fs, this);
                fs.Close();
            }
        }
        public void SaveSettingsT3()
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
                XmlSerializer xSer = new XmlSerializer(typeof(SquareColumnsReinforcementSettings));
                xSer.Serialize(fs, this);
                fs.Close();
            }
        }
        public void SaveSettingsT4()
        {
            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "SquareColumnsReinforcementSettingsT4.xml";
            string assemblyPath = assemblyPathAll.Replace("SquareColumnsReinforcement.dll", fileName);

            if (File.Exists(assemblyPath))
            {
                File.Delete(assemblyPath);
            }

            using (FileStream fs = new FileStream(assemblyPath, FileMode.Create))
            {
                XmlSerializer xSer = new XmlSerializer(typeof(SquareColumnsReinforcementSettings));
                xSer.Serialize(fs, this);
                fs.Close();
            }
        }
        public void SaveSettingsT5()
        {
            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "SquareColumnsReinforcementSettingsT5.xml";
            string assemblyPath = assemblyPathAll.Replace("SquareColumnsReinforcement.dll", fileName);

            if (File.Exists(assemblyPath))
            {
                File.Delete(assemblyPath);
            }

            using (FileStream fs = new FileStream(assemblyPath, FileMode.Create))
            {
                XmlSerializer xSer = new XmlSerializer(typeof(SquareColumnsReinforcementSettings));
                xSer.Serialize(fs, this);
                fs.Close();
            }
        }
        public void SaveSettingsT6()
        {
            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "SquareColumnsReinforcementSettingsT6.xml";
            string assemblyPath = assemblyPathAll.Replace("SquareColumnsReinforcement.dll", fileName);

            if (File.Exists(assemblyPath))
            {
                File.Delete(assemblyPath);
            }

            using (FileStream fs = new FileStream(assemblyPath, FileMode.Create))
            {
                XmlSerializer xSer = new XmlSerializer(typeof(SquareColumnsReinforcementSettings));
                xSer.Serialize(fs, this);
                fs.Close();
            }
        }
    }
}
