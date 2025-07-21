# PLAN-002: Technical Specification for Image Upload and OCR Feature

## System Architecture Overview

```mermaid
graph TB
    subgraph "Frontend (React)"
        A[ImageUpload Component] --> B[File Selection]
        B --> C[FormData Creation]
        C --> D[API Call]
    end
    
    subgraph "API Layer (.NET)"
        E[SudokuController] --> F[Image Validation]
        F --> G[File Storage]
        G --> H[Core Service Call]
    end
    
    subgraph "Core Processing (.NET)"
        I[ImageUploadService] --> J[ImageProcessingService]
        J --> K[GridDetectionService]
        K --> L[OCRService]
        L --> M[Result Aggregation]
    end
    
    subgraph "Response Flow"
        M --> N[API Response]
        N --> O[Frontend Display]
        O --> P[User Confirmation]
    end
    
    D --> E
    H --> I
```

## Image Processing Pipeline

```mermaid
flowchart TD
    A[Uploaded Image] --> B{Format Validation}
    B -->|Valid| C[Image Preprocessing]
    B -->|Invalid| Z[Error Response]
    
    C --> D[Resize & Optimize]
    D --> E[Convert to Grayscale]
    E --> F[Enhance Contrast]
    F --> G[Noise Reduction]
    
    G --> H[Grid Detection]
    H --> I{Grid Found?}
    I -->|Yes| J[Extract 9x9 Cells]
    I -->|No| Y[Manual Grid Selection]
    
    J --> K[OCR Processing]
    K --> L[Digit Recognition]
    L --> M[Confidence Scoring]
    
    M --> N{Confidence > Threshold?}
    N -->|Yes| O[Grid Validation]
    N -->|No| X[Manual Correction]
    
    O --> P{Valid Sudoku?}
    P -->|Yes| Q[Success Response]
    P -->|No| R[Validation Error]
    
    X --> S[User Correction Interface]
    S --> O
    Y --> J
    R --> T[Error Response]
    Z --> T
```

## Core Service Architecture

```mermaid
classDiagram
    class IImageProcessor {
        +ProcessImage(stream) ImageProcessingResult
        +ValidateFormat(stream) bool
        +PreprocessImage(image) Image
    }
    
    class IGridDetector {
        +DetectGrid(image) GridDetectionResult
        +ExtractCells(image, bounds) Cell[]
        +ValidateGrid(cells) bool
    }
    
    class IOCRService {
        +InitializeEngine() void
        +RecognizeDigits(cells) OCRResult[]
        +GetConfidence(result) float
        +Dispose() void
    }
    
    class IImageUploadService {
        +ProcessImageUpload(stream) ImageUploadResult
        +ValidateResult(result) bool
        +HandleErrors(exception) ErrorResult
    }
    
    class ImageProcessingService {
        -imageProcessor: IImageProcessor
        +ProcessImage(stream) ImageProcessingResult
        +PreprocessForOCR(image) Image
    }
    
    class GridDetectionService {
        -gridDetector: IGridDetector
        +DetectSudokuGrid(image) GridDetectionResult
        +ExtractGridCells(image) Cell[]
    }
    
    class OCRService {
        -tesseractEngine: Engine
        +RecognizeDigits(cells) OCRResult[]
        +Initialize() void
        +Dispose() void
    }
    
    class ImageUploadService {
        -imageProcessor: ImageProcessingService
        -gridDetector: GridDetectionService
        -ocrService: OCRService
        +ProcessImageUpload(stream) ImageUploadResult
    }
    
    IImageProcessor <|.. ImageProcessingService
    IGridDetector <|.. GridDetectionService
    IOCRService <|.. OCRService
    IImageUploadService <|.. ImageUploadService
    
    ImageUploadService --> ImageProcessingService
    ImageUploadService --> GridDetectionService
    ImageUploadService --> OCRService
```

## Data Flow Sequence

