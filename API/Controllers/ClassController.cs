﻿using Application;
using AutoMapper;
using Domain.Models;
using Infra.DTOs.Requests.Classes;
using Infra.DTOs.Responses.Classes;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassController : ControllerBase
    {
        private readonly XClassService _classService;
        private readonly IMapper _mapper;

        public ClassController(XClassService classService, IMapper mapper)
        {
            _classService = classService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult?> Get([FromRoute] int id)
        {
            var entity = await _classService.Get(id);
            var res = new ClassDetailRS();

            if (entity is not null)
            {
                res = _mapper.Map<ClassDetailRS>(entity);
            }

            return entity is null ? NotFound() : Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var classes = await _classService.Get();

            var res = classes.Select(x =>
            {
                return new ClassRS
                {
                    ID = x.Id,
                    Key = x.Key,
                    Name = x.Name,
                    IsPrimitive = x.IsPrimitive
                };
            }).ToList();

            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateClassRQ input)
        {
            var classID = await _classService.Create(new XClass
            {
                Name = input.Name,
                IsPrimitive = input.IsPrimitive,
            });

            var res = await _classService.Get(classID);

            return Ok(res);
        }

        [HttpPatch]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateClassRQ input)
        {
            var c = await _classService.Get(id);
            if (c is null)
                return NotFound();

            var updateClass = new XClass
            {
                Id = id,
                Name = input.Name,
                IsPrimitive = input.IsPrimitive,
                Key = input.Key                
            };

            var res = await _classService.Update(updateClass);

            return Ok();
        }

        [HttpPost]
        [Route("{ClassID}/property")]
        public async Task<IActionResult> AddProperty([FromRoute] int ClassID, [FromBody] AddPropertyRQ input)
        {
            var c = await _classService.Get(ClassID);

            if (c is null)
                return NotFound();

            var property = new XProperty
            {
                ClassId = c.Id,
                PropertyClassId = input.PropertyClassID,
                Name = input.Name,
                Key = input.Key
            };

            var newPropertyID = await _classService.AddProperty(c.Id, property);

            property.Id = newPropertyID;

            var res = await _classService.Get(c.Id);
            var x = _mapper.Map<ClassDetailRS>(res);
            return Ok(x);
        }

        [HttpDelete]
        [Route("{ClassID}/property/{PropertyID}")]
        public async Task<IActionResult> RemoveProperty([FromRoute] int ClassID, [FromRoute] int PropertyID)
        {
            var c = await _classService.Get(ClassID);
            if (c is null)
                return NotFound();

            var property = c.PropertyClasses.FirstOrDefault(x => x.Id == PropertyID);

            if (property is null)
                return NotFound();

            var deleted = await _classService.RemoveProperty(property.Id);

            return Ok();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await _classService.Delete(id);

            return Ok();
        }
    }
}
