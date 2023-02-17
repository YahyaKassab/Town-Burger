using EllipticCurve.Utils;
using MailKit.Search;
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


        Task<GenericResponse<Cart>> UpdateCartAsync(UpdateCartDto model);
        Task<GenericResponse<Cart>> GetCartByCustomerId(int customerId);
        Task<GenericResponse<Order>> PlaceOrder(int addressId);
        Task<GenericResponse<Order>> GetOrderByIdAsync(int orderId);
        Task<GenericResponse<Order>> EditOrder(UpdateOrderDto model);
        Task<GenericResponse<Order>> DeleteOrder(int orderId);

        Task<GenericResponse<IEnumerable<Order>>> GetOrdersByCustomerId(int customerId);

        Task<GenericResponse<int>> UpdateState(int orderId, int state);
        Task<GenericResponse<IEnumerable<(MenuItem item, int count)>>> GetMostOrdered();
        Task<GenericResponse<IEnumerable<(MenuItem item, int count)>>> GetMostOrderedByType(string type);
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

        //Add migration
        //fix anything includes orders
        //fix anything includes addresses


        public async Task<GenericResponse<Cart>> UpdateCartAsync(UpdateCartDto model)
        {
            var cart = await _context.Carts.Include(c => c.Items).ThenInclude(i => i.Item).FirstOrDefaultAsync(c=>c.Id == model.Id);
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

                //clearing the items
                if (cart.Items.Any())
                {
                    _context.RemoveRange(cart.Items);
                    cart.TotalPrice = 0;
                    _context.SaveChanges();
                }
                if(model.Items.Any())
                {

                    //adding new items
                    foreach (var cartItem in model.Items)
                    {
                        //menu item
                        var item = await _context.MenuItems.FindAsync(cartItem.ItemId);

                        if (item == null)
                            return new GenericResponse<Cart>
                            {
                                IsSuccess = false,
                                Message = "Menu Item Not Found"
                            };

                    cart.Items.Add(new CartItem
                    {
                        Item = item,
                        Quantity = cartItem.Quantity,
                        Description = cartItem.Description,
                       ItemId = item.Id
                    });
                        total += cartItem.Quantity * item.Price;
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
        public async Task<GenericResponse<Order>> PlaceOrder(int addressId)
        {
            try
            {
                //get the address
                //with the customer
                var address = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == addressId);
                var customer = await _context.Customers.Include(c => c.Orders).Include(c => c.Cart).ThenInclude(c => c.Items).ThenInclude(i=>i.Item).FirstOrDefaultAsync(c=>c.Id == address.CustomerId);
                if (address == null)
                    return new GenericResponse<Order>
                    {
                        IsSuccess = false,
                        Message = "Address doesnt exist"
                    };

                //create the order
                var cartItems = new List<CartItem>();
                double total = 0;
                foreach(var item in customer.Cart.Items)
                {
                    cartItems.Add(item);
                    total += item.Quantity * item.Item.Price;
                }
                customer.Cart.Items.Clear();


                var _order = new Order()
                {
                    CartItems = cartItems,
                    PlacedIn = DateTime.Now,
                    AddressId = addressId,
                    Address = address,
                    TotalPrice= total,
                    CustomerId=customer.Id,
                };
                customer.Orders.Add(_order);
                _context.CartItems.RemoveRange(customer.Cart.Items.ToList());
                await _context.SaveChangesAsync();
                var _result = await _balanceService.AddDepositAsync(address.CustomerId, _order.TotalPrice);
                if (!_result.IsSuccess)
                    return new GenericResponse<Order>
                    {
                        IsSuccess = false,
                        Message = _result.Message
                    };
                var result = await _mailService.SendOrderPlacedEmail(address.CustomerId);
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
                var order = await _context.Orders.Include(o=>o.Address).FirstOrDefaultAsync(o=>o.Id == orderId);
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
                    var result = await _mailService.SendOrderOutEmail(order.Address.CustomerId);
                    if (!result.IsSuccess)
                        return new GenericResponse<int>
                        {
                            IsSuccess = false,
                            Message = "Send Email Failed"
                        };
                }

                if(state == 2)
                {
                    order.DeliveredIn = DateTime.Now;
                    await _context.SaveChangesAsync();
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

            //All CartItems ordered in the last month
            var cartItems = await _context.CartItems.Include(c=>c.Item).Include(c=>c.Order).Where(e => e.OrderId != null && e.Order.PlacedIn > DateTime.Now.AddMonths(-1)).ToListAsync();

            if (cartItems.Count == 0)
                return new GenericResponse<IEnumerable<(MenuItem item, int count)>>
                {
                    IsSuccess = true,
                    Message = "There was no orders in the last month"
                };

            //fetch success

            var ItemsCounter = new List<MostOrderedCounter>();

            foreach(var cartItem in cartItems)
            {
                        //menu item exists
                    if(ItemsCounter.Exists(item=>item.item.Id == cartItem.Item.Id))
                    {
                        //got my item in the list
                        var ItemInList = ItemsCounter.Find(item => item.item.Id == cartItem.Item.Id);


                        //increase the counter
                        ItemInList.counter += cartItem.Quantity;


                    }
                    else
                    {
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
            var order = await _context.Orders.Include(o=>o.CartItems).ThenInclude(i=>i.Item).Include(o => o.Address).FirstOrDefaultAsync(o => o.Id == orderId);
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

        public async Task<GenericResponse<Order>> EditOrder(UpdateOrderDto model)
        {
            try
            {

                //address
                var address = await _context.Addresses.Include(o => o.Orders).FirstOrDefaultAsync(o => o.Id == model.AddressId);

                //order
                var order = await _context.Orders.Include(o=>o.CartItems).FirstOrDefaultAsync(o => o.Id == model.Id);

                //if order soon
                if (order.PlacedIn > DateTime.Now.AddMinutes(-15))
                {
                    //if the address is edited
                    if(model.AddressId != order.AddressId && model.AddressId.HasValue)
                    {
                    //add order to the new address
                        order.AddressId = model.AddressId.Value;
                    }
                    
                    double total = 0;

                    //if the cart is edited
                    if(model.Items != order.CartItems)
                    {
                        _context.CartItems.RemoveRange(order.CartItems);

                        var itemsToAdd = new List<CartItem>();
                        foreach (var item in model.Items)
                        {
                            itemsToAdd.Add(new CartItem
                            {
                                Description = item.Description,
                                ItemId = item.ItemId,
                                Quantity = item.Quantity,
                            });
                            var menuItem = await _context.MenuItems.FindAsync(item.ItemId);

                            total += item.Quantity * menuItem.Price ;
                        }
                        order.TotalPrice = total;
                        order.CartItems = itemsToAdd;
                    }
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
                    var order = await _context.Orders.Include(o=>o.CartItems).FirstOrDefaultAsync(o=>o.Id == orderId);
                    if (order == null)
                        return new GenericResponse<Order> { IsSuccess = false, Message = "Order not found" };

                if (order.PlacedIn > DateTime.Now.AddMinutes(-25))
                {

                    _context.RemoveRange(order.CartItems);
                    _context.Remove(order);
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
                var orders = await _context.Orders.Include(o=>o.Address).Include(o=>o.CartItems).ThenInclude(i=>i.Item).Where(o => o.CustomerId == customerId).ToListAsync();
                if (orders.Count == 0)
                    return new GenericResponse<IEnumerable<Order>>
                    {
                        IsSuccess = true,
                        Message = "You dont have any orderes yet"
                    };
                return new GenericResponse<IEnumerable<Order>> { IsSuccess = true, Message = "Orders fetched Successfully", Result = orders };
            }
            catch (Exception ex)
            {
                return new GenericResponse<IEnumerable<Order>>
                {
                    IsSuccess = false,
                    Message = "Nigga"
                };
            }
        }

        public async Task<GenericResponse<IEnumerable<(MenuItem item, int count)>>> GetMostOrderedByType(string type)
        {


            //All CartItems ordered in the last month
            var cartItems = await _context.CartItems.Include(c => c.Item).Include(c => c.Order).Where(e => e.OrderId != null && e.Order.PlacedIn > DateTime.Now.AddMonths(-1) && e.Item.Type.ToLower() == type.ToLower()).ToListAsync();

            if (cartItems.Count == 0)
                return new GenericResponse<IEnumerable<(MenuItem item, int count)>>
                {
                    IsSuccess = true,
                    Message = "There was no orders of this type in the last month"
                };

            //fetch success

            var ItemsCounter = new List<MostOrderedCounter>();

            foreach (var cartItem in cartItems)
            {
                //menu item exists
                if (ItemsCounter.Exists(item => item.item.Id == cartItem.Item.Id))
                {
                    //got my item in the list
                    var ItemInList = ItemsCounter.Find(item => item.item.Id == cartItem.Item.Id);


                    //increase the counter
                    ItemInList.counter += cartItem.Quantity;


                }
                else
                {
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
            if (ItemsOrdered.Count < 3)
            {
                var menuItems = new List<(MenuItem item, int count)>();
                foreach (var item in ItemsOrdered)
                {
                    menuItems.Add((item.item, item.counter));
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
    }
}
