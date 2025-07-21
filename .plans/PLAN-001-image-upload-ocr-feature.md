# PLAN-001: Image Upload and OCR Feature Implementation

## Overview
Implement image upload and OCR functionality to allow users to upload screenshots of Sudoku puzzles and automatically extract the grid data, eliminating manual input.

## Architecture Decision
- **Backend Processing**: All image processing and OCR logic will be implemented in the `SudokuSolver.Core` project
- **Frontend Responsibility**: React app will only handle image upload and display results
- **API Layer**: `SudokuSolver.Api` will coordinate between frontend and core processing

## Technical Stack
- **OCR Engine**: TesseractOCR for .NET
- **Image Processing**: SixLabors.ImageSharp
- **Backend**: .NET 9.0 with Core project handling business logic
- **Frontend**: React + TypeScript (existing)

## Implementation Plan

### Phase 1: Core Infrastructure (Week 1)
**Estimated Time**: 5-7 days

#### 1.1 Core Project Dependencies
- [ ] Add TesseractOCR NuGet package to Core project
- [ ] Add SixLabors.ImageSharp NuGet package to Core project
- [ ] Add SixLabors.ImageSharp.Drawing NuGet package to Core project
- [ ] Configure Tesseract training data for digit recognition

#### 1.2 Core Interfaces
- [ ] Create `IImageProcessor` interface in Core/Interfaces
- [ ] Create `IOCRService` interface in Core/Interfaces
- [ ] Create `IGridDetector` interface in Core/Interfaces
- [ ] Create `IImageUploadResult` interface in Core/Interfaces

#### 1.3 Core Models
- [ ] Create `ImageProcessingOptions` model in Core/Models
- [ ] Create `OCRResult` model in Core/Models
- [ ] Create `GridDetectionResult` model in Core/Models
- [ ] Create `ImageUploadResult` model in Core/Models

### Phase 2: Core Image Processing Services (Week 2)
**Estimated Time**: 7-10 days

#### 2.1 Image Processing Service
- [ ] Implement `ImageProcessingService` in Core/Services
  - Image format validation
  - Image resizing and optimization
  - Grayscale conversion
  - Contrast enhancement
  - Noise reduction
  - Grid detection preprocessing

#### 2.2 Grid Detection Service
- [ ] Implement `GridDetectionService` in Core/Services
  - Edge detection algorithms
  - Contour analysis for grid boundaries
  - Cell extraction (9x9 grid)
  - Grid validation and correction

#### 2.3 OCR Service
- [ ] Implement `OCRService` in Core/Services
  - Tesseract engine initialization
  - Individual cell processing
  - Digit recognition (1-9)
  - Confidence scoring
  - Empty cell detection

#### 2.4 Image Upload Service
- [ ] Implement `ImageUploadService` in Core/Services
  - Orchestrate the entire image processing pipeline
  - Coordinate between preprocessing, detection, and OCR
  - Error handling and validation
  - Result aggregation

### Phase 3: API Integration (Week 3)
**Estimated Time**: 3-5 days

#### 3.1 API Models
- [ ] Add `ImageUploadRequest` model to Api/Models
- [ ] Add `ImageUploadResponse` model to Api/Models
- [ ] Add `OCRConfidence` model to Api/Models

#### 3.2 API Controller Enhancement
- [ ] Add `UploadImage` endpoint to SudokuController
- [ ] Implement file upload handling
- [ ] Add image validation middleware
- [ ] Integrate with Core image processing services
- [ ] Add error handling and logging

#### 3.3 API Configuration
- [ ] Configure file upload limits
- [ ] Add CORS configuration for file uploads
- [ ] Configure temporary file storage
- [ ] Add health check for OCR services

### Phase 4: Frontend Implementation (Week 4)
**Estimated Time**: 5-7 days

#### 4.1 Frontend Types
- [ ] Add `ImageUploadRequest` type to types/api.ts
- [ ] Add `ImageUploadResponse` type to types/api.ts
- [ ] Add `OCRConfidence` type to types/api.ts
- [ ] Add `ImageUploadState` type to types/ui.ts

#### 4.2 API Service Enhancement
- [ ] Add `uploadImage` method to sudokuAPI.ts
- [ ] Implement FormData handling
- [ ] Add progress tracking
- [ ] Add error handling

#### 4.3 Store Enhancement
- [ ] Add image upload state to sudokuStore.ts
- [ ] Add `uploadImage` action to store
- [ ] Add `confirmExtractedGrid` action to store
- [ ] Add `retryExtraction` action to store
- [ ] Add `clearImageUpload` action to store

#### 4.4 UI Components
- [ ] Create `ImageUpload` component
- [ ] Create `ImagePreview` component
- [ ] Create `OCRResults` component
- [ ] Create `ImageProcessingStatus` component
- [ ] Integrate components into SudokuGame

### Phase 5: Testing and Optimization (Week 5)
**Estimated Time**: 5-7 days

#### 5.1 Unit Tests
- [ ] Test ImageProcessingService
- [ ] Test GridDetectionService
- [ ] Test OCRService
- [ ] Test ImageUploadService
- [ ] Test API endpoints

