using FigurasGeometricasApi.DTOs;
using FigurasGeometricasApi.Models;

namespace FigurasGeometricasApi.Services
{
    public interface IFiguraService
    {
        Task<IEnumerable<FiguraReadDto>> GetAllAsync();
        Task<FiguraReadDto?> GetByIdAsync(int id);
        Task<FiguraReadDto> CreateAsync(FiguraCreateDto dto);
        Task<bool> UpdateAsync(int id, FiguraUpdateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<(double area, double perimetro)> TotalesAsync();
    }
}
