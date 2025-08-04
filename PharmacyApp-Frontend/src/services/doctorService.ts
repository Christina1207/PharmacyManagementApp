import apiClient from './apiClient';


export interface Doctor {
    id: number;
    firstName: string;
    lastName: string;
    speciality: string;
}
export interface DoctorPayload {
    firstName: string;
    lastName: string;
    speciality: string;
}

const API_URL = '/api/admin/doctors';
// --- Service Methods ---

const getDoctors = async (): Promise<Doctor[]> => {
    const response = await apiClient.get(API_URL);
    return response.data;
};

const createDoctor = async (doctorData: DoctorPayload): Promise<Doctor> => {
    const response = await apiClient.post(API_URL, doctorData);
    return response.data;
};

const updateDoctor = async (id: number, doctorData: DoctorPayload): Promise<void> => {
    await apiClient.put(`${API_URL}/${id}`, doctorData);
};

const deleteDoctor = async (id: number): Promise<void> => {
    await apiClient.delete(`${API_URL}/${id}`);
};
const searchDoctors = async (term: string): Promise<Doctor[]> => {
    const response = await apiClient.get(`/api/search/doctors?term=${term}`);
    return response.data;
};

export default {
    getDoctors,
    createDoctor,
    updateDoctor,
    deleteDoctor,
    searchDoctors
};