<%@ Page Language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">
    void Page_Load()
    {
        var value = Session["Test"];
        lblResult.Text = value == null ? "OK" : "FAIL";
    }
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Title</title>
</head>
<body>
<form id="HtmlForm" runat="server">
    <div>
        <asp:Label ID="lblResult" runat="server"></asp:Label>
    </div>
</form>
</body>
</html>
