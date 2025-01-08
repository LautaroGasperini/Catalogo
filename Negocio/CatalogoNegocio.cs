using Dominio;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Negocio
{
    public class CatalogoNegocio
    {
        public List<Catalogo> listarCatalogo()
        {
            List<Catalogo> listacatalogo = new List<Catalogo>();
            AcessoDatos datos = new AcessoDatos();

            try
            {
                datos.setearConsulta("select a.Id, Codigo,Nombre,a.Descripcion,a.IdCategoria,c.Descripcion Categoria,a.IdMarca, m.Descripcion Marca, ImagenUrl, Precio from ARTICULOS a inner join CATEGORIAS c on a.IdCategoria = c.Id inner join MARCAS m on a.IdMarca = m.Id  ");
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Catalogo aux = new Catalogo();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.Codigo = (string)datos.Lector["Codigo"];
                    aux.Nombre = (string)datos.Lector["Nombre"];
                    aux.Descripcion = (string)datos.Lector["Descripcion"];
                    aux.Categoria = new Categorias();
                    aux.Categoria.Id = (int)datos.Lector["IdCategoria"];
                    aux.Categoria.Categoria = (string)datos.Lector["Categoria"];     
                    aux.Marca = new Marcas();
                    aux.Marca.Id = (int) datos.Lector["IdMarca"];
                    aux.Marca.Marca = (string)datos.Lector["Marca"];
                    if (!(datos.Lector["ImagenUrl"] is DBNull))
                        aux.UrlImagen = (string)datos.Lector["ImagenUrl"];
                    aux.Precio = (decimal)datos.Lector["Precio"];

                    
                    

                    listacatalogo.Add(aux);
                }
                return listacatalogo;
            }
            catch (Exception ex)
            {

                throw ex;
            }
           
        }
        public void agregar(Catalogo nuevo)
        {
            AcessoDatos datos = new AcessoDatos();
            try
            {
                datos.setearConsulta("insert into ARTICULOS(Codigo,Nombre,Descripcion,IdMarca,IdCategoria,ImagenUrl,Precio) values(@Codigo,@Nombre,@Descripcion,@IdMarca,@IdCategoria,@ImagenUrl,@Precio)");
                datos.setearParametro("@Codigo",nuevo.Codigo);
                datos.setearParametro("@Nombre",nuevo.Nombre);
                datos.setearParametro("@Descripcion",nuevo.Descripcion);
                datos.setearParametro("@IdMarca",nuevo.Marca.Id);
                datos.setearParametro("@IdCategoria",nuevo.Categoria.Id);
                datos.setearParametro("@ImagenUrl",nuevo.UrlImagen);
                datos.setearParametro("@Precio",nuevo.Precio);
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.cerrarConexion(); 
            }
        }
        public void modificar(Catalogo modificar)
        {
            AcessoDatos datos=new AcessoDatos();
            try
            {
                datos.setearConsulta("update ARTICULOS set Codigo = @Codigo,Nombre = @Nombre,Descripcion = @Descripcion, IdCategoria = @IdCategoria, IdMarca = @IdMarca, ImagenUrl = @ImagenUrl, Precio= @Precio where Id = @Id");
                datos.setearParametro("@Codigo", modificar.Codigo);
                datos.setearParametro("@Nombre", modificar.Nombre);
                datos.setearParametro("@Descripcion", modificar.Descripcion);
                datos.setearParametro("@IdMarca", modificar.Marca.Id);
                datos.setearParametro("@IdCategoria", modificar.Categoria.Id);
                datos.setearParametro("@ImagenUrl", modificar.UrlImagen);
                datos.setearParametro("@Precio", modificar.Precio);
                datos.setearParametro("@Id", modificar.Id);
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
        public void eliminar(int id)
        {
            try
            {
                AcessoDatos datos = new AcessoDatos();
                datos.setearConsulta("delete from ARTICULOS where Id = @Id");
                datos.setearParametro("@Id",id);
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }
        public void VerDetalles(int id)
        {
            
            AcessoDatos datos = new AcessoDatos();

            try
            {
                datos.setearConsulta("select Codigo, Nombre,a.Descripcion,c.Descripcion Categoria, m.Descripcion Marca,Precio from ARTICULOS a, CATEGORIAS c, MARCAS m  where a.Id = @Id");
                datos.setearParametro("@Id", id);
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Catalogo catalogo = new Catalogo();                    
                    catalogo.Codigo = (string)datos.Lector["Codigo"];
                    catalogo.Nombre = (string)datos.Lector["Nombre"];
                    catalogo.Descripcion = (string)datos.Lector["Descripcion"];
                    catalogo.Categoria = new Categorias();
                    catalogo.Categoria.Categoria = (string)datos.Lector["Categoria"];
                    catalogo.Marca = new Marcas();
                    catalogo.Marca.Marca = (string)datos.Lector["Marca"];
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public List<Catalogo> filtrar(string campo, string criterio, string filtro)
        {
            List<Catalogo> lista = new List<Catalogo>();
            AcessoDatos datos = new AcessoDatos();
            try
            {
                string consulta = "select a.Id, Codigo,Nombre,a.Descripcion,a.IdCategoria,c.Descripcion Categoria,a.IdMarca, m.Descripcion Marca, ImagenUrl, Precio from ARTICULOS a inner join CATEGORIAS c on a.IdCategoria = c.Id inner join MARCAS m on a.IdMarca = m.Id where  ";
                if(campo == "Precio")
                {
                    switch (criterio)
                    {
                        case "Mayor a":
                            consulta += "Precio > " + filtro;
                            break;
                        case "Menor a":
                            consulta += "Precio < " + filtro;
                            break;
                        default:
                            consulta += "Precio = " + filtro;
                            break;
                    }
                    
                }
                else if(campo == "Categoria")
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += "c.Descripcion like '"+ filtro +"%'";
                            break;
                        case "Termina con":
                            consulta += "c.Descripcion like '%" + filtro + "'";
                            break;
                        case "Contiene":
                            consulta += "c.Descripcion like '%" + filtro + "%'";
                            break;
                    }
                }
                else if (campo == "Marca")
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += "m.Descripcion like '" + filtro + "%'";
                            break;
                        case "Termina con":
                            consulta += "m.Descripcion like '%" + filtro + "'";
                            break;
                        case "Contiene":
                            consulta += "m.Descripcion like '%" + filtro + "%'";
                            break;
                    }
                }
                else
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += "Nombre like '" + filtro + "%'";
                            break;
                        case "Termina con":
                            consulta += "Nombre like '%" + filtro + "'";
                            break;
                        case "Contiene":
                            consulta += "Nombre like '%" + filtro + "%'";
                            break;
                    }
                }

                datos.setearConsulta( consulta );
                datos.ejecutarLectura();
                while (datos.Lector.Read())
                {
                    Catalogo aux = new Catalogo();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.Codigo = (string)datos.Lector["Codigo"];
                    aux.Nombre = (string)datos.Lector["Nombre"];
                    aux.Descripcion = (string)datos.Lector["Descripcion"];
                    aux.Categoria = new Categorias();
                    aux.Categoria.Id = (int)datos.Lector["IdCategoria"];
                    aux.Categoria.Categoria = (string)datos.Lector["Categoria"];
                    aux.Marca = new Marcas();
                    aux.Marca.Id = (int)datos.Lector["IdMarca"];
                    aux.Marca.Marca = (string)datos.Lector["Marca"];
                    if (!(datos.Lector["ImagenUrl"] is DBNull))
                        aux.UrlImagen = (string)datos.Lector["ImagenUrl"];
                    aux.Precio = (decimal)datos.Lector["Precio"];




                    lista.Add(aux);
                }

                return lista;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
