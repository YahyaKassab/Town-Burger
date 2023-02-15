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

        Task<GenericResponse<ReturnedReview>> GetReviewById(int Id);
        Task<GenericResponse<Review>> AddReviewAsync(ReviewDto review);
        Task<GenericResponse<Review>> UpdateReview(Review review);
        Task<GenericResponse<Review>> DeleteReview(int reviewId);
        Task<GenericResponse<IEnumerable<ReturnedReview>>> GetLatest();
        Task<GenericResponse<IEnumerable<ReturnedReview>>> GetAll();
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

        public async Task<GenericResponse<Review>> DeleteReview(int reviewId)
        {
            try
            {
                var review = await _context.Reviews.FindAsync(reviewId);
                if (review == null)
                    return new GenericResponse<Review>
                    {
                        IsSuccess = false,
                        Message = "Review Not Found",
                    };
                _context.Remove(review);
                await _context.SaveChangesAsync();
                return new GenericResponse<Review> { IsSuccess = true, Message = "Review Deleted Successfully", Result = review };
            }catch (Exception ex)
            {
                return new GenericResponse<Review>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }

        }

        public async Task<GenericResponse<IEnumerable<ReturnedReview>>> GetAll()
        {
            try
            {
                var reviews = await _context.Reviews.Include(r=>r.Customer).ThenInclude(c=>c.User).ToListAsync();
                var _reviewsToReturn = new List<ReturnedReview>();
                foreach (var review in reviews)
                {
                    _reviewsToReturn.Add(
                        new ReturnedReview
                        {
                            Title = review.Title,
                            Description = review.Description,
                            CustomerEmail = review.Customer.User.Email,
                            CustomerId = review.CustomerId,
                            CustomerName = review.Customer.FullName,
                            Id = review.Id,
                            Rating = review.Rating,
                            Time = review.Time,
                        }
                        );
                }
                if (reviews.Count == 0)
                    return new GenericResponse<IEnumerable<ReturnedReview>>()
                    {
                        IsSuccess = true,
                        Message = "No Reviews yet"
                    };
                return new GenericResponse<IEnumerable<ReturnedReview>>()
                {
                    IsSuccess = true,
                    Message = "Reviews fetched Successfully",
                    Result = _reviewsToReturn
                };
            }catch(Exception ex)
            {
                return new GenericResponse<IEnumerable<ReturnedReview>>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<GenericResponse<IEnumerable<ReturnedReview>>> GetLatest()
        {
            var Reviews = await GetAll();
            if (!Reviews.IsSuccess)
                return new GenericResponse<IEnumerable<ReturnedReview>>()
                {
                    IsSuccess = false,
                    Message = Reviews.Message
                };
            var latest = Reviews.Result.OrderByDescending(e => e.Time).Take(3);
            
            return new GenericResponse<IEnumerable<ReturnedReview>>()
            {
                IsSuccess = true,
                Message = Reviews.Message,
                Result = latest
            };
        }

        public async Task<GenericResponse<ReturnedReview>> GetReviewById(int Id)
        {
            try
            {
                var review = await _context.Reviews.Include(r=>r.Customer).ThenInclude(c=>c.User).FirstOrDefaultAsync(r=>r.Id == Id);
                if (review == null)
                    return new GenericResponse<ReturnedReview> { IsSuccess = false, Message = "Review not found" };
                return new GenericResponse<ReturnedReview>
                {
                    IsSuccess = true,
                    Message = "Review Fetched Successfully",
                    Result = new ReturnedReview
                    {
                        Id = review.Id,
                        Rating= review.Rating,
                        CustomerEmail = review.Customer.User.Email,
                        Description= review.Description,
                        CustomerId= review.CustomerId,
                        CustomerName = review.Customer.FullName,
                        Time= review.Time,
                        Title = review.Title,
                    }
                };

            }catch (Exception ex)
            {
                return new GenericResponse<ReturnedReview>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<GenericResponse<Review>> UpdateReview(Review review)
        {
            try
            {
                _context.Update(review);
                await _context.SaveChangesAsync();
                return new GenericResponse<Review>
                {
                    IsSuccess = true,
                    Message = "Successfully Updated",
                    Result = review
                };
            }catch (Exception ex)
            {
                return new GenericResponse<Review>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}
