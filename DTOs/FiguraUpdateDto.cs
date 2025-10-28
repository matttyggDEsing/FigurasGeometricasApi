using System.ComponentModel.DataAnnotations;

namespace FigurasGeometricasApi.DTOs
{
    public class FiguraUpdateDto
    {
        [Required]
        [MaxLength(120)]
        public string Nombre { get; set; } = null!;

        // Campos dependientes del tipo; se validan en el servicio
        public double? Radio { get; set; }
        public double? Base { get; set; }
        public double? Altura { get; set; }
        public double? LadoA { get; set; }
        public double? LadoB { get; set; }
        public double? LadoC { get; set; }
    }
}