```mermaid
sequenceDiagram
    participant U as User
    participant F as Frontend
    participant A as API
    participant C as Core
    participant O as OCR Engine
    
    U->>F: Select Image File
    F->>F: Validate File Format
    F->>A: Upload Image (FormData)
    A->>A: Validate File Size & Type
    A->>C: Process Image Upload
    
    C->>C: Image Preprocessing
    C->>C: Grid Detection
    C->>C: Cell Extraction
    
    loop For each cell
        C->>O: Recognize Digit
        O->>C: Return OCR Result
    end
    
    C->>C: Aggregate Results
    C->>C: Validate Sudoku Grid
    C->>A: Return Processed Result
    A->>F: Return API Response
    F->>U: Display Results
    
    alt Low Confidence
        U->>F: Manual Correction
        F->>A: Update Grid
        A->>F: Confirm Changes
    end
```

## Error Handling Flow

```mermaid
flowchart TD
    A[Image Upload] --> B{File Validation}
    B -->|Invalid| C[File Error Response]
    
    B -->|Valid| D[Processing Start]
    D --> E{Processing Success?}
    
    E -->|No| F[Error Classification]
    F --> G{Error Type}
    
    G -->|Format Error| H[Unsupported Format]
    G -->|Size Error| I[File Too Large]
    G -->|Grid Detection Error| J[Grid Not Found]
    G -->|OCR Error| K[Recognition Failed]
    G -->|Validation Error| L[Invalid Sudoku]
    
    H --> M[User Error Message]
    I --> M
    J --> N[Manual Grid Selection]
    K --> O[Manual Digit Entry]
    L --> P[Grid Correction Interface]
    
    E -->|Yes| Q[Success Response]
    
    N --> R[Retry Processing]
    O --> R
    P --> R
    R --> E
```

## Frontend Component Architecture

```mermaid
graph TB
    subgraph "SudokuGame Container"
        A[SudokuGame] --> B[SudokuGrid]
        A --> C[Controls]
        A --> D[ImageUpload]
    end
    
    subgraph "Image Upload Flow"
        D --> E[FileUpload]
        E --> F[ImagePreview]
        F --> G[OCRResults]
        G --> H[ConfirmationDialog]
    end
    
    subgraph "State Management"
        I[sudokuStore] --> J[imageUploadState]
        J --> K[uploadedImage]
        J --> L[extractedGrid]
        J --> M[confidence]
        J --> N[processingStatus]
    end
    
    D --> I
    F --> I
    G --> I
    H --> I
```

## API Endpoint Specification

### Image Upload Endpoint

```http
POST /api/sudoku/upload-image
Content-Type: multipart/form-data

Request:
- image: File (JPEG, PNG, WebP, max 10MB)
- options: ImageProcessingOptions (optional)

Response:
{
  "success": true,
  "data": {
    "extractedGrid": number[][],
    "confidence": number[][],
    "processingTime": number,
    "warnings": string[],
    "previewUrl": string
  },
  "correlationId": string
}
```

### Error Response Format

```json
{
  "success": false,
  "error": {
    "type": "ValidationError|ProcessingError|OCRError",
    "message": "Human readable error message",
    "details": {
      "field": "Specific field causing error",
      "value": "Invalid value",
      "suggestion": "Suggested fix"
    }
  },
  "correlationId": string
}
```

## Performance Considerations

### Image Processing Optimization

```mermaid
graph LR
    A[Original Image] --> B{Size > 2048px?}
    B -->|Yes| C[Resize to 2048px]
    B -->|No| D[Keep Original Size]
    C --> E[Grayscale Conversion]
    D --> E
    E --> F[Contrast Enhancement]
    F --> G[Grid Detection]
    G --> H[Cell Extraction]
    H --> I[OCR Processing]
```

### Memory Management Strategy

```mermaid
flowchart TD
    A[File Upload] --> B[Stream Processing]
    B --> C[Dispose Original Stream]
    C --> D[Image Processing]
    D --> E[Dispose Processed Image]
    E --> F[OCR Processing]
    F --> G[Dispose OCR Resources]
    G --> H[Return Results]
    H --> I[Cleanup Temporary Files]
```

