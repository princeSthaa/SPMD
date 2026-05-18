using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SPMD.Models;
using SPMD.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SPMD.Pages.Medicines
{
    public class SearchModel : PageModel
    {
        private readonly MedicineService _medicineService;

        public SearchModel(MedicineService medicineService)
        {
            _medicineService = medicineService;
        }

        public string? SearchTerm { get; set; }
        public IEnumerable<MedicineAvailabilityDto>? SearchResults { get; set; }

        public async Task OnGetAsync(string? searchTerm)
        {
            SearchTerm = searchTerm;
            SearchResults = await _medicineService.SearchMedicinesAsync(searchTerm ?? string.Empty);
        }

        public async Task<PartialViewResult> OnGetSearchPartialAsync(string? searchTerm)
        {
            var results = await _medicineService.SearchMedicinesAsync(searchTerm ?? string.Empty);
            return Partial("_MedicineSearchResults", results);
        }
    }
}
