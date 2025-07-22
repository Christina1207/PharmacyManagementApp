using Application.DTOs.MedicationClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IServices.MedicationClass
{
    public interface IMedicationClassService
    {
        public Task<IEnumerable<GetMedicationClassDTO>> GetAllMedicationClassesAsync();
        public Task<GetMedicationClassDTO> GetMedicationClassByIdAsync(int id);
        public Task<GetMedicationClassDTO> CreateMedicationClassAsync(CreateMedicationClassDTO dto);
        public Task UpdateMedicationClassAsync(UpdateMedicationClassDTO dto);
        public Task DeleteMedicationClassAsync(int id);
    }
}
