import { BaseDto } from '../models/api.models';

export interface AppUser extends BaseDto {
  fullName: string;
  email: string;
  phoneNumber: string | null;
  lastLoginDate: string | null;
  lastLoginIp: string | null;
  lastLoginDevice: string | null;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  user: AppUser;
  token: string;
  expiration: string;
}

export interface AuthSession extends LoginResponse {}
