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
        public string SelectedReinforcementTypeButtonName { get; set; }
        public SquareColumnsReinforcementSettings GetSettings()
        {
            SquareColumnsReinforcementSettings squareColumnsReinforcementSettings = null;
            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "SquareColumnsReinforcementSettings.xml";
            string assemblyPath = assemblyPathAll.Replace("SquareColumnsReinforcement.dll", fileName);

            if (File.Exists(assemblyPath))
            {
                using (FileStream fs = new FileStream(assemblyPath, FileMode.Open))
                {
                    XmlSerializer xSer = new XmlSerializer(typeof(SquareColumnsReinforcementSettings));
                    squareColumnsReinforcementSettings = xSer.Deserialize(fs) as SquareColumnsReinforcementSettings;
                    fs.Close();
                }
            }
            else
            {
                squareColumnsReinforcementSettings = null;
            }

            return squareColumnsReinforcementSettings;
        }

        public void SaveSettings()
        {
            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "SquareColumnsReinforcementSettings.xml";
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
