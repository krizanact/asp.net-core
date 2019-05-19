using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace ConsoleApp1
{
    class Program
    {
        
        static  void Main(string[] args)
        {

             ParseHtml().GetAwaiter().GetResult();   // GetAwaiter().GetResult(), console ceka da se funkcija ParseHtml() izvrsi do kraja

            //Console.ReadLine();
        }

        public static async Task  ParseHtml()
        {
            //ovaj url obuhvaca kateogriju sportska oprema i ucitavam s njega 200 itema (ipg=200&)
            var url = "https://www.ebay.com/sch/i.html?_sacat=0&_udlo=&_udhi=&_ftrt=901&_ftrv=1&_sabdlo=&_sabdhi=&_samilow=&_samihi=&_sadis=15&_stpos=&_sop=12&_dmd=1&_ipg=200&_fosrp=1&_nkw=sport+equipment&_blrs=spell_check"; ;
            var httpclient = new HttpClient();
            var html = await httpclient.GetStringAsync(url);

            var htmldocument = new HtmlDocument();
            htmldocument.LoadHtml(html);

            //spremiti iteme sa tagom "ul" i value "id" (npr ul id="ListViewInner")
            var htmlItems = htmldocument.DocumentNode.Descendants("ul")
                .Where(node => node.GetAttributeValue("id", "")
                .Equals("ListViewInner")).ToList();

            //itemi sa tagom "li" i value "id" koji sadrze u sebi "item"( li id="item...")
            var items = htmlItems[0].Descendants("li")
                .Where(node => node.GetAttributeValue("id","")
                .Contains("item")).ToList();
           
            
                 // foreach za prikaz nekih bitnih podataka itema u console
                 foreach (var item in items)
                {

                    Console.Write("Item ID: ");
                    Console.WriteLine(item.GetAttributeValue("listingid", ""));

                    Console.Write("Naziv: ");
                    Console.WriteLine(item.Descendants("h3")
                        .Where(node => node.GetAttributeValue("class","")
                        .Equals("lvtitle")).FirstOrDefault().InnerText.Trim('\n', '\r', '\t'));

                    Console.Write("Cijena: ");
                Console.WriteLine(Regex.Match(
                    item.Descendants("li")
                    .Where(node => node.GetAttributeValue("class", "")
                    .Equals("lvprice prc")).FirstOrDefault().InnerText.Trim('\n', '\r', '\t')
                    , @"\d+.\d+"));

                    Console.Write("Link: ");
                    Console.WriteLine(item.Descendants("a").FirstOrDefault().GetAttributeValue("href", ""));

                    Console.WriteLine("-------------------------------");
                        
                }

            string x;  //deklaracija x kao inputa koji cemo koristii za unos ebay ID

            do
            {
                Console.Write("Unesite neki eBay ID: ");
                 x = Console.ReadLine();
                var brojac = 0;


                foreach (var item in items)
                {
                    var id = item.GetAttributeValue("listingid", "");

                    var cijena = Regex.Match(
                            item.Descendants("li")
                            .Where(node => node.GetAttributeValue("class", "")
                            .Equals("lvprice prc")).FirstOrDefault().InnerText.Trim('\n', '\r', '\t')
                            , @"\d+.\d+");


                    if (id == x)
                    {
                        Console.WriteLine("Uneseni ID postoji na listi i cijena tog prozivoda je: " + cijena +" $");
                        brojac++;
                    }

                }

                if (brojac == 0)
                {
                    Console.WriteLine("ID koji ste unjeli ne postoji! ");
                }
            } while (x != "exit");   /*implementacija do while kako bi mogli pretrazivati ID vise puta,
                                     ,a upisom "exit" se zavrsava program tj zatvara console  */



        }
    }
}
