using System.Net;
using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace Blog.UseCases.Articles;

public class ArticlePostWebSocket
{
    public static Delegate Handle => Action;

    [AllowAnonymous]
    public static async Task Action(HttpContext context)
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();

            while (true)
            {
                if (webSocket.State == WebSocketState.Open)
                {
                    await webSocket.SendAsync(
                        Encoding.ASCII.GetBytes($"Send -> {DateTime.Now}"),
                        WebSocketMessageType.Text, 
                        true, 
                        CancellationToken.None);

                    await Task.Delay(3000);
                }
                else if (webSocket.State is WebSocketState.Closed or WebSocketState.Aborted)
                {
                    break;
                }
            }
        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
    }
}
