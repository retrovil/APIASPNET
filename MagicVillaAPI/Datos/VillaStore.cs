using MagicVillaAPI.Models.Dto;

namespace MagicVillaAPI.Datos
{
    public static class VillaStore
    {
        public static List<VillaDto> villaList = new List<VillaDto>()
        {
            new() {Id=1, Nombre="Visita la piscina", Ocupantes = 3, MetrosCuadrados = 50},
            new() {Id=2, Nombre="Visita a la playa", Ocupantes = 4, MetrosCuadrados = 80}
        };
    }
}
