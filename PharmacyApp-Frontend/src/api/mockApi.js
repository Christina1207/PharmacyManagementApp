import {
  mockUsers,
  mockPatients,
  mockDoctors,
  mockMedications,
  mockInventory,
  mockDepartments,
  mockPrescriptions,
  mockSales
} from './mockData.js';

// Helper function to simulate network delay
const delay = (ms = 500) => new Promise(resolve => setTimeout(resolve, ms));

// Authentication API
export const login = async (username, password) => {
  await delay(800);
  
  const user = mockUsers.find(u => u.username === username && u.password === password);
  
  if (user) {
    const { password: _, ...userWithoutPassword } = user;
    return {
      success: true,
      user: {
        ...userWithoutPassword,
        token: `fake-jwt-token-${Date.now()}`
      }
    };
  }
  
  throw new Error('Invalid username or password');
};

export const logout = async () => {
  await delay(200);
  return { success: true };
};

// Patients API
export const getPatients = async () => {
  await delay();
  return [...mockPatients];
};

export const getPatient = async (id) => {
  await delay();
  const patient = mockPatients.find(p => p.id === parseInt(id));
  if (!patient) throw new Error('Patient not found');
  return patient;
};

export const createPatient = async (patientData) => {
  await delay();
  const newPatient = {
    ...patientData,
    id: Math.max(...mockPatients.map(p => p.id)) + 1
  };
  mockPatients.push(newPatient);
  return newPatient;
};

export const updatePatient = async (id, patientData) => {
  await delay();
  const index = mockPatients.findIndex(p => p.id === parseInt(id));
  if (index === -1) throw new Error('Patient not found');
  
  mockPatients[index] = { ...mockPatients[index], ...patientData };
  return mockPatients[index];
};

export const deletePatient = async (id) => {
  await delay();
  const index = mockPatients.findIndex(p => p.id === parseInt(id));
  if (index === -1) throw new Error('Patient not found');
  
  mockPatients.splice(index, 1);
  return { success: true };
};

// Doctors API
export const getDoctors = async () => {
  await delay();
  return [...mockDoctors];
};

export const createDoctor = async (doctorData) => {
  await delay();
  const newDoctor = {
    ...doctorData,
    id: Math.max(...mockDoctors.map(d => d.id)) + 1
  };
  mockDoctors.push(newDoctor);
  return newDoctor;
};

// Medications API
export const getMedications = async () => {
  await delay();
  return [...mockMedications];
};

export const getMedication = async (id) => {
  await delay();
  const medication = mockMedications.find(m => m.id === parseInt(id));
  if (!medication) throw new Error('Medication not found');
  return medication;
};

export const createMedication = async (medicationData) => {
  await delay();
  const newMedication = {
    ...medicationData,
    id: Math.max(...mockMedications.map(m => m.id)) + 1
  };
  mockMedications.push(newMedication);
  return newMedication;
};

export const updateMedication = async (id, medicationData) => {
  await delay();
  const index = mockMedications.findIndex(m => m.id === parseInt(id));
  if (index === -1) throw new Error('Medication not found');
  
  mockMedications[index] = { ...mockMedications[index], ...medicationData };
  return mockMedications[index];
};

export const deleteMedication = async (id) => {
  await delay();
  const index = mockMedications.findIndex(m => m.id === parseInt(id));
  if (index === -1) throw new Error('Medication not found');
  
  mockMedications.splice(index, 1);
  return { success: true };
};

// Inventory API
export const getInventory = async () => {
  await delay();
  return [...mockInventory];
};

export const addStock = async (stockData) => {
  await delay();
  const newStock = {
    ...stockData,
    id: Math.max(...mockInventory.map(i => i.id)) + 1,
    receivedDate: new Date().toISOString().split('T')[0]
  };
  mockInventory.push(newStock);
  return newStock;
};

export const updateStock = async (id, stockData) => {
  await delay();
  const index = mockInventory.findIndex(i => i.id === parseInt(id));
  if (index === -1) throw new Error('Inventory item not found');
  
  mockInventory[index] = { ...mockInventory[index], ...stockData };
  return mockInventory[index];
};

// Departments API
export const getDepartments = async () => {
  await delay();
  return [...mockDepartments];
};

export const createDepartment = async (departmentData) => {
  await delay();
  const newDepartment = {
    ...departmentData,
    id: Math.max(...mockDepartments.map(d => d.id)) + 1
  };
  mockDepartments.push(newDepartment);
  return newDepartment;
};

// Prescription Dispensing API
export const dispensePrescription = async (prescriptionData) => {
  await delay(1000);
  
  const { medications, patientId, doctorId, totalAmount, organizationShare, patientShare } = prescriptionData;
  
  // Check inventory and update stock
  for (const medication of medications) {
    const inventoryItem = mockInventory.find(item => 
      item.medicationId === medication.medicationId
    );
    
    if (!inventoryItem || inventoryItem.quantity < medication.quantity) {
      throw new Error(`Insufficient stock for ${medication.medicationName}`);
    }
    
    // Reduce inventory
    inventoryItem.quantity -= medication.quantity;
  }
  
  // Create prescription record
  const newPrescription = {
    id: mockPrescriptions.length + 1,
    ...prescriptionData,
    date: new Date().toISOString(),
    status: 'Dispensed'
  };
  
  mockPrescriptions.push(newPrescription);
  
  // Create sale record
  const newSale = {
    id: mockSales.length + 1,
    prescriptionId: newPrescription.id,
    patientId,
    doctorId,
    totalAmount,
    organizationShare,
    patientShare,
    date: new Date().toISOString(),
    medications: medications.map(med => ({
      ...med,
      total: med.quantity * med.price
    }))
  };
  
  mockSales.push(newSale);
  
  return {
    prescription: newPrescription,
    sale: newSale,
    success: true
  };
};

// Search APIs
export const searchPatients = async (query) => {
  await delay(300);
  const searchTerm = query.toLowerCase();
  return mockPatients.filter(patient => 
    patient.firstName.toLowerCase().includes(searchTerm) ||
    patient.lastName.toLowerCase().includes(searchTerm) ||
    patient.employeeId.toLowerCase().includes(searchTerm)
  );
};

export const searchDoctors = async (query) => {
  await delay(300);
  const searchTerm = query.toLowerCase();
  return mockDoctors.filter(doctor => 
    doctor.firstName.toLowerCase().includes(searchTerm) ||
    doctor.lastName.toLowerCase().includes(searchTerm) ||
    doctor.licenseNumber.toLowerCase().includes(searchTerm)
  );
};

export const searchMedications = async (query) => {
  await delay(300);
  const searchTerm = query.toLowerCase();
  return mockMedications.filter(medication => 
    medication.name.toLowerCase().includes(searchTerm) ||
    medication.genericName.toLowerCase().includes(searchTerm)
  );
};