## Testing Strategy

### Unit Test Coverage

```mermaid
graph TB
    subgraph "Core Services"
        A[ImageProcessingService] --> A1[Format Validation Tests]
        A --> A2[Preprocessing Tests]
        A --> A3[Error Handling Tests]
        
        B[GridDetectionService] --> B1[Grid Detection Tests]
        B --> B2[Cell Extraction Tests]
        B --> B3[Edge Case Tests]
        
        C[OCRService] --> C1[Digit Recognition Tests]
        C --> C2[Confidence Scoring Tests]
        C --> C3[Engine Lifecycle Tests]
    end
    
    subgraph "Integration Tests"
        D[End-to-End Pipeline] --> D1[Success Scenarios]
        D --> D2[Error Scenarios]
        D --> D3[Performance Tests]
    end
    
    subgraph "API Tests"
        E[Controller Tests] --> E1[Upload Endpoint Tests]
        E --> E2[Validation Tests]
        E --> E3[Error Response Tests]
    end
```

## Security Considerations

### File Upload Security

```mermaid
flowchart TD
    A[File Upload] --> B[File Type Validation]
    B --> C[File Size Check]
    C --> D[Content Analysis]
    D --> E[Virus Scan]
    E --> F[Processing]
    
    B -->|Invalid Type| G[Reject Upload]
    C -->|Too Large| H[Size Limit Error]
    D -->|Suspicious Content| I[Security Error]
    E -->|Virus Detected| J[Security Error]
```

## Implementation Checklist

### Phase 1: Core Infrastructure
- [ ] Add NuGet packages to Core project
- [ ] Create Core interfaces
- [ ] Create Core models
- [ ] Set up Tesseract training data
- [ ] Configure build settings

### Phase 2: Core Services
- [ ] Implement ImageProcessingService
- [ ] Implement GridDetectionService
- [ ] Implement OCRService
- [ ] Implement ImageUploadService
- [ ] Add unit tests for each service

### Phase 3: API Integration
- [ ] Add API models
- [ ] Enhance SudokuController
- [ ] Add image validation middleware
- [ ] Configure file upload settings
- [ ] Add integration tests

### Phase 4: Frontend Implementation
- [ ] Add TypeScript types
- [ ] Enhance API service
- [ ] Update Zustand store
- [ ] Create UI components
- [ ] Integrate with existing UI

### Phase 5: Testing & Optimization
- [ ] Complete unit test coverage
- [ ] Performance testing
- [ ] User acceptance testing
- [ ] Security testing
- [ ] Documentation updates

## Risk Mitigation Strategies

### OCR Accuracy Issues
- Implement confidence thresholds (reject < 70% confidence)
- Provide manual correction interface
- Use multiple OCR passes with different preprocessing
- Train Tesseract specifically for digit recognition

### Grid Detection Failures
- Implement multiple detection algorithms
- Allow manual grid boundary selection
- Provide visual feedback for detection results
- Support different grid styles and layouts

### Performance Issues
- Implement image resizing before processing
- Use async processing with progress indicators
- Add timeout mechanisms
- Implement caching for repeated operations

### Memory Management
- Proper disposal of image resources
- Streaming for large files
- Garbage collection optimization
- Memory usage monitoring

## Success Metrics

### Functional Metrics
- OCR accuracy > 90% for clear images
- Grid detection success rate > 95%
- Processing time < 10 seconds for standard images
- Error rate < 5% for valid uploads

### Performance Metrics
- Memory usage < 500MB for 10MB images
- CPU usage < 80% during processing
- Response time < 15 seconds total
- Concurrent upload support > 5 users

### User Experience Metrics
- User satisfaction > 4.5/5
- Manual correction rate < 10%
- Feature adoption rate > 60%
- Support ticket reduction > 50% 