using Repos.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repos.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetProducts();
    }
}
