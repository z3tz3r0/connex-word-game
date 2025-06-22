export interface LoginDto {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
}

export interface RegisterDto {
  username: string;
  password: string;
  isVIP: boolean;
}

// Assuming registration returns a simple message or user object
export interface RegisterResponse {
  message: string;
}
