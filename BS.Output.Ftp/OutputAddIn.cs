using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.ServiceModel;
using System.Web;
using System.Threading.Tasks;

namespace BS.Output.Ftp
{
  public class OutputAddIn: V3.OutputAddIn<Output>
  {

    protected override string Name
    {
      get { return "FTP"; }
    }

    protected override Image Image64
    {
      get  { return Properties.Resources.logo_64; }
    }

    protected override Image Image16
    {
      get { return Properties.Resources.logo_16 ; }
    }

    protected override bool Editable
    {
      get { return true; }
    }

    protected override string Description
    {
      get { return "Upload screenshots to a FTP server."; }
    }
    
    protected override Output CreateOutput(IWin32Window Owner)
    {
      
      Output output = new Output(Name, 
                                 String.Empty, 
                                 21,
                                 false,
                                 String.Empty,
                                 String.Empty, 
                                 String.Empty, 
                                 "Screenshot",
                                 String.Empty, 
                                 false,
                                 false,
                                 false);

      return EditOutput(Owner, output);

    }

    protected override Output EditOutput(IWin32Window Owner, Output Output)
    {

      Edit edit = new Edit(Output);

      var ownerHelper = new System.Windows.Interop.WindowInteropHelper(edit);
      ownerHelper.Owner = Owner.Handle;
      
      if (edit.ShowDialog() == true) {

        return new Output(edit.OutputName,
                          edit.Server,
                          edit.Port,
                          edit.PassiveMode,
                          edit.UserName,
                          edit.Password,
                          edit.RemotePath,
                          edit.FileName,
                          edit.FileFormat,
                          edit.OverwriteExistFile,
                          edit.OpenFileInBrowser,
                          edit.CopyFileUrl);
      }
      else
      {
        return null; 
      }

    }

    protected override OutputValueCollection SerializeOutput(Output Output)
    {

      OutputValueCollection outputValues = new OutputValueCollection();

      outputValues.Add(new OutputValue("Name", Output.Name));
      outputValues.Add(new OutputValue("Server", Output.Server));
      outputValues.Add(new OutputValue("Port", Output.Port.ToString()));
      outputValues.Add(new OutputValue("PassiveMode", Convert.ToString(Output.PassiveMode)));
      outputValues.Add(new OutputValue("UserName", Output.UserName));
      outputValues.Add(new OutputValue("Password",Output.Password, true));
      outputValues.Add(new OutputValue("RemotePath", Output.RemotePath));
      outputValues.Add(new OutputValue("FileName", Output.FileName));
      outputValues.Add(new OutputValue("FileFormat", Output.FileFormat));
      outputValues.Add(new OutputValue("OverwriteExistingFile", Convert.ToString(Output.OverwriteExistingFile)));
      outputValues.Add(new OutputValue("OpenFilyInBrowser", Convert.ToString(Output.OpenFileInBrowser)));
      outputValues.Add(new OutputValue("CopyFileUrl", Convert.ToString(Output.CopyFileUrl)));

      return outputValues;
      
    }

    protected override Output DeserializeOutput(OutputValueCollection OutputValues)
    {

      return new Output(OutputValues["Name", this.Name].Value,
                        OutputValues["Server", ""].Value,
                        Convert.ToInt32(OutputValues["Port", Convert.ToString(21)].Value),
                        Convert.ToBoolean(OutputValues["PassiveMode", Convert.ToString(false)].Value),
                        OutputValues["UserName", ""].Value,
                        OutputValues["Password", ""].Value,
                        OutputValues["RemotePath", ""].Value,
                        OutputValues["FileName", "Screenshot"].Value, 
                        OutputValues["FileFormat", ""].Value,
                        Convert.ToBoolean(OutputValues["OverwriteExistingFile", Convert.ToString(false)].Value),
                        Convert.ToBoolean(OutputValues["OpenFileInBrowser", Convert.ToString(false)].Value),
                        Convert.ToBoolean(OutputValues["CopyFileUrl", Convert.ToString(false)].Value));

    }

