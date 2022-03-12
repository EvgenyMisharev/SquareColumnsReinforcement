using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace SquareColumnsReinforcement
{
    public partial class SquareColumnsReinforcementMainForm : System.Windows.Forms.Form
    {
        public RebarBarType MainRebarTape;
        public RebarBarType AdditionalMainRebarTape;
        public RebarBarType MainStirrupRebarTape;
        public RebarBarType AdditionalMainStirrupRebarTape;
        public RebarCoverType RebarCoverType;

        public double MainRebarOutletsLongLength;
        public double MainRebarOutletsShortLength;
        public double FloorThicknessAboveColumn;
        public double StandardStirrupStep;
        public double FrequentTopStirrupStep;
        public double FrequentBottomStirrupStep;
        public double FrequentTopStirrupPlacementHeight;
        public double FrequentBottomStirrupPlacementHeight;
        public double FirstBottomStirrupOffset;

        public double FirstUpRebarOffset;
        public double SecondUpRebarOffset;
        public double FirstDownRebarOffset;
        public double SecondDownRebarOffset;

        public double FirstLeftRebarOffset;
        public double SecondLeftRebarOffset;
        public double FirstRightRebarOffset;
        public double SecondRightRebarOffset;

        public string CheckedRebarOutletsTepeName;

        public bool СhangeSection;
        public double СhangeSectionOffset;

        public bool RebarDeepening;
        public double RebarDeepeningOffset;

        public bool BendIntoSlab;
        public double BendIntoSlabOffset;

        public bool TransitionToOverlap;

        private ComboBox comboBox_AdditionalMainRebarTape;
        private ComboBox comboBox_AdditionalMainStirrupRebarTape;

        private TextBox textBox_FirstUpRebarOffset;
        private TextBox textBox_FirstDownRebarOffset;
        private TextBox textBox_SecondUpRebarOffset;
        private TextBox textBox_SecondDownRebarOffset;

        private TextBox textBox_FirstLeftRebarOffset;
        private TextBox textBox_FirstRightRebarOffset;
        private TextBox textBox_SecondLeftRebarOffset;
        private TextBox textBox_SecondRightRebarOffset;

        private List<RebarBarType> listMainRebarTapeForComboBox;
        private List<RebarBarType> listAdditionalMainRebarTapeForComboBox;
        private List<RebarBarType> listMainStirrupRebarTapeForComboBox;
        private List<RebarBarType> listAdditionalMainStirrupRebarTapeForComboBox;
        private List<RebarCoverType> listRebarCoverTypeForComboBox;

        private Document doc;

        SquareColumnsReinforcementSettings squareColumnsReinforcementSettingsT1 = null;
        SquareColumnsReinforcementSettings squareColumnsReinforcementSettingsT2 = null;
        SquareColumnsReinforcementSettings squareColumnsReinforcementSettingsT3 = null;
        SquareColumnsReinforcementSettings squareColumnsReinforcementSettingsT4 = null;
        SquareColumnsReinforcementSettings squareColumnsReinforcementSettingsT5 = null;
        SquareColumnsReinforcementSettings squareColumnsReinforcementSettingsT6 = null;

        public SquareColumnsReinforcementMainForm(Document Doc)
        {
            doc = Doc;
            InitializeComponent();
            GetSettingsFromXML();
            GetListsForComboBoxes();
            MoveFieldsT1();
        }

        private void button_Ok_Click(object sender, EventArgs e)
        {
            SaveSettings();
            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        private void button_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        private void SquareColumnsReinforcementMainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (char)Keys.Enter || e.KeyValue == (char)Keys.Space)
            {
                SaveSettings();

                this.DialogResult = DialogResult.OK;
                this.Close();
            }

            else if (e.KeyValue == (char)Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void radioButton_T1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_T1.Checked)
            {
                pictureBox_S.Image = Properties.Resources.PNGSquareColumnsReinforcementT1_S;
                pictureBox_3D.Image = Properties.Resources.PNGSquareColumnsReinforcementT1_3D;
                MoveFieldsT1();
            }
        }
        private void radioButton_T2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_T2.Checked)
            {
                pictureBox_S.Image = Properties.Resources.PNGSquareColumnsReinforcementT2_S;
                pictureBox_3D.Image = Properties.Resources.PNGSquareColumnsReinforcementT2_3D;
                MoveFieldsT2();
            }
        }
        private void radioButton_T3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_T3.Checked)
            {
                pictureBox_S.Image = Properties.Resources.PNGSquareColumnsReinforcementT3_S;
                pictureBox_3D.Image = Properties.Resources.PNGSquareColumnsReinforcementT3_3D;
                MoveFieldsT3();
            }
        }
        private void radioButton_T4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_T4.Checked)
            {
                pictureBox_S.Image = Properties.Resources.PNGSquareColumnsReinforcementT4_S;
                pictureBox_3D.Image = Properties.Resources.PNGSquareColumnsReinforcementT4_3D;
                MoveFieldsT4();
            }
        }
        private void radioButton_T5_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_T5.Checked)
            {
                pictureBox_S.Image = Properties.Resources.PNGSquareColumnsReinforcementT5_S;
                pictureBox_3D.Image = Properties.Resources.PNGSquareColumnsReinforcementT5_3D;
                MoveFieldsT5();
            }
        }
        private void radioButton_T6_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_T6.Checked)
            {
                pictureBox_S.Image = Properties.Resources.PNGSquareColumnsReinforcementT6_S;
                pictureBox_3D.Image = Properties.Resources.PNGSquareColumnsReinforcementT6_3D;
                MoveFieldsT6();
            }
        }

        private void MoveFieldsT1()
        {
            comboBox_MainRebarTape.Location = new System.Drawing.Point(20, 39);
            comboBox_MainStirrupRebarTape.Location = new System.Drawing.Point(307, 183);
            comboBox_RebarCoverType.Location = new System.Drawing.Point(69, 230);
            textBox_MainRebarOutletsLongLength.Location = new System.Drawing.Point(455, 146);
            textBox_MainRebarOutletsShortLength.Location = new System.Drawing.Point(718, 137);
            textBox_FloorThicknessAboveColumn.Location = new System.Drawing.Point(455, 237);
            textBox_StandardStirrupStep.Location = new System.Drawing.Point(683, 318);
            textBox_FrequentTopStirrupStep.Location = new System.Drawing.Point(683, 239);
            textBox_FrequentBottomStirrupStep.Location = new System.Drawing.Point(683, 414);
            textBox_FrequentTopStirrupPlacementHeight.Location = new System.Drawing.Point(455, 268);
            textBox_FrequentBottomStirrupPlacementHeight.Location = new System.Drawing.Point(455, 451);
            textBox_FirstBottomStirrupOffset.Location = new System.Drawing.Point(455, 487);

            RemoveAdditionalFields();

            comboBox_MainRebarTape.DataSource = listMainRebarTapeForComboBox;
            comboBox_MainRebarTape.DisplayMember = "Name";

            comboBox_MainStirrupRebarTape.DataSource = listMainStirrupRebarTapeForComboBox;
            comboBox_MainStirrupRebarTape.DisplayMember = "Name";

            comboBox_RebarCoverType.DataSource = listRebarCoverTypeForComboBox;
            comboBox_RebarCoverType.DisplayMember = "Name";

            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "SquareColumnsReinforcementSettingsT1.xml";
            string assemblyPath = assemblyPathAll.Replace("SquareColumnsReinforcement.dll", fileName);
            if (File.Exists(assemblyPath))
            {
                comboBox_MainRebarTape.SelectedItem = listMainRebarTapeForComboBox
                    .FirstOrDefault(rbt => rbt.Name == squareColumnsReinforcementSettingsT1.MainRebarTape);
                comboBox_MainStirrupRebarTape.SelectedItem = listMainStirrupRebarTapeForComboBox
                    .FirstOrDefault(rbt => rbt.Name == squareColumnsReinforcementSettingsT1.MainStirrupRebarTape);
                comboBox_RebarCoverType.SelectedItem = listRebarCoverTypeForComboBox
                    .FirstOrDefault(rbt => rbt.Name == squareColumnsReinforcementSettingsT1.RebarCoverType);

                textBox_MainRebarOutletsLongLength.Text = squareColumnsReinforcementSettingsT1.MainRebarOutletsLongLength.ToString();
                textBox_MainRebarOutletsShortLength.Text = squareColumnsReinforcementSettingsT1.MainRebarOutletsShortLength.ToString();
                textBox_FloorThicknessAboveColumn.Text = squareColumnsReinforcementSettingsT1.FloorThicknessAboveColumn.ToString();
                textBox_StandardStirrupStep.Text = squareColumnsReinforcementSettingsT1.StandardStirrupStep.ToString();
                textBox_FrequentTopStirrupStep.Text = squareColumnsReinforcementSettingsT1.FrequentTopStirrupStep.ToString();
                textBox_FrequentBottomStirrupStep.Text = squareColumnsReinforcementSettingsT1.FrequentBottomStirrupStep.ToString();
                textBox_FrequentTopStirrupPlacementHeight.Text = squareColumnsReinforcementSettingsT1.FrequentTopStirrupPlacementHeight.ToString();
                textBox_FrequentBottomStirrupPlacementHeight.Text = squareColumnsReinforcementSettingsT1.FrequentBottomStirrupPlacementHeight.ToString();
                textBox_FirstBottomStirrupOffset.Text = squareColumnsReinforcementSettingsT1.FirstBottomStirrupOffset.ToString();
            }
        }
        private void MoveFieldsT2()
        {
            comboBox_MainRebarTape.Location = new System.Drawing.Point(20, 39);
            comboBox_MainStirrupRebarTape.Location = new System.Drawing.Point(307, 100);
            comboBox_RebarCoverType.Location = new System.Drawing.Point(69, 230);
            textBox_MainRebarOutletsLongLength.Location = new System.Drawing.Point(455, 146);
            textBox_MainRebarOutletsShortLength.Location = new System.Drawing.Point(718, 137);
            textBox_FloorThicknessAboveColumn.Location = new System.Drawing.Point(455, 237);
            textBox_StandardStirrupStep.Location = new System.Drawing.Point(683, 318);
            textBox_FrequentTopStirrupStep.Location = new System.Drawing.Point(683, 239);
            textBox_FrequentBottomStirrupStep.Location = new System.Drawing.Point(683, 414);
            textBox_FrequentTopStirrupPlacementHeight.Location = new System.Drawing.Point(455, 268);
            textBox_FrequentBottomStirrupPlacementHeight.Location = new System.Drawing.Point(455, 451);
            textBox_FirstBottomStirrupOffset.Location = new System.Drawing.Point(455, 487);

            RemoveAdditionalFields();

            comboBox_AdditionalMainRebarTape = new ComboBox();
            comboBox_AdditionalMainRebarTape.Name = "comboBox_AdditionalMainRebarTape";
            comboBox_AdditionalMainRebarTape.Location = new System.Drawing.Point(307, 237);
            comboBox_AdditionalMainRebarTape.Size = new Size(100, 21);
            comboBox_AdditionalMainRebarTape.Sorted = true;
            this.Controls.Add(comboBox_AdditionalMainRebarTape);
            comboBox_AdditionalMainRebarTape.BringToFront();

            comboBox_MainRebarTape.DataSource = listMainRebarTapeForComboBox;
            comboBox_MainRebarTape.DisplayMember = "Name";

            comboBox_AdditionalMainRebarTape.DataSource = listAdditionalMainRebarTapeForComboBox;
            comboBox_AdditionalMainRebarTape.DisplayMember = "Name";

            comboBox_MainStirrupRebarTape.DataSource = listMainStirrupRebarTapeForComboBox;
            comboBox_MainStirrupRebarTape.DisplayMember = "Name";

            comboBox_RebarCoverType.DataSource = listRebarCoverTypeForComboBox;
            comboBox_RebarCoverType.DisplayMember = "Name";

            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "SquareColumnsReinforcementSettingsT2.xml";
            string assemblyPath = assemblyPathAll.Replace("SquareColumnsReinforcement.dll", fileName);
            if (File.Exists(assemblyPath))
            {
                comboBox_MainRebarTape.SelectedItem = listMainRebarTapeForComboBox
                    .FirstOrDefault(rbt => rbt.Name == squareColumnsReinforcementSettingsT2.MainRebarTape);
                comboBox_AdditionalMainRebarTape.SelectedItem = listAdditionalMainRebarTapeForComboBox
                    .FirstOrDefault(rbt => rbt.Name == squareColumnsReinforcementSettingsT2.AdditionalMainRebarTape);
                comboBox_MainStirrupRebarTape.SelectedItem = listMainStirrupRebarTapeForComboBox
                    .FirstOrDefault(rbt => rbt.Name == squareColumnsReinforcementSettingsT2.MainStirrupRebarTape);
                comboBox_RebarCoverType.SelectedItem = listRebarCoverTypeForComboBox
                    .FirstOrDefault(rbt => rbt.Name == squareColumnsReinforcementSettingsT2.RebarCoverType);

                textBox_MainRebarOutletsLongLength.Text = squareColumnsReinforcementSettingsT2.MainRebarOutletsLongLength.ToString();
                textBox_MainRebarOutletsShortLength.Text = squareColumnsReinforcementSettingsT2.MainRebarOutletsShortLength.ToString();
                textBox_FloorThicknessAboveColumn.Text = squareColumnsReinforcementSettingsT2.FloorThicknessAboveColumn.ToString();
                textBox_StandardStirrupStep.Text = squareColumnsReinforcementSettingsT2.StandardStirrupStep.ToString();
                textBox_FrequentTopStirrupStep.Text = squareColumnsReinforcementSettingsT2.FrequentTopStirrupStep.ToString();
                textBox_FrequentBottomStirrupStep.Text = squareColumnsReinforcementSettingsT2.FrequentBottomStirrupStep.ToString();
                textBox_FrequentTopStirrupPlacementHeight.Text = squareColumnsReinforcementSettingsT2.FrequentTopStirrupPlacementHeight.ToString();
                textBox_FrequentBottomStirrupPlacementHeight.Text = squareColumnsReinforcementSettingsT2.FrequentBottomStirrupPlacementHeight.ToString();
                textBox_FirstBottomStirrupOffset.Text = squareColumnsReinforcementSettingsT2.FirstBottomStirrupOffset.ToString();
            }
        }
        private void MoveFieldsT3()
        {
            comboBox_MainRebarTape.Location = new System.Drawing.Point(20, 39);
            comboBox_MainStirrupRebarTape.Location = new System.Drawing.Point(307, 45);
            comboBox_RebarCoverType.Location = new System.Drawing.Point(69, 230);
            textBox_MainRebarOutletsLongLength.Location = new System.Drawing.Point(455, 146);
            textBox_MainRebarOutletsShortLength.Location = new System.Drawing.Point(718, 137);
            textBox_FloorThicknessAboveColumn.Location = new System.Drawing.Point(455, 237);
            textBox_StandardStirrupStep.Location = new System.Drawing.Point(683, 318);
            textBox_FrequentTopStirrupStep.Location = new System.Drawing.Point(683, 239);
            textBox_FrequentBottomStirrupStep.Location = new System.Drawing.Point(683, 414);
            textBox_FrequentTopStirrupPlacementHeight.Location = new System.Drawing.Point(455, 268);
            textBox_FrequentBottomStirrupPlacementHeight.Location = new System.Drawing.Point(455, 451);
            textBox_FirstBottomStirrupOffset.Location = new System.Drawing.Point(455, 487);

            RemoveAdditionalFields();

            comboBox_AdditionalMainRebarTape = new ComboBox();
            comboBox_AdditionalMainRebarTape.Name = "comboBox_AdditionalMainRebarTape";
            comboBox_AdditionalMainRebarTape.Location = new System.Drawing.Point(307, 237);
            comboBox_AdditionalMainRebarTape.Size = new Size(100, 21);
            comboBox_AdditionalMainRebarTape.Sorted = true;
            this.Controls.Add(comboBox_AdditionalMainRebarTape);
            comboBox_AdditionalMainRebarTape.BringToFront();

            comboBox_AdditionalMainStirrupRebarTape = new ComboBox();
            comboBox_AdditionalMainStirrupRebarTape.Name = "comboBox_AdditionalMainStirrupRebarTape";
            comboBox_AdditionalMainStirrupRebarTape.Location = new System.Drawing.Point(307, 110);
            comboBox_AdditionalMainStirrupRebarTape.Size = new Size(100, 21);
            comboBox_AdditionalMainStirrupRebarTape.Sorted = true;
            this.Controls.Add(comboBox_AdditionalMainStirrupRebarTape);
            comboBox_AdditionalMainStirrupRebarTape.BringToFront();

            comboBox_MainRebarTape.DataSource = listMainRebarTapeForComboBox;
            comboBox_MainRebarTape.DisplayMember = "Name";

            comboBox_AdditionalMainRebarTape.DataSource = listAdditionalMainRebarTapeForComboBox;
            comboBox_AdditionalMainRebarTape.DisplayMember = "Name";

            comboBox_MainStirrupRebarTape.DataSource = listMainStirrupRebarTapeForComboBox;
            comboBox_MainStirrupRebarTape.DisplayMember = "Name";

            comboBox_AdditionalMainStirrupRebarTape.DataSource = listAdditionalMainStirrupRebarTapeForComboBox;
            comboBox_AdditionalMainStirrupRebarTape.DisplayMember = "Name";

            comboBox_RebarCoverType.DataSource = listRebarCoverTypeForComboBox;
            comboBox_RebarCoverType.DisplayMember = "Name";

            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "SquareColumnsReinforcementSettingsT3.xml";
            string assemblyPath = assemblyPathAll.Replace("SquareColumnsReinforcement.dll", fileName);
            if (File.Exists(assemblyPath))
            {
                comboBox_MainRebarTape.SelectedItem = listMainRebarTapeForComboBox
                    .FirstOrDefault(rbt => rbt.Name == squareColumnsReinforcementSettingsT3.MainRebarTape);
                comboBox_AdditionalMainRebarTape.SelectedItem = listAdditionalMainRebarTapeForComboBox
                    .FirstOrDefault(rbt => rbt.Name == squareColumnsReinforcementSettingsT3.AdditionalMainRebarTape);
                comboBox_MainStirrupRebarTape.SelectedItem = listMainStirrupRebarTapeForComboBox
                    .FirstOrDefault(rbt => rbt.Name == squareColumnsReinforcementSettingsT3.MainStirrupRebarTape);
                comboBox_AdditionalMainStirrupRebarTape.SelectedItem = listAdditionalMainStirrupRebarTapeForComboBox
                    .FirstOrDefault(rbt => rbt.Name == squareColumnsReinforcementSettingsT3.AdditionalMainStirrupRebarTape);
                comboBox_RebarCoverType.SelectedItem = listRebarCoverTypeForComboBox
                    .FirstOrDefault(rbt => rbt.Name == squareColumnsReinforcementSettingsT3.RebarCoverType);

                textBox_MainRebarOutletsLongLength.Text = squareColumnsReinforcementSettingsT3.MainRebarOutletsLongLength.ToString();
                textBox_MainRebarOutletsShortLength.Text = squareColumnsReinforcementSettingsT3.MainRebarOutletsShortLength.ToString();
                textBox_FloorThicknessAboveColumn.Text = squareColumnsReinforcementSettingsT3.FloorThicknessAboveColumn.ToString();
                textBox_StandardStirrupStep.Text = squareColumnsReinforcementSettingsT3.StandardStirrupStep.ToString();
                textBox_FrequentTopStirrupStep.Text = squareColumnsReinforcementSettingsT3.FrequentTopStirrupStep.ToString();
                textBox_FrequentBottomStirrupStep.Text = squareColumnsReinforcementSettingsT3.FrequentBottomStirrupStep.ToString();
                textBox_FrequentTopStirrupPlacementHeight.Text = squareColumnsReinforcementSettingsT3.FrequentTopStirrupPlacementHeight.ToString();
                textBox_FrequentBottomStirrupPlacementHeight.Text = squareColumnsReinforcementSettingsT3.FrequentBottomStirrupPlacementHeight.ToString();
                textBox_FirstBottomStirrupOffset.Text = squareColumnsReinforcementSettingsT3.FirstBottomStirrupOffset.ToString();
            }

        }
        private void MoveFieldsT4()
        {
            comboBox_MainRebarTape.Location = new System.Drawing.Point(20, 39);
            comboBox_MainStirrupRebarTape.Location = new System.Drawing.Point(307, 45);
            comboBox_RebarCoverType.Location = new System.Drawing.Point(69, 230);
            textBox_MainRebarOutletsLongLength.Location = new System.Drawing.Point(455, 146);
            textBox_MainRebarOutletsShortLength.Location = new System.Drawing.Point(718, 138);
            textBox_FloorThicknessAboveColumn.Location = new System.Drawing.Point(455, 237);
            textBox_StandardStirrupStep.Location = new System.Drawing.Point(692, 320);
            textBox_FrequentTopStirrupStep.Location = new System.Drawing.Point(692, 241);
            textBox_FrequentBottomStirrupStep.Location = new System.Drawing.Point(692, 416);
            textBox_FrequentTopStirrupPlacementHeight.Location = new System.Drawing.Point(455, 268);
            textBox_FrequentBottomStirrupPlacementHeight.Location = new System.Drawing.Point(455, 458);
            textBox_FirstBottomStirrupOffset.Location = new System.Drawing.Point(455, 490);

            RemoveAdditionalFields();

            comboBox_AdditionalMainRebarTape = new ComboBox();
            comboBox_AdditionalMainRebarTape.Name = "comboBox_AdditionalMainRebarTape";
            comboBox_AdditionalMainRebarTape.Location = new System.Drawing.Point(307, 237);
            comboBox_AdditionalMainRebarTape.Size = new Size(100, 21);
            comboBox_AdditionalMainRebarTape.Sorted = true;
            this.Controls.Add(comboBox_AdditionalMainRebarTape);
            comboBox_AdditionalMainRebarTape.BringToFront();

            comboBox_AdditionalMainStirrupRebarTape = new ComboBox();
            comboBox_AdditionalMainStirrupRebarTape.Name = "comboBox_AdditionalMainStirrupRebarTape";
            comboBox_AdditionalMainStirrupRebarTape.Location = new System.Drawing.Point(307, 110);
            comboBox_AdditionalMainStirrupRebarTape.Size = new Size(100, 21);
            comboBox_AdditionalMainStirrupRebarTape.Sorted = true;
            this.Controls.Add(comboBox_AdditionalMainStirrupRebarTape);
            comboBox_AdditionalMainStirrupRebarTape.BringToFront();

            textBox_FirstUpRebarOffset = new TextBox();
            textBox_FirstUpRebarOffset.Name = "textBox_FirstUpRebarOffset";
            textBox_FirstUpRebarOffset.Location = new System.Drawing.Point(44, 133);
            textBox_FirstUpRebarOffset.Size = new Size(35, 20);
            textBox_FirstUpRebarOffset.TextAlign = HorizontalAlignment.Center;
            this.Controls.Add(textBox_FirstUpRebarOffset);
            textBox_FirstUpRebarOffset.BringToFront();

            textBox_FirstDownRebarOffset = new TextBox();
            textBox_FirstDownRebarOffset.Name = "textBox_FirstDownRebarOffset";
            textBox_FirstDownRebarOffset.Location = new System.Drawing.Point(44, 157);
            textBox_FirstDownRebarOffset.Size = new Size(35, 20);
            textBox_FirstDownRebarOffset.TextAlign = HorizontalAlignment.Center;
            this.Controls.Add(textBox_FirstDownRebarOffset);
            textBox_FirstDownRebarOffset.BringToFront();

            textBox_FirstLeftRebarOffset = new TextBox();
            textBox_FirstLeftRebarOffset.Name = "textBox_FirstLeftRebarOffset";
            textBox_FirstLeftRebarOffset.Location = new System.Drawing.Point(170, 275);
            textBox_FirstLeftRebarOffset.Size = new Size(35, 20);
            textBox_FirstLeftRebarOffset.TextAlign = HorizontalAlignment.Center;
            this.Controls.Add(textBox_FirstLeftRebarOffset);
            textBox_FirstLeftRebarOffset.BringToFront();

            textBox_FirstRightRebarOffset = new TextBox();
            textBox_FirstRightRebarOffset.Name = "textBox_FirstRightRebarOffset";
            textBox_FirstRightRebarOffset.Location = new System.Drawing.Point(214, 275);
            textBox_FirstRightRebarOffset.Size = new Size(35, 20);
            textBox_FirstRightRebarOffset.TextAlign = HorizontalAlignment.Center;
            this.Controls.Add(textBox_FirstRightRebarOffset);
            textBox_FirstRightRebarOffset.BringToFront();

            comboBox_MainRebarTape.DataSource = listMainRebarTapeForComboBox;
            comboBox_MainRebarTape.DisplayMember = "Name";

            comboBox_AdditionalMainRebarTape.DataSource = listAdditionalMainRebarTapeForComboBox;
            comboBox_AdditionalMainRebarTape.DisplayMember = "Name";

            comboBox_MainStirrupRebarTape.DataSource = listMainStirrupRebarTapeForComboBox;
            comboBox_MainStirrupRebarTape.DisplayMember = "Name";

            comboBox_AdditionalMainStirrupRebarTape.DataSource = listAdditionalMainStirrupRebarTapeForComboBox;
            comboBox_AdditionalMainStirrupRebarTape.DisplayMember = "Name";

            comboBox_RebarCoverType.DataSource = listRebarCoverTypeForComboBox;
            comboBox_RebarCoverType.DisplayMember = "Name";

            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "SquareColumnsReinforcementSettingsT4.xml";
            string assemblyPath = assemblyPathAll.Replace("SquareColumnsReinforcement.dll", fileName);
            if (File.Exists(assemblyPath))
            {
                comboBox_MainRebarTape.SelectedItem = listMainRebarTapeForComboBox
                    .FirstOrDefault(rbt => rbt.Name == squareColumnsReinforcementSettingsT4.MainRebarTape);
                comboBox_AdditionalMainRebarTape.SelectedItem = listAdditionalMainRebarTapeForComboBox
                    .FirstOrDefault(rbt => rbt.Name == squareColumnsReinforcementSettingsT4.AdditionalMainRebarTape);
                comboBox_MainStirrupRebarTape.SelectedItem = listMainStirrupRebarTapeForComboBox
                    .FirstOrDefault(rbt => rbt.Name == squareColumnsReinforcementSettingsT4.MainStirrupRebarTape);
                comboBox_AdditionalMainStirrupRebarTape.SelectedItem = listAdditionalMainStirrupRebarTapeForComboBox
                    .FirstOrDefault(rbt => rbt.Name == squareColumnsReinforcementSettingsT4.AdditionalMainStirrupRebarTape);
                comboBox_RebarCoverType.SelectedItem = listRebarCoverTypeForComboBox
                    .FirstOrDefault(rbt => rbt.Name == squareColumnsReinforcementSettingsT4.RebarCoverType);

                textBox_MainRebarOutletsLongLength.Text = squareColumnsReinforcementSettingsT4.MainRebarOutletsLongLength.ToString();
                textBox_MainRebarOutletsShortLength.Text = squareColumnsReinforcementSettingsT4.MainRebarOutletsShortLength.ToString();
                textBox_FloorThicknessAboveColumn.Text = squareColumnsReinforcementSettingsT4.FloorThicknessAboveColumn.ToString();
                textBox_StandardStirrupStep.Text = squareColumnsReinforcementSettingsT4.StandardStirrupStep.ToString();
                textBox_FrequentTopStirrupStep.Text = squareColumnsReinforcementSettingsT4.FrequentTopStirrupStep.ToString();
                textBox_FrequentBottomStirrupStep.Text = squareColumnsReinforcementSettingsT4.FrequentBottomStirrupStep.ToString();
                textBox_FrequentTopStirrupPlacementHeight.Text = squareColumnsReinforcementSettingsT4.FrequentTopStirrupPlacementHeight.ToString();
                textBox_FrequentBottomStirrupPlacementHeight.Text = squareColumnsReinforcementSettingsT4.FrequentBottomStirrupPlacementHeight.ToString();
                textBox_FirstBottomStirrupOffset.Text = squareColumnsReinforcementSettingsT4.FirstBottomStirrupOffset.ToString();
                textBox_FirstUpRebarOffset.Text = squareColumnsReinforcementSettingsT4.FirstUpRebarOffset.ToString();
                textBox_FirstDownRebarOffset.Text = squareColumnsReinforcementSettingsT4.FirstDownRebarOffset.ToString();
                textBox_FirstLeftRebarOffset.Text = squareColumnsReinforcementSettingsT4.FirstLeftRebarOffset.ToString();
                textBox_FirstRightRebarOffset.Text = squareColumnsReinforcementSettingsT4.FirstRightRebarOffset.ToString();
            }
        }
        private void MoveFieldsT5()
        {
            comboBox_MainRebarTape.Location = new System.Drawing.Point(20, 39);
            comboBox_MainStirrupRebarTape.Location = new System.Drawing.Point(307, 45);
            comboBox_RebarCoverType.Location = new System.Drawing.Point(69, 230);
            textBox_MainRebarOutletsLongLength.Location = new System.Drawing.Point(455, 146);
            textBox_MainRebarOutletsShortLength.Location = new System.Drawing.Point(718, 138);
            textBox_FloorThicknessAboveColumn.Location = new System.Drawing.Point(455, 237);
            textBox_StandardStirrupStep.Location = new System.Drawing.Point(692, 320);
            textBox_FrequentTopStirrupStep.Location = new System.Drawing.Point(692, 241);
            textBox_FrequentBottomStirrupStep.Location = new System.Drawing.Point(692, 416);
            textBox_FrequentTopStirrupPlacementHeight.Location = new System.Drawing.Point(455, 268);
            textBox_FrequentBottomStirrupPlacementHeight.Location = new System.Drawing.Point(455, 458);
            textBox_FirstBottomStirrupOffset.Location = new System.Drawing.Point(455, 490);

            RemoveAdditionalFields();

            comboBox_AdditionalMainRebarTape = new ComboBox();
            comboBox_AdditionalMainRebarTape.Name = "comboBox_AdditionalMainRebarTape";
            comboBox_AdditionalMainRebarTape.Location = new System.Drawing.Point(307, 237);
            comboBox_AdditionalMainRebarTape.Size = new Size(100, 21);
            comboBox_AdditionalMainRebarTape.Sorted = true;
            this.Controls.Add(comboBox_AdditionalMainRebarTape);
            comboBox_AdditionalMainRebarTape.BringToFront();

            comboBox_AdditionalMainStirrupRebarTape = new ComboBox();
            comboBox_AdditionalMainStirrupRebarTape.Name = "comboBox_AdditionalMainStirrupRebarTape";
            comboBox_AdditionalMainStirrupRebarTape.Location = new System.Drawing.Point(307, 110);
            comboBox_AdditionalMainStirrupRebarTape.Size = new Size(100, 21);
            comboBox_AdditionalMainStirrupRebarTape.Sorted = true;
            this.Controls.Add(comboBox_AdditionalMainStirrupRebarTape);
            comboBox_AdditionalMainStirrupRebarTape.BringToFront();

            textBox_FirstUpRebarOffset = new TextBox();
            textBox_FirstUpRebarOffset.Name = "textBox_FirstUpRebarOffset";
            textBox_FirstUpRebarOffset.Location = new System.Drawing.Point(44, 133);
            textBox_FirstUpRebarOffset.Size = new Size(35, 20);
            textBox_FirstUpRebarOffset.TextAlign = HorizontalAlignment.Center;
            this.Controls.Add(textBox_FirstUpRebarOffset);
            textBox_FirstUpRebarOffset.BringToFront();

            textBox_FirstDownRebarOffset = new TextBox();
            textBox_FirstDownRebarOffset.Name = "textBox_FirstDownRebarOffset";
            textBox_FirstDownRebarOffset.Location = new System.Drawing.Point(44, 157);
            textBox_FirstDownRebarOffset.Size = new Size(35, 20);
            textBox_FirstDownRebarOffset.TextAlign = HorizontalAlignment.Center;
            this.Controls.Add(textBox_FirstDownRebarOffset);
            textBox_FirstDownRebarOffset.BringToFront();

            textBox_FirstLeftRebarOffset = new TextBox();
            textBox_FirstLeftRebarOffset.Name = "textBox_FirstLeftRebarOffset";
            textBox_FirstLeftRebarOffset.Location = new System.Drawing.Point(170, 275);
            textBox_FirstLeftRebarOffset.Size = new Size(35, 20);
            textBox_FirstLeftRebarOffset.TextAlign = HorizontalAlignment.Center;
            this.Controls.Add(textBox_FirstLeftRebarOffset);
            textBox_FirstLeftRebarOffset.BringToFront();

            textBox_FirstRightRebarOffset = new TextBox();
            textBox_FirstRightRebarOffset.Name = "textBox_FirstRightRebarOffset";
            textBox_FirstRightRebarOffset.Location = new System.Drawing.Point(214, 275);
            textBox_FirstRightRebarOffset.Size = new Size(35, 20);
            textBox_FirstRightRebarOffset.TextAlign = HorizontalAlignment.Center;
            this.Controls.Add(textBox_FirstRightRebarOffset);
            textBox_FirstRightRebarOffset.BringToFront();

            comboBox_MainRebarTape.DataSource = listMainRebarTapeForComboBox;
            comboBox_MainRebarTape.DisplayMember = "Name";

            comboBox_AdditionalMainRebarTape.DataSource = listAdditionalMainRebarTapeForComboBox;
            comboBox_AdditionalMainRebarTape.DisplayMember = "Name";

            comboBox_MainStirrupRebarTape.DataSource = listMainStirrupRebarTapeForComboBox;
            comboBox_MainStirrupRebarTape.DisplayMember = "Name";

            comboBox_AdditionalMainStirrupRebarTape.DataSource = listAdditionalMainStirrupRebarTapeForComboBox;
            comboBox_AdditionalMainStirrupRebarTape.DisplayMember = "Name";

            comboBox_RebarCoverType.DataSource = listRebarCoverTypeForComboBox;
            comboBox_RebarCoverType.DisplayMember = "Name";

            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "SquareColumnsReinforcementSettingsT5.xml";
            string assemblyPath = assemblyPathAll.Replace("SquareColumnsReinforcement.dll", fileName);
            if (File.Exists(assemblyPath))
            {
                comboBox_MainRebarTape.SelectedItem = listMainRebarTapeForComboBox
                    .FirstOrDefault(rbt => rbt.Name == squareColumnsReinforcementSettingsT5.MainRebarTape);
                comboBox_AdditionalMainRebarTape.SelectedItem = listAdditionalMainRebarTapeForComboBox
                    .FirstOrDefault(rbt => rbt.Name == squareColumnsReinforcementSettingsT5.AdditionalMainRebarTape);
                comboBox_MainStirrupRebarTape.SelectedItem = listMainStirrupRebarTapeForComboBox
                    .FirstOrDefault(rbt => rbt.Name == squareColumnsReinforcementSettingsT5.MainStirrupRebarTape);
                comboBox_AdditionalMainStirrupRebarTape.SelectedItem = listAdditionalMainStirrupRebarTapeForComboBox
                    .FirstOrDefault(rbt => rbt.Name == squareColumnsReinforcementSettingsT5.AdditionalMainStirrupRebarTape);
                comboBox_RebarCoverType.SelectedItem = listRebarCoverTypeForComboBox
                    .FirstOrDefault(rbt => rbt.Name == squareColumnsReinforcementSettingsT5.RebarCoverType);

                textBox_MainRebarOutletsLongLength.Text = squareColumnsReinforcementSettingsT5.MainRebarOutletsLongLength.ToString();
                textBox_MainRebarOutletsShortLength.Text = squareColumnsReinforcementSettingsT5.MainRebarOutletsShortLength.ToString();
                textBox_FloorThicknessAboveColumn.Text = squareColumnsReinforcementSettingsT5.FloorThicknessAboveColumn.ToString();
                textBox_StandardStirrupStep.Text = squareColumnsReinforcementSettingsT5.StandardStirrupStep.ToString();
                textBox_FrequentTopStirrupStep.Text = squareColumnsReinforcementSettingsT5.FrequentTopStirrupStep.ToString();
                textBox_FrequentBottomStirrupStep.Text = squareColumnsReinforcementSettingsT5.FrequentBottomStirrupStep.ToString();
                textBox_FrequentTopStirrupPlacementHeight.Text = squareColumnsReinforcementSettingsT5.FrequentTopStirrupPlacementHeight.ToString();
                textBox_FrequentBottomStirrupPlacementHeight.Text = squareColumnsReinforcementSettingsT5.FrequentBottomStirrupPlacementHeight.ToString();
                textBox_FirstBottomStirrupOffset.Text = squareColumnsReinforcementSettingsT5.FirstBottomStirrupOffset.ToString();
                textBox_FirstUpRebarOffset.Text = squareColumnsReinforcementSettingsT5.FirstUpRebarOffset.ToString();
                textBox_FirstDownRebarOffset.Text = squareColumnsReinforcementSettingsT5.FirstDownRebarOffset.ToString();
                textBox_FirstLeftRebarOffset.Text = squareColumnsReinforcementSettingsT5.FirstLeftRebarOffset.ToString();
                textBox_FirstRightRebarOffset.Text = squareColumnsReinforcementSettingsT5.FirstRightRebarOffset.ToString();
            }
        }
        private void MoveFieldsT6()
        {
            comboBox_MainRebarTape.Location = new System.Drawing.Point(20, 39);
            comboBox_MainStirrupRebarTape.Location = new System.Drawing.Point(307, 45);
            comboBox_RebarCoverType.Location = new System.Drawing.Point(69, 230);
            textBox_MainRebarOutletsLongLength.Location = new System.Drawing.Point(455, 146);
            textBox_MainRebarOutletsShortLength.Location = new System.Drawing.Point(718, 138);
            textBox_FloorThicknessAboveColumn.Location = new System.Drawing.Point(455, 237);
            textBox_StandardStirrupStep.Location = new System.Drawing.Point(692, 320);
            textBox_FrequentTopStirrupStep.Location = new System.Drawing.Point(692, 241);
            textBox_FrequentBottomStirrupStep.Location = new System.Drawing.Point(692, 416);
            textBox_FrequentTopStirrupPlacementHeight.Location = new System.Drawing.Point(455, 268);
            textBox_FrequentBottomStirrupPlacementHeight.Location = new System.Drawing.Point(455, 458);
            textBox_FirstBottomStirrupOffset.Location = new System.Drawing.Point(455, 490);

            RemoveAdditionalFields();

            comboBox_AdditionalMainRebarTape = new ComboBox();
            comboBox_AdditionalMainRebarTape.Name = "comboBox_AdditionalMainRebarTape";
            comboBox_AdditionalMainRebarTape.Location = new System.Drawing.Point(307, 237);
            comboBox_AdditionalMainRebarTape.Size = new Size(100, 21);
            comboBox_AdditionalMainRebarTape.Sorted = true;
            this.Controls.Add(comboBox_AdditionalMainRebarTape);
            comboBox_AdditionalMainRebarTape.BringToFront();

            comboBox_AdditionalMainStirrupRebarTape = new ComboBox();
            comboBox_AdditionalMainStirrupRebarTape.Name = "comboBox_AdditionalMainStirrupRebarTape";
            comboBox_AdditionalMainStirrupRebarTape.Location = new System.Drawing.Point(307, 110);
            comboBox_AdditionalMainStirrupRebarTape.Size = new Size(100, 21);
            comboBox_AdditionalMainStirrupRebarTape.Sorted = true;
            this.Controls.Add(comboBox_AdditionalMainStirrupRebarTape);
            comboBox_AdditionalMainStirrupRebarTape.BringToFront();

            textBox_FirstUpRebarOffset = new TextBox();
            textBox_FirstUpRebarOffset.Name = "textBox_FirstUpRebarOffset";
            textBox_FirstUpRebarOffset.Location = new System.Drawing.Point(44, 133);
            textBox_FirstUpRebarOffset.Size = new Size(35, 20);
            textBox_FirstUpRebarOffset.TextAlign = HorizontalAlignment.Center;
            this.Controls.Add(textBox_FirstUpRebarOffset);
            textBox_FirstUpRebarOffset.BringToFront();

            textBox_FirstDownRebarOffset = new TextBox();
            textBox_FirstDownRebarOffset.Name = "textBox_FirstDownRebarOffset";
            textBox_FirstDownRebarOffset.Location = new System.Drawing.Point(44, 157);
            textBox_FirstDownRebarOffset.Size = new Size(35, 20);
            textBox_FirstDownRebarOffset.TextAlign = HorizontalAlignment.Center;
            this.Controls.Add(textBox_FirstDownRebarOffset);
            textBox_FirstDownRebarOffset.BringToFront();

            textBox_FirstLeftRebarOffset = new TextBox();
            textBox_FirstLeftRebarOffset.Name = "textBox_FirstLeftRebarOffset";
            textBox_FirstLeftRebarOffset.Location = new System.Drawing.Point(164, 278);
            textBox_FirstLeftRebarOffset.Size = new Size(35, 20);
            textBox_FirstLeftRebarOffset.TextAlign = HorizontalAlignment.Center;
            this.Controls.Add(textBox_FirstLeftRebarOffset);
            textBox_FirstLeftRebarOffset.BringToFront();

            textBox_FirstRightRebarOffset = new TextBox();
            textBox_FirstRightRebarOffset.Name = "textBox_FirstRightRebarOffset";
            textBox_FirstRightRebarOffset.Location = new System.Drawing.Point(219, 278);
            textBox_FirstRightRebarOffset.Size = new Size(35, 20);
            textBox_FirstRightRebarOffset.TextAlign = HorizontalAlignment.Center;
            this.Controls.Add(textBox_FirstRightRebarOffset);
            textBox_FirstRightRebarOffset.BringToFront();

            textBox_SecondUpRebarOffset = new TextBox();
            textBox_SecondUpRebarOffset.Name = "textBox_SecondUpRebarOffset";
            textBox_SecondUpRebarOffset.Location = new System.Drawing.Point(44, 103);
            textBox_SecondUpRebarOffset.Size = new Size(35, 20);
            textBox_SecondUpRebarOffset.TextAlign = HorizontalAlignment.Center;
            this.Controls.Add(textBox_SecondUpRebarOffset);
            textBox_SecondUpRebarOffset.BringToFront();

            textBox_SecondDownRebarOffset = new TextBox();
            textBox_SecondDownRebarOffset.Name = "textBox_SecondDownRebarOffset";
            textBox_SecondDownRebarOffset.Location = new System.Drawing.Point(44, 187);
            textBox_SecondDownRebarOffset.Size = new Size(35, 20);
            textBox_SecondDownRebarOffset.TextAlign = HorizontalAlignment.Center;
            this.Controls.Add(textBox_SecondDownRebarOffset);
            textBox_SecondDownRebarOffset.BringToFront();

            textBox_SecondLeftRebarOffset = new TextBox();
            textBox_SecondLeftRebarOffset.Name = "textBox_SecondLeftRebarOffset";
            textBox_SecondLeftRebarOffset.Location = new System.Drawing.Point(123, 278);
            textBox_SecondLeftRebarOffset.Size = new Size(35, 20);
            textBox_SecondLeftRebarOffset.TextAlign = HorizontalAlignment.Center;
            this.Controls.Add(textBox_SecondLeftRebarOffset);
            textBox_SecondLeftRebarOffset.BringToFront();

            textBox_SecondRightRebarOffset = new TextBox();
            textBox_SecondRightRebarOffset.Name = "textBox_SecondRightRebarOffset";
            textBox_SecondRightRebarOffset.Location = new System.Drawing.Point(260, 278);
            textBox_SecondRightRebarOffset.Size = new Size(35, 20);
            textBox_SecondRightRebarOffset.TextAlign = HorizontalAlignment.Center;
            this.Controls.Add(textBox_SecondRightRebarOffset);
            textBox_SecondRightRebarOffset.BringToFront();

            comboBox_MainRebarTape.DataSource = listMainRebarTapeForComboBox;
            comboBox_MainRebarTape.DisplayMember = "Name";

            comboBox_AdditionalMainRebarTape.DataSource = listAdditionalMainRebarTapeForComboBox;
            comboBox_AdditionalMainRebarTape.DisplayMember = "Name";

            comboBox_MainStirrupRebarTape.DataSource = listMainStirrupRebarTapeForComboBox;
            comboBox_MainStirrupRebarTape.DisplayMember = "Name";

            comboBox_AdditionalMainStirrupRebarTape.DataSource = listAdditionalMainStirrupRebarTapeForComboBox;
            comboBox_AdditionalMainStirrupRebarTape.DisplayMember = "Name";

            comboBox_RebarCoverType.DataSource = listRebarCoverTypeForComboBox;
            comboBox_RebarCoverType.DisplayMember = "Name";

            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = "SquareColumnsReinforcementSettingsT6.xml";
            string assemblyPath = assemblyPathAll.Replace("SquareColumnsReinforcement.dll", fileName);
            if (File.Exists(assemblyPath))
            {
                comboBox_MainRebarTape.SelectedItem = listMainRebarTapeForComboBox
                    .FirstOrDefault(rbt => rbt.Name == squareColumnsReinforcementSettingsT6.MainRebarTape);
                comboBox_AdditionalMainRebarTape.SelectedItem = listAdditionalMainRebarTapeForComboBox
                    .FirstOrDefault(rbt => rbt.Name == squareColumnsReinforcementSettingsT6.AdditionalMainRebarTape);
                comboBox_MainStirrupRebarTape.SelectedItem = listMainStirrupRebarTapeForComboBox
                    .FirstOrDefault(rbt => rbt.Name == squareColumnsReinforcementSettingsT6.MainStirrupRebarTape);
                comboBox_AdditionalMainStirrupRebarTape.SelectedItem = listAdditionalMainStirrupRebarTapeForComboBox
                    .FirstOrDefault(rbt => rbt.Name == squareColumnsReinforcementSettingsT6.AdditionalMainStirrupRebarTape);
                comboBox_RebarCoverType.SelectedItem = listRebarCoverTypeForComboBox
                    .FirstOrDefault(rbt => rbt.Name == squareColumnsReinforcementSettingsT6.RebarCoverType);

                textBox_MainRebarOutletsLongLength.Text = squareColumnsReinforcementSettingsT6.MainRebarOutletsLongLength.ToString();
                textBox_MainRebarOutletsShortLength.Text = squareColumnsReinforcementSettingsT6.MainRebarOutletsShortLength.ToString();
                textBox_FloorThicknessAboveColumn.Text = squareColumnsReinforcementSettingsT6.FloorThicknessAboveColumn.ToString();
                textBox_StandardStirrupStep.Text = squareColumnsReinforcementSettingsT6.StandardStirrupStep.ToString();
                textBox_FrequentTopStirrupStep.Text = squareColumnsReinforcementSettingsT6.FrequentTopStirrupStep.ToString();
                textBox_FrequentBottomStirrupStep.Text = squareColumnsReinforcementSettingsT6.FrequentBottomStirrupStep.ToString();
                textBox_FrequentTopStirrupPlacementHeight.Text = squareColumnsReinforcementSettingsT6.FrequentTopStirrupPlacementHeight.ToString();
                textBox_FrequentBottomStirrupPlacementHeight.Text = squareColumnsReinforcementSettingsT6.FrequentBottomStirrupPlacementHeight.ToString();
                textBox_FirstBottomStirrupOffset.Text = squareColumnsReinforcementSettingsT6.FirstBottomStirrupOffset.ToString();
                textBox_FirstUpRebarOffset.Text = squareColumnsReinforcementSettingsT6.FirstUpRebarOffset.ToString();
                textBox_FirstDownRebarOffset.Text = squareColumnsReinforcementSettingsT6.FirstDownRebarOffset.ToString();
                textBox_FirstLeftRebarOffset.Text = squareColumnsReinforcementSettingsT6.FirstLeftRebarOffset.ToString();
                textBox_FirstRightRebarOffset.Text = squareColumnsReinforcementSettingsT6.FirstRightRebarOffset.ToString();
                textBox_SecondUpRebarOffset.Text = squareColumnsReinforcementSettingsT6.SecondUpRebarOffset.ToString();
                textBox_SecondDownRebarOffset.Text = squareColumnsReinforcementSettingsT6.SecondDownRebarOffset.ToString();
                textBox_SecondLeftRebarOffset.Text = squareColumnsReinforcementSettingsT6.SecondLeftRebarOffset.ToString();
                textBox_SecondRightRebarOffset.Text = squareColumnsReinforcementSettingsT6.SecondRightRebarOffset.ToString();
            }
        }

        private void RemoveAdditionalFields()
        {
            if (this.Controls.Contains(comboBox_AdditionalMainRebarTape))
            {
                this.Controls.Remove(comboBox_AdditionalMainRebarTape);
            }
            if (this.Controls.Contains(comboBox_AdditionalMainStirrupRebarTape))
            {
                this.Controls.Remove(comboBox_AdditionalMainStirrupRebarTape);
            }
            if (this.Controls.Contains(textBox_FirstUpRebarOffset))
            {
                this.Controls.Remove(textBox_FirstUpRebarOffset);
            }
            if (this.Controls.Contains(textBox_FirstDownRebarOffset))
            {
                this.Controls.Remove(textBox_FirstDownRebarOffset);
            }
            if (this.Controls.Contains(textBox_SecondUpRebarOffset))
            {
                this.Controls.Remove(textBox_SecondUpRebarOffset);
            }
            if (this.Controls.Contains(textBox_SecondDownRebarOffset))
            {
                this.Controls.Remove(textBox_SecondDownRebarOffset);
            }
            if (this.Controls.Contains(textBox_FirstLeftRebarOffset))
            {
                this.Controls.Remove(textBox_FirstLeftRebarOffset);
            }
            if (this.Controls.Contains(textBox_FirstRightRebarOffset))
            {
                this.Controls.Remove(textBox_FirstRightRebarOffset);
            }
            if (this.Controls.Contains(textBox_SecondLeftRebarOffset))
            {
                this.Controls.Remove(textBox_SecondLeftRebarOffset);
            }
            if (this.Controls.Contains(textBox_SecondRightRebarOffset))
            {
                this.Controls.Remove(textBox_SecondRightRebarOffset);
            }
        }
        private void GetListsForComboBoxes()
        {
            listMainRebarTapeForComboBox = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .ToList();

            listAdditionalMainRebarTapeForComboBox = new FilteredElementCollector(doc)
                    .OfClass(typeof(RebarBarType))
                    .Cast<RebarBarType>()
                    .ToList();

            listMainStirrupRebarTapeForComboBox = new FilteredElementCollector(doc)
                    .OfClass(typeof(RebarBarType))
                    .Cast<RebarBarType>()
                    .ToList();

            listAdditionalMainStirrupRebarTapeForComboBox = new FilteredElementCollector(doc)
                    .OfClass(typeof(RebarBarType))
                    .Cast<RebarBarType>()
                    .ToList();

            listRebarCoverTypeForComboBox = new FilteredElementCollector(doc)
                .OfClass(typeof(RebarCoverType))
                .Cast<RebarCoverType>()
                .ToList();
        }

        private void SaveSettings()
        {
            MainRebarTape = comboBox_MainRebarTape.SelectedItem as RebarBarType;
            if (comboBox_AdditionalMainRebarTape != null)
            {
                AdditionalMainRebarTape = comboBox_AdditionalMainRebarTape.SelectedItem as RebarBarType;
            }
            
            MainStirrupRebarTape = comboBox_MainStirrupRebarTape.SelectedItem as RebarBarType;

            if (comboBox_AdditionalMainStirrupRebarTape != null)
            {
                AdditionalMainStirrupRebarTape = comboBox_AdditionalMainStirrupRebarTape.SelectedItem as RebarBarType;
            }
            
            RebarCoverType = comboBox_RebarCoverType.SelectedItem as RebarCoverType;

            CheckedRebarOutletsTepeName = groupBox_RebarOutletsTepe.Controls.OfType<RadioButton>().FirstOrDefault(rb => rb.Checked).Name;

            СhangeSection = checkBox_СhangeSection.Checked;
            RebarDeepening = checkBox_RebarDeepening.Checked;
            BendIntoSlab = checkBox_BendIntoSlab.Checked;
            TransitionToOverlap = checkBox_TransitionToOverlap.Checked;

            double.TryParse(textBox_MainRebarOutletsLongLength.Text, out MainRebarOutletsLongLength);
            double.TryParse(textBox_MainRebarOutletsShortLength.Text, out MainRebarOutletsShortLength);
            double.TryParse(textBox_FloorThicknessAboveColumn.Text, out FloorThicknessAboveColumn);
            double.TryParse(textBox_StandardStirrupStep.Text, out StandardStirrupStep);
            double.TryParse(textBox_FrequentTopStirrupStep.Text, out FrequentTopStirrupStep);
            double.TryParse(textBox_FrequentBottomStirrupStep.Text, out FrequentBottomStirrupStep);
            double.TryParse(textBox_FrequentTopStirrupPlacementHeight.Text, out FrequentTopStirrupPlacementHeight);
            double.TryParse(textBox_FrequentBottomStirrupPlacementHeight.Text, out FrequentBottomStirrupPlacementHeight);
            double.TryParse(textBox_FirstBottomStirrupOffset.Text, out FirstBottomStirrupOffset);
            double.TryParse(textBox_BendIntoSlab.Text, out BendIntoSlabOffset);
            double.TryParse(textBox_СhangeSection.Text, out СhangeSectionOffset);
            double.TryParse(textBox_RebarDeepening.Text, out RebarDeepeningOffset);

            if (textBox_FirstUpRebarOffset != null)
            {
                double.TryParse(textBox_FirstUpRebarOffset.Text, out FirstUpRebarOffset);
            }
            if (textBox_SecondUpRebarOffset != null)
            {
                double.TryParse(textBox_SecondUpRebarOffset.Text, out SecondUpRebarOffset);
            }
            if (textBox_FirstDownRebarOffset != null)
            {
                double.TryParse(textBox_FirstDownRebarOffset.Text, out FirstDownRebarOffset);
            }
            if (textBox_SecondDownRebarOffset != null)
            {
                double.TryParse(textBox_SecondDownRebarOffset.Text, out SecondDownRebarOffset);
            }

            if (textBox_FirstLeftRebarOffset != null)
            {
                double.TryParse(textBox_FirstLeftRebarOffset.Text, out FirstLeftRebarOffset);
            }
            if (textBox_SecondLeftRebarOffset != null)
            {
                double.TryParse(textBox_SecondLeftRebarOffset.Text, out SecondLeftRebarOffset);
            }
            if (textBox_FirstRightRebarOffset != null)
            {
                double.TryParse(textBox_FirstRightRebarOffset.Text, out FirstRightRebarOffset);
            }
            if (textBox_SecondRightRebarOffset != null)
            {
                double.TryParse(textBox_SecondRightRebarOffset.Text, out SecondRightRebarOffset);
            }

            if(radioButton_T1.Checked)
            {
                squareColumnsReinforcementSettingsT1.MainRebarTape = MainRebarTape.Name;
                squareColumnsReinforcementSettingsT1.MainStirrupRebarTape = MainStirrupRebarTape.Name;
                squareColumnsReinforcementSettingsT1.RebarCoverType = RebarCoverType.Name;

                squareColumnsReinforcementSettingsT1.MainRebarOutletsLongLength = textBox_MainRebarOutletsLongLength.Text;
                squareColumnsReinforcementSettingsT1.MainRebarOutletsShortLength = textBox_MainRebarOutletsShortLength.Text;
                squareColumnsReinforcementSettingsT1.FloorThicknessAboveColumn = textBox_FloorThicknessAboveColumn.Text;
                squareColumnsReinforcementSettingsT1.StandardStirrupStep = textBox_StandardStirrupStep.Text;
                squareColumnsReinforcementSettingsT1.FrequentTopStirrupStep = textBox_FrequentTopStirrupStep.Text;
                squareColumnsReinforcementSettingsT1.FrequentBottomStirrupStep = textBox_FrequentBottomStirrupStep.Text;
                squareColumnsReinforcementSettingsT1.FrequentTopStirrupPlacementHeight = textBox_FrequentTopStirrupPlacementHeight.Text;
                squareColumnsReinforcementSettingsT1.FrequentBottomStirrupPlacementHeight = textBox_FrequentBottomStirrupPlacementHeight.Text;
                squareColumnsReinforcementSettingsT1.FirstBottomStirrupOffset = textBox_FirstBottomStirrupOffset.Text;
                squareColumnsReinforcementSettingsT1.SaveSettingsT1();
            }
            else if (radioButton_T2.Checked)
            {
                squareColumnsReinforcementSettingsT2.MainRebarTape = MainRebarTape.Name;
                squareColumnsReinforcementSettingsT2.AdditionalMainRebarTape = AdditionalMainRebarTape.Name;
                squareColumnsReinforcementSettingsT2.MainStirrupRebarTape = MainStirrupRebarTape.Name;
                squareColumnsReinforcementSettingsT2.RebarCoverType = RebarCoverType.Name;

                squareColumnsReinforcementSettingsT2.MainRebarOutletsLongLength = textBox_MainRebarOutletsLongLength.Text;
                squareColumnsReinforcementSettingsT2.MainRebarOutletsShortLength = textBox_MainRebarOutletsShortLength.Text;
                squareColumnsReinforcementSettingsT2.FloorThicknessAboveColumn = textBox_FloorThicknessAboveColumn.Text;
                squareColumnsReinforcementSettingsT2.StandardStirrupStep = textBox_StandardStirrupStep.Text;
                squareColumnsReinforcementSettingsT2.FrequentTopStirrupStep = textBox_FrequentTopStirrupStep.Text;
                squareColumnsReinforcementSettingsT2.FrequentBottomStirrupStep = textBox_FrequentBottomStirrupStep.Text;
                squareColumnsReinforcementSettingsT2.FrequentTopStirrupPlacementHeight = textBox_FrequentTopStirrupPlacementHeight.Text;
                squareColumnsReinforcementSettingsT2.FrequentBottomStirrupPlacementHeight = textBox_FrequentBottomStirrupPlacementHeight.Text;
                squareColumnsReinforcementSettingsT2.FirstBottomStirrupOffset = textBox_FirstBottomStirrupOffset.Text;
                squareColumnsReinforcementSettingsT2.SaveSettingsT2();
            }
            else if (radioButton_T3.Checked)
            {
                squareColumnsReinforcementSettingsT3.MainRebarTape = MainRebarTape.Name;
                squareColumnsReinforcementSettingsT3.AdditionalMainRebarTape = AdditionalMainRebarTape.Name;
                squareColumnsReinforcementSettingsT3.MainStirrupRebarTape = MainStirrupRebarTape.Name;
                squareColumnsReinforcementSettingsT3.AdditionalMainStirrupRebarTape = AdditionalMainStirrupRebarTape.Name;
                squareColumnsReinforcementSettingsT3.RebarCoverType = RebarCoverType.Name;

                squareColumnsReinforcementSettingsT3.MainRebarOutletsLongLength = textBox_MainRebarOutletsLongLength.Text;
                squareColumnsReinforcementSettingsT3.MainRebarOutletsShortLength = textBox_MainRebarOutletsShortLength.Text;
                squareColumnsReinforcementSettingsT3.FloorThicknessAboveColumn = textBox_FloorThicknessAboveColumn.Text;
                squareColumnsReinforcementSettingsT3.StandardStirrupStep = textBox_StandardStirrupStep.Text;
                squareColumnsReinforcementSettingsT3.FrequentTopStirrupStep = textBox_FrequentTopStirrupStep.Text;
                squareColumnsReinforcementSettingsT3.FrequentBottomStirrupStep = textBox_FrequentBottomStirrupStep.Text;
                squareColumnsReinforcementSettingsT3.FrequentTopStirrupPlacementHeight = textBox_FrequentTopStirrupPlacementHeight.Text;
                squareColumnsReinforcementSettingsT3.FrequentBottomStirrupPlacementHeight = textBox_FrequentBottomStirrupPlacementHeight.Text;
                squareColumnsReinforcementSettingsT3.FirstBottomStirrupOffset = textBox_FirstBottomStirrupOffset.Text;
                squareColumnsReinforcementSettingsT3.SaveSettingsT3();
            }
            else if (radioButton_T4.Checked)
            {
                squareColumnsReinforcementSettingsT4.MainRebarTape = MainRebarTape.Name;
                squareColumnsReinforcementSettingsT4.AdditionalMainRebarTape = AdditionalMainRebarTape.Name;
                squareColumnsReinforcementSettingsT4.MainStirrupRebarTape = MainStirrupRebarTape.Name;
                squareColumnsReinforcementSettingsT4.AdditionalMainStirrupRebarTape = AdditionalMainStirrupRebarTape.Name;
                squareColumnsReinforcementSettingsT4.RebarCoverType = RebarCoverType.Name;

                squareColumnsReinforcementSettingsT4.MainRebarOutletsLongLength = textBox_MainRebarOutletsLongLength.Text;
                squareColumnsReinforcementSettingsT4.MainRebarOutletsShortLength = textBox_MainRebarOutletsShortLength.Text;
                squareColumnsReinforcementSettingsT4.FloorThicknessAboveColumn = textBox_FloorThicknessAboveColumn.Text;
                squareColumnsReinforcementSettingsT4.StandardStirrupStep = textBox_StandardStirrupStep.Text;
                squareColumnsReinforcementSettingsT4.FrequentTopStirrupStep = textBox_FrequentTopStirrupStep.Text;
                squareColumnsReinforcementSettingsT4.FrequentBottomStirrupStep = textBox_FrequentBottomStirrupStep.Text;
                squareColumnsReinforcementSettingsT4.FrequentTopStirrupPlacementHeight = textBox_FrequentTopStirrupPlacementHeight.Text;
                squareColumnsReinforcementSettingsT4.FrequentBottomStirrupPlacementHeight = textBox_FrequentBottomStirrupPlacementHeight.Text;
                squareColumnsReinforcementSettingsT4.FirstBottomStirrupOffset = textBox_FirstBottomStirrupOffset.Text;
                squareColumnsReinforcementSettingsT4.FirstUpRebarOffset = textBox_FirstUpRebarOffset.Text;
                squareColumnsReinforcementSettingsT4.FirstDownRebarOffset = textBox_FirstDownRebarOffset.Text;
                squareColumnsReinforcementSettingsT4.FirstLeftRebarOffset = textBox_FirstLeftRebarOffset.Text;
                squareColumnsReinforcementSettingsT4.FirstRightRebarOffset = textBox_FirstRightRebarOffset.Text;
                squareColumnsReinforcementSettingsT4.SaveSettingsT4();
            }
            else if (radioButton_T5.Checked)
            {
                squareColumnsReinforcementSettingsT5.MainRebarTape = MainRebarTape.Name;
                squareColumnsReinforcementSettingsT5.AdditionalMainRebarTape = AdditionalMainRebarTape.Name;
                squareColumnsReinforcementSettingsT5.MainStirrupRebarTape = MainStirrupRebarTape.Name;
                squareColumnsReinforcementSettingsT5.AdditionalMainStirrupRebarTape = AdditionalMainStirrupRebarTape.Name;
                squareColumnsReinforcementSettingsT5.RebarCoverType = RebarCoverType.Name;

                squareColumnsReinforcementSettingsT5.MainRebarOutletsLongLength = textBox_MainRebarOutletsLongLength.Text;
                squareColumnsReinforcementSettingsT5.MainRebarOutletsShortLength = textBox_MainRebarOutletsShortLength.Text;
                squareColumnsReinforcementSettingsT5.FloorThicknessAboveColumn = textBox_FloorThicknessAboveColumn.Text;
                squareColumnsReinforcementSettingsT5.StandardStirrupStep = textBox_StandardStirrupStep.Text;
                squareColumnsReinforcementSettingsT5.FrequentTopStirrupStep = textBox_FrequentTopStirrupStep.Text;
                squareColumnsReinforcementSettingsT5.FrequentBottomStirrupStep = textBox_FrequentBottomStirrupStep.Text;
                squareColumnsReinforcementSettingsT5.FrequentTopStirrupPlacementHeight = textBox_FrequentTopStirrupPlacementHeight.Text;
                squareColumnsReinforcementSettingsT5.FrequentBottomStirrupPlacementHeight = textBox_FrequentBottomStirrupPlacementHeight.Text;
                squareColumnsReinforcementSettingsT5.FirstBottomStirrupOffset = textBox_FirstBottomStirrupOffset.Text;
                squareColumnsReinforcementSettingsT5.FirstUpRebarOffset = textBox_FirstUpRebarOffset.Text;
                squareColumnsReinforcementSettingsT5.FirstDownRebarOffset = textBox_FirstDownRebarOffset.Text;
                squareColumnsReinforcementSettingsT5.FirstLeftRebarOffset = textBox_FirstLeftRebarOffset.Text;
                squareColumnsReinforcementSettingsT5.FirstRightRebarOffset = textBox_FirstRightRebarOffset.Text;
                squareColumnsReinforcementSettingsT5.SaveSettingsT5();
            }
            else if (radioButton_T6.Checked)
            {
                squareColumnsReinforcementSettingsT6.MainRebarTape = MainRebarTape.Name;
                squareColumnsReinforcementSettingsT6.AdditionalMainRebarTape = AdditionalMainRebarTape.Name;
                squareColumnsReinforcementSettingsT6.MainStirrupRebarTape = MainStirrupRebarTape.Name;
                squareColumnsReinforcementSettingsT6.AdditionalMainStirrupRebarTape = AdditionalMainStirrupRebarTape.Name;
                squareColumnsReinforcementSettingsT6.RebarCoverType = RebarCoverType.Name;

                squareColumnsReinforcementSettingsT6.MainRebarOutletsLongLength = textBox_MainRebarOutletsLongLength.Text;
                squareColumnsReinforcementSettingsT6.MainRebarOutletsShortLength = textBox_MainRebarOutletsShortLength.Text;
                squareColumnsReinforcementSettingsT6.FloorThicknessAboveColumn = textBox_FloorThicknessAboveColumn.Text;
                squareColumnsReinforcementSettingsT6.StandardStirrupStep = textBox_StandardStirrupStep.Text;
                squareColumnsReinforcementSettingsT6.FrequentTopStirrupStep = textBox_FrequentTopStirrupStep.Text;
                squareColumnsReinforcementSettingsT6.FrequentBottomStirrupStep = textBox_FrequentBottomStirrupStep.Text;
                squareColumnsReinforcementSettingsT6.FrequentTopStirrupPlacementHeight = textBox_FrequentTopStirrupPlacementHeight.Text;
                squareColumnsReinforcementSettingsT6.FrequentBottomStirrupPlacementHeight = textBox_FrequentBottomStirrupPlacementHeight.Text;
                squareColumnsReinforcementSettingsT6.FirstBottomStirrupOffset = textBox_FirstBottomStirrupOffset.Text;
                squareColumnsReinforcementSettingsT6.FirstUpRebarOffset = textBox_FirstUpRebarOffset.Text;
                squareColumnsReinforcementSettingsT6.FirstDownRebarOffset = textBox_FirstDownRebarOffset.Text;
                squareColumnsReinforcementSettingsT6.FirstLeftRebarOffset = textBox_FirstLeftRebarOffset.Text;
                squareColumnsReinforcementSettingsT6.FirstRightRebarOffset = textBox_FirstRightRebarOffset.Text;
                squareColumnsReinforcementSettingsT6.SecondUpRebarOffset = textBox_SecondUpRebarOffset.Text;
                squareColumnsReinforcementSettingsT6.SecondDownRebarOffset = textBox_SecondDownRebarOffset.Text;
                squareColumnsReinforcementSettingsT6.SecondLeftRebarOffset = textBox_SecondLeftRebarOffset.Text;
                squareColumnsReinforcementSettingsT6.SecondRightRebarOffset = textBox_SecondRightRebarOffset.Text;
                squareColumnsReinforcementSettingsT6.SaveSettingsT6();
            }
        }
        private void GetSettingsFromXML()
        {
            string assemblyPathAll = System.Reflection.Assembly.GetExecutingAssembly().Location;

            squareColumnsReinforcementSettingsT1 = SquareColumnsReinforcementSettings.GetSettingsT1();
            string fileNameT1 = "SquareColumnsReinforcementSettingsT1.xml";
            string assemblyPathT1 = assemblyPathAll.Replace("SquareColumnsReinforcement.dll", fileNameT1);

            squareColumnsReinforcementSettingsT2 = SquareColumnsReinforcementSettings.GetSettingsT2();
            string fileNameT2 = "SquareColumnsReinforcementSettingsT2.xml";
            string assemblyPathT2 = assemblyPathAll.Replace("SquareColumnsReinforcement.dll", fileNameT2);

            squareColumnsReinforcementSettingsT3 = SquareColumnsReinforcementSettings.GetSettingsT3();
            string fileNameT3 = "SquareColumnsReinforcementSettingsT3.xml";
            string assemblyPathT3 = assemblyPathAll.Replace("SquareColumnsReinforcement.dll", fileNameT3);

            squareColumnsReinforcementSettingsT4 = SquareColumnsReinforcementSettings.GetSettingsT4();
            string fileNameT4 = "SquareColumnsReinforcementSettingsT4.xml";
            string assemblyPathT4 = assemblyPathAll.Replace("SquareColumnsReinforcement.dll", fileNameT4);

            squareColumnsReinforcementSettingsT5 = SquareColumnsReinforcementSettings.GetSettingsT5();
            string fileNameT5 = "SquareColumnsReinforcementSettingsT5.xml";
            string assemblyPathT5 = assemblyPathAll.Replace("SquareColumnsReinforcement.dll", fileNameT5);

            squareColumnsReinforcementSettingsT6 = SquareColumnsReinforcementSettings.GetSettingsT6();
            string fileNameT6 = "SquareColumnsReinforcementSettingsT6.xml";
            string assemblyPathT6 = assemblyPathAll.Replace("SquareColumnsReinforcement.dll", fileNameT6);
        }
    }
}
