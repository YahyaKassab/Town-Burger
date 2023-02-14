using EllipticCurve.Utils;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.Xml;
using Town_Burger.Models;
using Town_Burger.Models.Context;
using Town_Burger.Models.Dto;
using Town_Burger.Models.Responses;

namespace Town_Burger.Services
{
    public interface IOrdersService
    {


        Task<GenericResponse<Cart>> UpdateCartAsync(Cart cart);
        Task<GenericResponse<Cart>> GetCartByCustomerId(int customerId);
        Task<GenericResponse<Order>> PlaceOrder(int customerId);
        Task<GenericResponse<Order>> GetOrderByIdAsync(int orderId);
        Task<GenericResponse<Order>> EditOrder(Order order);
        Task<GenericResponse<Order>> DeleteOrder(int orderId);

        Task<GenericResponse<IEnumerable<Order>>> GetOrdersByCustomerId(int customerId);

        Task<GenericResponse<int>> UpdateState(int orderId, int state);
        Task<GenericResponse<IEnumerable<(MenuItem item, int count)>>> GetMostOrdered();
    }

    public class OrdersService : IOrdersService
    {
        private readonly AppDbContext _context;
        private readonly ICustomerService _customerService;
        private readonly IBalanceService _balanceService;
        private readonly IMailService _mailService;

