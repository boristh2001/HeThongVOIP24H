using ApiMVC.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace ApiMVC.Controllers
{
    public class CallHistoryController : Controller
    {
        // GET: CallHistory
        public async Task<ActionResult> Index()
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync("http://dial.voip24h.vn/dial/history?voip=76af0a0d5f8445fa649525123d713c6bc2b2f9b8&secret=1366b46c23edb28f61aeae42fd571e00");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var jsonObject = JsonConvert.DeserializeObject<ApiResult>(json);
                    var jsonData = jsonObject.Result.Data;
                    using (var db = new CallDbContext())
                    {
                        var lastestCallDate = db.CallsHistory.OrderBy(x => x.CallDate).Select(x => x.CallDate).FirstOrDefault();  
                        foreach (var item in jsonData)
                        {
                            var entity = new CallHistory
                            {
                                Id = item.Id,
                                CallDate = item.CallDate,
                                CallId = item.CallId,
                                Recording = item.Recording,
                                Play = item.Play,
                                Eplay = item.Eplay,
                                Download = item.Download,
                                Did = item.Did,
                                Src = item.Src,
                                Dst = item.Dst,
                                Status = item.Status,
                                Note = item.Note,
                                Disposition = item.Disposition,
                                LastApp = item.LastApp,
                                BillSec = item.BillSec,
                                Duration = item.Duration,
                                Type = item.Type,
                                Duration_Minutes = item.Duration_Minutes,
                                Duration_Seconds = item.Duration_Seconds
                            };
                           

                            // Kiểm tra xem lịch sử cuộc gọi có tồn tại trong database hay không dựa theo callid
                            var existingData = db.CallsHistory.FirstOrDefault(x => x.CallId == entity.CallId);

                            // Nếu không tồn tại thì thêm mới
                            if (existingData == null && entity.CallDate > lastestCallDate)
                            {
                                db.CallsHistory.Add(entity);
                            }
                        }
                        await db.SaveChangesAsync();
                        return View();
                    }
                }
                else
                {
                    return View("Error");
                }
            }   
        }

        public async Task<ActionResult> GetHistoryCall()
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync("https://dial.voip24h.vn/dial/history?voip=76af0a0d5f8445fa649525123d713c6bc2b2f9b8&secret=1366b46c23edb28f61aeae42fd571e00");
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResult>(responseContent);
                    var apiData = apiResponse.Result.Data;
                    return View(apiData);
                }
                else
                {
                    return View("Error");
                }
            }
        }
    }
}
