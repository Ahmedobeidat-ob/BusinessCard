using AutoMapper;
using BusinessCard.Core.Data;
using BusinessCard.Core.DTO;
using BusinessCard.Core.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace BusinessCard.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessCardsController : ControllerBase
    {
        private readonly IBusinessCardsService _cardsService;
        private readonly IMapper _mapper;
        private readonly BusinessCardDbContext _Context;

        public BusinessCardsController(IMapper mapper, IBusinessCardsService cardsService, BusinessCardDbContext Context)
        {
            _mapper = mapper;
            _cardsService = cardsService;
            _Context = Context;

        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BusinessCards>>> GetCards()
        {

            var cards = await _cardsService.GetAllAsync();
          
            return Ok(cards);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BusinessCards>> GetCardById(int id)
        {
            try
            {
                // Validate the id
                if (id <= 0)
                {
                    return BadRequest("Invalid Id");
                }

                // Fetch the card from the service
                var card = await _cardsService.GetAsync(id);

                // Check if the card exists
                if (card == null)
                {
                    return NotFound();
                }

                // Return the card
                return Ok(card);
            }
            catch (Exception ex)
            {
                // Log the exception (if logging is configured)
                // Log.Error(ex, "Error fetching the business card");

                // Return a generic error message
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while fetching the business card.");
            }
        }


        [HttpPost]

        public async Task<ActionResult<BusinessCards>> CreateBusinessCards(BusinessCardsDTo businessCard)
        {
            var card = _mapper.Map<BusinessCards>(businessCard);
            await _cardsService.AddAsync(card);


            return CreatedAtAction("GetCardById", new { id =card.Id }, card);

        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCard(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Id");
            }


            var card = await _cardsService.GetAsync(id);
            if (card == null)
            {
                return NotFound();
            }

            await _cardsService.DeleteAsync(id);


            return NoContent();
        }

        [HttpGet("fillter")]
        public async Task<IActionResult> GetFilteredBusinessCards([FromQuery] FillterBusinessCardsDTo filter)
        {
            var result = await _cardsService.GetFilteredBusinessCardsAsync(filter);
            return Ok(result);
        }

        [HttpGet("export/csv")]
        public async Task<IActionResult> ExportToCsv()
        {
            var fileContents = await _cardsService.ExportToCsvAsync();
            return File(fileContents, "text/csv", "BusinessCards.csv");
        }

        [HttpGet("export/xml")]
        public async Task<IActionResult> ExportToXml()
        {
            var fileContents = await _cardsService.ExportToXmlAsync();
            return File(fileContents, "application/xml", "BusinessCards.xml");
        }

        [HttpPost("import/xml")]
        public async Task<IActionResult> ImportXml( IFormFile file)
        {
            try
            {
                await _cardsService.ImportFromXmlAsync(file);
                return Ok("XML file imported successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error importing XML file: {ex.Message}");
            }
        }



       

        [HttpPost("import/csv")]
        public async Task<IActionResult> ImportCsv(IFormFile file)
        {
            // Validate the file
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                await _cardsService.ImportFromCsvAsync(file);
                return Ok("CSV file imported successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error importing CSV file: {ex.Message}");
            }
        }



       






    }




}
