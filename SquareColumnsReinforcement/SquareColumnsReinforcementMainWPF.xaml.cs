using System;
using System.Collections.Generic;
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
    /// <summary>
    /// Логика взаимодействия для SquareColumnsReinforcementMainWPF.xaml
    /// </summary>
    public partial class SquareColumnsReinforcementMainWPF : Window
    {
        public SquareColumnsReinforcementMainWPF()
        {
            InitializeComponent();
            button_SquareColumnsReinforcement_T1.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }
        private void btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
        private void SquareColumnsReinforcementMainWPF_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Space)
            {
                this.DialogResult = true;
                this.Close();
            }

            else if (e.Key == Key.Escape)
            {
                this.DialogResult = false;
                this.Close();
            }
        }

        private void button_SquareColumnsReinforcement_T1_Click(object sender, RoutedEventArgs e)
        {
            button_SquareColumnsReinforcement_T1.BorderBrush = Brushes.Black;
            button_SquareColumnsReinforcement_T1.BorderThickness = new Thickness(1, 1, 1, 5);
            button_SquareColumnsReinforcement_T2.BorderBrush = null;
            button_SquareColumnsReinforcement_T3.BorderBrush = null;
            button_SquareColumnsReinforcement_T4.BorderBrush = null;
            button_SquareColumnsReinforcement_T5.BorderBrush = null;
            button_SquareColumnsReinforcement_T6.BorderBrush = null;
            SquareColumnsReinforcementType_S.Source = new BitmapImage(new Uri("Resources/PNGSquareColumnsReinforcementT1_S.png", UriKind.Relative));
            SquareColumnsReinforcementType_3D.Source = new BitmapImage(new Uri("Resources/PNGSquareColumnsReinforcementT1_3D.png", UriKind.Relative));
        }

        private void button_SquareColumnsReinforcement_T2_Click(object sender, RoutedEventArgs e)
        {
            button_SquareColumnsReinforcement_T2.BorderBrush = Brushes.Black;
            button_SquareColumnsReinforcement_T2.BorderThickness = new Thickness(1, 1, 1, 5);
            button_SquareColumnsReinforcement_T1.BorderBrush = null;
            button_SquareColumnsReinforcement_T3.BorderBrush = null;
            button_SquareColumnsReinforcement_T4.BorderBrush = null;
            button_SquareColumnsReinforcement_T5.BorderBrush = null;
            button_SquareColumnsReinforcement_T6.BorderBrush = null;
            SquareColumnsReinforcementType_S.Source = new BitmapImage(new Uri("Resources/PNGSquareColumnsReinforcementT2_S.png", UriKind.Relative));
            SquareColumnsReinforcementType_3D.Source = new BitmapImage(new Uri("Resources/PNGSquareColumnsReinforcementT2_3D.png", UriKind.Relative));
        }

        private void button_SquareColumnsReinforcement_T3_Click(object sender, RoutedEventArgs e)
        {
            button_SquareColumnsReinforcement_T3.BorderBrush = Brushes.Black;
            button_SquareColumnsReinforcement_T3.BorderThickness = new Thickness(1, 1, 1, 5);
            button_SquareColumnsReinforcement_T1.BorderBrush = null;
            button_SquareColumnsReinforcement_T2.BorderBrush = null;
            button_SquareColumnsReinforcement_T4.BorderBrush = null;
            button_SquareColumnsReinforcement_T5.BorderBrush = null;
            button_SquareColumnsReinforcement_T6.BorderBrush = null;
            SquareColumnsReinforcementType_S.Source = new BitmapImage(new Uri("Resources/PNGSquareColumnsReinforcementT3_S.png", UriKind.Relative));
            SquareColumnsReinforcementType_3D.Source = new BitmapImage(new Uri("Resources/PNGSquareColumnsReinforcementT3_3D.png", UriKind.Relative));
        }

        private void button_SquareColumnsReinforcement_T4_Click(object sender, RoutedEventArgs e)
        {
            button_SquareColumnsReinforcement_T4.BorderBrush = Brushes.Black;
            button_SquareColumnsReinforcement_T4.BorderThickness = new Thickness(1, 1, 1, 5);
            button_SquareColumnsReinforcement_T1.BorderBrush = null;
            button_SquareColumnsReinforcement_T2.BorderBrush = null;
            button_SquareColumnsReinforcement_T3.BorderBrush = null;
            button_SquareColumnsReinforcement_T5.BorderBrush = null;
            button_SquareColumnsReinforcement_T6.BorderBrush = null;
            SquareColumnsReinforcementType_S.Source = new BitmapImage(new Uri("Resources/PNGSquareColumnsReinforcementT4_S.png", UriKind.Relative));
            SquareColumnsReinforcementType_3D.Source = new BitmapImage(new Uri("Resources/PNGSquareColumnsReinforcementT4_3D.png", UriKind.Relative));
        }

        private void button_SquareColumnsReinforcement_T5_Click(object sender, RoutedEventArgs e)
        {
            button_SquareColumnsReinforcement_T5.BorderBrush = Brushes.Black;
            button_SquareColumnsReinforcement_T5.BorderThickness = new Thickness(1, 1, 1, 5);
            button_SquareColumnsReinforcement_T1.BorderBrush = null;
            button_SquareColumnsReinforcement_T2.BorderBrush = null;
            button_SquareColumnsReinforcement_T3.BorderBrush = null;
            button_SquareColumnsReinforcement_T4.BorderBrush = null;
            button_SquareColumnsReinforcement_T6.BorderBrush = null;
            SquareColumnsReinforcementType_S.Source = new BitmapImage(new Uri("Resources/PNGSquareColumnsReinforcementT5_S.png", UriKind.Relative));
            SquareColumnsReinforcementType_3D.Source = new BitmapImage(new Uri("Resources/PNGSquareColumnsReinforcementT5_3D.png", UriKind.Relative));
        }

        private void button_SquareColumnsReinforcement_T6_Click(object sender, RoutedEventArgs e)
        {
            button_SquareColumnsReinforcement_T6.BorderBrush = Brushes.Black;
            button_SquareColumnsReinforcement_T6.BorderThickness = new Thickness(1, 1, 1, 5);
            button_SquareColumnsReinforcement_T1.BorderBrush = null;
            button_SquareColumnsReinforcement_T2.BorderBrush = null;
            button_SquareColumnsReinforcement_T3.BorderBrush = null;
            button_SquareColumnsReinforcement_T4.BorderBrush = null;
            button_SquareColumnsReinforcement_T5.BorderBrush = null;
            SquareColumnsReinforcementType_S.Source = new BitmapImage(new Uri("Resources/PNGSquareColumnsReinforcementT6_S.png", UriKind.Relative));
            SquareColumnsReinforcementType_3D.Source = new BitmapImage(new Uri("Resources/PNGSquareColumnsReinforcementT6_3D.png", UriKind.Relative));
        }
    }
}
