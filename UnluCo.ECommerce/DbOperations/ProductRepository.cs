using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using UnluCo.ECommerce.DataAccess;
using UnluCo.ECommerce.Entities;
using System.Linq.Dynamic.Core;
using UnluCo.ECommerce.Extensions;


namespace UnluCo.ECommerce.DbOperations
{
    public class ProductRepository : IProductRepository
    {
        private readonly ECommerceDbContext _context;

        public ProductRepository(ECommerceDbContext context)
        {
            _context = context;
        }
        public List<Product> GetAll()
        {
            return _context.Products.Include(c=>c.Category).ToList();
        }

        public Product GetById(int id)
        {
            return _context.Products.Include(c => c.Category).SingleOrDefault(p => p.ProductId == id);
        }

        public List<Product> Get(Expression<Func<Product, bool>> filter)
        {
            return _context.Products.Where(filter).ToList();
        }

        public List<Product> GetProducts(QueryParams queryParams)
        {
            var entity = typeof(QueryParams);
            var property = entity.GetProperty(queryParams.Sort);

            if (queryParams.SortingDirection == SortingDirection.Asc)
            {
                if (string.IsNullOrWhiteSpace(queryParams.Search))
                {
                    return _context.Products.OrderBy(queryParams.Sort).ToList();
                }
                return  _context.Products.OrderBy(queryParams.Sort).SearchByName(queryParams.Search);
            }


            if (string.IsNullOrWhiteSpace(queryParams.Search))
            {
                return _context.Products.OrderByDescending(x => property.GetValue(x, null)).ToList();
            }

            return _context.Products.OrderByDescending(x => property.GetValue(x, null)).SearchByName(queryParams.Search);

        }

        public void Add(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        public void Update(Product product)
        {
            _context.Products.Update(product);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var product =  GetById(id);
            _context.Products.Add(product);
            _context.SaveChanges();
        }
    }
}
