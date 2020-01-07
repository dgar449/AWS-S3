using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.Configuration;

namespace AWS_S3
{
    public partial class AWS_Upload : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            AWSUploadKey.Items.Add(new ListItem(WebConfigurationManager.AppSettings["DDLValue1"]));
            AWSUploadKey.Items.Add(new ListItem(WebConfigurationManager.AppSettings["DDLValue2"]));
            AWSUploadKey.Items.Add(new ListItem(WebConfigurationManager.AppSettings["DDLValue3"]));
            AWSUploadKey.Items.Add(new ListItem(WebConfigurationManager.AppSettings["DDLValue4"]));
        }
        protected void UploadButton_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            if (AWSFileUpload.HasFiles)
            {
                string s3Bucket = WebConfigurationManager.AppSettings["AWSBucketName"];
                FileUpload fuObj = new FileUpload();
                int filecount = AWSFileUpload.PostedFiles.Count;
                
                try
                {
                    foreach (HttpPostedFile postfiles in AWSFileUpload.PostedFiles)
                    {

                        sb.AppendFormat(" Uploading file: {0}", AWSFileUpload.FileName);

                        //saving the file
                        postfiles.SaveAs(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory)) + postfiles.FileName);
                        string path = (Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory)) + postfiles.FileName);
                        //uploading to s3
                        fuObj.fUpload(path, s3Bucket, AWSUploadKey.SelectedValue + postfiles.FileName);
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
            else
            {
                UploadLabelMsg.Text = sb.ToString();
            }
        }
    }
}