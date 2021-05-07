using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;
using PeliculasAPI.Servicos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PeliculasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAlamacenadorArchivos _almacenadorArchivos;
        private readonly string contenedor = "actores";
        public ActoresController(ApplicationDbContext context, IMapper mapper, IAlamacenadorArchivos almacenadorArchivos)
        {
            _context = context;
            _mapper = mapper;
            _almacenadorArchivos = almacenadorArchivos;
        }

        [HttpGet]
        public async Task<ActionResult<List<ActorDTO>>> Get()
        {
            var actorEntidad = await _context.Actores.ToListAsync();
            var dto = _mapper.Map<List<ActorDTO>>(actorEntidad);

            return dto;
        }
        
        [HttpGet("{id}", Name ="ObtenerActor")]
        public async Task<ActionResult<ActorDTO>> Get(int id)
        {
            var actorEntidad = await _context.Actores.FirstOrDefaultAsync(i => i.Id == id);

            if (actorEntidad == null)
                return NotFound();

            return _mapper.Map<ActorDTO>(actorEntidad);

        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] ActorCreacionDTO actorCreacionDTO)
        {
            var actorEntidad =  _mapper.Map<Actor>(actorCreacionDTO);

            if(actorCreacionDTO.Foto != null)
            {
                using(var memoryStream = new MemoryStream())
                {
                    await actorCreacionDTO.Foto.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(actorCreacionDTO.Foto.FileName);
                    actorEntidad.Foto = await _almacenadorArchivos.GuardarArchivo(contenido, extension, contenedor,
                        actorCreacionDTO.Foto.ContentType);
                }
            }

            _context.Add(actorEntidad);
            await _context.SaveChangesAsync();
            var dto = _mapper.Map<ActorDTO>(actorEntidad);

            return new CreatedAtRouteResult("ObtenerActor", new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromForm] ActorCreacionDTO actorCreacionDTO)
        {
            //var actorEntidad = _mapper.Map<Actor>(actorCreacionDTO);
            //actorEntidad.Id = id;
            //_context.Entry(actorEntidad).State = EntityState.Modified;

            var actorDb = await _context.Actores.FirstOrDefaultAsync(i => i.Id == id);
            if (actorCreacionDTO.Foto != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await actorCreacionDTO.Foto.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(actorCreacionDTO.Foto.FileName);
                    actorDb.Foto = await _almacenadorArchivos.EditarArchivo(contenido, extension, contenedor,
                        actorDb.Foto,
                        actorCreacionDTO.Foto.ContentType);
                }
            }
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await _context.Actores.AnyAsync(i => i.Id == id);

            if (!existe)
                return NotFound();

            _context.Remove(new Actor() { Id = id });
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
