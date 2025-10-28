using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FigurasGeometricasApi.Models
{
    // Entidad base para EF Core (TPH inheritance)
    public abstract class Figura
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; } = null!;

        public abstract double CalcularArea();
        public abstract double CalcularPerimetro();

        public virtual void CargarDefaults() { /* para inicializar si hace falta */ }
    }
}
