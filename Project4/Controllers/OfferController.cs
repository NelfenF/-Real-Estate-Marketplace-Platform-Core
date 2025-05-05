﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Project4.Models;
using Project4.Models.ViewModels;
using System.Net;
using System.Reflection;
using System.Text;

namespace Project4.Controllers
{
	//Handles Offer Create and Manage
	public class OfferController : Controller
	{
		[HttpPost]

		public IActionResult MakeOffer(int homeID)
		{
			if (homeID > 0)
			{
				string apiUrl = $"https://cis-iis2.temple.edu/Fall2024/CIS3342_tui78495/WebAPI/ReadHome/ReadSingleHomeListing/{homeID}";
				WebRequest request = WebRequest.Create(apiUrl);
				WebResponse resposne = request.GetResponse();
				Stream dataStream = resposne.GetResponseStream();
				StreamReader reader = new StreamReader(dataStream);
				string data = reader.ReadToEnd();
				reader.Close();
				resposne.Close();
				Home currentHome = JsonConvert.DeserializeObject<Home>(data);
				string seralizedHome = JsonConvert.SerializeObject(currentHome);
				HttpContext.Session.SetString("CurrentHome", seralizedHome);
				if (HttpContext.Session.GetString("OfferContingencies") == null)
				{
					List<string> currentContingencies = new List<string>();
					string seralizedContingencies = JsonConvert.SerializeObject(currentContingencies);
					HttpContext.Session.SetString("OfferContingencies", seralizedContingencies);
					ViewBag.OfferContingencies = currentContingencies;
					return View();
				}
				else
				{
					string seralizedContingencies = HttpContext.Session.GetString("OfferContingencies");
					List<string> currentContingencies = JsonConvert.DeserializeObject<List<string>>(seralizedContingencies);
					ViewBag.OfferContingencies = currentContingencies;
					return View();
				}
			}
			else
			{
				return RedirectToAction("Dashboard", "Dashboard");
			}

		}
		[HttpPost]
		public IActionResult AddContingency(string newContingency)
		{
			ViewBag.FirstName = Request.Form["FirstName"];
			ViewBag.LastName = Request.Form["LastName"];
			ViewBag.Email = Request.Form["Email"];
			ViewBag.Phone = Request.Form["Phone"];
			ViewBag.ClientAddress = Request.Form["clientAddress"];
			ViewBag.ClientCity = Request.Form["clientCity"];
			ViewBag.ClientZip = Request.Form["clientZip"];
			ViewBag.OfferAmount = Request.Form["OfferAmount"];
			ViewBag.MoveInDate = Request.Form["MoveInDate"];
			if (newContingency == "")
			{
				ViewBag.OfferError = "Contingency cannot be empty.";
			}
			else
			{
				string seralizedContingencies = HttpContext.Session.GetString("OfferContingencies");
				List<string> currentContingencies = JsonConvert.DeserializeObject<List<string>>(seralizedContingencies);
				currentContingencies.Add(newContingency);
				seralizedContingencies = JsonConvert.SerializeObject(currentContingencies);
				HttpContext.Session.SetString("OfferContingencies", seralizedContingencies);
				ViewBag.OfferContingencies = currentContingencies;
			}
			return View("MakeOffer");

		}
		[HttpPost]
		public IActionResult RemoveContingency(string removedContingency)
		{
			ViewBag.FirstName = Request.Form["FirstName"];
			ViewBag.LastName = Request.Form["LastName"];
			ViewBag.Email = Request.Form["Email"];
			ViewBag.Phone = Request.Form["Phone"];
			ViewBag.ClientAddress = Request.Form["clientAddress"];
			ViewBag.ClientCity = Request.Form["clientCity"];
			ViewBag.ClientZip = Request.Form["clientZip"];
			ViewBag.OfferAmount = Request.Form["OfferAmount"];
			ViewBag.MoveInDate = Request.Form["MoveInDate"];
			string seralizedContingencies = HttpContext.Session.GetString("OfferContingencies");
			List<string> currentContingencies = JsonConvert.DeserializeObject<List<string>>(seralizedContingencies);

			if (currentContingencies.Contains(removedContingency))
			{
				currentContingencies.Remove(removedContingency);
			}
			seralizedContingencies = JsonConvert.SerializeObject(currentContingencies);
			HttpContext.Session.SetString("OfferContingencies", seralizedContingencies);
			ViewBag.OfferContingencies = currentContingencies;

			return View("MakeOffer");
		}

