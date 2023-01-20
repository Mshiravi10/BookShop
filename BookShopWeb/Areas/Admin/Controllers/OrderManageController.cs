using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models;
using BookShop.Models.ViewModels;
using BookShop.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookShopWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize]
	public class OrderManageController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderVM orderView { get; set; }

        public OrderManageController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Details(int orderId)
		{
			orderView = new OrderVM()
			{
                OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderId == orderId, includeProperties: "Product"),
            };
			return View(orderView);
		}

		[HttpPost]
		[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
		[ValidateAntiForgeryToken]
		public IActionResult UpdateOrderDetail()
		{
			var orderHeaderFromDb = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderView.OrderHeader.Id, tracked: false);
			orderHeaderFromDb.Name = orderView.OrderHeader.Name;
			orderHeaderFromDb.PhoneNumber = orderView.OrderHeader.PhoneNumber;
			orderHeaderFromDb.StreetAddress = orderView.OrderHeader.StreetAddress;
			orderHeaderFromDb.City = orderView.OrderHeader.City;
			orderHeaderFromDb.State = orderView.OrderHeader.State;
			orderHeaderFromDb.PostalCode = orderView.OrderHeader.PostalCode;
			if (orderView.OrderHeader.Carrier != null)
			{
				orderHeaderFromDb.Carrier = orderView.OrderHeader.Carrier;
			}
			if (orderView.OrderHeader.TrackingNumber != null)
			{
				orderHeaderFromDb.TrackingNumber = orderView.OrderHeader.TrackingNumber;
			}
			_unitOfWork.OrderHeader.Update(orderHeaderFromDb);
			_unitOfWork.Save();
			return RedirectToAction("Details", "OrderManage", new { orderId = orderHeaderFromDb.Id });
		}


		#region API CALLS
		[HttpGet]
		public IActionResult GetAll()
		{
            IEnumerable<OrderHeader> orderHeaders;
            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                orderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                orderHeaders = _unitOfWork.OrderHeader.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "ApplicationUser");
            }
            return Json(new { data = orderHeaders });
		}
		#endregion
	}
}
