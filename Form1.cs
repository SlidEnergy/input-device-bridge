using OpenCvSharp.ML;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace tser
{
    public partial class Form1 : Form
    {
        private const int XBUTTON1 = 0x0001;
        private const int XBUTTON2 = 0x0002;
        private const int LLKHF_INJECTED = 0x10;

        private InputSimulator inputSimulator = new InputSimulator();

        private MainActionHandler _mainActionHandler;
        private BuyHandler handler;
        private SellHandler handler2;
        private Select43Handler handler43;
        private Select52Handler handler52;
        private Select53Handler handler53;
        private Select61Handler handler61;
        private Select62Handler handler62;
        private QuickFrostHandle _quickFrostHandler;
        private Kmh _kmh;

        //SerialDebugReader _serialReader;

        public static double ScaleX = 1.0;
        public static double ScaleY = 1.0;

        public Form1()
        {
            InitializeComponent();

            inputSimulator.OpenSerialPort();

            _kmh = new Kmh(true);
            _kmh.KeyDown += _kmh_KeyDown;
            _kmh.MouseDown += _kmh_MouseDown;
            _kmh.MouseUp += _kmh_MouseUp;
            _kmh.KeyUp += _kmh_KeyUp;

            _mainActionHandler = new MainActionHandler(inputSimulator);
            handler = new BuyHandler(inputSimulator);
            handler2 = new SellHandler(inputSimulator);
            handler43 = new Select43Handler(inputSimulator);
            handler52 = new Select52Handler(inputSimulator);
            handler53 = new Select53Handler(inputSimulator);
            handler61 = new Select61Handler(inputSimulator);
            handler62 = new Select62Handler(inputSimulator);
            _quickFrostHandler = new QuickFrostHandle(inputSimulator);

            //_serialReader = new SerialDebugReader("COM4", 115200);

            //_serialReader.LineReceived += line =>
            //{
            //    Debug.WriteLine(line);
            //};

            //_serialReader.Start();

        }

        private bool _kmh_KeyDown(int wParam, KBDLLHOOKSTRUCT lParam)
        {
            try
            {
                if (buyAndSellRadioButton.Checked)
                {
                    if (lParam.vkCode == 0x64)
                    {
                        RunAsync(handler43.Run);
                        return true;
                    }
                    if (lParam.vkCode == 0x65)
                    {
                        RunAsync(handler52.Run);
                        return true;
                    }
                    if (lParam.vkCode == 0x66)
                    {
                        RunAsync(handler61.Run);
                        return true;
                    }
                    if (lParam.vkCode == 0x68)
                    {
                        RunAsync(handler53.Run);
                        return true;
                    }
                    if (lParam.vkCode == 0x69)
                    {
                        RunAsync(handler62.Run);
                        return true;
                    }
                }
                //if (quickFrostCheckBox.Checked &&
                //    (lParam.vkCode == (int)Keys.Q || lParam.vkCode == 0x419) &&
                //    spamActivated == false)
                //{
                //    // этот код начинает перехватывать оба устройства
                //    spamActivated = true;
                //    RunAsync(_quickFrostHandler.Activate);
                //    return true;
                //}

                //if (quickFrostCheckBox.Checked &&
                //  //(lParam.vkCode == (int)Keys.Q || lParam.vkCode == 0x419) &&
                //  (lParam.flags & LLKHF_INJECTED) != 0)
                //{

                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Trown error while hook key down. Restart app please." + Environment.NewLine + Environment.NewLine + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }

        public bool spamActivated = false;

        private bool _kmh_KeyUp(int wParam, KBDLLHOOKSTRUCT lParam)
        {
            try
            {
                //if (quickFrostCheckBox.Checked &&
                //    (lParam.vkCode == (int)Keys.Q || lParam.vkCode == 0x419) &&
                //    spamActivated == true)
                //{
                //    // этот код начинает перехватывать оба устройства
                //    spamActivated = false;
                //    RunAsync(_quickFrostHandler.Deactivate);
                //    return true;
                //}

                //if (quickFrostCheckBox.Checked &&
                //  //(lParam.vkCode == (int)Keys.Q || lParam.vkCode == 0x419) &&
                //  (lParam.flags & LLKHF_INJECTED) != 0)
                //{

                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Trown error while hook key down. Restart app please." + Environment.NewLine + Environment.NewLine + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }

        private void RunAsync(Func<Task> handler)
        {
            Task.Run(async () =>
            {
                try
                {
                    await handler();
                }
                catch (Exception ex)
                {
                    //LogError(ex);
                    // Если нужно что-то показать на UI, делаем это из UI-потока:
                    this.BeginInvoke(new Action(() =>
                    {
                        MessageBox.Show(this,
                            "Ошибка при выполнении действия:\n" + ex.Message,
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }));
                }
            });
        }

        private bool _kmh_MouseDown(int wParam, MSLLHOOKSTRUCT lParam)
        {
            try
            {
                if ((lParam.mouseData >> 16) == XBUTTON2)
                {
                    if (buyAndSellRadioButton.Checked)
                    {
                        //const int VK_CONTROL = 0x11;
                        //bool ctrlPressed = (GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0;

                        RunAsync(_mainActionHandler.Run);

                        return true; // событие обработано
                    }
                    else if (spamQRadioButton.Checked && spamActivated == false)
                    {
                        spamActivated = true;
                        RunAsync(_quickFrostHandler.Activate);
                        return true;
                    }

                    return true;
                }

                if ((lParam.mouseData >> 16) == XBUTTON1)
                {
                    if (this.WindowState == FormWindowState.Minimized)
                        this.WindowState = FormWindowState.Normal;

                    var pos = Cursor.Position;

                    this.StartPosition = FormStartPosition.Manual;
                    this.Location = pos;

                    if (!this.Visible)
                        this.Show();

                    this.Show();
                    this.Activate();
                    return true;
                }
            }
            catch (Exception ex)
            {
                //LogError(ex);
                this.BeginInvoke(new Action(() =>
                {
                    MessageBox.Show(this,
                        "Ошибка в хуке мыши:\n" + ex.Message,
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }));
            }

            return false;
        }

        private bool _kmh_MouseUp(int wParam, MSLLHOOKSTRUCT lParam)
        {
            try
            {
                if ((lParam.mouseData >> 16) == XBUTTON2)
                {
                    if (spamQRadioButton.Checked &&
                        spamActivated == true)
                    {
                        spamActivated = false;
                        RunAsync(_quickFrostHandler.Deactivate);
                        return true;
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                //LogError(ex);
                this.BeginInvoke(new Action(() =>
                {
                    MessageBox.Show(this,
                        "Ошибка в хуке мыши:\n" + ex.Message,
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }));
            }

            return false;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _kmh.unhook();

            inputSimulator?.Dispose();
            //_serialReader?.Dispose();

            base.OnFormClosing(e);
        }

        #region WinAPI

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        #endregion

        private void bootsellButton_Click(object sender, EventArgs e)
        {
            inputSimulator.SendCommand("BOOTSELL 1 1");
        }

        private void testButton_Click(object sender, EventArgs e)
        {
            RunAsync(handler43.Run);
        }

        private void calibrateButton_Click(object sender, EventArgs e)
        {
            MouseCalibration calibration = new MouseCalibration(inputSimulator);
            var result = calibration.Calibrate();
            Form1.ScaleX = result.scaleX;
            Form1.ScaleY = result.scaleY;
        }
    }
}
