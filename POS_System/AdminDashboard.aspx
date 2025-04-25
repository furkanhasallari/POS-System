<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminDashboard.aspx.cs" Inherits="POS_System.AdminDashboard" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Admin Dashboard</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link rel="stylesheet" href="style.css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="header">
                <h1 class="welcome-message">Mirësevini, <asp:Label ID="lblUsername" runat="server" />!</h1>
                <asp:Button ID="btnLogout" runat="server" Text="Dil" OnClick="btnLogout_Click" CssClass="btn btn-danger" />
            </div>
            
            <div class="admin-nav">
                <asp:Button ID="btnProducts" runat="server" Text="Menaxho Produktet" 
                    OnClick="btnProducts_Click" CssClass="nav-btn" />
                <asp:Button ID="btnSalesReport" runat="server" Text="Raportet e Shitjeve" 
                    OnClick="btnSalesReport_Click" CssClass="nav-btn" />
            </div>
            
            <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert alert-success">
                <asp:Label ID="lblMessage" runat="server" />
            </asp:Panel>
            
            <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger">
                <asp:Label ID="lblError" runat="server" />
            </asp:Panel>
            
            <!-- Products Section -->
            <asp:Panel ID="productsSection" runat="server">
                <div class="card">
                    <h3 class="card-title">Shto ose Përditëso Produkt</h3>
                    <div class="form-group">
                        <label for="txtName">Emri:</label>
                        <asp:TextBox ID="txtName" runat="server" CssClass="form-control" />
                    </div>
                    <div class="form-group">
                        <label for="txtPrice">Çmimi (€):</label>
                        <asp:TextBox ID="txtPrice" runat="server" CssClass="form-control" TextMode="Number" step="0.01" />
                    </div>
                    <div class="form-group">
                        <label for="txtQuantity">Sasia:</label>
                        <asp:TextBox ID="txtQuantity" runat="server" CssClass="form-control" TextMode="Number" />
                    </div>
                    <asp:HiddenField ID="hfProductId" runat="server" />
                    <asp:Button ID="btnSave" runat="server" Text="Ruaj Produkt" OnClick="btnSave_Click" CssClass="btn btn-success" />
                </div>
                
                <div class="card">
                    <h3 class="card-title">Lista e Produkteve</h3>
                    <div class="table-responsive">
                        <asp:GridView ID="gvProducts" runat="server" AutoGenerateColumns="False" CssClass="grid-view" 
                            OnRowCommand="gvProducts_RowCommand" DataKeyNames="Id" EmptyDataText="Nuk ka produkte të regjistruara">
                            <Columns>
                                <asp:BoundField DataField="Id" HeaderText="ID" />
                                <asp:BoundField DataField="Name" HeaderText="Emri" />
                                <asp:BoundField DataField="Price" HeaderText="Çmimi (€)" DataFormatString="{0:N2}" />
                                <asp:BoundField DataField="Quantity" HeaderText="Sasia" />
                                <asp:TemplateField HeaderText="Veprime">
                                    <ItemTemplate>
                                        <asp:Button ID="btnEdit" runat="server" Text="Edito" CommandName="EditProduct" 
                                            CommandArgument='<%# Container.DataItemIndex %>' CssClass="btn btn-primary" />
                                        <asp:Button ID="btnDelete" runat="server" Text="Fshij" CommandName="DeleteProduct" 
                                            CommandArgument='<%# Container.DataItemIndex %>' CssClass="btn btn-danger" 
                                            OnClientClick="return confirm('A jeni i sigurt që dëshironi të fshini këtë produkt?');" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </asp:Panel>
            
            <!-- Sales Report Section -->
            <asp:Panel ID="salesReportSection" runat="server" Visible="false">
                <div class="card">
                    <h3 class="card-title">Raportet e Shitjeve</h3>
                    <div class="filters">
                        <div class="form-group">
                            <label>Data nga:</label>
                            <asp:TextBox ID="txtFromDate" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label>Data deri:</label>
                            <asp:TextBox ID="txtToDate" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                        </div>
                        <asp:Button ID="btnFilter" runat="server" Text="Filtro" OnClick="btnFilter_Click" CssClass="btn btn-primary" />
                    </div>

                    <asp:GridView ID="gvSales" runat="server" AutoGenerateColumns="False" CssClass="grid-view"
                        EmptyDataText="Nuk u gjetën shitje për këto data">
                        <Columns>
                            <asp:BoundField DataField="SaleID" HeaderText="ID Shitje" />
                            <asp:BoundField DataField="Date" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                            <asp:BoundField DataField="TotalAmount" HeaderText="Shuma (€)" DataFormatString="{0:N2}" />
                            <asp:TemplateField HeaderText="Detaje">
                                <ItemTemplate>
                                    <asp:Button ID="btnDetails" runat="server" Text="Shiko Detaje" 
                                        CommandArgument='<%# Eval("SaleID") %>' OnClick="btnDetails_Click" CssClass="btn btn-primary" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>

                    <div class="summary">
                        <asp:Label ID="lblTotalSales" runat="server" Text="Totali i shitjeve: 0.00 €"></asp:Label>
                    </div>

                    <asp:Panel ID="pnlDetails" runat="server" Visible="false" style="margin-top:30px;">
                        <h3>Detajet e Shitjes #<asp:Label ID="lblSaleId" runat="server"></asp:Label></h3>
                        <asp:GridView ID="gvSaleDetails" runat="server" AutoGenerateColumns="False" CssClass="grid-view">
                            <Columns>
                                <asp:BoundField DataField="ProductID" HeaderText="ID Produkt" />
                                <asp:BoundField DataField="Name" HeaderText="Emri Produktit" />
                                <asp:BoundField DataField="Quantity" HeaderText="Sasia" />
                                <asp:BoundField DataField="Price" HeaderText="Çmimi (€)" DataFormatString="{0:N2}" />
                                <asp:BoundField DataField="Total" HeaderText="Totali (€)" DataFormatString="{0:N2}" />
                            </Columns>
                        </asp:GridView>
                    </asp:Panel>
                </div>
            </asp:Panel>
        </div>
    </form>
</body>
</html>