using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Models.ModelsDtos
{
    public class TrailerUpdateDto
    {
        
        public int id { get; set; }
        public string Titulo { get; set; }
        public string Director { get; set; }
        public string Reseña { get; set; }
        public string RutaTrailer { get; set; }
        public string Actores { get; set; }
        public DateTime Año { get; set; }
    }
}

