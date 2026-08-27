using System.Threading.Tasks;

namespace PharmacyInventory.Services
{
    public interface IProductImportService
    {
        Task<int> ImportMedicinesAsync(string filePath);
        Task<int> ImportGroceriesAsync(string filePath);
    }
}