
namespace Application.Utilities
{
    public static class PatientShareCalculator
    {
        public static decimal Calculate(bool patientType,decimal totalValue)
        {
            const decimal familyMemberCoverageRate = 0.75m; // 50% coverage for family

            if (patientType == false)
            {
                
                return 0; // Employees pay nothing
            }
            else // Patient is a FamilyMember
            {
                var patientShare = totalValue * (1 - familyMemberCoverageRate);
                return patientShare;
            }
        }
    }
}
