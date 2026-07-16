<%@ Page Title="Mensaje" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Mensaje.aspx.cs" Inherits="ProyectoFinalP5.Pages.Mensaje" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .mensaje-container {
            max-width: 600px;
            margin: 4rem auto;
            text-align: center;
        }
        
        .mensaje-icono {
            font-size: 5rem;
            margin-bottom: 2rem;
        }
        
        .icono-exito {
            color: #28a745;
        }
        
        .icono-error {
            color: #dc3545;
        }
        
        .icono-info {
            color: #0071c2;
        }
        
        .mensaje-panel {
            background: white;
            padding: 3rem;
            border-radius: 8px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.15);
        }
        
        .mensaje-texto {
            font-size: 1.25rem;
            color: #333;
            margin-bottom: 2rem;
        }
        
        .btn-regresar {
            background-color: #0071c2;
            color: white;
            border: none;
            padding: 1rem 3rem;
            border-radius: 8px;
            font-weight: 600;
            font-size: 1.1rem;
            transition: all 0.3s;
            text-decoration: none;
            display: inline-block;
        }
        
        .btn-regresar:hover {
            background-color: #003580;
            transform: translateY(-2px);
            box-shadow: 0 4px 8px rgba(0,0,0,0.2);
            color: white;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="mensaje-container">
        <div class="mensaje-panel">
            <div class="mensaje-icono">
                <asp:Label ID="lblIcono" runat="server"></asp:Label>
            </div>
            
            <div class="mensaje-texto">
                <asp:Label ID="lblMensaje" runat="server"></asp:Label>
            </div>
            
            <asp:HyperLink ID="lnkRegresar" runat="server" CssClass="btn-regresar">
                <i class="fas fa-arrow-left me-2"></i>Regresar
            </asp:HyperLink>
        </div>
    </div>
</asp:Content>