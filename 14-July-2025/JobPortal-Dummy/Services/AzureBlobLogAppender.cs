
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using log4net.Appender;
using log4net.Core;
using System.Text;

namespace JobPortal.Services
{
    public class AzureBlobLogAppender : AppenderSkeleton
    {

        public string? SasUrl { get; set; }
        public string? BlobName { get; set; }

        protected override void Append(LoggingEvent loggingEvent)
        {
            if (string.IsNullOrEmpty(SasUrl) || string.IsNullOrEmpty(BlobName))
                return;

            try
            {
                string message = RenderLoggingEvent(loggingEvent) + Environment.NewLine;

                string date = DateTime.UtcNow.ToString("yyyy-MM-dd");


                string directory = Path.GetDirectoryName(BlobName)?.Replace("\\", "/") ?? "";
                string baseName = Path.GetFileNameWithoutExtension(BlobName);
                string finalBlobName = $"{directory}/{baseName}-{date}.txt";

                var containerClient = new BlobContainerClient(new Uri(SasUrl));
                var appendBlobClient = containerClient.GetAppendBlobClient(finalBlobName);

      
                if (!appendBlobClient.Exists())
                {
                    appendBlobClient.CreateIfNotExists();
                }

          
                using var stream = new MemoryStream(Encoding.UTF8.GetBytes(message));
                appendBlobClient.AppendBlock(stream);
            }
            catch (Exception ex)
            {
                Console.WriteLine("AzureBlobLogAppender: Failed to write log to blob - " + ex.Message);
            }
        }
    }
}
