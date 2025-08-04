import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Button } from '../components/ui/button';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '../components/ui/card';
import { Separator } from '../components/ui/separator';
import { useToast } from '../hooks/use-toast';
import saleService from '../services/saleService';
import { Printer, Plus } from 'lucide-react';

const SaleReceiptPage = () => {
    const [saleDetails, setSaleDetails] = useState(null);
    const { id } = useParams();
    const navigate = useNavigate();
    const { toast } = useToast();

    useEffect(() => {
        saleService.getSaleDetails(id)
            .then(setSaleDetails)
            .catch(() => toast({ title: "Error", description: "Could not fetch sale details.", variant: "destructive" }));
    }, [id, toast]);

    if (!saleDetails) return <div>Loading receipt...</div>;

    return (
        <div className="max-w-2xl mx-auto space-y-6">
            <div className="flex justify-between items-center">
                <h1 className="text-3xl font-bold">Sale Receipt</h1>
                <div className="space-x-2">
                    <Button variant="outline" onClick={() => window.print()}><Printer className="w-4 h-4 mr-2"/>Print</Button>
                    <Button onClick={() => navigate('/dispense')}><Plus className="w-4 h-4 mr-2"/>New Dispense</Button>
                </div>
            </div>

            <Card className="p-6">
                <div className="text-center mb-6">
                    <h2 className="text-xl font-semibold">PharmaCare Pharmacy</h2>
                    <p className="text-sm text-muted-foreground">Sale ID: #{saleDetails.id} • Rx ID: #{saleDetails.prescription.id}</p>
                </div>
                {/* ... Render patient, doctor, and financial details from `saleDetails` object ... */}
            </Card>
        </div>
    );
};

export default SaleReceiptPage;