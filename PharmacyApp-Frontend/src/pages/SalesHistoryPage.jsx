import React, { useState, useEffect, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '../components/ui/card';
import { Input } from '../components/ui/input';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../components/ui/table';
import { useToast } from '../hooks/use-toast';
import saleService from '../services/saleService';
import { Receipt, Search, DollarSign } from 'lucide-react';

const SalesHistoryPage = () => {
    const [sales, setSales] = useState([]);
    const [loading, setLoading] = useState(true);
    const [searchTerm, setSearchTerm] = useState('');
    const { toast } = useToast();
    const navigate = useNavigate();

    const fetchSalesWithDetails = useCallback(async () => {
        try {
            // 1. Fetch the initial list of sales
            const initialSales = await saleService.getSales();

            // 2. Create an array of promises to fetch details for each sale
            const detailPromises = initialSales.map(sale =>
                saleService.getSaleDetails(sale.id).catch(err => {
                    console.error(`Failed to fetch details for sale #${sale.id}`, err);
                    // Return the original sale object on error to avoid breaking the UI
                    return sale; 
                })
            );

            // 3. Wait for all detail requests to complete
            const detailedSales = await Promise.all(detailPromises);

            setSales(detailedSales);

        } catch (error) {
            toast({ title: "Error", description: "Failed to fetch sales history.", variant: "destructive" });
        } finally {
            setLoading(false);
        }
    }, [toast]);

    useEffect(() => {
        fetchSalesWithDetails();
    }, [fetchSalesWithDetails]);

    const filteredSales = sales.filter(sale =>
        (sale.pharmacistName && sale.pharmacistName.toLowerCase().includes(searchTerm.toLowerCase())) ||
        (sale.id && sale.id.toString().includes(searchTerm)) ||
        (sale.prescriptionId && sale.prescriptionId.toString().includes(searchTerm))
    );

    const totalRevenue = sales.reduce((sum, sale) => sum + (sale.amountReceived || 0), 0);

    if (loading) {
        return (
            <div className="flex items-center justify-center h-64">
                <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary"></div>
            </div>
        );
    }
    
    return (
        <div className="space-y-6">
            <div>
                <h1 className="text-2xl font-bold">Sales History</h1>
                <p className="text-muted-foreground">Review all completed prescription sales. Click on a row to see details.</p>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                <div className="md:col-span-2">
                    <div className="relative">
                        <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                        <Input placeholder="Search by Sale ID, Rx ID, or Pharmacist..." value={searchTerm} onChange={(e) => setSearchTerm(e.target.value)} className="pl-10" />
                    </div>
                </div>
                <Card>
                    <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                        <CardTitle className="text-sm font-medium">Total Revenue</CardTitle>
                        <DollarSign className="h-4 w-4 text-muted-foreground" />
                    </CardHeader>
                    <CardContent>
                        <div className="text-2xl font-bold">${totalRevenue.toFixed(2)}</div>
                        <p className="text-xs text-muted-foreground">from {sales.length} transactions</p>
                    </CardContent>
                </Card>
            </div>

            <Card>
                <CardHeader>
                    <CardTitle className="flex items-center space-x-2"><Receipt className="w-5 h-5" /><span>Transactions</span></CardTitle>
                    <CardDescription>A list of all sales recorded in the system.</CardDescription>
                </CardHeader>
                <CardContent>
                    <Table>
                        <TableHeader>
                            <TableRow>
                                <TableHead>Sale ID</TableHead>
                                <TableHead>Date</TableHead>
                                <TableHead>Pharmacist</TableHead>
                                <TableHead className="text-right">Total Amount</TableHead>
                                <TableHead className="text-right">Discount</TableHead>
                                <TableHead className="text-right">Amount Received</TableHead>
                            </TableRow>
                        </TableHeader>
                        <TableBody>
                            {filteredSales.length > 0 ? filteredSales.map((sale) => (
                                <TableRow key={sale.id} className="cursor-pointer" onClick={() => navigate(`/sales/${sale.id}`)}>
                                    <TableCell className="font-mono">#{sale.id}</TableCell>
                                    <TableCell>
                                        {sale.dispenseDate ? new Date(sale.dispenseDate).toLocaleDateString() : 'N/A'}
                                    </TableCell>
                                    
                                    <TableCell className="font-medium">{sale.pharmacistName}</TableCell>
                                    <TableCell className="text-right font-mono">${(sale.totalAmount || 0).toFixed(2)}</TableCell>
                                    <TableCell className="text-right font-mono text-green-600">-${(sale.discount || 0).toFixed(2)}</TableCell>
                                    <TableCell className="text-right font-mono font-bold">${(sale.amountReceived || 0).toFixed(2)}</TableCell>
                                </TableRow>
                            )) : (
                                <TableRow>
                                    <TableCell colSpan="7" className="h-24 text-center text-muted-foreground">
                                        No sales found.
                                    </TableCell>
                                </TableRow>
                            )}
                        </TableBody>
                    </Table>
                </CardContent>
            </Card>
        </div>
    );
};

export default SalesHistoryPage;