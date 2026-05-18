using System.Collections.Generic;

namespace SPMD.Models
{
    public class ValidationResult
    {
        public bool IsValid { get; set; } = true;
        public List<string> Warnings { get; set; } = new();
        public List<string> Errors { get; set; } = new();
        public bool RequiresOverride { get; set; } = false;
    }
}
