import axios from 'axios';

const API_URL = '/account'; // The proxy in vite.config.js will handle the full URL

const login = async (username, password) => {
    const response = await axios.post(API_URL + '/login', {
        username,
        password,
    });
    if (response.data.token) {
        localStorage.setItem('user', JSON.stringify(response.data));
    }
    return response.data;
};

const logout = () => {
    // We don't need to call the backend here, just remove the token
    localStorage.removeItem('user');
};

const getCurrentUser = () => {
    return JSON.parse(localStorage.getItem('user'));
};

const authService = {
    login,
    logout,
    getCurrentUser,
};

export default authService;