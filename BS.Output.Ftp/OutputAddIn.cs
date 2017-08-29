using System;
using System.Drawing;
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
                          edit.Url,
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
      outputValues.Add(new OutputValue("Url", Output.Url));
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
                        OutputValues["Url", ""].Value,
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

        // TODO
        return null;

      }
      catch (Exception ex)
      {
        return new V3.SendResult(V3.Result.Failed, ex.Message);
      }

    }
      
  }
}