        public OrdersService(AppDbContext context, ICustomerService customerService, IBalanceService balanceService, IMailService mailService)
        {
            _context = context;
            _customerService = customerService;
            _balanceService = balanceService;
            _mailService = mailService;
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
                        item.Item = await _context.MenuItems.FindAsync(item.MenuItemId);
                        item.CartId = cart.Id;
                        total += item.Quantity * item.Item.Price;
                    }
                    cart.TotalPrice = total;
                }
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
        public async Task<GenericResponse<Order>> PlaceOrder(int customerId)
        {
            try
            {
                var customer = await _context.Customers.Include(c=>c.Orders).Include(c=>c.Cart).FirstOrDefaultAsync(c=>c.Id == customerId);
                if (customer == null)
                    return new GenericResponse<Order>
                    {
                        IsSuccess = false,
                        Message = "Customer doesnt exist"
                    };
                var _order = new Order()
                {
                    Cart = customer.Cart,
                    CustomerId = customerId,
                    PlacedIn = DateTime.Now,
                    State = 0
                };
                customer.Orders.Add(_order);
                await _context.SaveChangesAsync();
                customer.Cart.CustomerId = null;
                customer.Cart.Customer = null;
                customer.Cart.OrderId = _order.Id;
                customer.Cart.Order = _order;
                customer.Cart = new Cart()
                {
                    CustomerId = customerId,
                };
                await _context.SaveChangesAsync();
                var _result = await _balanceService.AddDepositAsync(customerId, customer.Cart.TotalPrice);
                if (!_result.IsSuccess)
                    return new GenericResponse<Order>
                    {
                        IsSuccess = false,
                        Message = _result.Message
                    };
                var result = await _mailService.SendOrderPlacedEmail(customerId);
                if (!result.IsSuccess)
                    return new GenericResponse<Order>
                    {
                        IsSuccess = false,
                        Message = _result.Message
                    };
                return new GenericResponse<Order>
                {
                    IsSuccess = true,
                    Message = "Order Added Successfully",
                    Result = _order
                };
            }catch (Exception ex)
            {
                return new GenericResponse<Order>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }

        public async Task<GenericResponse<Cart>> GetCartByCustomerId(int customerId)
        {
            var cart = _context.Carts.Include(e=>e.Items).ThenInclude(e=>e.Item).FirstOrDefault(x => x.CustomerId == customerId);
            if (cart == null)
                return new GenericResponse<Cart>
                {
                    IsSuccess = false,
                    Message = "Cart Doesnt exist",
                };
            return new GenericResponse<Cart>
            {
                IsSuccess = true,
                Message = "Cart Got Successfully",
                Result = cart
            };

        }

        public async Task<GenericResponse<int>> UpdateState(int orderId,int state)
        {
            try
            {
                var order = await _context.Orders.FindAsync(orderId);
                if (order == null)
                    return new GenericResponse<int>
                    {
                        IsSuccess = false,
                        Message = "Order Not Found"
                    };
                //Order Exists
                order.State = state;
                await _context.SaveChangesAsync();
                if (state == 1)
                {
                    var result = await _mailService.SendOrderOutEmail(order.CustomerId);
                    if (!result.IsSuccess)
                        return new GenericResponse<int>
                        {
                            IsSuccess = false,
                            Message = "Send Email Failed"
                        };
                }

                return new GenericResponse<int>
                {
                    IsSuccess = true,
                    Message = "State Updated Successfully",
                    Result = order.State
                };

            }catch (Exception ex)
            {
                return new GenericResponse<int>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }

        }

        public async Task<GenericResponse<IEnumerable<(MenuItem item, int count)>>> GetMostOrdered()
        {
            var carts = await _context.Carts.Include(e => e.Order).Include(e=>e.Items).ThenInclude(e=>e.Item).Where(e => e.Order != null && e.Order.PlacedIn > DateTime.Now.AddMonths(-1)).ToListAsync();
            if (carts.Count == 0)
                return new GenericResponse<IEnumerable<(MenuItem item, int count)>>
                {
                    IsSuccess = false,
                    Message = "Failed Or there was no orders in the last month"
                };

            //fetch success

            var ItemsCounter = new List<MostOrderedCounter>();

            foreach(var cart in carts)
            {
                foreach(var cartItem in cart.Items)
                {
                        //menu item exists
                    if(ItemsCounter.Exists(item=>item.item.Id == cartItem.Item.Id))
                    {
                        //got my item in the list
                        var ItemInList = ItemsCounter.Find(item => item.item.Id == cartItem.Item.Id);
                        //increase the counter
                        ItemInList.counter += cartItem.Quantity;
                    }
                    //menu item doesnt exist

                    //add the item
                    ItemsCounter.Add(new MostOrderedCounter()
                    {
                        counter = cartItem.Quantity,
                        item = cartItem.Item
                    });
                }
            }
            var ItemsOrdered = ItemsCounter.OrderByDescending(item => item.counter).ToList();
            if(ItemsOrdered.Count < 3)
            {
                var menuItems = new List<(MenuItem item, int count)>();
                foreach(var item in ItemsOrdered)
                {
                    menuItems.Add((item.item,item.counter));
                }
                return new GenericResponse<IEnumerable<(MenuItem item, int count)>>
                {
                    IsSuccess = true,
                    Message = $"Top {ItemsOrdered.Count} Most ordered",
                    Result = menuItems
                };
            }

            //more than 3
            return new GenericResponse<IEnumerable<(MenuItem item, int count)>>
            {
                IsSuccess = true,
                Message = "Top 3 Most ordered",
                Result = new (MenuItem item, int count)[]
                    {
                        (ItemsOrdered[0].item,ItemsOrdered[0].counter),
                        (ItemsOrdered[1].item, ItemsOrdered[1].counter),
                        (ItemsOrdered[2].item, ItemsOrdered[2].counter),
                    }
            };

        }

        public async Task<GenericResponse<Order>> GetOrderByIdAsync(int orderId)
        {
            var order = await _context.Orders.Include(o => o.Customer).FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
                return new GenericResponse<Order>
                {
                    IsSuccess = true,
                    Message = "No order with this id"
                };
            return new GenericResponse<Order>
            {
                IsSuccess = true,
                Message = "Order Fetched Successfully",
                Result = order
            };
        }

        public async Task<GenericResponse<Order>> EditOrder(Order order)
        {
            try
            {
                if(order.PlacedIn > DateTime.Now.AddMinutes(-15))
                {
                    order.Customer = await _context.Customers.Include(c=>c.Cart).FirstOrDefaultAsync(c=>c.Id == order.CustomerId);
//                    order.Cart = order.Customer.Cart;
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                    return new GenericResponse<Order> { IsSuccess = true, Message = "Edited Successfully", Result = order };
                }
                return new GenericResponse<Order>
                {
                    IsSuccess = false,
                    Message = "15 Mins passed you cant edit the order now"
                };
            }catch(Exception ex)
            {
                return new GenericResponse<Order>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }

        public async Task<GenericResponse<Order>> DeleteOrder(int orderId)
        {
            try
            {
                    var order = await _context.Orders.Include(o=>o.Cart).FirstOrDefaultAsync(o=>o.Id == orderId);
                    if (order == null)
                        return new GenericResponse<Order> { IsSuccess = false, Message = "Order not found" };
                if (order.PlacedIn > DateTime.Now.AddMinutes(-25))
                {
                    var customer = await _context.Customers.Include(c=>c.Orders).FirstOrDefaultAsync(c=>c.Id == order.CustomerId);
                    if (customer == null)
                        return new GenericResponse<Order> { IsSuccess = false, Message = "customer doesnt exist" };
                    _context.Remove(order);
                    _context.Remove(order.Cart);
                    await _context.SaveChangesAsync();
                    return new GenericResponse<Order> { IsSuccess = true, Message = "removed Successfully", Result = order };
                }
                return new GenericResponse<Order>
                {
                    IsSuccess = false,
                    Message = "25 Mins passed you cant delete the order now"
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<Order>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }

        public async Task<GenericResponse<IEnumerable<Order>>> GetOrdersByCustomerId(int customerId)
        {
            try
            {
                var orders = await _context.Orders.Where(o => o.CustomerId == customerId).ToListAsync();
                if (orders.Count == 0)
                    return new GenericResponse<IEnumerable<Order>>
                    {
                        IsSuccess = true,
                        Message = "You dont have any orderes yet"
                    };
                return new GenericResponse<IEnumerable<Order>> { IsSuccess = true, Message = "Orders fetched Successfully", Result = orders };
            }catch (Exception ex)
            {
                return new GenericResponse<IEnumerable<Order>>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }

    }
}
