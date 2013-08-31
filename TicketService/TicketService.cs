using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace TicketService
{
    public class TicketManager
    {
        public List<Ticket> GetTickets(String plate_number, String plate_state)
        {
            List<Ticket> tickets = new List<Ticket>();

            string citation_search_url = @"https://prodpci.etimspayments.com/pbw/include/sanfrancisco/input.jsp";
            string form_action_url = @"https://prodpci.etimspayments.com/pbw/inputAction.doh";

            WebClient client = new WebClient();

            var form_variables = new NameValueCollection
            {
                {"clientcode", "19"},
                {"requestType", "submit"},
                {"requestCount", "1"},
                {"clientAccount", "5"},
                {"ticketNumber", ""},
                {"plateNumber", plate_number},
                {"statePlate", plate_state}, //CA
                {"submit", "Search for citations"}

            };

            var result = client.UploadValues(form_action_url, form_variables);
            String result_str = Encoding.ASCII.GetString(result);

            if (result_str.Contains("Plate is not found"))
            {
                return tickets;
            }


            string table_header = @"<table width=""706"" border=""0"" cellspacing=""2"" cellpadding=""2"">";
            int location = result_str.IndexOf(table_header);
            if( location < 0 )
                throw new Exception("failed to find start of table.");


            string table_data = result_str.Substring(location);

            location = table_data.IndexOf("</table>");

            if (location < 0)
                throw new Exception("failed to find end of table.");

            location += "</table>".Length;

            table_data = table_data.Substring(0, location);


            HtmlDocument doc = new HtmlDocument();

            doc.LoadHtml(table_data);

            //var all_nodes = doc.DocumentNode.SelectNodes("//node()[not(node())]");

            var citation_number_nodes = doc.DocumentNode.SelectNodes("/table[1]/tr[position() > 1]/td[2]/font[1]");

            var date_nodes = doc.DocumentNode.SelectNodes("/table[1]/tr[position() > 1]/td[3]/font[1]");

            var code_nodes = doc.DocumentNode.SelectNodes("/table[1]/tr[position() > 1]/td[4]/font[1]");

            var violation_nodes = doc.DocumentNode.SelectNodes("/table[1]/tr[position() > 1]/td[5]/font[1]");

            var amount_nodes = doc.DocumentNode.SelectNodes("/table[1]/tr[position() > 1]/td[6]/font[1]");

            int count = citation_number_nodes.Count();

            for (int i = 0; i < count; ++i)
            {
                Ticket ticket = new Ticket();

                ticket.CitationNumber = citation_number_nodes[i].InnerText;
                ticket.IssueDate = DateTime.Parse(date_nodes[i].InnerText);
                ticket.ViolationCode = code_nodes[i].InnerText;
                ticket.Violation = violation_nodes[i].InnerText;

                ticket.Amount = Decimal.Parse(amount_nodes[i].InnerText.Replace("$", ""));

                tickets.Add(ticket);
            }

            


            return tickets;
        }


    }

    public class Ticket
    {
        public String CitationNumber { get; set; }
        public DateTime IssueDate { get; set; }
        public String ViolationCode { get; set; }
        public String Violation { get; set; }
        public Decimal Amount { get; set; }

        public override string ToString()
        {
            return CitationNumber + " " + IssueDate.ToShortDateString() + " " + ViolationCode + " " + Violation + " " + Amount.ToString();
        }
    }


}
