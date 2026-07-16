<%@ Page Title="Gestionar Habitaciones" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GestionarHabitaciones.aspx.cs" Inherits="ProyectoFinalP5.Pages.GestionarHabitaciones" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .table-zebra tbody tr:nth-child(odd){background:#f8f9fa}
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="d-flex justify-content-between align-items-center mb-3">
        <h2 class="page-title"><i class="fas fa-door-open me-2"></i>Gestionar Habitaciones</h2>
        <asp:Button ID="btnCrear" runat="server" CssClass="btn btn-primary" Text="Crear Habitación" OnClick="btnCrear_Click" />
    </div>

    <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert alert-info">
        <asp:Label ID="lblMensaje" runat="server"></asp:Label>
    </asp:Panel>

    <div class="table-responsive">
        <asp:GridView ID="gvHabitaciones" runat="server" CssClass="table table-zebra" AutoGenerateColumns="false">
            <Columns>
                <asp:BoundField DataField="idHabitacion" HeaderText="ID" />
                <asp:BoundField DataField="nombreHotel" HeaderText="Hotel" />
                <asp:BoundField DataField="numeroHabitacion" HeaderText="Número" />
                <asp:BoundField DataField="capacidadMaxima" HeaderText="Capacidad" />
                <asp:BoundField DataField="estado" HeaderText="Estado" />
                <asp:TemplateField HeaderText="Acciones">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkEditar" runat="server" Text="Editar" CommandArgument='<%# Eval("idHabitacion") %>' OnClick="lnkEditar_Click" />
                        &nbsp;|&nbsp;
                        <asp:LinkButton ID="lnkInactivar" runat="server" Text="Inactivar" CommandArgument='<%# Eval("idHabitacion") %>' OnClick="lnkInactivar_Click" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>