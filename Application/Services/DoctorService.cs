using Application.DTOs.Doctor;
using Application.IServices.Doctor;
using AutoMapper;
using Domain.Entities;
using Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Numerics;

namespace Application.Services
{
    public class DoctorService:IDoctorService
    {
        private readonly IRepository<Doctor, int> _doctorRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DoctorService> _logger;

        public DoctorService(IRepository<Doctor, int> doctorRepository, IMapper mapper, ILogger<DoctorService> logger)
        {
            _doctorRepository = doctorRepository ?? throw new ArgumentNullException(nameof(doctorRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<GetDoctorDTO> CreateDoctorAsync(CreateDoctorDTO dto)
        {
            _logger.LogInformation("Attmpting to create doctor: {DoctorFirstName}, {DoctorLastName}", dto.FirstName, dto.LastName);
            try
            {
                if (dto is null)
                {
                    _logger.LogError("CreateDoctorAsync called with null DTO.");
                    throw new ArgumentNullException(nameof(dto), "Doctor DTO cannot be null");
                }
                var existingDoctor = await _doctorRepository.GetByPredicateAsync(d => (dto.FirstName!.Equals(d.FirstName) && dto.Speciality!.Equals(d.Speciality) && dto.Speciality!.Equals(d.Speciality)));
                if (existingDoctor is not null)
                {
                    _logger.LogWarning("Doctor alreay exists.Creation FAILED.");
                    throw new InvalidOperationException("Doctor already exists.");

                }
                Doctor doctor = _mapper.Map<Doctor>(dto);
                await _doctorRepository.AddAsync(doctor);
                await _doctorRepository.SaveAsync();

                _logger.LogInformation("Doctor '{DoctorFirstName}' '{DoctorLastName}' (ID: {DoctorId}) created successfully.", doctor.FirstName, doctor.LastName, doctor.Id);
                return _mapper.Map<GetDoctorDTO>(doctor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding doctor with Name: {DoctorFirstName} {DoctorLastName}", dto.FirstName,dto.LastName);
                throw;
            }
        }

        public async Task DeleteDoctorAsync(int id)
        {
            _logger.LogInformation("Attempting to delete doctor with ID: {DoctorId}", id);
            try
            {
                var doctor = await _doctorRepository.GetByIdAsync(id);
                if (doctor is null)
                {
                    _logger.LogWarning("Doctor with ID: {DoctorId} not found for deletion.", id);
                    throw new KeyNotFoundException($"Doctor with ID '{id}' not found.");
                }
                if (doctor.Prescriptions is not null && doctor.Prescriptions.Count != 0)
                {
                    _logger.LogWarning("Can't delete Doctor with ID: {DoctorId} This Doctor is a foriegn key in Prescriptions.", id);
                    throw new DbUpdateException($"Doctor with ID '{id}' is used by an employee or more.");
                }
                _doctorRepository.Delete(doctor);
                await _doctorRepository.SaveAsync();

                _logger.LogInformation("Doctor with ID: {DoctorId} deleted successfully.", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting doctor with ID {Id}.", id);
                throw;
            }

        }

        public async Task<IEnumerable<GetDoctorDTO>> GetAllDoctorsAsync()
        {
            _logger.LogInformation("Attempting to retreive all doctors ");
            try
            {
                var doctors = await _doctorRepository.GetAllAsync();
                _logger.LogInformation("Retreived all doctors successfully");
                return _mapper.Map<IEnumerable<GetDoctorDTO>>(doctors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while retreiving all doctors");
                throw;
            }
        }

        public async Task<GetDoctorDTO> GetDoctorByIdAsync(int id)
        {
            _logger.LogInformation("Attempting to retrieve doctor with ID: {DoctorId}", id);
            try
            {
                var doctor = await _doctorRepository.GetByIdAsync(id);

                if (doctor is null)
                {
                    _logger.LogWarning("Doctor with ID: {DoctorId} not found.", id);
                    throw new KeyNotFoundException($"Doctor with ID '{id}' not found.");
                }

                _logger.LogInformation("Doctor with ID: {DoctorId} retrieved successfully.", id);
                return _mapper.Map<GetDoctorDTO>(doctor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving doctor with id: {DoctorID}.", id);
                throw;
            }

        }

        public async Task UpdateDoctorAsync(UpdateDoctorDTO dto)
        {
            _logger.LogInformation("Attempting to update doctor with ID: {DoctorId}", dto.Id);
            if (dto is null)
            {
                _logger.LogWarning("UpdateDoctorAsync called with null DTO.");
                throw new ArgumentNullException(nameof(dto), "Doctor update DTO cannot be null.");
            }
            try
            {
                var doctor = await _doctorRepository.GetByIdAsync(dto.Id);
                if (doctor is null)
                {
                    _logger.LogWarning("Doctor with ID: {DoctorId} not found for update.", dto.Id);
                    throw new KeyNotFoundException($"Doctor with ID '{dto.Id}' not found.");
                }

                // check for uniquness
                var sameDoctor = await _doctorRepository.GetByPredicateAsync(d => (dto.FirstName!.Equals(d.FirstName) && dto.Speciality!.Equals(d.Speciality) && dto.Speciality!.Equals(d.Speciality)));
                if (sameDoctor is not null)
                {
                    _logger.LogWarning("Another doctor with same attributes already exists. Update failed for ID: {DoctorId}.", dto.Id);
                    throw new InvalidOperationException("Another doctor with same attributes already exists.");
                }
                
                _mapper.Map(dto, doctor);
                _doctorRepository.Update(doctor);
                await _doctorRepository.SaveAsync();

                _logger.LogInformation("Doctor '{FirstName}' '{LastName}' updated successfully.", doctor.FirstName, doctor.LastName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating doctor with ID {Id}.", dto.Id);
                throw;
            }
        }
    }
}
