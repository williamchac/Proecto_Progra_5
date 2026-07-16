using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace ProyectoFinalP5.Pages
{
    public partial class CrearReservacion : System.Web.UI.Page
    {
        private string _connName = "PvProyectoFinalDB";

        protected void Page_Load(object sender, EventArgs e)
        {
            // RF-001 d): Validar existencia de sesión activa
            if (Session["idPersona"] == null)
            {
                Response.Redirect("~/Pages/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!IsPostBack)
            {
                CargarHoteles();
                CargarClientesActivos();

                // Si el usuario autenticado NO es empleado, seleccionar su nombre y bloquear el dropdown
                bool esEmpleado = Session["esEmpleado"] != null && Convert.ToBoolean(Session["esEmpleado"]);
                if (!esEmpleado)
                {
                    // Intentar seleccionar al usuario en el dropdown
                    int idPersonaSesion = Convert.ToInt32(Session["idPersona"]);
                    var item = ddlCliente.Items.FindByValue(idPersonaSesion.ToString());
                    if (item != null)
                    {
                        ddlCliente.ClearSelection();
                        item.Selected = true;
                    }

                    ddlCliente.Enabled = false;
                }
            }
        }

        private void CargarHoteles()
        {
            try
            {
                string cs = ConfigurationManager.ConnectionStrings[_connName].ConnectionString;
                using (SqlConnection conn = new SqlConnection(cs))
                using (SqlCommand cmd = new SqlCommand("sp_ObtenerHoteles", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    ddlHotel.DataSource = dt;
                    ddlHotel.DataTextField = "nombre";
                    ddlHotel.DataValueField = "idHotel";
                    ddlHotel.DataBind();

                    // Insertar opción por defecto vacía para validar RequiredFieldValidator
                    ddlHotel.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Seleccione --", ""));
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al cargar hoteles: " + ex.Message);
            }
        }

        private void CargarClientesActivos()
        {
            try
            {
                string cs = ConfigurationManager.ConnectionStrings[_connName].ConnectionString;
                using (SqlConnection conn = new SqlConnection(cs))
                using (SqlCommand cmd = new SqlCommand("sp_ObtenerPersonasActivas", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Filtrar solo clientes (no empleados)
                    DataView dv = new DataView(dt);
                    dv.RowFilter = "esEmpleado = 0";

                    ddlCliente.DataSource = dv;
                    ddlCliente.DataTextField = "nombreCompleto";
                    ddlCliente.DataValueField = "idPersona";
                    ddlCliente.DataBind();

                    ddlCliente.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Seleccione --", ""));
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al cargar clientes: " + ex.Message);
            }
        }

        protected void cvFechaEntrada_ServerValidate(object source, ServerValidateEventArgs args)
        {
            DateTime fechaEntrada;
            if (!DateTime.TryParse(txtFechaEntrada.Text, out fechaEntrada))
            {
                args.IsValid = false;
                return;
            }

            // No permitir fechas menores o iguales a la fecha actual
            args.IsValid = fechaEntrada.Date > DateTime.Now.Date;
        }

        protected void cvFechaSalida_ServerValidate(object source, ServerValidateEventArgs args)
        {
            DateTime fechaEntrada;
            DateTime fechaSalida;
            if (!DateTime.TryParse(txtFechaEntrada.Text, out fechaEntrada) ||
                !DateTime.TryParse(txtFechaSalida.Text, out fechaSalida))
            {
                args.IsValid = false;
                return;
            }

            // Fecha de salida debe ser >= fecha de entrada
            args.IsValid = fechaSalida.Date >= fechaEntrada.Date;
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            // Volver a la lista de reservaciones del usuario
            bool esEmpleado = Session["esEmpleado"] != null && Convert.ToBoolean(Session["esEmpleado"]);
            if (esEmpleado)
            {
                Response.Redirect("~/Pages/GestionarReservaciones.aspx", false);
            }
            else
            {
                Response.Redirect("~/Pages/MisReservaciones.aspx", false);
            }

            Context.ApplicationInstance.CompleteRequest();
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            int idHotel;
            int idCliente;
            DateTime fechaEntrada;
            DateTime fechaSalida;
            int numeroAdultos;
            int numeroNinhos;

            if (!int.TryParse(ddlHotel.SelectedValue, out idHotel))
            {
                MostrarMensaje("Debe seleccionar un hotel válido.");
                return;
            }

            if (!int.TryParse(ddlCliente.SelectedValue, out idCliente))
            {
                MostrarMensaje("Debe seleccionar un cliente válido.");
                return;
            }

            if (!DateTime.TryParse(txtFechaEntrada.Text, out fechaEntrada) ||
                !DateTime.TryParse(txtFechaSalida.Text, out fechaSalida))
            {
                MostrarMensaje("Fechas inválidas.");
                return;
            }

            if (!int.TryParse(txtNumeroAdultos.Text, out numeroAdultos) ||
                !int.TryParse(txtNumeroNinhos.Text, out numeroNinhos))
            {
                MostrarMensaje("Número de personas inválido.");
                return;
            }

            // Buscar habitación disponible usando el procedimiento almacenado
            int capacidadRequerida = numeroAdultos + numeroNinhos;
            int idHabitacionSeleccionada = ObtenerHabitacionDisponible(idHotel, capacidadRequerida);

            if (idHabitacionSeleccionada == 0)
            {
                MostrarMensaje("No existen habitaciones disponibles que cumplan la capacidad requerida.");
                return;
            }

            // Crear reservación usando sp_CrearReservacion
            try
            {
                string cs = ConfigurationManager.ConnectionStrings[_connName].ConnectionString;
                using (SqlConnection conn = new SqlConnection(cs))
                using (SqlCommand cmd = new SqlCommand("sp_CrearReservacion", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idPersona", idCliente);
                    cmd.Parameters.AddWithValue("@idHabitacion", idHabitacionSeleccionada);
                    cmd.Parameters.AddWithValue("@fechaEntrada", fechaEntrada);
                    cmd.Parameters.AddWithValue("@fechaSalida", fechaSalida);
                    cmd.Parameters.AddWithValue("@numeroAdultos", numeroAdultos);
                    cmd.Parameters.AddWithValue("@numeroNinhos", numeroNinhos);

                    // idPersonaCreador: usuario autenticado (puede ser empleado o el mismo cliente)
                    int idCreador = Convert.ToInt32(Session["idPersona"]);
                    cmd.Parameters.AddWithValue("@idPersonaCreador", idCreador);

                    SqlParameter outId = new SqlParameter("@idReservacionCreada", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outId);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    int idReservacionCreada = 0;
                    if (outId.Value != DBNull.Value)
                    {
                        idReservacionCreada = Convert.ToInt32(outId.Value);
                    }

                    // Redirigir a MisReservaciones o mostrar éxito
                    bool esEmpleado = Session["esEmpleado"] != null && Convert.ToBoolean(Session["esEmpleado"]);
                    if (esEmpleado)
                    {
                        Response.Redirect("~/Pages/GestionarReservaciones.aspx", false);
                    }
                    else
                    {
                        Response.Redirect("~/Pages/MisReservaciones.aspx", false);
                    }

                    Context.ApplicationInstance.CompleteRequest();
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al crear reservación: " + ex.Message);
            }
        }

        /// <summary>
        /// Llama a sp_ObtenerHabitacionesDisponibles y retorna el primer idHabitacion (la de menor cantidad de reservaciones)
        /// Retorna 0 si no hay habitaciones disponibles.
        /// </summary>
        private int ObtenerHabitacionDisponible(int idHotel, int capacidadRequerida)
        {
            try
            {
                string cs = ConfigurationManager.ConnectionStrings[_connName].ConnectionString;
                using (SqlConnection conn = new SqlConnection(cs))
                using (SqlCommand cmd = new SqlCommand("sp_ObtenerHabitacionesDisponibles", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idHotel", idHotel);
                    cmd.Parameters.AddWithValue("@capacidadRequerida", capacidadRequerida);

                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count == 0)
                        return 0;

                    return Convert.ToInt32(dt.Rows[0]["idHabitacion"]);
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al obtener habitaciones disponibles: " + ex.Message);
                return 0;
            }
        }

        private void MostrarMensaje(string mensaje)
        {
            lblMensaje.Text = mensaje;
            pnlMensaje.Visible = true;
        }
    }
}