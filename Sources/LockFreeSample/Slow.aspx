<%@ Page Language="C#" %>  

<script runat="server">
    protected void Page_Load(object sender, EventArgs e)  
    {
        System.Threading.Thread.Sleep(10000);
        lblLine1.Text = "Hello, SessionId " + Session.SessionID;
        lblLine2.Text = Request.Path;
    }  
</script> 

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Slow</title>
</head>
<body>
    <div><asp:Label ID="lblLine1" runat="server" /></div>
    <div><asp:Label ID="lblLine2" runat="server" /></div>
</body>
</html>