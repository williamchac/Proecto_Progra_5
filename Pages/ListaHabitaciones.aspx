<%@ Page Title="Gestionar Habitaciones" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ListaHabitaciones.aspx.cs" Inherits="ProyectoFinalP5.Pages.ListaHabitaciones" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .table-responsive {
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            border-radius: 8px;
            overflow: hidden;
            background: white;
        }
        
        .btn-crear-habitacion {
            background-color: #febb02;
            color: #003580;
            font-weight: bold;
            border: none;
            padding: 0.75rem 2rem;
            border-radius: 8px;
            transition: all 0.3s;
        }
        
        .btn-crear-habitacion:hover {
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
        
        .estado-activa {
            background-color: #28a745;
            color: white;
        }
        
        .estado-inactiva {
            background-color: #6c757d;
            color: white;
        }
        
        .link-editar {
            color: #0071c2;
            text-decoration: none;
            font-weight: 600;
            transition: color 0.3s;
        }
        
        .link-editar:hover {
            color: #003580;
            text-decoration: underline;
        }
        
        .link-disabled {
            color: #6c757d;
            text-decoration: none;
            cursor: not-allowed;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row mb-4">
        <div class="col-md-8">
            <h2 class="page-title">
                <i class="fas fa-door-open me-2"></i>Gestionar Habitaciones
            </h2>
        </div>
        <div class="col-md-4 text-end">
            <asp:Button ID="btnNuevaHabitacion" runat="server" Text="Nueva Habitación" 
                        CssClass="btn btn-crear-habitacion" OnClick="btnNuevaHabitacion_Click" />
        </div>
    </div>
    
    <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert alert-info alert-custom mb-4" role="alert">
        <i class="fas fa-info-circle me-2"></i>
        <asp:Label ID="lblMensaje" runat="server"></asp:Label>
    </asp:Panel>
    
    <div class="table-responsive">
        <asp:GridView ID="gvHabitaciones" runat="server" CssClass="table table-zebra mb-0" 
                      AutoGenerateColumns="False" EmptyDataText="No hay habitaciones registradas."
                      GridLines="None" OnRowDataBound="gvHabitaciones_RowDataBound">
            <Columns>
                <asp:BoundField DataField="idHabitacion" HeaderText="ID">
                    <HeaderStyle CssClass="text-center fw-bold" />
                    <ItemStyle CssClass="text-center" />
                </asp:BoundField>
                
                <asp:BoundField DataField="nombreHotel" HeaderText="Hotel">
                    <HeaderStyle CssClass="text-start fw-bold" />
                    <ItemStyle CssClass="text-start" />
                </asp:BoundField>
                
                <asp:BoundField DataField="numeroHabitacion" HeaderText="Número de Habitación">
                    <HeaderStyle CssClass="text-center fw-bold" />
                    <ItemStyle CssClass="text-center" />
                </asp:BoundField>
                
                <asp:BoundField DataField="capacidadMaxima" HeaderText="Capacidad Máxima">
                    <HeaderStyle CssClass="text-center fw-bold" />
                    <ItemStyle CssClass="text-center" />
                </asp:BoundField>
                
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
                        <asp:HyperLink ID="lnkEditar" runat="server" 
                                       NavigateUrl='<%# "~/Pages/EditarHabitacion.aspx?id=" + Eval("idHabitacion") %>' 
                                       CssClass="link-editar">
                            <i class="fas fa-edit me-1"></i>Editar
                        </asp:HyperLink>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>