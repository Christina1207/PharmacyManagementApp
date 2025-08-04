import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Button } from '../components/ui/button';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '../components/ui/card';
import { Badge } from '../components/ui/badge';
import { useToast } from '../hooks/use-toast';
import { useAuth } from '../context/AuthContext';
import inventoryCheckService from '../services/inventoryCheckService';
import { ArrowLeft, CheckCircle, AlertTriangle } from 'lucide-react';

const InventoryCheckDetailPage = () => {
    const [check, setCheck] = useState(null);
    const [loading, setLoading] = useState(true);
    const { id } = useParams();
    const { toast } = useToast();
    const navigate = useNavigate();
    const { user } = useAuth();

    useEffect(() => {
        inventoryCheckService.getInventoryCheckById(id)
            .then(setCheck)
            .catch(() => toast({ title: "Error", description: "Could not fetch check details.", variant: "destructive" }))
            .finally(() => setLoading(false));
    }, [id, toast]);

    const handleReconcile = async () => {
        try {
            await inventoryCheckService.reconcileInventoryCheck(id);
            toast({ title: "Success", description: "Inventory has been reconciled and stock levels are updated." });
            navigate('/inventory');
        } catch (error) {
            toast({ title: "Error", description: "Reconciliation failed.", variant: "destructive" });
        }
    };

    const getVarianceColor = (variance) => {
        if (variance > 0) return 'bg-yellow-500 text-yellow-900';
        if (variance < 0) return 'bg-red-500 text-white';
        return 'bg-green-500 text-white';
    };

    if (loading) return <div>Loading...</div>;
    if (!check) return <div>Inventory check not found.</div>;

    return (
        <div className="space-y-6 max-w-4xl mx-auto">
            <Button variant="outline" onClick={() => navigate('/inventory/checks')}><ArrowLeft className="w-4 h-4 mr-2" />Back to List</Button>
            
            <Card>
                <CardHeader>
                    <div className="flex justify-between items-start">
                        <div>
                            <CardTitle>Inventory Check #{check.id}</CardTitle>
                            <CardDescription>
                                Performed by {check.userName} on {new Date(check.date).toLocaleString()}
                            </CardDescription>
                        </div>
                        {user.role === 'Admin' && (
                             <Button onClick={handleReconcile}>
                                <CheckCircle className="w-4 h-4 mr-2"/>
                                Reconcile Stock
                            </Button>
                        )}
                    </div>
                </CardHeader>
                <CardContent>
                    <p className="text-sm italic text-muted-foreground mb-4">"{check.notes}"</p>
                     <table className="w-full">
                        <thead>
                            <tr className="border-b">
                                <th className="text-left p-2">Medication</th>
                                <th className="text-center p-2">Expected</th>
                                <th className="text-center p-2">Counted</th>
                                <th className="text-center p-2">Variance</th>
                            </tr>
                        </thead>
                        <tbody>
                            {check.inventoryCheckItems.map(item => (
                                <tr key={item.medicationName} className="border-b">
                                    <td className="p-2 font-medium">{item.medicationName}</td>
                                    <td className="p-2 text-center font-mono">{item.expectedQuantity}</td>
                                    <td className="p-2 text-center font-mono">{item.countedQuantity}</td>
                                    <td className="p-2 text-center">
                                        <Badge className={`${getVarianceColor(item.variance)}`}>
                                            {item.variance > 0 ? `+${item.variance}` : item.variance}
                                        </Badge>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </CardContent>
            </Card>
        </div>
    );
};

export default InventoryCheckDetailPage;