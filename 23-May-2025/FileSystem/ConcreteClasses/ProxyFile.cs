using FileAccess.Classes;
using FileAccess.Interfaces;
using FileAccess.Models;
public class ProxyFile : IFile
{
    private RealFile _realFile;
    private User _user;
    private string _filename;

    public ProxyFile(string filename, User user)
    {
        _filename = filename;
        _user = user;
    }

    public void Read()
    {
        switch (_user.role.ToLower())
        {
            case "admin":
                _realFile = new RealFile(_filename);
                _realFile.Read();
                break;

            case "user":
                Console.WriteLine($"[Limited Access] Hello {_user.username}, you can view only the file metadata of '{_filename}'.");
                _realFile = new RealFile(_filename);
                _realFile.ReadMetadata();
                break;

            case "guest":
            default:
                Console.WriteLine($"[Access Denied] Hello {_user.username}, you do not have permission to read this file.");
                break;
        }
    }
}
