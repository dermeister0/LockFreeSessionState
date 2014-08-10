<%@ Page Language="C#" AutoEventWireup="true" %>

<script runat="server">   
    void Page_Load()
    {
        // Create a cookie.
        Session["Test"] = "value";
    }
</script>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Lock-free sample</title>
</head>
<body>
    <div>SLOW</div>
    <iframe width="400" height="100" src="Slow.aspx"></iframe>

    <div>FAST</div>
    <iframe width="400" height="100" src="Fast.aspx"></iframe>
</body>
</html>
