using AutoMapper;
using BusinessCard.API.Controllers;
using BusinessCard.Core.Data;
using BusinessCard.Core.DTO;
using BusinessCard.Core.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Text;

namespace BusinessCardsControllerTests
{
    public class BusinessCardsControllerTests
    {

        private readonly Mock<IBusinessCardsService> _mockService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly BusinessCardsController _controller;

        public BusinessCardsControllerTests()
        {
            _mockService = new Mock<IBusinessCardsService>();
            _mockMapper = new Mock<IMapper>();
            _controller = new BusinessCardsController(_mockMapper.Object, _mockService.Object, null);
        }



        [Fact]
        public async Task CreateBusinessCards_ReturnsCreatedAtAction_WithValidCard()
        {
            // Arrange
            var dto = new BusinessCardsDTo
            {
                Name = "islam",
                Gender = "male",
                DateOfBirth = DateTime.Parse("2024-10-07T11:48:58.612"),  // Convert to DateTime
                Email = "islam@gmail.com",
                Phone = "078888888",
                Address = "amman",
                Photo = "System.Byte[]",
                CreatedAt = DateTime.Parse("2024-10-07T11:48:58.612")
            };

            var card = new BusinessCards
            {
                Id = 1,
                Name = "islam",
                Gender = "male",
                DateOfBirth = DateTime.Parse("2024-10-07T11:48:58.612"),  // Convert to DateTime
                Email = "islam@gmail.com",
                Phone = "078888888",
                Address = "amman",
                Photo = "System.Byte[]",
                CreatedAt = DateTime.Parse("2024-10-07T11:48:58.612")
            };

            // Mock the mapping to return the BusinessCards object
            _mockMapper.Setup(m => m.Map<BusinessCards>(dto)).Returns(card);

            // Set up AddAsync to return the created card wrapped in a Task
            _mockService.Setup(s => s.AddAsync(It.IsAny<BusinessCards>())).ReturnsAsync(card); // Correctly return the task

            // Act
            var result = await _controller.CreateBusinessCards(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedCard = Assert.IsType<BusinessCards>(createdResult.Value);
            Assert.Equal(1, returnedCard.Id);
            Assert.Equal("islam", returnedCard.Name); // Adjusted expected name to match the card
        }




        [Fact]
        public async Task GetCards_ReturnsOk_WithListOfCards()
        {
            // Arrange
            var cards = new List<BusinessCards>
    {
        new BusinessCards { Id = 1, Name = "John Doe" },
        new BusinessCards { Id = 2, Name = "Jane Smith" }
    };
            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(cards);

            // Act
            var result = await _controller.GetCards();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCards = Assert.IsAssignableFrom<IEnumerable<BusinessCards>>(okResult.Value);
            Assert.Equal(2, returnedCards.Count());
        }

        [Fact]
        public async Task GetCards_ReturnsOk_WithEmptyList()
        {
            // Arrange
            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<BusinessCards>());

            // Act
            var result = await _controller.GetCards();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCards = Assert.IsAssignableFrom<IEnumerable<BusinessCards>>(okResult.Value);
            Assert.Empty(returnedCards);
        }



        [Fact]
        public async Task GetCardById_ReturnsOk_WithValidCard()
        {
            // Arrange
            var card = new BusinessCards
            {
                Id = 2,
                Name = "islam",
                Gender = "male",
                DateOfBirth = DateTime.Parse("2024-10-07T11:48:58.612"),  // Convert to DateTime
                Email = "islam@gmail.com",
                Phone = "078888888",
                Address = "amman",
                Photo = "System.Byte[]",
                CreatedAt = DateTime.Parse("2024-10-07T11:48:58.612")  
            };
            _mockService.Setup(c => c.GetAsync(2)).ReturnsAsync(card);

           // Act
            var result = await _controller.GetCardById(2);
            Console.WriteLine(result);  // For debugging the result

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCard = Assert.IsType<BusinessCards>(okResult.Value);
            Assert.Equal(2, returnedCard.Id);
            Assert.Equal("islam", returnedCard.Name);
        }

