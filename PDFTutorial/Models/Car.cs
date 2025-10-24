using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFTutorial.Models
{
    public class Car
    {
        public string VIN { get; set; } = "XYZ1234567890ABC"; 
        public string Make { get; set; } = "Ford";
        public string Model { get; set; } = "Mustang GT";
        public int Year { get; set; } = 2024;
        public string Color { get; set; } = "Race Red";
        public double EngineSizeL { get; set; } = 5.0;
        public int Horsepower { get; set; } = 486;
        public decimal Price { get; set; } = 55995.00m;
    }
}
