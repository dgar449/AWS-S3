<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AWSUpload.aspx.cs" Inherits="AWS_S3.AWS_Upload" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="StyleSheetForGridView.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-alpha.6/css/bootstrap.min.css" type="text/css" />
</head>
<body class="csforbodytag">
   <form id="form1" runat="server" class="form-text" >
   
      <div style="text-align:center">
         <h3 class="csforh3">File Upload:</h3><br /><br />
         <asp:FileUpload ID="AWSFileUpload" runat="server" AllowMultiple="true"  CssClass="csfileupld"/>
         <asp:Button ID="UploadButton" runat="server" onclick="UploadButton_Click"  Text="Upload" style="width:85px;float:left;" CssClass="btn" />
         <br /><br />
         <br />
         <asp:DropDownList ID="AWSUploadKey" runat="server" AutoPostBack="true" CssClass="dropdown-header1" style="text-align:left" OnTextChanged="OnSelectedIndexChanged" Width="100%"/>
          <br />
      </div>

      <div>
          <asp:GridView ID="ListAWSS3GridView" runat="server" AutoGenerateColumns="false" EmptyDataText = "No files uploaded" CssClass="validateGridView" Width="100%">
           <Columns>
            <asp:BoundField DataField="Text" HeaderText="File Name" />
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton ID="AWS_Download" Text = "Download" runat="server" OnClick = "DownloadFile" CssClass="linkclass"></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton ID = "AWS_Delete" Text = "Delete" runat = "server" OnClick = "DeleteFile" CssClass="linkclass" OnClientClick="return confirm('Are you sure you want to delete?')" />
                </ItemTemplate>
            </asp:TemplateField>
            </Columns>
            </asp:GridView>
       </div>
   </form>
    
       <footer>
        <p>Created by: Daryll Joseph Garcia</p>
        </footer>
</body>
</html>