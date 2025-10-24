using PDFTutorial.Models;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Geom;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using PdfCell = iText.Layout.Element.Cell;
using iTextHorizontalAlignment = iText.Layout.Properties.HorizontalAlignment;
using iTextTextAlignment = iText.Layout.Properties.TextAlignment;

namespace PDFTutorial
{
    public partial class MainPage : ContentPage
    {
        private Car currentCar;

        public MainPage()
        {
            InitializeComponent();

            
            currentCar = new Car();

            
            UpdateUI();
        }

      
        private void UpdateUI()
        {
            VinLabel.Text = currentCar.VIN;
            MakeLabel.Text = currentCar.Make;
            ModelLabel.Text = currentCar.Model;
            YearLabel.Text = currentCar.Year.ToString();
            ColorLabel.Text = currentCar.Color;
            EngineLabel.Text = $"{currentCar.EngineSizeL:F1} L";
            HpLabel.Text = $"{currentCar.Horsepower} HP";
            PriceLabel.Text = $"{currentCar.Price:C}";
        }

        /// <summary>
       
        private async void OnGeneratePDFClicked(object sender, EventArgs e)
        {
            ActivitySpinner.IsRunning = true;
            StatusLabel.Text = "Generating PDF...";

            

            try
            {
                
                string filePath = await Task.Run(() => GenerateCarPDF(currentCar));

                if (!string.IsNullOrEmpty(filePath))
                {
                    StatusLabel.Text = $"PDF successfully saved to: {System.IO.Path.GetFileName(filePath)}";
                    await DisplayPDF(filePath);
                }
                else
                {
                    StatusLabel.Text = "PDF generation failed. File path is null.";
                }
            }
            catch (Exception ex)
            {
               
                Console.WriteLine($"Error during PDF generation: {ex}");
                StatusLabel.Text = $"Error generating PDF: {ex.Message}";
                await DisplayAlert("Error", "Could not generate or save the PDF file.", "OK");
            }
            finally
            {
                ActivitySpinner.IsRunning = false;
            }
        }

       
        private string GenerateCarPDF(Car car)
        {
            
            string fileName = $"CarReport_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            string filePath = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, fileName);

            
            using (var writer = new PdfWriter(filePath))
            using (var pdf = new PdfDocument(writer))
            {
                
                var document = new Document(pdf, PageSize.A4);
                document.SetMargins(50, 50, 50, 50); // Top, Right, Bottom, Left

                
                var title = new Paragraph("Vehicle Specifications Report")
                    .SetTextAlignment(iTextTextAlignment.CENTER)
                    .SetFontSize(24)
                    .SetFont(iText.Kernel.Font.PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD))
                    .SetFontColor(iText.Kernel.Colors.ColorConstants.BLUE)
                    .SetMarginBottom(20);
                document.Add(title);

                
                document.Add(new Paragraph($"Report generated on: {DateTime.Now:MMMM dd, yyyy}"));
                document.Add(new Paragraph($"VIN: {car.VIN}")
                    .SetFontSize(14)
                    .SetFont(iText.Kernel.Font.PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD))
                    .SetMarginBottom(30));

                
                float[] columnWidths = { 200f, 300f };
                Table table = new Table(UnitValue.CreatePercentArray(columnWidths))
                    .SetWidth(UnitValue.CreatePercentValue(80))
                    .SetHorizontalAlignment(iTextHorizontalAlignment.CENTER);

                
                void AddTableRow(string label, string value)
                {
                    table.AddCell(new PdfCell().Add(
                        new Paragraph(label)
                            .SetFont(iText.Kernel.Font.PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD))
                    ).SetBackgroundColor(iText.Kernel.Colors.ColorConstants.LIGHT_GRAY));
                    table.AddCell(new PdfCell().Add(new Paragraph(value)));
                }

                AddTableRow("Manufacturer", car.Make);
                AddTableRow("Model Name", car.Model);
                AddTableRow("Model Year", car.Year.ToString());
                AddTableRow("Exterior Color", car.Color);
                AddTableRow("Engine Displacement", $"{car.EngineSizeL:F1} Liters");
                AddTableRow("Max Horsepower", $"{car.Horsepower} HP");
                AddTableRow("Suggested Retail Price (MSRP)", $"{car.Price:C}");

                document.Add(table);

               
                document.Add(new Paragraph("\n\n-- End of Report --")
                    .SetTextAlignment(iTextTextAlignment.CENTER)
                    .SetFontSize(12)
                    .SetFontColor(iText.Kernel.Colors.ColorConstants.GRAY));

                document.Close();
            }

            return filePath;
        }

       
        private async Task DisplayPDF(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    // Use ReadOnlyFile to ensure permissions are handled correctly by the system launcher
                    await Launcher.OpenAsync(new OpenFileRequest
                    {
                        File = new ReadOnlyFile(filePath)
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error displaying PDF: {ex}");
                await DisplayAlert("View Error", "Could not open the PDF file. You can find it in the application's local data folder.", "OK");
            }
        }
    }
}

