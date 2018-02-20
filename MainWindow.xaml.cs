using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using eLED;

namespace eLED_PC_Console
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        eLED.eLED unit;
        static string ip = "IP_ADDRESS";

        bool speed_needs_slow = true;
        bool videoMatching = false;
        bool isStrobing = false;
        bool isBlinking = false;

        bool zone1_enabled = true; //work with zone 1 by default
        bool zone2_enabled = false;
        bool zone3_enabled = false;
        bool zone4_enabled = false;

        int frequency = 1;

        public MainWindow()
        {
            InitializeComponent();
            unit = new eLED.eLED(ip);
            Thread.Sleep(100);
            unit.setBrightness(0, 100);
            Console.WriteLine("Initialized.");
            IP.Content = ip;
        }

        public void Slider_Changed(Object sender, DragDeltaEventArgs e)
        {
            Slider t = sender as Slider;

            if (t == BrightnessSlider)
                BrightnessValue.Content = BrightnessSlider.Value.ToString();
            else if (t == FrequencySlider)
                FrequencyValue.Content = FrequencySlider.Value.ToString();
        }

        public void Brightness_Slider_Stopped(Object sender, DragCompletedEventArgs e)
        {
            SetBrightness((int)BrightnessSlider.Value);
        }

        public void Frequency_Slider_Stopped(Object sender, DragCompletedEventArgs e)
        {
            frequency = (int)FrequencySlider.Value;
        }
        
        public void Zone_Click(Object sender, RoutedEventArgs e)
        {
            ToggleButton b = sender as ToggleButton;

            if (b.IsChecked ?? false) //Turn on
            {
                if (b.Name == "zone1")
                    zone1_enabled = true;
                else if (b.Name == "zone2")
                    zone2_enabled = true;
                else if (b.Name == "zone3")
                    zone3_enabled = true;
                else if (b.Name == "zone4")
                    zone4_enabled = true;
            }
            else
            {
                if (b.Name == "zone1")
                    zone1_enabled = false;
                else if (b.Name == "zone2")
                    zone2_enabled = false;
                else if (b.Name == "zone3")
                    zone3_enabled = false;
                else if (b.Name == "zone4")
                    zone4_enabled = false;
            }

        }

        public void OnButton_Click(Object sender, RoutedEventArgs e)
        {
            if (zone1.IsChecked ?? false) //Nullable if check
                unit.turnOn(1);
            if (zone2.IsChecked ?? false)
                unit.turnOn(2);
            if (zone3.IsChecked ?? false)
                unit.turnOn(3);
            if (zone4.IsChecked ?? false)
                unit.turnOn(4);
        }

        public void OffButton_Click(Object sender, RoutedEventArgs e)
        {
            if (zone1.IsChecked ?? false)
                unit.turnOff(1);
            if (zone2.IsChecked ?? false)
                unit.turnOff(2);
            if (zone3.IsChecked ?? false)
                unit.turnOff(3);
            if (zone4.IsChecked ?? false)
                unit.turnOff(4);
            videoMatching = false;
        }

        public void SpeedDown()
        {
            if (speed_needs_slow)
            {
                for (int i = 0; i < 6; i++)
                    unit.speedDown(0);
                speed_needs_slow = false;
            }
        }

        public void PartyMode_Enable(Object sender, RoutedEventArgs e)
        {

            PartyModeWindow mode = new PartyModeWindow(unit, this);
            mode.Show();

            videoMatching = false;
        }
        

        public void WhiteMode_Click(Object sender, RoutedEventArgs e)
        {
            if (zone1.IsChecked ?? false)
                unit.whiteMode(1);
            if (zone2.IsChecked ?? false)
                unit.whiteMode(2);
            if (zone3.IsChecked ?? false)
                unit.whiteMode(3);
            if (zone4.IsChecked ?? false)
                unit.whiteMode(4);
            videoMatching = false;
        }

        public void SetBrightness(double f)
        {
            if (zone1.IsChecked ?? false)
                unit.setBrightness(1, f);
            if (zone2.IsChecked ?? false)
                unit.setBrightness(2, f);
            if (zone3.IsChecked ?? false)
                unit.setBrightness(3, f);
            if (zone4.IsChecked ?? false)
                unit.setBrightness(4, f);
        }

        public void FollowScreen_Click(Object sender, RoutedEventArgs e)
        {
            if (! FollowButton.IsChecked ?? false) //Unchecked
                videoMatching = false;
            else
            {
                Console.WriteLine("Starting video matcher.");

                videoMatching = true;

                new Thread(() =>
                {
                    VideoMatcher(unit, 0, 40);
                }).Start();
            }

        }

        public void VideoMatcher(eLED.eLED eLED, int zone, int frequency)
        {
            var matcher = new ScreenMatcher();

            new Thread(() =>
            {
                matcher.startWatching();
            }).Start();


            while (videoMatching)
            {
                Thread.Sleep(1000 / frequency);

                if (matcher.color == -1)
                    eLED.whiteMode(zone);
                else
                    eLED.setColor(zone, matcher.color);

            }
        }

        public void Strobe_Click(Object sender, RoutedEventArgs e)
        {
            if (StrobeButton.IsChecked ?? false) //If checked
            {
                if (BlinkButton.IsChecked ?? false)
                {
                    BlinkButton.IsChecked = false; //Uncheck blink
                    isBlinking = false;
                }

                isStrobing = true;
                new Thread(Strobe).Start();
            }
            else
            {
                isStrobing = false;
            }

        }

        public void Blink_Click(Object sender, RoutedEventArgs e)
        {

            if (BlinkButton.IsChecked ?? false) //If checked
            {
                if (StrobeButton.IsChecked ?? false)
                {
                    StrobeButton.IsChecked = false;  //Uncheck strobe
                    isStrobing = false;
                }

                isBlinking = true;
                new Thread(Blink).Start();
            }
            else
            {
                isBlinking = false;
            }
        }

        void Strobe()
        {
            while (isStrobing)
            {

                if (zone1_enabled)
                    unit.turnOff(1);
                if (zone2_enabled)
                    unit.turnOff(2);
                if (zone3_enabled)
                    unit.turnOff(3);
                if (zone4_enabled)
                    unit.turnOff(4);

                Thread.Sleep((1000 - 10)/frequency);

                if (zone1_enabled)
                    unit.turnOn(1);
                if (zone2_enabled)
                    unit.turnOn(2);
                if (zone3_enabled)
                    unit.turnOn(3);
                if (zone4_enabled)
                    unit.turnOn(4);

                Thread.Sleep(10);
            }
        }

        void Blink()
        {
            while (isBlinking)
            {
                if (zone1_enabled)
                    unit.turnOff(1);
                if (zone2_enabled)
                    unit.turnOff(2);
                if (zone3_enabled)
                    unit.turnOff(3);
                if (zone4_enabled)
                    unit.turnOff(4);

                Thread.Sleep(1000 / frequency);

                if (zone1_enabled)
                    unit.turnOn(1);
                if (zone2_enabled)
                    unit.turnOn(2);
                if (zone3_enabled)
                    unit.turnOn(3);
                if (zone4_enabled)
                    unit.turnOn(4);

                Thread.Sleep(1000 / frequency);
            }
        }
    }
}
        