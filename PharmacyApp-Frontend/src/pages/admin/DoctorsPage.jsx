import React, { useState, useEffect } from 'react';
import { Button } from '../../components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '../../components/ui/card';
import { Input } from '../../components/ui/input';
import { Label } from '../../components/ui/label';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from '../../components/ui/dialog';
import { AlertDialog, AlertDialogAction, AlertDialogCancel, AlertDialogContent, AlertDialogDescription, AlertDialogFooter, AlertDialogHeader, AlertDialogTitle, AlertDialogTrigger } from '../../components/ui/alert-dialog';
import { useToast } from '../../hooks/use-toast';
import doctorService from '../../services/doctorService';
import { Plus, Search, Edit, Trash2, UserMd } from 'lucide-react'; // UserMd is a placeholder icon for doctor

const DoctorsPage = () => {
    const [doctors, setDoctors] = useState([]);
    const [searchTerm, setSearchTerm] = useState('');
    const [loading, setLoading] = useState(true);
    const [isDialogOpen, setIsDialogOpen] = useState(false);
    const [editingDoctor, setEditingDoctor] = useState(null);
    const [formData, setFormData] = useState({ firstName: '', lastName: '', speciality: '' });
    const { toast } = useToast();

    const fetchDoctors = async () => {
        try {
            const data = await doctorService.getDoctors();
            setDoctors(data);
        } catch (error) {
            toast({ title: "Error", description: "Could not fetch doctors.", variant: "destructive" });
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchDoctors();
    }, []);

    const resetForm = () => {
        setEditingDoctor(null);
        setFormData({ firstName: '', lastName: '', speciality: '' });
    };

    const handleEdit = (doctor) => {
        setEditingDoctor(doctor);
        setFormData({ firstName: doctor.firstName, lastName: doctor.lastName, speciality: doctor.speciality });
        setIsDialogOpen(true);
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            if (editingDoctor) {
                await doctorService.updateDoctor(editingDoctor.id, formData);
                toast({ title: "Success", description: "Doctor updated successfully." });
            } else {
                await doctorService.createDoctor(formData);
                toast({ title: "Success", description: "Doctor created successfully." });
            }
            setIsDialogOpen(false);
            fetchDoctors();
        } catch (error) {
            toast({ title: "Error", description: "Operation failed.", variant: "destructive" });
        }
    };

    const handleDelete = async (id) => {
        try {
            await doctorService.deleteDoctor(id);
            toast({ title: "Success", description: "Doctor deleted." });
            fetchDoctors();
        } catch (error) {
            toast({ title: "Error", description: "Could not delete doctor.", variant: "destructive" });
        }
    };

    // Client-side filtering
    const filteredDoctors = doctors.filter(doc =>
        doc.firstName.toLowerCase().includes(searchTerm.toLowerCase()) ||
        doc.lastName.toLowerCase().includes(searchTerm.toLowerCase()) ||
        doc.speciality.toLowerCase().includes(searchTerm.toLowerCase())
    );

    return (
        <div className="space-y-6">
            <div className="flex justify-between items-center">
                <div>
                    <h1 className="text-2xl font-bold">Doctors</h1>
                    <p className="text-muted-foreground">Manage doctors in the system.</p>
                </div>
                <Dialog open={isDialogOpen} onOpenChange={setIsDialogOpen}>
                    <DialogTrigger asChild><Button onClick={resetForm}><Plus className="w-4 h-4 mr-2" />Add Doctor</Button></DialogTrigger>
                    <DialogContent>
                        <DialogHeader><DialogTitle>{editingDoctor ? 'Edit Doctor' : 'Add New Doctor'}</DialogTitle></DialogHeader>
                        <form onSubmit={handleSubmit} className="space-y-4">
                            <div><Label>First Name</Label><Input value={formData.firstName} onChange={e => setFormData({ ...formData, firstName: e.target.value })} required /></div>
                            <div><Label>Last Name</Label><Input value={formData.lastName} onChange={e => setFormData({ ...formData, lastName: e.target.value })} required /></div>
                            <div><Label>Speciality</Label><Input value={formData.speciality} onChange={e => setFormData({ ...formData, speciality: e.target.value })} required /></div>
                            <Button type="submit" className="w-full">{editingDoctor ? 'Save Changes' : 'Create Doctor'}</Button>
                        </form>
                    </DialogContent>
                </Dialog>
            </div>
            <div className="relative max-w-sm"><Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" /><Input placeholder="Search by name or speciality..." value={searchTerm} onChange={e => setSearchTerm(e.target.value)} className="pl-10" /></div>
            
            <Card>
                <CardHeader><CardTitle>All Doctors</CardTitle></CardHeader>
                <CardContent>
                    <table className="w-full">
                        <thead><tr className="border-b"><th className="text-left p-2">Name</th><th className="text-left p-2">Speciality</th><th className="text-right p-2">Actions</th></tr></thead>
                        <tbody>
                            {filteredDoctors.map(doctor => (
                                <tr key={doctor.id} className="border-b hover:bg-muted/50">
                                    <td className="p-2">{doctor.firstName} {doctor.lastName}</td>
                                    <td className="p-2">{doctor.speciality}</td>
                                    <td className="p-2 flex justify-end space-x-2">
                                        <Button variant="outline" size="sm" onClick={() => handleEdit(doctor)}><Edit className="w-4 h-4 mr-2" />Edit</Button>
                                        <AlertDialog>
                                            <AlertDialogTrigger asChild><Button variant="destructive" size="sm"><Trash2 className="w-4 h-4 mr-2" />Delete</Button></AlertDialogTrigger>
                                            <AlertDialogContent>
                                                <AlertDialogHeader><AlertDialogTitle>Are you sure?</AlertDialogTitle><AlertDialogDescription>This will permanently delete the doctor.</AlertDialogDescription></AlertDialogHeader>
                                                <AlertDialogFooter><AlertDialogCancel>Cancel</AlertDialogCancel><AlertDialogAction onClick={() => handleDelete(doctor.id)}>Delete</AlertDialogAction></AlertDialogFooter>
                                            </AlertDialogContent>
                                        </AlertDialog>
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

export default DoctorsPage;