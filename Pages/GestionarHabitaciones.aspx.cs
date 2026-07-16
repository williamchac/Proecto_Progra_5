using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProyectoFinalP5.Pages
{
    public partial class GestionarHabitaciones : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["idPersona"] == null)
            {
                Response.Redirect("~/Pages/Login.aspx");
                return;
            }

            bool esEmpleado = Session["esEmpleado"] != null && Convert.ToBoolean(Session["esEmpleado"]);
            if (!esEmpleado)
            {
                Response.Redirect("~/Pages/MisReservaciones.aspx");
                return;
            }

            if (!IsPostBack)
                CargarHabitaciones();
        }

        private void CargarHabitaciones()
        {
            try
            {
                string cs = ConfigurationManager.ConnectionStrings["PvProyectoFinalDB"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(cs))
                using (SqlCommand cmd = new SqlCommand("sp_ObtenerTodasHabitaciones", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    DataTable dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());

                    gvHabitaciones.DataSource = dt;
                    gvHabitaciones.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al cargar habitaciones: " + ex.Message;
                pnlMensaje.Visible = true;
            }
        }

        protected void btnCrear_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Pages/CrearHabitacion.aspx");
        }

        protected void lnkEditar_Click(object sender, EventArgs e)
        {
            var btn = (LinkButton)sender;
            int id = Convert.ToInt32(btn.CommandArgument);
            Response.Redirect($"~/Pages/EditarHabitacion.aspx?id={id}");
        }

        protected void lnkInactivar_Click(object sender, EventArgs e)
        {
            var btn = (LinkButton)sender;
            int id = Convert.ToInt32(btn.CommandArgument);

            // Verificar reservaciones activas
            try
            {
                string cs = ConfigurationManager.ConnectionStrings["PvProyectoFinalDB"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(cs))
                using (SqlCommand cmd = new SqlCommand("sp_VerificarReservacionesPendientes", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idHabitacion", id);
                    SqlParameter outParam = new SqlParameter("@tieneReservaciones", SqlDbType.Bit) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outParam);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    bool tiene = outParam.Value != DBNull.Value && Convert.ToBoolean(outParam.Value);
                    if (tiene)
                    {
                        lblMensaje.Text = "No se puede inactivar la habitación porque tiene reservaciones activas.";
                        pnlMensaje.Visible = true;
                        return;
                    }
                }

                // Inactivar
                using (SqlConnection conn2 = new SqlConnection(cs))
                using (SqlCommand cmd2 = new SqlCommand("sp_InactivarHabitacion", conn2))
                {
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Parameters.AddWithValue("@idHabitacion", id);
                    conn2.Open();
                    cmd2.ExecuteNonQuery();
                }

                // Refrescar
                CargarHabitaciones();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al inactivar: " + ex.Message;
                pnlMensaje.Visible = true;
            }
        }
    }
}