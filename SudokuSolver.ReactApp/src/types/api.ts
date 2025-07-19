// API request and response types
export interface SolveRequest {
  grid: number[];
}

export interface SolveResponse {
  grid: number[];
  isSolved: boolean;
  message?: string;
}

export interface ValidateRequest {
  grid: number[];
}

export interface ValidateResponse {
  isValid: boolean;
  message?: string;
}

export interface GenerateRequest {
  difficulty: string;
}

export interface GenerateResponse {
  grid: number[];
  difficulty: string;
  message?: string;
}

export interface ApiError {
  error: string;
  message: string;
  statusCode: number;
} 