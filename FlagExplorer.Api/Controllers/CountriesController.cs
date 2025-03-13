using FlagExplorer.Application.Features.Countries.Queries;
using FlagExplorer.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FlagExplorer.Api.Controllers
{
    public class CountriesController(IMediator mediator, ILogger<CountriesController> logger) : BaseApiController(mediator)
    {
        /// <summary>
        /// Retrieves all countries
        /// </summary>
        /// <returns>A list of countries</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CountryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<CountryDto>>> GetAllCountries(CancellationToken cancellationToken)
        {
            logger.LogInformation("Getting all countries");
            var result = await Mediator.Send(new GetAllCountriesQuery(), cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves details about a specific country
        /// </summary>
        /// <param name="name">The name of the country</param>
        /// <returns>Details about the country</returns>
        [HttpGet("{name}")]
        [ProducesResponseType(typeof(CountryDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CountryDetailsDto>> GetCountryByName(string name, CancellationToken cancellationToken)
        {
            logger.LogInformation("Getting details for country: {Name}", name);

            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    logger.LogWarning("Empty or whitespace country name provided");
                    return BadRequest("Country name cannot be empty");
                }

                var result = await Mediator.Send(new GetCountryByNameQuery(name), cancellationToken);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogWarning(ex, "Country not found: {Name}", name);
                return NotFound($"Country '{name}' not found");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                logger.LogWarning(ex, "Country not found: {Name}", name);
                return NotFound($"Country '{name}' not found");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting country by name: {Name}", name);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving country details");
            }
        }
    }
}
