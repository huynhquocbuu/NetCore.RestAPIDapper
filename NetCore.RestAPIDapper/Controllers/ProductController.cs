using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetCore.RestAPIDapper.Dtos;
using NetCore.RestAPIDapper.Filters;
using NetCore.RestAPIDapper.Models;
using System;
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
        private ILogger<ProductController> _logger;

        public ProductController(IConfiguration configuration, ILogger<ProductController> logger)
        {
            _sqlConnectionString = configuration.GetConnectionString("SqlConnectionString");
            _logger = logger;
        }

        // GET: api/Product
        [HttpGet]
        public async Task<IEnumerable<Product>> Get()
        {
            //throw new Exception("test");
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
        [HttpGet("paging",Name = "GetPaging")]
        public async Task<PagingResult<Product>> GetPaging(string keyword, int categoryId, int pageIndex, int pageSize)
        {
            using (var conn = new SqlConnection(_sqlConnectionString))
            {
                if (conn.State == System.Data.ConnectionState.Closed) { conn.Open(); }
                var parameters = new DynamicParameters();
                parameters.Add("@keyword", keyword);
                parameters.Add("@categoryId", categoryId);
                parameters.Add("@pageIndex", pageIndex);
                parameters.Add("@pageSize", pageSize);
                parameters.Add("@totalRow", dbType: DbType.Int32, direction: ParameterDirection.Output);
                var result = await conn.QueryAsync<Product>("Get_Product_AllPaging", parameters, null, null, System.Data.CommandType.StoredProcedure);
                int totalRow = parameters.Get<int>("@totalRow");
                return new PagingResult<Product> {
                    Items = result.ToList(),
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalRow = totalRow
                };
            }
        }
        // POST: api/Product
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Post([FromBody] Product product)
        {
            //Dung filter de kiem tra
            //if (!ModelState.IsValid)
            //    return BadRequest(ModelState);
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
                int newId = parameters.Get<int>("@id");
                return Ok(newId);
            }
            
        }

        // PUT: api/Product/5
        //update
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            using (var conn = new SqlConnection(_sqlConnectionString))
            {
                if (conn.State == System.Data.ConnectionState.Closed) { conn.Open(); }
                var parameters = new DynamicParameters();
                parameters.Add("@id", id);
                parameters.Add("@sku", product.Sku);
                parameters.Add("@price", product.Price);
                parameters.Add("@isActive", product.IsActive);
                parameters.Add("@imageUrl", product.ImageUrl);

                await conn.ExecuteAsync("Update_Product", parameters, null, null, CommandType.StoredProcedure);
                return Ok();
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