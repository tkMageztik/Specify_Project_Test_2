import axios from 'axios';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL ?? 'http://localhost:5000',
  withCredentials: true,
});

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  expiresIn: number;
}

export const authApi = {
  login: (data: LoginRequest) => api.post<LoginResponse>('/api/auth/login', data),
  refresh: () => api.post<LoginResponse>('/api/auth/refresh'),
  logout: () => api.post('/api/auth/logout'),
};
