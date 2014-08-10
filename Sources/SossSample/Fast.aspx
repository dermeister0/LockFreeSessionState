<%@ Page Language="C#" %>  

<script runat="server">
    protected void Page_Load(object sender, EventArgs e)  
    {
        System.Threading.Thread.Sleep(1000);
        
        lblLine1.Text = "Hello, SessionId " + Session.SessionID;
        lblLine2.Text = Request.Path;

        if (Session["Test"] != null)
            lblLine3.Text = Session["Test"].ToString();
        else
            lblLine3.Text = "null";
    }  
</script> 

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Fast</title>
</head>
<body>
    <div><asp:Label ID="lblLine1" runat="server" /></div>
    <div><asp:Label ID="lblLine2" runat="server" /></div>
    <div>Session[&quot;Test&quot;] = <asp:Label ID="lblLine3" runat="server" /></div>
</body>
</html>