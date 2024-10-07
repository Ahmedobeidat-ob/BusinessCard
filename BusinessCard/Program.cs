
using BusinessCard.Core.Configrations;
using BusinessCard.Core.Data;
using BusinessCard.Core.IRepository;
using BusinessCard.Core.IService;
using BusinessCard.Infra.Repository;
using BusinessCard.Infra.Service;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BusinessCard
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            // Add services to the container.
            if (ConnectionString == null)
            {
                // Handle the case where the connection string is not found
                Console.WriteLine("Connection string not found.");
            }
            else
            {
                // Attempt to open a connection to the database
                using (var connection = new SqlConnection(ConnectionString))
                {
                    try
                    {
                        connection.Open();
                        Console.WriteLine("Connection opened successfully.");
                        // Connection is open, you can perform your database operations here

                        // Always close the connection when done
                        connection.Close();
                    }
                    catch (Exception ex)
                    {
                        // Handle any errors that occur when trying to open the connection
                        Console.WriteLine("An error occurred while trying to open the connection: " + ex.Message);
                    }
                }
            }
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IBusinessCardsService, BusinessCardsService>();
            builder.Services.AddScoped<IBusinessCardsRepository, BusinessCardsRepository>();
            builder.Services.AddControllers();
            builder.Services.AddDbContext<BusinessCardDbContext>(option =>
            {
                option.UseSqlServer(ConnectionString);
            });
            builder.Services.AddAutoMapper(typeof(MapperConfig));






            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}