#### 5.2 Integration Tests
- [ ] Test complete image upload pipeline
- [ ] Test error scenarios
- [ ] Test performance with various image sizes
- [ ] Test OCR accuracy with sample images

#### 5.3 Performance Optimization
- [ ] Optimize image preprocessing algorithms
- [ ] Implement caching for OCR results
- [ ] Add image compression
- [ ] Optimize memory usage

#### 5.4 User Experience
- [ ] Add loading indicators
- [ ] Add error messages and retry options
- [ ] Add confidence indicators
- [ ] Add manual correction interface

## Detailed File Structure

### Core Project Additions
```
SudokuSolver.Core/
├── Interfaces/
│   ├── IImageProcessor.cs (new)
│   ├── IOCRService.cs (new)
│   ├── IGridDetector.cs (new)
│   └── IImageUploadService.cs (new)
├── Models/
│   ├── ImageProcessingOptions.cs (new)
│   ├── OCRResult.cs (new)
│   ├── GridDetectionResult.cs (new)
│   └── ImageUploadResult.cs (new)
├── Services/
│   ├── ImageProcessingService.cs (new)
│   ├── GridDetectionService.cs (new)
│   ├── OCRService.cs (new)
│   └── ImageUploadService.cs (new)
└── Helpers/
    ├── ImageUtils.cs (new)
    └── OCRUtils.cs (new)
```

### API Project Additions
```
SudokuSolver.Api/
├── Models/
│   ├── SudokuRequests.cs (enhanced)
│   └── SudokuResponses.cs (enhanced)
├── Controllers/
│   └── SudokuController.cs (enhanced)
└── Middleware/
    └── ImageValidationMiddleware.cs (new)
```

### Frontend Additions
```
SudokuSolver.ReactApp/src/
├── components/
│   ├── sudoku/
│   │   ├── ImageUpload.tsx (new)
│   │   ├── ImagePreview.tsx (new)
│   │   ├── OCRResults.tsx (new)
│   │   └── ImageProcessingStatus.tsx (new)
│   └── ui/
│       └── FileUpload.tsx (new)
├── services/
│   └── api/
│       └── sudokuAPI.ts (enhanced)
├── stores/
│   └── sudokuStore.ts (enhanced)
└── types/
    ├── api.ts (enhanced)
    └── ui.ts (enhanced)
```

## Risk Analysis

### High Risk
1. **OCR Accuracy**: Tesseract may not accurately recognize digits from all image sources
   - **Mitigation**: Implement confidence thresholds and manual correction interface
   - **Fallback**: Provide manual grid input option

2. **Grid Detection**: Automatic grid detection may fail with non-standard layouts
   - **Mitigation**: Implement multiple detection algorithms
   - **Fallback**: Allow manual grid boundary selection

### Medium Risk
1. **Performance**: Large images may cause slow processing
   - **Mitigation**: Implement image resizing and async processing
   - **Monitoring**: Add performance metrics and timeouts

2. **Memory Usage**: Image processing can consume significant memory
   - **Mitigation**: Implement proper disposal of image resources
   - **Optimization**: Use streaming for large files

### Low Risk
1. **File Format Support**: Limited to common image formats
   - **Mitigation**: Support JPEG, PNG, WebP formats
   - **Validation**: Implement format validation

## Success Criteria

### Functional Requirements
- [ ] Users can upload Sudoku puzzle images
- [ ] System automatically extracts 9x9 grid data
- [ ] OCR accuracy > 90% for clear images
- [ ] Processing time < 10 seconds for standard images
- [ ] Manual correction interface for low-confidence results

### Non-Functional Requirements
- [ ] Support for images up to 10MB
- [ ] Responsive UI with progress indicators
- [ ] Comprehensive error handling
- [ ] Detailed logging for debugging
- [ ] Unit test coverage > 80%

## Estimation Summary

| Phase | Duration | Effort | Dependencies |
|-------|----------|--------|--------------|
| Phase 1: Core Infrastructure | 5-7 days | High | None |
| Phase 2: Core Services | 7-10 days | High | Phase 1 |
| Phase 3: API Integration | 3-5 days | Medium | Phase 2 |
| Phase 4: Frontend | 5-7 days | Medium | Phase 3 |
| Phase 5: Testing & Optimization | 5-7 days | Medium | Phase 4 |

**Total Estimated Duration**: 25-36 days (5-7 weeks)
**Total Estimated Effort**: High complexity feature requiring significant development time

## Dependencies

### External Dependencies
- TesseractOCR NuGet package
- SixLabors.ImageSharp NuGet package
- Tesseract training data for English digits

### Internal Dependencies
- Existing Sudoku solver core logic
- Existing API infrastructure
- Existing React frontend framework

## Next Steps

1. **Approval**: Get stakeholder approval for the plan
2. **Setup**: Install required NuGet packages
3. **Development**: Begin Phase 1 implementation
4. **Testing**: Start unit tests early in development
5. **Review**: Regular code reviews and progress updates

## Notes

- The Core project will handle all complex image processing logic
- React app will focus on user experience and API communication
- Consider implementing a proof-of-concept for OCR accuracy before full development
- Plan for iterative improvements based on user feedback 