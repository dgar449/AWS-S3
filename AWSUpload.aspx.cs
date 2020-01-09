using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.Configuration;
using Amazon.S3;
using Amazon.S3.IO;
using Amazon.S3.Model;
using Amazon;
using Amazon.S3.Transfer;
using System.Net;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Configuration;

namespace AWS_S3
{
    public partial class AWS_Upload : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int i = 1;
                //Dynamically add new S3 keys
                while (WebConfigurationManager.AppSettings["DDLValue" + i] != null)
                {
                    AWSUploadKey.Items.Add(new ListItem(WebConfigurationManager.AppSettings["DDLValue" + i]));
                    i++;
                }
            }
        }
        protected void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            string region = ConfigurationManager.AppSettings["AWSRegion"].ToString();
            List<ListItem> files = new List<ListItem>();
            AmazonS3Client client = new AmazonS3Client(Amazon.RegionEndpoint.GetBySystemName(region));
            //s3 s = new Amazon.S3.IO.S3FileInfo()
            ListObjectsRequest request = new ListObjectsRequest
            {
                BucketName = WebConfigurationManager.AppSettings["AWSBucketName"],
                Prefix = AWSUploadKey.SelectedValue,
                Delimiter = ""
            };
            ListObjectsResponse listResponse;
            do
            {
                // Get a list of objects
                listResponse = client.ListObjects(request);

                //List<ListItem> files = new List<ListItem>();
                foreach (S3Object obj in listResponse.S3Objects)
                {
                    //To only list the files from the last part of the directory
                    if (AWSUploadKey.SelectedValue.LastIndexOf('/') == obj.Key.LastIndexOf('/'))
                        files.Add(new ListItem(obj.Key));
                }
                //To refresh the grid
                ListAWSS3GridView.DataSource = files;
                ListAWSS3GridView.DataBind();

                // Set the marker property
                request.Marker = listResponse.NextMarker;
            } while (listResponse.IsTruncated);
        }
        protected void DownloadFile(object sender, EventArgs e)
        {
            int rindex = (((GridViewRow)(((Control)(sender)).Parent.BindingContainer))).RowIndex;
            string filePath = ListAWSS3GridView.Rows[rindex].Cells[0].Text;
            string s3path = "https://" + WebConfigurationManager.AppSettings["AWSBucketName"] + ".s3.amazonaws.com/";
            string fileName = filePath.Substring(filePath.LastIndexOf("/") + 1, filePath.Length - filePath.LastIndexOf("/") - 1);
            string downloadPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory))+"\\S3Download\\";
            if(!Directory.Exists(downloadPath))
            {
                Directory.CreateDirectory(downloadPath);
            }
            WebClient webClient = new WebClient();
            //To download the file to the local system path
            webClient.DownloadFile(new Uri(s3path + filePath), downloadPath + fileName);
            webClient.Dispose();        //Freeing up memory to avoid exception errors
            webClient = null;

            //To Get the physical Path of the file
            string filepath = Server.MapPath("\\S3Download\\"+fileName);

            // Create New instance of FileInfo class to get the properties of the file being downloaded
            FileInfo myfile = new FileInfo(filepath);

            // Checking if file exists
            try
            {
                if (myfile.Exists)
                {
                    // Clear the content of the response
                    Response.ClearContent();

                    // Add the file name and attachment, which will force the open/cancel/save dialog box to show, to the header
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);

                    // Add the file size into the response header
                    Response.AddHeader("Content-Length", myfile.Length.ToString());

                    // Write the file into the response 
                    Response.TransmitFile(myfile.FullName);

                    // End the response
                    Response.Flush();

                }
            }
            finally
            {
                //To delete the local file
                File.Delete(downloadPath + fileName);
            }
        }
        protected void DeleteFile(object sender, EventArgs e)
        {
            //Get the index of the row to delete from
            int rindex = (((GridViewRow)(((Control)(sender)).Parent.BindingContainer))).RowIndex;
            //Get the name of the file to delete
            string fileName = ListAWSS3GridView.Rows[rindex].Cells[0].Text;

            Amazon.S3.AmazonS3Client client = new Amazon.S3.AmazonS3Client(
                WebConfigurationManager.AppSettings["AWSAccessKey"],
            WebConfigurationManager.AppSettings["AWSSecretKey"]);
            // To delete the file based on the file key
            client.DeleteObject(new Amazon.S3.Model.DeleteObjectRequest()
            {
                BucketName = WebConfigurationManager.AppSettings["AWSBucketName"], 
                Key = fileName });
            //Generate successful deletion alert
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage",
                            "alert('Object Successfully Deleted!!')", true);
            //Refresh grid
            OnSelectedIndexChanged(sender, e);
        }

        protected void UploadButton_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            if (AWSFileUpload.HasFiles)
            {
                string s3Bucket = WebConfigurationManager.AppSettings["AWSBucketName"];
                //Creating an object of the FileUpload.cs class
                FileUpload fuObj = new FileUpload();
                int filecount = AWSFileUpload.PostedFiles.Count;
                
                try
                {
                    foreach (HttpPostedFile postfiles in AWSFileUpload.PostedFiles)
                    {

                        sb.AppendFormat(" Uploading file: {0}", AWSFileUpload.FileName);

                        //saving the file
                        postfiles.SaveAs(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory)) + postfiles.FileName);
                        //Get the local path of the file
                        string path = (Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory)) + postfiles.FileName);
                        //uploading to s3
                        fuObj.fUpload(path, s3Bucket, AWSUploadKey.SelectedValue + postfiles.FileName);
                        //alter for successful upload
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", 
                            "alert('Object Uploaded Successfully!!')", true);
                        //Refresh grid
                        OnSelectedIndexChanged(sender, e);
                        //deleting the saved file
                        File.Delete(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory)) + postfiles.FileName);
                    }
                }
                catch (Exception ex)
                {

                    sb.Append("<br/> Error <br/>");
                    sb.AppendFormat("Unable to save file <br/> {0}", ex.Message);
                    throw ex;
                }
            }
        }
    }
}