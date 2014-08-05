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
    <title></title>
</head>
<body>
    <div>
    </div>
</body>
</html>
