using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;

namespace ProyectoFinalP5.Pages
{
    /// <summary>
    /// Página para editar habitaciones existentes
    /// Solo accesible para empleados
    /// Implementa el requerimiento RF-006
    /// </summary>
    public partial class EditarHabitacion : System.Web.UI.Page
    {
        private int idHabitacion;
        private int idHotel;
        private string estadoHabitacion;

        /// <summary>
        /// Evento que se ejecuta al cargar la página
        /// Valida el acceso y carga los datos de la habitación
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

            // Obtener el ID de la habitación del QueryString
            if (Request.QueryString["id"] == null || !int.TryParse(Request.QueryString["id"], out idHabitacion))
            {
                Response.Redirect("~/Pages/ListaHabitaciones.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarDatosHabitacion();
                ValidarAccesoEdicion();
            }
        }

        /// <summary>
        /// Carga los datos de la habitación en los controles del formulario
        /// Utiliza el SP sp_ObtenerDetalleHabitacion
        /// </summary>
        private void CargarDatosHabitacion()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["PV_ProyectoFinalConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_ObtenerDetalleHabitacion", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idHabitacion", idHabitacion);

                    try
                    {
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            txtHotel.Text = reader["nombreHotel"].ToString();
                            txtNumeroHabitacion.Text = reader["numeroHabitacion"].ToString();
                            txtCapacidadMaxima.Text = reader["capacidadMaxima"].ToString();
                            txtDescripcion.Text = reader["descripcion"].ToString();

                            idHotel = Convert.ToInt32(reader["idHotel"]);
                            estadoHabitacion = reader["estado"].ToString();
                        }
                        else
                        {
                            reader.Close();
                            Response.Redirect("~/Pages/ListaHabitaciones.aspx");
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
        /// Valida que la habitación pueda ser editada
        /// según las reglas de negocio del requerimiento RF-006
        /// </summary>
        private void ValidarAccesoEdicion()
        {
            // Si la habitación está inactiva, mostrar mensaje y deshabilitar edición
            if (estadoHabitacion == "I")
            {
                Response.Redirect("~/Pages/Mensaje.aspx?tipo=error&mensaje=La habitación no puede ser modificada porque está inactiva&pagina=ListaHabitaciones");
                return;
            }

            // Verificar si tiene reservaciones pendientes
            if (TieneReservacionesPendientes())
            {
                Response.Redirect("~/Pages/Mensaje.aspx?tipo=error&mensaje=La habitación no puede ser modificada porque tiene reservaciones en proceso o en espera&pagina=ListaHabitaciones");
                return;
            }
        }

        /// <summary>
        /// Verifica si la habitación tiene reservaciones activas pendientes
        /// Utiliza el SP sp_VerificarReservacionesPendientes
        /// </summary>
        private bool TieneReservacionesPendientes()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["PV_ProyectoFinalConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_VerificarReservacionesPendientes", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idHabitacion", idHabitacion);

                    SqlParameter paramTiene = new SqlParameter("@tieneReservaciones", SqlDbType.Bit);
                    paramTiene.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(paramTiene);

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        return Convert.ToBoolean(paramTiene.Value);
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// Evento que maneja el clic en el botón Guardar
        /// Valida y actualiza la habitación
        /// </summary>
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                // Validar que el número de habitación no esté duplicado
                if (ValidarNumeroHabitacionDuplicado())
                {
                    MostrarMensaje("El número de habitación ya existe en este hotel. Por favor, ingrese un número diferente.");
                    return;
                }

                // Actualizar la habitación
                if (ActualizarHabitacion())
                {
                    Response.Redirect("~/Pages/Mensaje.aspx?tipo=exito&mensaje=Habitación actualizada exitosamente&pagina=ListaHabitaciones");
                }
            }
        }

        /// <summary>
        /// Valida que el número de habitación no esté duplicado en el hotel
        /// (excepto la habitación actual)
        /// </summary>
        private bool ValidarNumeroHabitacionDuplicado()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["PV_ProyectoFinalConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT COUNT(*) 
                                FROM Habitacion 
                                WHERE idHotel = @idHotel 
                                AND numeroHabitacion = @numeroHabitacion
                                AND idHabitacion <> @idHabitacion";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@idHotel", idHotel);
                    cmd.Parameters.AddWithValue("@numeroHabitacion", txtNumeroHabitacion.Text.Trim());
                    cmd.Parameters.AddWithValue("@idHabitacion", idHabitacion);

                    try
                    {
                        conn.Open();
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// Actualiza la habitación en la base de datos
        /// Utiliza el SP sp_ModificarHabitacion
        /// </summary>
        private bool ActualizarHabitacion()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["PV_ProyectoFinalConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_ModificarHabitacion", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@idHabitacion", idHabitacion);
                    cmd.Parameters.AddWithValue("@numeroHabitacion", txtNumeroHabitacion.Text.Trim());
                    cmd.Parameters.AddWithValue("@capacidadMaxima", Convert.ToInt32(txtCapacidadMaxima.Text));
                    cmd.Parameters.AddWithValue("@descripcion", txtDescripcion.Text.Trim());

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        MostrarMensaje("Error al actualizar la habitación: " + ex.Message);
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// Evento que maneja el clic en el botón Inactivar
        /// Inactiva la habitación en la base de datos
        /// </summary>
        protected void btnInactivar_Click(object sender, EventArgs e)
        {
            if (InactivarHabitacion())
            {
                Response.Redirect("~/Pages/Mensaje.aspx?tipo=exito&mensaje=Habitación inactivada exitosamente&pagina=ListaHabitaciones");
            }
        }

        /// <summary>
        /// Inactiva la habitación en la base de datos
        /// Utiliza el SP sp_InactivarHabitacion
        /// </summary>
        private bool InactivarHabitacion()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["PV_ProyectoFinalConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_InactivarHabitacion", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idHabitacion", idHabitacion);

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        MostrarMensaje("Error al inactivar la habitación: " + ex.Message);
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// Evento que maneja el clic en el botón Cancelar
        /// Redirige a la lista de habitaciones
        /// </summary>
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Pages/ListaHabitaciones.aspx");
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