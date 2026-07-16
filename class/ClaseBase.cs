// ✅ BasePage.cs CORREGIDO
using System;
using System.Web.UI;

namespace ProyectoFinal
{
    public class BasePage : Page
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Obtener nombre de página actual
            string paginaActual = Request.Url.AbsolutePath.ToLower();

            // Si NO es Login, validar sesión
            if (!paginaActual.Contains("login.aspx"))
            {
                ValidarSesion();
            }
        }

        private void ValidarSesion()
        {
            // Si no hay sesión, redirigir al login
            if (Session["idPersona"] == null)
            {
                Response.Redirect("~/Pages/Login.aspx");
            }
        }

        // Propiedades de ayuda
        protected int IdUsuario
        {
            get { return Session["idPersona"] != null ? (int)Session["idPersona"] : 0; }
        }

        protected bool EsEmpleado
        {
            get { return Session["esEmpleado"] != null && (bool)Session["esEmpleado"]; }
        }
    }
}