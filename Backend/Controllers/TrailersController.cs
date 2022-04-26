using AutoMapper;
using Backend.Models;
using Backend.Models.ModelsDtos;
using Backend.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [Authorize]
    [Route("api/trailer")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "trailer")]

    public class TrailersController : Controller
    {
        private readonly IRepositoryTrailer _repo;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _Hosting;

        public TrailersController(IRepositoryTrailer repo, IMapper mapper, IWebHostEnvironment hosting)
        {
            _repo = repo;
            _mapper = mapper;
            _Hosting = hosting;
        }
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<TrailerDto>))]
        [ProducesResponseType(400)]
        public async Task<ActionResult> GetTrailer()
        {
            List<Trailer> list = await _repo.GetTrailer();
            List<TrailerDto> listDto = new List<TrailerDto>();

            foreach (var item in list)
            {
                listDto.Add(_mapper.Map<TrailerDto>(item));
            }

            return Ok(listDto); 
        }
        [AllowAnonymous]
        [HttpGet("{id:int}", Name = "GetTrailerById")]
        [ProducesResponseType(200, Type = typeof(TrailerDto))]
        [ProducesResponseType(400)]
        public async Task<ActionResult> GetTrailerById(int id)
        {
            Trailer trailer = await _repo.GetTrailerById(id);
            TrailerDto traileDto = new TrailerDto();

            traileDto = _mapper.Map<TrailerDto>(trailer);

            return Ok(traileDto);
        }
        [AllowAnonymous]
        [HttpGet("GetTrailerByName/{name}")]
        [ProducesResponseType(200, Type = typeof(TrailerDto))]
        [ProducesResponseType(400)]
        public async Task<ActionResult> GetTrailerByName(string name)
        {
            Trailer trailer = await _repo.GetTrailerByName(name);
            TrailerDto traileDto = new TrailerDto();

            traileDto = _mapper.Map<TrailerDto>(trailer);

            return Ok(traileDto);
        }
        [AllowAnonymous]
        [HttpGet("gettrailerbykeyword/{keyword}")]
        [ProducesResponseType(200, Type = typeof(List<TrailerDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> gettrailerbykeyword(string keyword)
        {
            try
            {
                IEnumerable<Trailer> result = await _repo.GetTraileraByKeyword(keyword);

                var ResultDto = _mapper.Map<IEnumerable<Trailer>, List<TrailerDto>>(result);


                if (ResultDto.Any())
                {
                    return Ok(ResultDto);
                }
                return NotFound();
            }
            catch (Exception Ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error recuperando datos de la aplicacion");
            }
        }
        
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(TrailerDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> createTrailer([FromBody] TrailerDto TrailerDto)
        {
            try
            {
                if (TrailerDto == null)
                {
                    return BadRequest(ModelState);
                }

                bool value = await _repo.ExisteTrailer(TrailerDto.Titulo);

                if (value)
                {
                    ModelState.AddModelError("", "Este producto ya esta creado ya fue creado.");
                    return StatusCode(404, ModelState);
                }
                Trailer item = _mapper.Map<Trailer>(TrailerDto);

                value = await _repo.CreateTrailer(item);

                if (!value)
                {
                    ModelState.AddModelError("", "Ha pasado algo con la base de datos, por favor comuniquese con el servicio tecnico.");
                    return StatusCode(500, ModelState);
                }

                return (Ok(item));
            }
            catch
            {
                ModelState.AddModelError("", $"Ha pasado algo con la base de datos, por favor comuniquese con servicio tecnico.");
                return StatusCode(500, ModelState);
            }
        }

        [HttpPatch("{id:int}", Name = "UpdateTrailer")]
        [ProducesResponseType(204, Type = typeof(TrailerUpdateDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> UpdateTrailer(int id, [FromBody] TrailerUpdateDto trailerdto)
        {
            if (trailerdto == null || trailerdto.id != id )
            {
                return BadRequest(ModelState);
            }
            Trailer trailer = _mapper.Map<Trailer>(trailerdto);

            bool Value = await _repo.ExisteTrailer(id);

            if (!Value)
            {
                ModelState.AddModelError("", $"El trailer de {trailerdto.Titulo} no existe en la base de datos.");
                return StatusCode(500, ModelState);
            }
            if(trailer.Id != id)
            {
                ModelState.AddModelError("", $"El id del trailer de {trailerdto.Titulo} no concuerda por favor revise el id.");
                return StatusCode(500, ModelState);
            }
            Value = await _repo.UpdateTrailer(trailer);

            if (!Value)
            {
                ModelState.AddModelError("", $"Algo salio mal actualizando la peliculas {trailerdto.Titulo}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetTrailerById", new { Id = trailer.Id }, trailer);
        }

        [HttpDelete("{id:int}", Name = "deletetrailer")]
        [ProducesResponseType(200, Type = typeof(TrailerDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> deletetrailer(int id)
        {
            bool Value = await _repo.ExisteTrailer(id);

            if (!Value)
            {
                ModelState.AddModelError("", "El trailer  que deseas eliminar no existe.");
                return StatusCode(404, ModelState);
            }

            Trailer trailer = await _repo.GetTrailerById(id);

            Value = await _repo.DeleteTrailer(trailer);

            if (!Value)
            {
                ModelState.AddModelError("", "Ha pasado algo con la base de datos, por favor comuniquese con servicio tecnico.");
                return StatusCode(500, ModelState);
            }

            return Ok($"Se ha eliminado correctamente la entrega de  {trailer.Titulo} de la base de datos.");
        }


    }
}