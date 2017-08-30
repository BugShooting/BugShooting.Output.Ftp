using System.Windows;
using System.Windows.Controls;

namespace BS.Output.Ftp
{
  partial class Credentials : Window
  {

    public Credentials(string server, int port, string remotePath, string userName, string password, bool remember)
    {
      InitializeComponent();

      Url.Text = string.Format("ftp://{0}:{1}/{2}", server, port, remotePath);
      UserNameTextBox.Text = userName;
      PasswordBox.Password = password;
      RememberCheckBox.IsChecked = remember;

    }
    
    public string UserName
    {
      get { return UserNameTextBox.Text; }
    }
   
    public string Password
    {
      get { return PasswordBox.Password; }
    }

    public bool Remember
    {
      get { return RememberCheckBox.IsChecked.Value; }
    }
  
    private void OK_Click(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
    }

  }
}
