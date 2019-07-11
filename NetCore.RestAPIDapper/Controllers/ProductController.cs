using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NetCore.RestAPIDapper.Models;

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
                string sql = "SELECT Id, Sku, Price, DiscountPrice, IsActive, ImageUrl, ViewCount, CreatedAt FROM Products";
                var result = await conn.QueryAsync<Product>(sql,null,null,null,System.Data.CommandType.Text);
                return result;
            }
            
        }

        // GET: api/Product/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Product
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Product/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
