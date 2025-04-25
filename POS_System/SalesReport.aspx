<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SalesReport.aspx.cs" Inherits="POS_System.SalesReport" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Raporti i Shitjeve</title>
    <link rel="stylesheet" href="style.css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h1>Raporti i Shitjeve</h1>
            
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
    </form>
</body>
</html>