<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="POS_System.Login" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login - POS System</title>
    <link rel="stylesheet" href="style.css" />
</head>
<body class="login-page">
    <form id="form1" runat="server">
        <div class="login-container">
            <h2>Hyr në POS System</h2>

            <div class="form-group">
                <label for="txtUsername">Username:</label>
                <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" />
            </div>

            <div class="form-group">
                <label for="txtPassword">Password:</label>
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" />
            </div>

            <div class="form-group">
                <asp:Button ID="btnLogin" runat="server" Text="Hyr" OnClick="btnLogin_Click" CssClass="btn btn-primary" />
            </div>

            <asp:Label ID="lblError" runat="server" CssClass="alert alert-danger" Visible="false"></asp:Label>
        </div>
    </form>
</body>
</html>