﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }


        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
        public IActionResult GetCategories()
        {
            var categories = _mapper.Map<List<CategoryDto>>(_categoryRepository.GetCategories());

            if (categories.Count() < 1)
            {
                return NotFound("No Category Available!");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(categories);
        }

        [HttpGet("{categoryId}")]
        [ProducesResponseType(200, Type = typeof(Category))]
        [ProducesResponseType(400)]
        public IActionResult GetCategory(int categoryId)
        {
            if (!_categoryRepository.CategoryExist(categoryId))
            {
                return NotFound();
            }

            var category = _mapper.Map<CategoryDto>(_categoryRepository.GetCategory(categoryId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(category);
        }

        [HttpGet("pokemon/{categoryId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonByCategoryId(int categoryId)
        {
            var pokemons = _mapper.Map<List<PokemonDto>>(_categoryRepository.GetPokemonByCategory(categoryId));

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            return Ok(pokemons);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateCategory([FromBody] CategoryDto categoryCreate)
        {
            if (categoryCreate == null) return BadRequest(ModelState);

            var category = _categoryRepository.GetCategories().Where(c => c.Name.Trim().ToUpper() == categoryCreate.Name.Trim().ToUpper()).FirstOrDefault();

            if (category != null)
            {
                ModelState.AddModelError("", "Category already exists.");
                return StatusCode(409, ModelState);
            }

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var categoryMap = _mapper.Map<Category>(categoryCreate);

            if (!_categoryRepository.CreateCategory(categoryMap))
            {
                ModelState.AddModelError("", "Something went wrong! Please try again");
                return StatusCode(500, ModelState);
            }

            return Ok("Category successfully created");   
        }

        [HttpPut("{categoryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult UpdateCategory(int categoryId, [FromBody] CategoryDto updatedCategory)
        {
            if (updatedCategory == null) return BadRequest(ModelState);

            if (categoryId != updatedCategory.Id) return BadRequest(ModelState);

            if (!_categoryRepository.CategoryExist(categoryId)) return NotFound();

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updatedCategoryMap = _mapper.Map<Category>(updatedCategory);

            if (!_categoryRepository.UpdateCategory(updatedCategoryMap))
            {
                ModelState.AddModelError("", "Something went wrong updating this category! Please try again");
                return StatusCode(500, ModelState);
            }

            return Ok("Category successfully updated.");
        }

        [HttpDelete("{categoryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult DeleteCategory(int categoryId)
        {
            if (!_categoryRepository.CategoryExist(categoryId)) return NotFound("Category does not exist");

            var categoryToDelete = _categoryRepository.GetCategory(categoryId);

            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!_categoryRepository.DeleteCategory(categoryToDelete))
            {
                ModelState.AddModelError("", "Something went wrong! Please try again");
                return StatusCode(500, ModelState);
            }

            return Ok("Category successfully deleted.");
        }
    }
}
