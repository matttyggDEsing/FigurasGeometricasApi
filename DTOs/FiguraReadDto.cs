namespace FigurasGeometricasApi.DTOs
{
    public class FiguraReadDto
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public double Area { get; set; }
        public double Perimetro { get; set; }

        // Campos opcionales según tipo:
        public double? Radio { get; set; }
        public double? Base { get; set; }
        public double? Altura { get; set; }
        public double? LadoA { get; set; }
        public double? LadoB { get; set; }
        public double? LadoC { get; set; }
    }
}
