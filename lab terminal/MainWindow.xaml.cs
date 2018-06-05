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

using System.Diagnostics;
using System.Threading;

using System.Timers;
using System.IO.Ports;

namespace lab_terminal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Terminal terminal;
        Terminal multimeter1;
        Terminal multimeter2;
        Terminal powersupply;

        Logging log;




        string mult1="0", mult2="0", powsup1="0", powsup2="0";



        System.Timers.Timer timer = new System.Timers.Timer(579);

        System.Timers.Timer log_timer;

        public MainWindow()
        {
            InitializeComponent();

            button.IsEnabled = false;

            textBlock1.Text = "First line\n";

  
            timer.Elapsed += Timer_Elapsed;

            slider.Value = 7;

        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {

        }

        static string pre_out;

        public void ComPort_DataReceived1(object sender, SerialDataReceivedEventArgs e)
        {
            string input = (sender as SerialPort).ReadExisting();
            pre_out += input;
            if (pre_out.EndsWith("\n"))
            {
                this.Dispatcher.Invoke(() =>
                {
                    textBlock1.Text = pre_out += textBlock1.Text;// textBlock1.Text.Substring(0, textBlock1.Text.IndexOf('\n'));

                    pre_out = String.Empty;
                });
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            powersupply.Send(textBox_Input.Text);
            if (textBox_Input.Text.Contains("V1"))
            {
                powsup1 = textBox_Input.Text;
            }
            else if (textBox_Input.Text.Contains("V2"))
            {
                powsup2 = textBox_Input.Text;
            }
        }


        private void button1_Click(object sender, RoutedEventArgs e)
        {

            if (terminal != null && terminal.ComPort.IsOpen)
            {
                terminal.ComPort.Close();
                button.IsEnabled = false;
            }
            else
            {
                terminal = new Terminal(listView.SelectedItem.ToString(), ComPort_DataReceived1);

                terminal.ComPort.DataReceived += ComPort_DataReceived1;

                if (terminal.ComPort.IsOpen)
                {
                    button.IsEnabled = true;
                }
                else
                {
                    MessageBox.Show("COM not enabled");
                    button.IsEnabled = false;
                }
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
                listView.Items.Clear();
            string[] items = SerialPort.GetPortNames();
            foreach (string item in items)
            {
                listView.Items.Add(item);
                Console.WriteLine(item);
            }
        }

        private void button_multimeter1_Click(object sender, RoutedEventArgs e)
        {
            if (multimeter1 == null)
            {
                if (listView.SelectedItem != null)
                {
                    multimeter1 = new Terminal(listView.SelectedItem.ToString(), DataReceived_mult1);
                    button_multimeter1.IsEnabled = false;
                }
            }
        }
        string input_mult1 = "";
        private void DataReceived_mult1(object sender, SerialDataReceivedEventArgs e)
        {
            string input_mult1 = (sender as SerialPort).ReadExisting();
            pre_out += input_mult1;
            if (pre_out.EndsWith("\n"))
            {
                this.Dispatcher.Invoke(() =>
                {
                    mult1 = pre_out.TrimEnd('\n');
                    textBlock1.Text = pre_out += textBlock1.Text;// textBlock1.Text.Substring(0, textBlock1.Text.IndexOf('\n'));
                    pre_out = "";
                });
            }

        }

        private void button_multimeter2_Click(object sender, RoutedEventArgs e)
        {
            if (multimeter2 == null)
            {
                if (listView.SelectedItem != null)
                {
                    multimeter2 = new Terminal(listView.SelectedItem.ToString(), DataReceived_mult2);
                    button_multimeter2.IsEnabled = false;
                }
            }
        }

        private void DataReceived_mult2(object sender, SerialDataReceivedEventArgs e)
        {
            string input_mult2 = (sender as SerialPort).ReadExisting();
            pre_out += input_mult2;
            if (pre_out.EndsWith("\n"))
            {
                this.Dispatcher.Invoke(() =>
                {
                    mult2 = pre_out.TrimEnd('\n');
                    textBlock1.Text = pre_out += textBlock1.Text;// textBlock1.Text.Substring(0, textBlock1.Text.IndexOf('\n'));
                    pre_out = "";
                });
            }
        }

        private void button_powersupply_Click(object sender, RoutedEventArgs e)
        {
            if (powersupply == null)
            {
                if (listView.SelectedItem != null)
                {
                    powersupply = new Terminal(listView.SelectedItem.ToString(), ComPort_DataReceived1);
                    button_powersupply.IsEnabled = false;
                    button.IsEnabled = true;
                    timer.Start();
                }
            }
        }

        private void button_log_stop_Click(object sender, RoutedEventArgs e)
        {
            log_timer.Stop();
            log.Stop();
        }

        private void button_reset_Click(object sender, RoutedEventArgs e)
        {
            button_multimeter1.IsEnabled = true;
            button_multimeter2.IsEnabled = true;
            button_powersupply.IsEnabled = true;
 
            if(multimeter1 != null) multimeter1.Dispose();
            if (multimeter2 != null) multimeter2.Dispose();
            if (powersupply != null) powersupply.Dispose();

            multimeter1 = null;
            multimeter1 = null;
            powersupply = null;

            button.IsEnabled = false;

            timer.Stop();
        }

   

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            powsup1 = slider.Value.ToString().Replace(',', '.');
            if (powersupply != null)powersupply.Send("V1 " + slider.Value.ToString().Replace(',','.'));
            //Int32.Parse(sender.ToString());
        }

        private void button1_Click_1(object sender, RoutedEventArgs e)
        {
            log = new Logging();
            /*
            log_timer = new Timer(Int32.Parse(textBox_log.Text.ToString()));

            log_timer.Elapsed += Log_timer_Elapsed;

            log_timer.Start();
            */
            var th = new System.Threading.Thread(makeLog);

            th.Start();

        }

        private void makeLog()
        {

            this.Dispatcher.Invoke(() =>
            {
                progressBar.Minimum = 0;
                progressBar.Maximum = 100;
            });


            double voltage = 0;
            for (int i = 0; i < 100; i++)
            {
                powersupply.Send("V1 " + voltage.ToString().Replace(',', '.'));
                powsup1 = voltage.ToString().Replace(',', '.');
                System.Threading.Thread.Sleep(3000);
                Log_timer_Elapsed(null, null);
                voltage += 2.0 / 100;

                this.Dispatcher.Invoke(() =>
                {
                    progressBar.Value = i;
                });


            }
            MessageBox.Show("Log done");
            log.Stop();
        }

        private void Log_timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            string line = "";

            line += DateTime.Now;
            line += " , ";
            line += mult1 + " , " + mult2 + " , " + powsup1 + " , " + powsup2;

            Console.WriteLine(line);
            log.AddLine(line);
        }
    }
}