		public IActionResult AllOffers()
		{
			if (HttpContext.Session.GetString("Agent") == null) { return RedirectToAction("Dashboard", "Dashboard"); }
			string agentSession = HttpContext.Session.GetString("Agent");
			Agent currentAgent = JsonConvert.DeserializeObject<Agent>(agentSession);
			ViewBag.Agent = currentAgent;
			return View("AllOffers");
		}


		public IActionResult FinalizeOffer()
		{
			string firstName = Request.Form["FirstName"];
			string lastName = Request.Form["LastName"];
			string email = Request.Form["Email"];
			string phone = Request.Form["Phone"];
			string offerAmount = Request.Form["OfferAmount"];
			string saleType = Request.Form["SaleType"];
			string sellHome = Request.Form["SellHomePrior"];
			string moveInDate = Request.Form["MoveInDate"];
			string clientAddress = Request.Form["clientAddress"];
			string clientCity = Request.Form["clientCity"];
			string clientState = Request.Form["clientState"];
			string clientZip = Request.Form["clientZip"];
			List<string> inputs = new List<string>();
			inputs.Add(firstName);
			inputs.Add(lastName);
			inputs.Add(email);
			inputs.Add(phone);
			inputs.Add(offerAmount);
			inputs.Add(sellHome);
			inputs.Add(moveInDate);
			inputs.Add(clientAddress);
			inputs.Add(clientCity);
			inputs.Add(clientZip);

			if (ValidateOffer(inputs) == false)
			{
				ViewBag.OfferError = "Please fix errors below and resubmit the offer!";
				ViewBag.FirstName = firstName;
				ViewBag.LastName = lastName;
				ViewBag.Email = email;
				ViewBag.Phone = phone;
				ViewBag.OfferAmount = offerAmount;
				ViewBag.MoveInDate = moveInDate;
				ViewBag.ClientAddress = clientAddress;
				ViewBag.ClientCity = clientCity;
				ViewBag.ClientZip = clientZip;
				return View("MakeOffer");
			}
			else
			{
				string serializedHome = HttpContext.Session.GetString("CurrentHome");
				Home home = JsonConvert.DeserializeObject<Home>(serializedHome);
				States stateEnum = Enum.Parse<States>(clientState);
				Address newAddress = new Address(clientAddress, clientCity, stateEnum, clientZip);
				Client newClient = new Client(firstName, lastName, newAddress, phone, email);
				int clientID = WriteClient.CreateNew(newClient);
				Client actualClient = ReadClients.GetClientByLastNameAndAddress(newClient);
				Offer newOffer = new Offer(home, actualClient, int.Parse(offerAmount), Enum.Parse<TypeOfSale>(saleType), bool.Parse(sellHome), DateTime.Parse(moveInDate), OfferStatus.Pending);

				int offerID = WriteOffer.CreateNew(newOffer);
				Offer actualOffer = ReadOffers.GetOfferByHomeClientAmount(home, actualClient, int.Parse(offerAmount));

				string seralizedContingencies = HttpContext.Session.GetString("OfferContingencies");
				List<string> currentContingencies = JsonConvert.DeserializeObject<List<string>>(seralizedContingencies);

				Contingencies newContingencies = new Contingencies();
				foreach (string contingency in currentContingencies)
				{
					newContingencies.Add(new Contingency(actualOffer.OfferID, contingency));
				}
				WriteContingencies.CreateNew(newContingencies);




				List<string> confirmationMessage = new List<string>();
				confirmationMessage.Add("Congraulations! Your offer was sucessfully placed!");
				confirmationMessage.Add("Offer First Name: " + actualOffer.Client.FirstName);
				confirmationMessage.Add("Offer Last Name: " + actualOffer.Client.LastName);
				confirmationMessage.Add("Offer Home Address: " + actualOffer.Home.Address.ToString());
				confirmationMessage.Add("Offer Amount: " + actualOffer.Amount);
				TempData["Message"] = JsonConvert.SerializeObject(confirmationMessage);
				TempData["Action"] = "Dashboard";
				TempData["Controller"] = "Dashboard";

				HttpContext.Session.Remove("OfferContingencies");
				HttpContext.Session.Remove("CurrentHome");

				return RedirectToAction("SharedConfirmation", "Shared");
			}


		}

