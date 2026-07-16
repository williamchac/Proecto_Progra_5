using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProyectoFinalP5.Pages
{
    /// <summary>
    /// Página para editar reservaciones existentes
    /// Implementa el requerimiento RF-004
    /// </summary>
    public partial class EditarReservacion : System.Web.UI.Page
    {
        private int idReservacion;
        private int capacidadMaxima;
        private DateTime fechaEntradaOriginal;

        /// <summary>
        /// Evento que se ejecuta al cargar la página
        /// Valida el acceso y carga los datos de la reservación
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            // Verificar sesión activa
            if (Session["idPersona"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            // Obtener el ID de la reservación del QueryString
            if (Request.QueryString["id"] == null || !int.TryParse(Request.QueryString["id"], out idReservacion))
            {
                Response.Redirect("~/Pages/MisReservaciones.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarDatosReservacion();
                ValidarAccesoEdicion();
            }
        }

        /// <summary>
        /// Carga los datos de la reservación en los controles del formulario
        /// Utiliza el SP sp_ObtenerDetalleReservacion
        /// </summary>
        private void CargarDatosReservacion()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["PV_ProyectoFinalConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_ObtenerDetalleReservacion", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idReservacion", idReservacion);

                    try
                    {
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            // Validar acceso: si no es empleado, verificar que la reservación le pertenece
                            int idPersonaReservacion = Convert.ToInt32(reader["idPersona"]);
                            bool esEmpleado = Session["esEmpleado"] != null && Convert.ToBoolean(Session["esEmpleado"]);
                            int idPersonaSesion = Convert.ToInt32(Session["idPersona"]);

                            if (!esEmpleado && idPersonaReservacion != idPersonaSesion)
                            {
                                reader.Close();
                                Response.Redirect("~/Pages/MisReservaciones.aspx");
                                return;
                            }

                            // Cargar los datos en los controles
                            txtIdReservacion.Text = reader["idReservacion"].ToString();
                            txtHotel.Text = reader["nombreHotel"].ToString();
                            txtNumeroHabitacion.Text = reader["numeroHabitacion"].ToString();
                            txtCliente.Text = reader["nombreCompleto"].ToString();
                            txtFechaEntrada.Text = Convert.ToDateTime(reader["fechaEntrada"]).ToString("yyyy-MM-dd");
                            txtFechaSalida.Text = Convert.ToDateTime(reader["fechaSalida"]).ToString("yyyy-MM-dd");
                            txtNumeroAdultos.Text = reader["numeroAdultos"].ToString();
                            txtNumeroNinhos.Text = reader["numeroNinhos"].ToString();

                            capacidadMaxima = Convert.ToInt32(reader["capacidadMaxima"]);
                            lblCapacidadMaxima.Text = capacidadMaxima.ToString();

                            fechaEntradaOriginal = Convert.ToDateTime(reader["fechaEntrada"]);

                            // Si la fecha de entrada ya pasó, bloquear el campo
                            if (fechaEntradaOriginal.Date <= DateTime.Now.Date)
                            {
                                txtFechaEntrada.Enabled = false;
                                txtFechaEntrada.CssClass += " campo-bloqueado";
                            }
                        }
                        else
                        {
                            reader.Close();
                            Response.Redirect("~/Pages/MisReservaciones.aspx");
                            return;
                        }

                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        MostrarMensaje("Error al cargar los datos: " + ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Valida que el usuario tenga acceso para editar la reservación
        /// según las reglas de negocio definidas en el requerimiento RF-004
        /// </summary>
        private void ValidarAccesoEdicion()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["PV_ProyectoFinalConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_ObtenerDetalleReservacion", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idReservacion", idReservacion);

                    try
                    {
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            string estado = reader["estado"].ToString();
                            DateTime fechaEntrada = Convert.ToDateTime(reader["fechaEntrada"]);
                            DateTime fechaSalida = Convert.ToDateTime(reader["fechaSalida"]);
                            DateTime fechaActual = DateTime.Now.Date;

                            bool esEmpleado = Session["esEmpleado"] != null && Convert.ToBoolean(Session["esEmpleado"]);

                            // Regla: si el estado es I, redirigir
                            if (estado == "I")
                            {
                                reader.Close();
                                RedirigirSegunTipoUsuario();
                                return;
                            }

                            // Regla: si la fecha de salida ya pasó, redirigir
                            if (fechaSalida.Date <= fechaActual)
                            {
                                reader.Close();
                                RedirigirSegunTipoUsuario();
                                return;
                            }

                            // Regla: si no es empleado y la reservación está en proceso, redirigir
                            if (!esEmpleado && fechaEntrada.Date <= fechaActual && fechaSalida.Date > fechaActual)
                            {
                                reader.Close();
                                Response.Redirect("~/Pages/MisReservaciones.aspx");
                                return;
                            }
                        }

                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        MostrarMensaje("Error al validar acceso: " + ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Validación personalizada para la fecha de entrada
        /// No permite fechas menores o iguales a la fecha actual
        /// </summary>
        protected void cvFechaEntrada_ServerValidate(object source, ServerValidateEventArgs args)
        {
            // Si el campo está deshabilitado, la validación es true
            if (!txtFechaEntrada.Enabled)
            {
                args.IsValid = true;
                return;
            }

            if (!string.IsNullOrEmpty(txtFechaEntrada.Text))
            {
                DateTime fechaEntrada = Convert.ToDateTime(txtFechaEntrada.Text);
                DateTime fechaActual = DateTime.Now.Date;
                args.IsValid = fechaEntrada > fechaActual;
            }
            else
            {
                args.IsValid = false;
            }
        }

        /// <summary>
        /// Validación personalizada para la fecha de salida
        /// Debe ser mayor o igual a la fecha de entrada
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
        /// Evento que maneja el clic en el botón Guardar
        /// Valida y actualiza la reservación
        /// </summary>
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                // Validar capacidad
                int numeroAdultos = Convert.ToInt32(txtNumeroAdultos.Text);
                int numeroNinhos = Convert.ToInt32(txtNumeroNinhos.Text);
                int totalPersonas = numeroAdultos + numeroNinhos;

                if (totalPersonas > capacidadMaxima)
                {
                    MostrarMensaje($"El total de personas ({totalPersonas}) excede la capacidad máxima de la habitación ({capacidadMaxima}).");
                    return;
                }

                // Actualizar la reservación
                if (ActualizarReservacion())
                {
                    bool esEmpleado = Session["esEmpleado"] != null && Convert.ToBoolean(Session["esEmpleado"]);
                    string paginaRetorno = esEmpleado ? "GestionarReservaciones" : "MisReservaciones";
                    Response.Redirect($"~/Pages/Mensaje.aspx?tipo=exito&mensaje=Reservación actualizada exitosamente&pagina={paginaRetorno}");
                }
            }
        }

        /// <summary>
        /// Actualiza la reservación en la base de datos
        /// Utiliza el SP sp_ModificarReservacion
        /// </summary>
        private bool ActualizarReservacion()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["PV_ProyectoFinalConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_ModificarReservacion", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@idReservacion", idReservacion);
                    cmd.Parameters.AddWithValue("@fechaEntrada", Convert.ToDateTime(txtFechaEntrada.Text));
                    cmd.Parameters.AddWithValue("@fechaSalida", Convert.ToDateTime(txtFechaSalida.Text));
                    cmd.Parameters.AddWithValue("@numeroAdultos", Convert.ToInt32(txtNumeroAdultos.Text));
                    cmd.Parameters.AddWithValue("@numeroNinhos", Convert.ToInt32(txtNumeroNinhos.Text));
                    cmd.Parameters.AddWithValue("@idPersonaModificador", Convert.ToInt32(Session["idPersona"]));

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        MostrarMensaje("Error al actualizar la reservación: " + ex.Message);
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// Evento que maneja el clic en el botón Cancelar
        /// Redirige según el tipo de usuario
        /// </summary>
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            RedirigirSegunTipoUsuario();
        }

        /// <summary>
        /// Redirige a la página correspondiente según el tipo de usuario
        /// </summary>
        private void RedirigirSegunTipoUsuario()
        {
            bool esEmpleado = Session["esEmpleado"] != null && Convert.ToBoolean(Session["esEmpleado"]);

            if (esEmpleado)
            {
                Response.Redirect("~/Pages/GestionarReservaciones.aspx");
            }
            else
            {
                Response.Redirect("~/Pages/MisReservaciones.aspx");
            }
        }

        /// <summary>
        /// Muestra un mensaje de error en la página
        /// </summary>
        private void MostrarMensaje(string mensaje)
        {
            lblMensaje.Text = mensaje;
            pnlMensaje.Visible = true;
        }
    }
}