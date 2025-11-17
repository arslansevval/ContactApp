using ContactApp.Core.Entities;

namespace ContactApp.Core.Interfaces
{
    public interface IEmployeeContactInfoRepository : IRepository<Employee>
    {
        /// <summary>
        /// Tüm çalışanları ContactInfos ile birlikte getirir
        /// </summary>
        Task<List<Employee>> GetAllWithContactInfosAsync();

        /// <summary>
        /// Tek bir Employee'yi ContactInfos ile birlikte getirir
        /// </summary>
        Task<Employee?> GetByIdWithContactInfosAsync(int id);

        /// <summary>
        /// Yeni ContactInfo ekler
        /// </summary>
        Task AddContactInfoAsync(ContactInfo contactInfo);

        /// <summary>
        /// Mevcut ContactInfo'yu günceller
        /// </summary>
        void UpdateContactInfo(ContactInfo contactInfo);

        /// <summary>
        /// ContactInfo siler
        /// </summary>
        void RemoveContactInfo(ContactInfo contactInfo);
    }
}
