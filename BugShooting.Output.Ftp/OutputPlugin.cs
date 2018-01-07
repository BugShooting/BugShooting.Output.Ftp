using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.ServiceModel;
using System.Web;
using System.Threading.Tasks;
using BS.Plugin.V3.Output;
using BS.Plugin.V3.Common;
using BS.Plugin.V3.Utilities;

namespace BugShooting.Output.Ftp
{
  public class OutputPlugin: OutputPlugin<Output>
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
                          edit.OverwriteExistFile);
      }
      else
      {
        return null; 
      }

    }

    protected override OutputValues SerializeOutput(Output Output)
    {

      OutputValues outputValues = new OutputValues();

      outputValues.Add("Name", Output.Name);
      outputValues.Add("Server", Output.Server);
      outputValues.Add("Port", Output.Port.ToString());
      outputValues.Add("PassiveMode", Convert.ToString(Output.PassiveMode));
      outputValues.Add("UserName", Output.UserName);
      outputValues.Add("Password",Output.Password, true);
      outputValues.Add("RemotePath", Output.RemotePath);
      outputValues.Add("FileName", Output.FileName);
      outputValues.Add("FileFormat", Output.FileFormat);
      outputValues.Add("OverwriteExistingFile", Convert.ToString(Output.OverwriteExistingFile));

      return outputValues;
      
    }

    protected override Output DeserializeOutput(OutputValues OutputValues)
    {

      return new Output(OutputValues["Name", this.Name],
                        OutputValues["Server", ""],
                        Convert.ToInt32(OutputValues["Port", Convert.ToString(21)]),
                        Convert.ToBoolean(OutputValues["PassiveMode", Convert.ToString(false)]),
                        OutputValues["UserName", ""],
                        OutputValues["Password", ""],
                        OutputValues["RemotePath", ""],
                        OutputValues["FileName", "Screenshot"], 
                        OutputValues["FileFormat", ""],
                        Convert.ToBoolean(OutputValues["OverwriteExistingFile", Convert.ToString(false)]));

    }

    protected override async Task<SendResult> Send(IWin32Window Owner, Output Output, ImageData ImageData)
    {

      try
      {

        string fileName = AttributeHelper.ReplaceAttributes(Output.FileName, ImageData);

        // Show send window
        Send send = new Send(Output.Server, Output.Port, Output.RemotePath, fileName);

        var sendOwnerHelper = new System.Windows.Interop.WindowInteropHelper(send);
        sendOwnerHelper.Owner = Owner.Handle;

        if (!send.ShowDialog() == true)
        {
          return new SendResult(Result.Canceled);
        }

        string url = string.Format("ftp://{0}:{1}/{2}/", Output.Server, Output.Port, send.RemotePath);
        string fullFileName = send.FileName + "." + FileHelper.GetFileExtension(Output.FileFormat);
        
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
              return new SendResult(Result.Canceled);
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

              using (FtpWebResponse response = (FtpWebResponse)await listRequest.GetResponseAsync())
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
                          return new SendResult(Result.Canceled);
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
            byte[] fileBytes = FileHelper.GetFileBytes(Output.FileFormat, ImageData);

            // Upload file
            FtpWebRequest uploadRequest = (FtpWebRequest)WebRequest.Create(fileUrl);
            uploadRequest.Method = WebRequestMethods.Ftp.UploadFile;
            uploadRequest.UsePassive = Output.PassiveMode;
            uploadRequest.Credentials = new NetworkCredential(userName, password);
            using (Stream requestStream = await uploadRequest.GetRequestStreamAsync())
            {
              await requestStream.WriteAsync(fileBytes, 0, fileBytes.Length);
              requestStream.Close();
            }

            return new SendResult(Result.Success,
                                  new Output(Output.Name,
                                            Output.Server,
                                            Output.Port,
                                            Output.PassiveMode,
                                            (rememberCredentials) ? userName : Output.UserName,
                                            (rememberCredentials) ? password : Output.Password,
                                            Output.RemotePath,
                                            Output.FileName,
                                            Output.FileFormat,
                                            Output.OverwriteExistingFile));


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
        return new SendResult(Result.Failed, ex.Message);
      }

    }

  }
}
