using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.Repositories
{
    public class DataRepository : IDataRepository
    {
        // private static ConcurrentDictionary<string, Data> items = new ConcurrentDictionary<string, Data>();
        public static List<Data> _stdInfo;

        
        public IEnumerable<Data> GetAll()
        {
            return _stdInfo;
        }

       
        public async Task InitializeData()
        {

            //ovaj url obuhvaca kateogriju sportska oprema i ucitavam s njega 200 itema (ipg=200&)
            var url = "https://www.ebay.com/sch/i.html?_sacat=0&_udlo=&_udhi=&_ftrt=901&_ftrv=1&_sabdlo=&_sabdhi=&_samilow=&_samihi=&_sadis=15&_stpos=&_sop=12&_dmd=1&_fosrp=1&_blrs=spell_check&_nkw=sport+equipment&_ipg=200&rt=nc"; 
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
                .Where(node => node.GetAttributeValue("id", "")
                .Contains("item")).ToList();




            _stdInfo = new List<Data>();
            
            for(var i=0; i<items.Count(); i++)
            {
                long itemid = Convert.ToInt64(items[i].GetAttributeValue("listingid", ""));

                string priceid = Regex.Match(
                    items[i].Descendants("li")
                    .Where(node => node.GetAttributeValue("class", "")
                    .Equals("lvprice prc")).FirstOrDefault().InnerText.Trim('\n', '\r', '\t')
                    , @"\d+.\d+").ToString();

                var studentInfo1 = new Data
                {
                    id = itemid,
                    price = priceid
                    


                };
                _stdInfo.Add(studentInfo1 );
            }
            

        }
    }

}
