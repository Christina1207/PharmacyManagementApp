import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Button } from '../components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '../components/ui/card';
import { useToast } from '../hooks/use-toast';
import inventoryCheckService from '../services/inventoryCheckService';
import { ClipboardCheck, Plus } from 'lucide-react';

const InventoryCheckListPage = () => {
    const [checks, setChecks] = useState([]);
    const [loading, setLoading] = useState(true);
    const { toast } = useToast();
    const navigate = useNavigate();

    useEffect(() => {
        inventoryCheckService.getInventoryChecks()
            .then(setChecks)
            .catch(() => toast({ title: "Error", description: "Could not fetch inventory checks.", variant: "destructive" }))
            .finally(() => setLoading(false));
    }, [toast]);

    return (
        <div className="space-y-6">
            <div className="flex justify-between items-center">
                <div>
                    <h1 className="text-2xl font-bold">Stock Counts</h1>
                    <p className="text-muted-foreground">Review past inventory checks and start new ones.</p>
                </div>
                <Button onClick={() => navigate('/inventory/new-check')}>
                    <Plus className="w-4 h-4 mr-2" />
                    Start New Stock Count
                </Button>
            </div>

            <Card>
                <CardHeader><CardTitle>Check History</CardTitle></CardHeader>
                <CardContent>
                    <div className="overflow-x-auto">
                        <table className="w-full">
                            <thead>
                                <tr className="border-b">
                                    <th className="text-left p-3">Check ID</th>
                                    <th className="text-left p-3">Date</th>
                                    <th className="text-left p-3">Performed By</th>
                                    <th className="text-left p-3">Notes</th>
                                </tr>
                            </thead>
                            <tbody>
                                {checks.map((check) => (
                                    <tr key={check.id} className="border-b hover:bg-muted/50 cursor-pointer" onClick={() => navigate(`/inventory/checks/${check.id}`)}>
                                        <td className="p-3 font-mono">#{check.id}</td>
                                        <td className="p-3">{new Date(check.date).toLocaleString()}</td>
                                        <td className="p-3">{check.userName}</td>
                                        <td className="p-3 text-sm text-muted-foreground truncate max-w-xs">{check.notes}</td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                </CardContent>
            </Card>
        </div>
    );
};

export default InventoryCheckListPage;