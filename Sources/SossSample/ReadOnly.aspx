<%@ Page Language="C#" AutoEventWireup="true" EnableSessionState="ReadOnly" %>

<script runat="server">
    protected void Page_Load(object sender, EventArgs e)  
    {
        Session["ReadOnlyTest"] = "Only Test";

        if (Session["ReadOnlyTest"] != null)
            TestLabel.Text = Session["ReadOnlyTest"].ToString();
        else
            TestLabel.Text = "null";
    }  
</script> 

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Slow</title>
</head>
<body>
    <div>Session[&quot;ReadOnlyTest&quot;] = <asp:Label ID="TestLabel" runat="server" /></div>
</body>
</html>