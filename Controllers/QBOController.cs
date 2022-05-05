using System;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using Intuit.Ipp.DataService;
using Intuit.Ipp.QueryFilter;
using Microsoft.AspNetCore.Mvc;
using OAuth2_CoreMVC_Sample.Helper;
using OAuth2_CoreMVC_Sample.Models;
using Square;
using Square.Models;
using Square.Exceptions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OAuth2_CoreMVC_Sample.Controllers
{
    public class QBOController : Controller
    {
        private readonly IServices _services;
        private string id;
        private static ISquareClient sqClient;
        private static IConfigurationRoot config;
        private decimal amount;

        public QBOController(IServices services)
        {
            _services = services;
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            var model = await SquarePayments();
            return View(model);
        }

        public async Task<IActionResult> CreateCustomer()
        {
            var apiCallFucntion = new Action<ServiceContext>(CreateNewCustomer);
            await _services.QBOApiCall(apiCallFucntion);
            return View("QBO");
        }

        //NS: Save to QBO
        public async Task<IActionResult> CreatePayment(string paymentAmount)
        {
            amount = Convert.ToDecimal(paymentAmount);
            var apiCallFucntion = new Action<ServiceContext>(CreateNewPayment);
            await _services.QBOApiCall(apiCallFucntion);

            ViewBag.Message = "Payment amount £" + amount.ToString() + " successfully saved to QBO";
            return View("QBO");
        }

        public async Task<IActionResult> CreateInvoice(string customerId)
        {
            id = customerId;
            var apiCallFucntion = new Action<ServiceContext>(CreateNewInvoice);
            await _services.QBOApiCall(apiCallFucntion);

            return View("QBO");
        }

        //NS: Load Payments from Square
        public async Task<ListPaymentsResponse> SquarePayments()
        {
            var builder = new ConfigurationBuilder().AddJsonFile($"appsettings.json", true, true);

            config = builder.Build();
            var accessToken = config["AppSettings:AccessToken"];
            sqClient = new SquareClient.Builder()
                .Environment(Square.Environment.Sandbox)
                .AccessToken(accessToken)
                .Build();

            ListPaymentsResponse sqResponse = await sqClient.PaymentsApi.ListPaymentsAsync();

            return sqResponse;
        }

        #region HelperMethods


        private void CreateNewCustomer(ServiceContext context)
        {
            var dataService = new DataService(context);
            var queryService = new QueryService<Intuit.Ipp.Data.Customer>(context);
            var customer = Inputs.CreateCustomer(dataService);
            ViewData["CustomerInfo"] = "Customer with ID:" + customer.Id + " created successfully";
            ViewData["CustomerId"] = customer.Id;
        }

        //NS: Create New Payment in QBO
        private void CreateNewPayment(ServiceContext context)
        {
            var customerService = new QueryService<Intuit.Ipp.Data.Customer>(context);
            var query = "Select * from Customer where Id='1'";
            var queryResponse = customerService.ExecuteIdsQuery(query).ToList();

            var dataService = new DataService(context);
            var queryService = new QueryService<Intuit.Ipp.Data.Payment>(context);
            var payment = Inputs.CreatePayment(dataService, queryResponse[0], amount);

            ViewData["PaymentInfo"] = "Payment Amount £" + amount + " saved to QBO successfully";
        }

        private void CreateNewInvoice(ServiceContext context)
        {
            var dataService = new DataService(context);
            var queryService = new QueryService<Account>(context);
            var customerService = new QueryService<Intuit.Ipp.Data.Customer>(context);
            var query = "Select * from Customer where Id='" + id + "'";
            var queryResponse = customerService.ExecuteIdsQuery(query).ToList();
            if (queryResponse != null)
            {
                var invoice = Inputs.CreateInvoice(dataService, queryService, queryResponse[0]);
                ViewData["InvoiceInfo"] = "Invoice with ID:" + invoice.Id + " created successfully";
            }
            else
            {
                ViewData["InvoiceInfo"] = "Invalid Customer information";
            }

            ViewData["CustomerId"] = id;
        }

        
        #endregion
    }
}