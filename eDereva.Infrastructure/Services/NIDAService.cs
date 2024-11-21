using System.Net.Http.Headers;
using System.Text;
using eDereva.Core.Contracts.Responses;
using eDereva.Core.Services;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace eDereva.Infrastructure.Services;

public class NIDAService(HttpClient httpClient, HybridCache hybridCache, ILogger<NIDAService> logger) : INIDAService
{
    private const string BaseUrl = "https://ors.brela.go.tz/um/load/load_nida/{0}";

    public async Task<UserDto?> LoadUserDataAsync(string NIN)
    {
        var cacheKey = NIN;

        return await hybridCache.GetOrCreateAsync<UserDto>(cacheKey, async entry =>
        {
            try
            {
                var requestUrl = string.Format(BaseUrl, NIN);

                using var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUrl)
                {
                    Content = new StringContent(string.Empty, Encoding.UTF8, "application/json")
                };

                requestMessage.Headers.Accept.Clear();
                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await httpClient.SendAsync(requestMessage, entry);

                if (!response.IsSuccessStatusCode)
                {
                    logger.LogError(
                        "Failed to retrieve user data from NIDA API. Status code: {StatusCode}, Reason: {ReasonPhrase}",
                        response.StatusCode, response.ReasonPhrase);
                    return null!;
                }

                var responseContent = await response.Content.ReadAsStringAsync(entry);
                var responseJson = JsonConvert.DeserializeObject<dynamic>(responseContent);

                if (responseJson?.obj?.result is not null)
                {
                    var userDataJson = responseJson.obj.result.ToString();
                    var userData = JsonConvert.DeserializeObject<UserDto>(userDataJson);

                    logger.LogInformation("Successfully retrieved user data for NIN: {NIN}", NIN);
                    return userData;
                }

                var error = responseJson?.obj?.error ?? "Unknown error";
                logger.LogInformation("No result found in NIDA response for NIN: {NIN}. Error: {Error}", NIN,
                    (string)error);
                return null!;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while loading user data for NIN: {NIN}", NIN);
                return null!;
            }
        });
    }
}