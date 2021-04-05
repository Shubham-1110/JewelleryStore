using JewelleryStore.BusinessService.Interface;
using JewelleryStore.Common;
using JewelleryStore.Enum;
using JewelleryStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using static JewelleryStore.Delegates.Delegates;

namespace JewelleryStore.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IGoldPriceEstimation _goldPriceEstimation;

        private readonly ServiceResolver _serviceResolver;

        private bool eligibleForDiscount;

        public HomeController(IGoldPriceEstimation goldPriceEstimation, ServiceResolver serviceResolver)
        {
            _goldPriceEstimation = goldPriceEstimation;
            _serviceResolver = serviceResolver;
        }

        [HttpPost]
        [Route(RouteConstants.Calculate)]
        public IActionResult CalculateTotalPrice([FromBody] PriceEstimationModel priceEstimationModel)
        {
            if(ModelState.IsValid)
            {
                var totalPrice = this.GetTotalPrice(priceEstimationModel);
                return Ok(new { TotalPrice = totalPrice });
            }

            return BadRequest();
        }

        [HttpPost]
        [Route(RouteConstants.PrintToScreen)]
        public IActionResult PrintToScreen([FromBody] PriceEstimationModel priceEstimationModel)
        {
            if(ModelState.IsValid)
            {
                var totalPrice = this.GetTotalPrice(priceEstimationModel);
                var calculatedPriceModel = this.GetCalculatedPriceModel(priceEstimationModel, totalPrice);
                return Ok(calculatedPriceModel);
            }

            return BadRequest();
        }

        [HttpPost]
        [Route(RouteConstants.PrintToFile)]
        public IActionResult PrintToFile([FromBody] PriceEstimationModel priceEstimationModel)
        {
            if (ModelState.IsValid)
            {
                var totalPrice = this.GetTotalPrice(priceEstimationModel);
                var calculatedPriceModel = this.GetCalculatedPriceModel(priceEstimationModel, totalPrice);
                var printService = _serviceResolver(PrintOptions.PrintToFile);
                printService.Print(calculatedPriceModel);
                return Ok("PrintToFile");
            }

            return BadRequest();
        }

        [HttpPost]
        [Route(RouteConstants.PrintToPaper)]
        public IActionResult PrintToPaper([FromBody] PriceEstimationModel priceEstimationModel)
        {
            if (ModelState.IsValid)
            {
                var totalPrice = this.GetTotalPrice(priceEstimationModel);
                var calculatedPriceModel = this.GetCalculatedPriceModel(priceEstimationModel, totalPrice);
                var printService = _serviceResolver(PrintOptions.PrintToPaper);
                printService.Print(calculatedPriceModel);
                return Ok();
            }

            return BadRequest();
        }

        private decimal GetTotalPrice(PriceEstimationModel priceEstimationModel)
        {
            eligibleForDiscount = Convert.ToBoolean(HttpContext.User.Claims.FirstOrDefault(x => x.Type == Constants.PrivilegedUserClaim)?.Value);
            return _goldPriceEstimation.CalculateTotalPrice(priceEstimationModel, eligibleForDiscount);
        }

        private CalculatedPriceModel GetCalculatedPriceModel(PriceEstimationModel priceEstimationModel, decimal totalPrice)
        {
            CalculatedPriceModel calculatedPriceModel = null;

            if(priceEstimationModel != null)
            {
                calculatedPriceModel = new CalculatedPriceModel
                {
                    GoldPrice = priceEstimationModel.GoldPrice,
                    Weight = priceEstimationModel.Weight,
                    Discount = eligibleForDiscount ? priceEstimationModel.Discount : null,
                    TotalPrice = totalPrice
                };
            }

            return calculatedPriceModel;
        }
    }
}
