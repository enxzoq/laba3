using Microsoft.Extensions.Caching.Memory;
using Lab3.Models;
using Lab3.Data;

namespace Lab3.Service
{
    public class CachedDataService
    {
        private readonly TelecomContext _context;
        private readonly IMemoryCache _cache;
        private const int CacheDurationSeconds = 2 * 23 + 240;
        private const int RowCount = 20; // Ограничение на количество записей

        public CachedDataService(TelecomContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _cache = memoryCache;
        }

        // Универсальный метод для получения кэшированных данных
        private IEnumerable<T> GetOrSetCache<T>(string cacheKey, Func<IEnumerable<T>> dataRetrievalFunc)
        {
            if (!_cache.TryGetValue(cacheKey, out IEnumerable<T> cachedData))
            {
                cachedData = dataRetrievalFunc();
                _cache.Set(cacheKey, cachedData, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(CacheDurationSeconds)
                });
            }
            return cachedData;
        }

        // Метод для получения 20 записей из таблицы Subscribers
        public IEnumerable<Subscriber> GetSubscribers()
        {
            return GetOrSetCache("Subscribers", () => _context.Subscribers.Take(RowCount).ToList());
        }

        // Метод для получения 20 записей из таблицы TariffPlans
        public IEnumerable<TariffPlan> GetTariffPlans()
        {
            return GetOrSetCache("TariffPlans", () => _context.TariffPlans.Take(RowCount).ToList());
        }

        // Метод для получения 20 записей из таблицы ServiceContracts
        public IEnumerable<ServiceContract> GetServiceContracts()
        {
            return GetOrSetCache("ServiceContracts", () => _context.ServiceContracts.Take(RowCount).ToList());
        }

        // Метод для получения 20 записей из таблицы Employees
        public IEnumerable<Employee> GetEmployees()
        {
            return GetOrSetCache("Employees", () => _context.Employees.Take(RowCount).ToList());
        }

        // Метод для получения 20 записей из таблицы ServiceStatistics
        public IEnumerable<ServiceStatistic> GetServiceStatistics()
        {
            return GetOrSetCache("ServiceStatistics", () => _context.ServiceStatistics.Take(RowCount).ToList());
        }
    }
}
