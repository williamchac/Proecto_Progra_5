using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;

namespace ProyectoFinalP5.Pages
{
    /// <summary>
    /// Página para crear nuevas habitaciones
    /// Solo accesible para empleados
    /// Implementa el requerimiento RF-006
    /// </summary>
    public partial class CrearHabitacion : System.Web.UI.Page
    {
        /// <summary>
        /// Evento que se ejecuta al cargar la página
        /// Verifica que el usuario sea empleado y carga los hoteles
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
                CargarHoteles();
            }
        }

        /// <summary>
        /// Carga la lista de hoteles desde la base de datos
        /// Utiliza el SP sp_ObtenerHoteles
        /// </summary>
        private void CargarHoteles()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["PV_ProyectoFinalConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_ObtenerHoteles", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    try
                    {
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        ddlHotel.Items.Clear();
                        ddlHotel.Items.Add(new System.Web.UI.WebControls.ListItem("Seleccione un hotel", ""));

                        while (reader.Read())
                        {
                            string nombre = reader["nombre"].ToString();
                            string idHotel = reader["idHotel"].ToString();
                            ddlHotel.Items.Add(new System.Web.UI.WebControls.ListItem(nombre, idHotel));
                        }

                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        MostrarMensaje("Error al cargar hoteles: " + ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Evento que maneja el clic en el botón Guardar
        /// Valida y guarda la nueva habitación
        /// </summary>
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                // Validar que el número de habitación no esté duplicado
                if (ValidarNumeroHabitacionDuplicado())
                {
                    MostrarMensaje("El número de habitación ya existe en el hotel seleccionado. Por favor, ingrese un número diferente.");
                    return;
                }

                // Guardar la habitación
                if (GuardarHabitacion())
                {
                    Response.Redirect("~/Pages/Mensaje.aspx?tipo=exito&mensaje=Habitación creada exitosamente&pagina=ListaHabitaciones");
                }
            }
        }

        /// <summary>
        /// Valida que el número de habitación no esté duplicado en el hotel
        /// </summary>
        private bool ValidarNumeroHabitacionDuplicado()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["PV_ProyectoFinalConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT COUNT(*) 
                                FROM Habitacion 
                                WHERE idHotel = @idHotel 
                                AND numeroHabitacion = @numeroHabitacion";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@idHotel", Convert.ToInt32(ddlHotel.SelectedValue));
                    cmd.Parameters.AddWithValue("@numeroHabitacion", txtNumeroHabitacion.Text.Trim());

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
        /// Guarda la nueva habitación en la base de datos
        /// Utiliza el SP sp_CrearHabitacion
        /// </summary>
        private bool GuardarHabitacion()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["PV_ProyectoFinalConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_CrearHabitacion", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@idHotel", Convert.ToInt32(ddlHotel.SelectedValue));
                    cmd.Parameters.AddWithValue("@numeroHabitacion", txtNumeroHabitacion.Text.Trim());
                    cmd.Parameters.AddWithValue("@capacidadMaxima", Convert.ToInt32(txtCapacidadMaxima.Text));
                    cmd.Parameters.AddWithValue("@descripcion", txtDescripcion.Text.Trim());

                    SqlParameter paramIdHabitacion = new SqlParameter("@idHabitacionCreada", SqlDbType.Int);
                    paramIdHabitacion.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(paramIdHabitacion);

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        MostrarMensaje("Error al guardar la habitación: " + ex.Message);
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