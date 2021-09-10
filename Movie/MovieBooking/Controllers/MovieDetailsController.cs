using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieBooking.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;

namespace MovieBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieDetailsController : ControllerBase
    {
        private readonly MovieContext _context;

        public MovieDetailsController(MovieContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("ViewAllBookings")]
        public async Task<ActionResult<IEnumerable<MovieDetail>>> GetMovieDetails()
        {
            return await _context.MovieDetails.ToListAsync();
        }

        [HttpGet]
        [Route("ViewBooking/{id}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<MovieDetail>> GetMovieDetail(long id)
        {
            var MovieDetail = await _context.MovieDetails.FindAsync(id);

            if (MovieDetail == null)
            {
                return NotFound();
            }

            return MovieDetail;
        }

        [HttpPut]
        [Route("UpdateBooking/{id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> PutMovieDetail(long id, MovieDetail MovieDetail)
        {
            if (id != MovieDetail.Id)
            {
                return BadRequest();
            }

            _context.Entry(MovieDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieDetailExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        [Route("AddBooking")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<ActionResult<MovieDetail>> PostMovieDetail(MovieDetail MovieDetail)
        {
            _context.MovieDetails.Add(MovieDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMovieDetail), new { id = MovieDetail.Id }, MovieDetail);
        }

        [HttpDelete]
        [Route("DeleteBooking/{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteMovieDetail(long id)
        {
            var MovieDetail = await _context.MovieDetails.FindAsync(id);
            if (MovieDetail == null)
            {
                return NotFound();
            }

            _context.MovieDetails.Remove(MovieDetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MovieDetailExists(long id)
        {
            return _context.MovieDetails.Any(e => e.Id == id);
        }

        [HttpGet]
        [Route("ConvertCurrency/{from}/{to}")]
        [ProducesResponseType(typeof(ConvertedValue), (int)HttpStatusCode.OK)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> ConvertCurrency(string from, string to)
        {
            ConvertedValue value = new ConvertedValue();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://cdn.jsdelivr.net/gh/fawazahmed0/currency-api@1/latest/currencies/"+ from.ToLower() + "/"+ to.ToLower() + ".json"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    value = JsonConvert.DeserializeObject<ConvertedValue>(apiResponse);
                }
            }
            return Ok(value);
        }
    }
}
