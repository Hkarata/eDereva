﻿using System.Net.Http.Headers;
using System.Text;
using eDereva.Core.Contracts.Requests;
using eDereva.Core.Services;
using eDereva.Core.ValueObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;

namespace eDereva.Infrastructure.Services
{
    public class SmsService : ISmsService, IDisposable
    {
        private readonly ILogger<SmsService> _logger;
        private readonly HttpClient _httpClient;
        private readonly SmsConfiguration _config;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
        private bool _disposed;

        public SmsService(
            ILogger<SmsService> logger,
            IOptions<SmsConfiguration> config,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config?.Value ?? throw new ArgumentNullException(nameof(config));
            _httpClient = httpClientFactory.CreateClient("SmsService");

            ConfigureHttpClient();
            _retryPolicy = CreateRetryPolicy();
        }

        private void ConfigureHttpClient()
        {
            var credentials = Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{_config.ApiKey}:{_config.SecretKey}"));

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", credentials);
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue(_config.ContentType));
            _httpClient.Timeout = TimeSpan.FromSeconds(_config.TimeoutSeconds);
        }

        private AsyncRetryPolicy<HttpResponseMessage> CreateRetryPolicy()
        {
            return Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .Or<TimeoutException>()
                .WaitAndRetryAsync(
                    _config.MaxRetries,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning(
                            exception.Exception,
                            "Retry attempt {RetryCount} of {MaxRetries} after {TimeSpan} seconds delay. Error: {ErrorMessage}",
                            retryCount,
                            _config.MaxRetries,
                            timeSpan.TotalSeconds,
                            exception.Exception.Message);
                    });
        }



        public async Task<bool> SendMessageAsync(Sms sms)
        {
            try
            {
                var requestBody = new
                {
                    source_addr = _config.SourceAddress,
                    schedule_time = "",
                    encoding = 0,
                    message = sms.message,
                    recipients = sms.recipients
                };

                var jsonRequestBody = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonRequestBody, Encoding.UTF8, _config.ContentType);

                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.PostAsync(_config.ApiUrl, content));

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("SMS sent successfully to recipients: {Recipients}",
                        JsonConvert.SerializeObject(sms.recipients));
                    return true;
                }

                _logger.LogError("Failed to send SMS. Status code: {StatusCode}",
                    response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending SMS");
                return false;
            }
        }

        public async Task<bool> SendBulkMessagesAsync(IEnumerable<Sms> messages)
        {
            try
            {
                var tasks = messages.Select(SendMessageAsync);
                var results = await Task.WhenAll(tasks);
                return results.All(r => r);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending bulk SMS messages");
                return false;
            }
        }

        public async Task<string> GetDeliveryStatusAsync(string messageId)
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    $"{_config.ApiUrl}/status/{messageId}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }

                _logger.LogError("Failed to get delivery status. Status code: {StatusCode}",
                    response.StatusCode);
                return null!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting delivery status");
                return null!;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _httpClient?.Dispose();
            }

            _disposed = true;
        }
    }
}