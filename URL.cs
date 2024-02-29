using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavegadorWeb
{
    internal class URL
    {
        public string Link { get; set; } 
        public int visitas{ get; set; }
        public DateTime UltimoAcceso { get; set; }
        
        public URL(string link)
        {
            this.Link = link;
            this.visitas = 1; 
            this.UltimoAcceso = DateTime.Now;
        }

        public void ActualizarAcceso() 
        {
            this.visitas++;
            this.UltimoAcceso = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{this.Link} |{this.visitas} | {this.UltimoAcceso}";
        }
    }

}
