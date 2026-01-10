using Microsoft.AspNetCore.Mvc;
using NZWalks.UI.Models;
using NZWalks.UI.Models.DTO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NZWalks.UI.Controllers
{
    public class RegionsController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public RegionsController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<RegionDto> response = new List<RegionDto>();

            try
            {
                // Get the list of regions from the API
                var client = httpClientFactory.CreateClient();  // Create HttpClient instance

                var httpResponseMessage = await client.GetAsync("https://localhost:7133/api/regions");

                httpResponseMessage.EnsureSuccessStatusCode();

                ////var stringResponseBody = await httpResponseMessage.Content.ReadAsStringAsync();
                response.AddRange(await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<RegionDto>>());
            }
            catch (Exception)
            {

                throw;
            }

            return View(response);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddRegionViewModel createRegionViewModel)
        {
            try
            {
                var client = httpClientFactory.CreateClient();

                var httpResonseMessage = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://localhost:7133/api/regions"),
                    Content = new StringContent(JsonSerializer.Serialize(createRegionViewModel), Encoding.UTF8, "application/json")
                };

                var httpResponeMessage = await client.SendAsync(httpResonseMessage);
                httpResponeMessage.EnsureSuccessStatusCode();

                var response = await httpResponeMessage.Content.ReadFromJsonAsync<RegionDto>();

                if (response is not null)
                {
                    return RedirectToAction("Index", "Regions");
                }

                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            ////ViewBag.Id = id;

            var client = httpClientFactory.CreateClient();

            var response = await client.GetFromJsonAsync<RegionDto>($"https://localhost:7133/api/regions/{id.ToString()}");

            if (response is not null)
            {
                return View(response);
            }

            return View(null);
        }


    }
}