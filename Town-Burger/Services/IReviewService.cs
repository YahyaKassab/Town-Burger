using Microsoft.EntityFrameworkCore;
using Town_Burger.Models;
using Town_Burger.Models.Context;
using Town_Burger.Models.Dto;
using Town_Burger.Models.Responses;

namespace Town_Burger.Services
{
    public interface IReviewService
    {

        //add from the parent table not from the child indepenedently 

        Task<GenericResponse<Review>> AddReviewAsync(ReviewDto review);
        Task<GenericResponse<IEnumerable<Review>>> GetLatest();
        Task<GenericResponse<IEnumerable<Review>>> GetAll();
    }
    public class ReviewService : IReviewService
    {
        private readonly AppDbContext _context;
        private readonly ICustomerService _customerService;

        public ReviewService(AppDbContext context, ICustomerService customerService)
        {
            _context = context;
            _customerService = customerService;
        }

        public async Task<GenericResponse<Review>> AddReviewAsync(ReviewDto review)
        {
            try
            {
                var customer = await _context.Customers.Include(c=>c.Reviews).SingleOrDefaultAsync(c=>c.Id == review.CustomerId);
                if (customer == null)
                    return new GenericResponse<Review>
                    {
                        IsSuccess = false,
                        Message = "Customer not found"
                    };
                var _review = new Review()
                {
                    Title = review.Title,
                    Description = review.Description,
                    Rating = review.Rating,
                    CustomerId = review.CustomerId,
                    Time = DateTime.Now,
                };
                customer.Reviews.Add(_review);
                await _context.SaveChangesAsync();
                return new GenericResponse<Review>
                {
                    IsSuccess = true,
                    Message = "Review Added Successfully",
                    Result = _review
                };
            }catch(Exception ex)
            {
                return new GenericResponse<Review>
                {
                    IsSuccess = false,
                    Message = "Failed"
                };
            }

        }

        public async Task<GenericResponse<IEnumerable<Review>>> GetAll()
        {
            try
            {
                var reviews = await _context.Reviews.Include(r=>r.Customer).ToListAsync();
                if (reviews.Count == 0)
                    return new GenericResponse<IEnumerable<Review>>()
                    {
                        IsSuccess = true,
                        Message = "No Reviews yet"
                    };
                return new GenericResponse<IEnumerable<Review>>()
                {
                    IsSuccess = true,
                    Message = "Reviews fetched Successfully",
                    Result = reviews
                };
            }catch(Exception ex)
            {
                return new GenericResponse<IEnumerable<Review>>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<GenericResponse<IEnumerable<Review>>> GetLatest()
        {
            var Reviews = await GetAll();
            if (!Reviews.IsSuccess)
                return new GenericResponse<IEnumerable<Review>>()
                {
                    IsSuccess = false,
                    Message = Reviews.Message
                };
            var latest = Reviews.Result.OrderByDescending(e => e.Time).Take(3);
            
            return new GenericResponse<IEnumerable<Review>>()
            {
                IsSuccess = true,
                Message = Reviews.Message,
                Result = latest
            };
        }
    }
}
