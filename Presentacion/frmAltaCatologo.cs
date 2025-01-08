using Dominio;
using Negocio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace Presentacion
{
    public partial class frmAltaCatologo : Form
    {
        private Catalogo catalogo = null;
        private OpenFileDialog archivo = null;
        private List<(TextBox, Label)> validar;
        public frmAltaCatologo()
        {
            InitializeComponent();
        }
        public frmAltaCatologo(Catalogo catalogo)
        {
            InitializeComponent();
            this.catalogo = catalogo;
            Text = "Modificar Catalogo";
        }

        private void frmAltaCatologo_Load(object sender, EventArgs e)
        {
            MarcaNegocio marcaNegocio = new MarcaNegocio();
            CategoriaNegocio categoriaNegocio = new CategoriaNegocio();

            
            try
            {
                cboMarca.DataSource = marcaNegocio.listarmarca();
                cboMarca.ValueMember = "Id";
                cboMarca.DisplayMember = "Marca";
                cboCategoria.DataSource = categoriaNegocio.listarcategoria();
                cboCategoria.ValueMember = "Id";
                cboCategoria.DisplayMember = "Categoria";

                if (catalogo != null)
                {
                    txtCodigo.Text = catalogo.Codigo;
                    txtNombre.Text = catalogo.Nombre;
                    txtDescripcion.Text = catalogo.Descripcion;
                    cboMarca.SelectedValue = catalogo.Marca.Id;
                    cboCategoria.SelectedValue = catalogo.Categoria.Id;
                    txtUrlImagen.Text = catalogo.UrlImagen;
                    cargarImagen(catalogo.UrlImagen);
                    txtPrecio.Text = catalogo.Precio.ToString();
                }

                aValidar();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {           
            CatalogoNegocio catalogoNegocio = new CatalogoNegocio();
            try
            {
                if (catalogo == null)
                    catalogo = new Catalogo();

                catalogo.Codigo = txtCodigo.Text;
                catalogo.Nombre = txtNombre.Text;
                catalogo.Descripcion = txtDescripcion.Text;
                catalogo.Marca = (Marcas)cboMarca.SelectedItem;
                catalogo.Categoria = (Categorias)cboCategoria.SelectedItem;
                catalogo.UrlImagen = txtUrlImagen.Text;
                if (soloNumeros(txtPrecio.Text) && validarCampos())
                {
                    catalogo.Precio = decimal.Parse(txtPrecio.Text);
                }
                


                if (validarCampos())
                {
                    if (catalogo.Id != 0)
                    {
                        catalogoNegocio.modificar(catalogo);
                        MessageBox.Show("Modificado Exitosamente!");
                    }
                    else
                    {
                        catalogoNegocio.agregar(catalogo);
                        MessageBox.Show("Agregado Exitosamente!");
                    }
                    Close();
                }
                else
                {
                    MessageBox.Show("Los campos resaltados son obligatorios!.", "Cuidado...",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                    
                }

              
                if (archivo != null && !(txtUrlImagen.Text.ToUpper().Contains("HTTP")))
                {
                    File.Copy(archivo.FileName, ConfigurationManager.AppSettings["images-folder"] + archivo.SafeFileName);
                }
                
                




            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }
        private void cargarImagen(string imagen)
        {
            try
            {
                pbxImagen.Load(imagen);
            }
            catch (Exception)
            {

                pbxImagen.Load("https://img.freepik.com/vector-premium/icono-marco-fotos-foto-vacia-blanco-vector-sobre-fondo-transparente-aislado-eps-10_399089-1290.jpg");
            }

        }

        private void txtUrlImagen_Leave(object sender, EventArgs e)
        {
            cargarImagen(txtUrlImagen.Text);
        }

        private void btnImagenLocal_Click(object sender, EventArgs e)
        {
            archivo = new OpenFileDialog();
            archivo.Filter = "jpg|*.jpg|png|*.png";
            if(archivo.ShowDialog() == DialogResult.OK)
            {
                txtUrlImagen.Text = archivo.FileName;
                cargarImagen(archivo.FileName);

                File.Copy(archivo.FileName, ConfigurationManager.AppSettings["images-folder"] + archivo.SafeFileName);
            }
            else
            {
                archivo = null;
            }
        }

        private void aValidar()
        {
            validar = new List<(TextBox, Label)>
            {
                (txtCodigo,lblCodigoError),
                (txtNombre,lblNombreError),
                (txtDescripcion,lblDescripcionError),
                (txtPrecio,lblPrecioError)
            };

            foreach (var campo in validar)
            {
                campo.Item2.Text = "*";
                campo.Item2.ForeColor = Color.Red;
                campo.Item2.Visible = false;
            }
        }

        private bool validarCampos()
        {
            bool esValido = true;

            foreach (var (textBox, label) in validar)
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    label.Visible = true; // Muestra el asterisco
                    textBox.BackColor = Color.LightPink; // Opcional: Resalta el campo
                    esValido = false;
                }
                else
                {
                    label.Visible = false; // Oculta el asterisco
                    textBox.BackColor = SystemColors.Window; // Restablece el color
                }
            }

            return esValido;
        }
        private bool soloNumeros(string cadena)
        {
            foreach (var caracter in cadena)
            {
                if (!(char.IsNumber(caracter)))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
