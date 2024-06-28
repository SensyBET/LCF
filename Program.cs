using System;
using System.IO.Ports;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            commHandler.initReading();
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000; // Mise à jour toutes les 100 ms
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
            private bool waitingForReception = false;
            private int cptRecept = 0;
            private string bufferReception = "";
            public Communication(Form1 inFrom)
            {
                this.form = inFrom;
                mySerialPort = new SerialPort("COM7", 9600, Parity.None, 8, StopBits.One)
                {
                    Handshake = Handshake.XOnXOff,
                    NewLine = "\r",
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
                try
                {
                    string indata = sp.ReadExisting();
                    if (indata.EndsWith("\r"))  // Contains("#")
                    {
                    bufferReception += indata;
                    Console.WriteLine("[RESULT]: "+ bufferReception);
                    bufferReception = "";
                    waitingForReception = false;
                    }
                    else
                    {
                    //Console.WriteLine("[RECEIVING]: " + indata);
                    bufferReception += indata;
                    }
                }
                catch (TimeoutException) {
                    Console.WriteLine("[RECEPTION TIMEOUT]");
                }
            }

        public void SendData(string data)
        {
            Console.Write("[SENDING]   " + data + "     ");
            data += "\n";
            try
            {
                //mySerialPort est initialisé et ouvert avant d'écrire des données
                if (mySerialPort != null && mySerialPort.IsOpen)
                {
                    try { 
                        mySerialPort.Write(data);
                        Console.WriteLine("[SENT]");
                    }
                    catch(TimeoutException)
                    {
                        Console.WriteLine("[TIMEOUT]");
                    }
                    
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

        public void initReading()
        {
            if (!mySerialPort.IsOpen)
            {
                Console.WriteLine("Serial port is not open.");
                throw new InvalidOperationException("Serial port is not open.");
            }
            //SendData(":SYST:AZER:STAT OFF");
            SendData("*RST");
            //SendData(":TRAC:CLE");
            SendData(":INITiate:CONTinuous OFF");
            // Disable continuous initiation
            //SendData("INIT:CONT OFF");
            //SendData("*TST?");
            // Set sample count to 1
            SendData(":SENSe:FUNCtion 'VOLTage:AC'");
            SendData(":SENSe:VOLTage:DC:AVERage:STATe OFF");
            SendData(":SAMP:COUN 1");
        }

        public string ReadCurrentValue()
            {
            string result = "";
            if (!mySerialPort.IsOpen)
            {
                Console.WriteLine("Serial port is not open.");
                throw new InvalidOperationException("Serial port is not open.");
            }
            // Read the measurement
            if (!waitingForReception)
            {
                //SendData(":ABORT");
                //SendData(":DATA?");
                SendData(":READ?");
                waitingForReception = true;
            }
            
            return result;
            }
        }
    }