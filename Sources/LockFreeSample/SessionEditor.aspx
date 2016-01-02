<%@ Page Language="C#" AutoEventWireup="true" EnableViewState="false" %>

<script runat="server">
    protected void Page_PreRender(object sender, EventArgs e)
    {
        var items = (from string key in Session.Keys select new KeyValuePair<string, string>(key, Session[key].ToString())).ToList();

        SessionGridView.DataSource = items;
        SessionGridView.DataBind();
    }

    protected void SubmitButton_OnClick(object sender, EventArgs e)
    {
        var name = NameTextBox.Text;
        var value = ValueTextBox.Text;

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(value))
            return;

        Session[name] = value;
    }
</script>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Session Editor</title>
</head>
<body>
<form runat="server">
    <div>
        <asp:GridView ID="SessionGridView" AutoGenerateColumns="True" runat="server"/>
        
        <label>Name:</label>
        <asp:TextBox ID="NameTextBox" runat="server"></asp:TextBox>
        <label>Value:</label>
        <asp:TextBox ID="ValueTextBox" runat="server"></asp:TextBox>

        <asp:Button Text="Submit" OnClick="SubmitButton_OnClick" runat="server"/>
    </div>
</form>
</body>
</html>
