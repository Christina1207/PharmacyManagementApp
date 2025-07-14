using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Department
{
    public class GetDepartmentDTO
    {
        [Display(Name = "Department Id")]
        public int Id { get; set; }

        [Display(Name = "Name")]
        public string? Name { get; set; }
    }
}
