using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace LCF_WPF.Models
{
    public class IO_Ports
    {
        string[] portNames = SerialPort.GetPortNames();
        public string[] scanPorts()
        {
            portNames = SerialPort.GetPortNames();

            if (portNames.Length == 0)
            {
                Console.WriteLine("Aucun port n'est détecté");
            };
            foreach (string port in portNames)
            {
                try
                {
                    using (SerialPort serialPort = new SerialPort(port))
                    {
                        serialPort.Open();
                        Console.WriteLine($"{port} est disponible.");
                        serialPort.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{port} est indisponible: {ex.Message}");
                }
            }
            return portNames;
        }
    }
}

