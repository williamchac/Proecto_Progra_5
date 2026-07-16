using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProyectoFinalP5.Pages
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Si ya existe una sesión activa, redirigir según el tipo de usuario
                if (Session["idPersona"] != null)
                {
                    bool esEmpleado = Session["esEmpleado"] != null && Convert.ToBoolean(Session["esEmpleado"]);

                    if (esEmpleado)
                    {
                        Response.Redirect("~/Pages/GestionarReservaciones.aspx", false);
                        // no CompleteRequest here to keep compatibility
                    }
                    else
                    {
                        Response.Redirect("~/Pages/MisReservaciones.aspx", false);
                    }
                }
            }
        }

        protected void btnIngresar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            var txtEmail = FindControlRecursive(this, "txtEmail") as TextBox;
            var txtClave = FindControlRecursive(this, "txtClave") as TextBox;
            var pnl = FindControlRecursive(this, "pnlMensaje") as Panel;
            var lbl = FindControlRecursive(this, "lblMensaje") as Label;

            string email = txtEmail != null ? txtEmail.Text.Trim() : string.Empty;
            string clave = txtClave != null ? txtClave.Text.Trim() : string.Empty;

            // Validar credenciales
            if (ValidarCredenciales(email, clave))
            {
                // Redirigir según el tipo de usuario
                bool esEmpleado = Session["esEmpleado"] != null && Convert.ToBoolean(Session["esEmpleado"]);

                if (esEmpleado)
                {
                    Response.Redirect("~/Pages/GestionarReservaciones.aspx", false);
                }
                else
                {
                    Response.Redirect("~/Pages/MisReservaciones.aspx", false);
                }
            }
            else
            {
                if (lbl != null && pnl != null)
                {
                    lbl.Text = "Las credenciales ingresadas no son correctas. Por favor, verifique su email y contraseña.";
                    pnl.Visible = true;
                }
            }
        }

        private bool ValidarCredenciales(string email, string clave)
        {
            var cs = ConfigurationManager.ConnectionStrings["PvProyectoFinalDB"];
            if (cs == null)
            {
                MostrarMensaje("Cadena de conexión no configurada.");
                return false;
            }

            string connectionString = cs.ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_ValidarCredenciales", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@clave", clave);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // Crear variables de sesión
                        Session["idPersona"] = Convert.ToInt32(reader["idPersona"]);
                        Session["nombreCompleto"] = reader["nombreCompleto"].ToString();
                        Session["email"] = reader["email"].ToString();
                        Session["esEmpleado"] = Convert.ToBoolean(reader["esEmpleado"]);

                        reader.Close();
                        return true;
                    }

                    reader.Close();
                    return false;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error ValidarCredenciales: " + ex.Message);
                    MostrarMensaje("Error al validar credenciales. Consulte al administrador.");
                    return false;
                }
            }
        }

        private void MostrarMensaje(string mensaje)
        {
            var pnl = FindControlRecursive(this, "pnlMensaje") as Panel;
            var lbl = FindControlRecursive(this, "lblMensaje") as Label;
            if (lbl != null && pnl != null)
            {
                lbl.Text = mensaje;
                pnl.Visible = true;
            }
        }

        private Control FindControlRecursive(Control root, string id)
        {
            if (root == null) return null;
            var c = root.FindControl(id);
            if (c != null) return c;
            foreach (Control child in root.Controls)
            {
                var result = FindControlRecursive(child, id);
                if (result != null) return result;
            }
            return null;
        }
    }
}