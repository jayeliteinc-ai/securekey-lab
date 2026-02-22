using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;

namespace iPhoneTool;

public partial class MainWindow : Window
{
    private readonly DeviceManager _deviceManager;
    private readonly RestoreManager _restoreManager;
    private readonly ObservableCollection<DeviceInfo> _devices = new();

    public MainWindow()
    {
        InitializeComponent();
        _deviceManager = new DeviceManager("bin");
        _restoreManager = new RestoreManager("bin");
        DeviceSelector.ItemsSource = _devices;
    }

    private async void RefreshDevicesButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Log("Refreshing device list...");
            var devices = await _deviceManager.GetConnectedDevicesAsync();

            _devices.Clear();
            foreach (var device in devices)
            {
                _devices.Add(device);
            }

            Log($"Found {_devices.Count} connected device(s).");
        }
        catch (Exception ex)
        {
            Log($"Error: {ex.Message}");
        }
    }

    private async void RestoreButton_Click(object sender, RoutedEventArgs e)
    {
        if (DeviceSelector.SelectedItem is not DeviceInfo selectedDevice)
        {
            Log("Select a device first.");
            return;
        }

        var picker = new OpenFileDialog
        {
            Title = "Select IPSW file",
            Filter = "IPSW files (*.ipsw)|*.ipsw|All files (*.*)|*.*"
        };

        if (picker.ShowDialog() != true)
        {
            return;
        }

        try
        {
            Log($"Starting restore for {selectedDevice.DisplayName}...");
            var output = await _restoreManager.RestoreAsync(selectedDevice.UdId, picker.FileName);
            Log(output);
            Log("Restore command completed.");
        }
        catch (Exception ex)
        {
            Log($"Restore failed: {ex.Message}");
        }
    }

    private void Log(string message)
    {
        LogOutput.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
        LogOutput.ScrollToEnd();
    }
}
