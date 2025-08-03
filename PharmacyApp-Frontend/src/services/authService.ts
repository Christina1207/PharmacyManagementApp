import apiClient from './apiClient'; 

// --- Interfaces matching Backend DTOs ---

// Matches UserInfoDTO
interface UserInfo {
  username: string;
  firstName: string;
  lastName: string;
  role: string;
}

// Matches AuthResponseDTO
export interface AuthResponse {
  token: string;
  expiration: string;
  user: UserInfo;
}

const API_URL = '/api/auth'; // Common prefix

const login = async (username: string, password: string): Promise<AuthResponse> => {
    const response = await apiClient.post<AuthResponse>(`${API_URL}/login`, {
        username,
        password,
    });

    if (response.data.token) {
        localStorage.setItem('pharmacy_user', JSON.stringify(response.data));
    }
    return response.data;
};

const logout = (): void => {
    localStorage.removeItem('pharmacy_user');
};


const getCurrentUser = (): AuthResponse | null => {
    const userStr = localStorage.getItem('pharmacy_user');
    if (userStr) {
        return JSON.parse(userStr) as AuthResponse;
    }
    return null;
};



const authService = {
  login,
  logout,
  getCurrentUser,
};

export default authService;