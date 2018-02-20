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
using System.Windows.Shapes;

namespace eLED_PC_Console
{
    /// <summary>
    /// Interaction logic for PartyModeWindow.xaml
    /// </summary>
    public partial class PartyModeWindow : Window
    {

        eLED.eLED unit;
        MainWindow win;

        public PartyModeWindow(eLED.eLED eLED, MainWindow win)
        {
            InitializeComponent();
            unit = eLED;
            this.win = win;
        }

        public void Mode_Click(Object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b.Name == "Rainbow")
                SetMode(9);
            else if (b.Name == "Cycle")
                SetMode(1);
            else if (b.Name == "Pulse")
                SetMode(2);
            else if (b.Name == "Crazy")
                SetMode(3);

            Close();

        }

        public void SetMode(int mode)
        {
            if (win.zone1.IsChecked ?? false)
                unit.setMode(1, mode);
            if (win.zone2.IsChecked ?? false)
                unit.setMode(2, mode);
            if (win.zone3.IsChecked ?? false)
                unit.setMode(3, mode);
            if (win.zone4.IsChecked ?? false)
                unit.setMode(4, mode);
        }

    }
}
