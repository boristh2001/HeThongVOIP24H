using ApiMVC.Models;
using ApiMVC.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Xml.Linq;

namespace ApiMVC.Controllers
{
    public class MakeCallController : Controller
    {
        public async Task<ActionResult> Index(string phone)
        {
            using(var client = new HttpClient())
            {
                var response = await client.GetAsync("https://dial.voip24h.vn/dial?voip=76af0a0d5f8445fa649525123d713c6bc2b2f9b8&secret=1366b46c23edb28f61aeae42fd571e00&sip=124&phone=" + phone);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var jsonObject = JObject.Parse(json);
                    var eventJson = jsonObject["events"].ToString();
                    var events = JsonConvert.DeserializeObject<List<Call>>(eventJson);

                    using (CallDbContext db = new CallDbContext())
                    {
                        foreach (var item in events)
                        {
                            var entity = new Call
                            {
                                Id = item.Id,
                                ActionId = item.ActionId,
                                Type = item.Type,
                                Extend = item.Extend,
                                Phone = item.Phone,
                                State = item.State,
                                CallId = item.CallId,
                                Channel = item.Channel,
                                ChannelStateDesc = item.ChannelStateDesc,
                                CallerIdName = item.CallerIdName,
                                CallerIdNum = item.CallerIdNum,
                                ConnectedLineName = item.ConnectedLineName,
                                ConnectedLineNum = item.ConnectedLineNum,
                                Exten = item.Exten,
                                UniqueId = item.UniqueId,
                                LinkedId = item.LinkedId
                            };
                            db.Calls.Add(entity);
                        }
                        await db.SaveChangesAsync();
                    }
                    return RedirectToAction("Call","CallApiMvc");
                }
                else
                {
                    return View("errors");
                }
            }
        }
    }
}
