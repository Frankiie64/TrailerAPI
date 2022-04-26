using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Models.ModelsDtos
{
    public class UserRegisterDto
    {        
            [Required(ErrorMessage = "Por favor introduzca su nombre de usuario.")]
            public string Username { get; set; }
           
            [Required(ErrorMessage = "Por favor introduzca una contraseña.")]
            [StringLength(12, ErrorMessage = "La {0} debe ser mayor a {2} caracteres y menor a {1}.", MinimumLength = 8)]
            [Display(Name = "Contraseña")]
            public string Password { get; set; }
        
    }
}
