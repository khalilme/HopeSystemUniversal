<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WCFServiceWebRole.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body dir="rtl">
    <form id="form1" runat="server">
    <div>
    
        <asp:TextBox ID="TextBox1" runat="server" Height="228px" TextMode="MultiLine" Width="376px">ذهب ميسي إلى البحر
            </asp:TextBox>
         <%--   قال لي هل نهذب سويا الى الحبحر ونسحبح
وتم تقديم تقرير مقصل حول--%>
&nbsp;<asp:TextBox ID="TextBox2" runat="server" Height="228px" TextMode="MultiLine" Width="376px"></asp:TextBox>
&nbsp;<asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Button" />
    
    </div>
    </form>
</body>
</html>
