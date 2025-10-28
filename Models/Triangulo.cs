namespace FigurasGeometricasApi.Models
{
    public class Triangulo : Figura
    {
        public double LadoA { get; set; }
        public double LadoB { get; set; }
        public double LadoC { get; set; }

        public Triangulo() { }

        public Triangulo(string nombre, double a, double b, double c)
        {
            Nombre = nombre;
            LadoA = a;
            LadoB = b;
            LadoC = c;
        }

        public override double CalcularPerimetro() => LadoA + LadoB + LadoC;

        public override double CalcularArea()
        {
            if (LadoA + LadoB <= LadoC || LadoA + LadoC <= LadoB || LadoB + LadoC <= LadoA)
                return double.NaN;

            double s = CalcularPerimetro() / 2.0;
            return Math.Sqrt(s * (s - LadoA) * (s - LadoB) * (s - LadoC));
        }

        public string TipoTriangulo()
        {
            if (LadoA == LadoB && LadoB == LadoC) return "Equilátero";
            if (LadoA == LadoB || LadoA == LadoC || LadoB == LadoC) return "Isósceles";
            return "Escaleno";
        }
    }
}