		private bool ValidateOffer(List<string> inputs)
		{
			bool isValid = true;
			if (string.IsNullOrEmpty(inputs[0]))
			{
				isValid = false;
				ViewBag.FnameError = "First Name Is Requried!";
			}
			if (string.IsNullOrEmpty(inputs[1]))
			{
				isValid = false;
				ViewBag.LnameError = "Last Name Is Requried!";
			}
			if (string.IsNullOrEmpty(inputs[2]))
			{
				isValid = false;
				ViewBag.EmailError = "Email Is Requried!";
			}
			if (string.IsNullOrEmpty(inputs[3]))
			{
				isValid = false;
				ViewBag.PhoneError = "Phone Number Is Requried!";
			}
			if (string.IsNullOrEmpty(inputs[4]) || int.TryParse(inputs[4], out _) == false)
			{
				isValid = false;
				ViewBag.AmountError = "Offer Amount Is Requried!";
			}
			if (string.IsNullOrEmpty(inputs[5]))
			{
				isValid = false;
				ViewBag.HomeError = "Current Home Sale Is Requried!";
			}
			if (string.IsNullOrEmpty(inputs[6]) || DateTime.TryParse(inputs[6], out _) == false)
			{
				isValid = false;
				ViewBag.DateError = "Move In Date Is Requried!";
			}
			if (string.IsNullOrEmpty(inputs[7]))
			{
				isValid = false;
				ViewBag.StreetError = "Street Address Sale Is Requried!";
			}
			if (string.IsNullOrEmpty(inputs[8]))
			{
				isValid = false;
				ViewBag.CityError = "City Is Requried!";
			}
			if (string.IsNullOrEmpty(inputs[9]))
			{
				isValid = false;
				ViewBag.ZipError = "Zip CodeIs Requried!";
			}


			return isValid;
		}
		public async Task<IActionResult> Confirmation()
		{
			return View("Confirmation");

		}

		public IActionResult AcceptOffer(int offerID)
		{
			WriteOffer.UpdateOfferStatus(offerID, OfferStatus.Accepted);
			string agentSession = HttpContext.Session.GetString("Agent");
			Agent currentAgent = JsonConvert.DeserializeObject<Agent>(agentSession);
			ViewBag.Agent = currentAgent;

			Offer currentOffer = ReadOffers.GetOfferByOfferID(offerID);


			EmailInfo info = new EmailInfo(
				currentOffer.Client.Email,
				"tur31103@temple.edu",
				"Offer Accepted!",
				$"Your offer for the property {currentOffer.Home.Address.ToString()} has been accepted for the amount of {currentOffer.Amount.ToString("C")}!");

			//Call the Email API and send the email
			StringContent content = new StringContent(System.Text.Json.JsonSerializer.Serialize(info), Encoding.UTF8, "application/json");
			using (HttpClient httpClient = new HttpClient())
			{
				try
				{
					HttpResponseMessage response = httpClient.PostAsync("https://cis-iis2.temple.edu/Fall2024/CIS3342_tui78495/WebAPITest/Email/SendToTempleEmail", content).GetAwaiter().GetResult();
					string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
					Console.WriteLine($"Response Body: {responseBody}");
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
			}

			return View("AllOffers");
		}

		public IActionResult DenyOffer(int offerID)
		{
			WriteOffer.UpdateOfferStatus(offerID, OfferStatus.Rejected);
			string agentSession = HttpContext.Session.GetString("Agent");
			Agent currentAgent = JsonConvert.DeserializeObject<Agent>(agentSession);
			ViewBag.Agent = currentAgent;
			Offer currentOffer = ReadOffers.GetOfferByOfferID(offerID);
			EmailInfo info = new EmailInfo(
				currentOffer.Client.Email,
				"tur31103@temple.edu",
				"Offer Rejected!",
				$"Your offer for the property {currentOffer.Home.Address.ToString()} has been rejected! Please feel free to submit another offer!");

			//Call the Email API and send the email
			StringContent content = new StringContent(System.Text.Json.JsonSerializer.Serialize(info), Encoding.UTF8, "application/json");
			using (HttpClient httpClient = new HttpClient())
			{
				try
				{
					HttpResponseMessage response = httpClient.PostAsync("https://cis-iis2.temple.edu/Fall2024/CIS3342_tui78495/WebAPITest/Email/SendToTempleEmail", content).GetAwaiter().GetResult();
					string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
					Console.WriteLine($"Response Body: {responseBody}");
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
			}
			return View("AllOffers");
		}




	}
}
