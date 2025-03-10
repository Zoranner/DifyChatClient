using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using DifyChatClient.Models;

namespace DifyChatClient
{
    /// <summary>
    /// Dify 聊天客户端
    /// </summary>
    public class ChatClient : IDisposable
    {
        private readonly HttpClient _HttpClient;
        private readonly string _BaseUrl;
        private bool _Disposed;

        /// <summary>
        /// 初始化 Dify 客户端
        /// </summary>
        /// <param name="apiKey">API密钥</param>
        /// <param name="baseUrl">基础URL，默认为 https://api.dify.ai/v1 </param>
        public ChatClient(string apiKey, string baseUrl = "https://api.dify.ai/v1")
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentNullException(nameof(apiKey));
            }

            _BaseUrl = baseUrl?.TrimEnd('/') ?? throw new ArgumentNullException(nameof(baseUrl));

            _HttpClient = new HttpClient();
            _HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                apiKey
            );
        }

        /// <summary>
        /// 发送聊天消息（阻塞模式）
        /// </summary>
        /// <param name="request">聊天消息请求</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>聊天完成响应</returns>
        public async Task<ChatCompletionResponse> SendMessageAsync(
            ChatMessageRequest request,
            CancellationToken cancellationToken = default
        )
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.ResponseMode = "blocking";

            var response = await _HttpClient.PostAsJsonAsync(
                $"{_BaseUrl}/chat-messages",
                request,
                cancellationToken
            );
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<ChatCompletionResponse>(
                cancellationToken: cancellationToken
            );
        }

        /// <summary>
        /// 发送聊天消息（流式模式）
        /// </summary>
        /// <param name="request">聊天消息请求</param>
        /// <param name="onMessage">消息回调</param>
        /// <param name="onError">错误回调</param>
        /// <param name="cancellationToken">取消令牌</param>
        public async Task SendMessageStreamAsync(
            ChatMessageRequest request,
            Action<MessageStreamResponse> onMessage,
            Action<ErrorStreamResponse> onError = null,
            CancellationToken cancellationToken = default
        )
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (onMessage == null)
            {
                throw new ArgumentNullException(nameof(onMessage));
            }

            request.ResponseMode = "streaming";

            using var response = await _HttpClient.PostAsJsonAsync(
                $"{_BaseUrl}/chat-messages",
                request,
                cancellationToken
            );
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new System.IO.StreamReader(stream);

            while (!reader.EndOfStream)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                var line = await reader.ReadLineAsync();
                if (string.IsNullOrEmpty(line) || !line.StartsWith("data: "))
                {
                    continue;
                }

                var json = line[6..];
                try
                {
                    var baseResponse = JsonSerializer.Deserialize<StreamResponseBase>(json);
                    switch (baseResponse.Event)
                    {
                        case "message":
                            var messageResponse = JsonSerializer.Deserialize<MessageStreamResponse>(
                                json
                            );
                            onMessage(messageResponse);
                            break;
                        case "error":
                            var errorResponse = JsonSerializer.Deserialize<ErrorStreamResponse>(
                                json
                            );
                            onError?.Invoke(errorResponse);
                            return;
                        case "message_end":
                            return;
                    }
                }
                catch (JsonException ex)
                {
                    onError?.Invoke(
                        new ErrorStreamResponse
                        {
                            Status = 500,
                            Code = "json_parse_error",
                            Message = ex.Message,
                        }
                    );
                    return;
                }
            }
        }

        /// <summary>
        /// 停止响应
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <param name="user">用户标识</param>
        /// <param name="cancellationToken">取消令牌</param>
        public async Task StopResponseAsync(
            string taskId,
            string user,
            CancellationToken cancellationToken = default
        )
        {
            if (string.IsNullOrEmpty(taskId))
            {
                throw new ArgumentNullException(nameof(taskId));
            }

            if (string.IsNullOrEmpty(user))
            {
                throw new ArgumentNullException(nameof(user));
            }

            var response = await _HttpClient.PostAsJsonAsync(
                $"{_BaseUrl}/chat-messages/{taskId}/stop",
                new { user },
                cancellationToken
            );

            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">是否正在释放</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_Disposed)
            {
                if (disposing)
                {
                    _HttpClient?.Dispose();
                }

                _Disposed = true;
            }
        }
    }
}
