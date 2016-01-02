<%@ Page Language="C#" CodeBehind="SessionEditor.aspx.cs" Inherits="SossSample.SessionEditor" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Session Editor</title>
</head>
<body>
<form runat="server">
    <div>
        <asp:BulletedList ID="SessionList" runat="server">
        </asp:BulletedList>

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
