<%@ Page Language="C#" AutoEventWireup="true" %>

<script runat="server">   
    void Page_Load()
    {
        // Create a cookie.
        Session["Test"] = "Default.aspx";
        
        if (Session["Test"] != null)
            lblLine1.Text = Session["Test"].ToString();
        else
            lblLine1.Text = "null";
    }
</script>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Lock-free sample</title>
</head>
<body>
    <div>Session[&quot;Test&quot;] = <asp:Label ID="lblLine1" runat="server" /></div>

    <div>SLOW</div>
    <iframe width="400" height="100" src="Slow.aspx"></iframe>

    <div>FAST</div>
    <iframe width="400" height="100" src="Fast.aspx"></iframe>
</body>
</html>
