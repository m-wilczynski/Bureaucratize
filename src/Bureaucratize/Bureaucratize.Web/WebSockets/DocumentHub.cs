using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bureaucratize.ImageProcessing.Contracts.ProcessingMessages.ProcessingOutcomes;
using Bureaucratize.Web.ExampleMapping;
using Newtonsoft.Json;

namespace Bureaucratize.Web.WebSockets
{
    public class DocumentHub
    {
        private readonly ConcurrentDictionary<string, WebSocket> Sockets = new ConcurrentDictionary<string, WebSocket>();
        private readonly DocumentPageMapper ExampleMapper = new DocumentPageMapper();

        public string AddSocket(WebSocket socket)
        {
            var id = Guid.NewGuid().ToString();
            Sockets.TryAdd(id, socket);
            return id;
        }

        public async Task RemoveSocket(string id)
        {
            Sockets.TryRemove(id, out var socket);
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server.", CancellationToken.None);
        }

        public async Task SendMessageToAllAsync(string message)
        {
            foreach (var pair in Sockets)
            {
                if (pair.Value.State == WebSocketState.Open)
                    await SendMessageAsync(pair.Value, message);
            }
        }

        public async Task MapAndSendDocumentPage(DocumentPageProcessingCompleted page)
        {
            var mappingResult = ExampleMapper.MapToDomainModel(page);

            foreach (var pair in Sockets)
            {
                if (pair.Value.State == WebSocketState.Open)
                    await SendMessageAsync(pair.Value, JsonConvert.SerializeObject(mappingResult));
            }
        }

        private async Task SendMessageAsync(WebSocket socket, string message)
        {
            if (socket.State != WebSocketState.Open)
                return;

            await socket.SendAsync(new ArraySegment<byte>(Encoding.ASCII.GetBytes(message), 0, message.Length),
                                   WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
