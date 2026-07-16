<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" 
    Inherits="ProyectoFinalP5.Pages.Login" %>

<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Iniciar sesión - Booking System</title>
    
    <%-- Bootstrap CSS --%>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />
    <%-- Bootstrap Icons --%>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.1/font/bootstrap-icons.css" rel="stylesheet" />
    
    <style>
        /* RNF-001 b): Diseño inspirado en Booking.com */
        :root {
            --booking-blue: #003580;
            --booking-light-blue: #0071c2;
            --booking-yellow: #febb02;
        }
        
        body {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
        }
        
        .login-container {
            max-width: 450px;
            width: 100%;
            padding: 0 1rem;
        }
        
        /* Logo y branding */
        .brand-header {
            text-align: center;
            margin-bottom: 2rem;
            color: white;
        }
        
        .brand-logo {
            font-size: 3.5rem;
            margin-bottom: 0.5rem;
            text-shadow: 0 2px 4px rgba(0,0,0,0.2);
        }
        
        .brand-name {
            font-size: 2rem;
            font-weight: 700;
            margin-bottom: 0.25rem;
            text-shadow: 0 2px 4px rgba(0,0,0,0.2);
        }
        
        .brand-tagline {
            font-size: 0.95rem;
            opacity: 0.95;
        }
        
        /* Tarjeta de login */
        .login-card {
            background: white;
            border-radius: 12px;
            box-shadow: 0 10px 40px rgba(0,0,0,0.2);
            padding: 2.5rem;
        }
        
        .login-title {
            color: var(--booking-blue);
            font-weight: 700;
            margin-bottom: 1.5rem;
            text-align: center;
            font-size: 1.5rem;
        }
        
        .form-label {
            color: #333;
            font-weight: 600;
            margin-bottom: 0.5rem;
        }
        
        .form-control {
            border: 2px solid #e0e0e0;
            border-radius: 6px;
            padding: 0.75rem;
            font-size: 0.95rem;
            transition: all 0.3s ease;
        }
        
        .form-control:focus {
            border-color: var(--booking-light-blue);
            box-shadow: 0 0 0 0.2rem rgba(0, 113, 194, 0.15);
        }
        
        .btn-login {
            background-color: var(--booking-blue);
            border: none;
            color: white;
            font-weight: 600;
            padding: 0.85rem;
            border-radius: 6px;
            font-size: 1rem;
            transition: all 0.3s ease;
            width: 100%;
        }
        
        .btn-login:hover {
            background-color: var(--booking-light-blue);
            transform: translateY(-2px);
            box-shadow: 0 5px 15px rgba(0, 113, 194, 0.3);
        }
        
        /* Mensajes de error */
        .text-danger {
            font-size: 0.875rem;
            margin-top: 0.25rem;
            display: block;
        }
        
        .alert-error {
            background-color: #fee;
            border: 1px solid #fcc;
            color: #c33;
            padding: 0.75rem;
            border-radius: 6px;
            margin-bottom: 1rem;
            font-size: 0.95rem;
        }
        
        /* Footer del login */
        .login-footer {
            text-align: center;
            margin-top: 2rem;
            color: white;
            font-size: 0.875rem;
        }
        
        .login-footer i {
            color: var(--booking-yellow);
            margin-right: 0.25rem;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-container">
            <%-- RNF-001 b): Logo y nombre del sistema --%>
            <div class="brand-header">
                <div class="brand-logo">
                    <i class="bi bi-house-door-fill"></i>
                </div>
                <h1 class="brand-name">Booking System</h1>
                <p class="brand-tagline">Sistema de Reservaciones de Hospedaje</p>
            </div>
            
            <%-- Tarjeta de inicio de sesión --%>
            <div class="login-card">
                <h2 class="login-title">Iniciar sesión</h2>
                
                <%-- RF-001 b): Mensaje de error si las credenciales son incorrectas --%>
                <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert-error">
                    <i class="bi bi-exclamation-triangle-fill"></i>
                    <asp:Label ID="lblMensaje" runat="server"></asp:Label>
                </asp:Panel>
                
                <%-- RF-001 a): Campo de email --%>
                <div class="mb-3">
                    <label for="txtEmail" class="form-label">
                        <i class="bi bi-envelope-fill"></i> Correo electrónico
                    </label>
                    <asp:TextBox ID="txtEmail" runat="server" ClientIDMode="Static"
                        CssClass="form-control" 
                        placeholder="correo@ejemplo.com"
                        autocomplete="email"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvEmail" runat="server" 
                        ControlToValidate="txtEmail"
                        ErrorMessage="El correo electrónico es obligatorio" 
                        CssClass="text-danger" 
                        Display="Dynamic" />
                </div>
                
                <%-- RF-001 a): Campo de clave --%>
                <div class="mb-4">
                    <label for="txtClave" class="form-label">
                        <i class="bi bi-lock-fill"></i> Contraseña
                    </label>
                    <asp:TextBox ID="txtClave" runat="server" ClientIDMode="Static"
                        CssClass="form-control" 
                        TextMode="Password" 
                        placeholder="********"
                        autocomplete="current-password"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvClave" runat="server" 
                        ControlToValidate="txtClave"
                        ErrorMessage="La contraseña es obligatoria" 
                        CssClass="text-danger" 
                        Display="Dynamic" />
                </div>
                
                <%-- Botón de ingreso --%>
                <div class="d-grid">
                    <asp:Button ID="btnIngresar" runat="server" 
                        CssClass="btn btn-login" 
                        Text="Ingresar al sistema"
                        OnClick="btnIngresar_Click" />
                </div>
            </div>
            
            <%-- Footer --%>
            <div class="login-footer">
                <p class="mb-0">
                    <i class="bi bi-mortarboard-fill"></i>
                    Universidad Castro Carazo - Programación V
                </p>
                <small>© <%= DateTime.Now.Year %> Sistema de Reservaciones</small>
            </div>
        </div>
    </form>
    
    <%-- Bootstrap JS Bundle --%>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
    
    <%-- Autofocus en el campo de email al cargar --%>
    <script>
        window.onload = function() {
            var el = document.getElementById('txtEmail');
            if (el) el.focus();
        };
    </script>
</body>
</html>