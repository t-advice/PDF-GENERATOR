PDFTutorial: .NET MAUI PDF Generation using iText7

This project demonstrates how to generate a structured PDF document from an application's data model using the .NET MAUI framework and the iText7 PDF library.

The application displays details about a Car object and allows the user to click a button to generate a clean, formatted PDF report containing those details.

## Features

Cross-Platform PDF Generation: Generate PDFs that work across Android, iOS, Windows, and macOS.

Data-Driven Reports: Use C# model data (Car.cs) to dynamically populate the PDF content.

Simple UI: A straightforward UI (MainPage.xaml) to display the car data and trigger the generation process.

## Prerequisites

.NET 8 SDK or newer

Visual Studio 2022 (with the ".NET Multi-platform App UI development" workload installed)

# Setup and Installation

1. Clone the Repository

git clone [Your-Repo-Link]/PDFTutorial
cd PDFTutorial


2. Install NuGet Packages

This project relies on the iText library for PDF manipulation. You need to install the following NuGet packages in your PDFTutorial project:

iText Core: The main library for basic PDF generation.

dotnet add package iText


iText7 Layout (Recommended): Used for higher-level document layout features like paragraphs, lists, and tables.

dotnet add package iText7.Layout


(You can also install these packages using the NuGet Package Manager UI in Visual Studio.)

## How to Generate the PDF (Core Logic)

The main logic for PDF generation is handled in the MainPage.xaml.cs file using the iText.Kernel.Pdf and iText.Layout namespaces.

Key Steps in the C# Code:

Define the File Path: Determine a location to save the PDF. On mobile devices, this is often the application's external storage or a public folder (e.g., FileSystem.Current.AppDataDirectory).

Initialize iText Document: Create a PdfWriter, a PdfDocument, and a Document object to begin writing the content.




Close and Open: Close the Document and then use Launcher.OpenAsync() to open the generated PDF file using the device's default PDF viewer.

## 📂 Project Structure

PDFTutorial/
├── Models/
│   └── Car.cs          # The C# data model for the car.
├── Resources/
├── Platforms/
└── MainPage.xaml
└── MainPage.xaml.cs    # Contains the PDF generation logic.
## MADE BY TASHWILL, UWC , 2025
