using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SquareColumnsReinforcement
{
    public partial class SquareColumnsReinforcementWPF : Window
    {
        List<RebarBarType> RebarBarTypesList;
        List<RebarCoverType> RebarCoverTypesList;
        List<RebarShape> RebarShapeList;
        List<RebarHookType> RebarHookTypeList;
        List<Family> RebarConnectionsList;

        public string SelectedReinforcementTypeButtonName;
        public RebarBarType FirstMainBarTape;
        public RebarBarType SecondMainBarTape;
        public RebarBarType FirstStirrupBarTape;
        public RebarBarType SecondStirrupBarTape;
        public RebarCoverType ColumnRebarCoverType;

        public double FirstTopRebarOffset;
        public double FirstLowerRebarOffset;
        public double FirstLeftRebarOffset;
        public double FirstRightRebarOffset;

        public double SecondTopRebarOffset;
        public double SecondLowerRebarOffset;
        public double SecondLeftRebarOffset;
        public double SecondRightRebarOffset;

        public string RebarConnectionOptionName;
        public bool DeepenRebarChecked;
        public double DeepenRebar;

        public bool OverlapTransitionChecked;

        public double FirstRebarOutletsLength;
        public double SecondRebarOutletsLength;
        public double StandardStirrupStep;
        public double FrequentButtomStirrupStep;
        public double FrequentButtomStirrupPlacementHeight;
        public double FrequentTopStirrupStep;
        public double FrequentTopStirrupPlacementHeight;
        public double FloorThickness;
        public double FirstStirrupButtomOffset;

        public bool BendInSlabChecked;
        public double BendInSlab;

        public bool SectionChangeChecked;
        public double SectionChange;

        public RebarShape Form01;
        public RebarShape Form26;
        public RebarShape Form11;
        public RebarShape Form51;
        public RebarHookType RebarHookTypeForStirrup;

        public bool ProgressiveCollapseBarIntoSlabChecked;
        public RebarBarType ProgressiveCollapseBarTape;
        public double ProgressiveCollapseColumnCenterOffset;
        public double ProgressiveCollapseUpLength;
        public double ProgressiveCollapseBottomIndent;
        public double ProgressiveCollapseSideLength;

        public string MechanicalConnectionOptionName;
        public Family WeldedConnectionFamily;
        public Family CouplingConnectionFamily;

        private bool CheckFillingOk = true;

        SquareColumnsReinforcementSettingsT1 SquareColumnsReinforcementSettingsT1Item;
        SquareColumnsReinforcementSettingsT2 SquareColumnsReinforcementSettingsT2Item;
        SquareColumnsReinforcementSettingsT3 SquareColumnsReinforcementSettingsT3Item;
        SquareColumnsReinforcementSettingsT4 SquareColumnsReinforcementSettingsT4Item;
        SquareColumnsReinforcementSettingsT5 SquareColumnsReinforcementSettingsT5Item;
        SquareColumnsReinforcementSettingsT6 SquareColumnsReinforcementSettingsT6Item;

        public SquareColumnsReinforcementWPF(List<RebarBarType> rebarBarTypesList
            , List<RebarCoverType> rebarCoverTypesList
            , List<RebarShape> rebarShapeList
            , List<RebarHookType> rebarHookTypeList
            , List<Family> rebarConnectionsList)
        {
            RebarBarTypesList = rebarBarTypesList;
            RebarCoverTypesList = rebarCoverTypesList;
            RebarShapeList = rebarShapeList;
            RebarHookTypeList = rebarHookTypeList;
            RebarConnectionsList = rebarConnectionsList;

            SquareColumnsReinforcementSettingsT1Item = new SquareColumnsReinforcementSettingsT1().GetSettings();
            SquareColumnsReinforcementSettingsT2Item = new SquareColumnsReinforcementSettingsT2().GetSettings();
            SquareColumnsReinforcementSettingsT3Item = new SquareColumnsReinforcementSettingsT3().GetSettings();
            SquareColumnsReinforcementSettingsT4Item = new SquareColumnsReinforcementSettingsT4().GetSettings();
            SquareColumnsReinforcementSettingsT5Item = new SquareColumnsReinforcementSettingsT5().GetSettings();
            SquareColumnsReinforcementSettingsT6Item = new SquareColumnsReinforcementSettingsT6().GetSettings();

            InitializeComponent();

            comboBox_FirstMainBarTapes.ItemsSource = RebarBarTypesList;
            comboBox_FirstMainBarTapes.DisplayMemberPath = "Name";

            comboBox_SecondMainBarTapes.ItemsSource = RebarBarTypesList;
            comboBox_SecondMainBarTapes.DisplayMemberPath = "Name";

            comboBox_FirstStirrupBarTapes.ItemsSource = RebarBarTypesList;
            comboBox_FirstStirrupBarTapes.DisplayMemberPath = "Name";

            comboBox_SecondStirrupBarTapes.ItemsSource = RebarBarTypesList;
            comboBox_SecondStirrupBarTapes.DisplayMemberPath = "Name";

            comboBox_RebarCoverTypes.ItemsSource = RebarCoverTypesList;
            comboBox_RebarCoverTypes.DisplayMemberPath = "Name";

            comboBox_Form01.ItemsSource = RebarShapeList;
            comboBox_Form01.DisplayMemberPath = "Name";

            comboBox_Form26.ItemsSource = RebarShapeList;
            comboBox_Form26.DisplayMemberPath = "Name";

            comboBox_Form11.ItemsSource = RebarShapeList;
            comboBox_Form11.DisplayMemberPath = "Name";

            comboBox_Form51.ItemsSource = RebarShapeList;
            comboBox_Form51.DisplayMemberPath = "Name";

            comboBox_ProgressiveCollapseBarTapes.ItemsSource = RebarBarTypesList;
            comboBox_ProgressiveCollapseBarTapes.DisplayMemberPath = "Name";

            comboBox_RebarHookType.ItemsSource = RebarHookTypeList;
            comboBox_RebarHookType.DisplayMemberPath = "Name";

            comboBox_WeldedConnection.ItemsSource = RebarConnectionsList;
            comboBox_WeldedConnection.DisplayMemberPath = "Name";

            comboBox_CouplingConnection.ItemsSource = RebarConnectionsList;
            comboBox_CouplingConnection.DisplayMemberPath = "Name";

            button_Type1.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }
        private void btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            if(CheckFillingOk)
            {
                DialogResult = true;
                Close();
            }
            else 
            {
                this.Focus();
            }
        }
        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        private void SquareColumnsReinforcementWPF_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Space)
            {
                SaveSettings();
                if (CheckFillingOk)
                {
                    DialogResult = true;
                    Close();
                }
                else
                {
                    this.Focus();
                }
            }

            else if (e.Key == Key.Escape)
            {
                DialogResult = false;
                Close();
            }
        }
        private void button_Type1_Click(object sender, RoutedEventArgs e)
        {
            SelectedReinforcementTypeButtonName = (sender as Button).Name;
            SetBorderForSelectedButton(sender);
            SetBorderForNonSelectedButtons(sender);
            image_Sections.Source = new BitmapImage(new Uri("pack://application:,,,/SquareColumnsReinforcement;component/Resources/Sections/Type1_Section.png"));
            image_3D.Source = new BitmapImage(new Uri("pack://application:,,,/SquareColumnsReinforcement;component/Resources/3D/Type1_3D.png"));

            //Типы арматуры в сечении
            comboBox_FirstMainBarTapes.Visibility = System.Windows.Visibility.Visible;
            comboBox_FirstMainBarTapes.Margin = new Thickness(30, 34, 0, 0);

            comboBox_SecondMainBarTapes.Visibility = System.Windows.Visibility.Hidden;
            comboBox_SecondMainBarTapes.Margin = new Thickness(30, 34, 0, 0);

            comboBox_FirstStirrupBarTapes.Visibility = System.Windows.Visibility.Visible;
            comboBox_FirstStirrupBarTapes.Margin = new Thickness(300, 161, 0, 0);

            comboBox_SecondStirrupBarTapes.Visibility = System.Windows.Visibility.Hidden;
            comboBox_SecondStirrupBarTapes.Margin = new Thickness(300, 161, 0, 0);

            comboBox_RebarCoverTypes.Visibility = System.Windows.Visibility.Visible;
            comboBox_RebarCoverTypes.Margin = new Thickness(70, 204, 0, 0);

            //Смещение стержней в сечении
            textBox_FirstTopRebarOffset.Visibility = System.Windows.Visibility.Hidden;
            textBox_FirstTopRebarOffset.Margin = new Thickness(55, 110, 0, 0);

            textBox_FirstLowerRebarOffset.Visibility = System.Windows.Visibility.Hidden;
            textBox_FirstLowerRebarOffset.Margin = new Thickness(55, 144, 0, 0);

            textBox_FirstLeftRebarOffset.Visibility = System.Windows.Visibility.Hidden;
            textBox_FirstLeftRebarOffset.Margin = new Thickness(168, 245, 0, 0);

            textBox_FirstRightRebarOffset.Visibility = System.Windows.Visibility.Hidden;
            textBox_FirstRightRebarOffset.Margin = new Thickness(216, 245, 0, 0);


            textBox_SecondTopRebarOffset.Visibility = System.Windows.Visibility.Hidden;
            textBox_SecondTopRebarOffset.Margin = new Thickness(55, 0, 0, 87);

            textBox_SecondLowerRebarOffset.Visibility = System.Windows.Visibility.Hidden;
            textBox_SecondLowerRebarOffset.Margin = new Thickness(55, 87, 0, 0);

            textBox_SecondLeftRebarOffset.Visibility = System.Windows.Visibility.Hidden;
            textBox_SecondLeftRebarOffset.Margin = new Thickness(0, 245, 150, 0);

            textBox_SecondRightRebarOffset.Visibility = System.Windows.Visibility.Hidden;
            textBox_SecondRightRebarOffset.Margin = new Thickness(150, 245, 0, 0);

            //Смещение стержней на 3D
            textBox_FirstRebarOutletsLength.Margin = new Thickness(0, 0, 280, 256);
            textBox_SecondRebarOutletsLength.Margin = new Thickness(250, 0, 0, 276);
            textBox_StandardStirrupStep.Margin = new Thickness(175, 107, 0, 0);
            textBox_FrequentButtomStirrupStep.Margin = new Thickness(175, 312, 0, 0);
            textBox_FrequentButtomStirrupPlacementHeight.Margin = new Thickness(0, 390, 280, 0);
            textBox_FrequentTopStirrupStep.Margin = new Thickness(175, 0, 0, 60);
            textBox_FrequentTopStirrupPlacementHeight.Margin = new Thickness(0, 1, 280, 0);
            textBox_FloorThickness.Margin = new Thickness(0, 0, 280, 64);
            textBox_FirstStirrupButtomOffset.Margin = new Thickness(0, 465, 280, 0);

            SetSavedSettingsT1();
        }
        private void button_Type2_Click(object sender, RoutedEventArgs e)
        {
            SelectedReinforcementTypeButtonName = (sender as Button).Name;
            SetBorderForSelectedButton(sender);
            SetBorderForNonSelectedButtons(sender);
            image_Sections.Source = new BitmapImage(new Uri("pack://application:,,,/SquareColumnsReinforcement;component/Resources/Sections/Type2_Section.png"));
            image_3D.Source = new BitmapImage(new Uri("pack://application:,,,/SquareColumnsReinforcement;component/Resources/3D/Type2_3D.png"));

            //Типы арматуры в сечении
            comboBox_FirstMainBarTapes.Visibility = System.Windows.Visibility.Visible;
            comboBox_FirstMainBarTapes.Margin = new Thickness(30, 34, 0, 0);

            comboBox_SecondMainBarTapes.Visibility = System.Windows.Visibility.Visible;
            comboBox_SecondMainBarTapes.Margin = new Thickness(300, 209, 0, 0);

            comboBox_FirstStirrupBarTapes.Visibility = System.Windows.Visibility.Visible;
            comboBox_FirstStirrupBarTapes.Margin = new Thickness(300, 89, 0, 0);

            comboBox_SecondStirrupBarTapes.Visibility = System.Windows.Visibility.Hidden;
            comboBox_SecondStirrupBarTapes.Margin = new Thickness(300, 89, 0, 0);

            comboBox_RebarCoverTypes.Visibility = System.Windows.Visibility.Visible;
            comboBox_RebarCoverTypes.Margin = new Thickness(70, 204, 0, 0);

            //Смещение стержней в сечении
            textBox_FirstTopRebarOffset.Visibility = System.Windows.Visibility.Hidden;
            textBox_FirstTopRebarOffset.Margin = new Thickness(55, 110, 0, 0);

            textBox_FirstLowerRebarOffset.Visibility = System.Windows.Visibility.Hidden;
            textBox_FirstLowerRebarOffset.Margin = new Thickness(55, 144, 0, 0);

            textBox_FirstLeftRebarOffset.Visibility = System.Windows.Visibility.Hidden;
            textBox_FirstLeftRebarOffset.Margin = new Thickness(168, 245, 0, 0);

            textBox_FirstRightRebarOffset.Visibility = System.Windows.Visibility.Hidden;
            textBox_FirstRightRebarOffset.Margin = new Thickness(216, 245, 0, 0);


            textBox_SecondTopRebarOffset.Visibility = System.Windows.Visibility.Hidden;
            textBox_SecondTopRebarOffset.Margin = new Thickness(55, 0, 0, 87);

            textBox_SecondLowerRebarOffset.Visibility = System.Windows.Visibility.Hidden;
            textBox_SecondLowerRebarOffset.Margin = new Thickness(55, 87, 0, 0);

            textBox_SecondLeftRebarOffset.Visibility = System.Windows.Visibility.Hidden;
            textBox_SecondLeftRebarOffset.Margin = new Thickness(0, 245, 150, 0);

            textBox_SecondRightRebarOffset.Visibility = System.Windows.Visibility.Hidden;
            textBox_SecondRightRebarOffset.Margin = new Thickness(150, 245, 0, 0);

            //Смещение стержней на 3D
            textBox_FirstRebarOutletsLength.Margin = new Thickness(0, 0, 280, 256);
            textBox_SecondRebarOutletsLength.Margin = new Thickness(250, 0, 0, 276);
            textBox_StandardStirrupStep.Margin = new Thickness(175, 107, 0, 0);
            textBox_FrequentButtomStirrupStep.Margin = new Thickness(175, 312, 0, 0);
            textBox_FrequentButtomStirrupPlacementHeight.Margin = new Thickness(0, 390, 280, 0);
            textBox_FrequentTopStirrupStep.Margin = new Thickness(175, 0, 0, 60);
            textBox_FrequentTopStirrupPlacementHeight.Margin = new Thickness(0, 1, 280, 0);
            textBox_FloorThickness.Margin = new Thickness(0, 0, 280, 64);
            textBox_FirstStirrupButtomOffset.Margin = new Thickness(0, 465, 280, 0);

            SetSavedSettingsT2();
        }
        private void button_Type3_Click(object sender, RoutedEventArgs e)
        {
            SelectedReinforcementTypeButtonName = (sender as Button).Name;
            SetBorderForSelectedButton(sender);
            SetBorderForNonSelectedButtons(sender);
            image_Sections.Source = new BitmapImage(new Uri("pack://application:,,,/SquareColumnsReinforcement;component/Resources/Sections/Type3_Section.png"));
            image_3D.Source = new BitmapImage(new Uri("pack://application:,,,/SquareColumnsReinforcement;component/Resources/3D/Type3_3D.png"));

            //Типы арматуры в сечении
            comboBox_FirstMainBarTapes.Visibility = System.Windows.Visibility.Visible;
            comboBox_FirstMainBarTapes.Margin = new Thickness(30, 34, 0, 0);

            comboBox_SecondMainBarTapes.Visibility = System.Windows.Visibility.Visible;
            comboBox_SecondMainBarTapes.Margin = new Thickness(300, 209, 0, 0);

            comboBox_FirstStirrupBarTapes.Visibility = System.Windows.Visibility.Visible;
            comboBox_FirstStirrupBarTapes.Margin = new Thickness(300, 40, 0, 0);

            comboBox_SecondStirrupBarTapes.Visibility = System.Windows.Visibility.Visible;
            comboBox_SecondStirrupBarTapes.Margin = new Thickness(300, 98, 0, 0);

            comboBox_RebarCoverTypes.Visibility = System.Windows.Visibility.Visible;
            comboBox_RebarCoverTypes.Margin = new Thickness(70, 204, 0, 0);

            //Смещение стержней в сечении
            textBox_FirstTopRebarOffset.Visibility = System.Windows.Visibility.Hidden;
            textBox_FirstTopRebarOffset.Margin = new Thickness(55, 110, 0, 0);

            textBox_FirstLowerRebarOffset.Visibility = System.Windows.Visibility.Hidden;
            textBox_FirstLowerRebarOffset.Margin = new Thickness(55, 144, 0, 0);

            textBox_FirstLeftRebarOffset.Visibility = System.Windows.Visibility.Hidden;
            textBox_FirstLeftRebarOffset.Margin = new Thickness(168, 245, 0, 0);

            textBox_FirstRightRebarOffset.Visibility = System.Windows.Visibility.Hidden;
            textBox_FirstRightRebarOffset.Margin = new Thickness(216, 245, 0, 0);


            textBox_SecondTopRebarOffset.Visibility = System.Windows.Visibility.Hidden;
            textBox_SecondTopRebarOffset.Margin = new Thickness(55, 0, 0, 87);

            textBox_SecondLowerRebarOffset.Visibility = System.Windows.Visibility.Hidden;
            textBox_SecondLowerRebarOffset.Margin = new Thickness(55, 87, 0, 0);

            textBox_SecondLeftRebarOffset.Visibility = System.Windows.Visibility.Hidden;
            textBox_SecondLeftRebarOffset.Margin = new Thickness(0, 245, 150, 0);

            textBox_SecondRightRebarOffset.Visibility = System.Windows.Visibility.Hidden;
            textBox_SecondRightRebarOffset.Margin = new Thickness(150, 245, 0, 0);

            //Смещение стержней на 3D
            textBox_FirstRebarOutletsLength.Margin = new Thickness(0, 0, 280, 256);
            textBox_SecondRebarOutletsLength.Margin = new Thickness(250, 0, 0, 276);
            textBox_StandardStirrupStep.Margin = new Thickness(175, 107, 0, 0);
            textBox_FrequentButtomStirrupStep.Margin = new Thickness(175, 312, 0, 0);
            textBox_FrequentButtomStirrupPlacementHeight.Margin = new Thickness(0, 390, 280, 0);
            textBox_FrequentTopStirrupStep.Margin = new Thickness(175, 0, 0, 60);
            textBox_FrequentTopStirrupPlacementHeight.Margin = new Thickness(0, 1, 280, 0);
            textBox_FloorThickness.Margin = new Thickness(0, 0, 280, 64);
            textBox_FirstStirrupButtomOffset.Margin = new Thickness(0, 465, 280, 0);

            SetSavedSettingsT3();
        }
        private void button_Type4_Click(object sender, RoutedEventArgs e)
        {
            SelectedReinforcementTypeButtonName = (sender as Button).Name;
            SetBorderForSelectedButton(sender);
            SetBorderForNonSelectedButtons(sender);
            image_Sections.Source = new BitmapImage(new Uri("pack://application:,,,/SquareColumnsReinforcement;component/Resources/Sections/Type4_Section.png"));
            image_3D.Source = new BitmapImage(new Uri("pack://application:,,,/SquareColumnsReinforcement;component/Resources/3D/Type4_3D.png"));

            //Типы арматуры в сечении
            comboBox_FirstMainBarTapes.Visibility = System.Windows.Visibility.Visible;
            comboBox_FirstMainBarTapes.Margin = new Thickness(30, 34, 0, 0);

            comboBox_SecondMainBarTapes.Visibility = System.Windows.Visibility.Visible;
            comboBox_SecondMainBarTapes.Margin = new Thickness(300, 209, 0, 0);

            comboBox_FirstStirrupBarTapes.Visibility = System.Windows.Visibility.Visible;
            comboBox_FirstStirrupBarTapes.Margin = new Thickness(300, 40, 0, 0);

            comboBox_SecondStirrupBarTapes.Visibility = System.Windows.Visibility.Visible;
            comboBox_SecondStirrupBarTapes.Margin = new Thickness(300, 98, 0, 0);

            comboBox_RebarCoverTypes.Visibility = System.Windows.Visibility.Visible;
            comboBox_RebarCoverTypes.Margin = new Thickness(70, 204, 0, 0);

            //Смещение стержней в сечении
            textBox_FirstTopRebarOffset.Visibility = System.Windows.Visibility.Visible;
            textBox_FirstTopRebarOffset.Margin = new Thickness(55, 0, 0, 34);

            textBox_FirstLowerRebarOffset.Visibility = System.Windows.Visibility.Visible;
            textBox_FirstLowerRebarOffset.Margin = new Thickness(55, 34, 0, 0);

            textBox_FirstLeftRebarOffset.Visibility = System.Windows.Visibility.Visible;
            textBox_FirstLeftRebarOffset.Margin = new Thickness(0, 245, 48, 0);

            textBox_FirstRightRebarOffset.Visibility = System.Windows.Visibility.Visible;
            textBox_FirstRightRebarOffset.Margin = new Thickness(48, 245, 0, 0);


            textBox_SecondTopRebarOffset.Visibility = System.Windows.Visibility.Hidden;
            textBox_SecondTopRebarOffset.Margin = new Thickness(55, 0, 0, 87);

            textBox_SecondLowerRebarOffset.Visibility = System.Windows.Visibility.Hidden;
            textBox_SecondLowerRebarOffset.Margin = new Thickness(55, 87, 0, 0);

            textBox_SecondLeftRebarOffset.Visibility = System.Windows.Visibility.Hidden;
            textBox_SecondLeftRebarOffset.Margin = new Thickness(0, 245, 150, 0);

            textBox_SecondRightRebarOffset.Visibility = System.Windows.Visibility.Hidden;
            textBox_SecondRightRebarOffset.Margin = new Thickness(150, 245, 0, 0);

            //Смещение стержней на 3D
            textBox_FirstRebarOutletsLength.Margin = new Thickness(0, 0, 280, 256);
            textBox_SecondRebarOutletsLength.Margin = new Thickness(250, 0, 0, 275);
            textBox_StandardStirrupStep.Margin = new Thickness(190, 111, 0, 0);
            textBox_FrequentButtomStirrupStep.Margin = new Thickness(190, 316, 0, 0);
            textBox_FrequentButtomStirrupPlacementHeight.Margin = new Thickness(0, 403, 280, 0);
            textBox_FrequentTopStirrupStep.Margin = new Thickness(190, 0, 0, 56);
            textBox_FrequentTopStirrupPlacementHeight.Margin = new Thickness(0, 1, 280, 0);
            textBox_FloorThickness.Margin = new Thickness(0, 0, 280, 64);
            textBox_FirstStirrupButtomOffset.Margin = new Thickness(0, 472, 280, 0);

            SetSavedSettingsT4();
        }
        private void button_Type5_Click(object sender, RoutedEventArgs e)
        {
            SelectedReinforcementTypeButtonName = (sender as Button).Name;
            SetBorderForSelectedButton(sender);
            SetBorderForNonSelectedButtons(sender);
            image_Sections.Source = new BitmapImage(new Uri("pack://application:,,,/SquareColumnsReinforcement;component/Resources/Sections/Type5_Section.png"));
            image_3D.Source = new BitmapImage(new Uri("pack://application:,,,/SquareColumnsReinforcement;component/Resources/3D/Type5_3D.png"));

            //Типы арматуры в сечении
            comboBox_FirstMainBarTapes.Visibility = System.Windows.Visibility.Visible;
            comboBox_FirstMainBarTapes.Margin = new Thickness(30, 34, 0, 0);

            comboBox_SecondMainBarTapes.Visibility = System.Windows.Visibility.Visible;
            comboBox_SecondMainBarTapes.Margin = new Thickness(300, 209, 0, 0);

            comboBox_FirstStirrupBarTapes.Visibility = System.Windows.Visibility.Visible;
            comboBox_FirstStirrupBarTapes.Margin = new Thickness(300, 40, 0, 0);

            comboBox_SecondStirrupBarTapes.Visibility = System.Windows.Visibility.Visible;
            comboBox_SecondStirrupBarTapes.Margin = new Thickness(300, 98, 0, 0);

            comboBox_RebarCoverTypes.Visibility = System.Windows.Visibility.Visible;
            comboBox_RebarCoverTypes.Margin = new Thickness(70, 204, 0, 0);

            //Смещение стержней в сечении
            textBox_FirstTopRebarOffset.Visibility = System.Windows.Visibility.Visible;
            textBox_FirstTopRebarOffset.Margin = new Thickness(55, 0, 0, 34);

            textBox_FirstLowerRebarOffset.Visibility = System.Windows.Visibility.Visible;
            textBox_FirstLowerRebarOffset.Margin = new Thickness(55, 34, 0, 0);

            textBox_FirstLeftRebarOffset.Visibility = System.Windows.Visibility.Visible;
            textBox_FirstLeftRebarOffset.Margin = new Thickness(0, 245, 48, 0);

            textBox_FirstRightRebarOffset.Visibility = System.Windows.Visibility.Visible;
            textBox_FirstRightRebarOffset.Margin = new Thickness(48, 245, 0, 0);


            textBox_SecondTopRebarOffset.Visibility = System.Windows.Visibility.Hidden;
            textBox_SecondTopRebarOffset.Margin = new Thickness(55, 0, 0, 87);

            textBox_SecondLowerRebarOffset.Visibility = System.Windows.Visibility.Hidden;
            textBox_SecondLowerRebarOffset.Margin = new Thickness(55, 87, 0, 0);

            textBox_SecondLeftRebarOffset.Visibility = System.Windows.Visibility.Hidden;
            textBox_SecondLeftRebarOffset.Margin = new Thickness(0, 245, 150, 0);

            textBox_SecondRightRebarOffset.Visibility = System.Windows.Visibility.Hidden;
            textBox_SecondRightRebarOffset.Margin = new Thickness(150, 245, 0, 0);

            //Смещение стержней на 3D
            textBox_FirstRebarOutletsLength.Margin = new Thickness(0, 0, 280, 256);
            textBox_SecondRebarOutletsLength.Margin = new Thickness(250, 0, 0, 275);
            textBox_StandardStirrupStep.Margin = new Thickness(190, 111, 0, 0);
            textBox_FrequentButtomStirrupStep.Margin = new Thickness(190, 316, 0, 0);
            textBox_FrequentButtomStirrupPlacementHeight.Margin = new Thickness(0, 403, 280, 0);
            textBox_FrequentTopStirrupStep.Margin = new Thickness(190, 0, 0, 56);
            textBox_FrequentTopStirrupPlacementHeight.Margin = new Thickness(0, 1, 280, 0);
            textBox_FloorThickness.Margin = new Thickness(0, 0, 280, 64);
            textBox_FirstStirrupButtomOffset.Margin = new Thickness(0, 472, 280, 0);

            SetSavedSettingsT5();
        }
        private void button_Type6_Click(object sender, RoutedEventArgs e)
        {
            SelectedReinforcementTypeButtonName = (sender as Button).Name;
            SetBorderForSelectedButton(sender);
            SetBorderForNonSelectedButtons(sender);
            image_Sections.Source = new BitmapImage(new Uri("pack://application:,,,/SquareColumnsReinforcement;component/Resources/Sections/Type6_Section.png"));
            image_3D.Source = new BitmapImage(new Uri("pack://application:,,,/SquareColumnsReinforcement;component/Resources/3D/Type6_3D.png"));

            //Типы арматуры в сечении
            comboBox_FirstMainBarTapes.Visibility = System.Windows.Visibility.Visible;
            comboBox_FirstMainBarTapes.Margin = new Thickness(30, 34, 0, 0);

            comboBox_SecondMainBarTapes.Visibility = System.Windows.Visibility.Visible;
            comboBox_SecondMainBarTapes.Margin = new Thickness(300, 209, 0, 0);

            comboBox_FirstStirrupBarTapes.Visibility = System.Windows.Visibility.Visible;
            comboBox_FirstStirrupBarTapes.Margin = new Thickness(300, 40, 0, 0);

            comboBox_SecondStirrupBarTapes.Visibility = System.Windows.Visibility.Visible;
            comboBox_SecondStirrupBarTapes.Margin = new Thickness(300, 98, 0, 0);

            comboBox_RebarCoverTypes.Visibility = System.Windows.Visibility.Visible;
            comboBox_RebarCoverTypes.Margin = new Thickness(70, 204, 0, 0);

            //Смещение стержней в сечении
            textBox_FirstTopRebarOffset.Visibility = System.Windows.Visibility.Visible;
            textBox_FirstTopRebarOffset.Margin = new Thickness(55, 0, 0, 29);

            textBox_FirstLowerRebarOffset.Visibility = System.Windows.Visibility.Visible;
            textBox_FirstLowerRebarOffset.Margin = new Thickness(55, 29, 0, 0);

            textBox_FirstLeftRebarOffset.Visibility = System.Windows.Visibility.Visible;
            textBox_FirstLeftRebarOffset.Margin = new Thickness(0, 245, 58, 0);

            textBox_FirstRightRebarOffset.Visibility = System.Windows.Visibility.Visible;
            textBox_FirstRightRebarOffset.Margin = new Thickness(58, 245, 0, 0);


            textBox_SecondTopRebarOffset.Visibility = System.Windows.Visibility.Visible;
            textBox_SecondTopRebarOffset.Margin = new Thickness(55, 0, 0, 87);

            textBox_SecondLowerRebarOffset.Visibility = System.Windows.Visibility.Visible;
            textBox_SecondLowerRebarOffset.Margin = new Thickness(55, 87, 0, 0);

            textBox_SecondLeftRebarOffset.Visibility = System.Windows.Visibility.Visible;
            textBox_SecondLeftRebarOffset.Margin = new Thickness(0, 245, 150, 0);

            textBox_SecondRightRebarOffset.Visibility = System.Windows.Visibility.Visible;
            textBox_SecondRightRebarOffset.Margin = new Thickness(150, 245, 0, 0);

            //Смещение стержней на 3D
            textBox_FirstRebarOutletsLength.Margin = new Thickness(0, 0, 280, 256);
            textBox_SecondRebarOutletsLength.Margin = new Thickness(250, 0, 0, 275);
            textBox_StandardStirrupStep.Margin = new Thickness(190, 111, 0, 0);
            textBox_FrequentButtomStirrupStep.Margin = new Thickness(190, 316, 0, 0);
            textBox_FrequentButtomStirrupPlacementHeight.Margin = new Thickness(0, 403, 280, 0);
            textBox_FrequentTopStirrupStep.Margin = new Thickness(190, 0, 0, 56);
            textBox_FrequentTopStirrupPlacementHeight.Margin = new Thickness(0, 1, 280, 0);
            textBox_FloorThickness.Margin = new Thickness(0, 0, 280, 64);
            textBox_FirstStirrupButtomOffset.Margin = new Thickness(0, 472, 280, 0);

            SetSavedSettingsT6();
        }
        private static void SetBorderForSelectedButton(object sender)
        {
            BrushConverter bc = new BrushConverter();
            (sender as Button).BorderThickness = new Thickness(3, 3, 3, 3);
        }
        private void SetBorderForNonSelectedButtons(object sender)
        {
            BrushConverter bc = new BrushConverter();
            IEnumerable<Button> buttonsSet = buttonsTypeGrid.Children.OfType<Button>()
                .Where(b => b.Name.StartsWith("button_Type"))
                .Where(b => b.Name != (sender as Button).Name);
            foreach (Button btn in buttonsSet)
            {
                btn.BorderThickness = new Thickness(1, 1, 1, 1);
            }
        }
        private void SaveSettings()
        {
            //Проверка выбора форм стержней для всех типов
            Form01 = comboBox_Form01.SelectedItem as RebarShape;
            if (Form01 == null)
            {
                CheckFillingOk = false;
                TaskDialog.Show("Revit", "Выберите форму арматуры для прямых стержней (Форма 01), чтобы продолжить работу!");
                return;
            }

            Form26 = comboBox_Form26.SelectedItem as RebarShape;
            if (Form26 == null)
            {
                CheckFillingOk = false;
                TaskDialog.Show("Revit", "Выберите форму арматуры для Z-образных стержней (Форма 26), чтобы продолжить работу!");
                return;
            }

            Form11 = comboBox_Form11.SelectedItem as RebarShape;
            if (Form11 == null)
            {
                CheckFillingOk = false;
                TaskDialog.Show("Revit", "Выберите форму арматуры для Г-образных стержней (Форма 11), чтобы продолжить работу!");
                return;
            }

            Form51 = comboBox_Form51.SelectedItem as RebarShape;
            if (Form51 == null)
            {
                CheckFillingOk = false;
                TaskDialog.Show("Revit", "Выберите форму арматуры для хомутов (Форма 51, 52 и т.д.), чтобы продолжить работу!");
                return;
            }

            RebarHookTypeForStirrup = comboBox_RebarHookType.SelectedItem as RebarHookType;
            if (RebarHookTypeForStirrup == null)
            {
                CheckFillingOk = false;
                TaskDialog.Show("Revit", "Выберите тип отгибов для хомута, чтобы продолжить работу!");
                return;
            }

            //Проверка заполнения полей в сечении для всех типов
            FirstMainBarTape = comboBox_FirstMainBarTapes.SelectedItem as RebarBarType;
            if(FirstMainBarTape == null)
            {
                CheckFillingOk = false;
                TaskDialog.Show("Revit","Выберите тип углового стержня, чтобы продолжить работу!");
                return;
            }

            FirstStirrupBarTape = comboBox_FirstStirrupBarTapes.SelectedItem as RebarBarType;
            if (FirstStirrupBarTape == null)
            {
                CheckFillingOk = false;
                TaskDialog.Show("Revit", "Выберите тип стержня основного хомута, чтобы продолжить работу!");
                return;
            }

            ColumnRebarCoverType = comboBox_RebarCoverTypes.SelectedItem as RebarCoverType;
            if (ColumnRebarCoverType == null)
            {
                CheckFillingOk = false;
                TaskDialog.Show("Revit", "Выберите защитный слой, чтобы продолжить работу!");
                return;
            }

            //Проверка заполнения полей на 3D для всех типов
            double.TryParse(textBox_FirstRebarOutletsLength.Text, out FirstRebarOutletsLength);
            if(string.IsNullOrEmpty(textBox_FirstRebarOutletsLength.Text))
            {
                CheckFillingOk = false;
                TaskDialog.Show("Revit", "Укажите размер удлиненного выпуска, чтобы продолжить работу!");
                return;
            }

            double.TryParse(textBox_SecondRebarOutletsLength.Text, out SecondRebarOutletsLength);
            if (string.IsNullOrEmpty(textBox_SecondRebarOutletsLength.Text))
            {
                CheckFillingOk = false;
                TaskDialog.Show("Revit", "Укажите размер укороченного выпуска, чтобы продолжить работу!");
                return;
            }

            double.TryParse(textBox_StandardStirrupStep.Text, out StandardStirrupStep);
            if (string.IsNullOrEmpty(textBox_StandardStirrupStep.Text))
            {
                CheckFillingOk = false;
                TaskDialog.Show("Revit", "Укажите основной шаг хомутов, чтобы продолжить работу!");
                return;
            }

            double.TryParse(textBox_FrequentButtomStirrupStep.Text, out FrequentButtomStirrupStep);
            if (string.IsNullOrEmpty(textBox_FrequentButtomStirrupStep.Text))
            {
                CheckFillingOk = false;
                TaskDialog.Show("Revit", "Укажите учащенный шаг хомутов снизу, чтобы продолжить работу!");
                return;
            }

            double.TryParse(textBox_FrequentButtomStirrupPlacementHeight.Text, out FrequentButtomStirrupPlacementHeight);
            if (string.IsNullOrEmpty(textBox_FrequentButtomStirrupPlacementHeight.Text))
            {
                CheckFillingOk = false;
                TaskDialog.Show("Revit", "Укажите высоту размещения хомутов с учащенным шагом снизу, чтобы продолжить работу!");
                return;
            }

            double.TryParse(textBox_FrequentTopStirrupStep.Text, out FrequentTopStirrupStep);
            if (string.IsNullOrEmpty(textBox_FrequentTopStirrupStep.Text))
            {
                CheckFillingOk = false;
                TaskDialog.Show("Revit", "Укажите учащенный шаг хомутов сверху, чтобы продолжить работу!");
                return;
            }

            double.TryParse(textBox_FrequentTopStirrupPlacementHeight.Text, out FrequentTopStirrupPlacementHeight);
            if (string.IsNullOrEmpty(textBox_FrequentTopStirrupPlacementHeight.Text))
            {
                CheckFillingOk = false;
                TaskDialog.Show("Revit", "Укажите высоту размещения хомутов с учащенным шагом сверху, чтобы продолжить работу!");
                return;
            }

            double.TryParse(textBox_FloorThickness.Text, out FloorThickness);
            if (string.IsNullOrEmpty(textBox_FloorThickness.Text))
            {
                CheckFillingOk = false;
                TaskDialog.Show("Revit", "Укажите толщину перекрытия над колонной, чтобы продолжить работу!");
                return;
            }

            double.TryParse(textBox_FirstStirrupButtomOffset.Text, out FirstStirrupButtomOffset);
            if (string.IsNullOrEmpty(textBox_FirstStirrupButtomOffset.Text))
            {
                CheckFillingOk = false;
                TaskDialog.Show("Revit", "Укажите отступ первого хомута снизу, чтобы продолжить работу!");
                return;
            }

            //Проверка заполнения полей для соединения арматуры
            RebarConnectionOptionName = (this.groupBox_RebarConnectionOptions.Content as System.Windows.Controls.Grid)
                .Children.OfType<RadioButton>()
                .FirstOrDefault(rb => rb.IsChecked.Value == true)
                .Name;

            DeepenRebarChecked = (bool)checkBox_DeepenRebar.IsChecked;
            double.TryParse(textBox_DeepenRebar.Text, out DeepenRebar);

            OverlapTransitionChecked = (bool)checkBox_OverlapTransition.IsChecked;
            WeldedConnectionFamily = comboBox_WeldedConnection.SelectedItem as Family;
            CouplingConnectionFamily = comboBox_CouplingConnection.SelectedItem as Family;

            MechanicalConnectionOptionName = (this.groupBox_MechanicalConnectionOptions.Content as System.Windows.Controls.Grid)
                .Children.OfType<RadioButton>()
                .FirstOrDefault(rb => rb.IsChecked.Value == true)
                .Name;

            if (RebarConnectionOptionName.Equals("radioButton_Overlap"))
            {
                if (DeepenRebarChecked)
                {
                    if(string.IsNullOrEmpty(textBox_DeepenRebar.Text))
                    {
                        CheckFillingOk = false;
                        TaskDialog.Show("Revit", "Укажите размер заглубления стержней, чтобы продолжить работу!");
                        return;
                    }
                }
            }
            else
            {
                if(MechanicalConnectionOptionName.Equals("radioButton_WeldedConnection"))
                {
                    if (WeldedConnectionFamily == null)
                    {
                        CheckFillingOk = false;
                        TaskDialog.Show("Revit", "Выберите семейство ванной сварки, чтобы продолжить работу!");
                        return;
                    }
                }
                else
                {
                    if (CouplingConnectionFamily == null)
                    {
                        CheckFillingOk = false;
                        TaskDialog.Show("Revit", "Выберите семейство муфты, чтобы продолжить работу!");
                        return;
                    }
                }
            }

            //Проверка дополнительных параметров
            BendInSlabChecked = (bool)checkBox_BendInSlab.IsChecked;
            double.TryParse(textBox_BendInSlab.Text, out BendInSlab);

            if (BendInSlabChecked)
            {
                if (string.IsNullOrEmpty(textBox_BendInSlab.Text))
                {
                    CheckFillingOk = false;
                    TaskDialog.Show("Revit", "Укажите отступ загиба выпусков от верха плиты, чтобы продолжить работу!");
                    return;
                }
            }    

            SectionChangeChecked = (bool)checkBox_SectionChange.IsChecked;
            double.TryParse(textBox_SectionChange.Text, out SectionChange);
            if (SectionChangeChecked)
            {
                if (string.IsNullOrEmpty(textBox_SectionChange.Text))
                {
                    CheckFillingOk = false;
                    TaskDialog.Show("Revit", "Укажите размер изменения сечения колонны, чтобы продолжить работу!");
                    return;
                }
                if(SectionChange > 50)
                {
                    CheckFillingOk = false;
                    TaskDialog.Show("Revit", "Укажите размер изменения сечения колонны не более 50мм, чтобы продолжить работу!");
                    return;
                }
            }

            //Проверка параметров прогрессирующего обрушения
            ProgressiveCollapseBarIntoSlabChecked = (bool)checkBox_ProgressiveCollapseBarIntoSlab.IsChecked;
            ProgressiveCollapseBarTape = comboBox_ProgressiveCollapseBarTapes.SelectedItem as RebarBarType;
            double.TryParse(textBox_ProgressiveCollapseColumnCenterOffset.Text, out ProgressiveCollapseColumnCenterOffset);
            double.TryParse(textBox_ProgressiveCollapseUpLength.Text, out ProgressiveCollapseUpLength);
            double.TryParse(textBox_ProgressiveCollapseBottomIndent.Text, out ProgressiveCollapseBottomIndent);
            double.TryParse(textBox_ProgressiveCollapseSideLength.Text, out ProgressiveCollapseSideLength);

            if (ProgressiveCollapseBarIntoSlabChecked)
            {
                if (ProgressiveCollapseBarTape == null)
                {
                    CheckFillingOk = false;
                    TaskDialog.Show("Revit", "Выберите тип дополнительного стержня компенсации прогрессирующего обрушения, чтобы продолжить работу!");
                    return;
                }

                if (string.IsNullOrEmpty(textBox_ProgressiveCollapseColumnCenterOffset.Text))
                {
                    CheckFillingOk = false;
                    TaskDialog.Show("Revit", "Укажите размер смещения дополнительного стержня компенсации прогрессирующего обрушения от центра колонны, чтобы продолжить работу!");
                    return;
                }

                if (string.IsNullOrEmpty(textBox_ProgressiveCollapseUpLength.Text))
                {
                    CheckFillingOk = false;
                    TaskDialog.Show("Revit", "Укажите размер дополнительного стержня компенсации прогрессирующего обрушения вверх, чтобы продолжить работу!");
                    return;
                }

                if (string.IsNullOrEmpty(textBox_ProgressiveCollapseBottomIndent.Text))
                {
                    CheckFillingOk = false;
                    TaskDialog.Show("Revit", "Укажите размер смещения дополнительного стержня компенсации прогрессирующего обрушения от низа колонны, чтобы продолжить работу!");
                    return;
                }

                if (string.IsNullOrEmpty(textBox_ProgressiveCollapseSideLength.Text))
                {
                    CheckFillingOk = false;
                    TaskDialog.Show("Revit", "Укажите размер дополнительного стержня компенсации прогрессирующего обрушения в сторону, чтобы продолжить работу!");
                    return;
                }
            }

            //Сохранение настроек
            if (SelectedReinforcementTypeButtonName == "button_Type1")
            {
                SquareColumnsReinforcementSettingsT1Item = new SquareColumnsReinforcementSettingsT1();

                SquareColumnsReinforcementSettingsT1Item.Form01Name = Form01.Name;
                SquareColumnsReinforcementSettingsT1Item.Form26Name = Form26.Name;
                SquareColumnsReinforcementSettingsT1Item.Form11Name = Form11.Name;
                SquareColumnsReinforcementSettingsT1Item.Form51Name = Form51.Name;
                SquareColumnsReinforcementSettingsT1Item.RebarHookTypeForStirrupName = RebarHookTypeForStirrup.Name;

                SquareColumnsReinforcementSettingsT1Item.FirstMainBarTapeName = FirstMainBarTape.Name;
                SquareColumnsReinforcementSettingsT1Item.FirstStirrupBarTapeName = FirstStirrupBarTape.Name;
                SquareColumnsReinforcementSettingsT1Item.ColumnRebarCoverTypeName = ColumnRebarCoverType.Name;

                SquareColumnsReinforcementSettingsT1Item.FirstRebarOutletsLength = FirstRebarOutletsLength;
                SquareColumnsReinforcementSettingsT1Item.SecondRebarOutletsLength = SecondRebarOutletsLength;
                SquareColumnsReinforcementSettingsT1Item.StandardStirrupStep = StandardStirrupStep;
                SquareColumnsReinforcementSettingsT1Item.FrequentButtomStirrupStep = FrequentButtomStirrupStep;
                SquareColumnsReinforcementSettingsT1Item.FrequentButtomStirrupPlacementHeight = FrequentButtomStirrupPlacementHeight;
                SquareColumnsReinforcementSettingsT1Item.FrequentTopStirrupStep = FrequentTopStirrupStep;
                SquareColumnsReinforcementSettingsT1Item.FrequentTopStirrupPlacementHeight = FrequentTopStirrupPlacementHeight;
                SquareColumnsReinforcementSettingsT1Item.FloorThickness = FloorThickness;
                SquareColumnsReinforcementSettingsT1Item.FirstStirrupButtomOffset = FirstStirrupButtomOffset;

                SquareColumnsReinforcementSettingsT1Item.RebarConnectionOptionName = RebarConnectionOptionName;
                SquareColumnsReinforcementSettingsT1Item.DeepenRebarChecked = DeepenRebarChecked;
                SquareColumnsReinforcementSettingsT1Item.DeepenRebar = DeepenRebar;

                SquareColumnsReinforcementSettingsT1Item.OverlapTransitionChecked = OverlapTransitionChecked;

                SquareColumnsReinforcementSettingsT1Item.MechanicalConnectionOptionName = MechanicalConnectionOptionName;
                if(WeldedConnectionFamily != null)
                {
                    SquareColumnsReinforcementSettingsT1Item.WeldedConnectionFamilyName = WeldedConnectionFamily.Name;
                }
                if(CouplingConnectionFamily != null)
                {
                    SquareColumnsReinforcementSettingsT1Item.CouplingConnectionFamilyName = CouplingConnectionFamily.Name;
                }
                
                SquareColumnsReinforcementSettingsT1Item.BendInSlabChecked = BendInSlabChecked;
                SquareColumnsReinforcementSettingsT1Item.BendInSlab = BendInSlab;

                SquareColumnsReinforcementSettingsT1Item.SectionChangeChecked = SectionChangeChecked;
                SquareColumnsReinforcementSettingsT1Item.SectionChange = SectionChange;

                SquareColumnsReinforcementSettingsT1Item.ProgressiveCollapseBarIntoSlabChecked = ProgressiveCollapseBarIntoSlabChecked;
                if(ProgressiveCollapseBarTape != null)
                {
                    SquareColumnsReinforcementSettingsT1Item.ProgressiveCollapseBarTapeName = ProgressiveCollapseBarTape.Name;
                }
                SquareColumnsReinforcementSettingsT1Item.ProgressiveCollapseColumnCenterOffset = ProgressiveCollapseColumnCenterOffset;
                SquareColumnsReinforcementSettingsT1Item.ProgressiveCollapseUpLength = ProgressiveCollapseUpLength;
                SquareColumnsReinforcementSettingsT1Item.ProgressiveCollapseBottomIndent = ProgressiveCollapseBottomIndent;
                SquareColumnsReinforcementSettingsT1Item.ProgressiveCollapseSideLength = ProgressiveCollapseSideLength;

                SquareColumnsReinforcementSettingsT1Item.SaveSettings();
            }

            if (SelectedReinforcementTypeButtonName != "button_Type1")
            {
                SecondMainBarTape = comboBox_SecondMainBarTapes.SelectedItem as RebarBarType;
                if (SecondMainBarTape == null)
                {
                    CheckFillingOk = false;
                    TaskDialog.Show("Revit", "Выберите тип для стержней по грани, чтобы продолжить работу!");
                    return;
                }
                if (SelectedReinforcementTypeButtonName == "button_Type2")
                {
                    SquareColumnsReinforcementSettingsT2Item = new SquareColumnsReinforcementSettingsT2();

                    SquareColumnsReinforcementSettingsT2Item.Form01Name = Form01.Name;
                    SquareColumnsReinforcementSettingsT2Item.Form26Name = Form26.Name;
                    SquareColumnsReinforcementSettingsT2Item.Form11Name = Form11.Name;
                    SquareColumnsReinforcementSettingsT2Item.Form51Name = Form51.Name;
                    SquareColumnsReinforcementSettingsT2Item.RebarHookTypeForStirrupName = RebarHookTypeForStirrup.Name;

                    SquareColumnsReinforcementSettingsT2Item.FirstMainBarTapeName = FirstMainBarTape.Name;
                    SquareColumnsReinforcementSettingsT2Item.FirstStirrupBarTapeName = FirstStirrupBarTape.Name;
                    SquareColumnsReinforcementSettingsT2Item.ColumnRebarCoverTypeName = ColumnRebarCoverType.Name;
                    SquareColumnsReinforcementSettingsT2Item.SecondMainBarTapeName = SecondMainBarTape.Name;

                    SquareColumnsReinforcementSettingsT2Item.FirstRebarOutletsLength = FirstRebarOutletsLength;
                    SquareColumnsReinforcementSettingsT2Item.SecondRebarOutletsLength = SecondRebarOutletsLength;
                    SquareColumnsReinforcementSettingsT2Item.StandardStirrupStep = StandardStirrupStep;
                    SquareColumnsReinforcementSettingsT2Item.FrequentButtomStirrupStep = FrequentButtomStirrupStep;
                    SquareColumnsReinforcementSettingsT2Item.FrequentButtomStirrupPlacementHeight = FrequentButtomStirrupPlacementHeight;
                    SquareColumnsReinforcementSettingsT2Item.FrequentTopStirrupStep = FrequentTopStirrupStep;
                    SquareColumnsReinforcementSettingsT2Item.FrequentTopStirrupPlacementHeight = FrequentTopStirrupPlacementHeight;
                    SquareColumnsReinforcementSettingsT2Item.FloorThickness = FloorThickness;
                    SquareColumnsReinforcementSettingsT2Item.FirstStirrupButtomOffset = FirstStirrupButtomOffset;

                    SquareColumnsReinforcementSettingsT2Item.RebarConnectionOptionName = RebarConnectionOptionName;
                    SquareColumnsReinforcementSettingsT2Item.DeepenRebarChecked = DeepenRebarChecked;
                    SquareColumnsReinforcementSettingsT2Item.DeepenRebar = DeepenRebar;

                    SquareColumnsReinforcementSettingsT2Item.OverlapTransitionChecked = OverlapTransitionChecked;

                    SquareColumnsReinforcementSettingsT2Item.MechanicalConnectionOptionName = MechanicalConnectionOptionName;
                    if (WeldedConnectionFamily != null)
                    {
                        SquareColumnsReinforcementSettingsT2Item.WeldedConnectionFamilyName = WeldedConnectionFamily.Name;
                    }
                    if (CouplingConnectionFamily != null)
                    {
                        SquareColumnsReinforcementSettingsT2Item.CouplingConnectionFamilyName = CouplingConnectionFamily.Name;
                    }

                    SquareColumnsReinforcementSettingsT2Item.BendInSlabChecked = BendInSlabChecked;
                    SquareColumnsReinforcementSettingsT2Item.BendInSlab = BendInSlab;

                    SquareColumnsReinforcementSettingsT2Item.SectionChangeChecked = SectionChangeChecked;
                    SquareColumnsReinforcementSettingsT2Item.SectionChange = SectionChange;

                    SquareColumnsReinforcementSettingsT2Item.ProgressiveCollapseBarIntoSlabChecked = ProgressiveCollapseBarIntoSlabChecked;
                    if (ProgressiveCollapseBarTape != null)
                    {
                        SquareColumnsReinforcementSettingsT2Item.ProgressiveCollapseBarTapeName = ProgressiveCollapseBarTape.Name;
                    }
                    SquareColumnsReinforcementSettingsT2Item.ProgressiveCollapseColumnCenterOffset = ProgressiveCollapseColumnCenterOffset;
                    SquareColumnsReinforcementSettingsT2Item.ProgressiveCollapseUpLength = ProgressiveCollapseUpLength;
                    SquareColumnsReinforcementSettingsT2Item.ProgressiveCollapseBottomIndent = ProgressiveCollapseBottomIndent;
                    SquareColumnsReinforcementSettingsT2Item.ProgressiveCollapseSideLength = ProgressiveCollapseSideLength;

                    SquareColumnsReinforcementSettingsT2Item.SaveSettings();
                }

                if (SelectedReinforcementTypeButtonName != "button_Type2")
                {
                    SecondStirrupBarTape = comboBox_SecondStirrupBarTapes.SelectedItem as RebarBarType;
                    if (SecondStirrupBarTape == null)
                    {
                        CheckFillingOk = false;
                        TaskDialog.Show("Revit", "Выберите тип стержня дополнительного хомута, чтобы продолжить работу!");
                        return;
                    }

                    if (SelectedReinforcementTypeButtonName == "button_Type3")
                    {
                        SquareColumnsReinforcementSettingsT3Item = new SquareColumnsReinforcementSettingsT3();

                        SquareColumnsReinforcementSettingsT3Item.Form01Name = Form01.Name;
                        SquareColumnsReinforcementSettingsT3Item.Form26Name = Form26.Name;
                        SquareColumnsReinforcementSettingsT3Item.Form11Name = Form11.Name;
                        SquareColumnsReinforcementSettingsT3Item.Form51Name = Form51.Name;
                        SquareColumnsReinforcementSettingsT3Item.RebarHookTypeForStirrupName = RebarHookTypeForStirrup.Name;

                        SquareColumnsReinforcementSettingsT3Item.FirstMainBarTapeName = FirstMainBarTape.Name;
                        SquareColumnsReinforcementSettingsT3Item.FirstStirrupBarTapeName = FirstStirrupBarTape.Name;
                        SquareColumnsReinforcementSettingsT3Item.ColumnRebarCoverTypeName = ColumnRebarCoverType.Name;
                        SquareColumnsReinforcementSettingsT3Item.SecondMainBarTapeName = SecondMainBarTape.Name;
                        SquareColumnsReinforcementSettingsT3Item.SecondStirrupBarTapeName = SecondStirrupBarTape.Name;

                        SquareColumnsReinforcementSettingsT3Item.FirstRebarOutletsLength = FirstRebarOutletsLength;
                        SquareColumnsReinforcementSettingsT3Item.SecondRebarOutletsLength = SecondRebarOutletsLength;
                        SquareColumnsReinforcementSettingsT3Item.StandardStirrupStep = StandardStirrupStep;
                        SquareColumnsReinforcementSettingsT3Item.FrequentButtomStirrupStep = FrequentButtomStirrupStep;
                        SquareColumnsReinforcementSettingsT3Item.FrequentButtomStirrupPlacementHeight = FrequentButtomStirrupPlacementHeight;
                        SquareColumnsReinforcementSettingsT3Item.FrequentTopStirrupStep = FrequentTopStirrupStep;
                        SquareColumnsReinforcementSettingsT3Item.FrequentTopStirrupPlacementHeight = FrequentTopStirrupPlacementHeight;
                        SquareColumnsReinforcementSettingsT3Item.FloorThickness = FloorThickness;
                        SquareColumnsReinforcementSettingsT3Item.FirstStirrupButtomOffset = FirstStirrupButtomOffset;

                        SquareColumnsReinforcementSettingsT3Item.RebarConnectionOptionName = RebarConnectionOptionName;
                        SquareColumnsReinforcementSettingsT3Item.DeepenRebarChecked = DeepenRebarChecked;
                        SquareColumnsReinforcementSettingsT3Item.DeepenRebar = DeepenRebar;

                        SquareColumnsReinforcementSettingsT3Item.OverlapTransitionChecked = OverlapTransitionChecked;

                        SquareColumnsReinforcementSettingsT3Item.MechanicalConnectionOptionName = MechanicalConnectionOptionName;
                        if (WeldedConnectionFamily != null)
                        {
                            SquareColumnsReinforcementSettingsT3Item.WeldedConnectionFamilyName = WeldedConnectionFamily.Name;
                        }
                        if (CouplingConnectionFamily != null)
                        {
                            SquareColumnsReinforcementSettingsT3Item.CouplingConnectionFamilyName = CouplingConnectionFamily.Name;
                        }

                        SquareColumnsReinforcementSettingsT3Item.BendInSlabChecked = BendInSlabChecked;
                        SquareColumnsReinforcementSettingsT3Item.BendInSlab = BendInSlab;

                        SquareColumnsReinforcementSettingsT3Item.SectionChangeChecked = SectionChangeChecked;
                        SquareColumnsReinforcementSettingsT3Item.SectionChange = SectionChange;

                        SquareColumnsReinforcementSettingsT3Item.ProgressiveCollapseBarIntoSlabChecked = ProgressiveCollapseBarIntoSlabChecked;
                        if (ProgressiveCollapseBarTape != null)
                        {
                            SquareColumnsReinforcementSettingsT3Item.ProgressiveCollapseBarTapeName = ProgressiveCollapseBarTape.Name;
                        }
                        SquareColumnsReinforcementSettingsT3Item.ProgressiveCollapseColumnCenterOffset = ProgressiveCollapseColumnCenterOffset;
                        SquareColumnsReinforcementSettingsT3Item.ProgressiveCollapseUpLength = ProgressiveCollapseUpLength;
                        SquareColumnsReinforcementSettingsT3Item.ProgressiveCollapseBottomIndent = ProgressiveCollapseBottomIndent;
                        SquareColumnsReinforcementSettingsT3Item.ProgressiveCollapseSideLength = ProgressiveCollapseSideLength;

                        SquareColumnsReinforcementSettingsT3Item.SaveSettings();
                    }

                    if (SelectedReinforcementTypeButtonName != "button_Type3")
                    {
                        double.TryParse(textBox_FirstTopRebarOffset.Text, out FirstTopRebarOffset);
                        if (string.IsNullOrEmpty(textBox_FirstTopRebarOffset.Text))
                        {
                            CheckFillingOk = false;
                            TaskDialog.Show("Revit", "Укажите смещение стержня по грани вверх, чтобы продолжить работу!");
                            return;
                        }
                        double.TryParse(textBox_FirstLowerRebarOffset.Text, out FirstLowerRebarOffset);
                        if (string.IsNullOrEmpty(textBox_FirstLowerRebarOffset.Text))
                        {
                            CheckFillingOk = false;
                            TaskDialog.Show("Revit", "Укажите смещение стержня по грани вниз, чтобы продолжить работу!");
                            return;
                        }
                        double.TryParse(textBox_FirstLeftRebarOffset.Text, out FirstLeftRebarOffset);
                        if (string.IsNullOrEmpty(textBox_FirstLeftRebarOffset.Text))
                        {
                            CheckFillingOk = false;
                            TaskDialog.Show("Revit", "Укажите смещение стержня по грани влево, чтобы продолжить работу!");
                            return;
                        }
                        double.TryParse(textBox_FirstRightRebarOffset.Text, out FirstRightRebarOffset);
                        if (string.IsNullOrEmpty(textBox_FirstRightRebarOffset.Text))
                        {
                            CheckFillingOk = false;
                            TaskDialog.Show("Revit", "Укажите смещение стержня по грани вправо, чтобы продолжить работу!");
                            return;
                        }
                        if (SelectedReinforcementTypeButtonName == "button_Type4")
                        {
                            SquareColumnsReinforcementSettingsT4Item = new SquareColumnsReinforcementSettingsT4();

                            SquareColumnsReinforcementSettingsT4Item.Form01Name = Form01.Name;
                            SquareColumnsReinforcementSettingsT4Item.Form26Name = Form26.Name;
                            SquareColumnsReinforcementSettingsT4Item.Form11Name = Form11.Name;
                            SquareColumnsReinforcementSettingsT4Item.Form51Name = Form51.Name;
                            SquareColumnsReinforcementSettingsT4Item.RebarHookTypeForStirrupName = RebarHookTypeForStirrup.Name;

                            SquareColumnsReinforcementSettingsT4Item.FirstMainBarTapeName = FirstMainBarTape.Name;
                            SquareColumnsReinforcementSettingsT4Item.FirstStirrupBarTapeName = FirstStirrupBarTape.Name;
                            SquareColumnsReinforcementSettingsT4Item.ColumnRebarCoverTypeName = ColumnRebarCoverType.Name;
                            SquareColumnsReinforcementSettingsT4Item.SecondMainBarTapeName = SecondMainBarTape.Name;
                            SquareColumnsReinforcementSettingsT4Item.SecondStirrupBarTapeName = SecondStirrupBarTape.Name;

                            SquareColumnsReinforcementSettingsT4Item.FirstTopRebarOffset = FirstTopRebarOffset;
                            SquareColumnsReinforcementSettingsT4Item.FirstLowerRebarOffset = FirstLowerRebarOffset;
                            SquareColumnsReinforcementSettingsT4Item.FirstLeftRebarOffset = FirstLeftRebarOffset;
                            SquareColumnsReinforcementSettingsT4Item.FirstRightRebarOffset = FirstRightRebarOffset;

                            SquareColumnsReinforcementSettingsT4Item.FirstRebarOutletsLength = FirstRebarOutletsLength;
                            SquareColumnsReinforcementSettingsT4Item.SecondRebarOutletsLength = SecondRebarOutletsLength;
                            SquareColumnsReinforcementSettingsT4Item.StandardStirrupStep = StandardStirrupStep;
                            SquareColumnsReinforcementSettingsT4Item.FrequentButtomStirrupStep = FrequentButtomStirrupStep;
                            SquareColumnsReinforcementSettingsT4Item.FrequentButtomStirrupPlacementHeight = FrequentButtomStirrupPlacementHeight;
                            SquareColumnsReinforcementSettingsT4Item.FrequentTopStirrupStep = FrequentTopStirrupStep;
                            SquareColumnsReinforcementSettingsT4Item.FrequentTopStirrupPlacementHeight = FrequentTopStirrupPlacementHeight;
                            SquareColumnsReinforcementSettingsT4Item.FloorThickness = FloorThickness;
                            SquareColumnsReinforcementSettingsT4Item.FirstStirrupButtomOffset = FirstStirrupButtomOffset;

                            SquareColumnsReinforcementSettingsT4Item.RebarConnectionOptionName = RebarConnectionOptionName;
                            SquareColumnsReinforcementSettingsT4Item.DeepenRebarChecked = DeepenRebarChecked;
                            SquareColumnsReinforcementSettingsT4Item.DeepenRebar = DeepenRebar;

                            SquareColumnsReinforcementSettingsT4Item.OverlapTransitionChecked = OverlapTransitionChecked;

                            SquareColumnsReinforcementSettingsT4Item.MechanicalConnectionOptionName = MechanicalConnectionOptionName;
                            if (WeldedConnectionFamily != null)
                            {
                                SquareColumnsReinforcementSettingsT4Item.WeldedConnectionFamilyName = WeldedConnectionFamily.Name;
                            }
                            if (CouplingConnectionFamily != null)
                            {
                                SquareColumnsReinforcementSettingsT4Item.CouplingConnectionFamilyName = CouplingConnectionFamily.Name;
                            }

                            SquareColumnsReinforcementSettingsT4Item.BendInSlabChecked = BendInSlabChecked;
                            SquareColumnsReinforcementSettingsT4Item.BendInSlab = BendInSlab;

                            SquareColumnsReinforcementSettingsT4Item.SectionChangeChecked = SectionChangeChecked;
                            SquareColumnsReinforcementSettingsT4Item.SectionChange = SectionChange;

                            SquareColumnsReinforcementSettingsT4Item.ProgressiveCollapseBarIntoSlabChecked = ProgressiveCollapseBarIntoSlabChecked;
                            if (ProgressiveCollapseBarTape != null)
                            {
                                SquareColumnsReinforcementSettingsT4Item.ProgressiveCollapseBarTapeName = ProgressiveCollapseBarTape.Name;
                            }
                            SquareColumnsReinforcementSettingsT4Item.ProgressiveCollapseColumnCenterOffset = ProgressiveCollapseColumnCenterOffset;
                            SquareColumnsReinforcementSettingsT4Item.ProgressiveCollapseUpLength = ProgressiveCollapseUpLength;
                            SquareColumnsReinforcementSettingsT4Item.ProgressiveCollapseBottomIndent = ProgressiveCollapseBottomIndent;
                            SquareColumnsReinforcementSettingsT4Item.ProgressiveCollapseSideLength = ProgressiveCollapseSideLength;

                            SquareColumnsReinforcementSettingsT4Item.SaveSettings();
                        }
                        if (SelectedReinforcementTypeButtonName == "button_Type5")
                        {
                            SquareColumnsReinforcementSettingsT5Item = new SquareColumnsReinforcementSettingsT5();

                            SquareColumnsReinforcementSettingsT5Item.Form01Name = Form01.Name;
                            SquareColumnsReinforcementSettingsT5Item.Form26Name = Form26.Name;
                            SquareColumnsReinforcementSettingsT5Item.Form11Name = Form11.Name;
                            SquareColumnsReinforcementSettingsT5Item.Form51Name = Form51.Name;
                            SquareColumnsReinforcementSettingsT5Item.RebarHookTypeForStirrupName = RebarHookTypeForStirrup.Name;

                            SquareColumnsReinforcementSettingsT5Item.FirstMainBarTapeName = FirstMainBarTape.Name;
                            SquareColumnsReinforcementSettingsT5Item.FirstStirrupBarTapeName = FirstStirrupBarTape.Name;
                            SquareColumnsReinforcementSettingsT5Item.ColumnRebarCoverTypeName = ColumnRebarCoverType.Name;
                            SquareColumnsReinforcementSettingsT5Item.SecondMainBarTapeName = SecondMainBarTape.Name;
                            SquareColumnsReinforcementSettingsT5Item.SecondStirrupBarTapeName = SecondStirrupBarTape.Name;

                            SquareColumnsReinforcementSettingsT5Item.FirstTopRebarOffset = FirstTopRebarOffset;
                            SquareColumnsReinforcementSettingsT5Item.FirstLowerRebarOffset = FirstLowerRebarOffset;
                            SquareColumnsReinforcementSettingsT5Item.FirstLeftRebarOffset = FirstLeftRebarOffset;
                            SquareColumnsReinforcementSettingsT5Item.FirstRightRebarOffset = FirstRightRebarOffset;

                            SquareColumnsReinforcementSettingsT5Item.FirstRebarOutletsLength = FirstRebarOutletsLength;
                            SquareColumnsReinforcementSettingsT5Item.SecondRebarOutletsLength = SecondRebarOutletsLength;
                            SquareColumnsReinforcementSettingsT5Item.StandardStirrupStep = StandardStirrupStep;
                            SquareColumnsReinforcementSettingsT5Item.FrequentButtomStirrupStep = FrequentButtomStirrupStep;
                            SquareColumnsReinforcementSettingsT5Item.FrequentButtomStirrupPlacementHeight = FrequentButtomStirrupPlacementHeight;
                            SquareColumnsReinforcementSettingsT5Item.FrequentTopStirrupStep = FrequentTopStirrupStep;
                            SquareColumnsReinforcementSettingsT5Item.FrequentTopStirrupPlacementHeight = FrequentTopStirrupPlacementHeight;
                            SquareColumnsReinforcementSettingsT5Item.FloorThickness = FloorThickness;
                            SquareColumnsReinforcementSettingsT5Item.FirstStirrupButtomOffset = FirstStirrupButtomOffset;

                            SquareColumnsReinforcementSettingsT5Item.RebarConnectionOptionName = RebarConnectionOptionName;
                            SquareColumnsReinforcementSettingsT5Item.DeepenRebarChecked = DeepenRebarChecked;
                            SquareColumnsReinforcementSettingsT5Item.DeepenRebar = DeepenRebar;

                            SquareColumnsReinforcementSettingsT5Item.OverlapTransitionChecked = OverlapTransitionChecked;

                            SquareColumnsReinforcementSettingsT5Item.MechanicalConnectionOptionName = MechanicalConnectionOptionName;
                            if (WeldedConnectionFamily != null)
                            {
                                SquareColumnsReinforcementSettingsT5Item.WeldedConnectionFamilyName = WeldedConnectionFamily.Name;
                            }
                            if (CouplingConnectionFamily != null)
                            {
                                SquareColumnsReinforcementSettingsT5Item.CouplingConnectionFamilyName = CouplingConnectionFamily.Name;
                            }

                            SquareColumnsReinforcementSettingsT5Item.BendInSlabChecked = BendInSlabChecked;
                            SquareColumnsReinforcementSettingsT5Item.BendInSlab = BendInSlab;

                            SquareColumnsReinforcementSettingsT5Item.SectionChangeChecked = SectionChangeChecked;
                            SquareColumnsReinforcementSettingsT5Item.SectionChange = SectionChange;

                            SquareColumnsReinforcementSettingsT5Item.ProgressiveCollapseBarIntoSlabChecked = ProgressiveCollapseBarIntoSlabChecked;
                            if (ProgressiveCollapseBarTape != null)
                            {
                                SquareColumnsReinforcementSettingsT5Item.ProgressiveCollapseBarTapeName = ProgressiveCollapseBarTape.Name;
                            }
                            SquareColumnsReinforcementSettingsT5Item.ProgressiveCollapseColumnCenterOffset = ProgressiveCollapseColumnCenterOffset;
                            SquareColumnsReinforcementSettingsT5Item.ProgressiveCollapseUpLength = ProgressiveCollapseUpLength;
                            SquareColumnsReinforcementSettingsT5Item.ProgressiveCollapseBottomIndent = ProgressiveCollapseBottomIndent;
                            SquareColumnsReinforcementSettingsT5Item.ProgressiveCollapseSideLength = ProgressiveCollapseSideLength;

                            SquareColumnsReinforcementSettingsT5Item.SaveSettings();
                        }

                        if (SelectedReinforcementTypeButtonName == "button_Type6")
                        {
                            double.TryParse(textBox_SecondTopRebarOffset.Text, out SecondTopRebarOffset);
                            if (string.IsNullOrEmpty(textBox_SecondTopRebarOffset.Text))
                            {
                                CheckFillingOk = false;
                                TaskDialog.Show("Revit", "Укажите смещение второго стержня по грани вверх, чтобы продолжить работу!");
                                return;
                            }
                            double.TryParse(textBox_SecondLowerRebarOffset.Text, out SecondLowerRebarOffset);
                            if (string.IsNullOrEmpty(textBox_SecondLowerRebarOffset.Text))
                            {
                                CheckFillingOk = false;
                                TaskDialog.Show("Revit", "Укажите смещение второго стержня по грани вниз, чтобы продолжить работу!");
                                return;
                            }
                            double.TryParse(textBox_SecondLeftRebarOffset.Text, out SecondLeftRebarOffset);
                            if (string.IsNullOrEmpty(textBox_SecondLeftRebarOffset.Text))
                            {
                                CheckFillingOk = false;
                                TaskDialog.Show("Revit", "Укажите смещение второго стержня по грани влево, чтобы продолжить работу!");
                                return;
                            }
                            double.TryParse(textBox_SecondRightRebarOffset.Text, out SecondRightRebarOffset);
                            if (string.IsNullOrEmpty(textBox_SecondRightRebarOffset.Text))
                            {
                                CheckFillingOk = false;
                                TaskDialog.Show("Revit", "Укажите смещение второго стержня по грани вправо, чтобы продолжить работу!");
                                return;
                            }

                            SquareColumnsReinforcementSettingsT6Item = new SquareColumnsReinforcementSettingsT6();

                            SquareColumnsReinforcementSettingsT6Item.Form01Name = Form01.Name;
                            SquareColumnsReinforcementSettingsT6Item.Form26Name = Form26.Name;
                            SquareColumnsReinforcementSettingsT6Item.Form11Name = Form11.Name;
                            SquareColumnsReinforcementSettingsT6Item.Form51Name = Form51.Name;
                            SquareColumnsReinforcementSettingsT6Item.RebarHookTypeForStirrupName = RebarHookTypeForStirrup.Name;

                            SquareColumnsReinforcementSettingsT6Item.FirstMainBarTapeName = FirstMainBarTape.Name;
                            SquareColumnsReinforcementSettingsT6Item.FirstStirrupBarTapeName = FirstStirrupBarTape.Name;
                            SquareColumnsReinforcementSettingsT6Item.ColumnRebarCoverTypeName = ColumnRebarCoverType.Name;
                            SquareColumnsReinforcementSettingsT6Item.SecondMainBarTapeName = SecondMainBarTape.Name;
                            SquareColumnsReinforcementSettingsT6Item.SecondStirrupBarTapeName = SecondStirrupBarTape.Name;

                            SquareColumnsReinforcementSettingsT6Item.FirstTopRebarOffset = FirstTopRebarOffset;
                            SquareColumnsReinforcementSettingsT6Item.FirstLowerRebarOffset = FirstLowerRebarOffset;
                            SquareColumnsReinforcementSettingsT6Item.FirstLeftRebarOffset = FirstLeftRebarOffset;
                            SquareColumnsReinforcementSettingsT6Item.FirstRightRebarOffset = FirstRightRebarOffset;

                            SquareColumnsReinforcementSettingsT6Item.SecondTopRebarOffset = SecondTopRebarOffset;
                            SquareColumnsReinforcementSettingsT6Item.SecondLowerRebarOffset = SecondLowerRebarOffset;
                            SquareColumnsReinforcementSettingsT6Item.SecondLeftRebarOffset = SecondLeftRebarOffset;
                            SquareColumnsReinforcementSettingsT6Item.SecondRightRebarOffset = SecondRightRebarOffset;

                            SquareColumnsReinforcementSettingsT6Item.FirstRebarOutletsLength = FirstRebarOutletsLength;
                            SquareColumnsReinforcementSettingsT6Item.SecondRebarOutletsLength = SecondRebarOutletsLength;
                            SquareColumnsReinforcementSettingsT6Item.StandardStirrupStep = StandardStirrupStep;
                            SquareColumnsReinforcementSettingsT6Item.FrequentButtomStirrupStep = FrequentButtomStirrupStep;
                            SquareColumnsReinforcementSettingsT6Item.FrequentButtomStirrupPlacementHeight = FrequentButtomStirrupPlacementHeight;
                            SquareColumnsReinforcementSettingsT6Item.FrequentTopStirrupStep = FrequentTopStirrupStep;
                            SquareColumnsReinforcementSettingsT6Item.FrequentTopStirrupPlacementHeight = FrequentTopStirrupPlacementHeight;
                            SquareColumnsReinforcementSettingsT6Item.FloorThickness = FloorThickness;
                            SquareColumnsReinforcementSettingsT6Item.FirstStirrupButtomOffset = FirstStirrupButtomOffset;

                            SquareColumnsReinforcementSettingsT6Item.RebarConnectionOptionName = RebarConnectionOptionName;
                            SquareColumnsReinforcementSettingsT6Item.DeepenRebarChecked = DeepenRebarChecked;
                            SquareColumnsReinforcementSettingsT6Item.DeepenRebar = DeepenRebar;

                            SquareColumnsReinforcementSettingsT6Item.OverlapTransitionChecked = OverlapTransitionChecked;

                            SquareColumnsReinforcementSettingsT6Item.MechanicalConnectionOptionName = MechanicalConnectionOptionName;
                            if (WeldedConnectionFamily != null)
                            {
                                SquareColumnsReinforcementSettingsT6Item.WeldedConnectionFamilyName = WeldedConnectionFamily.Name;
                            }
                            if (CouplingConnectionFamily != null)
                            {
                                SquareColumnsReinforcementSettingsT6Item.CouplingConnectionFamilyName = CouplingConnectionFamily.Name;
                            }

                            SquareColumnsReinforcementSettingsT6Item.BendInSlabChecked = BendInSlabChecked;
                            SquareColumnsReinforcementSettingsT6Item.BendInSlab = BendInSlab;

                            SquareColumnsReinforcementSettingsT6Item.SectionChangeChecked = SectionChangeChecked;
                            SquareColumnsReinforcementSettingsT6Item.SectionChange = SectionChange;

                            SquareColumnsReinforcementSettingsT6Item.ProgressiveCollapseBarIntoSlabChecked = ProgressiveCollapseBarIntoSlabChecked;
                            if (ProgressiveCollapseBarTape != null)
                            {
                                SquareColumnsReinforcementSettingsT6Item.ProgressiveCollapseBarTapeName = ProgressiveCollapseBarTape.Name;
                            }
                            SquareColumnsReinforcementSettingsT6Item.ProgressiveCollapseColumnCenterOffset = ProgressiveCollapseColumnCenterOffset;
                            SquareColumnsReinforcementSettingsT6Item.ProgressiveCollapseUpLength = ProgressiveCollapseUpLength;
                            SquareColumnsReinforcementSettingsT6Item.ProgressiveCollapseBottomIndent = ProgressiveCollapseBottomIndent;
                            SquareColumnsReinforcementSettingsT6Item.ProgressiveCollapseSideLength = ProgressiveCollapseSideLength;

                            SquareColumnsReinforcementSettingsT6Item.SaveSettings();
                        }
                    }
                }
            }
            CheckFillingOk = true;
        }
        private void SetSavedSettingsT1()
        {
            if(SquareColumnsReinforcementSettingsT1Item != null)
            {
                //Задание сохраненных форм
                if (RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT1Item.Form01Name) != null)
                {
                    comboBox_Form01.SelectedItem = RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT1Item.Form01Name);
                }
                else
                {
                    if (comboBox_Form01.Items.Count != 0)
                    {
                        comboBox_Form01.SelectedItem = comboBox_Form01.Items.GetItemAt(0);
                    }
                }

                if (RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT1Item.Form26Name) != null)
                {
                    comboBox_Form26.SelectedItem = RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT1Item.Form26Name);
                }
                else
                {
                    if (comboBox_Form26.Items.Count != 0)
                    {
                        comboBox_Form26.SelectedItem = comboBox_Form26.Items.GetItemAt(0);
                    }
                }

                if (RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT1Item.Form11Name) != null)
                {
                    comboBox_Form11.SelectedItem = RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT1Item.Form11Name);
                }
                else
                {
                    if (comboBox_Form11.Items.Count != 0)
                    {
                        comboBox_Form11.SelectedItem = comboBox_Form11.Items.GetItemAt(0);
                    }
                }

                if (RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT1Item.Form51Name) != null)
                {
                    comboBox_Form51.SelectedItem = RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT1Item.Form51Name);
                }
                else
                {
                    if (comboBox_Form51.Items.Count != 0)
                    {
                        comboBox_Form51.SelectedItem = comboBox_Form51.Items.GetItemAt(0);
                    }
                }

                if (RebarHookTypeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT1Item.RebarHookTypeForStirrupName) != null)
                {
                    comboBox_RebarHookType.SelectedItem = RebarHookTypeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT1Item.RebarHookTypeForStirrupName);
                }
                else
                {
                    if (comboBox_RebarHookType.Items.Count != 0)
                    {
                        comboBox_RebarHookType.SelectedItem = comboBox_RebarHookType.Items.GetItemAt(0);
                    }
                }

                //Заполнение сохраненных параметров сечения
                if (RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT1Item.FirstMainBarTapeName) != null)
                {
                    comboBox_FirstMainBarTapes.SelectedItem = RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT1Item.FirstMainBarTapeName);
                }
                else
                {
                    if (comboBox_FirstMainBarTapes.Items.Count != 0)
                    {
                        comboBox_FirstMainBarTapes.SelectedItem = comboBox_FirstMainBarTapes.Items.GetItemAt(0);
                    }
                }

                if (RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT1Item.FirstStirrupBarTapeName) != null)
                {
                    comboBox_FirstStirrupBarTapes.SelectedItem = RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT1Item.FirstStirrupBarTapeName);
                }
                else
                {
                    if (comboBox_FirstStirrupBarTapes.Items.Count != 0)
                    {
                        comboBox_FirstStirrupBarTapes.SelectedItem = comboBox_FirstStirrupBarTapes.Items.GetItemAt(0);
                    }
                }

                if (RebarCoverTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT1Item.ColumnRebarCoverTypeName) != null)
                {
                    comboBox_RebarCoverTypes.SelectedItem = RebarCoverTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT1Item.ColumnRebarCoverTypeName);
                }
                else
                {
                    if (comboBox_RebarCoverTypes.Items.Count != 0)
                    {
                        comboBox_RebarCoverTypes.SelectedItem = comboBox_RebarCoverTypes.Items.GetItemAt(0);
                    }
                }

                //Заполнение сохраненных параметров на 3D
                textBox_FirstRebarOutletsLength.Text = SquareColumnsReinforcementSettingsT1Item.FirstRebarOutletsLength.ToString();
                textBox_SecondRebarOutletsLength.Text = SquareColumnsReinforcementSettingsT1Item.SecondRebarOutletsLength.ToString();
                textBox_StandardStirrupStep.Text = SquareColumnsReinforcementSettingsT1Item.StandardStirrupStep.ToString();
                textBox_FrequentButtomStirrupStep.Text = SquareColumnsReinforcementSettingsT1Item.FrequentButtomStirrupStep.ToString();
                textBox_FrequentButtomStirrupPlacementHeight.Text = SquareColumnsReinforcementSettingsT1Item.FrequentButtomStirrupPlacementHeight.ToString();
                textBox_FrequentTopStirrupStep.Text = SquareColumnsReinforcementSettingsT1Item.FrequentTopStirrupStep.ToString();
                textBox_FrequentTopStirrupPlacementHeight.Text = SquareColumnsReinforcementSettingsT1Item.FrequentTopStirrupPlacementHeight.ToString();
                textBox_FloorThickness.Text = SquareColumnsReinforcementSettingsT1Item.FloorThickness.ToString();
                textBox_FirstStirrupButtomOffset.Text = SquareColumnsReinforcementSettingsT1Item.FirstStirrupButtomOffset.ToString();

                //Заполнение сохраненных полей для соединения арматуры
                if(SquareColumnsReinforcementSettingsT1Item.RebarConnectionOptionName == "radioButton_Overlap")
                {
                    radioButton_Overlap.IsChecked = true;
                }
                else
                {
                    radioButton_Mechanical.IsChecked = true;
                }
                checkBox_DeepenRebar.IsChecked = SquareColumnsReinforcementSettingsT1Item.DeepenRebarChecked;
                if(SquareColumnsReinforcementSettingsT1Item.DeepenRebar != 0)
                {
                    textBox_DeepenRebar.Text = SquareColumnsReinforcementSettingsT1Item.DeepenRebar.ToString();
                }
                else
                {
                    textBox_DeepenRebar.Text = "";
                }

                checkBox_OverlapTransition.IsChecked = SquareColumnsReinforcementSettingsT1Item.OverlapTransitionChecked;

                if (SquareColumnsReinforcementSettingsT1Item.MechanicalConnectionOptionName == "radioButton_WeldedConnection")
                {
                    radioButton_WeldedConnection.IsChecked = true;
                }
                else
                {
                    radioButton_CouplingConnection.IsChecked = true;
                }

                if (RebarConnectionsList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT1Item.WeldedConnectionFamilyName) != null)
                {
                    comboBox_WeldedConnection.SelectedItem = RebarConnectionsList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT1Item.WeldedConnectionFamilyName);
                }
                else
                {
                    if (comboBox_WeldedConnection.Items.Count != 0)
                    {
                        comboBox_WeldedConnection.SelectedItem = comboBox_WeldedConnection.Items.GetItemAt(0);
                    }
                }

                if (RebarConnectionsList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT1Item.CouplingConnectionFamilyName) != null)
                {
                    comboBox_CouplingConnection.SelectedItem = RebarConnectionsList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT1Item.CouplingConnectionFamilyName);
                }
                else
                {
                    if (comboBox_CouplingConnection.Items.Count != 0)
                    {
                        comboBox_CouplingConnection.SelectedItem = comboBox_CouplingConnection.Items.GetItemAt(0);
                    }
                }

                //Заполнение сохраненных дополнительных параметров
                checkBox_BendInSlab.IsChecked = SquareColumnsReinforcementSettingsT1Item.BendInSlabChecked;
                if(SquareColumnsReinforcementSettingsT1Item.BendInSlab != 0)
                {
                    textBox_BendInSlab.Text = SquareColumnsReinforcementSettingsT1Item.BendInSlab.ToString();
                }
                else
                {
                    textBox_BendInSlab.Text = "";
                }

                checkBox_SectionChange.IsChecked = SquareColumnsReinforcementSettingsT1Item.SectionChangeChecked;
                if(SquareColumnsReinforcementSettingsT1Item.SectionChange != 0)
                {
                    textBox_SectionChange.Text = SquareColumnsReinforcementSettingsT1Item.SectionChange.ToString();
                }
                else
                {
                    textBox_SectionChange.Text = "";
                }

                //Заполнение сохраненных параметров прогрессирующего обрушения
                checkBox_ProgressiveCollapseBarIntoSlab.IsChecked = SquareColumnsReinforcementSettingsT1Item.ProgressiveCollapseBarIntoSlabChecked;
                
                if (RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT1Item.ProgressiveCollapseBarTapeName) != null)
                {
                    comboBox_ProgressiveCollapseBarTapes.SelectedItem = RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT1Item.ProgressiveCollapseBarTapeName);
                }
                else
                {
                    if (comboBox_ProgressiveCollapseBarTapes.Items.Count != 0)
                    {
                        comboBox_ProgressiveCollapseBarTapes.SelectedItem = comboBox_ProgressiveCollapseBarTapes.Items.GetItemAt(0);
                    }
                }

                if(SquareColumnsReinforcementSettingsT1Item.ProgressiveCollapseColumnCenterOffset != 0)
                {
                    textBox_ProgressiveCollapseColumnCenterOffset.Text = SquareColumnsReinforcementSettingsT1Item.ProgressiveCollapseColumnCenterOffset.ToString();
                }
                else
                {
                    textBox_ProgressiveCollapseColumnCenterOffset.Text = "";
                }

                if(SquareColumnsReinforcementSettingsT1Item.ProgressiveCollapseUpLength != 0)
                {
                    textBox_ProgressiveCollapseUpLength.Text = SquareColumnsReinforcementSettingsT1Item.ProgressiveCollapseUpLength.ToString();
                }
                else
                {
                    textBox_ProgressiveCollapseUpLength.Text = "";
                }

                if (SquareColumnsReinforcementSettingsT1Item.ProgressiveCollapseBottomIndent != 0)
                {
                    textBox_ProgressiveCollapseBottomIndent.Text = SquareColumnsReinforcementSettingsT1Item.ProgressiveCollapseBottomIndent.ToString();
                }
                else
                {
                    textBox_ProgressiveCollapseBottomIndent.Text = "";
                }
                
                if(SquareColumnsReinforcementSettingsT1Item.ProgressiveCollapseSideLength != 0)
                {
                    textBox_ProgressiveCollapseSideLength.Text = SquareColumnsReinforcementSettingsT1Item.ProgressiveCollapseSideLength.ToString();
                }
                else
                {
                    textBox_ProgressiveCollapseSideLength.Text = "";
                }
            }
        }
        private void SetSavedSettingsT2()
        {
            if (SquareColumnsReinforcementSettingsT2Item != null)
            {
                //Задание сохраненных форм
                if (RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT2Item.Form01Name) != null)
                {
                    comboBox_Form01.SelectedItem = RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT2Item.Form01Name);
                }
                else
                {
                    if (comboBox_Form01.Items.Count != 0)
                    {
                        comboBox_Form01.SelectedItem = comboBox_Form01.Items.GetItemAt(0);
                    }
                }

                if (RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT2Item.Form26Name) != null)
                {
                    comboBox_Form26.SelectedItem = RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT2Item.Form26Name);
                }
                else
                {
                    if (comboBox_Form26.Items.Count != 0)
                    {
                        comboBox_Form26.SelectedItem = comboBox_Form26.Items.GetItemAt(0);
                    }
                }

                if (RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT2Item.Form11Name) != null)
                {
                    comboBox_Form11.SelectedItem = RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT2Item.Form11Name);
                }
                else
                {
                    if (comboBox_Form11.Items.Count != 0)
                    {
                        comboBox_Form11.SelectedItem = comboBox_Form11.Items.GetItemAt(0);
                    }
                }

                if (RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT2Item.Form51Name) != null)
                {
                    comboBox_Form51.SelectedItem = RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT2Item.Form51Name);
                }
                else
                {
                    if (comboBox_Form51.Items.Count != 0)
                    {
                        comboBox_Form51.SelectedItem = comboBox_Form51.Items.GetItemAt(0);
                    }
                }

                if (RebarHookTypeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT2Item.RebarHookTypeForStirrupName) != null)
                {
                    comboBox_RebarHookType.SelectedItem = RebarHookTypeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT2Item.RebarHookTypeForStirrupName);
                }
                else
                {
                    if (comboBox_RebarHookType.Items.Count != 0)
                    {
                        comboBox_RebarHookType.SelectedItem = comboBox_RebarHookType.Items.GetItemAt(0);
                    }
                }

                //Заполнение сохраненных параметров сечения
                if (RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT2Item.FirstMainBarTapeName) != null)
                {
                    comboBox_FirstMainBarTapes.SelectedItem = RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT2Item.FirstMainBarTapeName);
                }
                else
                {
                    if (comboBox_FirstMainBarTapes.Items.Count != 0)
                    {
                        comboBox_FirstMainBarTapes.SelectedItem = comboBox_FirstMainBarTapes.Items.GetItemAt(0);
                    }
                }

                if (RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT2Item.SecondMainBarTapeName) != null)
                {
                    comboBox_SecondMainBarTapes.SelectedItem = RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT2Item.SecondMainBarTapeName);
                }
                else
                {
                    if (comboBox_SecondMainBarTapes.Items.Count != 0)
                    {
                        comboBox_SecondMainBarTapes.SelectedItem = comboBox_SecondMainBarTapes.Items.GetItemAt(0);
                    }
                }

                if (RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT2Item.FirstStirrupBarTapeName) != null)
                {
                    comboBox_FirstStirrupBarTapes.SelectedItem = RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT2Item.FirstStirrupBarTapeName);
                }
                else
                {
                    if (comboBox_FirstStirrupBarTapes.Items.Count != 0)
                    {
                        comboBox_FirstStirrupBarTapes.SelectedItem = comboBox_FirstStirrupBarTapes.Items.GetItemAt(0);
                    }
                }

                if (RebarCoverTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT2Item.ColumnRebarCoverTypeName) != null)
                {
                    comboBox_RebarCoverTypes.SelectedItem = RebarCoverTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT2Item.ColumnRebarCoverTypeName);
                }
                else
                {
                    if (comboBox_RebarCoverTypes.Items.Count != 0)
                    {
                        comboBox_RebarCoverTypes.SelectedItem = comboBox_RebarCoverTypes.Items.GetItemAt(0);
                    }
                }

                //Заполнение сохраненных параметров на 3D
                textBox_FirstRebarOutletsLength.Text = SquareColumnsReinforcementSettingsT2Item.FirstRebarOutletsLength.ToString();
                textBox_SecondRebarOutletsLength.Text = SquareColumnsReinforcementSettingsT2Item.SecondRebarOutletsLength.ToString();
                textBox_StandardStirrupStep.Text = SquareColumnsReinforcementSettingsT2Item.StandardStirrupStep.ToString();
                textBox_FrequentButtomStirrupStep.Text = SquareColumnsReinforcementSettingsT2Item.FrequentButtomStirrupStep.ToString();
                textBox_FrequentButtomStirrupPlacementHeight.Text = SquareColumnsReinforcementSettingsT2Item.FrequentButtomStirrupPlacementHeight.ToString();
                textBox_FrequentTopStirrupStep.Text = SquareColumnsReinforcementSettingsT2Item.FrequentTopStirrupStep.ToString();
                textBox_FrequentTopStirrupPlacementHeight.Text = SquareColumnsReinforcementSettingsT2Item.FrequentTopStirrupPlacementHeight.ToString();
                textBox_FloorThickness.Text = SquareColumnsReinforcementSettingsT2Item.FloorThickness.ToString();
                textBox_FirstStirrupButtomOffset.Text = SquareColumnsReinforcementSettingsT2Item.FirstStirrupButtomOffset.ToString();

                //Заполнение сохраненных полей для соединения арматуры
                if (SquareColumnsReinforcementSettingsT2Item.RebarConnectionOptionName == "radioButton_Overlap")
                {
                    radioButton_Overlap.IsChecked = true;
                }
                else
                {
                    radioButton_Mechanical.IsChecked = true;
                }
                checkBox_DeepenRebar.IsChecked = SquareColumnsReinforcementSettingsT2Item.DeepenRebarChecked;
                if (SquareColumnsReinforcementSettingsT2Item.DeepenRebar != 0)
                {
                    textBox_DeepenRebar.Text = SquareColumnsReinforcementSettingsT2Item.DeepenRebar.ToString();
                }
                else
                {
                    textBox_DeepenRebar.Text = "";
                }

                checkBox_OverlapTransition.IsChecked = SquareColumnsReinforcementSettingsT2Item.OverlapTransitionChecked;

                if (SquareColumnsReinforcementSettingsT2Item.MechanicalConnectionOptionName == "radioButton_WeldedConnection")
                {
                    radioButton_WeldedConnection.IsChecked = true;
                }
                else
                {
                    radioButton_CouplingConnection.IsChecked = true;
                }

                if (RebarConnectionsList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT2Item.WeldedConnectionFamilyName) != null)
                {
                    comboBox_WeldedConnection.SelectedItem = RebarConnectionsList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT2Item.WeldedConnectionFamilyName);
                }
                else
                {
                    if (comboBox_WeldedConnection.Items.Count != 0)
                    {
                        comboBox_WeldedConnection.SelectedItem = comboBox_WeldedConnection.Items.GetItemAt(0);
                    }
                }

                if (RebarConnectionsList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT2Item.CouplingConnectionFamilyName) != null)
                {
                    comboBox_CouplingConnection.SelectedItem = RebarConnectionsList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT2Item.CouplingConnectionFamilyName);
                }
                else
                {
                    if (comboBox_CouplingConnection.Items.Count != 0)
                    {
                        comboBox_CouplingConnection.SelectedItem = comboBox_CouplingConnection.Items.GetItemAt(0);
                    }
                }

                //Заполнение сохраненных дополнительных параметров
                checkBox_BendInSlab.IsChecked = SquareColumnsReinforcementSettingsT2Item.BendInSlabChecked;
                if (SquareColumnsReinforcementSettingsT2Item.BendInSlab != 0)
                {
                    textBox_BendInSlab.Text = SquareColumnsReinforcementSettingsT2Item.BendInSlab.ToString();
                }
                else
                {
                    textBox_BendInSlab.Text = "";
                }

                checkBox_SectionChange.IsChecked = SquareColumnsReinforcementSettingsT2Item.SectionChangeChecked;
                if (SquareColumnsReinforcementSettingsT2Item.SectionChange != 0)
                {
                    textBox_SectionChange.Text = SquareColumnsReinforcementSettingsT2Item.SectionChange.ToString();
                }
                else
                {
                    textBox_SectionChange.Text = "";
                }

                //Заполнение сохраненных параметров прогрессирующего обрушения
                checkBox_ProgressiveCollapseBarIntoSlab.IsChecked = SquareColumnsReinforcementSettingsT2Item.ProgressiveCollapseBarIntoSlabChecked;

                if (RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT2Item.ProgressiveCollapseBarTapeName) != null)
                {
                    comboBox_ProgressiveCollapseBarTapes.SelectedItem = RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT2Item.ProgressiveCollapseBarTapeName);
                }
                else
                {
                    if (comboBox_ProgressiveCollapseBarTapes.Items.Count != 0)
                    {
                        comboBox_ProgressiveCollapseBarTapes.SelectedItem = comboBox_ProgressiveCollapseBarTapes.Items.GetItemAt(0);
                    }
                }

                if (SquareColumnsReinforcementSettingsT2Item.ProgressiveCollapseColumnCenterOffset != 0)
                {
                    textBox_ProgressiveCollapseColumnCenterOffset.Text = SquareColumnsReinforcementSettingsT2Item.ProgressiveCollapseColumnCenterOffset.ToString();
                }
                else
                {
                    textBox_ProgressiveCollapseColumnCenterOffset.Text = "";
                }

                if (SquareColumnsReinforcementSettingsT2Item.ProgressiveCollapseUpLength != 0)
                {
                    textBox_ProgressiveCollapseUpLength.Text = SquareColumnsReinforcementSettingsT2Item.ProgressiveCollapseUpLength.ToString();
                }
                else
                {
                    textBox_ProgressiveCollapseUpLength.Text = "";
                }

                if (SquareColumnsReinforcementSettingsT2Item.ProgressiveCollapseBottomIndent != 0)
                {
                    textBox_ProgressiveCollapseBottomIndent.Text = SquareColumnsReinforcementSettingsT2Item.ProgressiveCollapseBottomIndent.ToString();
                }
                else
                {
                    textBox_ProgressiveCollapseBottomIndent.Text = "";
                }

                if (SquareColumnsReinforcementSettingsT2Item.ProgressiveCollapseSideLength != 0)
                {
                    textBox_ProgressiveCollapseSideLength.Text = SquareColumnsReinforcementSettingsT2Item.ProgressiveCollapseSideLength.ToString();
                }
                else
                {
                    textBox_ProgressiveCollapseSideLength.Text = "";
                }
            }
        }
        private void SetSavedSettingsT3()
        {
            if (SquareColumnsReinforcementSettingsT3Item != null)
            {
                //Задание сохраненных форм
                if (RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT3Item.Form01Name) != null)
                {
                    comboBox_Form01.SelectedItem = RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT3Item.Form01Name);
                }
                else
                {
                    if (comboBox_Form01.Items.Count != 0)
                    {
                        comboBox_Form01.SelectedItem = comboBox_Form01.Items.GetItemAt(0);
                    }
                }

                if (RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT3Item.Form26Name) != null)
                {
                    comboBox_Form26.SelectedItem = RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT3Item.Form26Name);
                }
                else
                {
                    if (comboBox_Form26.Items.Count != 0)
                    {
                        comboBox_Form26.SelectedItem = comboBox_Form26.Items.GetItemAt(0);
                    }
                }

                if (RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT3Item.Form11Name) != null)
                {
                    comboBox_Form11.SelectedItem = RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT3Item.Form11Name);
                }
                else
                {
                    if (comboBox_Form11.Items.Count != 0)
                    {
                        comboBox_Form11.SelectedItem = comboBox_Form11.Items.GetItemAt(0);
                    }
                }

                if (RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT3Item.Form51Name) != null)
                {
                    comboBox_Form51.SelectedItem = RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT3Item.Form51Name);
                }
                else
                {
                    if (comboBox_Form51.Items.Count != 0)
                    {
                        comboBox_Form51.SelectedItem = comboBox_Form51.Items.GetItemAt(0);
                    }
                }

                if (RebarHookTypeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT3Item.RebarHookTypeForStirrupName) != null)
                {
                    comboBox_RebarHookType.SelectedItem = RebarHookTypeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT3Item.RebarHookTypeForStirrupName);
                }
                else
                {
                    if (comboBox_RebarHookType.Items.Count != 0)
                    {
                        comboBox_RebarHookType.SelectedItem = comboBox_RebarHookType.Items.GetItemAt(0);
                    }
                }

                //Заполнение сохраненных параметров сечения
                if (RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT3Item.FirstMainBarTapeName) != null)
                {
                    comboBox_FirstMainBarTapes.SelectedItem = RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT3Item.FirstMainBarTapeName);
                }
                else
                {
                    if (comboBox_FirstMainBarTapes.Items.Count != 0)
                    {
                        comboBox_FirstMainBarTapes.SelectedItem = comboBox_FirstMainBarTapes.Items.GetItemAt(0);
                    }
                }

                if (RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT3Item.SecondMainBarTapeName) != null)
                {
                    comboBox_SecondMainBarTapes.SelectedItem = RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT3Item.SecondMainBarTapeName);
                }
                else
                {
                    if (comboBox_SecondMainBarTapes.Items.Count != 0)
                    {
                        comboBox_SecondMainBarTapes.SelectedItem = comboBox_SecondMainBarTapes.Items.GetItemAt(0);
                    }
                }

                if (RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT3Item.FirstStirrupBarTapeName) != null)
                {
                    comboBox_FirstStirrupBarTapes.SelectedItem = RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT3Item.FirstStirrupBarTapeName);
                }
                else
                {
                    if (comboBox_FirstStirrupBarTapes.Items.Count != 0)
                    {
                        comboBox_FirstStirrupBarTapes.SelectedItem = comboBox_FirstStirrupBarTapes.Items.GetItemAt(0);
                    }
                }

                if (RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT3Item.SecondStirrupBarTapeName) != null)
                {
                    comboBox_SecondStirrupBarTapes.SelectedItem = RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT3Item.SecondStirrupBarTapeName);
                }
                else
                {
                    if (comboBox_SecondStirrupBarTapes.Items.Count != 0)
                    {
                        comboBox_SecondStirrupBarTapes.SelectedItem = comboBox_SecondStirrupBarTapes.Items.GetItemAt(0);
                    }
                }

                if (RebarCoverTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT3Item.ColumnRebarCoverTypeName) != null)
                {
                    comboBox_RebarCoverTypes.SelectedItem = RebarCoverTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT3Item.ColumnRebarCoverTypeName);
                }
                else
                {
                    if (comboBox_RebarCoverTypes.Items.Count != 0)
                    {
                        comboBox_RebarCoverTypes.SelectedItem = comboBox_RebarCoverTypes.Items.GetItemAt(0);
                    }
                }

                //Заполнение сохраненных параметров на 3D
                textBox_FirstRebarOutletsLength.Text = SquareColumnsReinforcementSettingsT3Item.FirstRebarOutletsLength.ToString();
                textBox_SecondRebarOutletsLength.Text = SquareColumnsReinforcementSettingsT3Item.SecondRebarOutletsLength.ToString();
                textBox_StandardStirrupStep.Text = SquareColumnsReinforcementSettingsT3Item.StandardStirrupStep.ToString();
                textBox_FrequentButtomStirrupStep.Text = SquareColumnsReinforcementSettingsT3Item.FrequentButtomStirrupStep.ToString();
                textBox_FrequentButtomStirrupPlacementHeight.Text = SquareColumnsReinforcementSettingsT3Item.FrequentButtomStirrupPlacementHeight.ToString();
                textBox_FrequentTopStirrupStep.Text = SquareColumnsReinforcementSettingsT3Item.FrequentTopStirrupStep.ToString();
                textBox_FrequentTopStirrupPlacementHeight.Text = SquareColumnsReinforcementSettingsT3Item.FrequentTopStirrupPlacementHeight.ToString();
                textBox_FloorThickness.Text = SquareColumnsReinforcementSettingsT3Item.FloorThickness.ToString();
                textBox_FirstStirrupButtomOffset.Text = SquareColumnsReinforcementSettingsT3Item.FirstStirrupButtomOffset.ToString();

                //Заполнение сохраненных полей для соединения арматуры
                if (SquareColumnsReinforcementSettingsT3Item.RebarConnectionOptionName == "radioButton_Overlap")
                {
                    radioButton_Overlap.IsChecked = true;
                }
                else
                {
                    radioButton_Mechanical.IsChecked = true;
                }
                checkBox_DeepenRebar.IsChecked = SquareColumnsReinforcementSettingsT3Item.DeepenRebarChecked;
                if (SquareColumnsReinforcementSettingsT3Item.DeepenRebar != 0)
                {
                    textBox_DeepenRebar.Text = SquareColumnsReinforcementSettingsT3Item.DeepenRebar.ToString();
                }
                else
                {
                    textBox_DeepenRebar.Text = "";
                }

                checkBox_OverlapTransition.IsChecked = SquareColumnsReinforcementSettingsT3Item.OverlapTransitionChecked;

                if (SquareColumnsReinforcementSettingsT3Item.MechanicalConnectionOptionName == "radioButton_WeldedConnection")
                {
                    radioButton_WeldedConnection.IsChecked = true;
                }
                else
                {
                    radioButton_CouplingConnection.IsChecked = true;
                }

                if (RebarConnectionsList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT3Item.WeldedConnectionFamilyName) != null)
                {
                    comboBox_WeldedConnection.SelectedItem = RebarConnectionsList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT3Item.WeldedConnectionFamilyName);
                }
                else
                {
                    if (comboBox_WeldedConnection.Items.Count != 0)
                    {
                        comboBox_WeldedConnection.SelectedItem = comboBox_WeldedConnection.Items.GetItemAt(0);
                    }
                }

                if (RebarConnectionsList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT3Item.CouplingConnectionFamilyName) != null)
                {
                    comboBox_CouplingConnection.SelectedItem = RebarConnectionsList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT3Item.CouplingConnectionFamilyName);
                }
                else
                {
                    if (comboBox_CouplingConnection.Items.Count != 0)
                    {
                        comboBox_CouplingConnection.SelectedItem = comboBox_CouplingConnection.Items.GetItemAt(0);
                    }
                }

                //Заполнение сохраненных дополнительных параметров
                checkBox_BendInSlab.IsChecked = SquareColumnsReinforcementSettingsT3Item.BendInSlabChecked;
                if (SquareColumnsReinforcementSettingsT3Item.BendInSlab != 0)
                {
                    textBox_BendInSlab.Text = SquareColumnsReinforcementSettingsT3Item.BendInSlab.ToString();
                }
                else
                {
                    textBox_BendInSlab.Text = "";
                }

                checkBox_SectionChange.IsChecked = SquareColumnsReinforcementSettingsT3Item.SectionChangeChecked;
                if (SquareColumnsReinforcementSettingsT3Item.SectionChange != 0)
                {
                    textBox_SectionChange.Text = SquareColumnsReinforcementSettingsT3Item.SectionChange.ToString();
                }
                else
                {
                    textBox_SectionChange.Text = "";
                }

                //Заполнение сохраненных параметров прогрессирующего обрушения
                checkBox_ProgressiveCollapseBarIntoSlab.IsChecked = SquareColumnsReinforcementSettingsT3Item.ProgressiveCollapseBarIntoSlabChecked;

                if (RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT3Item.ProgressiveCollapseBarTapeName) != null)
                {
                    comboBox_ProgressiveCollapseBarTapes.SelectedItem = RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT3Item.ProgressiveCollapseBarTapeName);
                }
                else
                {
                    if (comboBox_ProgressiveCollapseBarTapes.Items.Count != 0)
                    {
                        comboBox_ProgressiveCollapseBarTapes.SelectedItem = comboBox_ProgressiveCollapseBarTapes.Items.GetItemAt(0);
                    }
                }

                if (SquareColumnsReinforcementSettingsT3Item.ProgressiveCollapseColumnCenterOffset != 0)
                {
                    textBox_ProgressiveCollapseColumnCenterOffset.Text = SquareColumnsReinforcementSettingsT3Item.ProgressiveCollapseColumnCenterOffset.ToString();
                }
                else
                {
                    textBox_ProgressiveCollapseColumnCenterOffset.Text = "";
                }

                if (SquareColumnsReinforcementSettingsT3Item.ProgressiveCollapseUpLength != 0)
                {
                    textBox_ProgressiveCollapseUpLength.Text = SquareColumnsReinforcementSettingsT3Item.ProgressiveCollapseUpLength.ToString();
                }
                else
                {
                    textBox_ProgressiveCollapseUpLength.Text = "";
                }

                if (SquareColumnsReinforcementSettingsT3Item.ProgressiveCollapseBottomIndent != 0)
                {
                    textBox_ProgressiveCollapseBottomIndent.Text = SquareColumnsReinforcementSettingsT3Item.ProgressiveCollapseBottomIndent.ToString();
                }
                else
                {
                    textBox_ProgressiveCollapseBottomIndent.Text = "";
                }

                if (SquareColumnsReinforcementSettingsT3Item.ProgressiveCollapseSideLength != 0)
                {
                    textBox_ProgressiveCollapseSideLength.Text = SquareColumnsReinforcementSettingsT3Item.ProgressiveCollapseSideLength.ToString();
                }
                else
                {
                    textBox_ProgressiveCollapseSideLength.Text = "";
                }
            }
        }
        private void SetSavedSettingsT4()
        {
            if (SquareColumnsReinforcementSettingsT4Item != null)
            {
                //Задание сохраненных форм
                if (RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT4Item.Form01Name) != null)
                {
                    comboBox_Form01.SelectedItem = RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT4Item.Form01Name);
                }
                else
                {
                    if (comboBox_Form01.Items.Count != 0)
                    {
                        comboBox_Form01.SelectedItem = comboBox_Form01.Items.GetItemAt(0);
                    }
                }

                if (RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT4Item.Form26Name) != null)
                {
                    comboBox_Form26.SelectedItem = RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT4Item.Form26Name);
                }
                else
                {
                    if (comboBox_Form26.Items.Count != 0)
                    {
                        comboBox_Form26.SelectedItem = comboBox_Form26.Items.GetItemAt(0);
                    }
                }

                if (RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT4Item.Form11Name) != null)
                {
                    comboBox_Form11.SelectedItem = RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT4Item.Form11Name);
                }
                else
                {
                    if (comboBox_Form11.Items.Count != 0)
                    {
                        comboBox_Form11.SelectedItem = comboBox_Form11.Items.GetItemAt(0);
                    }
                }

                if (RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT4Item.Form51Name) != null)
                {
                    comboBox_Form51.SelectedItem = RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT4Item.Form51Name);
                }
                else
                {
                    if (comboBox_Form51.Items.Count != 0)
                    {
                        comboBox_Form51.SelectedItem = comboBox_Form51.Items.GetItemAt(0);
                    }
                }

                if (RebarHookTypeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT4Item.RebarHookTypeForStirrupName) != null)
                {
                    comboBox_RebarHookType.SelectedItem = RebarHookTypeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT4Item.RebarHookTypeForStirrupName);
                }
                else
                {
                    if (comboBox_RebarHookType.Items.Count != 0)
                    {
                        comboBox_RebarHookType.SelectedItem = comboBox_RebarHookType.Items.GetItemAt(0);
                    }
                }

                //Заполнение сохраненных параметров сечения
                if (RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT4Item.FirstMainBarTapeName) != null)
                {
                    comboBox_FirstMainBarTapes.SelectedItem = RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT4Item.FirstMainBarTapeName);
                }
                else
                {
                    if (comboBox_FirstMainBarTapes.Items.Count != 0)
                    {
                        comboBox_FirstMainBarTapes.SelectedItem = comboBox_FirstMainBarTapes.Items.GetItemAt(0);
                    }
                }

                if (RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT4Item.SecondMainBarTapeName) != null)
                {
                    comboBox_SecondMainBarTapes.SelectedItem = RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT4Item.SecondMainBarTapeName);
                }
                else
                {
                    if (comboBox_SecondMainBarTapes.Items.Count != 0)
                    {
                        comboBox_SecondMainBarTapes.SelectedItem = comboBox_SecondMainBarTapes.Items.GetItemAt(0);
                    }
                }

                if (RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT4Item.FirstStirrupBarTapeName) != null)
                {
                    comboBox_FirstStirrupBarTapes.SelectedItem = RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT4Item.FirstStirrupBarTapeName);
                }
                else
                {
                    if (comboBox_FirstStirrupBarTapes.Items.Count != 0)
                    {
                        comboBox_FirstStirrupBarTapes.SelectedItem = comboBox_FirstStirrupBarTapes.Items.GetItemAt(0);
                    }
                }

                if (RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT4Item.SecondStirrupBarTapeName) != null)
                {
                    comboBox_SecondStirrupBarTapes.SelectedItem = RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT4Item.SecondStirrupBarTapeName);
                }
                else
                {
                    if (comboBox_SecondStirrupBarTapes.Items.Count != 0)
                    {
                        comboBox_SecondStirrupBarTapes.SelectedItem = comboBox_SecondStirrupBarTapes.Items.GetItemAt(0);
                    }
                }

                if (RebarCoverTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT4Item.ColumnRebarCoverTypeName) != null)
                {
                    comboBox_RebarCoverTypes.SelectedItem = RebarCoverTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT4Item.ColumnRebarCoverTypeName);
                }
                else
                {
                    if (comboBox_RebarCoverTypes.Items.Count != 0)
                    {
                        comboBox_RebarCoverTypes.SelectedItem = comboBox_RebarCoverTypes.Items.GetItemAt(0);
                    }
                }

                textBox_FirstTopRebarOffset.Text = SquareColumnsReinforcementSettingsT4Item.FirstTopRebarOffset.ToString();
                textBox_FirstLowerRebarOffset.Text = SquareColumnsReinforcementSettingsT4Item.FirstLowerRebarOffset.ToString();
                textBox_FirstLeftRebarOffset.Text = SquareColumnsReinforcementSettingsT4Item.FirstLeftRebarOffset.ToString();
                textBox_FirstRightRebarOffset.Text = SquareColumnsReinforcementSettingsT4Item.FirstRightRebarOffset.ToString();

                //Заполнение сохраненных параметров на 3D
                textBox_FirstRebarOutletsLength.Text = SquareColumnsReinforcementSettingsT4Item.FirstRebarOutletsLength.ToString();
                textBox_SecondRebarOutletsLength.Text = SquareColumnsReinforcementSettingsT4Item.SecondRebarOutletsLength.ToString();
                textBox_StandardStirrupStep.Text = SquareColumnsReinforcementSettingsT4Item.StandardStirrupStep.ToString();
                textBox_FrequentButtomStirrupStep.Text = SquareColumnsReinforcementSettingsT4Item.FrequentButtomStirrupStep.ToString();
                textBox_FrequentButtomStirrupPlacementHeight.Text = SquareColumnsReinforcementSettingsT4Item.FrequentButtomStirrupPlacementHeight.ToString();
                textBox_FrequentTopStirrupStep.Text = SquareColumnsReinforcementSettingsT4Item.FrequentTopStirrupStep.ToString();
                textBox_FrequentTopStirrupPlacementHeight.Text = SquareColumnsReinforcementSettingsT4Item.FrequentTopStirrupPlacementHeight.ToString();
                textBox_FloorThickness.Text = SquareColumnsReinforcementSettingsT4Item.FloorThickness.ToString();
                textBox_FirstStirrupButtomOffset.Text = SquareColumnsReinforcementSettingsT4Item.FirstStirrupButtomOffset.ToString();

                //Заполнение сохраненных полей для соединения арматуры
                if (SquareColumnsReinforcementSettingsT4Item.RebarConnectionOptionName == "radioButton_Overlap")
                {
                    radioButton_Overlap.IsChecked = true;
                }
                else
                {
                    radioButton_Mechanical.IsChecked = true;
                }
                checkBox_DeepenRebar.IsChecked = SquareColumnsReinforcementSettingsT4Item.DeepenRebarChecked;
                if (SquareColumnsReinforcementSettingsT4Item.DeepenRebar != 0)
                {
                    textBox_DeepenRebar.Text = SquareColumnsReinforcementSettingsT4Item.DeepenRebar.ToString();
                }
                else
                {
                    textBox_DeepenRebar.Text = "";
                }

                checkBox_OverlapTransition.IsChecked = SquareColumnsReinforcementSettingsT4Item.OverlapTransitionChecked;

                if (SquareColumnsReinforcementSettingsT4Item.MechanicalConnectionOptionName == "radioButton_WeldedConnection")
                {
                    radioButton_WeldedConnection.IsChecked = true;
                }
                else
                {
                    radioButton_CouplingConnection.IsChecked = true;
                }

                if (RebarConnectionsList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT4Item.WeldedConnectionFamilyName) != null)
                {
                    comboBox_WeldedConnection.SelectedItem = RebarConnectionsList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT4Item.WeldedConnectionFamilyName);
                }
                else
                {
                    if (comboBox_WeldedConnection.Items.Count != 0)
                    {
                        comboBox_WeldedConnection.SelectedItem = comboBox_WeldedConnection.Items.GetItemAt(0);
                    }
                }

                if (RebarConnectionsList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT4Item.CouplingConnectionFamilyName) != null)
                {
                    comboBox_CouplingConnection.SelectedItem = RebarConnectionsList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT4Item.CouplingConnectionFamilyName);
                }
                else
                {
                    if (comboBox_CouplingConnection.Items.Count != 0)
                    {
                        comboBox_CouplingConnection.SelectedItem = comboBox_CouplingConnection.Items.GetItemAt(0);
                    }
                }

                //Заполнение сохраненных дополнительных параметров
                checkBox_BendInSlab.IsChecked = SquareColumnsReinforcementSettingsT4Item.BendInSlabChecked;
                if (SquareColumnsReinforcementSettingsT4Item.BendInSlab != 0)
                {
                    textBox_BendInSlab.Text = SquareColumnsReinforcementSettingsT4Item.BendInSlab.ToString();
                }
                else
                {
                    textBox_BendInSlab.Text = "";
                }

                checkBox_SectionChange.IsChecked = SquareColumnsReinforcementSettingsT4Item.SectionChangeChecked;
                if (SquareColumnsReinforcementSettingsT4Item.SectionChange != 0)
                {
                    textBox_SectionChange.Text = SquareColumnsReinforcementSettingsT4Item.SectionChange.ToString();
                }
                else
                {
                    textBox_SectionChange.Text = "";
                }

                //Заполнение сохраненных параметров прогрессирующего обрушения
                checkBox_ProgressiveCollapseBarIntoSlab.IsChecked = SquareColumnsReinforcementSettingsT4Item.ProgressiveCollapseBarIntoSlabChecked;

                if (RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT4Item.ProgressiveCollapseBarTapeName) != null)
                {
                    comboBox_ProgressiveCollapseBarTapes.SelectedItem = RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT4Item.ProgressiveCollapseBarTapeName);
                }
                else
                {
                    if (comboBox_ProgressiveCollapseBarTapes.Items.Count != 0)
                    {
                        comboBox_ProgressiveCollapseBarTapes.SelectedItem = comboBox_ProgressiveCollapseBarTapes.Items.GetItemAt(0);
                    }
                }

                if (SquareColumnsReinforcementSettingsT4Item.ProgressiveCollapseColumnCenterOffset != 0)
                {
                    textBox_ProgressiveCollapseColumnCenterOffset.Text = SquareColumnsReinforcementSettingsT4Item.ProgressiveCollapseColumnCenterOffset.ToString();
                }
                else
                {
                    textBox_ProgressiveCollapseColumnCenterOffset.Text = "";
                }

                if (SquareColumnsReinforcementSettingsT4Item.ProgressiveCollapseUpLength != 0)
                {
                    textBox_ProgressiveCollapseUpLength.Text = SquareColumnsReinforcementSettingsT4Item.ProgressiveCollapseUpLength.ToString();
                }
                else
                {
                    textBox_ProgressiveCollapseUpLength.Text = "";
                }

                if (SquareColumnsReinforcementSettingsT4Item.ProgressiveCollapseBottomIndent != 0)
                {
                    textBox_ProgressiveCollapseBottomIndent.Text = SquareColumnsReinforcementSettingsT4Item.ProgressiveCollapseBottomIndent.ToString();
                }
                else
                {
                    textBox_ProgressiveCollapseBottomIndent.Text = "";
                }

                if (SquareColumnsReinforcementSettingsT4Item.ProgressiveCollapseSideLength != 0)
                {
                    textBox_ProgressiveCollapseSideLength.Text = SquareColumnsReinforcementSettingsT4Item.ProgressiveCollapseSideLength.ToString();
                }
                else
                {
                    textBox_ProgressiveCollapseSideLength.Text = "";
                }
            }
        }
        private void SetSavedSettingsT5()
        {
            if (SquareColumnsReinforcementSettingsT5Item != null)
            {
                //Задание сохраненных форм
                if (RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT5Item.Form01Name) != null)
                {
                    comboBox_Form01.SelectedItem = RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT5Item.Form01Name);
                }
                else
                {
                    if (comboBox_Form01.Items.Count != 0)
                    {
                        comboBox_Form01.SelectedItem = comboBox_Form01.Items.GetItemAt(0);
                    }
                }

                if (RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT5Item.Form26Name) != null)
                {
                    comboBox_Form26.SelectedItem = RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT5Item.Form26Name);
                }
                else
                {
                    if (comboBox_Form26.Items.Count != 0)
                    {
                        comboBox_Form26.SelectedItem = comboBox_Form26.Items.GetItemAt(0);
                    }
                }

                if (RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT5Item.Form11Name) != null)
                {
                    comboBox_Form11.SelectedItem = RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT5Item.Form11Name);
                }
                else
                {
                    if (comboBox_Form11.Items.Count != 0)
                    {
                        comboBox_Form11.SelectedItem = comboBox_Form11.Items.GetItemAt(0);
                    }
                }

                if (RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT5Item.Form51Name) != null)
                {
                    comboBox_Form51.SelectedItem = RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT5Item.Form51Name);
                }
                else
                {
                    if (comboBox_Form51.Items.Count != 0)
                    {
                        comboBox_Form51.SelectedItem = comboBox_Form51.Items.GetItemAt(0);
                    }
                }

                if (RebarHookTypeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT5Item.RebarHookTypeForStirrupName) != null)
                {
                    comboBox_RebarHookType.SelectedItem = RebarHookTypeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT5Item.RebarHookTypeForStirrupName);
                }
                else
                {
                    if (comboBox_RebarHookType.Items.Count != 0)
                    {
                        comboBox_RebarHookType.SelectedItem = comboBox_RebarHookType.Items.GetItemAt(0);
                    }
                }

                //Заполнение сохраненных параметров сечения
                if (RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT5Item.FirstMainBarTapeName) != null)
                {
                    comboBox_FirstMainBarTapes.SelectedItem = RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT5Item.FirstMainBarTapeName);
                }
                else
                {
                    if (comboBox_FirstMainBarTapes.Items.Count != 0)
                    {
                        comboBox_FirstMainBarTapes.SelectedItem = comboBox_FirstMainBarTapes.Items.GetItemAt(0);
                    }
                }

                if (RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT5Item.SecondMainBarTapeName) != null)
                {
                    comboBox_SecondMainBarTapes.SelectedItem = RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT5Item.SecondMainBarTapeName);
                }
                else
                {
                    if (comboBox_SecondMainBarTapes.Items.Count != 0)
                    {
                        comboBox_SecondMainBarTapes.SelectedItem = comboBox_SecondMainBarTapes.Items.GetItemAt(0);
                    }
                }

                if (RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT5Item.FirstStirrupBarTapeName) != null)
                {
                    comboBox_FirstStirrupBarTapes.SelectedItem = RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT5Item.FirstStirrupBarTapeName);
                }
                else
                {
                    if (comboBox_FirstStirrupBarTapes.Items.Count != 0)
                    {
                        comboBox_FirstStirrupBarTapes.SelectedItem = comboBox_FirstStirrupBarTapes.Items.GetItemAt(0);
                    }
                }

                if (RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT5Item.SecondStirrupBarTapeName) != null)
                {
                    comboBox_SecondStirrupBarTapes.SelectedItem = RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT5Item.SecondStirrupBarTapeName);
                }
                else
                {
                    if (comboBox_SecondStirrupBarTapes.Items.Count != 0)
                    {
                        comboBox_SecondStirrupBarTapes.SelectedItem = comboBox_SecondStirrupBarTapes.Items.GetItemAt(0);
                    }
                }

                if (RebarCoverTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT5Item.ColumnRebarCoverTypeName) != null)
                {
                    comboBox_RebarCoverTypes.SelectedItem = RebarCoverTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT5Item.ColumnRebarCoverTypeName);
                }
                else
                {
                    if (comboBox_RebarCoverTypes.Items.Count != 0)
                    {
                        comboBox_RebarCoverTypes.SelectedItem = comboBox_RebarCoverTypes.Items.GetItemAt(0);
                    }
                }

                textBox_FirstTopRebarOffset.Text = SquareColumnsReinforcementSettingsT5Item.FirstTopRebarOffset.ToString();
                textBox_FirstLowerRebarOffset.Text = SquareColumnsReinforcementSettingsT5Item.FirstLowerRebarOffset.ToString();
                textBox_FirstLeftRebarOffset.Text = SquareColumnsReinforcementSettingsT5Item.FirstLeftRebarOffset.ToString();
                textBox_FirstRightRebarOffset.Text = SquareColumnsReinforcementSettingsT5Item.FirstRightRebarOffset.ToString();

                //Заполнение сохраненных параметров на 3D
                textBox_FirstRebarOutletsLength.Text = SquareColumnsReinforcementSettingsT5Item.FirstRebarOutletsLength.ToString();
                textBox_SecondRebarOutletsLength.Text = SquareColumnsReinforcementSettingsT5Item.SecondRebarOutletsLength.ToString();
                textBox_StandardStirrupStep.Text = SquareColumnsReinforcementSettingsT5Item.StandardStirrupStep.ToString();
                textBox_FrequentButtomStirrupStep.Text = SquareColumnsReinforcementSettingsT5Item.FrequentButtomStirrupStep.ToString();
                textBox_FrequentButtomStirrupPlacementHeight.Text = SquareColumnsReinforcementSettingsT5Item.FrequentButtomStirrupPlacementHeight.ToString();
                textBox_FrequentTopStirrupStep.Text = SquareColumnsReinforcementSettingsT5Item.FrequentTopStirrupStep.ToString();
                textBox_FrequentTopStirrupPlacementHeight.Text = SquareColumnsReinforcementSettingsT5Item.FrequentTopStirrupPlacementHeight.ToString();
                textBox_FloorThickness.Text = SquareColumnsReinforcementSettingsT5Item.FloorThickness.ToString();
                textBox_FirstStirrupButtomOffset.Text = SquareColumnsReinforcementSettingsT5Item.FirstStirrupButtomOffset.ToString();

                //Заполнение сохраненных полей для соединения арматуры
                if (SquareColumnsReinforcementSettingsT5Item.RebarConnectionOptionName == "radioButton_Overlap")
                {
                    radioButton_Overlap.IsChecked = true;
                }
                else
                {
                    radioButton_Mechanical.IsChecked = true;
                }
                checkBox_DeepenRebar.IsChecked = SquareColumnsReinforcementSettingsT5Item.DeepenRebarChecked;
                if (SquareColumnsReinforcementSettingsT5Item.DeepenRebar != 0)
                {
                    textBox_DeepenRebar.Text = SquareColumnsReinforcementSettingsT5Item.DeepenRebar.ToString();
                }
                else
                {
                    textBox_DeepenRebar.Text = "";
                }

                checkBox_OverlapTransition.IsChecked = SquareColumnsReinforcementSettingsT5Item.OverlapTransitionChecked;

                if (SquareColumnsReinforcementSettingsT5Item.MechanicalConnectionOptionName == "radioButton_WeldedConnection")
                {
                    radioButton_WeldedConnection.IsChecked = true;
                }
                else
                {
                    radioButton_CouplingConnection.IsChecked = true;
                }

                if (RebarConnectionsList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT5Item.WeldedConnectionFamilyName) != null)
                {
                    comboBox_WeldedConnection.SelectedItem = RebarConnectionsList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT5Item.WeldedConnectionFamilyName);
                }
                else
                {
                    if (comboBox_WeldedConnection.Items.Count != 0)
                    {
                        comboBox_WeldedConnection.SelectedItem = comboBox_WeldedConnection.Items.GetItemAt(0);
                    }
                }

                if (RebarConnectionsList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT5Item.CouplingConnectionFamilyName) != null)
                {
                    comboBox_CouplingConnection.SelectedItem = RebarConnectionsList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT5Item.CouplingConnectionFamilyName);
                }
                else
                {
                    if (comboBox_CouplingConnection.Items.Count != 0)
                    {
                        comboBox_CouplingConnection.SelectedItem = comboBox_CouplingConnection.Items.GetItemAt(0);
                    }
                }

                //Заполнение сохраненных дополнительных параметров
                checkBox_BendInSlab.IsChecked = SquareColumnsReinforcementSettingsT5Item.BendInSlabChecked;
                if (SquareColumnsReinforcementSettingsT5Item.BendInSlab != 0)
                {
                    textBox_BendInSlab.Text = SquareColumnsReinforcementSettingsT5Item.BendInSlab.ToString();
                }
                else
                {
                    textBox_BendInSlab.Text = "";
                }

                checkBox_SectionChange.IsChecked = SquareColumnsReinforcementSettingsT5Item.SectionChangeChecked;
                if (SquareColumnsReinforcementSettingsT5Item.SectionChange != 0)
                {
                    textBox_SectionChange.Text = SquareColumnsReinforcementSettingsT5Item.SectionChange.ToString();
                }
                else
                {
                    textBox_SectionChange.Text = "";
                }

                //Заполнение сохраненных параметров прогрессирующего обрушения
                checkBox_ProgressiveCollapseBarIntoSlab.IsChecked = SquareColumnsReinforcementSettingsT5Item.ProgressiveCollapseBarIntoSlabChecked;

                if (RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT5Item.ProgressiveCollapseBarTapeName) != null)
                {
                    comboBox_ProgressiveCollapseBarTapes.SelectedItem = RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT5Item.ProgressiveCollapseBarTapeName);
                }
                else
                {
                    if (comboBox_ProgressiveCollapseBarTapes.Items.Count != 0)
                    {
                        comboBox_ProgressiveCollapseBarTapes.SelectedItem = comboBox_ProgressiveCollapseBarTapes.Items.GetItemAt(0);
                    }
                }

                if (SquareColumnsReinforcementSettingsT5Item.ProgressiveCollapseColumnCenterOffset != 0)
                {
                    textBox_ProgressiveCollapseColumnCenterOffset.Text = SquareColumnsReinforcementSettingsT5Item.ProgressiveCollapseColumnCenterOffset.ToString();
                }
                else
                {
                    textBox_ProgressiveCollapseColumnCenterOffset.Text = "";
                }

                if (SquareColumnsReinforcementSettingsT5Item.ProgressiveCollapseUpLength != 0)
                {
                    textBox_ProgressiveCollapseUpLength.Text = SquareColumnsReinforcementSettingsT5Item.ProgressiveCollapseUpLength.ToString();
                }
                else
                {
                    textBox_ProgressiveCollapseUpLength.Text = "";
                }

                if (SquareColumnsReinforcementSettingsT5Item.ProgressiveCollapseBottomIndent != 0)
                {
                    textBox_ProgressiveCollapseBottomIndent.Text = SquareColumnsReinforcementSettingsT5Item.ProgressiveCollapseBottomIndent.ToString();
                }
                else
                {
                    textBox_ProgressiveCollapseBottomIndent.Text = "";
                }

                if (SquareColumnsReinforcementSettingsT5Item.ProgressiveCollapseSideLength != 0)
                {
                    textBox_ProgressiveCollapseSideLength.Text = SquareColumnsReinforcementSettingsT5Item.ProgressiveCollapseSideLength.ToString();
                }
                else
                {
                    textBox_ProgressiveCollapseSideLength.Text = "";
                }
            }
        }
        private void SetSavedSettingsT6()
        {
            if (SquareColumnsReinforcementSettingsT6Item != null)
            {
                //Задание сохраненных форм
                if (RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT6Item.Form01Name) != null)
                {
                    comboBox_Form01.SelectedItem = RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT6Item.Form01Name);
                }
                else
                {
                    if (comboBox_Form01.Items.Count != 0)
                    {
                        comboBox_Form01.SelectedItem = comboBox_Form01.Items.GetItemAt(0);
                    }
                }

                if (RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT6Item.Form26Name) != null)
                {
                    comboBox_Form26.SelectedItem = RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT6Item.Form26Name);
                }
                else
                {
                    if (comboBox_Form26.Items.Count != 0)
                    {
                        comboBox_Form26.SelectedItem = comboBox_Form26.Items.GetItemAt(0);
                    }
                }

                if (RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT6Item.Form11Name) != null)
                {
                    comboBox_Form11.SelectedItem = RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT6Item.Form11Name);
                }
                else
                {
                    if (comboBox_Form11.Items.Count != 0)
                    {
                        comboBox_Form11.SelectedItem = comboBox_Form11.Items.GetItemAt(0);
                    }
                }

                if (RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT6Item.Form51Name) != null)
                {
                    comboBox_Form51.SelectedItem = RebarShapeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT6Item.Form51Name);
                }
                else
                {
                    if (comboBox_Form51.Items.Count != 0)
                    {
                        comboBox_Form51.SelectedItem = comboBox_Form51.Items.GetItemAt(0);
                    }
                }

                if (RebarHookTypeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT6Item.RebarHookTypeForStirrupName) != null)
                {
                    comboBox_RebarHookType.SelectedItem = RebarHookTypeList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT6Item.RebarHookTypeForStirrupName);
                }
                else
                {
                    if (comboBox_RebarHookType.Items.Count != 0)
                    {
                        comboBox_RebarHookType.SelectedItem = comboBox_RebarHookType.Items.GetItemAt(0);
                    }
                }

                //Заполнение сохраненных параметров сечения
                if (RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT6Item.FirstMainBarTapeName) != null)
                {
                    comboBox_FirstMainBarTapes.SelectedItem = RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT6Item.FirstMainBarTapeName);
                }
                else
                {
                    if (comboBox_FirstMainBarTapes.Items.Count != 0)
                    {
                        comboBox_FirstMainBarTapes.SelectedItem = comboBox_FirstMainBarTapes.Items.GetItemAt(0);
                    }
                }

                if (RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT6Item.SecondMainBarTapeName) != null)
                {
                    comboBox_SecondMainBarTapes.SelectedItem = RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT6Item.SecondMainBarTapeName);
                }
                else
                {
                    if (comboBox_SecondMainBarTapes.Items.Count != 0)
                    {
                        comboBox_SecondMainBarTapes.SelectedItem = comboBox_SecondMainBarTapes.Items.GetItemAt(0);
                    }
                }

                if (RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT6Item.FirstStirrupBarTapeName) != null)
                {
                    comboBox_FirstStirrupBarTapes.SelectedItem = RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT6Item.FirstStirrupBarTapeName);
                }
                else
                {
                    if (comboBox_FirstStirrupBarTapes.Items.Count != 0)
                    {
                        comboBox_FirstStirrupBarTapes.SelectedItem = comboBox_FirstStirrupBarTapes.Items.GetItemAt(0);
                    }
                }

                if (RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT6Item.SecondStirrupBarTapeName) != null)
                {
                    comboBox_SecondStirrupBarTapes.SelectedItem = RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT6Item.SecondStirrupBarTapeName);
                }
                else
                {
                    if (comboBox_SecondStirrupBarTapes.Items.Count != 0)
                    {
                        comboBox_SecondStirrupBarTapes.SelectedItem = comboBox_SecondStirrupBarTapes.Items.GetItemAt(0);
                    }
                }

                if (RebarCoverTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT6Item.ColumnRebarCoverTypeName) != null)
                {
                    comboBox_RebarCoverTypes.SelectedItem = RebarCoverTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT6Item.ColumnRebarCoverTypeName);
                }
                else
                {
                    if (comboBox_RebarCoverTypes.Items.Count != 0)
                    {
                        comboBox_RebarCoverTypes.SelectedItem = comboBox_RebarCoverTypes.Items.GetItemAt(0);
                    }
                }

                textBox_FirstTopRebarOffset.Text = SquareColumnsReinforcementSettingsT6Item.FirstTopRebarOffset.ToString();
                textBox_FirstLowerRebarOffset.Text = SquareColumnsReinforcementSettingsT6Item.FirstLowerRebarOffset.ToString();
                textBox_FirstLeftRebarOffset.Text = SquareColumnsReinforcementSettingsT6Item.FirstLeftRebarOffset.ToString();
                textBox_FirstRightRebarOffset.Text = SquareColumnsReinforcementSettingsT6Item.FirstRightRebarOffset.ToString();

                textBox_SecondTopRebarOffset.Text = SquareColumnsReinforcementSettingsT6Item.SecondTopRebarOffset.ToString();
                textBox_SecondLowerRebarOffset.Text = SquareColumnsReinforcementSettingsT6Item.SecondLowerRebarOffset.ToString();
                textBox_SecondLeftRebarOffset.Text = SquareColumnsReinforcementSettingsT6Item.SecondLeftRebarOffset.ToString();
                textBox_SecondRightRebarOffset.Text = SquareColumnsReinforcementSettingsT6Item.SecondRightRebarOffset.ToString();

                //Заполнение сохраненных параметров на 3D
                textBox_FirstRebarOutletsLength.Text = SquareColumnsReinforcementSettingsT6Item.FirstRebarOutletsLength.ToString();
                textBox_SecondRebarOutletsLength.Text = SquareColumnsReinforcementSettingsT6Item.SecondRebarOutletsLength.ToString();
                textBox_StandardStirrupStep.Text = SquareColumnsReinforcementSettingsT6Item.StandardStirrupStep.ToString();
                textBox_FrequentButtomStirrupStep.Text = SquareColumnsReinforcementSettingsT6Item.FrequentButtomStirrupStep.ToString();
                textBox_FrequentButtomStirrupPlacementHeight.Text = SquareColumnsReinforcementSettingsT6Item.FrequentButtomStirrupPlacementHeight.ToString();
                textBox_FrequentTopStirrupStep.Text = SquareColumnsReinforcementSettingsT6Item.FrequentTopStirrupStep.ToString();
                textBox_FrequentTopStirrupPlacementHeight.Text = SquareColumnsReinforcementSettingsT6Item.FrequentTopStirrupPlacementHeight.ToString();
                textBox_FloorThickness.Text = SquareColumnsReinforcementSettingsT6Item.FloorThickness.ToString();
                textBox_FirstStirrupButtomOffset.Text = SquareColumnsReinforcementSettingsT6Item.FirstStirrupButtomOffset.ToString();

                //Заполнение сохраненных полей для соединения арматуры
                if (SquareColumnsReinforcementSettingsT6Item.RebarConnectionOptionName == "radioButton_Overlap")
                {
                    radioButton_Overlap.IsChecked = true;
                }
                else
                {
                    radioButton_Mechanical.IsChecked = true;
                }
                checkBox_DeepenRebar.IsChecked = SquareColumnsReinforcementSettingsT6Item.DeepenRebarChecked;
                if (SquareColumnsReinforcementSettingsT6Item.DeepenRebar != 0)
                {
                    textBox_DeepenRebar.Text = SquareColumnsReinforcementSettingsT6Item.DeepenRebar.ToString();
                }
                else
                {
                    textBox_DeepenRebar.Text = "";
                }

                checkBox_OverlapTransition.IsChecked = SquareColumnsReinforcementSettingsT6Item.OverlapTransitionChecked;

                if (SquareColumnsReinforcementSettingsT6Item.MechanicalConnectionOptionName == "radioButton_WeldedConnection")
                {
                    radioButton_WeldedConnection.IsChecked = true;
                }
                else
                {
                    radioButton_CouplingConnection.IsChecked = true;
                }

                if (RebarConnectionsList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT6Item.WeldedConnectionFamilyName) != null)
                {
                    comboBox_WeldedConnection.SelectedItem = RebarConnectionsList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT6Item.WeldedConnectionFamilyName);
                }
                else
                {
                    if (comboBox_WeldedConnection.Items.Count != 0)
                    {
                        comboBox_WeldedConnection.SelectedItem = comboBox_WeldedConnection.Items.GetItemAt(0);
                    }
                }

                if (RebarConnectionsList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT6Item.CouplingConnectionFamilyName) != null)
                {
                    comboBox_CouplingConnection.SelectedItem = RebarConnectionsList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT6Item.CouplingConnectionFamilyName);
                }
                else
                {
                    if (comboBox_CouplingConnection.Items.Count != 0)
                    {
                        comboBox_CouplingConnection.SelectedItem = comboBox_CouplingConnection.Items.GetItemAt(0);
                    }
                }

                //Заполнение сохраненных дополнительных параметров
                checkBox_BendInSlab.IsChecked = SquareColumnsReinforcementSettingsT6Item.BendInSlabChecked;
                if (SquareColumnsReinforcementSettingsT6Item.BendInSlab != 0)
                {
                    textBox_BendInSlab.Text = SquareColumnsReinforcementSettingsT6Item.BendInSlab.ToString();
                }
                else
                {
                    textBox_BendInSlab.Text = "";
                }

                checkBox_SectionChange.IsChecked = SquareColumnsReinforcementSettingsT6Item.SectionChangeChecked;
                if (SquareColumnsReinforcementSettingsT6Item.SectionChange != 0)
                {
                    textBox_SectionChange.Text = SquareColumnsReinforcementSettingsT6Item.SectionChange.ToString();
                }
                else
                {
                    textBox_SectionChange.Text = "";
                }

                //Заполнение сохраненных параметров прогрессирующего обрушения
                checkBox_ProgressiveCollapseBarIntoSlab.IsChecked = SquareColumnsReinforcementSettingsT6Item.ProgressiveCollapseBarIntoSlabChecked;

                if (RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT6Item.ProgressiveCollapseBarTapeName) != null)
                {
                    comboBox_ProgressiveCollapseBarTapes.SelectedItem = RebarBarTypesList.FirstOrDefault(rbt => rbt.Name == SquareColumnsReinforcementSettingsT6Item.ProgressiveCollapseBarTapeName);
                }
                else
                {
                    if (comboBox_ProgressiveCollapseBarTapes.Items.Count != 0)
                    {
                        comboBox_ProgressiveCollapseBarTapes.SelectedItem = comboBox_ProgressiveCollapseBarTapes.Items.GetItemAt(0);
                    }
                }

                if (SquareColumnsReinforcementSettingsT6Item.ProgressiveCollapseColumnCenterOffset != 0)
                {
                    textBox_ProgressiveCollapseColumnCenterOffset.Text = SquareColumnsReinforcementSettingsT6Item.ProgressiveCollapseColumnCenterOffset.ToString();
                }
                else
                {
                    textBox_ProgressiveCollapseColumnCenterOffset.Text = "";
                }

                if (SquareColumnsReinforcementSettingsT6Item.ProgressiveCollapseUpLength != 0)
                {
                    textBox_ProgressiveCollapseUpLength.Text = SquareColumnsReinforcementSettingsT6Item.ProgressiveCollapseUpLength.ToString();
                }
                else
                {
                    textBox_ProgressiveCollapseUpLength.Text = "";
                }

                if (SquareColumnsReinforcementSettingsT6Item.ProgressiveCollapseBottomIndent != 0)
                {
                    textBox_ProgressiveCollapseBottomIndent.Text = SquareColumnsReinforcementSettingsT6Item.ProgressiveCollapseBottomIndent.ToString();
                }
                else
                {
                    textBox_ProgressiveCollapseBottomIndent.Text = "";
                }

                if (SquareColumnsReinforcementSettingsT6Item.ProgressiveCollapseSideLength != 0)
                {
                    textBox_ProgressiveCollapseSideLength.Text = SquareColumnsReinforcementSettingsT6Item.ProgressiveCollapseSideLength.ToString();
                }
                else
                {
                    textBox_ProgressiveCollapseSideLength.Text = "";
                }
            }
        }
    }
}
