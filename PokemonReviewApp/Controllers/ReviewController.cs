using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using PokemonReviewApp.Repository;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : Controller
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IReviewerRepository _reviewerRepository;
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IMapper _mapper;

        public ReviewController(IReviewRepository reviewRepository, IReviewerRepository reviewerRepository, IPokemonRepository pokemonRepository, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _reviewerRepository = reviewerRepository;
            _pokemonRepository = pokemonRepository;
            _mapper = mapper;
        }

        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(Review))]
        public IActionResult GetReviews()
        {
            var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviews());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(reviews);
        }

        [HttpGet("{reviewId}")]
        [ProducesResponseType(200, Type = typeof(Review))]
        [ProducesResponseType(400)]
        public IActionResult GetReview(int reviewId)
        {
            if (!_reviewRepository.ReviewExists(reviewId))
            {
                return NotFound();
            }

            var review = _mapper.Map<ReviewDto>(_reviewRepository.GetReview(reviewId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(review);
        }

        [HttpGet("pokemon/{pokeId}")]
        [ProducesResponseType(200, Type = typeof(Review))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewsOfAPokemon(int pokeId)
        {
            var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviewsOfAPokemon(pokeId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(reviews);

        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateReview([FromQuery] int reviewerId, [FromQuery] int pokeId, [FromBody] ReviewDto reviewCreate)
        {
            if (reviewCreate == null) return BadRequest(ModelState);

            if (reviewCreate.Text == "")
            {
                ModelState.AddModelError("", "Comment box cannot be empty. Please leave a review for this pokemon.");
                return StatusCode(409);
            }

            if (!ModelState.IsValid) return BadRequest(ModelState);
            var reviewMap = _mapper.Map<Review>(reviewCreate);
            reviewMap.Reviewer = _reviewerRepository.GetReviewer(reviewerId);
            reviewMap.Pokemon = _pokemonRepository.GetPokemon(pokeId);

            if (!_reviewRepository.CreateReview(reviewMap))
            {
                ModelState.AddModelError("", "Something went wrong! Please try again");
                return StatusCode(500, ModelState);
            }

            return Ok("Review successfully added");
        }

        [HttpPut("{reviewId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult UpdateReview(int reviewId, [FromBody] ReviewDto updatedReview)
        {
            if (updatedReview == null) return BadRequest(ModelState);

            if (reviewId != updatedReview.Id) return BadRequest(ModelState);

            if (!_reviewRepository.ReviewExists(reviewId)) return NotFound();

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updatedReviewMap = _mapper.Map<Review>(updatedReview);

            if (!_reviewRepository.UpdateReview(updatedReviewMap))
            {
                ModelState.AddModelError("", "Something went wrong! Please try again");
                return StatusCode(500, ModelState);
            }

            return Ok("Review successfully updated.");
        }
    }
}