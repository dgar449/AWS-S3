<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AWS Upload.aspx.cs" Inherits="AWS_S3.AWS_Upload" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
   <form id="form1" runat="server">
   
      <div>
         <h3> File Upload:</h3>
         <br />
         <asp:FileUpload ID="AWSFileUpload" runat="server" AllowMultiple="true" />
         <br /><br />
         <asp:Button ID="UploadButton" runat="server" onclick="UploadButton_Click"  Text="Upload" style="width:85px" />
         <br /><br />
         <asp:Label ID="UploadLabelMsg" runat="server" />
         <br /><br />
         <asp:DropDownList ID="AWSUploadKey" runat="server" />
      </div>
      
   </form>
</body>
</html>
