﻿using LexLibrary.Line.NotifyBot.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace LexLibrary.Line.NotifyBot
{
    public class LineNotifyBotApi
    {
        private readonly LineNotifyBotSetting _setting = null;
        private readonly HttpClient _httpClient = null;
        private readonly ILogger _logger = null;

        public LineNotifyBotApi(
            LineNotifyBotSetting setting,
            IHttpClientFactory httpClientFactory,
            ILogger<LineNotifyBotApi> logger)
        {
            _setting = setting;
            _httpClient = httpClientFactory.CreateClient(nameof(LineNotifyBotApi));
            _logger = logger;
        }

        public async Task<NotifyResponseDTO> Notify(NotifyRequestDTO requestDTO)
        {
            var query = new Dictionary<string, string>();

            query.Add("message", Environment.NewLine + Environment.NewLine + requestDTO.Message);

            var result = await executeApi<NotifyResponseDTO>(HttpMethod.Post, _setting.NotifyApi, requestDTO.AccessToken, query);

            return result;
        }

        public async Task<StatusResponseDTO> Status(StatusRequestDTO requestDTO)
        {
            var result = await executeApi<StatusResponseDTO>(HttpMethod.Get, _setting.StatusApi, requestDTO.AccessToken);

            return result;
        }

        public async Task<RevokeResponseDTO> Revoke(RevokeRequestDTO requestDTO)
        {
            var result = await executeApi<RevokeResponseDTO>(HttpMethod.Post, _setting.RevokeApi, requestDTO.AccessToken);

            return result;
        }

        public async Task<TokenResponseDTO> Token(TokenRequestDTO model)
        {
            var query = new Dictionary<string, string>();

            query.Add("grant_type", "authorization_code");
            query.Add("code", model.Code);
            query.Add("redirect_uri", model.RedirectUri);
            query.Add("client_id", _setting.ClientID);
            query.Add("client_secret", _setting.ClientSecret);

            var result = await executeApi<TokenResponseDTO>(HttpMethod.Post, _setting.TokenApi, query: query);

            return result;
        }

        private async Task<T> executeApi<T>(
            HttpMethod httpMethod,
            string url,
            string accessToken = null,
            IDictionary<string, string> query = null) where T : BaseResponseDTO
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(httpMethod, url);

            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            if (query != null)
            {
                httpRequestMessage.Content = new FormUrlEncodedContent(query);
            }

            string guid = Guid.NewGuid().ToString();
            _logger.LogInformation(JsonConvert.SerializeObject(new
            {
                Guid = guid,
                Url = httpRequestMessage.RequestUri,
                httpRequestMessage.Method,
                httpRequestMessage.Headers,
                httpRequestMessage.Content,
                Query = query
            }));

            HttpResponseMessage httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage);

            string stringResult = await httpResponseMessage.Content.ReadAsStringAsync();

            _logger.LogInformation(JsonConvert.SerializeObject(new
            {
                Guid = guid,
                httpResponseMessage.Headers,
                Body = tryParseJson(stringResult, out JToken token) ? token : stringResult
            }));

            if (string.IsNullOrWhiteSpace(stringResult))
            {
                stringResult = JsonConvert.SerializeObject(new BaseResponseDTO
                {
                    Status = 999,
                    Message = "Line Notify 系統繁忙中，請稍後在試。"
                });
            }

            if (httpRequestMessage.Content != null)
            {
                httpRequestMessage.Content.Dispose();
            }
            httpRequestMessage.Dispose();
            httpResponseMessage.Dispose();

            try
            {
                return JsonConvert.DeserializeObject<T>(stringResult);
            }
            catch (Exception ex)
            {
                throw new JsonException($"反序列化失敗, json = {stringResult}", ex);
            }
        }

        private bool tryParseJson(string str, out JToken token)
        {
            try
            {
                token = JToken.Parse(str);
                return true;
            }
            catch
            {
                token = null;
                return false;
            }
        }
    }
}
