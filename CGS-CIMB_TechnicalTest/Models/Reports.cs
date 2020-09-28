using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CGS_CIMB_TechnicalTest.Models
{
    public class Reports
    {
        [Key]
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int ReadershipStats { get; set; }
        public Guid ReportFilesId { get; set; }
        public ReportFiles ReportFiles { get; set; }
        
    }
}
