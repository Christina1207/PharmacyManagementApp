import axios from 'axios';

// This interface should match your backend's AuthResponseDTO and UserInfoDTO
interface UserInfo {
  username: string;
  firstName: string;
  lastName: string;
  role: string;
}

export interface AuthResponse {
  token: string;
  expiration: string;
  user: UserInfo;
}

const API_URL = `${import.meta.env.VITE_API_BASE_URL}/api/account`;
//const API_URL = `https://localhost:7144/api/account`;

const login = async (username: string, password: string): Promise<AuthResponse> => {
  const response = await axios.post<AuthResponse>(`${API_URL}/login`, {
    username,
    password,
  });

  if (response.data.token) {
    // Store the entire auth response in local storage
    localStorage.setItem('pharmacy_user', JSON.stringify(response.data));
  }

  return response.data;
};

const logout = (): void => {
  // No API call is strictly necessary for JWT logout, just remove the token
  localStorage.removeItem('pharmacy_user');
};

const getCurrentUser = (): AuthResponse | null => {
  const userStr = localStorage.getItem('pharmacy_user');
  if (userStr) {
    return JSON.parse(userStr) as AuthResponse;
  }
  return null;
};
const registerAdmin = async (adminData: RegisterPayload): Promise<void> => {
    await axios.post(`${URL}/register-admin`, adminData, { headers: getAuthHeaders() });
};
const registerPharmacist = async (pharmacistData: RegisterPayload): Promise<void> =>{
    await axios.post(`${URL}/register-pharmacist`, pharmacistData,{headers:getAuthHeaders()});
}


const authService = {
  login,
  logout,
  getCurrentUser,
};

export default authService;