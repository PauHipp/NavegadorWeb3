using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using System.IO;
using System.Web;
using System.Drawing.Text; //(Input, Output)


namespace NavegadorWeb
{
    public partial class Form1 : Form


    {
        private List<URL> historial=new List<URL>();

        private void GuardarHistorial()

        {
            string fileName = @"C:\Users\paula\OneDrive\Escritorio\Historial.txt";
            using (StreamWriter file = new StreamWriter(fileName, false)) // false para sobrescribir el archivo
            { 
                foreach (var url in historial)
                {
                    file.WriteLine (url.ToString());
                }
            }
        }

        private void AgregarActualizarURL (string url)
        {
            var existente = historial.FirstOrDefault(u => u.Link.Equals(url, StringComparison.OrdinalIgnoreCase));
            if (existente != null)
            {
                existente.ActualizarAcceso();
            }
            else
            {
                historial.Add(new URL(url));
            }
            GuardarHistorial();
            ActualizarHistorial();
        }

        public Form1()
        {
            InitializeComponent();

            //comboBox_PaginasWeb.Items.Add("https://espanol.yahoo.com/");
            //comboBox_PaginasWeb.Items.Add("https://es.wikipedia.org/wiki/Wikipedia:Portada");
            //comboBox_PaginasWeb.Items.Add("https://www.youtube.com/");
            //comboBox_PaginasWeb.Items.Add("https://www.bing.com/?setlang=es");
            //comboBox_PaginasWeb.Items.Add("https://www.google.com.gt/?hl=es");




            //Selecciona el primer Item por defecto
            //comboBox_PaginasWeb.SelectedIndex = 4;

            leer();

        }
        private void Guardar(string fileName, string texto)
        {
            FileStream stream = new FileStream(fileName, FileMode.Append, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream);
            writer.WriteLine(texto);
            writer.Close();
        }

        private void comboBox_PaginasWeb_SelectedIndexChanged(object sender, EventArgs e)

        {

            
            
        }

        private void button_ir_Click(object sender, EventArgs e)
        {
            //Ir a las paginas web
            //verificar y colocarles https a la pagina para que funcionen


            string url = comboBox_PaginasWeb.Text.Trim();//.text porque estamos escribiendo

            // Verifica si la URL comienza con "http://" o "https://", si no añade "https://" para que la pagina funcione
            if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase) && !url.StartsWith(".", StringComparison.OrdinalIgnoreCase))
            {
                url = " " + url;
            }
            if (!url.StartsWith("https://www.google.com/search?q=", StringComparison.OrdinalIgnoreCase))
            {
                url = "https://www.google.com/search?q=" + url;
            }


            webView2.CoreWebView2.Navigate(url);

            //Codigo para guardar el historial

            if (!comboBox_PaginasWeb.Items.Contains(url))
            {
                comboBox_PaginasWeb.Items.Add(url); // Agrega la URL al ComboBox del historial
                Guardar(@"C:\Users\paula\OneDrive\Escritorio\Historial.txt", comboBox_PaginasWeb.Text); // Guarda la URL en el archivo de historial

                comboBox_PaginasWeb.Items.Clear();
                leer();
            }

            AgregarActualizarURL(url);
           

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void haciaAtrasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Regresar a la pagina anterior
            //Con CanGoback verifica si podemos regresar 
            if (webView2.CoreWebView2.CanGoBack)
            {
                webView2.CoreWebView2.GoBack();
            }

        }

        private void haciaAdelanteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Ir a la pagina siguiente
            if (webView2.CoreWebView2.CanGoForward)
            {
                webView2.CoreWebView2.GoForward();
            }

        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Salir de la pagina
            this.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
       
            leer();
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        private void inicioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            webView2.CoreWebView2.Navigate("https://www.google.com.gt/?hl=es");
        }
        
        private void leer()
        {

            historial.Clear();
            string fileName = @"C:\Users\paula\OneDrive\Escritorio\Historial.txt";
            if (File.Exists(fileName))
            {
                using (StreamReader reader = new StreamReader(fileName)) 
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var partes = line.Split('|');
                        if (partes.Length == 3)
                        {
                            var url = new URL(partes[0].Trim())
                            {
                                visitas = int.Parse(partes[1]),
                                UltimoAcceso = DateTime.Parse(partes[2])
                            };
                            if (!historial.Any(u => u.Link.Equals(url.Link, StringComparison.OrdinalIgnoreCase)))
                            {
                                historial.Add(url);
                            }
                        }
                    }
                }
            }
            ActualizarHistorial();

            ////aca debe mostral el historial de las paginas visitadas anteriormente

            //FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            //StreamReader reader = new StreamReader(stream);

            //while (reader.Peek() > -1)

            //{
            //    string textoleido = reader.ReadLine();
            //    comboBox_PaginasWeb.Items.Add(textoleido);  
            //}
            //Cerrar el archivo, esta linea es importante porque sino despues de correr varias veces el programa daría error de que el archivo quedó abierto muchas veces. Entonces es necesario cerrarlo despues de terminar de leerlo.
            //reader.Close();
        }

        private void OrdenarPorVisitas()
        {
            historial = historial.OrderByDescending(u => u.visitas).ToList();
            ActualizarHistorial() ;

        }

        private void OrdenarPorFechaAcceso()
        {

            historial = historial.OrderByDescending(u => u.UltimoAcceso).ToList();
            ActualizarHistorial();
        }

        private void EliminarURl(string url)
        {
            historial.RemoveAll(u => u.Link == url);
            GuardarHistorial();
            ActualizarHistorial();

        }

        private void ekl_Click(object sender, EventArgs e)
        {
            if (comboBox_PaginasWeb.SelectedItem != null)
            {
                EliminarURl(comboBox_PaginasWeb.SelectedItem.ToString());
                comboBox_PaginasWeb.Items.Remove(comboBox_PaginasWeb.SelectedItem);
            }

        }

        private void OrdenarVisitas_Click(object sender, EventArgs e)
        {
            OrdenarPorVisitas();
        }

        private void OrdenarPorFechaDeAcceso_Click(object sender, EventArgs e)
        {
            OrdenarPorFechaAcceso();
            ActualizarHistorial();
        }
        private void ActualizarHistorial()
        {
            comboBox_PaginasWeb.Items.Clear();
            foreach (var item in historial)
            {
                comboBox_PaginasWeb.Items.Add(item.Link);

            }
            if (comboBox_PaginasWeb.Items.Count > 0) 
            { 
                comboBox_PaginasWeb.SelectedIndex = 0;
            }
        }
        
        private void EliminarHistorialCompleto()
        {
            historial.Clear();
            //Borra el contenido del archivo donde guarda el historial
            File.WriteAllText(@"C:\Users\paula\OneDrive\Escritorio\Historial.txt", string.Empty);
            comboBox_PaginasWeb.Items.Clear();//Actualiza el combobox
        }

        private void button1_Click(object sender, EventArgs e)
        {
            EliminarHistorialCompleto();    
        }
    }
}