    protected override async Task<V3.SendResult> Send(IWin32Window Owner, Output Output, V3.ImageData ImageData)
    {

      try
      {

        string fileName = V3.FileHelper.GetFileName(Output.FileName, Output.FileFormat, ImageData);

        // Show send window
        Send send = new Send(Output.Server, Output.Port, Output.RemotePath, fileName);

        var sendOwnerHelper = new System.Windows.Interop.WindowInteropHelper(send);
        sendOwnerHelper.Owner = Owner.Handle;

        if (!send.ShowDialog() == true)
        {
          return new V3.SendResult(V3.Result.Canceled);
        }

        string url = string.Format("ftp://{0}:{1}/{2}/", Output.Server, Output.Port, send.RemotePath);
        string fullFileName = send.FileName + "." + V3.FileHelper.GetFileExtention(Output.FileFormat);
        
        string userName = Output.UserName;
        string password = Output.Password;
        bool showLogin = string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password);
        bool rememberCredentials = false;
        
        while (true)
        {

          if (showLogin)
          {

            // Show credentials window
            Credentials credentials = new Credentials(Output.Server, Output.Port, Output.RemotePath, userName, password, rememberCredentials);

            var credentialsOwnerHelper = new System.Windows.Interop.WindowInteropHelper(credentials);
            credentialsOwnerHelper.Owner = Owner.Handle;

            if (credentials.ShowDialog() != true)
            {
              return new V3.SendResult(V3.Result.Canceled);
            }

            userName = credentials.UserName;
            password = credentials.Password;
            rememberCredentials = credentials.Remember;

          }

          try
          {
            
            // Check if file already exists
            if (!Output.OverwriteExistingFile)
            {
            
              FtpWebRequest listRequest = (FtpWebRequest)WebRequest.Create(url);
              listRequest.Method = WebRequestMethods.Ftp.ListDirectory;
              listRequest.UsePassive = Output.PassiveMode;
              listRequest.Credentials = new NetworkCredential(userName, password);

              using (FtpWebResponse response = (FtpWebResponse)listRequest.GetResponse())
              {
                long size = response.ContentLength;
                using (Stream dataStream = response.GetResponseStream())
                {
                  using (StreamReader reader = new StreamReader(dataStream))
                  {

                    string line = reader.ReadLine();
                  
                    while (line != null)
                    {

                      if (line == fullFileName)
                      {

                        if (MessageBox.Show(Owner, string.Format("File \'{0}\' already exists.\nOverwrite?", fullFileName), Output.Name, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                          break;
                        }
                        else
                        {
                          return new V3.SendResult(V3.Result.Canceled);
                        }

                      }

                      line = reader.ReadLine();
                    }

                    reader.Close();
                  }
                  dataStream.Close();
                }
                response.Close();
              }

            }


            string fileUrl = url + fullFileName;
            byte[] fileBytes = V3.FileHelper.GetFileBytes(Output.FileFormat, ImageData);

            // Upload file
            FtpWebRequest uploadRequest = (FtpWebRequest)WebRequest.Create(fileUrl);
            uploadRequest.Method = WebRequestMethods.Ftp.UploadFile;
            uploadRequest.UsePassive = Output.PassiveMode;
            uploadRequest.Credentials = new NetworkCredential(userName, password);
            using (Stream requestStream = uploadRequest.GetRequestStream())
            {
              requestStream.Write(fileBytes, 0, fileBytes.Length);
              requestStream.Close();
            }


            // Open file in browser
            if (Output.OpenFileInBrowser)
            {
              V3.WebHelper.OpenUrl(url + fullFileName);
            }

            // Copy file URL
            if (Output.CopyFileUrl)
            {
              Clipboard.SetText(url + fullFileName);
            }

            return new V3.SendResult(V3.Result.Success,
                                     new Output(Output.Name,
                                                Output.Server,
                                                Output.Port,
                                                Output.PassiveMode,
                                                (rememberCredentials) ? userName : Output.UserName,
                                                (rememberCredentials) ? password : Output.Password,
                                                Output.RemotePath,
                                                Output.FileName,
                                                Output.FileFormat,
                                                Output.OverwriteExistingFile,
                                                Output.OpenFileInBrowser,
                                                Output.CopyFileUrl));


          }
          catch (WebException ex) when (ex.Response is FtpWebResponse)
          {

            switch (((FtpWebResponse)ex.Response).StatusCode)
            {
              case FtpStatusCode.NotLoggedIn:
                // Login failed
                showLogin = true;
                break;
              default:
                throw;
            }

          }

        }

      }
      catch (Exception ex)
      {
        return new V3.SendResult(V3.Result.Failed, ex.Message);
      }

    }

  }
}
