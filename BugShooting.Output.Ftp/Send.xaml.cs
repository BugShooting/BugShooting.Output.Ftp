using System;
using System.Windows;

namespace BugShooting.Output.Ftp
{
  partial class Send : Window
  {

    public Send(string server, int port, string remotePath, string fileName)
    {
      InitializeComponent();

      Url.Text = string.Format("ftp://{0}:{1}", server, port); 
      RemotePathTextBox.Text = remotePath;
      FileNameTextBox.Text = fileName;

      FileNameTextBox.TextChanged += ValidateData;
      ValidateData(null, null);

    }
      
    public string RemotePath
    {
      get { return RemotePathTextBox.Text; }
    }
    
    public string FileName
    {
      get { return FileNameTextBox.Text; }
    }
    
    private void ValidateData(object sender, EventArgs e)
    {
      OK.IsEnabled = Validation.IsValid(FileNameTextBox);
    }

    private void OK_Click(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
    }

  }
}
