import axios from 'axios';

// Define the structure of the user object we get from the API
interface UserAuthData {
    token: string;
    expiration: string;
    username: string;
}

const API_URL = `${import.meta.env.VITE_API_BASE_URL}/account`;

const login = async (username: string, password: string): Promise<UserAuthData> => {
    const response = await axios.post<UserAuthData>(API_URL + '/login', {
        username,
        password,
    });
    if (response.data.token) {
        localStorage.setItem('user', JSON.stringify(response.data));
    }
    return response.data;
};

const logout = (): void => {
    localStorage.removeItem('user');
};

const getCurrentUser = (): UserAuthData | null => {
    const userStr = localStorage.getItem('user');
    if (userStr) {
        return JSON.parse(userStr) as UserAuthData;
    }
    return null;
};

const authService = {
    login,
    logout,
    getCurrentUser,
};

export default authService;