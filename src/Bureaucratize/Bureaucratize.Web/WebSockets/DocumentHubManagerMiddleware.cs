using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Bureaucratize.Web.WebSockets
{
    public class DocumentHubManagerMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly DocumentHubManager _hubManager;

        public DocumentHubManagerMiddleware(RequestDelegate requestDelegate,
            DocumentHubManager hubManager)
        {
            _requestDelegate = requestDelegate;
            _hubManager = hubManager;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await _requestDelegate.Invoke(context);
                return;
            }

            var documentId = context.Request.Query["documentId"].ToString();
            var socket = await context.WebSockets.AcceptWebSocketAsync();
            var hub = _hubManager.GetOrStartHubForDocumentOfId(documentId);

            var socketId = hub.AddSocket(socket);

            await Receive(socket, async (result, buffer) =>
            {
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await hub.RemoveSocket(socketId);
                }
            });
        }

        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                handleMessage(result, buffer);
            }
        }
    }
}
