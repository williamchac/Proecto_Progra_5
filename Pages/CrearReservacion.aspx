<%@ Page Title="Crear Reservación" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CrearReservacion.aspx.cs" Inherits="ProyectoFinalP5.Pages.CrearReservacion" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .form-panel {
            background: white;
            padding: 2rem;
            border-radius: 8px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
        }
        
        .btn-guardar {
            background-color: #28a745;
            color: white;
            border: none;
            padding: 0.75rem 2rem;
            border-radius: 4px;
            font-weight: 600;
            transition: all 0.3s;
        }
        
        .btn-guardar:hover {
            background-color: #218838;
            transform: translateY(-2px);
        }
        
        .btn-cancelar {
            background-color: #6c757d;
            color: white;
            border: none;
            padding: 0.75rem 2rem;
            border-radius: 4px;
            font-weight: 600;
            transition: all 0.3s;
        }
        
        .btn-cancelar:hover {
            background-color: #5a6268;
        }
        
        .campo-requerido {
            color: #dc3545;
            font-weight: bold;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="page-title">
        <i class="fas fa-plus-circle me-2"></i>Crear Nueva Reservación
    </h2>
    
    <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert alert-danger alert-custom mb-4" role="alert">
        <i class="fas fa-exclamation-triangle me-2"></i>
        <asp:Label ID="lblMensaje" runat="server"></asp:Label>
    </asp:Panel>
    
    <div class="form-panel">
        <div class="row g-3">
            <div class="col-md-6">
                <label for="ddlHotel" class="form-label">
                    Hotel <span class="campo-requerido">*</span>
                </label>
                <asp:DropDownList ID="ddlHotel" runat="server" CssClass="form-select">
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="rfvHotel" runat="server" 
                                            ControlToValidate="ddlHotel" 
                                            InitialValue=""
                                            ErrorMessage="Debe seleccionar un hotel" 
                                            CssClass="text-danger small" 
                                            Display="Dynamic">
                </asp:RequiredFieldValidator>
            </div>
            
            <div class="col-md-6">
                <label for="ddlCliente" class="form-label">
                    Cliente <span class="campo-requerido">*</span>
                </label>
                <asp:DropDownList ID="ddlCliente" runat="server" CssClass="form-select">
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="rfvCliente" runat="server" 
                                            ControlToValidate="ddlCliente" 
                                            InitialValue=""
                                            ErrorMessage="Debe seleccionar un cliente" 
                                            CssClass="text-danger small" 
                                            Display="Dynamic">
                </asp:RequiredFieldValidator>
            </div>
            
            <div class="col-md-6">
                <label for="txtFechaEntrada" class="form-label">
                    Fecha de Entrada <span class="campo-requerido">*</span>
                </label>
                <asp:TextBox ID="txtFechaEntrada" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvFechaEntrada" runat="server" 
                                            ControlToValidate="txtFechaEntrada" 
                                            ErrorMessage="La fecha de entrada es requerida" 
                                            CssClass="text-danger small" 
                                            Display="Dynamic">
                </asp:RequiredFieldValidator>
                <asp:CustomValidator ID="cvFechaEntrada" runat="server" 
                                     ControlToValidate="txtFechaEntrada" 
                                     ErrorMessage="No se permiten fechas menores o iguales a la fecha actual" 
                                     CssClass="text-danger small" 
                                     Display="Dynamic"
                                     OnServerValidate="cvFechaEntrada_ServerValidate">
                </asp:CustomValidator>
            </div>
            
            <div class="col-md-6">
                <label for="txtFechaSalida" class="form-label">
                    Fecha de Salida <span class="campo-requerido">*</span>
                </label>
                <asp:TextBox ID="txtFechaSalida" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvFechaSalida" runat="server" 
                                            ControlToValidate="txtFechaSalida" 
                                            ErrorMessage="La fecha de salida es requerida" 
                                            CssClass="text-danger small" 
                                            Display="Dynamic">
                </asp:RequiredFieldValidator>
                <asp:CustomValidator ID="cvFechaSalida" runat="server" 
                                     ControlToValidate="txtFechaSalida" 
                                     ErrorMessage="La fecha de salida debe ser mayor o igual a la fecha de entrada" 
                                     CssClass="text-danger small" 
                                     Display="Dynamic"
                                     OnServerValidate="cvFechaSalida_ServerValidate">
                </asp:CustomValidator>
            </div>
            
            <div class="col-md-6">
                <label for="txtNumeroAdultos" class="form-label">
                    Número de Adultos <span class="campo-requerido">*</span>
                </label>
                <asp:TextBox ID="txtNumeroAdultos" runat="server" CssClass="form-control" 
                             TextMode="Number" min="1"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvNumeroAdultos" runat="server" 
                                            ControlToValidate="txtNumeroAdultos" 
                                            ErrorMessage="El número de adultos es requerido" 
                                            CssClass="text-danger small" 
                                            Display="Dynamic">
                </asp:RequiredFieldValidator>
                <asp:RangeValidator ID="rvNumeroAdultos" runat="server" 
                                    ControlToValidate="txtNumeroAdultos" 
                                    MinimumValue="1" 
                                    MaximumValue="8" 
                                    Type="Integer"
                                    ErrorMessage="Debe ingresar al menos 1 adulto" 
                                    CssClass="text-danger small" 
                                    Display="Dynamic">
                </asp:RangeValidator>
            </div>
            
            <div class="col-md-6">
                <label for="txtNumeroNinhos" class="form-label">
                    Número de Niños <span class="campo-requerido">*</span>
                </label>
                <asp:TextBox ID="txtNumeroNinhos" runat="server" CssClass="form-control" 
                             TextMode="Number" min="0"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvNumeroNinhos" runat="server" 
                                            ControlToValidate="txtNumeroNinhos" 
                                            ErrorMessage="El número de niños es requerido" 
                                            CssClass="text-danger small" 
                                            Display="Dynamic">
                </asp:RequiredFieldValidator>
                <asp:RangeValidator ID="rvNumeroNinhos" runat="server" 
                                    ControlToValidate="txtNumeroNinhos" 
                                    MinimumValue="0" 
                                    MaximumValue="7" 
                                    Type="Integer"
                                    ErrorMessage="El número de niños debe estar entre 0 y 7" 
                                    CssClass="text-danger small" 
                                    Display="Dynamic">
                </asp:RangeValidator>
            </div>
            
            <div class="col-12">
                <div class="alert alert-info">
                    <i class="fas fa-info-circle me-2"></i>
                    <strong>Nota:</strong> El sistema asignará automáticamente la habitación disponible 
                    con la menor cantidad de reservaciones que cumpla con la capacidad requerida.
                </div>
            </div>
            
            <div class="col-12 text-end mt-4">
                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" 
                            CssClass="btn btn-cancelar me-2" OnClick="btnCancelar_Click" CausesValidation="false" />
                <asp:Button ID="btnGuardar" runat="server" Text="Guardar Reservación" 
                            CssClass="btn btn-guardar" OnClick="btnGuardar_Click" />
            </div>
        </div>
    </div>
</asp:Content>