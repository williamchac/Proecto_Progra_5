using System;
using System.Web.UI;

namespace ProyectoFinalP5.Pages
{
    /// <summary>
    /// Página genérica para mostrar mensajes de confirmación o error
    /// Recibe parámetros por QueryString: tipo, mensaje, pagina
    /// </summary>
    public partial class Mensaje : System.Web.UI.Page
    {
        /// <summary>
        /// Evento que se ejecuta al cargar la página
        /// Configura el mensaje y el enlace de retorno según los parámetros
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            // Verificar sesión activa
            if (Session["idPersona"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                ConfigurarMensaje();
            }
        }

        /// <summary>
        /// Configura el mensaje y el ícono según el tipo recibido por QueryString
        /// Tipos válidos: exito, error, info
        /// </summary>
        private void ConfigurarMensaje()
        {
            // Obtener parámetros del QueryString
            string tipo = Request.QueryString["tipo"]?.ToLower() ?? "info";
            string mensaje = Request.QueryString["mensaje"] ?? "Operación completada";
            string paginaRetorno = Request.QueryString["pagina"] ?? "MisReservaciones";

            // Configurar el mensaje
            lblMensaje.Text = mensaje;

            // Configurar el ícono según el tipo
            switch (tipo)
            {
                case "exito":
                    lblIcono.Text = "<i class='fas fa-check-circle icono-exito'></i>";
                    break;
                case "error":
                    lblIcono.Text = "<i class='fas fa-times-circle icono-error'></i>";
                    break;
                case "info":
                default:
                    lblIcono.Text = "<i class='fas fa-info-circle icono-info'></i>";
                    break;
            }

            // Configurar el enlace de retorno
            ConfigurarEnlaceRetorno(paginaRetorno);
        }

        /// <summary>
        /// Configura el enlace de retorno según la página especificada
        /// </summary>
        private void ConfigurarEnlaceRetorno(string paginaRetorno)
        {
            string url = "~/Pages/MisReservaciones.aspx"; // Página por defecto

            switch (paginaRetorno)
            {
                case "MisReservaciones":
                    url = "~/Pages/MisReservaciones.aspx";
                    break;
                case "GestionarReservaciones":
                    url = "~/Pages/GestionarReservaciones.aspx";
                    break;
                case "ListaHabitaciones":
                    url = "~/Pages/ListaHabitaciones.aspx";
                    break;
                case "CrearReservacion":
                    url = "~/Pages/CrearReservacion.aspx";
                    break;
                case "CrearHabitacion":
                    url = "~/Pages/CrearHabitacion.aspx";
                    break;
                default:
                    // Determinar según el tipo de usuario
                    bool esEmpleado = Session["esEmpleado"] != null && Convert.ToBoolean(Session["esEmpleado"]);
                    url = esEmpleado ? "~/Pages/GestionarReservaciones.aspx" : "~/Pages/MisReservaciones.aspx";
                    break;
            }

            lnkRegresar.NavigateUrl = url;
        }
    }
}