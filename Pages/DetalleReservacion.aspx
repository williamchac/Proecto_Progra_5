<%@ Page Title="Detalle de Reservación" Language="C#" MasterPageFile="~/Site.Master" 
    AutoEventWireup="true" CodeBehind="DetalleReservacion.aspx.cs" 
    Inherits="ProyectoFinalP5.Pages.DetalleReservacion" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <div class="container mt-4">
       <h3 class="mb-4">Detalle de Reservación</h3>

<!-- MENSAJE DE ERROR / ALERTA -->
<asp:Panel ID="pnlMensaje" runat="server" CssClass="alert alert-danger" Visible="false">
    <asp:Label ID="lblMensaje" runat="server" />
</asp:Panel>

        <!-- Sección de Información de la Reservación -->
        <div class="card shadow-sm mb-4">
            <div class="card-header bg-primary text-white">
                <h5 class="mb-0">
                    <i class="fas fa-calendar-alt me-2"></i>
                    Información de la Reservación #<asp:Literal ID="litIdReservacion" runat="server" />
                </h5>
            </div>
            <div class="card-body">
                <div class="row">
                    <!-- Columna Izquierda -->
                    <div class="col-md-6">
                        <div class="form-group row mb-3">
                            <label class="col-sm-4 col-form-label fw-bold">Hotel:</label>
                            <div class="col-sm-8">
                                <p class="form-control-plaintext"><asp:Literal ID="litHotel" runat="server" /></p>
                            </div>
                        </div>
                        <div class="form-group row mb-3">
                            <label class="col-sm-4 col-form-label fw-bold">Número de Habitación:</label>
                            <div class="col-sm-8">
                                <p class="form-control-plaintext"><asp:Literal ID="litNumeroHabitacion" runat="server" /></p>
                            </div>
                        </div>
                        <div class="form-group row mb-3">
                            <label class="col-sm-4 col-form-label fw-bold">Cliente:</label>
                            <div class="col-sm-8">
                                <p class="form-control-plaintext"><asp:Literal ID="litCliente" runat="server" /></p>
                            </div>
                        </div>
                        <div class="form-group row mb-3">
                            <label class="col-sm-4 col-form-label fw-bold">Fecha de Entrada:</label>
                            <div class="col-sm-8">
                                <p class="form-control-plaintext"><asp:Literal ID="litFechaEntrada" runat="server" /></p>
                            </div>
                        </div>
                        <div class="form-group row mb-3">
                            <label class="col-sm-4 col-form-label fw-bold">Fecha de Salida:</label>
                            <div class="col-sm-8">
                                <p class="form-control-plaintext"><asp:Literal ID="litFechaSalida" runat="server" /></p>
                            </div>
                        </div>
                    </div>
                    
                    <!-- Columna Derecha -->
                    <div class="col-md-6">
                        <div class="form-group row mb-3">
                            <label class="col-sm-4 col-form-label fw-bold">Total de Días:</label>
                            <div class="col-sm-8">
                                <p class="form-control-plaintext"><asp:Literal ID="litTotalDias" runat="server" /></p>
                            </div>
                        </div>
                        <div class="form-group row mb-3">
                            <label class="col-sm-4 col-form-label fw-bold">Número de Adultos:</label>
                            <div class="col-sm-8">
                                <p class="form-control-plaintext"><asp:Literal ID="litNumeroAdultos" runat="server" /></p>
                            </div>
                        </div>
                        <div class="form-group row mb-3">
                            <label class="col-sm-4 col-form-label fw-bold">Número de Niños:</label>
                            <div class="col-sm-8">
                                <p class="form-control-plaintext"><asp:Literal ID="litNumeroNinhos" runat="server" /></p>
                            </div>
                        </div>
                        <div class="form-group row mb-3">
                            <label class="col-sm-4 col-form-label fw-bold">Costo Total:</label>
                            <div class="col-sm-8">
                                <p class="form-control-plaintext text-success fw-bold">
                                    <asp:Literal ID="litCostoTotal" runat="server" />
                                </p>
                            </div>
                        </div>
                        <div class="form-group row mb-3">
                            <label class="col-sm-4 col-form-label fw-bold">Estado:</label>
                            <div class="col-sm-8">
                                <span class="badge bg-info fs-6">
                                    <asp:Literal ID="litEstado" runat="server" />
                                </span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        
        <!-- Sección de Botones de Acción -->
        <div class="d-flex justify-content-between mb-4">
            <div>
                <asp:Button ID="btnEditar" runat="server" Text="Editar Reservación" 
                    CssClass="btn btn-warning me-2" Visible="false" OnClick="btnEditar_Click" />
                
                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar Reservación" 
                    CssClass="btn btn-danger me-2" Visible="false" OnClick="btnCancelarReservacion_Click" 
                    OnClientClick="return confirm('¿Está seguro de cancelar esta reservación?');" />
            </div>
            
            <asp:Button ID="btnRegresar" runat="server" Text="Regresar" 
                CssClass="btn btn-outline-secondary" OnClick="btnRegresar_Click" />
        </div>
        
        <!-- Sección de Bitácora -->
        <div class="card shadow-sm">
            <div class="card-header bg-secondary text-white">
                <h5 class="mb-0">
                    <i class="fas fa-history me-2"></i>
                    Historial de Acciones (Bitácora)
                </h5>
            </div>
            <div class="card-body">
                <div class="table-responsive">
                    <asp:GridView ID="gvBitacora" runat="server" CssClass="table table-hover table-bordered"
                        AutoGenerateColumns="False" EmptyDataText="No hay registros en bitácora."
                        GridLines="None" HeaderStyle-CssClass="table-dark">
                        <Columns>
                            <asp:BoundField DataField="FechaHoraFormateada" HeaderText="Fecha y Hora" 
                                HeaderStyle-Width="20%" ItemStyle-CssClass="text-center" />
                            <asp:BoundField DataField="accionRealizada" HeaderText="Acción Realizada" 
                                HeaderStyle-Width="50%" />
                            <asp:BoundField DataField="RealizadaPor" HeaderText="Usuario" 
                                HeaderStyle-Width="30%" ItemStyle-CssClass="text-center" />
                        </Columns>
                        <EmptyDataRowStyle CssClass="text-center py-4" />
                        <HeaderStyle CssClass="bg-dark text-white" />
                        <RowStyle CssClass="align-middle" />
                        <AlternatingRowStyle CssClass="bg-light" />
                    </asp:GridView>
                </div>
            </div>
        </div>
    </div>
    
    <!-- Script para iconos (Font Awesome) -->
    <script src="https://kit.fontawesome.com/a076d05399.js" crossorigin="anonymous"></script>
    
</asp:Content>
