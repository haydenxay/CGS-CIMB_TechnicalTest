using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CGS_CIMB_TechnicalTest.Models
{
    public class ReportFiles
    {
        [Key]
        public Guid Id { get; set; }
        public string FilePath { get; set; }
        public string FileExt { get; set; }
        public string FileName { get; set; }
    }
}
