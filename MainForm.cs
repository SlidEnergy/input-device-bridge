using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Forms;
using System.Text.Json;
using tser.Battle.Maps;
using System.Management;
using System.IO.Ports;
using OpenCvSharp.ML;
using System.Text;
using Windows.Graphics.Capture;
using tser.Wgc;
using System.Text.RegularExpressions;
using Microsoft.UI.Xaml.Controls;
using OpenCvSharp;

namespace tser
{
    internal partial class MainForm : Form
    {
        private const int XBUTTON1 = 0x0001;
        private const int XBUTTON2 = 0x0002;
        private const int LLKHF_INJECTED = 0x10;

        private InputSimulator inputSimulator = new InputSimulator();

        private Select43Handler handler43;
        private Select52Handler handler52;
        private Select53Handler handler53;
        private Select61Handler handler61;
        private Select62Handler handler62;
        private SpamQHandler _spamQHandler;
        private SpamEHandler _spamEHandler;
        private LowHpPlayerHandler _lowHpPlayerHelperHandler;
        private MarkerHelperHandler _markerHelperHandler;

        private Kmh _kmh;
        private AppSettings _settings;
        private readonly IServiceProvider _serviceProvider;
        private RegionManager _regionManager;
        private MapsManager _mapManager;

        //SerialDebugReader _serialReader;

        public static double ScaleX = 1.0;
        public static double ScaleY = 1.0;

        private HandlerContext uiContext = new HandlerContext();

        private bool _debugMode = false; //System.Diagnostics.Debugger.IsAttached;
        private bool _isConnected = false;

        public MainForm(IServiceProvider provider)
        {
            InitializeComponent();

            uiContext.SynchronizationContext = SynchronizationContext.Current;

            _serviceProvider = provider;

            inputSimulator = provider.GetRequiredService<InputSimulator>();
            _regionManager = provider.GetRequiredService<RegionManager>();
            _settings = provider.GetRequiredService<AppSettings>();
            _mapManager = provider.GetRequiredService<MapsManager>();

            // Отключаем лаги при отладке
            var autohook = !_debugMode;

            _kmh = new Kmh(autohook);
            _kmh.KeyDown += _kmh_KeyDown;
            _kmh.MouseDown += _kmh_MouseDown;
            _kmh.MouseUp += _kmh_MouseUp;
            _kmh.KeyUp += _kmh_KeyUp;

            handler43 = _serviceProvider.GetRequiredService<Select43Handler>();
            handler52 = _serviceProvider.GetRequiredService<Select52Handler>();
            handler53 = _serviceProvider.GetRequiredService<Select53Handler>();
            handler61 = _serviceProvider.GetRequiredService<Select61Handler>();
            handler62 = _serviceProvider.GetRequiredService<Select62Handler>();
            _spamQHandler = _serviceProvider.GetRequiredService<SpamQHandler>();
            _spamEHandler = _serviceProvider.GetRequiredService<SpamEHandler>();
            _lowHpPlayerHelperHandler = _serviceProvider.GetRequiredService<LowHpPlayerHandler>();
            _markerHelperHandler = _serviceProvider.GetRequiredService<MarkerHelperHandler>();

            _settings.TradingSettings.AllowedBestPriceOrderPosition = (int)allowedBestPriceOrderPositionNumericUpDown.Value;
            _settings.BattleSettings.LootStrategy = bestLootStrategyRadioButton.Checked ? LootStrategy.Best : LootStrategy.All;
            _settings.BattleSettings.OpenLootWindow = openLootWindowCheckBox.Checked;
            _settings.TradingSettings.FastBuy = fastBuyCheckBox.Checked;
        }

        ManagementEventWatcher watcher;

        private void StartWatching()
        {
            watcher = new ManagementEventWatcher(
                new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent")
            );

            watcher.EventArrived += (s, e) =>
            {
                // Важно: событие не в UI-потоке
                BeginInvoke(new Action(UpdatePorts));
            };

            watcher.Start();
        }

        private void StopWatching()
        {
            watcher?.Stop();
            watcher?.Dispose();
        }

        private void UpdatePorts()
        {
            string selected = comPortsComboBox.SelectedItem as string;

            comPortsComboBox.Items.Clear();
            comPortsComboBox.Items.AddRange(SerialPort.GetPortNames());

            if (selected != null && comPortsComboBox.Items.Contains(selected))
                comPortsComboBox.SelectedItem = selected;
            else if (comPortsComboBox.Items.Count > 0)
                comPortsComboBox.SelectedIndex = 0;
        }

