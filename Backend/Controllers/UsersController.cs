using AutoMapper;
using Backend.Models;
using Backend.Models.ModelsDtos;
using Backend.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [Authorize]
    [Route("api/user")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "users")]
    public class UsersController : Controller
    {
        private readonly IRepositoryUser _repo;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public UsersController(IRepositoryUser repo, IMapper mapper,IConfiguration config)
        {
            _repo = repo;
            _mapper = mapper;
            _config = config;
        }
        [AllowAnonymous]
        [HttpPost("registro")]
        [ProducesResponseType(201, Type = typeof(UserRegisterDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> RegisterUser([FromBody] UserRegisterDto userDto)
        {
           
            var Value = await _repo.ExistUser(userDto.Username);

            if (Value)
            {
                return BadRequest("El usuario que desea registrar ya existe.");
            }

            User NewUser = _mapper.Map<User>(userDto);

            NewUser.IdTypeUser = 1;

            var user = await _repo.Register(NewUser, userDto.Password);

            if (user == null)
            {
                ModelState.AddModelError("", "Ha pasado algo con la base de datos, por favor comuniquese con servicio tecnico.De lo contrario gracias por preferirnos.");
                return StatusCode(500, ModelState);
            }
            return Ok(userDto);
        }
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(202, Type = typeof(UserLoginDto))]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userDto)
        {
     

            if (!await _repo.ExistUser(userDto.Username))
            {
                return BadRequest("El usuario que ha ingrasado no existe.");
            }

            User item = await _repo.Login(userDto.Password, userDto.Username);

            if (item == null)
            {
                return Unauthorized("Los datos no coinciden con los dato que se han  encontrado en la base de datos.");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,item.Id.ToString()),
                new Claim(ClaimTypes.Name,item.Username.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            var Credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var TokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = Credentials
            };

            var TokenHandler = new JwtSecurityTokenHandler();
            var token = TokenHandler.CreateToken(TokenDescriptor);
            var Hr = new
            {
                token = TokenHandler.WriteToken(token)
            };

            Response.Headers.Add("Bearer", Hr.token);

            return Ok(new
            {
                token = TokenHandler.WriteToken(token),                
                SuperUser = item.IdTypeUser == 2 ? true : false
            });
        }
    }
}
