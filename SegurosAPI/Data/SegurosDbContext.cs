using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SegurosAPI.Models;

namespace SegurosAPI.Data
{
    public class SegurosDbContext : DbContext
    {
        public SegurosDbContext(DbContextOptions<SegurosDbContext> options) : base(options) { }

        #region Tablas Base de datos

        public DbSet<PolizaDTO> Polizas { get; set; }
        public DbSet<TipoPolizasDTO> TipoPolizas { get; set; }
        public DbSet<UsuariosDTO> Usuarios { get; set; }
        public DbSet<spCotizacion_polizasUsuarioDTO> spCotizacion_polizasUsuarioDTO { get; set; }
        public DbSet<SiniestrosDTO> Siniestro { get; set; }

        #endregion

        #region Metodos Polizas

        // Método para obtener pólizas mediante el procedimiento almacenado
        public async Task<List<PolizaDTO>> Contultar_PolizasAsync()
        {
            return await Polizas.FromSqlInterpolated($"EXEC spContultar_Polizas").ToListAsync();
        }

        public async Task<bool> Polizas_CrearPoliza(PolizaDTO Polizas)
        {
            // Crear el parámetro de salida
            var resultadoParam = new SqlParameter("@Resultado", System.Data.SqlDbType.Bit)
            {
                Direction = System.Data.ParameterDirection.Output
            };

            // Ejecutar el procedimiento almacenado
            await Database.ExecuteSqlRawAsync(
                "EXEC spPolizas_CrearPoliza @UsuarioId, @TipoPolizaId, @FechaAdquisicion, @CostoPago, @Estado, @Resultado OUTPUT",
                new SqlParameter("@UsuarioId", Polizas.UsuarioId),
                new SqlParameter("@TipoPolizaId", Polizas.TipoPolizaId),
                new SqlParameter("@FechaAdquisicion", Polizas.FechaAdquisicion),
                new SqlParameter("@CostoPago", Polizas.CostoPago),
                new SqlParameter("@Estado", Polizas.Estado),
                resultadoParam
            );

            // Retornar el valor del parámetro de salida
            return (bool)resultadoParam.Value;
        }

        public async Task<bool> Polizas_PolizaPagar(PolizaDTO Polizas)
        {
            // Crear el parámetro de salida
            var resultadoParam = new SqlParameter("@Resultado", System.Data.SqlDbType.Bit)
            {
                Direction = System.Data.ParameterDirection.Output
            };

            // Ejecutar el procedimiento almacenado
            await Database.ExecuteSqlRawAsync(
                "EXEC spPolizas_PolizaPagar @PolizaId, @Resultado OUTPUT",
                new SqlParameter("@PolizaId", Polizas.PolizaId),                
                resultadoParam
            );

            // Retornar el valor del parámetro de salida
            return (bool)resultadoParam.Value;
        }

        public async Task<bool> Polizas_PolizaNoAdquirir(PolizaDTO Polizas)
        {
            // Crear el parámetro de salida
            var resultadoParam = new SqlParameter("@Resultado", System.Data.SqlDbType.Bit)
            {
                Direction = System.Data.ParameterDirection.Output
            };

            // Ejecutar el procedimiento almacenado
            await Database.ExecuteSqlRawAsync(
                "EXEC spPolizas_PolizaNoAdquirir @PolizaId, @Resultado OUTPUT",
                new SqlParameter("@PolizaId", Polizas.PolizaId),
                resultadoParam
            );

            // Retornar el valor del parámetro de salida
            return (bool)resultadoParam.Value;
        }




        #endregion

        #region TipoPolizas
        public async Task<List<TipoPolizasDTO>> Contultar_TipoPolizas()
        {
            return await TipoPolizas.FromSqlInterpolated($"EXEC spContultar_TipoPolizas").ToListAsync();
        }



        #endregion

        #region Usuarios

        public async Task<int> Usuarios_CrearUsuarioNuevoAsync(UsuariosDTO usuario)
        {
            // Crear el parámetro de salida
            var resultadoParam = new SqlParameter("@Resultado", System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.Output
            };

            // Ejecutar el procedimiento almacenado
            await Database.ExecuteSqlRawAsync(
                "EXEC spUsuarios_CrearUsuarioNuevo @Nombre, @CorreoElectronico, @Rol, @Contraseña, @Estado, @Resultado OUTPUT",
                new SqlParameter("@Nombre", usuario.Nombre),
                new SqlParameter("@CorreoElectronico", usuario.CorreoElectronico),
                new SqlParameter("@Rol", usuario.Rol),
                new SqlParameter("@Contraseña", usuario.Contraseña),
                new SqlParameter("@Estado", usuario.Estado),
                resultadoParam
            );

            // Retornar el valor del parámetro de salida
            return (int)resultadoParam.Value;
        }

        public async Task<UsuariosDTO> Usuarios_ConsultaUsuarioCorreo(string CorreoElectronico)
        {

            var listaUsuarios = await Usuarios
                .FromSqlInterpolated($"EXEC spUsuarios_ConsultaUsuarioCorreo @Correo = {CorreoElectronico}")
                .ToListAsync();

            return listaUsuarios.FirstOrDefault() ?? new UsuariosDTO();
        }

        public async Task<List<UsuariosDTO>> Usuarios_Obtener()
        {
            return await Usuarios.FromSqlInterpolated($"EXEC spUsuarios_Obtener").ToListAsync();
        }


        #endregion

        #region Cotizaciones

        public async Task<List<spCotizacion_polizasUsuarioDTO>> spCotizacion_polizasUsuario(int Usuario)
        {

            var listaPolizaUsuarios = await spCotizacion_polizasUsuarioDTO
                .FromSqlInterpolated($"EXEC spCotizacion_polizasUsuario @UsuarioId = {Usuario}")
                .ToListAsync();


            return listaPolizaUsuarios ?? new List<spCotizacion_polizasUsuarioDTO>();
        }

        #endregion

        #region Siniestros

        public async Task<bool> Siniestro_AgregarNuevoPoliza(SiniestrosDTO siniestro)
        {
            // Crear el parámetro de salida
            var resultadoParam = new SqlParameter("@Resultado", System.Data.SqlDbType.Bit)
            {
                Direction = System.Data.ParameterDirection.Output
            };

            // Ejecutar el procedimiento almacenado
            await Database.ExecuteSqlRawAsync(
                "EXEC spSiniestro_AgregarNuevoPoliza @PolizaId, @FechaSiniestro, @Descripcion, @Estado, @MontoReclamo, @Resultado OUTPUT",
                new SqlParameter("PolizaId", siniestro.PolizaId),
                new SqlParameter("FechaSiniestro", siniestro.FechaSiniestro),
                new SqlParameter("Descripcion", siniestro.Descripcion),
                new SqlParameter("Estado", siniestro.Estado),
                new SqlParameter("MontoReclamo", siniestro.MontoReclamo),
                resultadoParam
            );

            // Retornar el valor del parámetro de salida
            return (bool)resultadoParam.Value;
        }

        public async Task<List<SiniestrosDTO>> Siniestro_ConsultaPoliza(int PolizaId)
        {

            var listaSiniestros = await Siniestro
                .FromSqlInterpolated($"EXEC spSiniestro_ConsultaPoliza @PolizaId = {PolizaId}")
                .ToListAsync();


            return listaSiniestros ?? new List<SiniestrosDTO>();
        }


        #endregion

    }
}