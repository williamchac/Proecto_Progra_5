using System;

namespace ProyectoFinal
{
    public class ClaseReservacion
    {
        public int IdReservacion { get; set; }
        public string Cliente { get; set; } // Solo para empleados
        public string Hotel { get; set; }
        public DateTime FechaEntrada { get; set; }
        public DateTime FechaSalida { get; set; }
        public decimal CostoTotal { get; set; }
        public string Estado { get; set; } // "A" o "I" de BD

        // Propiedades calculadas
        public string EstadoDescripcion
        {
            get { return GetEstadoDescripcion(this.Estado, this.FechaEntrada, this.FechaSalida); }
        }

        public string EstadoCssClass
        {
            get { return GetEstadoCssClass(this.Estado, this.FechaEntrada, this.FechaSalida); }
        }

        // Métodos estáticos auxiliares
        public static string GetEstadoDescripcion(string estado, DateTime fechaEntrada, DateTime fechaSalida)
        {
            if (estado == "I")
                return "Cancelada";

            DateTime ahora = DateTime.Now;

            if (estado == "A")
            {
                if (fechaSalida < ahora)
                    return "Finalizada";
                else if (fechaEntrada <= ahora)
                    return "En proceso";
                else
                    return "En espera";
            }

            return "Desconocido";
        }

        public static string GetEstadoCssClass(string estado, DateTime fechaEntrada, DateTime fechaSalida)
        {
            string descripcion = GetEstadoDescripcion(estado, fechaEntrada, fechaSalida);

            switch (descripcion)
            {
                case "En espera":
                    return "badge bg-primary";
                case "En proceso":
                    return "badge bg-warning text-dark";
                case "Finalizada":
                    return "badge bg-success";
                case "Cancelada":
                    return "badge bg-danger";
                default:
                    return "badge bg-secondary";
            }
        }
    }
}