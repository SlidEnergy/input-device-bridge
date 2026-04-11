using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tser
{
    public class SerialDebugReader : IDisposable
    {
        private readonly SerialPort _serialPort;
        private readonly StringBuilder _buffer = new StringBuilder();

        public event Action<string> LineReceived;

        public bool IsOpen => _serialPort?.IsOpen ?? false;

        public SerialDebugReader(string portName, int baudRate = 115200)
        {
            _serialPort = new SerialPort(portName, baudRate)
            {
                Encoding = Encoding.UTF8,
                NewLine = "\n",
                ReadTimeout = 500
            };

            _serialPort.DataReceived += OnDataReceived;
        }

        public void Start()
        {
            if (!_serialPort.IsOpen)
                _serialPort.Open();
        }

        public void Stop()
        {
            if (_serialPort.IsOpen)
                _serialPort.Close();
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = _serialPort.ReadExisting();
                _buffer.Append(data);

                while (true)
                {
                    var bufferStr = _buffer.ToString();
                    int newLineIndex = bufferStr.IndexOf('\n');

                    if (newLineIndex < 0)
                        break;

                    string line = bufferStr.Substring(0, newLineIndex).Trim('\r');
                    _buffer.Remove(0, newLineIndex + 1);

                    LineReceived?.Invoke(line);
                }
            }
            catch (Exception ex)
            {
                LineReceived?.Invoke($"[ERROR] {ex.Message}");
            }
        }

        public void Dispose()
        {
            try
            {
                Stop();
                _serialPort.DataReceived -= OnDataReceived;
                _serialPort.Dispose();
            }
            catch { }
        }
    }
}
