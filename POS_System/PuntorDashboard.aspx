<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PuntorDashboard.aspx.cs" Inherits="POS_System.PuntorDashboard" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Puntor Dashboard</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link rel="stylesheet" href="style.css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="container no-print">
            <div class="header">
                <h1 class="welcome-message">Mirësevini, <asp:Label ID="lblUsername" runat="server" />!</h1>
                <asp:Button ID="btnLogout" runat="server" Text="Dil" OnClick="btnLogout_Click" CssClass="btn btn-danger" />
            </div>
            
            <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert alert-success">
                <asp:Label ID="lblMessage" runat="server" />
            </asp:Panel>
            
            <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger">
                <asp:Label ID="lblError" runat="server" />
            </asp:Panel>
            
            <div class="card">
                <h3 class="card-title">Shto Produkt në Shitje</h3>
                <div class="form-group">
                    <label for="ddlProducts">Produkti:</label>
                    <asp:DropDownList ID="ddlProducts" runat="server" CssClass="form-control"></asp:DropDownList>
                </div>
                <div class="form-group">
                    <label for="txtQuantity">Sasia:</label>
                    <asp:TextBox ID="txtQuantity" runat="server" Text="1" TextMode="Number" CssClass="form-control"></asp:TextBox>
                </div>
                <asp:Button ID="btnAddToSale" runat="server" Text="Shto në Shitje" OnClick="btnAddToSale_Click" CssClass="btn btn-primary" />
            </div>
            
            <div class="card">
                <h3 class="card-title">Produktet në Shitje</h3>
                <div class="table-responsive">
                    <asp:GridView ID="gvSaleItems" runat="server" AutoGenerateColumns="False" CssClass="grid-view" 
                        EmptyDataText="Nuk ka produkte të shtuara në shitje">
                        <Columns>
                            <asp:BoundField DataField="ProductId" HeaderText="ID" />
                            <asp:BoundField DataField="Name" HeaderText="Emri" />
                            <asp:BoundField DataField="Quantity" HeaderText="Sasia" />
                            <asp:BoundField DataField="Price" HeaderText="Çmimi (€)" DataFormatString="{0:N2}" />
                            <asp:BoundField DataField="Total" HeaderText="Totali (€)" DataFormatString="{0:N2}" />
                        </Columns>
                    </asp:GridView>
                </div>
                
                <div class="sale-summary">
                    <h4 class="total-amount">Totali i Shitjes: <asp:Label ID="lblTotal" runat="server" Text="0.00"></asp:Label> €</h4>
                    <asp:Button ID="btnCompleteSale" runat="server" Text="Kryej Shitjen" OnClick="btnCompleteSale_Click" CssClass="btn btn-success" />
                    <asp:Button ID="btnPrintInvoice" runat="server" Text="Printo Faturën" OnClick="btnPrintInvoice_Click" CssClass="btn btn-primary" style="margin-left: 10px;" />
                </div>
            </div>
        </div>

        <!-- Seksioni për printim -->
        <div id="printSection" runat="server" class="print-section">
            <h2>Faturë #<asp:Label ID="lblInvoiceNumber" runat="server"></asp:Label></h2>
            <p>Data: <asp:Label ID="lblInvoiceDate" runat="server"></asp:Label></p>
            <p>Puntor: <asp:Label ID="lblCashierName" runat="server"></asp:Label></p>
            
            <asp:GridView ID="gvInvoiceItems" runat="server" AutoGenerateColumns="False" Width="100%">
                <Columns>
                    <asp:BoundField DataField="Name" HeaderText="Produkti" />
                    <asp:BoundField DataField="Quantity" HeaderText="Sasia" />
                    <asp:BoundField DataField="Price" HeaderText="Çmimi" DataFormatString="{0:N2} €" />
                    <asp:BoundField DataField="Total" HeaderText="Totali" DataFormatString="{0:N2} €" />
                </Columns>
            </asp:GridView>
            
            <h3>Totali: <asp:Label ID="lblInvoiceTotal" runat="server"></asp:Label> €</h3>
            <p>Faleminderit për blerjen!</p>
        </div>

        <script type="text/javascript">
            function printInvoice() {
                var printContent = document.getElementById('<%= printSection.ClientID %>');
                var windowUrl = 'about:blank';
                var uniqueName = new Date();
                var windowName = 'Print_' + uniqueName.getTime();

                var printWindow = window.open(windowUrl, windowName, 'left=100,top=100,width=800,height=600');

                printWindow.document.write('<html><head><title>Fatura</title>');
                printWindow.document.write('<style>body { font-family: Arial; } table { width: 100%; border-collapse: collapse; } th, td { border: 1px solid #ddd; padding: 8px; text-align: left; } h2 { color: #333; }</style>');
                printWindow.document.write('</head><body>');
                printWindow.document.write(printContent.innerHTML);
                printWindow.document.write('</body></html>');
                printWindow.document.close();
                printWindow.focus();

                setTimeout(function () {
                    printWindow.print();
                    printWindow.close();
                }, 500);
            }
        </script>
    </form>
</body>
</html>