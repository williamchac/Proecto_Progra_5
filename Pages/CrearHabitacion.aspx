<%@ Page Title="Crear Habitación" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CrearHabitacion.aspx.cs" Inherits="ProyectoFinalP5.Pages.CrearHabitacion" %>

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
        <i class="fas fa-plus-circle me-2"></i>Crear Nueva Habitación
    </h2>
    
    <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert alert-danger alert-custom mb-4" role="alert">
        <i class="fas fa-exclamation-triangle me-2"></i>
        <asp:Label ID="lblMensaje" runat="server"></asp:Label>
    </asp:Panel>
    
    <div class="form-panel">
        <div class="row g-3">
            <div class="col-md-6">
                <label for="ddlHotel" class="form-label">
                    <i class="fas fa-hotel me-1"></i> Hotel <span class="campo-requerido">*</span>
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
                <label for="txtNumeroHabitacion" class="form-label">
                    <i class="fas fa-door-open me-1"></i> Número de Habitación <span class="campo-requerido">*</span>
                </label>
                <asp:TextBox ID="txtNumeroHabitacion" runat="server" CssClass="form-control" 
                             placeholder="Ej: 101, A201, Suite 5" MaxLength="10"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvNumeroHabitacion" runat="server" 
                                            ControlToValidate="txtNumeroHabitacion" 
                                            ErrorMessage="El número de habitación es requerido" 
                                            CssClass="text-danger small" 
                                            Display="Dynamic">
                </asp:RequiredFieldValidator>
                <small class="text-muted">Máximo 10 caracteres</small>
            </div>
            
            <div class="col-md-6">
                <label for="txtCapacidadMaxima" class="form-label">
                    <i class="fas fa-users me-1"></i> Capacidad Máxima <span class="campo-requerido">*</span>
                </label>
                <asp:TextBox ID="txtCapacidadMaxima" runat="server" CssClass="form-control" 
                             TextMode="Number" min="1" max="8"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvCapacidadMaxima" runat="server" 
                                            ControlToValidate="txtCapacidadMaxima" 
                                            ErrorMessage="La capacidad máxima es requerida" 
                                            CssClass="text-danger small" 
                                            Display="Dynamic">
                </asp:RequiredFieldValidator>
                <asp:RangeValidator ID="rvCapacidadMaxima" runat="server" 
                                    ControlToValidate="txtCapacidadMaxima" 
                                    MinimumValue="1" 
                                    MaximumValue="8" 
                                    Type="Integer"
                                    ErrorMessage="La capacidad debe estar entre 1 y 8 personas" 
                                    CssClass="text-danger small" 
                                    Display="Dynamic">
                </asp:RangeValidator>
            </div>
            
            <div class="col-12">
                <label for="txtDescripcion" class="form-label">
                    <i class="fas fa-align-left me-1"></i> Descripción <span class="campo-requerido">*</span>
                </label>
                <asp:TextBox ID="txtDescripcion" runat="server" CssClass="form-control" 
                             TextMode="MultiLine" Rows="4" MaxLength="500"
                             placeholder="Describa las características de la habitación..."></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvDescripcion" runat="server" 
                                            ControlToValidate="txtDescripcion" 
                                            ErrorMessage="La descripción es requerida" 
                                            CssClass="text-danger small" 
                                            Display="Dynamic">
                </asp:RequiredFieldValidator>
                <small class="text-muted">Máximo 500 caracteres</small>
            </div>
            
            <div class="col-12">
                <div class="alert alert-info">
                    <i class="fas fa-info-circle me-2"></i>
                    <strong>Nota:</strong> El número de habitación no puede estar duplicado para el mismo hotel. 
                    La habitación se creará con estado "Activa" automáticamente.
                </div>
            </div>
            
            <div class="col-12 text-end mt-4">
                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" 
                            CssClass="btn btn-cancelar me-2" OnClick="btnCancelar_Click" CausesValidation="false" />
                <asp:Button ID="btnGuardar" runat="server" Text="Guardar Habitación" 
                            CssClass="btn btn-guardar" OnClick="btnGuardar_Click" />
            </div>
        </div>
    </div>
</asp:Content>