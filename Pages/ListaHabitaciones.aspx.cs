using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProyectoFinalP5.Pages
{
    /// <summary>
    /// Página que muestra la lista de todas las habitaciones del sistema
    /// Solo accesible para empleados
    /// Implementa el requerimiento RF-006
    /// </summary>
    public partial class ListaHabitaciones : System.Web.UI.Page
    {
        /// <summary>
        /// Evento que se ejecuta al cargar la página
        /// Verifica que el usuario sea empleado y carga las habitaciones
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            // Verificar sesión activa
            if (Session["idPersona"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            // Verificar que el usuario sea empleado
            bool esEmpleado = Session["esEmpleado"] != null && Convert.ToBoolean(Session["esEmpleado"]);
            if (!esEmpleado)
            {
                Response.Redirect("~/Pages/MisReservaciones.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarHabitaciones();
            }
        }

        /// <summary>
        /// Carga todas las habitaciones desde la base de datos
        /// Utiliza el SP sp_ObtenerTodasHabitaciones
        /// </summary>
        private void CargarHabitaciones()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["PV_ProyectoFinalConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_ObtenerTodasHabitaciones", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    try
                    {
                        conn.Open();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        gvHabitaciones.DataSource = dt;
                        gvHabitaciones.DataBind();

                        if (dt.Rows.Count == 0)
                        {
                            MostrarMensaje("No hay habitaciones registradas en el sistema.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MostrarMensaje("Error al cargar las habitaciones: " + ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Evento que se ejecuta al enlazar cada fila del GridView
        /// Configura el estado y los enlaces según las reglas de negocio
        /// </summary>
        protected void gvHabitaciones_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Obtener el estado de la habitación
                string estado = DataBinder.Eval(e.Row.DataItem, "estado").ToString();

                // Configurar el label de estado
                Label lblEstado = (Label)e.Row.FindControl("lblEstado");
                HyperLink lnkEditar = (HyperLink)e.Row.FindControl("lnkEditar");

                if (estado == "A")
                {
                    lblEstado.Text = "Activa";
                    lblEstado.CssClass = "estado-badge estado-activa";
                    // El link de editar permanece activo
                }
                else // estado == "I"
                {
                    lblEstado.Text = "Inactiva";
                    lblEstado.CssClass = "estado-badge estado-inactiva";

                    // Para habitaciones inactivas, cambiar el comportamiento del link
                    // En lugar de redirigir directamente, se puede mostrar un mensaje
                    // pero mantenemos el link para que el usuario vea el mensaje en la página de edición
                }
            }
        }

        /// <summary>
        /// Evento que maneja el clic en el botón de nueva habitación
        /// Redirige al formulario de creación
        /// </summary>
        protected void btnNuevaHabitacion_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Pages/CrearHabitacion.aspx");
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