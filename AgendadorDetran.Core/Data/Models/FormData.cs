using System.ComponentModel.DataAnnotations;

namespace AgendadorDetran.Core.Data.Models
{
    public class FormData
    {
        public string FullName { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        
        [RegularExpression(@"[0-3][0-9]/[0-1][0-9]/\d{4}")]
        public string BirthDay { get; set; }
        
        public bool HasPreviousDocument { get; set; }
        [RegularExpression(@"\d{9}")]
        public string Rg { get; set; }
        public bool RgHasExpiryDate { get; set; }
    }
}