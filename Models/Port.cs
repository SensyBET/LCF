using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace LCF_WPF.Models
{ 
    public class Port
    {
        private bool startRX = false;
        public string Name { get; set; }
        private Parity _parity;
        public Parity parity { get { return _parity; } set { _parity = value; if (serialPort != null) { try { serialPort.Parity = value; } catch (Exception ex) { Console.WriteLine("Erreur Parity, exception: " + ex); } } } }
        private Int32 _dataSize;
        public Int32 dataSize { get { return _dataSize; } set { _dataSize = value; if (serialPort != null) { try { serialPort.DataBits = value; } catch (Exception ex) { Console.WriteLine("Erreur DataBits, exception: " + ex); } } } }
        private int _terminator;
        public int terminator { get { return _terminator; } set { _terminator = value;}}

        private Int32 _baudRate;
        public Int32 baudRate { get { return _baudRate; } set { _baudRate = value; if (serialPort != null) { try { serialPort.BaudRate = value; } catch (Exception ex) { Console.WriteLine("Erreur BaudRate, exception: " + ex); } } } }
        private StopBits _stopBits;
        public StopBits stopBits { get { return _stopBits; } set { _stopBits = value; if (serialPort != null) { try { serialPort.StopBits = value; } catch (Exception ex) { Console.WriteLine("Erreur stopbits, exception: " + ex); } } } }
        private Handshake _handshake;
        public Handshake handshake { get { return _handshake; } set { _handshake = value; if (serialPort != null) { try { serialPort.Handshake = value; } catch (Exception ex) { Console.WriteLine("Erreur Handshake, exception: " + ex); } } } }
        private int _newLine;
        public int newLine { get { return _newLine; } set { _newLine = value; if (serialPort != null) { try { serialPort.NewLine = collectionTerminator[value]; } catch (Exception ex) { Console.WriteLine("Erreur NewLine, exception: " + ex); } } } }
        private int _readTimeout;
        public int readTimeout { get { return _readTimeout; } set { _readTimeout = value; if (serialPort != null) { try { serialPort.ReadTimeout = value; } catch (Exception ex) { Console.WriteLine("Erreur ReadTimeout, exception: " + ex); } } } }
        private int _writeTimeout;
        public int writeTimeout { get { return _writeTimeout; } set { _writeTimeout = value; if (serialPort != null) { try { serialPort.WriteTimeout = value; } catch (Exception ex) { Console.WriteLine("Erreur WriteTimeout, exception: " + ex); } } } }

        public SerialPort serialPort { get; set; }

        public bool waitingForReception = false;

        public string bufferReception = "";

        List<string> collectionTerminator = new List<string>
        {
            "\r",
            "\n",
            "\r\n",
            "\n\r"
        };
    public Port(string inName, Int32 inBaudRate, Parity inParity, Int32 inDataSize, StopBits inStopBits, int inTerminator, Handshake inHandshake, int inNewLine, int inReadTimeout, int inWriteTimeout)
        {
            this.Name = inName;
            this.baudRate = inBaudRate;
            this.parity = inParity;
            this.dataSize = inDataSize;
            this.stopBits = inStopBits;
            this.handshake = inHandshake;
            this.newLine = inNewLine;
            this.readTimeout = inReadTimeout;
            this.writeTimeout = inWriteTimeout;

            try
            {
                serialPort = new SerialPort(inName, inBaudRate, inParity, inDataSize, inStopBits)
                {
                    Handshake = inHandshake,
                    NewLine = collectionTerminator[inNewLine],
                    ReadTimeout = inReadTimeout,
                    WriteTimeout = inWriteTimeout
                };
                serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                //serialPort.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Création du port " + inName + " impossible.");
                Console.WriteLine("Exception: " + ex);
            }
        }
        protected void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {

            SerialPort sp = (SerialPort)sender;
            try
            {
                string indata = sp.ReadExisting();
                if (indata.EndsWith(this.collectionTerminator[terminator]))
                {
                    Console.WriteLine();
                    bufferReception += indata;
                    Console.WriteLine("[RX CONCAT]: " + bufferReception);
                    bufferReception = "";
                    startRX = false;
                    waitingForReception = false;
                }
                else
                {
                    if (!startRX)
                    {
                        startRX = true;
                        Console.Write("[RX]: ");
                        Console.Write(indata);
                    }
                    else
                    {
                        Console.Write(indata);
                    }
                    bufferReception += indata;
                }
            }
            catch (TimeoutException)
            {
                Console.WriteLine("[RX TIMEOUT]");
            }
        }

        public void SendData(string data)
        {
            
            try
            {
                if (serialPort != null)
                {
                    if (!serialPort.IsOpen)
                        try
                        {
                            Console.WriteLine("Ouverture du port");
                            serialPort.Open();
                        }
                        catch (Exception ex) { Console.WriteLine("Ouverture du port impossible, exception: " + ex); }
                    Console.Write("[TX] [" + this.serialPort.PortName + "]" + data);
                    data += collectionTerminator[this.terminator];

                    
                    //mySerialPort est initialisé et ouvert avant d'écrire des données
                    if (serialPort.IsOpen)
                    {

                        try
                        {
                            serialPort.Write(data);
                            Console.WriteLine("[TX CHECK]");
                        }
                        catch (TimeoutException)
                        {
                            Console.WriteLine("[TX TIMEOUT]");
                        }

                    }
                    else
                    {
                        Console.WriteLine("Le port n'est pas ouvert.");
                    }
                }
                else { Console.WriteLine("Aucun port sélectionné."); }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                // Rejeter l'exception pour la gestion ultérieure si nécessaire
                // throw;
            }
        }

    }
}

