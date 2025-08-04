import apiClient from './apiClient';

// --- Interfaces matching Backend DTOs ---

export interface Patient {
    id: number;
    firstName: string;
    lastName: string;
    status: string;
    patientType: 'Employee' | 'FamilyMember';
}

export interface FamilyMember extends Patient {
    employeeId: number;
    relationship: string;
}

export interface EmployeePayload {
  firstName: string;
  lastName: string;
  email: string;
  dateOfBirth: string;
  departmentId: number;
}
export interface EmployeeDetails {
    id: number; // This is the InsuredPerson ID
    firstName: string;
    lastName: string;
    familyMembers: FamilyMemberPayload[];
}

export interface FamilyMemberPayload {
  firstName: string;
  lastName: string;
  dateOfBirth: string;
  relationship: 'Spouse' | 'Child';
}


// --- Service Methods ---

/**
 * [Management] Fetches ALL patients for the admin page.
 * Corresponds to: GET /api/admin/patients
 */
const getPatients = async (): Promise<Patient[]> => {
    const response = await apiClient.get('/api/patients');
    return response.data;
};

/**
 * [Search] Fetches a single patient by their ID.
 */
const getPatient = async (id: number): Promise<Patient> => {
    // NOTE: Assuming a standard REST endpoint for fetching a single patient.
    // This may need to be adjusted if the backend endpoint is different.
    const response = await apiClient.get(`/api/patients/${id}`);
    return response.data;
};


/**
 * [Search] Fetches only patients matching a search term for autocomplete.
 * Corresponds to: GET /api/search/patients?term=...
 */
const searchPatients = async (term: string): Promise<Patient[]> => {
    const response = await apiClient.get(`/api/search/patients?term=${term}`);
    return response.data;
};

/**
 * [Search] Fetches family members for a given employee.
 * Corresponds to: GET /api/patients/employees/{employeeId}/familymembers
 */
const getFamilyMembers = async (employeeId: number): Promise<FamilyMember[]> => {
    const response = await apiClient.get(`/api/patients/employees/${employeeId}/familymembers`);
    return response.data;
};

/**
 * [Management] Activates a patient's account.
 * Corresponds to: PUT /api/admin/patients/{id}/activate
 */
const activatePatient = async (id: number): Promise<void> => {
    await apiClient.put(`/api/patients/${id}/activate`);
};

/**
 * [Management] Deactivates a patient's account.
 * Corresponds to: PUT /api/admin/patients/${id}/deactivate
 */
const deactivatePatient = async (id: number): Promise<void> => {
    await apiClient.put(`/api/patients/${id}/deactivate`);
};

/**
 * [Management] Creates a new Employee patient.
 * Corresponds to: POST /api/admin/patients/employees
 */
const createEmployee = async (employeeData: EmployeePayload): Promise<Patient> => {
    const response = await apiClient.post('/api/patients/employees', employeeData);
    return response.data;
};

/**
 * [Management] Adds a family member to an existing employee.
 * Corresponds to: POST /api/admin/patients/employees/{employeeId}/familymembers
 */
const addFamilyMember = async (employeeId: number, memberData: FamilyMemberPayload): Promise<Patient> => {
    const response = await apiClient.post(`/api/patients/employees/${employeeId}/familymembers`, memberData);
    return response.data;
};
const getEmployeeDetailsById = async (id: number): Promise<EmployeeDetails> => {
    const response = await apiClient.get(`/api/patients/employees/${id}`);
    return response.data;
};



export default {
    getPatients,
    getPatient,
    searchPatients,
    getFamilyMembers,
    activatePatient,
    deactivatePatient,
    createEmployee,
    addFamilyMember,
    getEmployeeDetailsById
};