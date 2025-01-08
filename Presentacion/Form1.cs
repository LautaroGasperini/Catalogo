using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dominio;
using Negocio;

namespace Presentacion
{
    public partial class frmCatalogo : Form
    {
        private List<Catalogo> listacatalogos;
        public frmCatalogo()
        {
            InitializeComponent();
        }


        private void btnAgregar_Click(object sender, EventArgs e)
        {
            frmAltaCatologo altaCatologo = new frmAltaCatologo();
            altaCatologo.ShowDialog();
            cargar();
        }

        private void frmCatalogo_Load(object sender, EventArgs e)
        {
            cargar();
            ocultarColumnas();
            cboCampo.Items.Add("Categoria");
            cboCampo.Items.Add("Marca");
            cboCampo.Items.Add("Precio");
            cboCampo.Items.Add("Nombre");


        }
        private void cargar()
        {
            CatalogoNegocio negocio = new CatalogoNegocio();
            try
            {
                listacatalogos = negocio.listarCatalogo();
                dgvCatalogo.DataSource = listacatalogos;

                cargarImagen(listacatalogos[0].UrlImagen);
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
        private void ocultarColumnas()
        {
            dgvCatalogo.Columns["Id"].Visible = false;
            dgvCatalogo.Columns["UrlImagen"].Visible = false;
        }

        private void dgvCatalogo_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCatalogo.CurrentRow != null)
            {
                Catalogo seleccionado = (Catalogo)dgvCatalogo.CurrentRow.DataBoundItem;
                cargarImagen(seleccionado.UrlImagen);

            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            Catalogo seleccionado;
            try
            {
                seleccionado = (Catalogo)dgvCatalogo.CurrentRow.DataBoundItem;
                frmAltaCatologo modificar = new frmAltaCatologo(seleccionado);
                modificar.ShowDialog();
                cargar();              
            }
            catch (Exception)
            {

                MessageBox.Show("No hay algun elemento para modificar.");
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            CatalogoNegocio negocio = new CatalogoNegocio();
            Catalogo seleccionado = new Catalogo();
            try
            {
                if (dgvCatalogo.CurrentRow != null)
                {
                    DialogResult respuesta = MessageBox.Show("De verdad queres eliminarlo?", "Eliminando...", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (respuesta == DialogResult.Yes)
                    {
                        seleccionado = (Catalogo)dgvCatalogo.CurrentRow.DataBoundItem;
                        negocio.eliminar(seleccionado.Id);
                        cargar();
                    }
                }
                else
                {
                    MessageBox.Show("No hay elemento para eliminar");
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void btnVerDetalles_Click(object sender, EventArgs e)
        {
            CatalogoNegocio negocio = new CatalogoNegocio();
            Catalogo seleccionado = new Catalogo();
            try
            {
                if (dgvCatalogo.CurrentRow != null)
                {
                    seleccionado = (Catalogo)dgvCatalogo.CurrentRow.DataBoundItem;
                    negocio.VerDetalles(seleccionado.Id);

                    if (seleccionado != null)
                    {
                        MessageBox.Show("--------DETALLES DE PRODUCTO--------\n" +
                                        $"Codigo = {seleccionado.Codigo}\n" +
                                        $"Nombre = {seleccionado.Nombre}\n" +
                                        $"Descripcion = {seleccionado.Descripcion}\n" +
                                        $"Categoria = {seleccionado.Categoria}\n" +
                                        $"Marca = {seleccionado.Marca}\n" +
                                        $"Precio = {seleccionado.Precio}\n",
                                        "DETALLES DE PRODUCTO", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    }
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }

        }

        private void cboCampo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string opcion = cboCampo.SelectedItem.ToString();
            if (opcion == "Precio")
            {
                cboCriterio.Items.Clear();
                cboCriterio.Items.Add("Mayor a");
                cboCriterio.Items.Add("Menor a");
                cboCriterio.Items.Add("Igual a");
            }
            else
            {
                cboCriterio.Items.Clear();
                cboCriterio.Items.Add("Comienza con");
                cboCriterio.Items.Add("Termina con");
                cboCriterio.Items.Add("Contiene");
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            CatalogoNegocio negocio = new CatalogoNegocio();
            try
            {
                if (validarFiltro())
                    return;

                string campo = cboCampo.SelectedItem.ToString();
                string criterio = cboCriterio.SelectedItem.ToString();
                string filtro = txtFiltro.Text;

                dgvCatalogo.DataSource = negocio.filtrar(campo, criterio, filtro);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }
        private bool validarFiltro()
        {
            if (cboCampo.SelectedIndex < 0)
            {
                MessageBox.Show("Por favor, seleccione el campo para filtrar");
                return true;
            }
            if(cboCriterio.SelectedIndex < 0)
            {
                MessageBox.Show("Por favor, seleccione el criterio para filtrar");
                return true;
            }
            if(cboCampo.SelectedItem.ToString() == "Precio")
            {
                if (string.IsNullOrEmpty(txtFiltro.Text))
                {
                    MessageBox.Show("Debes cargar el filtro.");
                    return true;
                }
                if (!(soloNumeros(txtFiltro.Text)))
                {
                    MessageBox.Show("Ingrese solo numeros!");
                    return true;
                }
            }
            return false;
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
