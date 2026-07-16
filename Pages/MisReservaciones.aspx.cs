using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProyectoFinalP5.Pages
{
    /// <summary>
    /// Página que muestra las reservaciones del cliente autenticado
    /// Implementa el requerimiento RF-002 - Mis Reservaciones
    /// </summary>
    public partial class MisReservaciones : System.Web.UI.Page
    {
        /// <summary>
        /// Evento que se ejecuta al cargar la página
        /// Verifica la sesión y carga las reservaciones del usuario
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            // Verificar que existe una sesión activa
            if (Session["idPersona"] == null)
            {
                Response.Redirect("~/Pages/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarReservaciones();
            }
        }

        /// <summary>
        /// Carga las reservaciones del cliente desde la base de datos
        /// Utiliza el SP sp_ObtenerReservacionesPorCliente
        /// </summary>
        private void CargarReservaciones()
        {
            int idPersona = Convert.ToInt32(Session["idPersona"]);

            // Use the connection string name defined in Web.config
            var csSetting = ConfigurationManager.ConnectionStrings["PvProyectoFinalDB"];
            if (csSetting == null)
            {
                MostrarMensaje("Cadena de conexión no encontrada en Web.config (PvProyectoFinalDB).");
                return;
            }

            string connectionString = csSetting.ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_ObtenerReservacionesPorCliente", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idPersona", idPersona);

                    try
                    {
                        conn.Open();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        gvReservaciones.DataSource = dt;
                        gvReservaciones.DataBind();

                        if (dt.Rows.Count == 0)
                        {
                            MostrarMensaje("No tiene reservaciones registradas en el sistema.");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the exception and show a friendly message
                        System.Diagnostics.Debug.WriteLine("Error CargarReservaciones: " + ex.Message);
                        MostrarMensaje("Error al cargar las reservaciones. Consulte al administrador.");
                    }
                }
            }
        }

        /// <summary>
        /// Evento que se ejecuta al enlazar cada fila del GridView
        /// Determina el estado de la reservación según las reglas de negocio
        /// </summary>
        protected void gvReservaciones_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Obtener los valores de la fila
                string estado = DataBinder.Eval(e.Row.DataItem, "estado").ToString();
                DateTime fechaEntrada = Convert.ToDateTime(DataBinder.Eval(e.Row.DataItem, "fechaEntrada"));
                DateTime fechaSalida = Convert.ToDateTime(DataBinder.Eval(e.Row.DataItem, "fechaSalida"));
                DateTime fechaActual = DateTime.Now.Date;

                // Determinar el texto y estilo del estado
                Label lblEstado = (Label)e.Row.FindControl("lblEstado");
                string textoEstado = "";
                string claseEstado = "estado-badge ";

                if (estado == "I")
                {
                    textoEstado = "Cancelada";
                    claseEstado += "estado-cancelada";
                }
                else if (estado == "A" && fechaSalida.Date < fechaActual)
                {
                    textoEstado = "Finalizada";
                    claseEstado += "estado-finalizada";
                }
                else if (estado == "A" && fechaEntrada.Date <= fechaActual)
                {
                    textoEstado = "En proceso";
                    claseEstado += "estado-proceso";
                }
                else if (estado == "A" && fechaEntrada.Date > fechaActual)
                {
                    textoEstado = "En espera";
                    claseEstado += "estado-espera";
                }

                lblEstado.Text = textoEstado;
                lblEstado.CssClass = claseEstado;
            }
        }

        /// <summary>
        /// Evento que maneja el clic en el botón de nueva reservación
        /// Redirige al formulario de creación de reservaciones
        /// </summary>
        protected void btnNuevaReservacion_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Pages/CrearReservacion.aspx");
        }

        /// <summary>
        /// Muestra un mensaje informativo en la página
        /// </summary>
        private void MostrarMensaje(string mensaje)
        {
            lblMensaje.Text = mensaje;
            pnlMensaje.Visible = true;
        }
    }
}