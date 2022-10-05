using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SquareColumnsReinforcement
{
    public partial class SquareColumnsReinforcementWPF : Window
    {
        public SquareColumnsReinforcementWPF(List<RebarShape> rebarShapeList, List<RebarHookType> rebarHookTypeList)
        {
            InitializeComponent();
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
        }

        private void button_Type2_Click(object sender, RoutedEventArgs e)
        {
            SetBorderForSelectedButton(sender);
            SetBorderForNonSelectedButtons(sender);
            image_Sections.Source = new BitmapImage(new Uri("pack://application:,,,/SquareColumnsReinforcement;component/Resources/Sections/Type2_Section.png"));
            image_3D.Source = new BitmapImage(new Uri("pack://application:,,,/SquareColumnsReinforcement;component/Resources/3D/Type2_3D.png"));
        }

        private void button_Type3_Click(object sender, RoutedEventArgs e)
        {
            SetBorderForSelectedButton(sender);
            SetBorderForNonSelectedButtons(sender);
            image_Sections.Source = new BitmapImage(new Uri("pack://application:,,,/SquareColumnsReinforcement;component/Resources/Sections/Type3_Section.png"));
            image_3D.Source = new BitmapImage(new Uri("pack://application:,,,/SquareColumnsReinforcement;component/Resources/3D/Type3_3D.png"));
        }

        private void button_Type4_Click(object sender, RoutedEventArgs e)
        {
            SetBorderForSelectedButton(sender);
            SetBorderForNonSelectedButtons(sender);
            image_Sections.Source = new BitmapImage(new Uri("pack://application:,,,/SquareColumnsReinforcement;component/Resources/Sections/Type4_Section.png"));
            image_3D.Source = new BitmapImage(new Uri("pack://application:,,,/SquareColumnsReinforcement;component/Resources/3D/Type4_3D.png"));
        }

        private void button_Type5_Click(object sender, RoutedEventArgs e)
        {
            SetBorderForSelectedButton(sender);
            SetBorderForNonSelectedButtons(sender);
            image_Sections.Source = new BitmapImage(new Uri("pack://application:,,,/SquareColumnsReinforcement;component/Resources/Sections/Type5_Section.png"));
            image_3D.Source = new BitmapImage(new Uri("pack://application:,,,/SquareColumnsReinforcement;component/Resources/3D/Type5_3D.png"));
        }

        private void button_Type6_Click(object sender, RoutedEventArgs e)
        {
            SetBorderForSelectedButton(sender);
            SetBorderForNonSelectedButtons(sender);
            image_Sections.Source = new BitmapImage(new Uri("pack://application:,,,/SquareColumnsReinforcement;component/Resources/Sections/Type6_Section.png"));
            image_3D.Source = new BitmapImage(new Uri("pack://application:,,,/SquareColumnsReinforcement;component/Resources/3D/Type6_3D.png"));
        }

        private static void SetBorderForSelectedButton(object sender)
        {
            BrushConverter bc = new BrushConverter();
            //(sender as Button).BorderBrush = bc.ConvertFrom("#047db2") as Brush;
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
                //btn.BorderBrush = bc.ConvertFrom("#FF707070") as Brush;
                btn.BorderThickness = new Thickness(1, 1, 1, 1);
            }
        }
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            // for .NET Core you need to add UseShellExecute = true
            // see https://docs.microsoft.com/dotnet/api/system.diagnostics.processstartinfo.useshellexecute#property-value
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
