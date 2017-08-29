namespace BS.Output.Ftp
{

  public class Output: IOutput 
  {
    
    string name;
    string url;
    int port;
    bool passiveMode;
    string userName;
    string password;
    string remotePath;
    string fileName;
    string fileFormat;
    bool overwriteExistingFile;
    bool openFileInBrowser;
    bool copyFileUrl;

    public Output(string name,
                  string url,
                  int port,
                  bool passiveMode,
                  string userName,
                  string password,
                  string remotePath,
                  string fileName, 
                  string fileFormat,
                  bool overwriteExistingFile,
                  bool openFileInBrowser, 
                  bool copyFileUrl)
    {
      this.name = name;
      this.url = url;
      this.port = port;
      this.passiveMode = passiveMode;
      this.userName = userName;
      this.password = password;
      this.remotePath = remotePath;
      this.fileName = fileName;
      this.fileFormat = fileFormat;
      this.overwriteExistingFile = overwriteExistingFile;
      this.openFileInBrowser = openFileInBrowser;
      this.copyFileUrl = copyFileUrl;
    }
    
    public string Name
    {
      get { return name; }
    }

    public string Information
    {
      get { return url; }
    }

    public string Url
    {
      get { return url; }
    }

    public int Port
    {
      get { return port; }
    }

    public bool PassiveMode
    {
      get { return passiveMode; }
    }

    public string UserName
    {
      get { return userName; }
    }

    public string Password
    {
      get { return password; }
    }

    public string RemotePath
    {
      get { return remotePath; }
    }

    public string FileName
    {
      get { return fileName; }
    }

    public string FileFormat
    {
      get { return fileFormat; }
    }

    public bool OverwriteExistingFile
    {
      get { return overwriteExistingFile; }
    }

    public bool OpenFileInBrowser
    {
      get { return openFileInBrowser; }
    }

    public bool CopyFileUrl
    {
      get { return copyFileUrl; }
    }

  }
}
