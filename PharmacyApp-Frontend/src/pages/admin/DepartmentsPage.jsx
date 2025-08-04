import React, { useState, useEffect } from 'react';
import { Button } from '../../components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '../../components/ui/card';
import { Input } from '../../components/ui/input';
import { Label } from '../../components/ui/label';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from '../../components/ui/dialog';
import { AlertDialog, AlertDialogAction, AlertDialogCancel, AlertDialogContent, AlertDialogDescription, AlertDialogFooter, AlertDialogHeader, AlertDialogTitle, AlertDialogTrigger } from '../../components/ui/alert-dialog';
import { useToast } from '../../hooks/use-toast';
import departmentService from '../../services/departmentService'; // <-- Import the REAL service
import { Plus, Search, Edit, Trash2, Building } from 'lucide-react';

const DepartmentsPage = () => {
    const [departments, setDepartments] = useState([]);
    const [searchTerm, setSearchTerm] = useState('');
    const [loading, setLoading] = useState(true);
    const [isDialogOpen, setIsDialogOpen] = useState(false);
    const [editingDepartment, setEditingDepartment] = useState(null);
    const [formData, setFormData] = useState({ name: '' });
    const { toast } = useToast();

    const fetchDepartments = async () => {
        setLoading(true);
        try {
            const data = await departmentService.getDepartments();
            setDepartments(data);
        } catch (error) {
            toast({ title: "Error", description: "Could not fetch departments.", variant: "destructive" });
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchDepartments();
    }, []);

    const resetForm = () => {
        setEditingDepartment(null);
        setFormData({ name: '' });
    };

    const handleEdit = (department) => {
        setEditingDepartment(department);
        setFormData({ name: department.name });
        setIsDialogOpen(true);
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            if (editingDepartment) {
                await departmentService.updateDepartment(editingDepartment.id, formData);
                toast({ title: "Success", description: "Department updated successfully." });
            } else {
                await departmentService.createDepartment(formData);
                toast({ title: "Success", description: "Department created successfully." });
            }
            setIsDialogOpen(false);
            fetchDepartments(); // Refresh data from the server
        } catch (error) {
            toast({ title: "Error", description: "Operation failed.", variant: "destructive" });
        }
    };

    const handleDelete = async (id) => {
        try {
            await departmentService.deleteDepartment(id);
            toast({ title: "Success", description: "Department deleted." });
            fetchDepartments(); // Refresh data from the server
        } catch (error) {
            toast({ title: "Error", description: "Could not delete department.", variant: "destructive" });
        }
    };

    // Client-side filtering for search
    const filteredDepartments = departments.filter(dept =>
        dept.name.toLowerCase().includes(searchTerm.toLowerCase())
    );

    return (
        <div className="space-y-6">
            <div className="flex justify-between items-center">
                <div>
                    <h1 className="text-2xl font-bold">Departments</h1>
                    <p className="text-muted-foreground">Manage employee departments.</p>
                </div>
                <Dialog open={isDialogOpen} onOpenChange={setIsDialogOpen}>
                    <DialogTrigger asChild>
                        <Button onClick={resetForm}><Plus className="w-4 h-4 mr-2" />Add Department</Button>
                    </DialogTrigger>
                    <DialogContent>
                        <DialogHeader><DialogTitle>{editingDepartment ? 'Edit Department' : 'Add New Department'}</DialogTitle></DialogHeader>
                        <form onSubmit={handleSubmit} className="space-y-4">
                            <div>
                                <Label htmlFor="dept-name">Department Name</Label>
                                <Input id="dept-name" value={formData.name} onChange={e => setFormData({ name: e.target.value })} required />
                            </div>
                            <Button type="submit" className="w-full">{editingDepartment ? 'Save Changes' : 'Create Department'}</Button>
                        </form>
                    </DialogContent>
                </Dialog>
            </div>

            <div className="relative max-w-sm">
                <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                <Input placeholder="Search departments..." value={searchTerm} onChange={e => setSearchTerm(e.target.value)} className="pl-10" />
            </div>
            
            <Card>
                <CardHeader><CardTitle className="flex items-center space-x-2"><Building className="w-5 h-5"/><span>All Departments</span></CardTitle></CardHeader>
                <CardContent>
                    <table className="w-full">
                        <thead>
                            <tr className="border-b">
                                <th className="text-left p-2">Name</th>
                                <th className="text-right p-2">Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {filteredDepartments.map(department => (
                                <tr key={department.id} className="border-b hover:bg-muted/50">
                                    <td className="p-2 font-medium">{department.name}</td>
                                    <td className="p-2 flex justify-end space-x-2">
                                        <Button variant="outline" size="sm" onClick={() => handleEdit(department)}>
                                            <Edit className="w-4 h-4 mr-2" />Edit
                                        </Button>
                                        <AlertDialog>
                                            <AlertDialogTrigger asChild><Button variant="destructive" size="sm"><Trash2 className="w-4 h-4 mr-2" />Delete</Button></AlertDialogTrigger>
                                            <AlertDialogContent>
                                                <AlertDialogHeader>
                                                    <AlertDialogTitle>Are you sure?</AlertDialogTitle>
                                                    <AlertDialogDescription>This will permanently delete the department.</AlertDialogDescription>
                                                </AlertDialogHeader>
                                                <AlertDialogFooter>
                                                    <AlertDialogCancel>Cancel</AlertDialogCancel>
                                                    <AlertDialogAction onClick={() => handleDelete(department.id)}>Delete</AlertDialogAction>
                                                </AlertDialogFooter>
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

export default DepartmentsPage;