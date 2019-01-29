using DutchTreat.Data;
using DutchTreat.Services;
using DutchTreat.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DutchTreat.Controllers
{
    public class AppController : Controller
    {
        private readonly IMailService _mailService;
        private readonly IDutchRepository _repository;

        public AppController(IMailService mailService, IDutchRepository repository)
        {
            this._mailService = mailService;
            _repository = repository;
        }

        public IActionResult Index()
        {
            var result = _repository.GetAllProducts();      // The seeder returns 800 count of product
            return View();
        }

        [HttpGet("Contact")]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost("Contact")]
        public IActionResult Contact(ContactViewModel model)
        {    
            if (ModelState.IsValid)
            {
                // Send the email
                _mailService.SendMessage("josephinegbe.uk@gmail.com", model.Subject, $"From: {model.Name} - {model.Email}, Message: {model.Message}");
                ViewBag.UserMessage = "Mail Sent";
                ModelState.Clear();
            }           

            return View();
        }

        // 
        public IActionResult About()
        {
            ViewBag.Title = "About Us";

            return View();
        }

        // [Authorize]   // force people to login in order to shop. Now we use the "cart" button. This allow people to shop but force then to login on checkout.
        public IActionResult Shop()
        {            
            return View();
        }

    }
}
