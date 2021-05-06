using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;
using System;
using System.Collections.Generic;
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

        public ActoresController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
            _context.Add(actorEntidad);
            await _context.SaveChangesAsync();
            var dto = _mapper.Map<ActorDTO>(actorEntidad);

            return new CreatedAtRouteResult("ObtenerActor", new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromForm] ActorCreacionDTO actorCreacionDTO)
        {
            var actorEntidad = _mapper.Map<Actor>(actorCreacionDTO);
            actorEntidad.Id = id;
            _context.Entry(actorEntidad).State = EntityState.Modified;
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
