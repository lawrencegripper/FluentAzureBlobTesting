using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentAzureBlobTesting
{
    /// <summary>
    /// Extensions to allow easy integration testing of Blob storage
    /// using the emulator. 
    /// Requires Azure SDK 2.3
    /// </summary>
    public static class FluentBlobAssertions
    {
        public static CloudBlobContainer AssertContainerExists(this CloudBlobClient blobClient, string containerName)
        {
            var container = blobClient.GetContainerReference(containerName);
            if (!container.Exists())
            {
                Assert.Fail(string.Format("Container doesn't exist '{0}'", containerName));
            }

            Trace.WriteLine(string.Format("Container exists '{0}'", containerName));

            return container;
        }

        public static ICloudBlob AssertBlobExists(this CloudBlobContainer blobContainer, string blobName)
        {
            var blob = blobContainer.GetBlobReferenceFromServer(blobName);
            if (!blob.Exists())
            {
                Assert.Fail(string.Format("Blog doesn't exist '{0}' in conatiner '{1}'", blobName, blobContainer.Name));
            }

            Trace.WriteLine(string.Format("Blob exists '{0}'", blobName));

            return blob;
        }

        public static ICloudBlob AssertBlobDataIs(this ICloudBlob blob, byte[] expectedData)
        {
            byte[] blobData = new byte[blob.Properties.Length];
            var blobBytes = blob.DownloadToByteArray(blobData, 0);

            if (!blobData.SequenceEqual(expectedData))
            {
                Assert.Fail(string.Format("Blob data doesn't match blobName '{0}'", blob.Name));
            }

            Trace.WriteLine(string.Format("Blob data matches blobName '{0}'", blob.Name));

            return blob;
        }

        public static ICloudBlob AssertBlobContainsMetaData(this ICloudBlob blob, string key, string value)
        {
            if (!blob.Metadata.ContainsKey(key))
            {
                Assert.Fail(string.Format("Blob metadata doesn't contain key '{1}' blobName '{0}'", blob.Name, key));
            }

            if (blob.Metadata[key] != value)
            {
                var actualValue = blob.Metadata[key];
                Assert.Fail(string.Format("Blob metadata value doesn't match expected: '{0}' actual: '{1}', blobname: '{2}'", value, actualValue, blob.Name));
            }

            Trace.WriteLine(string.Format("Blob metadata matches blobName '{0}'", blob.Name));

            return blob;
        }

        public static CloudBlobClient ClearBlobItemsFromEmulator(this CloudBlobClient blobClient)
        {
            ExecuteCommandOnEmulator("clear blob");

            return blobClient;
        }

        public static CloudBlobClient StartEmulator(this CloudBlobClient blobClient)
        {
            ExecuteCommandOnEmulator("start");

            return blobClient;
        }

        public static CloudBlobClient StopEmulator(this CloudBlobClient blobClient)
        {
            ExecuteCommandOnEmulator("stop");

            return blobClient;
        }

        private static void ExecuteCommandOnEmulator(string arguments)
        {
            ProcessStartInfo start = new ProcessStartInfo
            {
                Arguments = arguments,
                FileName = StorageEmulatorLocation
            };
            Process proc = new Process
            {
                StartInfo = start
            };

            proc.Start();
            proc.WaitForExit();
        }

        private static string StorageEmulatorLocation = @"C:\Program Files (x86)\Microsoft SDKs\Windows Azure\Storage Emulator\WAStorageEmulator.exe";
    }


}
