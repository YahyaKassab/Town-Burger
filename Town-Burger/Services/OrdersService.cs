using Microsoft.EntityFrameworkCore;
using Town_Burger.Models;
using Town_Burger.Models.Context;
using Town_Burger.Models.Responses;

namespace Town_Burger.Services
{
    public interface IOrdersService
    {
        Task<GenericResponse<Cart>> UpdateCartAsync(Cart cart);
        Task<GenericResponse<Cart>> GetCartByCustomerId(int customerId);
        Task<GenericResponse<Order>> PlaceOrder(Order order); 
    }

    public class OrdersService : IOrdersService
    {
        private readonly AppDbContext _context;

        public OrdersService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GenericResponse<Cart>> UpdateCartAsync(Cart cart)
        {
            if (cart == null)
                return new GenericResponse<Cart>
                {
                    IsSuccess = false,
                    Message = "Cart is null"
                };
            //cart isnt null
            try
            {
                double total = 0;
                if(cart.Items.Any())
                {
                    foreach (var item in cart.Items)
                    {
                        total += item.Quantity * item.Item.Price;
                    }
                    cart.TotalPrice = total;
                }
                var result = _context.Carts.Update(cart);
                await _context.SaveChangesAsync();
                return new GenericResponse<Cart>
                {
                    IsSuccess = true,
                    Message = "Cart Updated Successfully",
                    Result = cart
                };
            }catch(Exception ex)
            {
                return new GenericResponse<Cart>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
            
        }
        public async Task<GenericResponse<Order>> PlaceOrder(Order order)
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResponse<Cart>> GetCartByCustomerId(int customerId)
        {
            var cart = _context.Carts.Include(e=>e.Items).ThenInclude(e=>e.Item).FirstOrDefault(x => x.CustomerId == customerId);
            if (cart == null)
                return new GenericResponse<Cart>
                {
                    IsSuccess = false,
                    Message = "User Doesnt exist",
                };
            return new GenericResponse<Cart>
            {
                IsSuccess = true,
                Message = "Cart Got Successfully",
                Result = cart
            };

        }
    }
}
