using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicketService;

namespace QuickTicket.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index(String plate, string state, string email)
        {
            string nl = "\n<br />";
            ViewBag.Error = false;

            if (!String.IsNullOrWhiteSpace(plate) && !String.IsNullOrWhiteSpace(state))
            {
                if (state.Length > 2)
                    return View(new List<TicketService.Ticket>());
                if (plate.Length > 8)
                    return View(new List<TicketService.Ticket>());

                ViewBag.Plate = plate;
                ViewBag.State = state;

                List<Ticket> tickets = new List<Ticket>();
                TicketManager manager = new TicketManager();
                try
                {
                    tickets = manager.GetTickets(plate, state);
                }
                catch (Exception ex)
                {
                    ViewBag.Error = true;
                    return View(tickets);
                }

                if (tickets.Count() == 0)
                {
                    return Content("Congrats! No tickets!");
                }

                /*if (!String.IsNullOrWhiteSpace(email))
                {
                    ViewBag.Email = email;
                }*/



                return View(tickets);

            }

            return View(new List<TicketService.Ticket>());
        }

    }
}
