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
    public class GenerosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GenerosController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<GeneroDTO>>> Get()
        {
            var generoEntidad = await _context.Generos.ToListAsync();
            var dto = _mapper.Map<List<GeneroDTO>>(generoEntidad);

            return dto;
        }

        [HttpGet("{id:int}", Name = "ObtenerGenero")]
        public async Task<ActionResult<GeneroDTO>> Get(int id)
        {
            var generoEntidad = await _context.Generos.FirstOrDefaultAsync(i => i.Id == id);

            if (generoEntidad == null)
                return NotFound();

            var dto = _mapper.Map<GeneroDTO>(generoEntidad);

            return dto;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] GeneroCreacionDTO generoCreacion)
        {
            var generoEntidad = _mapper.Map<Genero>(generoCreacion); //Mapea la entidad para agregar el nuevo campo
            _context.Add(generoEntidad); // Se agrega 
            await _context.SaveChangesAsync(); // Guarda los cambios
            var dto = _mapper.Map<GeneroDTO>(generoEntidad); //Nuevo mapeo para retornar el creado

            return new CreatedAtRouteResult("ObtenerGenero", new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] GeneroCreacionDTO generoCreacionDTO)
        {
            var generoEntidad = _mapper.Map<Genero>(generoCreacionDTO);
            generoEntidad.Id = id;
            _context.Entry(generoEntidad).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await _context.Generos.AnyAsync(i => i.Id == id);

            if (!existe)
                return NotFound();

            _context.Remove(new Genero() { Id = id });
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
