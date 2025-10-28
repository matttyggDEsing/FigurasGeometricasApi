using System.ComponentModel.DataAnnotations.Schema;

namespace FigurasGeometricasApi.Models
{
    public class Circulo : Figura
    {
        public double Radio { get; set; }

        public Circulo() { }

        public Circulo(string nombre, double radio)
        {
            Nombre = nombre;
            Radio = radio;
        }

        public override double CalcularArea() => Math.PI * Radio * Radio;
        public override double CalcularPerimetro() => 2 * Math.PI * Radio;
    }
}
