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

            // 1. Initialize the data model
            currentCar = new Car();

            // 2. Bind the data to the XAML labels (basic, non-MVVM binding for simplicity)
            UpdateUI();
        }

        /// <summary>
        /// Populates the XAML labels with the current car data.
        /// </summary>
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
        /// Event handler for the "Generate PDF" button.
        /// </summary>
        private async void OnGeneratePDFClicked(object sender, EventArgs e)
        {
            ActivitySpinner.IsRunning = true;
            StatusLabel.Text = "Generating PDF...";

            // Ensure permissions are handled in a real-world app, especially for external storage.

            try
            {
                // This call should be wrapped in a Task.Run to prevent UI blocking, 
                // especially for complex PDF generation.
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
                // Log the error and display a user-friendly message
                Console.WriteLine($"Error during PDF generation: {ex}");
                StatusLabel.Text = $"Error generating PDF: {ex.Message}";
                await DisplayAlert("Error", "Could not generate or save the PDF file.", "OK");
            }
            finally
            {
                ActivitySpinner.IsRunning = false;
            }
        }

        /// <summary>
        /// The core logic for creating the PDF using iText7.
        /// </summary>
        /// <param name="car">The Car object containing data to be rendered.</param>
        /// <returns>The full path to the saved PDF file.</returns>
        private string GenerateCarPDF(Car car)
        {
            // 1. Define the save path
            // Use FileSystem.Current.AppDataDirectory for reliable, app-private storage
            string fileName = $"CarReport_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            string filePath = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, fileName);

            // Must dispose of writer and document to finalize the file handle
            using (var writer = new PdfWriter(filePath))
            using (var pdf = new PdfDocument(writer))
            {
                // Set page size to A4
                var document = new Document(pdf, PageSize.A4);
                document.SetMargins(50, 50, 50, 50); // Top, Right, Bottom, Left

                // --- 2. Add Content: Title ---
                var title = new Paragraph("Vehicle Specifications Report")
                    .SetTextAlignment(iTextTextAlignment.CENTER)
                    .SetFontSize(24)
                    .SetFont(iText.Kernel.Font.PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD))
                    .SetFontColor(iText.Kernel.Colors.ColorConstants.BLUE)
                    .SetMarginBottom(20);
                document.Add(title);

                // --- 3. Add Content: Introduction and Basic Details ---
                document.Add(new Paragraph($"Report generated on: {DateTime.Now:MMMM dd, yyyy}"));
                document.Add(new Paragraph($"VIN: {car.VIN}")
                    .SetFontSize(14)
                    .SetFont(iText.Kernel.Font.PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD))
                    .SetMarginBottom(30));

                // --- 4. Add Content: Detailed Table (using a 2-column layout) ---
                float[] columnWidths = { 200f, 300f };
                Table table = new Table(UnitValue.CreatePercentArray(columnWidths))
                    .SetWidth(UnitValue.CreatePercentValue(80))
                    .SetHorizontalAlignment(iTextHorizontalAlignment.CENTER);

                // Helper to add a formatted row
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

                // --- 5. Add Content: Footer ---
                document.Add(new Paragraph("\n\n-- End of Report --")
                    .SetTextAlignment(iTextTextAlignment.CENTER)
                    .SetFontSize(12)
                    .SetFontColor(iText.Kernel.Colors.ColorConstants.GRAY));

                document.Close();
            }

            return filePath;
        }

        /// <summary>
        /// Uses the MAUI Launcher to open the generated PDF file with the system's default viewer.
        /// </summary>
        /// <param name="filePath">The path to the file to open.</param>
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

