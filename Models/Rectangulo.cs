namespace FigurasGeometricasApi.Models
{
    public class Rectangulo : Figura
    {
        public double Base { get; set; }
        public double Altura { get; set; }

        public Rectangulo() { }

        public Rectangulo(string nombre, double b, double h)
        {
            Nombre = nombre;
            Base = b;
            Altura = h;
        }

        public override double CalcularArea() => Base * Altura;
        public override double CalcularPerimetro() => 2 * (Base + Altura);
    }
}
