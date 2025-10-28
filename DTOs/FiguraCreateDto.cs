using System.ComponentModel.DataAnnotations;

namespace FigurasGeometricasApi.DTOs
{
    // DTO para crear; 'Tipo' define la subclase esperada: "Circulo", "Rectangulo", "Triangulo"
    public class FiguraCreateDto
    {
        [Required]
        public string Tipo { get; set; } = null!;

        [Required]
        [MaxLength(120)]
        public string Nombre { get; set; } = null!;

        // Opcionales según Tipo:
        public double? Radio { get; set; }
        public double? Base { get; set; }
        public double? Altura { get; set; }
        public double? LadoA { get; set; }
        public double? LadoB { get; set; }
        public double? LadoC { get; set; }
    }
}
