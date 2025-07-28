import React, { useState, useEffect } from 'react';
import { Button } from '../components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '../components/ui/card';
import { Input } from '../components/ui/input';
import { Label } from '../components/ui/label';
import { Badge } from '../components/ui/badge';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../components/ui/select';
import { useToast } from '../hooks/use-toast';
import { searchPatients, searchDoctors, searchMedications, dispensePrescription } from '../api/mockApi.js';
import { FileText, User, UserCheck, Plus, Trash2, Calculator, Receipt } from 'lucide-react';

const DispensePage = () => {
  const [selectedPatient, setSelectedPatient] = useState(null);
  const [selectedDoctor, setSelectedDoctor] = useState(null);
  const [prescriptionMedications, setPrescriptionMedications] = useState([]);
  const [patientSearch, setPatientSearch] = useState('');
  const [doctorSearch, setDoctorSearch] = useState('');
  const [medicationSearch, setMedicationSearch] = useState('');
  const [patientSuggestions, setPatientSuggestions] = useState([]);
  const [doctorSuggestions, setDoctorSuggestions] = useState([]);
  const [medicationSuggestions, setMedicationSuggestions] = useState([]);
  const [loading, setLoading] = useState(false);
  const { toast } = useToast();

  // Search handlers
  useEffect(() => {
    if (patientSearch.length > 1) {
      searchPatients(patientSearch).then(setPatientSuggestions);
    } else {
      setPatientSuggestions([]);
    }
  }, [patientSearch]);

  useEffect(() => {
    if (doctorSearch.length > 1) {
      searchDoctors(doctorSearch).then(setDoctorSuggestions);
    } else {
      setDoctorSuggestions([]);
    }
  }, [doctorSearch]);

  useEffect(() => {
    if (medicationSearch.length > 1) {
      searchMedications(medicationSearch).then(setMedicationSuggestions);
    } else {
      setMedicationSuggestions([]);
    }
  }, [medicationSearch]);

  const addMedication = (medication) => {
    const existingIndex = prescriptionMedications.findIndex(med => med.id === medication.id);
    
    if (existingIndex >= 0) {
      const updated = [...prescriptionMedications];
      updated[existingIndex] = {
        ...updated[existingIndex],
        quantity: updated[existingIndex].quantity + 1
      };
      setPrescriptionMedications(updated);
    } else {
      setPrescriptionMedications([
        ...prescriptionMedications,
        {
          id: medication.id,
          medicationId: medication.id,
          medicationName: `${medication.name} ${medication.strength}`,
          price: medication.price,
          quantity: 1,
          dosage: '',
          instructions: ''
        }
      ]);
    }
    setMedicationSearch('');
    setMedicationSuggestions([]);
  };

  const updateMedication = (index, field, value) => {
    const updated = [...prescriptionMedications];
    updated[index] = { ...updated[index], [field]: value };
    setPrescriptionMedications(updated);
  };

  const removeMedication = (index) => {
    setPrescriptionMedications(prescriptionMedications.filter((_, i) => i !== index));
  };

  const calculateTotals = () => {
    const subtotal = prescriptionMedications.reduce(
      (sum, med) => sum + (med.price * med.quantity), 0
    );
    
    const coveragePercent = selectedPatient ? selectedPatient.organizationCoverage : 0;
    const organizationShare = (subtotal * coveragePercent) / 100;
    const patientShare = subtotal - organizationShare;
    
    return { subtotal, organizationShare, patientShare };
  };

  const handleDispense = async () => {
    if (!selectedPatient || !selectedDoctor || prescriptionMedications.length === 0) {
      toast({
        title: "Validation Error",
        description: "Please select patient, doctor, and add at least one medication",
        variant: "destructive"
      });
      return;
    }

    const { subtotal, organizationShare, patientShare } = calculateTotals();

    setLoading(true);
    try {
      const prescriptionData = {
        patientId: selectedPatient.id,
        doctorId: selectedDoctor.id,
        medications: prescriptionMedications,
        totalAmount: subtotal,
        organizationShare,
        patientShare
      };

      await dispensePrescription(prescriptionData);
      
      toast({
        title: "Success",
        description: "Prescription dispensed successfully"
      });

      // Reset form
      setSelectedPatient(null);
      setSelectedDoctor(null);
      setPrescriptionMedications([]);
      setPatientSearch('');
      setDoctorSearch('');
    } catch (error) {
      toast({
        title: "Error",
        description: error.message,
        variant: "destructive"
      });
    } finally {
      setLoading(false);
    }
  };

  const { subtotal, organizationShare, patientShare } = calculateTotals();

  return (
    <div className="space-y-6">
      {/* Header */}
      <div>
        <h1 className="text-2xl font-bold text-foreground">Prescription Dispensing</h1>
        <p className="text-muted-foreground">Process and dispense patient prescriptions</p>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Left Column - Patient & Doctor Selection */}
        <div className="space-y-6">
          {/* Patient Selection */}
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center space-x-2">
                <User className="w-5 h-5" />
                <span>Patient Information</span>
              </CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              <div>
                <Label htmlFor="patientSearch">Search Patient</Label>
                <Input
                  id="patientSearch"
                  value={patientSearch}
                  onChange={(e) => setPatientSearch(e.target.value)}
                  placeholder="Search by name or employee ID..."
                />
                {patientSuggestions.length > 0 && (
                  <div className="mt-2 bg-card border rounded-lg shadow-lg">
                    {patientSuggestions.map((patient) => (
                      <button
                        key={patient.id}
                        onClick={() => {
                          setSelectedPatient(patient);
                          setPatientSearch(`${patient.firstName} ${patient.lastName}`);
                          setPatientSuggestions([]);
                        }}
                        className="w-full text-left p-3 hover:bg-muted border-b last:border-b-0"
                      >
                        <div className="font-medium">{patient.firstName} {patient.lastName}</div>
                        <div className="text-sm text-muted-foreground">
                          {patient.employeeId} • {patient.type} • {patient.organizationCoverage}% coverage
                        </div>
                      </button>
                    ))}
                  </div>
                )}
              </div>
              
              {selectedPatient && (
                <div className="p-4 bg-muted rounded-lg">
                  <div className="space-y-2">
                    <div className="flex justify-between">
                      <span className="font-medium">{selectedPatient.firstName} {selectedPatient.lastName}</span>
                      <Badge variant={selectedPatient.type === 'Employee' ? 'default' : 'secondary'}>
                        {selectedPatient.type}
                      </Badge>
                    </div>
                    <div className="text-sm text-muted-foreground">
                      <div>ID: {selectedPatient.employeeId}</div>
                      <div>Coverage: {selectedPatient.organizationCoverage}%</div>
                      <div>Phone: {selectedPatient.phone}</div>
                    </div>
                  </div>
                </div>
              )}
            </CardContent>
          </Card>

          {/* Doctor Selection */}
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center space-x-2">
                <UserCheck className="w-5 h-5" />
                <span>Prescribing Doctor</span>
              </CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              <div>
                <Label htmlFor="doctorSearch">Search Doctor</Label>
                <Input
                  id="doctorSearch"
                  value={doctorSearch}
                  onChange={(e) => setDoctorSearch(e.target.value)}
                  placeholder="Search by name or license..."
                />
                {doctorSuggestions.length > 0 && (
                  <div className="mt-2 bg-card border rounded-lg shadow-lg">
                    {doctorSuggestions.map((doctor) => (
                      <button
                        key={doctor.id}
                        onClick={() => {
                          setSelectedDoctor(doctor);
                          setDoctorSearch(`${doctor.firstName} ${doctor.lastName}`);
                          setDoctorSuggestions([]);
                        }}
                        className="w-full text-left p-3 hover:bg-muted border-b last:border-b-0"
                      >
                        <div className="font-medium">{doctor.firstName} {doctor.lastName}</div>
                        <div className="text-sm text-muted-foreground">
                          {doctor.specialization} • {doctor.licenseNumber}
                        </div>
                      </button>
                    ))}
                  </div>
                )}
              </div>
              
              {selectedDoctor && (
                <div className="p-4 bg-muted rounded-lg">
                  <div className="space-y-2">
                    <div className="font-medium">{selectedDoctor.firstName} {selectedDoctor.lastName}</div>
                    <div className="text-sm text-muted-foreground">
                      <div>{selectedDoctor.specialization}</div>
                      <div>License: {selectedDoctor.licenseNumber}</div>
                      <div>Phone: {selectedDoctor.phone}</div>
                    </div>
                  </div>
                </div>
              )}
            </CardContent>
          </Card>
        </div>

        {/* Middle Column - Prescription Form */}
        <div className="space-y-6">
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center space-x-2">
                <FileText className="w-5 h-5" />
                <span>Prescription Medications</span>
              </CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              {/* Add Medication */}
              <div>
                <Label htmlFor="medicationSearch">Add Medication</Label>
                <Input
                  id="medicationSearch"
                  value={medicationSearch}
                  onChange={(e) => setMedicationSearch(e.target.value)}
                  placeholder="Search medications..."
                />
                {medicationSuggestions.length > 0 && (
                  <div className="mt-2 bg-card border rounded-lg shadow-lg">
                    {medicationSuggestions.map((medication) => (
                      <button
                        key={medication.id}
                        onClick={() => addMedication(medication)}
                        className="w-full text-left p-3 hover:bg-muted border-b last:border-b-0"
                      >
                        <div className="font-medium">{medication.name} {medication.strength}</div>
                        <div className="text-sm text-muted-foreground">
                          {medication.dosageForm} • ${medication.price.toFixed(2)}
                        </div>
                      </button>
                    ))}
                  </div>
                )}
              </div>

              {/* Medications List */}
              <div className="space-y-3">
                {prescriptionMedications.map((medication, index) => (
                  <div key={index} className="p-4 border rounded-lg">
                    <div className="space-y-3">
                      <div className="flex justify-between items-start">
                        <div className="font-medium">{medication.medicationName}</div>
                        <Button
                          size="sm"
                          variant="outline"
                          onClick={() => removeMedication(index)}
                        >
                          <Trash2 className="w-4 h-4" />
                        </Button>
                      </div>
                      
                      <div className="grid grid-cols-2 gap-2">
                        <div>
                          <Label>Quantity</Label>
                          <Input
                            type="number"
                            min="1"
                            value={medication.quantity}
                            onChange={(e) => updateMedication(index, 'quantity', parseInt(e.target.value))}
                          />
                        </div>
                        <div>
                          <Label>Price</Label>
                          <Input
                            value={`$${medication.price.toFixed(2)}`}
                            disabled
                          />
                        </div>
                      </div>
                      
                      <div>
                        <Label>Dosage Instructions</Label>
                        <Input
                          value={medication.dosage}
                          onChange={(e) => updateMedication(index, 'dosage', e.target.value)}
                          placeholder="e.g., 1 tablet twice daily"
                        />
                      </div>
                      
                      <div>
                        <Label>Additional Instructions</Label>
                        <Input
                          value={medication.instructions}
                          onChange={(e) => updateMedication(index, 'instructions', e.target.value)}
                          placeholder="e.g., Take with food"
                        />
                      </div>
                      
                      <div className="flex justify-between items-center pt-2 border-t">
                        <span className="text-sm text-muted-foreground">Subtotal:</span>
                        <span className="font-mono font-medium">${(medication.price * medication.quantity).toFixed(2)}</span>
                      </div>
                    </div>
                  </div>
                ))}
                
                {prescriptionMedications.length === 0 && (
                  <div className="text-center py-8 text-muted-foreground">
                    <Plus className="w-8 h-8 mx-auto mb-2 opacity-50" />
                    <p>No medications added yet</p>
                    <p className="text-sm">Search and add medications above</p>
                  </div>
                )}
              </div>
            </CardContent>
          </Card>
        </div>

        {/* Right Column - Prescription Paper Style */}
        <div>
          <Card className="prescription-paper max-w-2xl mx-auto">
            <CardHeader className="border-b-2 border-muted-foreground/20 pb-4">
              <div className="flex justify-between items-start">
                <div>
                  <h2 className="text-2xl font-serif font-bold text-primary">PharmaCare Pharmacy</h2>
                  <p className="text-sm text-muted-foreground">Licensed Pharmacy Services</p>
                  <p className="text-xs text-muted-foreground">123 Healthcare Ave, Medical District</p>
                </div>
                <div className="opacity-20">
                  <span className="text-6xl font-serif font-bold text-primary">Rx</span>
                </div>
              </div>
            </CardHeader>
            
            <CardContent className="p-6 space-y-6 font-serif">
              {/* Patient Info */}
              <div className="space-y-2">
                <div className="text-xs uppercase tracking-wider text-muted-foreground font-sans">PATIENT:</div>
                <hr className="border-muted-foreground/30" />
                {selectedPatient ? (
                  <div className="space-y-1">
                    <div className="font-mono text-lg">{selectedPatient.firstName} {selectedPatient.lastName}</div>
                    <div className="text-sm text-muted-foreground font-sans">
                      ID: {selectedPatient.employeeId} • {selectedPatient.type} • {selectedPatient.organizationCoverage}% Coverage
                    </div>
                  </div>
                ) : (
                  <div className="text-muted-foreground text-sm font-sans">___________________________</div>
                )}
              </div>

              {/* Doctor Info */}
              <div className="space-y-2">
                <div className="text-xs uppercase tracking-wider text-muted-foreground font-sans">PRESCRIBER:</div>
                <hr className="border-muted-foreground/30" />
                {selectedDoctor ? (
                  <div className="space-y-1">
                    <div className="font-mono text-lg">{selectedDoctor.firstName} {selectedDoctor.lastName}</div>
                    <div className="text-sm text-muted-foreground font-sans">
                      {selectedDoctor.specialization} • License: {selectedDoctor.licenseNumber}
                    </div>
                  </div>
                ) : (
                  <div className="text-muted-foreground text-sm font-sans">___________________________</div>
                )}
              </div>

              {/* Medications */}
              <div className="space-y-3">
                <div className="flex items-center space-x-3">
                  <span className="text-3xl font-bold text-primary">Rx</span>
                  <div className="text-xs uppercase tracking-wider text-muted-foreground font-sans">MEDICATIONS:</div>
                </div>
                <hr className="border-muted-foreground/30" />
                
                {prescriptionMedications.length > 0 ? (
                  <div className="space-y-3 min-h-[120px]">
                    {prescriptionMedications.map((medication, index) => (
                      <div key={index} className="flex justify-between items-start border-b border-dotted border-muted-foreground/30 pb-2">
                        <div className="flex-1">
                          <div className="font-mono text-base">{medication.medicationName}</div>
                          {medication.dosage && (
                            <div className="text-sm text-muted-foreground font-sans italic">Sig: {medication.dosage}</div>
                          )}
                          {medication.instructions && (
                            <div className="text-xs text-muted-foreground font-sans">{medication.instructions}</div>
                          )}
                        </div>
                        <div className="text-right font-mono">
                          <div>Qty: {medication.quantity}</div>
                          <div className="text-sm text-muted-foreground">${(medication.price * medication.quantity).toFixed(2)}</div>
                        </div>
                      </div>
                    ))}
                  </div>
                ) : (
                  <div className="min-h-[120px] flex items-center justify-center">
                    <div className="text-muted-foreground text-sm font-sans">___ No medications prescribed ___</div>
                  </div>
                )}
              </div>

              {/* Cost Calculation */}
              {prescriptionMedications.length > 0 && selectedPatient && (
                <div className="space-y-3 border-t-2 border-muted-foreground/30 pt-4">
                  <div className="grid grid-cols-3 gap-4 text-center font-sans">
                    <div className="space-y-1">
                      <div className="text-xs uppercase tracking-wider text-muted-foreground">SUBTOTAL</div>
                      <div className="text-lg font-bold">${subtotal.toFixed(2)}</div>
                    </div>
                    <div className="space-y-1">
                      <div className="text-xs uppercase tracking-wider text-success">COVERAGE</div>
                      <div className="text-lg font-bold text-success">${organizationShare.toFixed(2)}</div>
                    </div>
                    <div className="space-y-1">
                      <div className="text-xs uppercase tracking-wider text-primary">PATIENT PAYS</div>
                      <div className="text-xl font-bold text-primary">${patientShare.toFixed(2)}</div>
                    </div>
                  </div>
                </div>
              )}

              {/* Dispense Button - Outside the prescription paper aesthetic */}
              <div className="mt-6 pt-4 border-t-4 border-primary/20">
                <Button
                  onClick={handleDispense}
                  disabled={loading || !selectedPatient || !selectedDoctor || prescriptionMedications.length === 0}
                  className="w-full font-sans"
                  size="lg"
                >
                {loading ? (
                  <div className="flex items-center space-x-2">
                    <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-primary-foreground"></div>
                    <span>Processing...</span>
                  </div>
                ) : (
                  <div className="flex items-center space-x-2">
                    <Receipt className="w-5 h-5" />
                    <span>Dispense Prescription</span>
                  </div>
                )}
                </Button>
              </div>

              {/* Date and Signature Lines */}
              <div className="space-y-4 border-t border-muted-foreground/30 pt-4 font-sans">
                <div className="flex justify-between text-xs">
                  <div>
                    <div className="text-muted-foreground">DATE:</div>
                    <div className="font-mono">{new Date().toLocaleDateString()}</div>
                  </div>
                  <div>
                    <div className="text-muted-foreground">PHARMACIST:</div>
                    <div className="border-b border-muted-foreground/30 w-32 h-6"></div>
                  </div>
                </div>
              </div>
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  );
};

export default DispensePage;