using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using APICORE.Models;

using System.Data;
using System.Data.SqlClient;

using Microsoft.AspNetCore.Cors;

namespace APICORE.Controllers
{
    [EnableCors("ReglasCors")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {

        private readonly string cadenaSQL;

        public ProductoController(IConfiguration config) {

            cadenaSQL = config.GetConnectionString("CadenaSQL");

        }

        [HttpGet]
        [Route("Lista")]

        public IActionResult Lista() { 
        
            List<Producto> lista = new List<Producto>();

            try
            {

                using (var conexion = new SqlConnection(cadenaSQL)) { 
                
                    conexion.Open();
                    var cmd = new SqlCommand("sp_lista_productos", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var rd = cmd.ExecuteReader()) {

                        while (rd.Read())
                        {

                            lista.Add(new Producto()
                            {

                                IdProducto = Convert.ToInt32(rd["IdProducto"]),
                                CodigoBarra = rd["CodigoBarra"].ToString(),
                                Nombre = rd["Nombre"].ToString(),
                                Marca = rd["Marca"].ToString(),
                                Categoria = rd["Categoria"].ToString(),
                                Precio = Convert.ToDecimal(rd["Precio"])

                            }); 

                        }

                    }

                }

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = lista });

            }
            catch (Exception error) {

                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message, response = lista });

            }

        }

        [HttpGet] // Cuando es get es porque se toma información de la base de datos
        [Route("Obtener/{IdProducto:int}")] // Se pone que recibe a IdProducto que es un entero, esto se marca con dos puntos, el tipo de archivo.

        public IActionResult Obtener(int IdProducto)
        {

            List<Producto> lista = new List<Producto>();
            Producto producto = new Producto();

            try
            {

                using (var conexion = new SqlConnection(cadenaSQL))
                {

                    conexion.Open();
                    var cmd = new SqlCommand("sp_lista_productos", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var rd = cmd.ExecuteReader())
                    {

                        while (rd.Read())
                        {

                            lista.Add(new Producto()
                            {

                                IdProducto = Convert.ToInt32(rd["IdProducto"]),
                                CodigoBarra = rd["CodigoBarra"].ToString(),
                                Nombre = rd["Nombre"].ToString(),
                                Marca = rd["Marca"].ToString(),
                                Categoria = rd["Categoria"].ToString(),
                                Precio = Convert.ToDecimal(rd["Precio"])

                            });
                        }

                    }



                }

                producto = lista.Where(item => item.IdProducto == IdProducto).FirstOrDefault();

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = producto }); // En vez de lista devuelve producto

            }
            catch (Exception error)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message, response = producto }); // Lo mismo aplica acá

            }

        }

        [HttpPost] // Cuando es post es porque se manda información a la base de datos
        [Route("Guardar")] // Se cubre acá el caso de guardar un producto.

        public IActionResult Guardar([FromBody] Producto objeto) // Siempre el nombre del método IActionResult que sea el mismo que la ruta. Se toma un objeto del tipo producto con la sentencia [FromBody]
        {

            try
            {

                using (var conexion = new SqlConnection(cadenaSQL))
                {

                    conexion.Open();
                    var cmd = new SqlCommand("sp_guardar_producto", conexion);
                    cmd.Parameters.AddWithValue("codigoBarra", objeto.CodigoBarra);
                    cmd.Parameters.AddWithValue("nombre", objeto.Nombre);
                    cmd.Parameters.AddWithValue("marca", objeto.Marca);
                    cmd.Parameters.AddWithValue("categoria", objeto.Categoria);
                    cmd.Parameters.AddWithValue("precio", objeto.Precio); // Todos estos son los parámetros que se mandan al SP que no es más que lo que recibe como parámetro de entrada el Stored
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery(); // Si esto no se pone en ningún momento se ejecuta la query.

                }
                
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok" }); // En vez de lista devuelve producto

            }
            catch (Exception error)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message }); // Lo mismo aplica acá

            }

        }

        [HttpPut] // Cuando es Put es porque se modifica información a la base de datos
        [Route("Editar")] // Se cubre acá el caso de guardar un producto.

        public IActionResult Editar([FromBody] Producto objeto) // Siempre el nombre del método IActionResult que sea el mismo que la ruta. Se toma un objeto del tipo producto con la sentencia [FromBody]
        {

            try
            {

                using (var conexion = new SqlConnection(cadenaSQL))
                {

                    conexion.Open();
                    var cmd = new SqlCommand("sp_editar_producto", conexion);
                    cmd.Parameters.AddWithValue("IdProducto", objeto.IdProducto == 0 ? DBNull.Value : objeto.IdProducto); // Recibe esto para tomar el correcto y editarlo
                    cmd.Parameters.AddWithValue("codigoBarra", objeto.CodigoBarra is null ? DBNull.Value : objeto.CodigoBarra);
                    cmd.Parameters.AddWithValue("nombre", objeto.Nombre is null ? DBNull.Value : objeto.Nombre);
                    cmd.Parameters.AddWithValue("marca", objeto.Marca is null ? DBNull.Value : objeto.Marca);
                    cmd.Parameters.AddWithValue("categoria", objeto.Categoria is null ? DBNull.Value : objeto.Categoria); // Todas estas validaciones son porque puede pasar que pasen valores nulos.
                    cmd.Parameters.AddWithValue("precio", objeto.Precio == 0 ? DBNull.Value : objeto.Precio); // Todos estos son los parámetros que se mandan al SP que no es más que lo que recibe como parámetro de entrada el Stored
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.ExecuteNonQuery(); // Si esto no se pone en ningún momento se ejecuta la query.

                }

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "editado" }); // Mensaje editado

            }
            catch (Exception error)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message }); // Lo mismo aplica acá

            }

        }

        [HttpDelete] // Cuando es Delete es porque se elimina información a la base de datos
        [Route("Eliminar/{IdProducto:int}")] // Se cubre acá el caso de guardar un producto.

        public IActionResult Eliminar(int IdProducto) // Siempre el nombre del método IActionResult que sea el mismo que la ruta. Se toma un objeto del tipo producto con la sentencia [FromBody]
        {

            try
            {

                using (var conexion = new SqlConnection(cadenaSQL))
                {

                    conexion.Open();
                    var cmd = new SqlCommand("sp_eliminar_producto", conexion);
                    cmd.Parameters.AddWithValue("IdProducto", IdProducto); // Recibe esto para tomar el correcto y editarlo
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.ExecuteNonQuery(); // Si esto no se pone en ningún momento se ejecuta la query.

                }

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "eliminado" }); // Mensaje eliminado

            }
            catch (Exception error)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message }); // Lo mismo aplica acá

            }

        }

    }
}