        [Fact]
        public async Task GetCardById_ReturnsNotFound_WhenCardDoesNotExist()
        {
            // Arrange
            _mockService.Setup(c => c.GetAsync(It.IsAny<int>())).ReturnsAsync((BusinessCards)null);

            // Act
            var result = await _controller.GetCardById(1);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetCardById_ReturnsBadRequest_WhenIdIsInvalid()
        {
            // Act
            var result = await _controller.GetCardById(-1);  // Invalid ID

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }


        [Fact]
        public async Task DeleteCard_ReturnsNoContent_WhenCardDeletedSuccessfully()
        {
            // Arrange
            _mockService.Setup(s => s.GetAsync(1)).ReturnsAsync(new BusinessCards { Id = 1 });
            _mockService.Setup(s => s.DeleteAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteCard(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteCard_ReturnsNotFound_WhenCardDoesNotExist()
        {
            // Arrange
            _mockService.Setup(s => s.GetAsync(1)).ReturnsAsync((BusinessCards)null);

            // Act
            var result = await _controller.DeleteCard(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteCard_ReturnsBadRequest_WhenIdIsInvalid()
        {
            // Act
            var result = await _controller.DeleteCard(-1);  // Invalid ID

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }



        [Fact]
        public async Task ExportToCsv_ReturnsFile_WithValidContent()
        {
            // Arrange
            var fileContent = new byte[] { 0x1, 0x2, 0x3 };
            _mockService.Setup(s => s.ExportToCsvAsync()).ReturnsAsync(fileContent);

            // Act
            var result = await _controller.ExportToCsv();

            // Assert
            var fileResult = Assert.IsType<FileContentResult>(result);
            Assert.Equal("text/csv", fileResult.ContentType);
        }

        [Fact]
        public async Task ExportToXml_ReturnsFile_WithValidContent()
        {
            // Arrange
            var fileContent = new byte[] { 0x1, 0x2, 0x3 };
            _mockService.Setup(s => s.ExportToXmlAsync()).ReturnsAsync(fileContent);

            // Act
            var result = await _controller.ExportToXml();

            // Assert
            var fileResult = Assert.IsType<FileContentResult>(result);
            Assert.Equal("application/xml", fileResult.ContentType);
        }


        [Fact]
        public async Task ImportCsv_ReturnsOk_WhenFileIsUploaded()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            var fileName = "test.csv";
            var fileContent = "Name,Gender,DateOfBirth,Email,Phone,Address,Photo,CreatedAt\r\nislam,male,2024-10-07,islam@gmail.com,078888888,amman,System.Byte[],2024-10-07T11:48:58\r\n";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            await writer.WriteAsync(fileContent);
            await writer.FlushAsync();
            stream.Position = 0; // Reset the stream position to the beginning

            // Mock IFormFile behavior
            mockFile.Setup(f => f.OpenReadStream()).Returns(stream);
            mockFile.Setup(f => f.FileName).Returns(fileName);
            mockFile.Setup(f => f.Length).Returns(stream.Length);

            // Act
            var result = await _controller.ImportCsv(mockFile.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("CSV file imported successfully.", okResult.Value);

            // Verify that ImportFromCsvAsync was called once with the correct IFormFile
            _mockService.Verify(s => s.ImportFromCsvAsync(It.IsAny<IFormFile>()), Times.Once);
        }




        [Fact]
        public async Task ImportCsv_ReturnsBadRequest_WhenNoFileUploaded()
        {
            // Arrange
            IFormFile file = null;

            // Act
            var result = await _controller.ImportCsv(file);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("No file uploaded.", badRequestResult.Value);  // Update this line to match the actual message
        }

        [Fact]
        public async Task ImportCsv_ReturnsBadRequest_WhenFileIsInvalid()
        {
            // Act
            var result = await _controller.ImportCsv(null);  // Invalid file

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }



        [Fact]
        public async Task ImportXml_ReturnsOk_WhenFileIsUploaded()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            var fileName = "test.xml";
            var fileContent = "<BusinessCards><Card><Name>Islam</Name></Card></BusinessCards>";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            await writer.WriteAsync(fileContent);
            await writer.FlushAsync();
            stream.Position = 0; // Reset the stream position to the beginning

            mockFile.Setup(f => f.OpenReadStream()).Returns(stream);
            mockFile.Setup(f => f.FileName).Returns(fileName);
            mockFile.Setup(f => f.Length).Returns(stream.Length);

            // Mock the service to handle the import operation
            _mockService.Setup(s => s.ImportFromXmlAsync(mockFile.Object)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ImportXml(mockFile.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("XML file imported successfully.", okResult.Value);
            _mockService.Verify(s => s.ImportFromXmlAsync(mockFile.Object), Times.Once); // Verify that the service method was called once
        }


        [Fact]
        public async Task ImportXml_ReturnsBadRequest_WhenExceptionIsThrown()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            var fileName = "test.xml";
            var stream = new MemoryStream();
            mockFile.Setup(f => f.OpenReadStream()).Returns(stream);
            mockFile.Setup(f => f.FileName).Returns(fileName);
            mockFile.Setup(f => f.Length).Returns(stream.Length);

            // Mock the service to throw an exception
            _mockService.Setup(s => s.ImportFromXmlAsync(mockFile.Object)).ThrowsAsync(new Exception("Import failed"));

            // Act
            var result = await _controller.ImportXml(mockFile.Object);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Error importing XML file: Import failed", badRequestResult.Value);
        }


    }
}