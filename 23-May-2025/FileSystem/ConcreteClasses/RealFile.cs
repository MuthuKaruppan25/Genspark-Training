using System;
using System.IO;
using FileAccess.Interfaces;

namespace FileAccess.Classes
{
    public class RealFile : IFile
    {
        private string _filename;

        public RealFile(string filename)
        {
            _filename = filename;
        }

        public void Read()
        {
            try
            {
                if (File.Exists(_filename))
                {
                    Console.WriteLine($"[Access Granted] Reading sensitive file content from '{_filename}'...\n");

                    string content = File.ReadAllText(_filename);
                    Console.WriteLine(content);
                }
                else
                {
                    Console.WriteLine($"[Error] File '{_filename}' does not exist.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Could not read file: {ex.Message}");
            }
        }

        
        public void ReadMetadata()
        {
            try
            {
                if (File.Exists(_filename))
                {
                    FileInfo fileInfo = new FileInfo(_filename);

                    Console.WriteLine($"Metadata for file '{_filename}':");
                    Console.WriteLine($"- Size: {fileInfo.Length} bytes");
                    Console.WriteLine($"- Created: {fileInfo.CreationTime}");
                    Console.WriteLine($"- Last Modified: {fileInfo.LastWriteTime}");
                    Console.WriteLine($"- Attributes: {fileInfo.Attributes}");
                }
                else
                {
                    Console.WriteLine($"[Error] File '{_filename}' does not exist.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Could not read file metadata: {ex.Message}");
            }
        }
    }
}
