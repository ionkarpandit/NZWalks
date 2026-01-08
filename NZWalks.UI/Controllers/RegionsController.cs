using Microsoft.AspNetCore.Mvc;
using NZWalks.UI.Models.DTO;
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
    }
}
