using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace LCF
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Form1 form = new Form1();
            Communication commHandler = new Communication(form);

            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 100; // Mise à jour toutes les 100 ms
            timer.Tick += (sender, e) => {
                commHandler.ReadCurrentValue();
            };
            timer.Start();
            Application.Run(form);
        }
    }
    public class Communication
        {
            private SerialPort mySerialPort;
            private Form1 form;

            public Communication(Form1 inFrom)
            {
                this.form = inFrom;
                mySerialPort = new SerialPort("COM7", 9600, Parity.None, 8, StopBits.One)
                {
                    Handshake = Handshake.XOnXOff,
                    NewLine = "\n",
                    ReadTimeout = 500,
                    WriteTimeout = 500
                };
                mySerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                mySerialPort.Open();
            }

            public void Open()
            {
                if (!mySerialPort.IsOpen)
                {
                    mySerialPort.Open();
                }
            }

            public void Close()
            {
                if (mySerialPort.IsOpen)
                {
                    mySerialPort.Close();
                }
            }

            ~Communication()
            {
                mySerialPort.Close();
            }

            private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
            {
                SerialPort sp = (SerialPort)sender;
                string indata = sp.ReadExisting();
                Console.WriteLine("Data Received: " + indata);
            }

        public void SendData(string data)
        {
            Console.Write("[SENDING]   " + data + "     ");

            try
            {
                // Assurez-vous que mySerialPort est initialisé et ouvert avant d'écrire des données
                if (mySerialPort != null && mySerialPort.IsOpen)
                {
                    mySerialPort.Write(data);
                    Console.WriteLine("[SENT]");
                }
                else
                {
                    Console.WriteLine("Serial port is not open");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                // Rejeter l'exception pour la gestion ultérieure si nécessaire
                throw;
            }
        }

        public string ReadCurrentValue()
            {
            if (!mySerialPort.IsOpen)
            {
                Console.WriteLine("Serial port is not open.");
                throw new InvalidOperationException("Serial port is not open.");
            }

               
            SendData("INIT:CONT OFF");
            // Disable continuous initiation
            //SendData("INIT:CONT OFF");
            //Thread.Sleep(100);
            //SendData("*TST?");
            //Thread.Sleep(100);
            // Set sample count to 1
            SendData("SAMP:COUN 1");
            //Thread.Sleep(100);

            // Set measurement function to DC Current
            SendData("FUNC 'CURR:DC'");
            //Thread.Sleep(100);
            // Read the measurement
            SendData("READ?");
            Thread.Sleep(1000);
            //Console.WriteLine(mySerialPort.ReadLine());
            //Thread.Sleep(1000);
            string result = "";
            return result;
            }
        }
    }