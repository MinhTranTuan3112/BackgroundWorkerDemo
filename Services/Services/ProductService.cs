using Azure.Core;
using Newtonsoft.Json;
using Repos.Entities;
using Repos.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        private readonly IRedisCacheService<List<Product>> _redisCacheService;

        public ProductService(IProductRepository productRepository, IRedisCacheService<List<Product>> redisCacheService)
        {
            _productRepository = productRepository;
            _redisCacheService = redisCacheService;
        }

        public async Task<List<Product>> GetCachedProducts()
        {
            return await _redisCacheService.GetCacheValueAsync("[products]") ?? [];
        }

        public async Task<List<Product>> GetProducts()
        {
            return await _productRepository.GetProducts();
        }
    }
}
