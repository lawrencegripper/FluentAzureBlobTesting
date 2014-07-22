using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace FluentAzureBlobTesting
{
    [TestClass]
    public class ExampleIntegrationTests
    {
        private static CloudStorageAccount account;
        private static CloudBlobClient blobClient;

        [ClassInitialize]
        public static void StartAndCleanStorage(TestContext cont)
        {
            account = CloudStorageAccount.DevelopmentStorageAccount;
            blobClient = account.CreateCloudBlobClient();
            blobClient.StartEmulator();
        }

        [ClassCleanup]
        public static void ShutdownStorage()
        {
            blobClient.StopEmulator();
        }

        [TestInitialize]
        public void CleanAndRestartStorage()
        {
            blobClient.ClearBlobItemsFromEmulator();
        }

        [TestMethod]
        public void ExampleTest()
        {
            var expectedContainerName = "sample";
            var expectedBlobName = "test";
            var expectedBlobData = new byte[1];

            Action ExampleMethodUnderTest = new Action(() =>
            {
                CloudBlobContainer container =
                    blobClient.GetContainerReference(expectedContainerName);

                container.CreateIfNotExists();

                CloudBlockBlob blob =
                    container.GetBlockBlobReference(expectedBlobName);

                blob.UploadFromByteArray(expectedBlobData, 0, expectedBlobData.Length);
            });

            ExampleMethodUnderTest.Invoke();

            blobClient
                .AssertContainerExists(expectedContainerName)
                .AssertBlobExists(expectedBlobName)
                .AssertBlobMatchesData(expectedBlobData);
        }
    }
}
