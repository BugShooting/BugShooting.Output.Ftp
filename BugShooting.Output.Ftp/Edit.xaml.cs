using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BS.Plugin.V3.Utilities;

namespace BugShooting.Output.Ftp
{
  partial class Edit : Window
  {

    public Edit(Output output)
    {
      InitializeComponent();

      foreach (string fileNameReplacement in AttributeHelper.GetAttributeReplacements())
      {
        MenuItem item = new MenuItem();
        item.Header = new TextBlock() { Text = fileNameReplacement };
        item.Tag = fileNameReplacement;
        item.Click += FileNameReplacementItem_Click;
        FileNameReplacementList.Items.Add(item);
      }

      IEnumerable<string> fileFormats = FileHelper.GetFileFormats();
      foreach (string fileFormat in fileFormats)
      {
        ComboBoxItem item = new ComboBoxItem();
        item.Content = fileFormat;
        item.Tag = fileFormat;
        FileFormatComboBox.Items.Add(item);
      }

      NameTextBox.Text = output.Name;
      ServerTextBox.Text = output.Server;
      PortTextBox.Text = output.Port.ToString();
      RemotePathTextBox.Text = output.RemotePath;
      PassiveModeCheckBox.IsChecked = output.PassiveMode;
      UserNameTextBox.Text = output.UserName;
      PasswordBox.Password = output.Password;
      FileNameTextBox.Text = output.FileName;

      if (fileFormats.Contains(output.FileFormat))
      {
        FileFormatComboBox.SelectedValue = output.FileFormat;
      }
      else {
        FileFormatComboBox.SelectedValue = fileFormats.First();
      }
            
      OverwriteExistFileCheckBox.IsChecked = output.OverwriteExistingFile;
    
      NameTextBox.TextChanged += ValidateData;
      ServerTextBox.TextChanged += ValidateData;
      PortTextBox.TextChanged += ValidateData;
      FileFormatComboBox.SelectionChanged += ValidateData;
      ValidateData(null, null);

      ServerTextBox.Focus();

    }
     
    public string OutputName
    {
      get { return NameTextBox.Text; }
    }

    public string Server
    {
      get { return ServerTextBox.Text; }
    }

    public int Port
    {
      get { return Convert.ToInt32(PortTextBox.Text); }
    }

    public bool PassiveMode
    {
      get { return PassiveModeCheckBox.IsChecked.Value; }
    }

    public string RemotePath
    {
      get { return RemotePathTextBox.Text; }
    }

    public string UserName
    {
      get { return UserNameTextBox.Text; }
    }

    public string Password
    {
      get { return PasswordBox.Password; }
    }

    public string FileName
    {
      get { return FileNameTextBox.Text; }
    }

    public string FileFormat
    {
      get { return (string)FileFormatComboBox.SelectedValue; }
    }

    public bool OverwriteExistFile
    {
      get { return OverwriteExistFileCheckBox.IsChecked.Value; }
    }

    private void Port_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
      e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
    }


    private void FileNameReplacement_Click(object sender, RoutedEventArgs e)
    {
      FileNameReplacement.ContextMenu.IsEnabled = true;
      FileNameReplacement.ContextMenu.PlacementTarget = FileNameReplacement;
      FileNameReplacement.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
      FileNameReplacement.ContextMenu.IsOpen = true;
    }

    private void FileNameReplacementItem_Click(object sender, RoutedEventArgs e)
    {

      MenuItem item = (MenuItem)sender;

      int selectionStart = FileNameTextBox.SelectionStart;

      FileNameTextBox.Text = FileNameTextBox.Text.Substring(0, FileNameTextBox.SelectionStart) + item.Tag.ToString() + FileNameTextBox.Text.Substring(FileNameTextBox.SelectionStart, FileNameTextBox.Text.Length - FileNameTextBox.SelectionStart);

      FileNameTextBox.SelectionStart = selectionStart + item.Tag.ToString().Length;
      FileNameTextBox.Focus();

    }

    private void ValidateData(object sender, EventArgs e)
    {
      OK.IsEnabled = Validation.IsValid(NameTextBox) &&
                     Validation.IsValid(ServerTextBox) &&
                     Validation.IsValid(PortTextBox) &&
                     Validation.IsValid(FileFormatComboBox);
    }

    private void OK_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = true;     
    }

    private void FullPath_TextChanged(object sender, TextChangedEventArgs e)
    {
      FullPath.Text = string.Format("ftp://{0}:{1}/{2}", ServerTextBox.Text, PortTextBox.Text, RemotePathTextBox.Text);
    }

  }
}
