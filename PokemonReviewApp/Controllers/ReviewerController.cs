using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Interfaces;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewerController : Controller
    {
        private readonly IReviewerRepository _reviewerRepository;
        private readonly IMapper _mapper;

        public ReviewerController(IReviewerRepository reviewerRepository, IMapper mapper)
        {
            _reviewerRepository = reviewerRepository;
            _mapper = mapper;
        }

        [HttpGet()]
        [ProducesResponseType(200)]

    }
}
