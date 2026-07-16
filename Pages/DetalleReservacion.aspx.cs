using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProyectoFinalP5.Pages
{
    public partial class DetalleReservacion : System.Web.UI.Page
    {
        private int idReservacion;
        private string estadoReservacion;
        private DateTime fechaEntrada;
        private DateTime fechaSalida;
        private int idPersonaReservacion;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Validar sesión
            if (Session["idPersona"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            // Validar ID por QueryString
            if (!int.TryParse(Request.QueryString["id"], out idReservacion))
            {
                Response.Redirect("~/Pages/MisReservaciones.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarDetalleReservacion();
                CargarBitacora();
                ConfigurarBotones();
            }
        }

        private void CargarDetalleReservacion()
        {
            string cs = ConfigurationManager.ConnectionStrings["PV_ProyectoFinalConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand("sp_ObtenerDetalleReservacion", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idReservacion", idReservacion);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (!reader.Read())
                    {
                        MostrarMensaje("No se encontró la reservación solicitada.");
                        return;
                    }

                    // Validación de permisos
                    idPersonaReservacion = Convert.ToInt32(reader["idPersona"]);
                    bool esEmpleado = Session["esEmpleado"] != null && Convert.ToBoolean(Session["esEmpleado"]);
                    int idSesion = Convert.ToInt32(Session["idPersona"]);

                    if (!esEmpleado && idSesion != idPersonaReservacion)
                    {
                        MostrarMensaje("No tienes permiso para ver esta reservación.");
                        return;
                    }

                    // Llenar Literals
                    litIdReservacion.Text = reader["idReservacion"].ToString();
                    litHotel.Text = reader["nombreHotel"].ToString();
                    litNumeroHabitacion.Text = reader["numeroHabitacion"].ToString();
                    litCliente.Text = reader["nombreCompleto"].ToString();
                    litFechaEntrada.Text = Convert.ToDateTime(reader["fechaEntrada"]).ToString("dd/MM/yyyy");
                    litFechaSalida.Text = Convert.ToDateTime(reader["fechaSalida"]).ToString("dd/MM/yyyy");
                    litTotalDias.Text = reader["totalDiasReservacion"].ToString();
                    litNumeroAdultos.Text = reader["numeroAdultos"].ToString();
                    litNumeroNinhos.Text = reader["numeroNinhos"].ToString();

                    decimal costo = Convert.ToDecimal(reader["costoTotal"]);
                    litCostoTotal.Text = costo.ToString("C", new System.Globalization.CultureInfo("es-CR"));

                    estadoReservacion = reader["estado"].ToString();
                    fechaEntrada = Convert.ToDateTime(reader["fechaEntrada"]);
                    fechaSalida = Convert.ToDateTime(reader["fechaSalida"]);

                    litEstado.Text = GetEstadoDescripcion(estadoReservacion, fechaEntrada, fechaSalida);
                }
                catch (Exception ex)
                {
                    MostrarMensaje("Error al cargar la reservación: " + ex.Message);
                }
            }
        }

        private void CargarBitacora()
        {
            string cs = ConfigurationManager.ConnectionStrings["PV_ProyectoFinalConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand("sp_ObtenerBitacoraPorReservacion", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idReservacion", idReservacion);

                try
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvBitacora.DataSource = dt;
                    gvBitacora.DataBind();
                }
                catch (Exception ex)
                {
                    MostrarMensaje("Error al cargar la bitácora: " + ex.Message);
                }
            }
        }

        private void ConfigurarBotones()
        {
            bool esEmpleado = Session["esEmpleado"] != null && Convert.ToBoolean(Session["esEmpleado"]);
            DateTime hoy = DateTime.Now;

            // Editar
            btnEditar.Visible =
                estadoReservacion == "A" &&
                ((esEmpleado && fechaSalida > hoy) || (!esEmpleado && fechaEntrada > hoy));

            // Cancelar
            btnCancelar.Visible = (estadoReservacion == "A" && fechaEntrada > hoy);
        }

        protected void btnEditar_Click(object sender, EventArgs e)
        {
            Response.Redirect($"~/Pages/EditarReservacion.aspx?id={idReservacion}");
        }

        protected void btnCancelarReservacion_Click(object sender, EventArgs e)
        {
            if (!CancelarReservacion()) return;

            bool esEmpleado = Session["esEmpleado"] != null && Convert.ToBoolean(Session["esEmpleado"]);
            string destino = esEmpleado ? "GestionarReservaciones" : "MisReservaciones";

            Response.Redirect($"~/Pages/Mensaje.aspx?tipo=exito&mensaje=Reservación cancelada exitosamente&pagina={destino}");
        }

        private bool CancelarReservacion()
        {
            string cs = ConfigurationManager.ConnectionStrings["PV_ProyectoFinalConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand("sp_CancelarReservacion", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idReservacion", idReservacion);
                cmd.Parameters.AddWithValue("@idPersonaCancelador", Convert.ToInt32(Session["idPersona"]));

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    MostrarMensaje("Error al cancelar la reservación: " + ex.Message);
                    return false;
                }
            }
        }

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            bool esEmpleado = Session["esEmpleado"] != null && Convert.ToBoolean(Session["esEmpleado"]);

            Response.Redirect(esEmpleado
                ? "~/Pages/GestionarReservaciones.aspx"
                : "~/Pages/MisReservaciones.aspx");
        }

        private string GetEstadoDescripcion(string estado, DateTime entrada, DateTime salida)
        {
            DateTime hoy = DateTime.Now;

            if (estado == "I") return "Cancelada";

            if (estado == "A")
            {
                if (salida < hoy) return "Finalizada";
                if (entrada <= hoy) return "En proceso";
                return "En espera";
            }

            return "Desconocido";
        }

        private void MostrarMensaje(string mensaje)
        {
            lblMensaje.Text = mensaje;
            pnlMensaje.Visible = true;
        }
    }
}
