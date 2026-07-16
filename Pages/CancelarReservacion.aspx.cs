using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace ProyectoFinalP5.Pages
{
    public partial class CancelarReservacion : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["idPersona"] == null)
            {
                Response.Redirect("~/Pages/Login.aspx");
                return;
            }

            if (string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                Response.Redirect(GetPaginaRegreso());
                return;
            }

            int id = Convert.ToInt32(Request.QueryString["id"]);
            ProcesarCancelacion(id);
        }

        private void ProcesarCancelacion(int idReservacion)
        {
            try
            {
                // Validar reglas: obtener reservación
                string cs = ConfigurationManager.ConnectionStrings["PvProyectoFinalDB"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(cs))
                using (SqlCommand cmd = new SqlCommand("SELECT estado, fechaEntrada FROM Reservacion WHERE idReservacion = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", idReservacion);
                    conn.Open();
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        if (!r.Read())
                        {
                            Response.Redirect(GetPaginaRegreso());
                            return;
                        }

                        string estado = r["estado"].ToString();
                        DateTime fechaEntrada = Convert.ToDateTime(r["fechaEntrada"]);

                        DateTime ahora = DateTime.Now;

                        if (estado != "A")
                        {
                            // No se puede cancelar
                            Response.Redirect(GetPaginaRegreso());
                            return;
                        }

                        if (fechaEntrada <= ahora && Session["esEmpleado"] != null && !(bool)Session["esEmpleado"]) // cliente
                        {
                            // Cliente no puede cancelar si ya inició o está en proceso
                            Response.Redirect(GetPaginaRegreso());
                            return;
                        }
                    }
                }

                // Actualizar estado a I y registrar bitácora usando sp_CancelarReservacion
                using (SqlConnection conn2 = new SqlConnection(cs))
                using (SqlCommand cmd2 = new SqlCommand("sp_CancelarReservacion", conn2))
                {
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Parameters.AddWithValue("@idReservacion", idReservacion);
                    cmd2.Parameters.AddWithValue("@idPersonaCancelador", Convert.ToInt32(Session["idPersona"]));

                    conn2.Open();
                    cmd2.ExecuteNonQuery();
                }

                // Redirigir a pantalla de éxito
                Response.Redirect("~/Pages/OperacionExitosa.aspx?op=cancelar", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                // Log error
                System.Diagnostics.Debug.WriteLine("Error cancelar: " + ex.Message);
                Response.Redirect(GetPaginaRegreso());
            }
        }

        private string GetPaginaRegreso()
        {
            bool esEmpleado = Session["esEmpleado"] != null && Convert.ToBoolean(Session["esEmpleado"]);
            return esEmpleado ? "~/Pages/GestionarReservaciones.aspx" : "~/Pages/MisReservaciones.aspx";
        }
    }
}