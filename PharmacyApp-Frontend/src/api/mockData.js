// Mock data for the Pharmacy Management System

export const mockUsers = [
  {
    id: 1,
    username: 'admin',
    password: 'admin123',
    role: 'Admin',
    firstName: 'John',
    lastName: 'Administrator',
    email: 'admin@pharmacy.com'
  },
  {
    id: 2,
    username: 'pharmacist',
    password: 'pharm123',
    role: 'Pharmacist',
    firstName: 'Sarah',
    lastName: 'Johnson',
    email: 'sarah@pharmacy.com'
  }
];

export const mockPatients = [
  {
    id: 1,
    employeeId: 'EMP001',
    firstName: 'Michael',
    lastName: 'Smith',
    dateOfBirth: '1985-06-15',
    gender: 'Male',
    phone: '555-0101',
    email: 'michael.smith@company.com',
    address: '123 Main St, City, State 12345',
    type: 'Employee',
    department: 'Engineering',
    organizationCoverage: 80
  },
  {
    id: 2,
    employeeId: 'FAM001',
    firstName: 'Emily',
    lastName: 'Johnson',
    dateOfBirth: '1990-03-22',
    gender: 'Female',
    phone: '555-0102',
    email: 'emily.johnson@email.com',
    address: '456 Oak Ave, City, State 12345',
    type: 'FamilyMember',
    parentEmployee: 'John Johnson',
    organizationCoverage: 60
  },
  {
    id: 3,
    employeeId: 'EMP002',
    firstName: 'David',
    lastName: 'Wilson',
    dateOfBirth: '1978-11-08',
    gender: 'Male',
    phone: '555-0103',
    email: 'david.wilson@company.com',
    address: '789 Pine St, City, State 12345',
    type: 'Employee',
    department: 'HR',
    organizationCoverage: 80
  }
];

export const mockDoctors = [
  {
    id: 1,
    firstName: 'Dr. Robert',
    lastName: 'Anderson',
    specialization: 'General Medicine',
    licenseNumber: 'MD12345',
    phone: '555-0201',
    email: 'dr.anderson@hospital.com'
  },
  {
    id: 2,
    firstName: 'Dr. Lisa',
    lastName: 'Thompson',
    specialization: 'Cardiology',
    licenseNumber: 'MD12346',
    phone: '555-0202',
    email: 'dr.thompson@hospital.com'
  },
  {
    id: 3,
    firstName: 'Dr. James',
    lastName: 'Brown',
    specialization: 'Pediatrics',
    licenseNumber: 'MD12347',
    phone: '555-0203',
    email: 'dr.brown@hospital.com'
  }
];

export const mockMedications = [
  {
    id: 1,
    name: 'Amoxicillin',
    genericName: 'Amoxicillin',
    strength: '500mg',
    dosageForm: 'Capsule',
    manufacturer: 'PharmaCorp',
    price: 15.50,
    ingredients: ['Amoxicillin 500mg', 'Gelatin', 'Magnesium Stearate'],
    category: 'Antibiotic'
  },
  {
    id: 2,
    name: 'Lisinopril',
    genericName: 'Lisinopril',
    strength: '10mg',
    dosageForm: 'Tablet',
    manufacturer: 'MediPharm',
    price: 22.30,
    ingredients: ['Lisinopril 10mg', 'Lactose', 'Corn Starch'],
    category: 'ACE Inhibitor'
  },
  {
    id: 3,
    name: 'Metformin',
    genericName: 'Metformin HCl',
    strength: '850mg',
    dosageForm: 'Tablet',
    manufacturer: 'DiabetesCare',
    price: 18.75,
    ingredients: ['Metformin HCl 850mg', 'Microcrystalline Cellulose', 'Povidone'],
    category: 'Antidiabetic'
  },
  {
    id: 4,
    name: 'Ibuprofen',
    genericName: 'Ibuprofen',
    strength: '200mg',
    dosageForm: 'Tablet',
    manufacturer: 'PainRelief Inc',
    price: 8.95,
    ingredients: ['Ibuprofen 200mg', 'Lactose', 'Starch'],
    category: 'NSAID'
  }
];

export const mockInventory = [
  {
    id: 1,
    medicationId: 1,
    medicationName: 'Amoxicillin 500mg',
    batchNumber: 'AMX2024001',
    expiryDate: '2025-12-31',
    quantity: 150,
    costPrice: 12.40,
    sellingPrice: 15.50,
    supplier: 'PharmaCorp',
    receivedDate: '2024-01-15'
  },
  {
    id: 2,
    medicationId: 2,
    medicationName: 'Lisinopril 10mg',
    batchNumber: 'LIS2024001',
    expiryDate: '2025-08-15',
    quantity: 200,
    costPrice: 17.84,
    sellingPrice: 22.30,
    supplier: 'MediPharm',
    receivedDate: '2024-02-01'
  },
  {
    id: 3,
    medicationId: 3,
    medicationName: 'Metformin 850mg',
    batchNumber: 'MET2024001',
    expiryDate: '2025-06-30',
    quantity: 100,
    costPrice: 15.00,
    sellingPrice: 18.75,
    supplier: 'DiabetesCare',
    receivedDate: '2024-01-20'
  },
  {
    id: 4,
    medicationId: 4,
    medicationName: 'Ibuprofen 200mg',
    batchNumber: 'IBU2024001',
    expiryDate: '2025-10-15',
    quantity: 300,
    costPrice: 7.16,
    sellingPrice: 8.95,
    supplier: 'PainRelief Inc',
    receivedDate: '2024-02-10'
  }
];

export const mockDepartments = [
  { id: 1, name: 'Engineering', code: 'ENG' },
  { id: 2, name: 'Human Resources', code: 'HR' },
  { id: 3, name: 'Marketing', code: 'MKT' },
  { id: 4, name: 'Finance', code: 'FIN' },
  { id: 5, name: 'Operations', code: 'OPS' }
];

export const mockPrescriptions = [];

export const mockSales = [];