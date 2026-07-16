<%@ Page Title="Mis Reservaciones" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MisReservaciones.aspx.cs" Inherits="ProyectoFinalP5.Pages.MisReservaciones" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .table-responsive {
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            border-radius: 8px;
            overflow: hidden;
            background: white;
        }
        
        .btn-crear-reservacion {
            background-color: #febb02;
            color: #003580;
            font-weight: bold;
            border: none;
            padding: 0.75rem 2rem;
            border-radius: 8px;
            transition: all 0.3s;
        }
        
        .btn-crear-reservacion:hover {
            background-color: #e5a800;
            transform: translateY(-2px);
            box-shadow: 0 4px 8px rgba(0,0,0,0.2);
        }
        
        .estado-badge {
            padding: 0.375rem 0.75rem;
            border-radius: 20px;
            font-weight: 600;
            font-size: 0.875rem;
        }
        
        .estado-cancelada {
            background-color: #dc3545;
            color: white;
        }
        
        .estado-finalizada {
            background-color: #6c757d;
            color: white;
        }
        
        .estado-proceso {
            background-color: #28a745;
            color: white;
        }
        
        .estado-espera {
            background-color: #0071c2;
            color: white;
        }
        
        .link-detalle {
            color: #0071c2;
            text-decoration: none;
            font-weight: 600;
            transition: color 0.3s;
        }
        
        .link-detalle:hover {
            color: #003580;
            text-decoration: underline;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row mb-4">
        <div class="col-md-8">
            <h2 class="page-title">
                <i class="fas fa-calendar-check me-2"></i>Mis Reservaciones
            </h2>
        </div>
        <div class="col-md-4 text-end">
            <asp:Button ID="btnNuevaReservacion" runat="server" Text="Nueva Reservación" 
                        CssClass="btn btn-crear-reservacion" OnClick="btnNuevaReservacion_Click" />
        </div>
    </div>
    
    <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert alert-info alert-custom mb-4" role="alert">
        <i class="fas fa-info-circle me-2"></i>
        <asp:Label ID="lblMensaje" runat="server"></asp:Label>
    </asp:Panel>
    
    <div class="table-responsive">
        <asp:GridView ID="gvReservaciones" runat="server" CssClass="table table-zebra mb-0" 
                      AutoGenerateColumns="False" EmptyDataText="No hay reservaciones registradas."
                      GridLines="None" OnRowDataBound="gvReservaciones_RowDataBound">
            <Columns>
                <asp:BoundField DataField="idReservacion" HeaderText="# Reservación">
                    <HeaderStyle CssClass="text-center fw-bold" />
                    <ItemStyle CssClass="text-center" />
                </asp:BoundField>
                
                <asp:BoundField DataField="nombreHotel" HeaderText="Hotel">
                    <HeaderStyle CssClass="text-start fw-bold" />
                    <ItemStyle CssClass="text-start" />
                </asp:BoundField>
                
                <asp:BoundField DataField="fechaEntrada" HeaderText="Fecha Entrada" DataFormatString="{0:dd/MM/yyyy}">
                    <HeaderStyle CssClass="text-center fw-bold" />
                    <ItemStyle CssClass="text-center" />
                </asp:BoundField>
                
                <asp:BoundField DataField="fechaSalida" HeaderText="Fecha Salida" DataFormatString="{0:dd/MM/yyyy}">
                    <HeaderStyle CssClass="text-center fw-bold" />
                    <ItemStyle CssClass="text-center" />
                </asp:BoundField>
                
                <asp:TemplateField HeaderText="Costo">
                    <HeaderStyle CssClass="text-end fw-bold" />
                    <ItemStyle CssClass="text-end" />
                    <ItemTemplate>
                        <%# String.Format("${0:N2}", Eval("costoTotal")) %>
                    </ItemTemplate>
                </asp:TemplateField>
                
                <asp:TemplateField HeaderText="Estado">
                    <HeaderStyle CssClass="text-center fw-bold" />
                    <ItemStyle CssClass="text-center" />
                    <ItemTemplate>
                        <asp:Label ID="lblEstado" runat="server" CssClass="estado-badge"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                
                <asp:TemplateField HeaderText="Acciones">
                    <HeaderStyle CssClass="text-center fw-bold" />
                    <ItemStyle CssClass="text-center" />
                    <ItemTemplate>
                        <asp:HyperLink ID="lnkDetalle" runat="server" 
                                       NavigateUrl='<%# "~/Pages/DetalleReservacion.aspx?id=" + Eval("idReservacion") %>' 
                                       CssClass="link-detalle">
                            <i class="fas fa-eye me-1"></i>Ver Detalle
                        </asp:HyperLink>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>