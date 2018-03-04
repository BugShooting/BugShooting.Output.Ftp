using BS.Plugin.V3.Output;
using System;

namespace BugShooting.Output.Ftp
{

  public class Output: IOutput 
  {
    
    string name;
    string server;
    int port;
    bool passiveMode;
    string userName;
    string password;
    string remotePath;
    string fileName;
    Guid fileFormatID;
    bool overwriteExistingFile;

    public Output(string name,
                  string server,
                  int port,
                  bool passiveMode,
                  string userName,
                  string password,
                  string remotePath,
                  string fileName,
                  Guid fileFormatID,
                  bool overwriteExistingFile)
    {
      this.name = name;
      this.server = server;
      this.port = port;
      this.passiveMode = passiveMode;
      this.userName = userName;
      this.password = password;
      this.remotePath = remotePath;
      this.fileName = fileName;
      this.fileFormatID = fileFormatID;
      this.overwriteExistingFile = overwriteExistingFile;
    }
    
    public string Name
    {
      get { return name; }
    }

    public string Information
    {
      get { return string.Format("ftp://{0}:{1}/{2}", server, port, remotePath); }
    }

    public string Server
    {
      get { return server; }
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

    public Guid FileFormatID
    {
      get { return fileFormatID; }
    }

    public bool OverwriteExistingFile
    {
      get { return overwriteExistingFile; }
    }

  }
}
