using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace ProyectoFinalP5
{
    /// <summary>
    /// Página maestra del sistema que centraliza la estructura gráfica
    /// Incluye encabezado, menú de navegación y pie de página
    /// RF-001 f), g), h)
    /// </summary>
    public partial class Site : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ConfigurarMenu();
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

        private void ConfigurarMenu()
        {
            try
            {
                var lblNombre = FindControlRecursive(this, "lblNombreUsuario") as Label;
                var liMis = FindControlRecursive(this, "liMisReservaciones") as HtmlGenericControl;
                var liGestionarRes = FindControlRecursive(this, "liGestionarReservaciones") as HtmlGenericControl;
                var liGestionarHab = FindControlRecursive(this, "liGestionarHabitaciones") as HtmlGenericControl;
                var lnkCerrar = FindControlRecursive(this, "lnkCerrarSesion") as LinkButton;

                if (lblNombre == null || liMis == null || liGestionarRes == null || liGestionarHab == null || lnkCerrar == null)
                {
                    return;
                }

                if (Session["idPersona"] != null)
                {
                    if (Session["nombreCompleto"] != null)
                    {
                        lblNombre.Text = Session["nombreCompleto"].ToString();
                    }

                    bool esEmpleado = Session["esEmpleado"] != null && (bool)Session["esEmpleado"];

                    liMis.Visible = true;
                    liGestionarRes.Visible = esEmpleado;
                    liGestionarHab.Visible = esEmpleado;
                    lnkCerrar.Visible = true;
                }
                else
                {
                    liMis.Visible = false;
                    liGestionarRes.Visible = false;
                    liGestionarHab.Visible = false;
                    lnkCerrar.Visible = false;
                    lblNombre.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en ConfigurarMenu: {ex.Message}");
            }
        }

        /// <summary>
        /// Manejadores para los LinkButtons del menú
        /// Se agregan para evitar errores de compilación cuando OnClick está definido en Site.Master
        /// </summary>
        protected void lnkMisReservaciones_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("~/Pages/MisReservaciones.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error al navegar a MisReservaciones: " + ex.Message);
            }
        }

        protected void lnkGestionarReservaciones_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("~/Pages/GestionarReservaciones.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error al navegar a GestionarReservaciones: " + ex.Message);
            }
        }

        protected void lnkGestionarHabitaciones_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("~/Pages/GestionarHabitaciones.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error al navegar a GestionarHabitaciones: " + ex.Message);
            }
        }

        protected void lnkCerrarSesion_Click(object sender, EventArgs e)
        {
            try
            {
                Session.Clear();
                Session.Abandon();

                Response.Redirect("~/Pages/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cerrar sesión: {ex.Message}");
            }
        }

        #region Métodos auxiliares

        public bool ValidarSesion()
        {
            return Session["idPersona"] != null;
        }

        public int ObtenerIdPersona()
        {
            if (Session["idPersona"] != null)
            {
                return Convert.ToInt32(Session["idPersona"]);
            }
            return 0;
        }

        public string ObtenerNombreCompleto()
        {
            if (Session["nombreCompleto"] != null)
            {
                return Session["nombreCompleto"].ToString();
            }
            return string.Empty;
        }

        public bool EsEmpleado()
        {
            return Session["esEmpleado"] != null && (bool)Session["esEmpleado"];
        }

        #endregion
    }
}