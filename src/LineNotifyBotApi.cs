using LexLibrary.Line.NotifyBot.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace LexLibrary.Line.NotifyBot
{
    /// <summary>
    /// LineNotifyBotApi
    /// </summary>
    public class LineNotifyBotApi
    {
        private readonly LineNotifyBotSetting _setting = null;
        private readonly HttpClient _httpClient = null;
        private readonly ILogger _logger = null;

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="httpClientFactory"></param>
        /// <param name="logger"></param>
        public LineNotifyBotApi(
            LineNotifyBotSetting setting,
            IHttpClientFactory httpClientFactory,
            ILogger<LineNotifyBotApi> logger)
        {
            _setting = setting;
            _httpClient = httpClientFactory.CreateClient(nameof(LineNotifyBotApi));
            _logger = logger;
        }

        /// <summary>
        /// 通知使用者
        /// </summary>
        /// <param name="requestDTO"></param>
        /// <returns></returns>
        public async Task<NotifyResponseDTO> Notify(NotifyRequestDTO requestDTO)
        {
            var query = new Dictionary<string, string>();

            query.Add("message", Environment.NewLine + Environment.NewLine + requestDTO.Message);

            var result = await executeApi<NotifyResponseDTO>(HttpMethod.Post, _setting.NotifyApi, requestDTO.AccessToken, query);

            return result;
        }

        /// <summary>
        /// 查詢 Token 狀態
        /// </summary>
        /// <param name="requestDTO"></param>
        /// <returns></returns>
        public async Task<StatusResponseDTO> Status(StatusRequestDTO requestDTO)
        {
            var result = await executeApi<StatusResponseDTO>(HttpMethod.Get, _setting.StatusApi, requestDTO.AccessToken);

            return result;
        }

        /// <summary>
        /// 撤回 Token
        /// </summary>
        /// <param name="requestDTO"></param>
        /// <returns></returns>
        public async Task<RevokeResponseDTO> Revoke(RevokeRequestDTO requestDTO)
        {
            var result = await executeApi<RevokeResponseDTO>(HttpMethod.Post, _setting.RevokeApi, requestDTO.AccessToken);

            return result;
        }

        /// <summary>
        /// 取得 Token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 產生 Authorize Url
        /// </summary>
        /// <param name="callbackUrl"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public string GenerateAuthorizeUrl(string callbackUrl, string state)
        {
            var query = new Dictionary<string, string>();

            query.Add("response_type", "code");
            query.Add("client_id", _setting.ClientID);
            query.Add("redirect_uri", callbackUrl);
            query.Add("scope", "notify");
            query.Add("state", state);

            var list = query.Select(x => string.Format("{0}={1}", x.Key, HttpUtility.UrlEncode(x.Value)));

            return $"{_setting.AuthorizeApi}?{string.Join("&", list)}";
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
            var headers = httpResponseMessage.Headers?.ToDictionary(x => x.Key, x => x.Value);

            _logger.LogInformation(JsonConvert.SerializeObject(new
            {
                Guid = guid,
                Headers = headers,
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
                var model = JsonConvert.DeserializeObject<T>(stringResult);
                model.Headers = headers;

                return model;
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
