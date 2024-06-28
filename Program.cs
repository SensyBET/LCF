using System;
using System.CodeDom.Compiler;
using System.Configuration;
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
            c_keithley commHandlerKeithley = new c_keithley(form, "COM8", 9600, Parity.None, 8, StopBits.One,"\r",Handshake.XOnXOff,"\r", 500, 500);
            c_sefelec commHandlerSefelec = new c_sefelec(form,"COM7",19200, Parity.None,8,StopBits.One,"\r",Handshake.XOnXOff,"\n", 500, 500);
            commHandlerKeithley.initReading();
            commHandlerSefelec.initReading();
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 10000; // Mise à jour toutes les 10 ms
            timer.Tick += (sender, e) =>
            {
                //commHandlerKeithley.ReadCurrentValue();
                commHandlerSefelec.ReadCurrentValue();
            };

            timer.Start();
            Application.Run(form);


        }
    }

    public class c_sefelec : Communication
    {
        public c_sefelec(Form1 inForm, string inPortCom, Int32 inBaudRate, Parity inParity, Int32 inDataSize, StopBits inStopBits, string inTerminator,Handshake inHandshake, string inNewLine, int inReadTimeout, int inWriteTimeout) : base(inForm, inPortCom, inBaudRate, inParity, inDataSize, inStopBits, inTerminator, inHandshake, inNewLine, inReadTimeout, inWriteTimeout)
        {

        }
        public override void initReading()
        {
            if (!mySerialPort.IsOpen)
            {
                Console.WriteLine("Serial port is not open.");
                throw new InvalidOperationException("Serial port is not open.");
            }
            SendData("REM");
            //SendData("*RST");
            //SendData("*TST");
            
        }
        public override string ReadCurrentValue() {
            string result = "";
            if (!mySerialPort.IsOpen)
            {
                Console.WriteLine("Serial port is not open.");
                throw new InvalidOperationException("Serial port is not open.");
            }
            if (!waitingForReception)
            {
                SendData("MEAS");
                SendData("MEAS?");
                waitingForReception = true;
            }
            return result;
        }

        protected override void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {

            SerialPort sp = (SerialPort)sender;
            try
            {
                string indata = sp.ReadExisting();
                if (indata.EndsWith(this.terminator))
                {
                    bufferReception += indata;
                    Console.WriteLine("[RX]: " + bufferReception);
                    bufferReception = "";
                    waitingForReception = false;
                }
                else
                {
                    Console.WriteLine("[RX Event]: " + indata);
                    bufferReception += indata;
                }
            }
            catch (TimeoutException)
            {
                Console.WriteLine("[RX TIMEOUT]");
            }
        }
    }
    public class c_keithley : Communication
    {
        public c_keithley(Form1 inForm, string inPortCom, Int32 inBaudRate, Parity inParity, Int32 inDataSize, StopBits inStopBits, string inTerminator, Handshake inHandshake, string inNewLine, int inReadTimeout, int inWriteTimeout) : base(inForm, inPortCom, inBaudRate, inParity, inDataSize, inStopBits,inTerminator, inHandshake, inNewLine, inReadTimeout, inWriteTimeout)
        {

        }

        public override void initReading()
        {
            if (!mySerialPort.IsOpen)
            {
                Console.WriteLine("Serial port is not open.");
                //throw new InvalidOperationException("Serial port is not open.");
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
        protected override void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {

            SerialPort sp = (SerialPort)sender;
            try
            {
                string indata = sp.ReadExisting();
                if (indata.EndsWith(this.terminator))  // Contains("#")
                {
                    bufferReception += indata;
                    Console.WriteLine("[RX]: " + bufferReception);
                    bufferReception = "";
                    waitingForReception = false;
                }
                else
                {
                    //Console.WriteLine("[RECEIVING]: " + indata);
                    bufferReception += indata;
                }
            }
            catch (TimeoutException)
            {
                Console.WriteLine("[RX TIMEOUT]");
            }
        }
        public override string ReadCurrentValue()
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
    public abstract class Communication
    {
        /*
        <CR>: cariage return: "\r"
        <LF>: Line Feed: "\n"

        Le paramètre Newline est-il équivalent au paramètre terminator ?
         */
        protected SerialPort mySerialPort;
        private Form1 form;
        protected bool waitingForReception = false;
        protected string bufferReception = "";
        protected string terminator;
        protected virtual void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e) { }
        public virtual void initReading() { }
        public abstract string ReadCurrentValue();

        public Communication(Form1 inFrom, string inPortCom, Int32 inBaudRate, Parity inParity, Int32 inDataSize, StopBits inStopBits, string inTerminator, Handshake inHandshake, string inNewLine, int inReadTimeout, int inWriteTimeout)
        {
            this.form = inFrom;
            this.terminator = inTerminator;
            mySerialPort = new SerialPort(inPortCom, inBaudRate, inParity, inDataSize, inStopBits)
            {
                Handshake = inHandshake,
                NewLine = inNewLine,
                ReadTimeout = inReadTimeout,
                WriteTimeout = inWriteTimeout
            };
            mySerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            try
            {
                mySerialPort.Open();
            }
            catch (System.IO.FileNotFoundException ex)
            {
                Console.WriteLine("Port incompatible/indisponible.");
                Console.WriteLine("Execption: "+ex);   
            }
            
        }
        ~Communication()
        {
            mySerialPort.Close();
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
        public void SendData(string data)
        {
            Console.Write("[TX] " + data + " [" + this.mySerialPort.PortName + "]");
            data += this.terminator;
            try
            {
                //mySerialPort est initialisé et ouvert avant d'écrire des données
                if (mySerialPort != null && mySerialPort.IsOpen)
                {
                    try
                    {
                        mySerialPort.Write(data);
                        Console.WriteLine(" [OK]");
                    }
                    catch (TimeoutException)
                    {
                        Console.WriteLine("[TX TIMEOUT]");
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
    }
}