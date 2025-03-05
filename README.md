# DifyChatClient

对话应用支持会话持久化，可将之前的聊天记录作为上下文进行回答，可适用于聊天/客服 AI 等。

### 基础 URL

```
https://api.dify.ai/v1
```

### 鉴权

Service API 使用 `API-Key` 进行鉴权。

*__强烈建议开发者把 `API-Key` 放在后端存储，而非分享或者放在客户端存储，以免 `API-Key` 泄露，导致财产损失。__*

所有 API 请求都应在 **`Authorization`** HTTP Header 中包含您的 `API-Key`，如下所示：

```javascript
Authorization: Bearer {API_KEY}
```

---

## 发送对话消息

`POST /chat-messages`

创建会话消息。

### Request Body

- `query` (string)
  - 用户输入/提问内容。

- `inputs` (object)
  - 允许传入 App 定义的各变量值。
  - inputs 参数包含了多组键值对（Key/Value pairs），每组的键对应一个特定变量，每组的值则是该变量的具体值。
  - 如果变量是文件类型，请指定一个包含以下 `files` 中所述键的对象。
  - 默认 `{}`

- `response_mode` (string)
  - `streaming` 流式模式（推荐）。基于 SSE（[Server-Sent Events](https://developer.mozilla.org/en-US/docs/Web/API/Server-sent_events/Using_server-sent_events)）实现类似打字机输出方式的流式返回。
  - `blocking` 阻塞模式，等待执行完毕后返回结果。（请求若流程较长可能会被中断）。
  - *由于 Cloudflare 限制，请求会在 100 秒超时无返回后中断。*

- `user` (string)
  - 用户标识，用于定义终端用户的身份，方便检索、统计。
  - 由开发者定义规则，需保证用户标识在应用内唯一。

- `conversation_id` (string)
  - （选填）会话 ID，需要基于之前的聊天记录继续对话，必须传之前消息的 conversation_id。

- `files` (array[object])
  - 文件列表，适用于传入文件结合文本理解并回答问题，仅当模型支持 Vision 能力时可用。
  - 文件对象属性：
    - `type` (string) 支持类型：
      - `document`: 'TXT', 'MD', 'MARKDOWN', 'PDF', 'HTML', 'XLSX', 'XLS', 'DOCX', 'CSV', 'EML', 'MSG', 'PPTX', 'PPT', 'XML', 'EPUB'
      - `image`: 'JPG', 'JPEG', 'PNG', 'GIF', 'WEBP', 'SVG'
      - `audio`: 'MP3', 'M4A', 'WAV', 'WEBM', 'AMR'
      - `video`: 'MP4', 'MOV', 'MPEG', 'MPGA'
      - `custom`: 其他文件类型
    - `transfer_method` (string) 传递方式:
      - `remote_url`: 图片地址
      - `local_file`: 上传文件
    - `url` 图片地址。（仅当传递方式为 `remote_url` 时）
    - `upload_file_id` 上传文件 ID。（仅当传递方式为 `local_file` 时）

- `auto_generate_name` (bool)
  - （选填）自动生成标题，默认 `true`
  - 若设置为 `false`，则可通过调用会话重命名接口并设置 `auto_generate` 为 `true` 实现异步生成标题

### Response

当 `response_mode` 为 `blocking` 时，返回 ChatCompletionResponse object。
当 `response_mode` 为 `streaming` 时，返回 ChunkChatCompletionResponse object 流式序列。

#### ChatCompletionResponse

返回完整的 App 结果，`Content-Type` 为 `application/json`。

- `message_id` (string) 消息唯一 ID
- `conversation_id` (string) 会话 ID
- `mode` (string) App 模式，固定为 chat
- `answer` (string) 完整回复内容
- `metadata` (object) 元数据
  - `usage` (Usage) 模型用量信息
  - `retriever_resources` (array[RetrieverResource]) 引用和归属分段列表
- `created_at` (int) 消息创建时间戳，如：1705395332

#### ChunkChatCompletionResponse

返回 App 输出的流式块，`Content-Type` 为 `text/event-stream`。
每个流式块均为 data: 开头，块之间以 \n\n 即两个换行符分隔，如下所示：

```streaming
data: {"event": "message", "task_id": "900bbd43-dc0b-4383-a372-aa6e6c414227", "id": "663c5084-a254-4040-8ad3-51f2a3c1a77c", "answer": "Hi", "created_at": 1705398420}
```

流式块中根据 event 不同，结构也不同：

1. `event: message` LLM 返回文本块事件
   - `task_id` (string) 任务 ID，用于请求跟踪和下方的停止响应接口
   - `message_id` (string) 消息唯一 ID
   - `conversation_id` (string) 会话 ID
   - `answer` (string) LLM 返回文本块内容
   - `created_at` (int) 创建时间戳

2. `event: message_file` 文件事件
   - `id` (string) 文件唯一ID
   - `type` (string) 文件类型，目前仅为image
   - `belongs_to` (string) 文件归属，user或assistant，该接口返回仅为 `assistant`
   - `url` (string) 文件访问地址
   - `conversation_id` (string) 会话ID

3. `event: message_end` 消息结束事件
   - `task_id` (string) 任务 ID
   - `message_id` (string) 消息唯一 ID
   - `conversation_id` (string) 会话 ID
   - `metadata` (object) 元数据
     - `usage` (Usage) 模型用量信息
     - `retriever_resources` (array[RetrieverResource]) 引用和归属分段列表

4. `event: tts_message` TTS 音频流事件
   - `task_id` (string) 任务 ID
   - `message_id` (string) 消息唯一 ID
   - `audio` (string) 语音合成之后的音频块使用 Base64 编码之后的文本内容
   - `created_at` (int) 创建时间戳

5. `event: tts_message_end` TTS 音频流结束事件
   - `task_id` (string) 任务 ID
   - `message_id` (string) 消息唯一 ID
   - `audio` (string) 结束事件是没有音频的，所以这里是空字符串
   - `created_at` (int) 创建时间戳

6. `event: workflow_started` workflow 开始执行
   - `task_id` (string) 任务 ID
   - `workflow_run_id` (string) workflow 执行 ID
   - `data` (object) 详细内容
     - `id` (string) workflow 执行 ID
     - `workflow_id` (string) 关联 Workflow ID
     - `sequence_number` (int) 自增序号，App 内自增，从 1 开始
     - `created_at` (timestamp) 开始时间

7. `event: node_started` node 开始执行
   - `task_id` (string) 任务 ID
   - `workflow_run_id` (string) workflow 执行 ID
   - `data` (object) 详细内容
     - `id` (string) workflow 执行 ID
     - `node_id` (string) 节点 ID
     - `node_type` (string) 节点类型
     - `title` (string) 节点名称
     - `index` (int) 执行序号，用于展示 Tracing Node 顺序
     - `predecessor_node_id` (string) 前置节点 ID，用于画布展示执行路径
     - `inputs` (object) 节点中所有使用到的前置节点变量内容
     - `created_at` (timestamp) 开始时间

8. `event: node_finished` node 执行结束
   - `task_id` (string) 任务 ID
   - `workflow_run_id` (string) workflow 执行 ID
   - `data` (object) 详细内容
     - `id` (string) node 执行 ID
     - `node_id` (string) 节点 ID
     - `index` (int) 执行序号
     - `predecessor_node_id` (string) 前置节点 ID（可选）
     - `inputs` (object) 节点中所有使用到的前置节点变量内容
     - `process_data` (json) 节点过程数据（可选）
     - `outputs` (json) 输出内容（可选）
     - `status` (string) 执行状态 `running` / `succeeded` / `failed` / `stopped`
     - `error` (string) 错误原因（可选）
     - `elapsed_time` (float) 耗时(s)（可选）
     - `execution_metadata` (json) 元数据
       - `total_tokens` (int) 总使用 tokens（可选）
       - `total_price` (decimal) 总费用（可选）
       - `currency` (string) 货币，如 `USD` / `RMB`（可选）
     - `created_at` (timestamp) 开始时间

9. `event: workflow_finished` workflow 执行结束
   - `task_id` (string) 任务 ID
   - `workflow_run_id` (string) workflow 执行 ID
   - `data` (object) 详细内容
     - `id` (string) workflow 执行 ID
     - `workflow_id` (string) 关联 Workflow ID
     - `status` (string) 执行状态
     - `outputs` (json) 输出内容（可选）
     - `error` (string) 错误原因（可选）
     - `elapsed_time` (float) 耗时(s)（可选）
     - `total_tokens` (int) 总使用 tokens（可选）
     - `total_steps` (int) 总步数，默认 0
     - `created_at` (timestamp) 开始时间
     - `finished_at` (timestamp) 结束时间

10. `event: error` 错误事件
    - `task_id` (string) 任务 ID
    - `message_id` (string) 消息唯一 ID
    - `status` (int) HTTP 状态码
    - `code` (string) 错误码
    - `message` (string) 错误消息

11. `event: ping` 每 10s 一次的 ping 事件，保持连接存活

### 错误码

- 404：对话不存在
- 400：`invalid_param`，传入参数异常
- 400：`app_unavailable`，App 配置不可用
- 400：`provider_not_initialize`，无可用模型凭据配置
- 400：`provider_quota_exceeded`，模型调用额度不足
- 400：`model_currently_not_support`，当前模型不可用
- 400：`completion_request_error`，文本生成失败
- 500：服务内部异常

### 示例

#### 请求示例

```bash
curl -X POST 'https://api.dify.ai/v1/chat-messages' \
--header 'Authorization: Bearer {api_key}' \
--header 'Content-Type: application/json' \
--data-raw '{
    "inputs": {},
    "query": "What are the specs of the iPhone 13 Pro Max?",
    "response_mode": "streaming",
    "conversation_id": "",
    "user": "abc-123",
    "files": [
      {
        "type": "image",
        "transfer_method": "remote_url",
        "url": "https://cloud.dify.ai/logo/logo-site.png"
      }
    ]
}'
```

#### 阻塞模式响应示例

```json
{
    "event": "message",
    "message_id": "9da23599-e713-473b-982c-4328d4f5c78a",
    "conversation_id": "45701982-8118-4bc5-8e9b-64562b4555f2",
    "mode": "chat",
    "answer": "iPhone 13 Pro Max specs are listed here:...",
    "metadata": {
        "usage": {
            "prompt_tokens": 1033,
            "prompt_unit_price": "0.001",
            "prompt_price_unit": "0.001",
            "prompt_price": "0.0010330",
            "completion_tokens": 128,
            "completion_unit_price": "0.002",
            "completion_price_unit": "0.001",
            "completion_price": "0.0002560",
            "total_tokens": 1161,
            "total_price": "0.0012890",
            "currency": "USD",
            "latency": 0.7682376249867957
        },
        "retriever_resources": [
            {
                "position": 1,
                "dataset_id": "101b4c97-fc2e-463c-90b1-5261a4cdcafb",
                "dataset_name": "iPhone",
                "document_id": "8dd1ad74-0b5f-4175-b735-7d98bbbb4e00",
                "document_name": "iPhone List",
                "segment_id": "ed599c7f-2766-4294-9d1d-e5235a61270a",
                "score": 0.98457545,
                "content": "\"Model\",\"Release Date\",\"Display Size\",\"Resolution\",\"Processor\",\"RAM\",\"Storage\",\"Camera\",\"Battery\",\"Operating System\"\n\"iPhone 13 Pro Max\",\"September 24, 2021\",\"6.7 inch\",\"1284 x 2778\",\"Hexa-core (2x3.23 GHz Avalanche + 4x1.82 GHz Blizzard)\",\"6 GB\",\"128, 256, 512 GB, 1TB\",\"12 MP\",\"4352 mAh\",\"iOS 15\""
            }
        ]
    },
    "created_at": 1705407629
}
```

## 停止响应

`POST /chat-messages/:task_id/stop`

仅支持流式模式。

### Path 参数

- `task_id` (string) 任务 ID，可在流式返回 Chunk 中获取

### Request Body

- `user` (string) Required
  - 用户标识，用于定义终端用户的身份，必须和发送消息接口传入 user 保持一致。

### Response

- `result` (string) 固定返回 success

### 示例

#### 请求示例

```bash
curl -X POST 'https://api.dify.ai/v1/chat-messages/:task_id/stop' \
-H 'Authorization: Bearer {api_key}' \
-H 'Content-Type: application/json' \
--data-raw '{ "user": "abc-123"}'
```

#### 响应示例

```json
{
  "result": "success"
}
```
