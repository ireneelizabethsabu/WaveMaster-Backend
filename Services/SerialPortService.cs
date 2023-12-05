﻿using Microsoft.AspNetCore.SignalR;
using Serilog;
using System;
using System.IO.Ports;
using System.Text;
using WaveMaster_Backend.HubConfig;
using WaveMaster_Backend.Models;
using WaveMaster_Backend.ViewModels;

namespace WaveMaster_Backend.Services
{
    /// <summary>
    /// Interface defining method to send data to serial port
    /// </summary>
    public interface ISerialPortService
    {
        public void SendData(string command);
        public void Connect(ConenctionParamsModel value);
        public void Disconnect();
        public void DataReceivedHandler();
        
    }
    public class SerialPortService : ISerialPortService
    {      
        private readonly IHubContext<PlotDataHub> _hub;
        private static SerialPort serialPort { get; set; }

        private readonly IDataService _dataService;
        /// <summary>
        /// Initializes an instance of Serial Port service by injecting required dependencies. 
        /// </summary>
        /// <param name="hub"> The hub service instance</param>
        public SerialPortService(
            IHubContext<PlotDataHub> hub, IDataService dataService)
        {
            _hub = hub;
            _dataService = dataService;
        }

        /// <summary>
        /// Open serial port connection
        /// </summary>
        /// <param name="value">ConnectionParamsModel instance</param>
        public void Connect(ConenctionParamsModel value)
        {
            try
            {
                serialPort = new SerialPort
                {
                    PortName = value.portName,
                    BaudRate = value.baudRate,
                    Parity = (Parity)Enum.Parse(typeof(Parity), value.parity, true),
                    DataBits = value.dataBit,
                    StopBits = (StopBits)value.stopBit,
                    Handshake = (Handshake)Enum.Parse(typeof(Handshake), "None", true),
                    ReadTimeout = 500,
                    WriteTimeout = 500
                };
                serialPort.Open();
                SendData(Commands.RESET);
            }
            catch (Exception) { }

        }

        /// <summary>
        /// Disconnect serial port connection
        /// </summary>
        public void Disconnect()
        {
            try
            {
                SendData(Commands.CONNECTION_STOP);
                serialPort.Close();
            }
            catch (Exception) { }

        }

        /// <summary>
        /// Sends commands to the serial port. 
        /// </summary>
        /// <param name="command"> The command string to be sent</param>
        public void SendData(string command)
        {
            try
            {
                //SocketCommunication.SendData(command);
                Console.WriteLine("sent command : " + command);

                serialPort.Write(command);
                serialPort.DiscardOutBuffer();
            }
            catch (Exception)
            {
                _hub.Clients.All.SendAsync("captureControl", "DEVICE DISCONNECTED");
            }
        }
        
        /// <summary>
        /// Handles the read operation from the serial port.
        /// </summary>
        public void DataReceivedHandler()
        {
            
            while (serialPort.IsOpen)
            {
                try
                {
                    var buffer = new byte[1024];
                    if (serialPort.BytesToRead == 0)
                        continue;
                    int bytesRead = serialPort.BaseStream.Read(buffer, 0, 1024);

                    string asciiString = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Console.WriteLine(asciiString);
                    _dataService.handleData(asciiString,buffer,bytesRead);
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss:fff")} RxCommandsAsync: Exception {ex}");
                }
            }
        }
    }
}
