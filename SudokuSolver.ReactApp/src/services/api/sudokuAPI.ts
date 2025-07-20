import axios, { AxiosResponse } from 'axios';
import {
  SolveRequest,
  SolveResponse,
  ValidateRequest,
  ValidateResponse,
  GenerateRequest,
  GenerateResponse,
  ApiError,
} from '@/types/api';

class SudokuAPI {
  private baseURL: string;

  constructor(baseURL: string = 'https://localhost:7001/api') {
    this.baseURL = baseURL;
  }

  // Solve a Sudoku puzzle
  async solvePuzzle(grid: number[]): Promise<SolveResponse> {
    try {
      const request: SolveRequest = { grid };
      const response: AxiosResponse<SolveResponse> = await axios.post(
        `${this.baseURL}/sudoku/solve`,
        request
      );
      return response.data;
    } catch (error) {
      throw this.handleError(error);
    }
  }

  // Validate a Sudoku puzzle
  async validatePuzzle(grid: number[]): Promise<ValidateResponse> {
    try {
      const request: ValidateRequest = { grid };
      const response: AxiosResponse<ValidateResponse> = await axios.post(
        `${this.baseURL}/sudoku/validate`,
        request
      );
      return response.data;
    } catch (error) {
      throw this.handleError(error);
    }
  }

  // Generate a new Sudoku puzzle
  async generatePuzzle(difficulty: string): Promise<GenerateResponse> {
    try {
      const request: GenerateRequest = { difficulty };
      const response: AxiosResponse<GenerateResponse> = await axios.post(
        `${this.baseURL}/sudoku/generate`,
        request
      );
      return response.data;
    } catch (error) {
      throw this.handleError(error);
    }
  }

  // Handle API errors
  private handleError(error: any): ApiError {
    if (axios.isAxiosError(error)) {
      const response = error.response;
      if (response) {
        return {
          error: 'API Error',
          message: response.data?.error || response.data?.message || 'An error occurred',
          statusCode: response.status,
        };
      } else if (error.request) {
        return {
          error: 'Network Error',
          message: 'Unable to connect to the server',
          statusCode: 0,
        };
      }
    }
    
    return {
      error: 'Unknown Error',
      message: error.message || 'An unexpected error occurred',
      statusCode: 500,
    };
  }
}

// Export singleton instance
export const sudokuAPI = new SudokuAPI();
export default SudokuAPI; 