using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Dtos;
using WebApi.Errors;

namespace WebApi.Controllers
{
    
    public class ProductoController : BaseApiController
    {
        private readonly IGenericRepository<Producto> _productoRePository;
        private readonly IMapper _mapper;
        

        public ProductoController(IGenericRepository<Producto> productoRePository, IMapper mapper)
        {
            _productoRePository = productoRePository;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<Pagination<ProductoDto>>> getProductos([FromQuery] ProductspecificationParams productParams)
        {

            var spec = new ProductoWithCategoriaAndMarcaSpecification(productParams);
            var productos = await _productoRePository.GetAllWithSpec(spec);
            var specCount = new ProductoForCountingSpecification(productParams);
            var totalProductos = await _productoRePository.CountAsync(specCount);
            var rounded = Math.Ceiling(Convert.ToDecimal( totalProductos / productParams.PageSize));
            var totalPage = Convert.ToInt32(rounded);
            var data = _mapper.Map<IReadOnlyList<Producto>,IReadOnlyList<ProductoDto>>(productos);

            return Ok(
                new Pagination<ProductoDto>
                {
                    Count = totalProductos,
                    Data = data,
                    PageCount = totalPage,
                    PageIndex = (int)productParams.PageIndex,
                    PageSize = productParams.PageSize
                }

                );

            //return Ok(_mapper.Map<IReadOnlyList<Producto>, IReadOnlyList<ProductoDto>>(productos));
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDto>> getProducto(int id)
        {
            var spec = new ProductoWithCategoriaAndMarcaSpecification(id);
            var producto = await _productoRePository.GetByIdWithSpec(spec);
            if(producto == null)
            {
                return NotFound(new CodeErrorResponse(404,"El producto no exise"));
            }

            return _mapper.Map<Producto, ProductoDto>(producto);
            
        }

    }
}
