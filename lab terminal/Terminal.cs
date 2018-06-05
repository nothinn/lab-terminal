using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Ports;

namespace lab_terminal
{
    class Terminal : IDisposable
    {
        public SerialPort ComPort { get; }


        public int Port { get; set; }

        public int Baud { get; set; }

        public Terminal(string port, SerialDataReceivedEventHandler handler)
        {
            try
            {
                ComPort = new SerialPort(port, 9600);

                ComPort.Open();

                ComPort.DataReceived += handler;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        ~Terminal()
        {
            Console.WriteLine(ComPort.PortName + " is closing auto");
            ComPort.Close();
            ComPort.Dispose();
        }

     
        public void Send(string something)
        {
            ComPort.Write(something+"\n");
            Console.WriteLine(ComPort.PortName + " send: " + something);
        }

        public string read()
        {
            return ComPort.ReadExisting();
        }

        public void Dispose()
        {
            Console.WriteLine(ComPort.PortName + " closing");
            ComPort.Close();
            ComPort.Dispose();
        }
    }
}
