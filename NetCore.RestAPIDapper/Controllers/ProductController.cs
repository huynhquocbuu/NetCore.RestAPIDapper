using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NetCore.RestAPIDapper.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore.RestAPIDapper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly string _sqlConnectionString;

        public ProductController(IConfiguration configuration)
        {
            _sqlConnectionString = configuration.GetConnectionString("SqlConnectionString");
        }

        // GET: api/Product
        [HttpGet]
        public async Task<IEnumerable<Product>> Get()
        {
            using (var conn = new SqlConnection(_sqlConnectionString))
            {
                if (conn.State == System.Data.ConnectionState.Closed) { conn.Open(); }
                //string sql = "SELECT Id, Sku, Price, DiscountPrice, IsActive, ImageUrl, ViewCount, CreatedAt FROM Products";
                //var result = await conn.QueryAsync<Product>(sql,null,null,null,System.Data.CommandType.Text);
                var result = await conn.QueryAsync<Product>("Get_Product_All", null, null, null, System.Data.CommandType.StoredProcedure);
                return result;
            }
        }

        // GET: api/Product/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<Product> Get(int id)
        {
            using (var conn = new SqlConnection(_sqlConnectionString))
            {
                if (conn.State == System.Data.ConnectionState.Closed) { conn.Open(); }
                var parameters = new DynamicParameters();
                parameters.Add("@id", id);
                var result = await conn.QueryAsync<Product>("Get_Product_ById", parameters, null, null, System.Data.CommandType.StoredProcedure);
                return result.Single();
            }
        }

        // POST: api/Product
        [HttpPost]
        public async Task<int> Post([FromBody] Product product)
        {
            int newId = 0;
            using (var conn = new SqlConnection(_sqlConnectionString))
            {
                if (conn.State == System.Data.ConnectionState.Closed) { conn.Open(); }
                var parameters = new DynamicParameters();
                parameters.Add("@sku", product.Sku);
                parameters.Add("@price", product.Price);
                parameters.Add("@isActive", product.IsActive);
                parameters.Add("@imageUrl", product.ImageUrl);
                parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var result = await conn.ExecuteAsync("Create_Product", parameters, null, null, CommandType.StoredProcedure);
                newId = parameters.Get<int>("@id");
            }
            return newId;
        }

        // PUT: api/Product/5
        //update
        [HttpPut("{id}")]
        public async Task Put(int id, [FromBody] Product product)
        {
            using (var conn = new SqlConnection(_sqlConnectionString))
            {
                if (conn.State == System.Data.ConnectionState.Closed) { conn.Open(); }
                var parameters = new DynamicParameters();
                parameters.Add("@id", id);
                parameters.Add("@sku", product.Sku);
                parameters.Add("@price", product.Price);
                parameters.Add("@isActive", product.IsActive);
                parameters.Add("@imageUrl", product.ImageUrl);

                var result = await conn.ExecuteAsync("Update_Product", parameters, null, null, CommandType.StoredProcedure);
            }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            using (var conn = new SqlConnection(_sqlConnectionString))
            {
                if (conn.State == System.Data.ConnectionState.Closed) { conn.Open(); }
                var parameters = new DynamicParameters();
                parameters.Add("@id", id);
                await conn.ExecuteAsync("Delete_Product_ById", parameters, null, null, System.Data.CommandType.StoredProcedure);
            }
        }
    }
}