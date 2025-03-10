using System.Text.Json.Serialization;

namespace DifyChatClient.Models
{
    /// <summary>
    /// 流式响应基类
    /// </summary>
    public class StreamResponseBase
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        [JsonPropertyName("event")]
        public string Event { get; set; }

        /// <summary>
        /// 任务ID
        /// </summary>
        [JsonPropertyName("task_id")]
        public string TaskId { get; set; }

        /// <summary>
        /// 消息ID
        /// </summary>
        [JsonPropertyName("message_id")]
        public string MessageId { get; set; }

        /// <summary>
        /// 会话ID
        /// </summary>
        [JsonPropertyName("conversation_id")]
        public string ConversationId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonPropertyName("created_at")]
        public long CreatedAt { get; set; }
    }

    /// <summary>
    /// 消息流式响应
    /// </summary>
    public class MessageStreamResponse : StreamResponseBase
    {
        /// <summary>
        /// 回答内容
        /// </summary>
        [JsonPropertyName("answer")]
        public string Answer { get; set; }
    }

    /// <summary>
    /// 消息结束流式响应
    /// </summary>
    public class MessageEndStreamResponse : StreamResponseBase
    {
        /// <summary>
        /// 元数据
        /// </summary>
        [JsonPropertyName("metadata")]
        public Metadata Metadata { get; set; }
    }

    /// <summary>
    /// 错误流式响应
    /// </summary>
    public class ErrorStreamResponse : StreamResponseBase
    {
        /// <summary>
        /// HTTP状态码
        /// </summary>
        [JsonPropertyName("status")]
        public int Status { get; set; }

        /// <summary>
        /// 错误码
        /// </summary>
        [JsonPropertyName("code")]
        public string Code { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
