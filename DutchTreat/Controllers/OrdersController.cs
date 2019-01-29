using AutoMapper;
using DutchTreat.Data;
using DutchTreat.Data.Entities;
using DutchTreat.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DutchTreat.Controllers
{
    [Route("api/[Controller]")]
    [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
    public class OrdersController : Controller
    {
        private readonly IDutchRepository _repository;
        private readonly ILogger<OrdersController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<StoreUser> _userManager;

        public OrdersController(IDutchRepository repository, ILogger<OrdersController> logger, IMapper mapper, UserManager<StoreUser> userManager)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Get(bool includeItems = true)
        {
            try
            {
                // use identity in read operations
                var username = User.Identity.Name;

                var results = _repository.GetAllOrdersByUser(username, includeItems);
                //var results = _repository.GetAllOrders(includeItems);

                return Ok(_mapper.Map<IEnumerable<Order>, IEnumerable<OrderViewModel>>(results));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get orders: {ex}");
                return BadRequest("Failed to get orders");
            }
            // Manual mapping
            //try
            //{
            //    return Ok(_repository.GetAllOrders());
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError($"Failed to get orders: {ex}");
            //    return BadRequest("Failed to get orders");
            //}
        }

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            try
            {
                var order = _repository.GetOrderById(User.Identity.Name, id);

                if (order != null) return Ok(_mapper.Map<Order, OrderViewModel>(order));
                else return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get orders: {ex}");
                return BadRequest("Failed to get orders");
            }
            // Manual mapping
            //try
            //{
            //    var order = _repository.GetOrderById(id);

            //    if (order != null) return Ok(order);
            //    else return NotFound();
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError($"Failed to get orders: {ex}");
            //    return BadRequest("Failed to get orders");
            //}
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]OrderViewModel model)
        {
                        
            // add it to the db
            try
            {
                if (ModelState.IsValid)
                {
                    var newOrder = _mapper.Map<OrderViewModel, Order>(model);

                    if (newOrder.OrderDate == DateTime.MinValue)   // if order date was not specified by client
                    {
                        newOrder.OrderDate = DateTime.Now;
                    }

                    // get the current user so that we can create an order for the user
                    var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);  // this is the list of Claims user
                    newOrder.User = currentUser;                                               // this user is assigned to the new data added to the database

                    //_repository.AddEntity(newOrder);
                    _repository.AddOrder(newOrder);
                    if (_repository.SaveAll())
                    {
                        return Created($"/api/orders/{newOrder.Id}", _mapper.Map<Order, OrderViewModel>(newOrder));
                    }

                    // manual mapping
                    //var newOrder = new Order()
                    //{
                    //    OrderDate = model.OrderDate,
                    //    OrderNumber = model.OrderNumber,
                    //    Id = model.OrderId
                    //};

                    //if (newOrder.OrderDate == DateTime.MinValue)  // if order date was not specified by client
                    //{
                    //    newOrder.OrderDate = DateTime.Now;
                    //}

                    //_repository.AddEntity(model);
                    //if (_repository.SaveAll())
                    //{
                    //    var vm = new OrderViewModel()
                    //    {
                    //        OrderId = newOrder.Id,
                    //        OrderDate = newOrder.OrderDate,
                    //        OrderNumber = newOrder.OrderNumber
                    //    };

                    //    return Created($"/api/orders/{vm.OrderId}", vm);   // return the vm
                    //}                    
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"++++++ Failed to save a new order+++++++++: {ex}");
            }

            return BadRequest("Failed to save new order");
        }

    }
}
