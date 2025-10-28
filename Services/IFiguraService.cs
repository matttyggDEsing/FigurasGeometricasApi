using FigurasGeometricasApi.Data;
using FigurasGeometricasApi.DTOs;
using FigurasGeometricasApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FigurasGeometricasApi.Services
{
    public class FiguraService : IFiguraService
    {
        private readonly FigurasContext _context;

        public FiguraService(FigurasContext context)
        {
            _context = context;
        }

        public async Task<FiguraReadDto> CreateAsync(FiguraCreateDto dto)
        {
            Figura entidad = dto.Tipo.ToLower() switch
            {
                "circulo" => new Circulo(dto.Nombre, dto.Radio ?? 0),
                "rectangulo" => new Rectangulo(dto.Nombre, dto.Base ?? 0, dto.Altura ?? 0),
                "triangulo" => new Triangulo(dto.Nombre, dto.LadoA ?? 0, dto.LadoB ?? 0, dto.LadoC ?? 0),
                _ => throw new ArgumentException("Tipo de figura no soportado: " + dto.Tipo)
            };

            // Validaciones básicas
            if (entidad is Circulo c && c.Radio <= 0) throw new ArgumentException("Radio debe ser > 0");
            if (entidad is Rectangulo r && (r.Base <= 0 || r.Altura <= 0)) throw new ArgumentException("Base y Altura deben ser > 0");
            if (entidad is Triangulo t && (t.LadoA <= 0 || t.LadoB <= 0 || t.LadoC <= 0)) throw new ArgumentException("Lados deben ser > 0");

            _context.Figuras.Add(entidad);
            await _context.SaveChangesAsync();
            return MapToRead(entidad);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var f = await _context.Figuras.FindAsync(id);
            if (f == null) return false;
            _context.Figuras.Remove(f);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<FiguraReadDto>> GetAllAsync()
        {
            var list = await _context.Figuras.ToListAsync();
            return list.Select(MapToRead);
        }

        public async Task<FiguraReadDto?> GetByIdAsync(int id)
        {
            var f = await _context.Figuras.FindAsync(id);
            return f is null ? null : MapToRead(f);
        }

        public async Task<(double area, double perimetro)> TotalesAsync()
        {
            var list = await _context.Figuras.ToListAsync();
            double area = list.Sum(x => x.CalcularArea());
            double perim = list.Sum(x => x.CalcularPerimetro());
            return (area, perim);
        }

        public async Task<bool> UpdateAsync(int id, FiguraUpdateDto dto)
        {
            var f = await _context.Figuras.FindAsync(id);
            if (f == null) return false;

            f.Nombre = dto.Nombre;

            switch (f)
            {
                case Circulo c:
                    if (dto.Radio is null || dto.Radio <= 0) throw new ArgumentException("Radio inválido");
                    c.Radio = dto.Radio.Value;
                    break;
                case Rectangulo r:
                    if (dto.Base is null || dto.Base <= 0 || dto.Altura is null || dto.Altura <= 0)
                        throw new ArgumentException("Base/Altura inválidas");
                    r.Base = dto.Base.Value;
                    r.Altura = dto.Altura.Value;
                    break;
                case Triangulo t:
                    if (dto.LadoA is null || dto.LadoB is null || dto.LadoC is null)
                        throw new ArgumentException("Faltan lados");
                    t.LadoA = dto.LadoA.Value;
                    t.LadoB = dto.LadoB.Value;
                    t.LadoC = dto.LadoC.Value;
                    // valida desigualdad triangular
                    if (t.LadoA + t.LadoB <= t.LadoC || t.LadoA + t.LadoC <= t.LadoB || t.LadoB + t.LadoC <= t.LadoA)
                        throw new ArgumentException("Lados no forman triángulo válido");
                    break;
            }

            _context.Figuras.Update(f);
            await _context.SaveChangesAsync();
            return true;
        }

        private static FiguraReadDto MapToRead(Figura f)
        {
            var dto = new FiguraReadDto
            {
                Id = f.Id,
                Nombre = f.Nombre,
                Tipo = f.GetType().Name,
                Area = f.CalcularArea(),
                Perimetro = f.CalcularPerimetro()
            };

            switch (f)
            {
                case Circulo c:
                    dto.Radio = c.Radio;
                    break;
                case Rectangulo r:
                    dto.Base = r.Base;
                    dto.Altura = r.Altura;
                    break;
                case Triangulo t:
                    dto.LadoA = t.LadoA;
                    dto.LadoB = t.LadoB;
                    dto.LadoC = t.LadoC;
                    break;
            }

            return dto;
        }
    }
}
