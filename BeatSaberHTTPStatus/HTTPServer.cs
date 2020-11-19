using System;
using System.Text;
using WebSocketSharp.Server;
using Zenject;

namespace BeatSaberHTTPStatus
{
    public class HTTPServer : IInitializable, IDisposable
    {
        private int ServerPort = 6557;

        private HttpServer server;
        [Inject]
        private StatusManager statusManager;
        private bool disposedValue;

        public void OnHTTPGet(HttpRequestEventArgs e)
        {
            var req = e.Request;
            var res = e.Response;

            if (req.RawUrl == "/status.json") {
                res.StatusCode = 200;
                res.ContentType = "application/json";
                res.ContentEncoding = Encoding.UTF8;

                var stringifiedStatus = Encoding.UTF8.GetBytes(statusManager.statusJSON.ToString());

                res.ContentLength64 = stringifiedStatus.Length;
                res.Close(stringifiedStatus, false);

                return;
            }

            res.StatusCode = 404;
            res.Close();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue) {
                if (disposing) {
                    // TODO: マネージド状態を破棄します (マネージド オブジェクト)
                    BeatSaberHTTPStatus.Plugin.log.Info("Stopping HTTP server");
                    server.Stop();
                }

                // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、ファイナライザーをオーバーライドします
                // TODO: 大きなフィールドを null に設定します
                disposedValue = true;
            }
        }

        // // TODO: 'Dispose(bool disposing)' にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします
        // ~HTTPServer()
        // {
        //     // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void Initialize()
        {
            server = new HttpServer(this.ServerPort);

            server.OnGet += (sender, e) =>
            {
                OnHTTPGet(e);
            };

            server.AddWebSocketService<StatusBroadcastBehavior>("/socket", initializer => initializer.SetStatusManager(this.statusManager));

            BeatSaberHTTPStatus.Plugin.log.Info("Starting HTTP server on port " + this.ServerPort);
            server.Start();
        }
    }
}
