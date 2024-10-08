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

            if (id <= 0)
            {
                return BadRequest("Invalid Id");
            }

            var card = await _cardsService.GetAsync(id);

            if (card == null)
            {
                return NotFound();
            }

            return card;
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
        public async Task<IActionResult> ImportCsv( IFormFile file)
        {
            // Validate the file
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is empty or not provided.");
            }

            try
            {
                // Pass the IFormFile directly to the service
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
