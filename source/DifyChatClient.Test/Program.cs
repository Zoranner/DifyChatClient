using DifyChatClient;
using DifyChatClient.Models;

class Program
{
    static async Task Main(string[] _)
    {
        Console.WriteLine("DifyChat 客户端示例程序");

        try
        {
            // 创建聊天客户端实例
            var client = new ChatClient("YOUR_API_KEY", "YOUR_DIFY_API_ENDPOINT");

            // 开始新的对话
            Console.WriteLine("开始新的对话...");

            while (true)
            {
                Console.Write("\n请输入消息 (输入 'exit' 退出): ");
                var userInput = Console.ReadLine();

                if (
                    string.IsNullOrEmpty(userInput)
                    || userInput.Equals("exit", StringComparison.CurrentCultureIgnoreCase)
                )
                {
                    break;
                }

                Console.Write("\nAI 响应: ");

                var request = new ChatMessageRequest
                {
                    Query = userInput,
                    User = "test_user", // 可选：设置用户标识
                    Inputs = [], // 可选：如果应用配置了输入参数
                };

                // 使用流式模式接收响应
                await client.SendMessageStreamAsync(
                    request,
                    response => Console.Write(response.Answer),
                    error => Console.WriteLine($"\n错误: {error.Message}"),
                    CancellationToken.None
                );

                Console.WriteLine(); // 换行
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"发生错误: {ex.Message}");
        }

        Console.WriteLine("\n程序结束。按任意键退出...");
        Console.ReadKey();
    }
}
