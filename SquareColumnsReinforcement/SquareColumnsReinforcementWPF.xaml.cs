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
        public SquareColumnsReinforcementWPF(List<RebarBarType> rebarBarTypesList, List<RebarCoverType> rebarCoverTypesList, List<RebarShape> rebarShapeList, List<RebarHookType> rebarHookTypeList)
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

            comboBox_RebarHookType.ItemsSource = rebarHookTypeList;
            comboBox_RebarHookType.DisplayMemberPath = "Name";
        }
        private void btn_Ok_Click(object sender, RoutedEventArgs e)
        {
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
            SetBorderForSelectedButton(sender);
            SetBorderForNonSelectedButtons(sender);
            image_Sections.Source = new BitmapImage(new Uri("pack://application:,,,/SquareColumnsReinforcement;component/Resources/Sections/Type1_Section.png"));
            image_3D.Source = new BitmapImage(new Uri("pack://application:,,,/SquareColumnsReinforcement;component/Resources/3D/Type1_3D.png"));

            //Типы арматуры в сечении
            comboBox_FirstMainBarTapes.Visibility = Visibility.Visible;
            comboBox_FirstMainBarTapes.Margin = new Thickness(30, 34, 0, 0);

            comboBox_SecondMainBarTapes.Visibility = Visibility.Hidden;
            comboBox_SecondMainBarTapes.Margin = new Thickness(30, 34, 0, 0);

            comboBox_FirstStirrupBarTapes.Visibility = Visibility.Visible;
            comboBox_FirstStirrupBarTapes.Margin = new Thickness(300, 161, 0, 0);

            comboBox_SecondStirrupBarTapes.Visibility = Visibility.Hidden;
            comboBox_SecondStirrupBarTapes.Margin = new Thickness(300, 161, 0, 0);

            comboBox_RebarCoverTypes.Visibility = Visibility.Visible;
            comboBox_RebarCoverTypes.Margin = new Thickness(70, 204, 0, 0);

            //Смещение стержней в сечении
            textBox_FirstTopRebarOffset.Visibility = Visibility.Hidden;
            textBox_FirstTopRebarOffset.Margin = new Thickness(55, 110, 0, 0);

            textBox_FirstLowerRebarOffset.Visibility = Visibility.Hidden;
            textBox_FirstLowerRebarOffset.Margin = new Thickness(55, 144, 0, 0);

            textBox_FirstLeftRebarOffset.Visibility = Visibility.Hidden;
            textBox_FirstLeftRebarOffset.Margin = new Thickness(168, 245, 0, 0);

            textBox_FirstRightRebarOffset.Visibility = Visibility.Hidden;
            textBox_FirstRightRebarOffset.Margin = new Thickness(216, 245, 0, 0);


            textBox_SecondTopRebarOffset.Visibility = Visibility.Hidden;
            textBox_SecondTopRebarOffset.Margin = new Thickness(55, 0, 0, 87);

            textBox_SecondLowerRebarOffset.Visibility = Visibility.Hidden;
            textBox_SecondLowerRebarOffset.Margin = new Thickness(55, 87, 0, 0);

            textBox_SecondLeftRebarOffset.Visibility = Visibility.Hidden;
            textBox_SecondLeftRebarOffset.Margin = new Thickness(0, 245, 150, 0);

            textBox_SecondRightRebarOffset.Visibility = Visibility.Hidden;
            textBox_SecondRightRebarOffset.Margin = new Thickness(150, 245, 0, 0);
        }

        private void button_Type2_Click(object sender, RoutedEventArgs e)
        {
            SetBorderForSelectedButton(sender);
            SetBorderForNonSelectedButtons(sender);
            image_Sections.Source = new BitmapImage(new Uri("pack://application:,,,/SquareColumnsReinforcement;component/Resources/Sections/Type2_Section.png"));
            image_3D.Source = new BitmapImage(new Uri("pack://application:,,,/SquareColumnsReinforcement;component/Resources/3D/Type2_3D.png"));

            //Типы арматуры в сечении
            comboBox_FirstMainBarTapes.Visibility = Visibility.Visible;
            comboBox_FirstMainBarTapes.Margin = new Thickness(30, 34, 0, 0);

            comboBox_SecondMainBarTapes.Visibility = Visibility.Visible;
            comboBox_SecondMainBarTapes.Margin = new Thickness(300, 209, 0, 0);

            comboBox_FirstStirrupBarTapes.Visibility = Visibility.Visible;
            comboBox_FirstStirrupBarTapes.Margin = new Thickness(300, 89, 0, 0);

            comboBox_SecondStirrupBarTapes.Visibility = Visibility.Hidden;
            comboBox_SecondStirrupBarTapes.Margin = new Thickness(300, 89, 0, 0);

            comboBox_RebarCoverTypes.Visibility = Visibility.Visible;
            comboBox_RebarCoverTypes.Margin = new Thickness(70, 204, 0, 0);

            //Смещение стержней в сечении
            textBox_FirstTopRebarOffset.Visibility = Visibility.Hidden;
            textBox_FirstTopRebarOffset.Margin = new Thickness(55, 110, 0, 0);

            textBox_FirstLowerRebarOffset.Visibility = Visibility.Hidden;
            textBox_FirstLowerRebarOffset.Margin = new Thickness(55, 144, 0, 0);

            textBox_FirstLeftRebarOffset.Visibility = Visibility.Hidden;
            textBox_FirstLeftRebarOffset.Margin = new Thickness(168, 245, 0, 0);

            textBox_FirstRightRebarOffset.Visibility = Visibility.Hidden;
            textBox_FirstRightRebarOffset.Margin = new Thickness(216, 245, 0, 0);


            textBox_SecondTopRebarOffset.Visibility = Visibility.Hidden;
            textBox_SecondTopRebarOffset.Margin = new Thickness(55, 0, 0, 87);

            textBox_SecondLowerRebarOffset.Visibility = Visibility.Hidden;
            textBox_SecondLowerRebarOffset.Margin = new Thickness(55, 87, 0, 0);

            textBox_SecondLeftRebarOffset.Visibility = Visibility.Hidden;
            textBox_SecondLeftRebarOffset.Margin = new Thickness(0, 245, 150, 0);

            textBox_SecondRightRebarOffset.Visibility = Visibility.Hidden;
            textBox_SecondRightRebarOffset.Margin = new Thickness(150, 245, 0, 0);
        }

        private void button_Type3_Click(object sender, RoutedEventArgs e)
        {
            SetBorderForSelectedButton(sender);
            SetBorderForNonSelectedButtons(sender);
            image_Sections.Source = new BitmapImage(new Uri("pack://application:,,,/SquareColumnsReinforcement;component/Resources/Sections/Type3_Section.png"));
            image_3D.Source = new BitmapImage(new Uri("pack://application:,,,/SquareColumnsReinforcement;component/Resources/3D/Type3_3D.png"));

            //Типы арматуры в сечении
            comboBox_FirstMainBarTapes.Visibility = Visibility.Visible;
            comboBox_FirstMainBarTapes.Margin = new Thickness(30, 34, 0, 0);

            comboBox_SecondMainBarTapes.Visibility = Visibility.Visible;
            comboBox_SecondMainBarTapes.Margin = new Thickness(300, 209, 0, 0);

            comboBox_FirstStirrupBarTapes.Visibility = Visibility.Visible;
            comboBox_FirstStirrupBarTapes.Margin = new Thickness(300, 40, 0, 0);

            comboBox_SecondStirrupBarTapes.Visibility = Visibility.Visible;
            comboBox_SecondStirrupBarTapes.Margin = new Thickness(300, 98, 0, 0);

            comboBox_RebarCoverTypes.Visibility = Visibility.Visible;
            comboBox_RebarCoverTypes.Margin = new Thickness(70, 204, 0, 0);

            //Смещение стержней в сечении
            textBox_FirstTopRebarOffset.Visibility = Visibility.Hidden;
            textBox_FirstTopRebarOffset.Margin = new Thickness(55, 110, 0, 0);

            textBox_FirstLowerRebarOffset.Visibility = Visibility.Hidden;
            textBox_FirstLowerRebarOffset.Margin = new Thickness(55, 144, 0, 0);

            textBox_FirstLeftRebarOffset.Visibility = Visibility.Hidden;
            textBox_FirstLeftRebarOffset.Margin = new Thickness(168, 245, 0, 0);

            textBox_FirstRightRebarOffset.Visibility = Visibility.Hidden;
            textBox_FirstRightRebarOffset.Margin = new Thickness(216, 245, 0, 0);


            textBox_SecondTopRebarOffset.Visibility = Visibility.Hidden;
            textBox_SecondTopRebarOffset.Margin = new Thickness(55, 0, 0, 87);

            textBox_SecondLowerRebarOffset.Visibility = Visibility.Hidden;
            textBox_SecondLowerRebarOffset.Margin = new Thickness(55, 87, 0, 0);

            textBox_SecondLeftRebarOffset.Visibility = Visibility.Hidden;
            textBox_SecondLeftRebarOffset.Margin = new Thickness(0, 245, 150, 0);

            textBox_SecondRightRebarOffset.Visibility = Visibility.Hidden;
            textBox_SecondRightRebarOffset.Margin = new Thickness(150, 245, 0, 0);
        }

        private void button_Type4_Click(object sender, RoutedEventArgs e)
        {
            SetBorderForSelectedButton(sender);
            SetBorderForNonSelectedButtons(sender);
            image_Sections.Source = new BitmapImage(new Uri("pack://application:,,,/SquareColumnsReinforcement;component/Resources/Sections/Type4_Section.png"));
            image_3D.Source = new BitmapImage(new Uri("pack://application:,,,/SquareColumnsReinforcement;component/Resources/3D/Type4_3D.png"));

            //Типы арматуры в сечении
            comboBox_FirstMainBarTapes.Visibility = Visibility.Visible;
            comboBox_FirstMainBarTapes.Margin = new Thickness(30, 34, 0, 0);

            comboBox_SecondMainBarTapes.Visibility = Visibility.Visible;
            comboBox_SecondMainBarTapes.Margin = new Thickness(300, 209, 0, 0);

            comboBox_FirstStirrupBarTapes.Visibility = Visibility.Visible;
            comboBox_FirstStirrupBarTapes.Margin = new Thickness(300, 40, 0, 0);

            comboBox_SecondStirrupBarTapes.Visibility = Visibility.Visible;
            comboBox_SecondStirrupBarTapes.Margin = new Thickness(300, 98, 0, 0);

            comboBox_RebarCoverTypes.Visibility = Visibility.Visible;
            comboBox_RebarCoverTypes.Margin = new Thickness(70, 204, 0, 0);

            //Смещение стержней в сечении
            textBox_FirstTopRebarOffset.Visibility = Visibility.Visible;
            textBox_FirstTopRebarOffset.Margin = new Thickness(55, 0, 0, 34);

            textBox_FirstLowerRebarOffset.Visibility = Visibility.Visible;
            textBox_FirstLowerRebarOffset.Margin = new Thickness(55, 34, 0, 0);

            textBox_FirstLeftRebarOffset.Visibility = Visibility.Visible;
            textBox_FirstLeftRebarOffset.Margin = new Thickness(0, 245, 48, 0);

            textBox_FirstRightRebarOffset.Visibility = Visibility.Visible;
            textBox_FirstRightRebarOffset.Margin = new Thickness(48, 245, 0, 0);


            textBox_SecondTopRebarOffset.Visibility = Visibility.Hidden;
            textBox_SecondTopRebarOffset.Margin = new Thickness(55, 0, 0, 87);

            textBox_SecondLowerRebarOffset.Visibility = Visibility.Hidden;
            textBox_SecondLowerRebarOffset.Margin = new Thickness(55, 87, 0, 0);

            textBox_SecondLeftRebarOffset.Visibility = Visibility.Hidden;
            textBox_SecondLeftRebarOffset.Margin = new Thickness(0, 245, 150, 0);

            textBox_SecondRightRebarOffset.Visibility = Visibility.Hidden;
            textBox_SecondRightRebarOffset.Margin = new Thickness(150, 245, 0, 0);
        }

        private void button_Type5_Click(object sender, RoutedEventArgs e)
        {
            SetBorderForSelectedButton(sender);
            SetBorderForNonSelectedButtons(sender);
            image_Sections.Source = new BitmapImage(new Uri("pack://application:,,,/SquareColumnsReinforcement;component/Resources/Sections/Type5_Section.png"));
            image_3D.Source = new BitmapImage(new Uri("pack://application:,,,/SquareColumnsReinforcement;component/Resources/3D/Type5_3D.png"));

            //Типы арматуры в сечении
            comboBox_FirstMainBarTapes.Visibility = Visibility.Visible;
            comboBox_FirstMainBarTapes.Margin = new Thickness(30, 34, 0, 0);

            comboBox_SecondMainBarTapes.Visibility = Visibility.Visible;
            comboBox_SecondMainBarTapes.Margin = new Thickness(300, 209, 0, 0);

            comboBox_FirstStirrupBarTapes.Visibility = Visibility.Visible;
            comboBox_FirstStirrupBarTapes.Margin = new Thickness(300, 40, 0, 0);

            comboBox_SecondStirrupBarTapes.Visibility = Visibility.Visible;
            comboBox_SecondStirrupBarTapes.Margin = new Thickness(300, 98, 0, 0);

            comboBox_RebarCoverTypes.Visibility = Visibility.Visible;
            comboBox_RebarCoverTypes.Margin = new Thickness(70, 204, 0, 0);

            //Смещение стержней в сечении
            textBox_FirstTopRebarOffset.Visibility = Visibility.Visible;
            textBox_FirstTopRebarOffset.Margin = new Thickness(55, 0, 0, 34);

            textBox_FirstLowerRebarOffset.Visibility = Visibility.Visible;
            textBox_FirstLowerRebarOffset.Margin = new Thickness(55, 34, 0, 0);

            textBox_FirstLeftRebarOffset.Visibility = Visibility.Visible;
            textBox_FirstLeftRebarOffset.Margin = new Thickness(0, 245, 48, 0);

            textBox_FirstRightRebarOffset.Visibility = Visibility.Visible;
            textBox_FirstRightRebarOffset.Margin = new Thickness(48, 245, 0, 0);


            textBox_SecondTopRebarOffset.Visibility = Visibility.Hidden;
            textBox_SecondTopRebarOffset.Margin = new Thickness(55, 0, 0, 87);

            textBox_SecondLowerRebarOffset.Visibility = Visibility.Hidden;
            textBox_SecondLowerRebarOffset.Margin = new Thickness(55, 87, 0, 0);

            textBox_SecondLeftRebarOffset.Visibility = Visibility.Hidden;
            textBox_SecondLeftRebarOffset.Margin = new Thickness(0, 245, 150, 0);

            textBox_SecondRightRebarOffset.Visibility = Visibility.Hidden;
            textBox_SecondRightRebarOffset.Margin = new Thickness(150, 245, 0, 0);
        }

        private void button_Type6_Click(object sender, RoutedEventArgs e)
        {
            SetBorderForSelectedButton(sender);
            SetBorderForNonSelectedButtons(sender);
            image_Sections.Source = new BitmapImage(new Uri("pack://application:,,,/SquareColumnsReinforcement;component/Resources/Sections/Type6_Section.png"));
            image_3D.Source = new BitmapImage(new Uri("pack://application:,,,/SquareColumnsReinforcement;component/Resources/3D/Type6_3D.png"));

            //Типы арматуры в сечении
            comboBox_FirstMainBarTapes.Visibility = Visibility.Visible;
            comboBox_FirstMainBarTapes.Margin = new Thickness(30, 34, 0, 0);

            comboBox_SecondMainBarTapes.Visibility = Visibility.Visible;
            comboBox_SecondMainBarTapes.Margin = new Thickness(300, 209, 0, 0);

            comboBox_FirstStirrupBarTapes.Visibility = Visibility.Visible;
            comboBox_FirstStirrupBarTapes.Margin = new Thickness(300, 40, 0, 0);

            comboBox_SecondStirrupBarTapes.Visibility = Visibility.Visible;
            comboBox_SecondStirrupBarTapes.Margin = new Thickness(300, 98, 0, 0);

            comboBox_RebarCoverTypes.Visibility = Visibility.Visible;
            comboBox_RebarCoverTypes.Margin = new Thickness(70, 204, 0, 0);

            //Смещение стержней в сечении
            textBox_FirstTopRebarOffset.Visibility = Visibility.Visible;
            textBox_FirstTopRebarOffset.Margin = new Thickness(55, 0, 0, 29);

            textBox_FirstLowerRebarOffset.Visibility = Visibility.Visible;
            textBox_FirstLowerRebarOffset.Margin = new Thickness(55, 29, 0, 0);

            textBox_FirstLeftRebarOffset.Visibility = Visibility.Visible;
            textBox_FirstLeftRebarOffset.Margin = new Thickness(0, 245, 58, 0);

            textBox_FirstRightRebarOffset.Visibility = Visibility.Visible;
            textBox_FirstRightRebarOffset.Margin = new Thickness(58, 245, 0, 0);


            textBox_SecondTopRebarOffset.Visibility = Visibility.Visible;
            textBox_SecondTopRebarOffset.Margin = new Thickness(55, 0, 0, 87);

            textBox_SecondLowerRebarOffset.Visibility = Visibility.Visible;
            textBox_SecondLowerRebarOffset.Margin = new Thickness(55, 87, 0, 0);

            textBox_SecondLeftRebarOffset.Visibility = Visibility.Visible;
            textBox_SecondLeftRebarOffset.Margin = new Thickness(0, 245, 150, 0);

            textBox_SecondRightRebarOffset.Visibility = Visibility.Visible;
            textBox_SecondRightRebarOffset.Margin = new Thickness(150, 245, 0, 0);
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
    }
}
