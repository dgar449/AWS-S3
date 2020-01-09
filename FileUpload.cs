using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Amazon.S3;
using System.Configuration;

namespace AWS_S3
{
    public class FileUpload
    {
        public string fUpload(string filePath, string s3Bucket, string newFileName)
        {
            try
            {
                string region = ConfigurationManager.AppSettings["AWSRegion"].ToString();
                TransferUtility fileTransferUtility = new
                    TransferUtility(new AmazonS3Client(Amazon.RegionEndpoint.GetBySystemName(region)));

                //Specify object key name(newFileName) to upload in that path.
                fileTransferUtility.Upload(filePath,
                                          s3Bucket, newFileName);
                System.Diagnostics.Debug.WriteLine("Upload completed!");

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                return newFileName;
            }
            catch (AmazonS3Exception s3Exception)
            {
                throw s3Exception;
            }
        }
    }
}