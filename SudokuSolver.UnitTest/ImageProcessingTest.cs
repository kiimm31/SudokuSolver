using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SixLabors.ImageSharp;
using SudokuSolver.Core.Interfaces;
using SudokuSolver.Core.Models;
using SudokuSolver.Core.Services;
using SudokuSolver.Domain.Models;
using SudokuSolver.UnitTest.Base;

namespace SudokuSolver.UnitTest;

public class ImageProcessingTest : BaseTest
{
    private readonly ILogger<ImageProcessingService> _imageProcessorLogger;
    private readonly ILogger<GridDetectionService> _gridDetectorLogger;
    private readonly ILogger<OCRService> _ocrServiceLogger;
    private readonly ILogger<ImageUploadService> _imageUploadServiceLogger;

    public ImageProcessingTest()
    {
        _imageProcessorLogger = Substitute.For<ILogger<ImageProcessingService>>();
        _gridDetectorLogger = Substitute.For<ILogger<GridDetectionService>>();
        _ocrServiceLogger = Substitute.For<ILogger<OCRService>>();
        _imageUploadServiceLogger = Substitute.For<ILogger<ImageUploadService>>();
    }

    [Test]
    public async Task ImageToGrid_CompletePipeline_ShouldExtractAndSolve()
    {
        // Arrange
        // This test uses the actual sampleGrid.png file to test the complete pipeline
        var imageStream = LoadSampleGridImage();
        
        var processingOptions = ImageProcessingOptions.Default;
        
        // Create the actual services
        var imageProcessor = new ImageProcessingService(_imageProcessorLogger);
        var gridDetector = new GridDetectionService(_gridDetectorLogger);
        var ocrService = new OCRService(_ocrServiceLogger);
        
        // Initialize the OCR service
        var initResult = await ocrService.InitializeAsync("mock-tessdata-path");
        initResult.Should().BeTrue();
        
        var imageUploadService = new ImageUploadService(_imageUploadServiceLogger, imageProcessor, gridDetector, ocrService);
        
        // Expected grid data from the sample image
        var expectedGridData = new int[9, 9]
        {
            { 0, 0, 0, 0, 7, 0, 0, 0, 3 },
            { 5, 8, 0, 0, 3, 0, 0, 0, 0 },
            { 0, 0, 2, 9, 0, 6, 8, 1, 0 },
            { 0, 2, 0, 0, 5, 0, 0, 0, 0 },
            { 3, 0, 6, 0, 0, 0, 4, 0, 5 },
            { 0, 9, 0, 0, 0, 0, 0, 7, 0 },
            { 0, 0, 0, 0, 6, 0, 0, 0, 1 },
            { 0, 0, 0, 0, 0, 1, 7, 2, 4 },
            { 0, 0, 0, 3, 0, 0, 0, 0, 0 }
        };

        // Act
        // Use the complete image upload service pipeline
        // Create a fresh stream for the processing to avoid stream position issues
        var processingStream = CreateFreshImageStream();
        
        // Test individual components first
        try
        {
            Console.WriteLine("Testing image loading...");
            var testImage = await Image.LoadAsync(processingStream);
            Console.WriteLine($"Image loaded successfully: {testImage.Width}x{testImage.Height}");
            
            // Reset stream for the full pipeline test
            processingStream = CreateFreshImageStream();
            
            Console.WriteLine("Testing full pipeline...");
            var result = await imageUploadService.ProcessImageUploadAsync(processingStream, processingOptions);

            // Debug: Print error details if the result failed
            if (!result.IsSuccess)
            {
                Console.WriteLine("=== Image Processing Pipeline Failed ===");
                Console.WriteLine($"Error Type: {result.Error?.ErrorType}");
                Console.WriteLine($"Error Message: {result.Error?.Message}");
                Console.WriteLine($"Error Details: {result.Error?.Details}");
                Console.WriteLine($"Error Suggestion: {result.Error?.Suggestion}");
                Console.WriteLine($"Error Code: {result.Error?.ErrorCode}");
                Console.WriteLine($"Processing Time: {result.ProcessingTimeMs}ms");
            }

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.ExtractedGrid.Should().NotBeNull();
            
            // Verify the extracted grid can be solved
            var solvedGrid = SolveGrid(result.ExtractedGrid);
            solvedGrid.IsSolved().Should().BeTrue();
            
            // Verify all cells are filled with valid digits
            var allCells = solvedGrid.GetAllCells();
            allCells.Should().NotBeNull();
            allCells!.Count.Should().Be(81);
            allCells.All(cell => cell.Value >= 1 && cell.Value <= 9).Should().BeTrue();
            
            Console.WriteLine("=== Image Processing Pipeline Test Results ===");
            Console.WriteLine("Original Extracted Grid:");
            Console.WriteLine(result.ExtractedGrid.ToString());
            Console.WriteLine();
            Console.WriteLine("Solved Grid:");
            Console.WriteLine(solvedGrid.ToString());
            Console.WriteLine();
            Console.WriteLine($"Processing Time: {result.ProcessingTimeMs}ms");
            Console.WriteLine($"Overall Confidence: {result.OverallConfidence:P}");
            Console.WriteLine($"Low Confidence Cells: {result.LowConfidenceCells}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"=== Exception caught: {ex.GetType().Name} ===");
            Console.WriteLine($"Message: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
            throw;
        }
        
        // Clean up
        ocrService.Dispose();
    }

    [Test]
    public async Task ImageProcessing_WithLowConfidence_ShouldHandleGracefully()
    {
        // Arrange
        var imageStream = LoadSampleGridImage();
        var processingOptions = ImageProcessingOptions.Default;
        processingOptions.ConfidenceThreshold = 0.9f; // High threshold

        // Create the actual services
        var imageProcessor = new ImageProcessingService(_imageProcessorLogger);
        var gridDetector = new GridDetectionService(_gridDetectorLogger);
        var ocrService = new OCRService(_ocrServiceLogger);
        
        // Initialize the OCR service
        var initResult = await ocrService.InitializeAsync("mock-tessdata-path");
        initResult.Should().BeTrue();
        
        var imageUploadService = new ImageUploadService(_imageUploadServiceLogger, imageProcessor, gridDetector, ocrService);

        // Act
        // Create a fresh stream for the processing to avoid stream position issues
        var processingStream = CreateFreshImageStream();
        var result = await imageUploadService.ProcessImageUploadAsync(processingStream, processingOptions);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        // Should have some low-confidence results due to high threshold
        result.LowConfidenceCells.Should().BeGreaterThan(0);
        
        // Should provide suggestions for manual correction
        result.OCRResults.Where(r => !r.IsConfident).All(r => !string.IsNullOrEmpty(r.RawText)).Should().BeTrue();
        
        // Clean up
        ocrService.Dispose();
    }

    [Test]
    public async Task ImageProcessing_InvalidImage_ShouldReturnError()
    {
        // Arrange
        var invalidImageStream = CreateInvalidImageStream();
        var processingOptions = ImageProcessingOptions.Default;

        // Create the actual services
        var imageProcessor = new ImageProcessingService(_imageProcessorLogger);
        var gridDetector = new GridDetectionService(_gridDetectorLogger);
        var ocrService = new OCRService(_ocrServiceLogger);
        var imageUploadService = new ImageUploadService(_imageUploadServiceLogger, imageProcessor, gridDetector, ocrService);

        // Act
        var result = await imageUploadService.ProcessImageUploadAsync(invalidImageStream, processingOptions);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.ErrorType.Should().Be("ValidationError");
        
        // Clean up
        ocrService.Dispose();
    }

    [Test]
    public async Task ImageProcessing_SampleGrid_ShouldLoadSuccessfully()
    {
        // Arrange
        var imageStream = LoadSampleGridImage();

        // Act
        var image = await Image.LoadAsync(imageStream);

        // Assert
        image.Should().NotBeNull();
        image.Width.Should().BeGreaterThan(0);
        image.Height.Should().BeGreaterThan(0);
        
        Console.WriteLine($"Sample grid image loaded successfully:");
        Console.WriteLine($"Dimensions: {image.Width}x{image.Height}");
        Console.WriteLine($"Format: {image.Metadata.DecodedImageFormat?.Name ?? "Unknown"}");
    }

    [Test]
    public async Task ImageProcessing_IndividualComponents_ShouldWork()
    {
        // Arrange
        var processingStream = CreateFreshImageStream();
        var processingOptions = ImageProcessingOptions.Default;
        
        // Create the actual services
        var imageProcessor = new ImageProcessingService(_imageProcessorLogger);
        var gridDetector = new GridDetectionService(_gridDetectorLogger);
        var ocrService = new OCRService(_ocrServiceLogger);
        
        // Initialize the OCR service
        var initResult = await ocrService.InitializeAsync("mock-tessdata-path");
        initResult.Should().BeTrue();
        
        // Test individual components
        try
        {
            // Test 1: Image loading
            Console.WriteLine("Test 1: Image loading...");
            var testImage = await Image.LoadAsync(processingStream);
            Console.WriteLine($"✓ Image loaded successfully: {testImage.Width}x{testImage.Height}");
            
            // Test 2: Image preprocessing
            Console.WriteLine("Test 2: Image preprocessing...");
            var freshStream1 = CreateFreshImageStream();
            var processedImage = await imageProcessor.PreprocessImageAsync(freshStream1, processingOptions);
            Console.WriteLine($"✓ Image preprocessing successful: {processedImage.Width}x{processedImage.Height}");
            
            // Test 3: Grid detection
            Console.WriteLine("Test 3: Grid detection...");
            var gridResult = await gridDetector.DetectGridAsync(processedImage);
            Console.WriteLine($"✓ Grid detection result: IsDetected={gridResult.IsDetected}, Confidence={gridResult.Confidence}");
            
            // Test 4: Cell extraction
            Console.WriteLine("Test 4: Cell extraction...");
            var cellImages = await gridDetector.ExtractCellsAsync(processedImage, gridResult.GridBounds);
            Console.WriteLine($"✓ Cell extraction successful: {cellImages.Length} cells");
            
            // Test 5: OCR processing
            Console.WriteLine("Test 5: OCR processing...");
            var ocrResults = await ocrService.RecognizeDigitsAsync(cellImages, processingOptions.ConfidenceThreshold);
            Console.WriteLine($"✓ OCR processing successful: {ocrResults.Length} results");
            
            Console.WriteLine("All individual components are working correctly!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Component test failed: {ex.GetType().Name} - {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"  Inner exception: {ex.InnerException.Message}");
            }
            throw;
        }
        
        // Clean up
        ocrService.Dispose();
    }

    #region Helper Methods (These will be replaced by actual service implementations)

    private Stream LoadSampleGridImage()
    {
        // Load the actual sampleGrid.png file from the test project
        var testDirectory = TestContext.CurrentContext.TestDirectory;
        var imagePath = Path.Combine(testDirectory, "sampleGrid.png");
        
        if (!File.Exists(imagePath))
        {
            throw new FileNotFoundException($"Sample grid image not found at: {imagePath}");
        }
        
        var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
        return stream;
    }

    private Stream CreateInvalidImageStream()
    {
        // Create an invalid image stream
        var stream = new MemoryStream();
        var invalidData = new byte[] { 0x00, 0x01, 0x02, 0x03 }; // Invalid image data
        stream.Write(invalidData, 0, invalidData.Length);
        stream.Position = 0;
        return stream;
    }

    private Stream CreateFreshImageStream()
    {
        // Create a fresh stream for image processing
        var testDirectory = TestContext.CurrentContext.TestDirectory;
        var imagePath = Path.Combine(testDirectory, "sampleGrid.png");
        
        if (!File.Exists(imagePath))
        {
            throw new FileNotFoundException($"Sample grid image not found at: {imagePath}");
        }
        
        // Read the file into a memory stream
        var fileBytes = File.ReadAllBytes(imagePath);
        var stream = new MemoryStream(fileBytes);
        return stream;
    }

    private async Task<Image> ProcessImageAsync(Stream imageStream, ImageProcessingOptions options)
    {
        // Simplified version - just load the image
        var image = await Image.LoadAsync(imageStream);
        return image;
    }

    private async Task<GridDetectionResult> DetectGridAsync(Image image)
    {
        // TODO: This will be implemented by GridDetectionService
        // For now, return a mock result based on the image dimensions
        var gridSize = Math.Min(image.Width, image.Height) * 8 / 10; // Assume grid takes 80% of image
        var offset = (Math.Min(image.Width, image.Height) - gridSize) / 2;
        
        var gridBounds = new SixLabors.ImageSharp.Rectangle(offset, offset, gridSize, gridSize);
        
        return await Task.FromResult(GridDetectionResult.CreateSuccess(
            gridBounds, 
            0.95f, 
            "MockDetection"));
    }

    private async Task<Image[]> ExtractCellsAsync(Image image, SixLabors.ImageSharp.Rectangle gridBounds)
    {
        // Simplified version - return the original image for each cell
        var cells = new Image[81];
        for (int i = 0; i < 81; i++)
        {
            cells[i] = image;
        }
        
        return await Task.FromResult(cells);
    }

    private async Task<OCRResult[]> PerformOCRAsync(Image[] cellImages, float confidenceThreshold)
    {
        // TODO: This will be implemented by OCRService
        // For now, return mock OCR results based on the expected grid data
        var expectedData = new int[9, 9]
        {
            { 0, 0, 0, 0, 7, 0, 0, 0, 3 },
            { 5, 8, 0, 0, 3, 0, 0, 0, 0 },
            { 0, 0, 2, 9, 0, 6, 8, 1, 0 },
            { 0, 2, 0, 0, 5, 0, 0, 0, 0 },
            { 3, 0, 6, 0, 0, 0, 4, 0, 5 },
            { 0, 9, 0, 0, 0, 0, 0, 7, 0 },
            { 0, 0, 0, 0, 6, 0, 0, 0, 1 },
            { 0, 0, 0, 0, 0, 1, 7, 2, 4 },
            { 0, 0, 0, 3, 0, 0, 0, 0, 0 }
        };

        var results = new OCRResult[81];
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                var cellIndex = row * 9 + col;
                var digit = expectedData[row, col];
                
                if (digit == 0)
                {
                    results[cellIndex] = OCRResult.CreateEmpty(cellIndex, row, col);
                }
                else
                {
                    results[cellIndex] = OCRResult.CreateDigit(digit, 0.95f, digit.ToString(), cellIndex, row, col, confidenceThreshold);
                }
            }
        }
        
        return await Task.FromResult(results);
    }

    private Grid ConvertOCRResultsToGrid(OCRResult[] ocrResults)
    {
        var cells = new List<Cell>();
        
        for (int i = 0; i < ocrResults.Length; i++)
        {
            var result = ocrResults[i];
            var row = (i / 9) + 1;
            var col = (i % 9) + 1;
            
            var cell = new Cell { Row = row, Column = col };
            if (result.RecognizedDigit > 0)
            {
                cell.SetValue(result.RecognizedDigit);
            }
            
            cells.Add(cell);
        }
        
        return new Grid(cells);
    }

    private Grid SolveGrid(Grid grid)
    {
        // Use the existing solver to solve the grid
        var solver = SudokuSolver.Core.Factories.SudokuSolverFactory.CreateClassicSolver(grid);
        return solver.Solve();
    }

    private void VerifyExtractedGrid(Grid extractedGrid, int[,] expectedData)
    {
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                var cell = extractedGrid.GetCell(row + 1, col + 1);
                cell.Should().NotBeNull();
                cell!.Value.Should().Be(expectedData[row, col]);
            }
        }
    }

    #endregion
} 