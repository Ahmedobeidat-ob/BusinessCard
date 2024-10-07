using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCard.Core.Data
{
    public class BusinessCards
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string? Address { get; set; }
        public string? Photo { get; set; } // Base64 encoded photo stored as a byte array
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Optional: method to convert Photo to Base64
      
    }
}
