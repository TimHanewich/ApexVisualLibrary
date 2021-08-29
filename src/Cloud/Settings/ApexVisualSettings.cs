using System;
using ApexVisual.Cloud;
using System.Threading.Tasks;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage;
using Newtonsoft.Json;

namespace ApexVisual.Cloud.Settings
{
    public class ApexVisualSettings
    {
        public bool UserAccountsDisabled {get; set;} //Both user login and user registration will be disabled if this is turned to true.

        //Retrieving from cloud
        public static async Task<ApexVisualSettings> DownloadAsync(ApexVisualManager avm)
        {
            CloudStorageAccount csa;
            CloudStorageAccount.TryParse(avm.AzureStorageConnectionString, out csa);
            CloudBlobClient cbc = csa.CreateCloudBlobClient();
            CloudBlobContainer cont = cbc.GetContainerReference("general");
            await cont.CreateIfNotExistsAsync();
            CloudBlockBlob blb = cont.GetBlockBlobReference("settings");
            if (blb.Exists())
            {
                string content = await blb.DownloadTextAsync();
                ApexVisualSettings settings = JsonConvert.DeserializeObject<ApexVisualSettings>(content);
                return settings;
            }
            else
            {
                ApexVisualSettings settings = new ApexVisualSettings();
                await settings.UploadAsync(avm); //Upload it to the cloud.
                return settings;
            }
        }

        //Saving to cloud
        public async Task UploadAsync(ApexVisualManager avm)
        {
            CloudStorageAccount csa;
            CloudStorageAccount.TryParse(avm.AzureStorageConnectionString, out csa);
            CloudBlobClient cbc = csa.CreateCloudBlobClient();
            CloudBlobContainer cont = cbc.GetContainerReference("general");
            await cont.CreateIfNotExistsAsync();
            CloudBlockBlob blb = cont.GetBlockBlobReference("settings");
            await blb.UploadTextAsync(JsonConvert.SerializeObject(this));
        }
    }
}