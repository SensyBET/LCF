using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Input;
using System.Windows;
using LCF_WPF.Commands;
using LCF_WPF.Models;
using System.Windows.Controls;
using System.Windows.Media;
using System.Printing;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Runtime.ConstrainedExecution;

namespace LCF_WPF.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Port> _portList = new();
        public ObservableCollection<Port> Port_list
        {
            get { return _portList; }
            set
            {
                if (_portList != value)
                {
                    _portList = value;
                    OnPropertyChanged(nameof(Port_list));
                }
            }
        }

        private Port _selectedPort = new Port("",9600,Parity.None,8,StopBits.One,0,Handshake.None, 0,500,500);
        public Port SelectedPort
        {
            get { return _selectedPort; }
            set
            {
                if (_selectedPort != value)
                {
                    _selectedPort = value;
                    OnPropertyChanged(nameof(SelectedPort));
                }
            }
        }

        public IO_Ports instIOPort;
        string[] portListArray = Array.Empty<string>();
        private StringBuilder consoleOutputBuilder;
        private string _consoleOutput;
        public ICommand ScanButtonCommand { get; }
        public ICommand SendButtonCommand { get; }

        public ICommand InitKeithleyButtonCommand { get; }

        private string _messageToSend;
        public ObservableCollection<int> collectionBaudrates { get; set; }
        public List<Parity> collectionParity { get; set; }

        public List<int> collectionDatasize { get; set; }

        public List<StopBits> collectionStopbits { get; set; }

        public List<Handshake> collectionHandshake { get; set; }
        
        private ObservableCollection<string> _commandHistory = new();
        public ObservableCollection<string> CommandHistory
        {
            get { return _commandHistory; }
            set
            {
                if (_commandHistory != value)
                {
                    _commandHistory = value;
                    OnPropertyChanged(nameof(CommandHistory));
                }
            }
        }

        private string _selectedCommandHistory;
        public string SelectedCommandHistory {  get { return _selectedCommandHistory; }
            set
            {
                if (_selectedCommandHistory != value)
                {
                    _selectedCommandHistory = value;
                    MessageToSend = _selectedCommandHistory;
                    OnPropertyChanged(nameof(SelectedCommandHistory));
                }
            }
        }

        public MainViewModel()
        {
            instIOPort = new IO_Ports();
            collectionBaudrates = new ObservableCollection<int> { 9600, 19200, 38400, 57600, 115200 };
            collectionParity = new List<Parity>
            {
                Parity.None,
                Parity.Odd,
                Parity.Even,
                Parity.Mark,
                Parity.Space
            };
            collectionDatasize = new List<int> { 5, 6, 7, 8 };
            collectionStopbits = new List<StopBits> 
            {
                    StopBits.One,
                    StopBits.OnePointFive,
                    StopBits.Two
                };

            collectionHandshake = new List<Handshake>
            {
                Handshake.None,
                Handshake.RequestToSend,
                Handshake.RequestToSendXOnXOff,
                Handshake.XOnXOff
            };

            
            consoleOutputBuilder = new StringBuilder();
            ConsoleOutput = string.Empty;
            Console.SetOut(new ConsoleOutputWriter(this));

            // Initialiser la commande du bouton
            ScanButtonCommand = new RelayCommand(ExecuteScanButtonCommand);
            SendButtonCommand = new RelayCommand(ExecuteSendButtonCommand);
            InitKeithleyButtonCommand = new RelayCommand(ExecuteInitKeithleyButtonCommand);

            scanForPorts();
        }

        private void scanForPorts()
        {
            portListArray = instIOPort.scanPorts();
            Port_list.Clear();

            foreach (var port in portListArray)
            {
                Port_list.Add(new Port(port,9600, Parity.None, 8, StopBits.One, 0, Handshake.None, 0, 500, 500));
            }
        }

        public string MessageToSend
        {
            get { return _messageToSend; }
            set
            {
                if (_messageToSend != value)
                {
                    _messageToSend = value;
                    OnPropertyChanged(nameof(MessageToSend));
                }
            }
        }

        public string ConsoleOutput
        {
            get { return _consoleOutput; }
            private set
            {
                if (_consoleOutput != value)
                {
                    _consoleOutput = value;
                    OnPropertyChanged(nameof(ConsoleOutput));

                }
            }
        }

        // Commande pour les boutons
        private void ExecuteInitKeithleyButtonCommand(object parameter)
        {
            // Logique à exécuter lors du clic sur le bouton
            //ConsoleOutput += "Bouton cliqué !\n";
            Console.WriteLine("Envoi des commandes pour mesure avec multiplexeur 7700 sur keithley 2700");
            //_selectedPort.SendData("DISPlay: TEXT: DATA 'Init.'");
            //_selectedPort.SendData("DISP: TEXT: STAT ON");

            _selectedPort.SendData("TRAC:CLE"); //Efface le buffer
            _selectedPort.SendData("*OPC"); //Attente fin de traitement de la commande précédente (vérifier nécessité)
            _selectedPort.SendData("SYST:PRES");
            _selectedPort.SendData("*OPC");
            _selectedPort.SendData("*RST");
            _selectedPort.SendData("*OPC");
            //_selectedPort.SendData(":INIT:CONT OFF");
            //_selectedPort.SendData(":SENS:FUNC 'VOLT:AC'");
            //_selectedPort.SendData(":SENS:VOLT:DC:AVER:STAT OFF");
            //_selectedPort.SendData(":SAMP:COUN 1");

            //_selectedPort.SendData("ROUT:MULT:OPEN: ALL");
            _selectedPort.SendData("ROUT:OPEN:ALL");
            _selectedPort.SendData("*OPC");
            _selectedPort.SendData("ROUT:MULT:CLOSE (@101,102,103,104,105,106,121,122)");
            _selectedPort.SendData("*OPC");
            _selectedPort.SendData("ROUT:SCAN (@101,102,103,104,105,106,121,122)");
            _selectedPort.SendData("*OPC");
            _selectedPort.SendData("SAMP:COUN 8");
            _selectedPort.SendData("*OPC");
            _selectedPort.SendData("ROUT:SCAN:LSEL INT");
            _selectedPort.SendData("*OPC");
            _selectedPort.SendData("INIT");
            _selectedPort.SendData("*OPC");
            _selectedPort.SendData("FETCH?");
            _selectedPort.SendData("*OPC");

            //Fermeture de la mesure
            _selectedPort.SendData("ROUT:SCAN:LSEL NONE");
            _selectedPort.SendData("*OPC");
            _selectedPort.SendData("ROUT:OPEN:ALL");
            _selectedPort.SendData("*OPC");

            Console.WriteLine();
        }
        private void ExecuteScanButtonCommand(object parameter)
        {
            // Logique à exécuter lors du clic sur le bouton
            //ConsoleOutput += "Bouton cliqué !\n";
            Console.WriteLine("Scan des ports COM");
            foreach (Port port in _portList)
            {
                if (port.serialPort.IsOpen)
                {
                    port.serialPort.Close();
                }
            }
            scanForPorts();
            Console.WriteLine();
        }

        private void ExecuteSendButtonCommand(object parameter)
        {
            // Logique à exécuter lors du clic sur le bouton
            //ConsoleOutput += "Bouton cliqué !\n";
            if (_selectedPort != null) {
                _selectedPort.SendData(_messageToSend);
                CommandHistory.Add(new string(_messageToSend));
                //Console.WriteLine("[" + _selectedPort.Name + "] "+ _messageToSend);
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("Auncun port disponible/sélectionné");
                
            }

        }
        

        private class ConsoleOutputWriter : TextWriter
        {
            private MainViewModel viewModel;

            public ConsoleOutputWriter(MainViewModel viewModel)
            {
                this.viewModel = viewModel;
            }

            public override void Write(char value)
            {
                viewModel.consoleOutputBuilder.Append(value);
                viewModel.UpdateOutput();
            }

            public override void Write(string value)
            {
                viewModel.consoleOutputBuilder.Append(value);
                viewModel.UpdateOutput();
            }

            public override Encoding Encoding => Encoding.UTF8;
        }

        private void UpdateOutput()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ConsoleOutput = consoleOutputBuilder.ToString();
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