        private async void Form1_Shown(object sender, EventArgs e)
        {
            await _mapManager.Load();
            StartWatching();
        }

        //private CancellationTokenSource _cts = new();

        //private async Task MonitorPortAsync()
        //{
        //    while (!_cts.Token.IsCancellationRequested)
        //    {
        //        if (!inputSimulator.IsOpen)
        //        {
        //            try
        //            {
        //                inputSimulator.OpenSerialPort();
        //                Debug.WriteLine("COM port opened");
        //            }
        //            catch
        //            {
        //                // игнор, пробуем снова
        //            }
        //        }

        //        await Task.Delay(2000, _cts.Token);
        //    }
        //}

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        private bool IsGameWindowActive()
        {
            IntPtr hwnd = GetForegroundWindow(); // Получаем дескриптор активного окна
            StringBuilder windowText = new StringBuilder(255);
            GetWindowText(hwnd, windowText, 255); // Получаем название активного окна

            string activeWindowTitle = windowText.ToString();

            // Проверяем, если название окна совпадает с нужным
            string targetWindowTitle = "Albion Online Client"; // Замените на название игры
            if (activeWindowTitle.Contains(targetWindowTitle))
                return true;
            else
                return false;
        }

        private bool _kmh_KeyDown(int wParam, KBDLLHOOKSTRUCT lParam)
        {
            try
            {
                if (numPadCheckBox.Checked)
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
                //if (lParam.vkCode == 0x061 && lowHpHelperRadioButton.Checked && lowHpPlayerHelperActivated)
                //{
                //    RunAsync(_lowHpPlayerHelperHandler.Run);

                //    return true;
                //}
                //else if (lParam.vkCode == 0x062 && fastLootRadioButton.Checked)
                //{
                //    var handler = _serviceProvider.GetRequiredService<FastLootHandler>();
                //    RunAsync(handler.Run);
                //    return true;
                //}
                //else if (lParam.vkCode == 0x063 && gateHelperRadioButton.Checked)
                //{
                //    var handler = _serviceProvider.GetRequiredService<GateHelperHandler>();
                //    RunAsync(handler.Run);

                //    return true;
                //}


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

        private void RunAsync(Func<HandlerContext, Task> handler)
        {
            Task.Run(async () =>
            {
                try
                {
                    await handler(uiContext);
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
                const int VK_CONTROL = 0x11;
                bool ctrlPressed = (GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0;

                if ((lParam.mouseData >> 16) == XBUTTON2 && (!_debugMode || IsGameWindowActive()))
                {
                    if (setPositionMode)
                    {
                        RunAsync(_lowHpPlayerHelperHandler.Calibrate);
                        setPositionMode = false;
                        return true;
                    }

                    // With Ctrl
                    if (ctrlPressed)
                    {
                        if (gateHelperRadioButton.Checked)
                        {
                            var handler = _serviceProvider.GetRequiredService<GateHelperHandler>();
                            RunAsync(handler.Run);

                            return true;
                        }
                    }

                    // Without modificator

                    if (buyAndSellRadioButton.Checked)
                    {
                        //const int VK_CONTROL = 0x11;
                        //bool ctrlPressed = (GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0;
                        var handler = _serviceProvider.GetRequiredService<MarketActionHandler>();
                        RunAsync(handler.Run);

                        return true; // событие обработано
                    }
                    else if (spamQRadioButton.Checked && spamActivated == false)
                    {
                        spamActivated = true;
                        RunAsync(_spamQHandler.Activate);
                        return true;
                    }
                    else if (spamERadioButton.Checked && spamActivated == false)
                    {
                        spamActivated = true;
                        RunAsync(_spamEHandler.Activate);
                        return true;
                    }
                    else if (lowHpHelperRadioButton.Checked && lowHpPlayerHelperActivated)
                    {
                        RunAsync(_lowHpPlayerHelperHandler.Run);

                        return true;
                    }
                }

                //if ((lParam.mouseData >> 16) == XBUTTON1 && IsGameWindowActive())
                //{
                //    if (this.WindowState == FormWindowState.Minimized)
                //        this.WindowState = FormWindowState.Normal;

                //    var pos = Cursor.Position;

                //    this.StartPosition = FormStartPosition.Manual;
                //    this.Location = pos;

                //    if (!this.Visible)
                //        this.Show();

                //    this.Show();
                //    this.Activate();
                //    return true;
                //}

                if ((lParam.mouseData >> 16) == XBUTTON1 && IsGameWindowActive())
                {
                    // With Ctrl
                    if (ctrlPressed)
                    {
                        if (markerHelperRadioButton.Checked)
                        {
                            RunAsync(_markerHelperHandler.Run);

                            return true;
                        }
                    }

                    if (fastLootRadioButton.Checked)
                    {
                        var handler = _serviceProvider.GetRequiredService<FastLootHandler>();
                        RunAsync(handler.Run);
                        return true;
                    }
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
                        RunAsync(_spamQHandler.Deactivate);
                        return true;
                    }

                    if (spamERadioButton.Checked &&
                     spamActivated == true)
                    {
                        spamActivated = false;
                        RunAsync(_spamEHandler.Deactivate);
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
            StopWatching();

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

        private async void testButton_Click(object sender, EventArgs e)
        {


            //context.SynchronizationContext.Post(_ =>
            //{
            //    var form = new GateHelperForm(_mapManager);
            //    form.StartPosition = FormStartPosition.Manual;
            //    form.Location = new Point(cursor.X + 20, cursor.Y + 20);
            //    form.SetText("test text title");
            //    form.Show();
            //    form.InitAutoClose(Cursor.Position);
            //}, null);
            //});
        }

        private void lootAllCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _settings.BattleSettings.LootStrategy = bestLootStrategyRadioButton.Checked ? LootStrategy.Best : LootStrategy.All;
        }

        private void regionManagerButton_Click(object sender, EventArgs e)
        {
            using var form = _serviceProvider.GetRequiredService<RegionManagerForm>();

            form.ShowDialog();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //_regionManager.Save();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            UpdatePorts();
            _regionManager.InitRegions();

            var analyzer = _serviceProvider.GetRequiredService<ScreenAnalyzer>();
            analyzer.Init();
        }

        private async void gateHelperRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            await _mapManager.Load();
        }

        private void openLootWindowCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _settings.BattleSettings.OpenLootWindow = openLootWindowCheckBox.Checked;
        }

        private void fastBuyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _settings.TradingSettings.FastBuy = fastBuyCheckBox.Checked;
        }

        private async void connectButton_Click(object sender, EventArgs e)
        {
            inputSimulator.InitComPort((string)comPortsComboBox.SelectedItem);

            inputSimulator.OpenSerialPort();

            _isConnected = true;
            connectButton.Text = "Connected";
        }

        private void setGroupPanelPositionButton_Click(object sender, EventArgs e)
        {
            setPositionMode = true;
            //_settings.BattleSettings.GroupPanelPosition = new Point(610, 554);
        }

        private bool setPositionMode = false;
        private bool lowHpPlayerHelperActivated = false;

        private void lowHpHelperRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (lowHpHelperRadioButton.Checked && lowHpPlayerHelperActivated == false)
            {
                lowHpPlayerHelperActivated = true;
                RunAsync(_lowHpPlayerHelperHandler.Activate);
                return;
            }

            if (!lowHpHelperRadioButton.Checked && lowHpPlayerHelperActivated == true)
            {
                lowHpPlayerHelperActivated = false;
                RunAsync(_lowHpPlayerHelperHandler.Deactivate);
                return;
            }
        }


        private bool markerhelperActivated = false;

        private void markerHelperRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (markerHelperRadioButton.Checked && markerhelperActivated == false)
            {
                markerhelperActivated = true;
                RunAsync(_markerHelperHandler.Activate);
                return;
            }

            if (!markerHelperRadioButton.Checked && markerhelperActivated == true)
            {
                markerhelperActivated = false;
                RunAsync(_markerHelperHandler.Deactivate);
                return;
            }
        }

        private void nameTextBox_TextChanged(object sender, EventArgs e)
        {
            _settings.BattleSettings.Name = nameTextBox.Text;
        }

        private void initMarkerHelperButton_Click(object sender, EventArgs e)
        {
            var hwnd = WgcHelper.FindWindow(null, "Albion Online Client");

            var item = WgcHelper.CreateItemForWindow(hwnd);


            var capture = new WgcCapture();
            capture.Start(item);

            //await Task.Delay(1000);
            //var frame = capture.GetFrame();

            //var frame = await capture.WaitForFrame(capture);
            //if (frame == null)
            //    throw new TimeoutException("No frame captured within timeout");

            //Cv2.ImShow("debug", frame);

            _markerHelperHandler.SetCapture(capture);
        }

        private void hightPriorityCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _settings.BattleSettings.HightPriorityForFirst = hightPriorityCheckBox.Checked;
        }
    }
}
