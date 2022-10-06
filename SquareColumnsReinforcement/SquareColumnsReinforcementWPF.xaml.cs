using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SquareColumnsReinforcement
{
    public partial class SquareColumnsReinforcementWPF : Window
    {
        List<RebarBarType> RebarBarTypesList;
        List<RebarCoverType> RebarCoverTypesList;

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

        public SquareColumnsReinforcementWPF(List<RebarBarType> rebarBarTypesList
            , List<RebarCoverType> rebarCoverTypesList
            , List<RebarShape> rebarShapeList
            , List<RebarHookType> rebarHookTypeList
            , List<Family> rebarConnectionsList)
        {
            RebarBarTypesList = rebarBarTypesList;
            RebarCoverTypesList = rebarCoverTypesList;

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

            comboBox_Form01.ItemsSource = rebarShapeList;
            comboBox_Form01.DisplayMemberPath = "Name";

            comboBox_Form26.ItemsSource = rebarShapeList;
            comboBox_Form26.DisplayMemberPath = "Name";

            comboBox_Form11.ItemsSource = rebarShapeList;
            comboBox_Form11.DisplayMemberPath = "Name";

            comboBox_Form51.ItemsSource = rebarShapeList;
            comboBox_Form51.DisplayMemberPath = "Name";

            comboBox_ProgressiveCollapseBarTapes.ItemsSource = RebarBarTypesList;
            comboBox_ProgressiveCollapseBarTapes.DisplayMemberPath = "Name";

            comboBox_RebarHookType.ItemsSource = rebarHookTypeList;
            comboBox_RebarHookType.DisplayMemberPath = "Name";

            comboBox_WeldedConnection.ItemsSource = rebarConnectionsList;
            comboBox_WeldedConnection.DisplayMemberPath = "Name";

            comboBox_CouplingConnection.ItemsSource = rebarConnectionsList;
            comboBox_CouplingConnection.DisplayMemberPath = "Name";
        }
        private void btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            DialogResult = true;
            Close();
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
                DialogResult = true;
                Close();
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
            FirstMainBarTape = comboBox_FirstMainBarTapes.SelectedItem as RebarBarType;
            SecondMainBarTape = comboBox_SecondMainBarTapes.SelectedItem as RebarBarType;
            FirstStirrupBarTape = comboBox_FirstStirrupBarTapes.SelectedItem as RebarBarType;
            SecondStirrupBarTape = comboBox_SecondStirrupBarTapes.SelectedItem as RebarBarType;
            ColumnRebarCoverType = comboBox_RebarCoverTypes.SelectedItem as RebarCoverType;

            double.TryParse(textBox_FirstTopRebarOffset.Text, out FirstTopRebarOffset);
            double.TryParse(textBox_FirstLowerRebarOffset.Text, out FirstLowerRebarOffset);
            double.TryParse(textBox_FirstLeftRebarOffset.Text, out FirstLeftRebarOffset);
            double.TryParse(textBox_FirstRightRebarOffset.Text, out FirstRightRebarOffset);

            double.TryParse(textBox_SecondTopRebarOffset.Text, out SecondTopRebarOffset);
            double.TryParse(textBox_SecondLowerRebarOffset.Text, out SecondLowerRebarOffset);
            double.TryParse(textBox_SecondLeftRebarOffset.Text, out SecondLeftRebarOffset);
            double.TryParse(textBox_SecondRightRebarOffset.Text, out SecondRightRebarOffset);

            RebarConnectionOptionName = (this.groupBox_RebarConnectionOptions.Content as System.Windows.Controls.Grid)
                .Children.OfType<RadioButton>()
                .FirstOrDefault(rb => rb.IsChecked.Value == true)
                .Name;

            DeepenRebarChecked = (bool)checkBox_DeepenRebar.IsChecked;
            double.TryParse(textBox_DeepenRebar.Text, out DeepenRebar);

            OverlapTransitionChecked = (bool)checkBox_OverlapTransition.IsChecked;

            double.TryParse(textBox_FirstRebarOutletsLength.Text, out FirstRebarOutletsLength);
            double.TryParse(textBox_SecondRebarOutletsLength.Text, out SecondRebarOutletsLength);
            double.TryParse(textBox_StandardStirrupStep.Text, out StandardStirrupStep);
            double.TryParse(textBox_FrequentButtomStirrupStep.Text, out FrequentButtomStirrupStep);
            double.TryParse(textBox_FrequentButtomStirrupPlacementHeight.Text, out FrequentButtomStirrupPlacementHeight);
            double.TryParse(textBox_FrequentTopStirrupStep.Text, out FrequentTopStirrupStep);
            double.TryParse(textBox_FrequentTopStirrupPlacementHeight.Text, out FrequentTopStirrupPlacementHeight);
            double.TryParse(textBox_FloorThickness.Text, out FloorThickness);
            double.TryParse(textBox_FirstStirrupButtomOffset.Text, out FirstStirrupButtomOffset);

            BendInSlabChecked = (bool)checkBox_BendInSlab.IsChecked;
            double.TryParse(textBox_BendInSlab.Text, out BendInSlab);

            SectionChangeChecked = (bool)checkBox_SectionChange.IsChecked;
            double.TryParse(textBox_SectionChange.Text, out SectionChange);

            Form01 = comboBox_Form01.SelectedItem as RebarShape;
            Form26 = comboBox_Form26.SelectedItem as RebarShape;
            Form11 = comboBox_Form11.SelectedItem as RebarShape;
            Form51 = comboBox_Form51.SelectedItem as RebarShape;
            RebarHookTypeForStirrup = comboBox_RebarHookType.SelectedItem as RebarHookType;

            ProgressiveCollapseBarIntoSlabChecked = (bool)checkBox_ProgressiveCollapseBarIntoSlab.IsChecked;
            ProgressiveCollapseBarTape = comboBox_ProgressiveCollapseBarTapes.SelectedItem as RebarBarType;
            double.TryParse(textBox_ProgressiveCollapseColumnCenterOffset.Text, out ProgressiveCollapseColumnCenterOffset);
            double.TryParse(textBox_ProgressiveCollapseUpLength.Text, out ProgressiveCollapseUpLength);
            double.TryParse(textBox_ProgressiveCollapseBottomIndent.Text, out ProgressiveCollapseBottomIndent);
            double.TryParse(textBox_ProgressiveCollapseSideLength.Text, out ProgressiveCollapseSideLength);

            MechanicalConnectionOptionName = (this.groupBox_MechanicalConnectionOptions.Content as System.Windows.Controls.Grid)
                .Children.OfType<RadioButton>()
                .FirstOrDefault(rb => rb.IsChecked.Value == true)
                .Name;

            WeldedConnectionFamily = comboBox_WeldedConnection.SelectedItem as Family;
            CouplingConnectionFamily = comboBox_CouplingConnection.SelectedItem as Family;
        }
    }
}
