using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace tser
{
    internal class InputSimulator: IDisposable
    {
        SerialPort _serialPort;

        public bool IsOpen => _serialPort.IsOpen;

        public InputSimulator()
        {
            _serialPort = new SerialPort("COM5", 115200)
            {
                DataBits = 8,
                Parity = Parity.None,
                StopBits = StopBits.One,
                Handshake = Handshake.None,
                Encoding = System.Text.Encoding.ASCII,
                NewLine = "\n"
            };

            _serialPort.DtrEnable = true;
            _serialPort.RtsEnable = false;

            _serialPort.DataReceived += OnDataReceived;

        }

        private readonly StringBuilder _buffer = new StringBuilder();

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

                    Debug.WriteLine(line);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] {ex.Message}");
            }
        }

        internal void OpenSerialPort()
        {
            if(!_serialPort.IsOpen)
               _serialPort.Open();

            if (!_serialPort.IsOpen)
                throw new Exception("COM port doesn't opened.");
        }

        public void LeftButtonClick()
        {
            //OpenSerialPort();

            _serialPort.WriteLine("MOUSECLICK 1");
        }

        public void RightButtonClick()
        {
            //OpenSerialPort();

            _serialPort.WriteLine("MOUSECLICK 2");
        }

        public void MouseScroll(int value)
        {
            //OpenSerialPort();

            _serialPort.WriteLine("MOUSESCROLL " + value);
        }

        public void MouseMove(int dx, int dy)
        {
            //OpenSerialPort();

            _serialPort.WriteLine($"MOUSEMOVE {dx} {dy}");
        }

        public void KeyPress(int vkCode)
        {
            //OpenSerialPort();

            _serialPort.WriteLine("VKEY " + vkCode);
        }

        public void KeyPress(Keys key)
        {
            //OpenSerialPort();

            _serialPort.WriteLine("VKEY " + (int)key);
        }

        public void ShiftDown(Keys key)
        {
            //OpenSerialPort();

            _serialPort.WriteLine("SHIFTDOWN");
        }

        public void ShiftUp(Keys key)
        {
            //OpenSerialPort();

            _serialPort.WriteLine("SHIFTUP");
        }

        public void SendCommand(string command, params string[] args)
        {
            //OpenSerialPort();

            _serialPort.WriteLine(command + (args != null && args.Length > 0 ? " " + String.Join(" ", args) : ""));
        }

        public void Dispose()
        {
            if (_serialPort != null)
            {
                _serialPort.Close();
                _serialPort.Dispose();
            }
        }
        
    }
}
