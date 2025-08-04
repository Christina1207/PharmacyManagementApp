import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Button } from '../components/ui/button';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '../components/ui/card';
import { Separator } from '../components/ui/separator';
import { useToast } from '../hooks/use-toast';
import saleService from '../services/saleService';
import { Printer, Plus, Pill } from 'lucide-react';

const SaleReceiptPage = () => {
    const [saleDetails, setSaleDetails] = useState(null);
    const [loading, setLoading] = useState(true);
    const { id } = useParams();
    const navigate = useNavigate();
    const { toast } = useToast();

    useEffect(() => {
        if (id) {
            saleService.getSaleDetails(id)
                .then(setSaleDetails)
                .catch(() => toast({ title: "Error", description: "Could not fetch sale details.", variant: "destructive" }))
                .finally(() => setLoading(false));
        }
    }, [id, toast]);

    if (loading) {
        return (
            <div className="flex items-center justify-center h-screen">
                <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary"></div>
            </div>
        );
    }
    
    if (!saleDetails) {
        return <div className="text-center py-12">Receipt not found.</div>;
    }

    return (
        <div className="bg-secondary min-h-screen p-4 sm:p-8 flex flex-col items-center">
            <div className="w-full max-w-3xl space-y-4">
                <div className="flex justify-between items-center print:hidden">
                    <h1 className="text-2xl font-bold">Sale Details</h1>
                    <div className="space-x-2">
                        <Button variant="outline" onClick={() => window.print()}><Printer className="w-4 h-4 mr-2"/>Print Receipt</Button>
                        <Button onClick={() => navigate('/dispense')}><Plus className="w-4 h-4 mr-2"/>New Dispense</Button>
                    </div>
                </div>

                <Card className="p-6 sm:p-8" id="receipt">
                    <CardHeader className="text-center p-0 mb-8">
                         <div className="flex justify-center items-center gap-2 mb-2">
                            <Pill className="w-7 h-7 text-primary" />
                            <h2 className="text-2xl font-bold">PharmaCare</h2>
                        </div>
                        <p className="text-sm text-muted-foreground">Official Sale Receipt</p>
                    </CardHeader>
                    <CardContent className="p-0">
                        {/* Transaction Details */}
                        <div className="grid grid-cols-2 gap-4 text-sm mb-6">
                           <div><strong>Sale ID:</strong> <span className="font-mono">#{saleDetails.id}</span></div>
                           <div className="text-right"><strong>Date:</strong> {new Date(saleDetails.dispenseDate).toLocaleString()}</div>
                           <div><strong>Prescription ID:</strong> <span className="font-mono">#{saleDetails.prescription.id}</span></div>
                           <div className="text-right"><strong>Pharmacist:</strong> {saleDetails.pharmacistName}</div>
                        </div>

                        <Separator className="my-6" />

                        {/* Patient & Doctor Details */}
                        <div className="grid grid-cols-2 gap-4 text-sm">
                            <div>
                                <h3 className="font-semibold mb-2">Patient</h3>
                                <p>{saleDetails.prescription.patientName}</p>
                            </div>
                             <div>
                                <h3 className="font-semibold mb-2">Doctor</h3>
                                <p>Dr. {saleDetails.prescription.doctorName}</p>
                            </div>
                        </div>

                        <Separator className="my-6" />
                        
                        {/* Items Table */}
                        <div>
                            <h3 className="font-semibold mb-4">Dispensed Items</h3>
                            <div className="space-y-3">
                                <div className="flex justify-between font-medium text-muted-foreground text-sm border-b pb-2">
                                    <span>Medication</span>
                                    <span>Total</span>
                                </div>
                                {saleDetails.prescription.items.map(item => (
                                    <div key={item.medicationName} className="flex justify-between items-center text-sm">
                                        <p>{item.medicationName} <span className="text-muted-foreground">x{item.quantity}</span></p>
                                        <p className="font-mono">${(item.quantity * item.unitPrice).toFixed(2)}</p>
                                    </div>
                                ))}
                            </div>
                        </div>

                        <Separator className="my-8" />
                        
                        {/* Financials */}
                        <div className="space-y-2 max-w-sm ml-auto text-sm">
                            <div className="flex justify-between items-center">
                                <span className="text-muted-foreground">Subtotal:</span> 
                                <span className="font-mono font-medium">${saleDetails.totalAmount.toFixed(2)}</span>
                            </div>
                            <div className="flex justify-between items-center">
                                <span className="text-muted-foreground">Organization Coverage (Discount):</span> 
                                <span className="font-mono font-medium text-green-600">-${saleDetails.discount.toFixed(2)}</span>
                            </div>
                            <Separator className="my-2"/>
                            <div className="flex justify-between items-center font-bold text-lg">
                                <span className="text-foreground">Total Paid:</span> 
                                <span className="font-mono">${saleDetails.amountReceived.toFixed(2)}</span>
                            </div>
                        </div>
                    </CardContent>
                </Card>
            </div>
        </div>
    );
};

export default SaleReceiptPage;