using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DifyChatClient.Models
{
    /// <summary>
    /// 聊天完成响应
    /// </summary>
    public class ChatCompletionResponse
    {
        /// <summary>
        /// 消息唯一ID
        /// </summary>
        [JsonPropertyName("message_id")]
        public string MessageId { get; set; }

        /// <summary>
        /// 会话ID
        /// </summary>
        [JsonPropertyName("conversation_id")]
        public string ConversationId { get; set; }

        /// <summary>
        /// App模式
        /// </summary>
        [JsonPropertyName("mode")]
        public string Mode { get; set; }

        /// <summary>
        /// 完整回复内容
        /// </summary>
        [JsonPropertyName("answer")]
        public string Answer { get; set; }

        /// <summary>
        /// 元数据
        /// </summary>
        [JsonPropertyName("metadata")]
        public Metadata Metadata { get; set; }

        /// <summary>
        /// 消息创建时间戳
        /// </summary>
        [JsonPropertyName("created_at")]
        public long CreatedAt { get; set; }
    }

    /// <summary>
    /// 元数据
    /// </summary>
    public class Metadata
    {
        /// <summary>
        /// 模型用量信息
        /// </summary>
        [JsonPropertyName("usage")]
        public Usage Usage { get; set; }

        /// <summary>
        /// 引用和归属分段列表
        /// </summary>
        [JsonPropertyName("retriever_resources")]
        public List<RetrieverResource> RetrieverResources { get; set; }
    }

    /// <summary>
    /// 模型用量信息
    /// </summary>
    public class Usage
    {
        /// <summary>
        /// 提示词tokens
        /// </summary>
        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; set; }

        /// <summary>
        /// 提示词单价
        /// </summary>
        [JsonPropertyName("prompt_unit_price")]
        public string PromptUnitPrice { get; set; }

        /// <summary>
        /// 提示词价格单位
        /// </summary>
        [JsonPropertyName("prompt_price_unit")]
        public string PromptPriceUnit { get; set; }

        /// <summary>
        /// 提示词价格
        /// </summary>
        [JsonPropertyName("prompt_price")]
        public string PromptPrice { get; set; }

        /// <summary>
        /// 完成tokens
        /// </summary>
        [JsonPropertyName("completion_tokens")]
        public int CompletionTokens { get; set; }

        /// <summary>
        /// 完成单价
        /// </summary>
        [JsonPropertyName("completion_unit_price")]
        public string CompletionUnitPrice { get; set; }

        /// <summary>
        /// 完成价格单位
        /// </summary>
        [JsonPropertyName("completion_price_unit")]
        public string CompletionPriceUnit { get; set; }

        /// <summary>
        /// 完成价格
        /// </summary>
        [JsonPropertyName("completion_price")]
        public string CompletionPrice { get; set; }

        /// <summary>
        /// 总tokens
        /// </summary>
        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }

        /// <summary>
        /// 总价格
        /// </summary>
        [JsonPropertyName("total_price")]
        public string TotalPrice { get; set; }

        /// <summary>
        /// 货币
        /// </summary>
        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        /// <summary>
        /// 延迟
        /// </summary>
        [JsonPropertyName("latency")]
        public double Latency { get; set; }
    }

    /// <summary>
    /// 引用资源
    /// </summary>
    public class RetrieverResource
    {
        /// <summary>
        /// 位置
        /// </summary>
        [JsonPropertyName("position")]
        public int Position { get; set; }

        /// <summary>
        /// 数据集ID
        /// </summary>
        [JsonPropertyName("dataset_id")]
        public string DatasetId { get; set; }

        /// <summary>
        /// 数据集名称
        /// </summary>
        [JsonPropertyName("dataset_name")]
        public string DatasetName { get; set; }

        /// <summary>
        /// 文档ID
        /// </summary>
        [JsonPropertyName("document_id")]
        public string DocumentId { get; set; }

        /// <summary>
        /// 文档名称
        /// </summary>
        [JsonPropertyName("document_name")]
        public string DocumentName { get; set; }

        /// <summary>
        /// 分段ID
        /// </summary>
        [JsonPropertyName("segment_id")]
        public string SegmentId { get; set; }

        /// <summary>
        /// 相关度分数
        /// </summary>
        [JsonPropertyName("score")]
        public double Score { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [JsonPropertyName("content")]
        public string Content { get; set; }
    }
}
