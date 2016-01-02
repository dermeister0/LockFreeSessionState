<%@ Page Language="C#" AutoEventWireup="true" EnableSessionState="false" %>

<script runat="server">
    protected void Page_Load(object sender, EventArgs e)  
    {
        Session["NoSession"] = "Only Test";

        if (Session["NoSession"] != null)
            TestLabel.Text = Session["NoSession"].ToString();
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
    <div>Session[&quot;NoSession&quot;] = <asp:Label ID="TestLabel" runat="server" /></div>
</body>
</html>