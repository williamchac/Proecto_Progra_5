using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProyectoFinalP5.Pages
{
    /// <summary>
    /// Página que permite a los empleados gestionar todas las reservaciones del sistema
    /// Implementa el requerimiento RF-002 - Gestionar Reservaciones
    /// </summary>
    public partial class GestionarReservaciones : System.Web.UI.Page
    {
        /// <summary>
        /// Evento que se ejecuta al cargar la página
        /// Verifica que el usuario sea empleado y carga los datos
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            // Verificar que existe una sesión activa
            if (Session["idPersona"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            // Verificar que el usuario es empleado
            bool esEmpleado = Session["esEmpleado"] != null && Convert.ToBoolean(Session["esEmpleado"]);
            if (!esEmpleado)
            {
                Response.Redirect("~/Pages/MisReservaciones.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarClientes();
                CargarTodasReservaciones();
            }
        }

        /// <summary>
        /// Carga la lista de clientes activos en el dropdown
        /// Utiliza el SP sp_ObtenerPersonasActivas
        /// </summary>
        private void CargarClientes()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["PV_ProyectoFinalConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_ObtenerPersonasActivas", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    try
                    {
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        ddlCliente.Items.Clear();
                        ddlCliente.Items.Add(new ListItem("Todos los clientes", ""));

                        while (reader.Read())
                        {
                            string nombreCompleto = reader["nombreCompleto"].ToString();
                            string idPersona = reader["idPersona"].ToString();
                            ddlCliente.Items.Add(new ListItem(nombreCompleto, idPersona));
                        }

                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        MostrarMensaje("Error al cargar clientes: " + ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Carga todas las reservaciones del sistema
        /// Utiliza el SP sp_ObtenerTodasReservaciones
        /// </summary>
        private void CargarTodasReservaciones()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["PV_ProyectoFinalConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_ObtenerTodasReservaciones", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    try
                    {
                        conn.Open();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        gvReservaciones.DataSource = dt;
                        gvReservaciones.DataBind();
                    }
                    catch (Exception ex)
                    {
                        MostrarMensaje("Error al cargar las reservaciones: " + ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Evento que maneja el clic en el botón filtrar
        /// Filtra las reservaciones según los criterios seleccionados
        /// </summary>
        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                FiltrarReservaciones();
            }
        }

        /// <summary>
        /// Filtra las reservaciones según los criterios seleccionados
        /// Utiliza el SP sp_FiltrarReservaciones
        /// </summary>
        private void FiltrarReservaciones()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["PV_ProyectoFinalConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_FiltrarReservaciones", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parámetro idPersona (puede ser NULL)
                    if (!string.IsNullOrEmpty(ddlCliente.SelectedValue))
                    {
                        cmd.Parameters.AddWithValue("@idPersona", Convert.ToInt32(ddlCliente.SelectedValue));
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@idPersona", DBNull.Value);
                    }

                    cmd.Parameters.AddWithValue("@fechaEntrada", Convert.ToDateTime(txtFechaEntrada.Text));
                    cmd.Parameters.AddWithValue("@fechaSalida", Convert.ToDateTime(txtFechaSalida.Text));

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
                            MostrarMensaje("No se encontraron reservaciones con los criterios seleccionados.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MostrarMensaje("Error al filtrar las reservaciones: " + ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Validación personalizada para la fecha de salida
        /// Verifica que sea mayor o igual a la fecha de entrada
        /// </summary>
        protected void cvFechaSalida_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (!string.IsNullOrEmpty(txtFechaEntrada.Text) && !string.IsNullOrEmpty(txtFechaSalida.Text))
            {
                DateTime fechaEntrada = Convert.ToDateTime(txtFechaEntrada.Text);
                DateTime fechaSalida = Convert.ToDateTime(txtFechaSalida.Text);
                args.IsValid = fechaSalida >= fechaEntrada;
            }
            else
            {
                args.IsValid = false;
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