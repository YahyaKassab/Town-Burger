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
        Task<GenericResponse<ReturnedCart>> GetCartByCustomerId(int customerId);
        Task<GenericResponse<Order>> PlaceOrder(int addressId);
        Task<GenericResponse<ReturnedOrder>> GetOrderByIdAsync(int orderId);
        Task<GenericResponse<Order>> EditOrder(UpdateOrderDto model);
        Task<GenericResponse<Order>> DeleteOrder(int orderId);

        Task<GenericResponse<IEnumerable<ReturnedOrder>>> GetOrdersByCustomerId(int customerId);

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

                //clearing the items
                _context.RemoveRange(cart.Items);
                double total = 0;
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
                            CartId = model.Id,
                            Cart = cart,
                            Description = cartItem.Description,
                            MenuItemId = item.Id
                        });
                        total += cartItem.Quantity * item.Price;
                    }
                    cart.TotalPrice = total;
                }
                //_context.Update(cart);
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
                var address = await _context.Addresses.Include(a => a.Orders).FirstOrDefaultAsync(a => a.Id == addressId);
                var customer = await _context.Customers.Include(c => c.Orders).Include(c => c.Cart).ThenInclude(c => c.Items).FirstOrDefaultAsync(c=>c.Id == address.CustomerId);
                if (address == null)
                    return new GenericResponse<Order>
                    {
                        IsSuccess = false,
                        Message = "Address doesnt exist"
                    };

                //create the order
                var _order = new Order()
                {
                    Cart = new Cart
                    {
                        Items = customer.Cart.Items,
                        TotalPrice= customer.Cart.TotalPrice,
                    },
                    PlacedIn = DateTime.Now,
                    AddressId = addressId,
                    Address = address,
                };
                customer.Orders.Add(_order);
                _context.CartItems.RemoveRange(customer.Cart.Items.ToList());
                await _context.SaveChangesAsync();
                var _result = await _balanceService.AddDepositAsync(address.CustomerId, _order.Cart.TotalPrice);
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

        public async Task<GenericResponse<ReturnedCart>> GetCartByCustomerId(int customerId)
        {
            var cart = _context.Carts.Include(e=>e.Items).ThenInclude(e=>e.Item).FirstOrDefault(x => x.CustomerId == customerId);
            if (cart == null)
                return new GenericResponse<ReturnedCart>
                {
                    IsSuccess = false,
                    Message = "Cart Doesnt exist",
                };
            var items = new List<ReturnedCartItem>();
            foreach(var item in cart.Items)
            {
                items.Add(new ReturnedCartItem
                {
                    Description = item.Description,
                    Item = item.Item,
                    Quantity = item.Quantity,
                    ItemId= item.MenuItemId,
                });
            }
            return new GenericResponse<ReturnedCart>
            {
                IsSuccess = true,
                Message = "Cart Got Successfully",
                Result = new ReturnedCart
                {
                    Id = cart.Id,
                    Items =items
                }
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

        public async Task<GenericResponse<ReturnedOrder>> GetOrderByIdAsync(int orderId)
        {
            var order = await _context.Orders.Include(o=>o.Cart).ThenInclude(c=>c.Items).ThenInclude(i=>i.Item).Include(o => o.Address).FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
                return new GenericResponse<ReturnedOrder>
                {
                    IsSuccess = true,
                    Message = "No order with this id"
                };
            var returnedCartItems = new List<ReturnedCartItem>();
            double total = 0;
            foreach(var item in order.Cart.Items)
            {
                returnedCartItems.Add(
                    new ReturnedCartItem
                    {
                        Description = item.Description,
                        Quantity = item.Quantity,
                        ItemId = item.MenuItemId,
                        Item = item.Item

                    }
                    );
                total += item.Quantity * item.Item.Price;
            }

            var returnedOrder = new ReturnedOrder
            {
                Id = orderId,
                PlacedIn = new MyDate
                {
                    Year = order.PlacedIn.Year,
                    Month = order.PlacedIn.Month + 1,
                    Day = order.PlacedIn.Day,
                    Hour = order.PlacedIn.Hour,
                    Minute = order.PlacedIn.Minute,
                    Second = order.PlacedIn.Second
                },
                Address = order.Address,
                Cart = new ReturnedCart { 
                Id = order.Cart.Id,
                Items = returnedCartItems
                },
                State = order.State,
                TotalPrice = total,
            };
            if (order.DeliveredIn.HasValue)
            {
                returnedOrder.DeliveredIn = new MyDate
                {
                    Year = order.DeliveredIn.Value.Year,
                    Month = order.DeliveredIn.Value.Month + 1,
                    Day = order.DeliveredIn.Value.Day,
                    Hour = order.DeliveredIn.Value.Hour,
                    Minute = order.DeliveredIn.Value.Minute,
                    Second = order.DeliveredIn.Value.Second
                };
            }
            return new GenericResponse<ReturnedOrder>
            {
                IsSuccess = true,
                Message = "Order Fetched Successfully",
                Result = returnedOrder
            };
        }

        public async Task<GenericResponse<Order>> EditOrder(UpdateOrderDto model)
        {
            try
            {

                //address
                var address = await _context.Addresses.Include(o => o.Orders).FirstOrDefaultAsync(o => o.Id == model.AddressId);

                //order
                var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == model.Id);

                //if order soon
                if (order.PlacedIn > DateTime.Now.AddMinutes(-15))
                {
                    //if the address is edited
                    if(model.AddressId != order.AddressId)
                    {
                        //get the new address
                        var newAddress = await _context.Addresses.FindAsync(model.AddressId);


                    //add order to the new address
                        order.Address = newAddress;
                        order.AddressId = model.AddressId.Value;
                    }

                    //if the cart is edited
                    if(model.Cart.Items != order.Cart.Items)
                    {
                        var result = await UpdateCartAsync(model.Cart);
                        if (!result.IsSuccess)
                            return new GenericResponse<Order>
                            {
                                IsSuccess = false,
                                Message = result.Message
                            };
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
                    var order = await _context.Orders.Include(o=>o.Cart).FirstOrDefaultAsync(o=>o.Id == orderId);
                    if (order == null)
                        return new GenericResponse<Order> { IsSuccess = false, Message = "Order not found" };
                if (order.PlacedIn > DateTime.Now.AddMinutes(-25))
                {

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

        public async Task<GenericResponse<IEnumerable<ReturnedOrder>>> GetOrdersByCustomerId(int customerId)
        {
            try
            {
                var orders = await _context.Orders.Include(o=>o.Address).Include(o=>o.Cart).ThenInclude(c=>c.Items).ThenInclude(i=>i.Item).Where(o => o.CustomerId == customerId).ToListAsync();
                if (orders.Count == 0)
                    return new GenericResponse<IEnumerable<ReturnedOrder>>
                    {
                        IsSuccess = true,
                        Message = "You dont have any orderes yet"
                    };
                var returnedOrders = new List<ReturnedOrder>();
                foreach (var order in orders)
                {
                double total = 0;
                var returnedCartItems = new List<ReturnedCartItem>();
                    foreach(var item in order.Cart.Items)
                    {
                        returnedCartItems.Add(new ReturnedCartItem
                        {
                            ItemId = item.Id,
                            Item = item.Item,
                            Description = item.Description,
                            Quantity = item.Quantity,
                        });
                        total += item.Quantity* item.Item.Price ;
                    }
                    var myCart = new ReturnedCart
                    {
                        Id = order.Cart.Id,
                        Items = returnedCartItems
                    };

                    returnedOrders.Add(new ReturnedOrder
                    {
                        Id = order.Id,
                        Address = order.Address,
                        Cart = myCart,
                        PlacedIn = new MyDate
                        {
                            Year = order.PlacedIn.Year,
                            Month = order.PlacedIn.Month,
                            Day = order.PlacedIn.Day,
                            Hour = order.PlacedIn.Hour,
                            Minute = order.PlacedIn.Minute,
                            Second = order.PlacedIn.Second,
                        },
                        State = order.State,
                        TotalPrice = total
                    });
                    if (order.DeliveredIn.HasValue)
                    {
                        returnedOrders.Last().DeliveredIn = new MyDate
                        {
                            Year = order.DeliveredIn.Value.Year,
                            Month = order.DeliveredIn.Value.Month,
                            Day = order.DeliveredIn.Value.Day,
                            Hour = order.DeliveredIn.Value.Hour,
                            Minute = order.DeliveredIn.Value.Minute,
                            Second = order.DeliveredIn.Value.Second,
                        };
                    }
                }
                return new GenericResponse<IEnumerable<ReturnedOrder>> { IsSuccess = true, Message = "Orders fetched Successfully", Result = returnedOrders };
            }
            catch (Exception ex)
            {
                return new GenericResponse<IEnumerable<ReturnedOrder>>
                {
                    IsSuccess = false,
                    Message = "Nigga"
                };
            }
        }

    }
}
