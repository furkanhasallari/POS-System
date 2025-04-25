<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Products.aspx.cs" Inherits="POS_System.Products" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Menaxhimi i Produkteve</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link rel="stylesheet" href="style.css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h1>Menaxhimi i Produkteve</h1>
            
            <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert alert-success">
                <asp:Label ID="lblMessage" runat="server" />
            </asp:Panel>
            
            <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger">
                <asp:Label ID="lblError" runat="server" />
            </asp:Panel>
            
            <div class="form-group">
                <asp:TextBox ID="txtName" runat="server" CssClass="form-control" Placeholder="Emri i Produktit" />
            </div>
            
            <div class="form-group">
                <asp:TextBox ID="txtPrice" runat="server" CssClass="form-control" Placeholder="Çmimi" TextMode="Number" step="0.01" />
            </div>
            
            <div class="form-group">
                <asp:TextBox ID="txtQuantity" runat="server" CssClass="form-control" Placeholder="Sasia" TextMode="Number" />
            </div>
            
            <asp:Button ID="btnAdd" runat="server" Text="Shto Produkt" OnClick="btnAdd_Click" CssClass="btn btn-primary" />
            
            <asp:GridView ID="gvProducts" runat="server" AutoGenerateColumns="False" CssClass="grid-view"
                OnRowEditing="gvProducts_RowEditing" OnRowUpdating="gvProducts_RowUpdating" 
                OnRowCancelingEdit="gvProducts_RowCancelingEdit" OnRowDeleting="gvProducts_RowDeleting"
                DataKeyNames="Id" EmptyDataText="Nuk ka produkte të regjistruara">
                <Columns>
                    <asp:BoundField DataField="Id" HeaderText="ID" ReadOnly="True" />
                    <asp:BoundField DataField="Name" HeaderText="Emri" />
                    <asp:BoundField DataField="Price" HeaderText="Çmimi" DataFormatString="{0:N2}" />
                    <asp:BoundField DataField="Quantity" HeaderText="Sasia" />
                    <asp:CommandField ShowEditButton="True" ShowDeleteButton="True" 
                        EditText="Edito" UpdateText="Përditëso" CancelText="Anulo" DeleteText="Fshij"
                        ControlStyle-CssClass="btn" />
                </Columns>
            </asp:GridView>
        </div>
    </form>
</body>
</html>