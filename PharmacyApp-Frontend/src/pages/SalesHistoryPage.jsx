import React, { useState, useEffect } from 'react';
import { Card, CardContent, CardHeader, CardTitle , CardDescription } from '../components/ui/card';
import { Input } from '../components/ui/input';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '../components/ui/dialog';
import { Separator } from '../components/ui/separator';
import { useToast } from '../hooks/use-toast';
import saleService from '../services/saleService';
import { Receipt, Search } from 'lucide-react';

const SalesHistoryPage = () => {
    const [sales, setSales] = useState([]);
    const [loading, setLoading] = useState(true);
    const [searchTerm, setSearchTerm] = useState('');
     const [selectedSale, setSelectedSale] = useState(null); 
    const { toast } = useToast();

    useEffect(() => {
        const fetchSales = async () => {
            try {
                const data = await saleService.getSales();
                setSales(data);
            } catch (error) {
                toast({ title: "Error", description: "Failed to fetch sales history.", variant: "destructive" });
            } finally {
                setLoading(false);
            }
        };
        fetchSales();
    }, [toast]);


        useEffect(() => {
        saleService.getSales()
            .then(setSales)
            .catch(() => toast({ title: "Error", description: "Failed to fetch sales history.", variant: "destructive" }))
            .finally(() => setLoading(false));
    }, [toast]);
    
    const handleRowClick = async (saleId) => {
        try {
            const saleDetails = await saleService.getSaleDetails(saleId);
            setSelectedSale(saleDetails);
        } catch (error) {
            toast({ title: "Error", description: "Could not fetch sale details.", variant: "destructive" });
        }
    };

    const filteredSales = sales.filter(sale =>
        sale.pharmacistName.toLowerCase().includes(searchTerm.toLowerCase()) ||
        sale.id.toString().includes(searchTerm) ||
        sale.prescriptionId.toString().includes(searchTerm)
    );

    const totalRevenue = filteredSales.reduce((sum, sale) => sum + sale.amountReceived, 0);

    if (loading) {
        return <div className="flex items-center justify-center h-64"><div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary"></div></div>;
    }
    
 /*TODO*/
    return (
        <div className="space-y-6">
            <div>
                <h1 className="text-2xl font-bold">Sales History</h1>
                <p className="text-muted-foreground">Review all completed prescription sales.</p>
            </div>

            <div className="flex justify-between items-center">
                <div className="relative max-w-sm">
                    <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                    <Input placeholder="Search by Sale ID, Rx ID, or Pharmacist..." value={searchTerm} onChange={(e) => setSearchTerm(e.target.value)} className="pl-10" />
                </div>
                <Card className="p-3">
                    <div className="text-sm font-medium text-muted-foreground">Total Revenue</div>
                    <div className="text-2xl font-bold">${totalRevenue.toFixed(2)}</div>
                </Card>
            </div>

            <Card>
                <CardHeader>
                    <CardTitle className="flex items-center space-x-2"><Receipt className="w-5 h-5" /><span>Transactions</span></CardTitle>
                </CardHeader>
                <CardContent>
                    <div className="overflow-x-auto">
                        <table className="w-full">
                            <thead>
                                <tr className="border-b">
                                    <th className="text-left p-3">Sale ID</th>
                                    <th className="text-left p-3">Prescription ID</th>
                                    <th className="text-left p-3">Pharmacist</th>
                                    <th className="text-right p-3">Total Amount</th>
                                    <th className="text-right p-3">Discount</th>
                                    <th className="text-right p-3">Amount Received</th>
                                </tr>
                            </thead>
                           <tbody>
                                {filteredSales.map((sale) => (
                                    <tr key={sale.id} className="border-b hover:bg-muted/50 cursor-pointer" onClick={() => handleRowClick(sale.id)}>
                                        <td className="p-3 font-mono">#{sale.id}</td>
                                        <td className="p-3 font-mono">Rx-{sale.prescriptionId}</td>
                                        <td className="p-3">{sale.pharmacistName}</td>
                                        <td className="p-3 text-right font-mono">${sale.totalAmount.toFixed(2)}</td>
                                        <td className="p-3 text-right font-mono text-green-600">-${sale.discount.toFixed(2)}</td>
                                        <td className="p-3 text-right font-mono font-bold">${sale.amountReceived.toFixed(2)}</td>
                                    </tr>
                                ))}
                                {filteredSales.length === 0 && (
                                    <tr><td colSpan="6" className="text-center p-8 text-muted-foreground">No sales found.</td></tr>
                                )}
                            </tbody>
                        </table>
                    </div>
                </CardContent>
            </Card>
 <Dialog open={!!selectedSale} onOpenChange={() => setSelectedSale(null)}>
                <DialogContent className="max-w-2xl">
                    <DialogHeader>
                        <DialogTitle>Sale Receipt #{selectedSale?.id}</DialogTitle>
                    </DialogHeader>
                    {selectedSale && (
                        <div className="space-y-4">
                             <div className="grid grid-cols-2 gap-4 text-sm">
                                <div><strong>Patient:</strong> {selectedSale.prescription.patientName}</div>
                                <div><strong>Doctor:</strong> {selectedSale.prescription.doctorName}</div>
                                <div><strong>Pharmacist:</strong> {selectedSale.pharmacistName}</div>
                                <div><strong>Date:</strong> {new Date(selectedSale.dispenseDate).toLocaleString()}</div>
                            </div>
                            <Separator />
                            <div>
                                <h4 className="font-semibold mb-2">Dispensed Medications</h4>
                                <div className="space-y-2">
                                    {selectedSale.prescription.items.map(item => (
                                        <div key={item.medicationName} className="flex justify-between items-center text-sm">
                                            <p>{item.medicationName} (x{item.quantity})</p>
                                            <p className="font-mono">${(item.quantity * item.unitPrice).toFixed(2)}</p>
                                        </div>
                                    ))}
                                </div>
                            </div>
                            <Separator />
                             <div className="space-y-2 text-right">
                                <div className="flex justify-end items-center"><span className="text-muted-foreground w-32 text-left">Subtotal:</span> <span className="font-mono">${selectedSale.totalAmount.toFixed(2)}</span></div>
                                <div className="flex justify-end items-center"><span className="text-muted-foreground w-32 text-left">Discount:</span> <span className="font-mono text-green-600">-${selectedSale.discount.toFixed(2)}</span></div>
                                <div className="flex justify-end items-center font-bold text-lg"><span className="text-foreground w-32 text-left">Total Paid:</span> <span className="font-mono">${selectedSale.amountReceived.toFixed(2)}</span></div>
                            </div>
                        </div>
                    )}
                </DialogContent>
            </Dialog>

        </div>
    );
};

export default SalesHistoryPage;