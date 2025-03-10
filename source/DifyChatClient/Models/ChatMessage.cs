using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DifyChatClient.Models
{
    /// <summary>
    /// 聊天消息请求模型
    /// </summary>
    public class ChatMessageRequest
    {
        /// <summary>
        /// 用户输入/提问内容
        /// </summary>
        [JsonPropertyName("query")]
        public string Query { get; set; }

        /// <summary>
        /// 允许传入 App 定义的各变量值
        /// </summary>
        [JsonPropertyName("inputs")]
        public Dictionary<string, object> Inputs { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// 响应模式：streaming（流式）或 blocking（阻塞）
        /// </summary>
        [JsonPropertyName("response_mode")]
        public string ResponseMode { get; set; } = "streaming";

        /// <summary>
        /// 用户标识
        /// </summary>
        [JsonPropertyName("user")]
        public string User { get; set; }

        /// <summary>
        /// 会话ID
        /// </summary>
        [JsonPropertyName("conversation_id")]
        public string ConversationId { get; set; }

        /// <summary>
        /// 是否自动生成标题
        /// </summary>
        [JsonPropertyName("auto_generate_name")]
        public bool? AutoGenerateName { get; set; }

        /// <summary>
        /// 文件列表
        /// </summary>
        [JsonPropertyName("files")]
        public List<FileInfo> Files { get; set; }
    }

    /// <summary>
    /// 文件信息
    /// </summary>
    public class FileInfo
    {
        /// <summary>
        /// 文件类型
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// 传递方式
        /// </summary>
        [JsonPropertyName("transfer_method")]
        public string TransferMethod { get; set; }

        /// <summary>
        /// 文件URL
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; }

        /// <summary>
        /// 上传文件ID
        /// </summary>
        [JsonPropertyName("upload_file_id")]
        public string UploadFileId { get; set; }
    }
}
