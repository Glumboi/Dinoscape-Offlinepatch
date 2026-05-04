// Filename: HttpServer.cs

using DinoScape_Server;
using Il2CppPlayFab.AdminModels;
using Il2CppPlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static DinoScape_Server.ServerSetup;

namespace HttpListenerExample
{
    class HttpServerSettings
    {
        public const string PLAYFAB_ID = "DEV_123456";
        public const string PLAYFAB_USERNAME = "DevUser";

        public const string SESSION_TICKET = "DEV_SESSION_TICKET";
        public const string ENTITY_TOKEN = "DEV_ENTITY_TOKEN";
        public const string CATALOG_VERSION = "1.0";
    }

    class HttpServer
    {

        private static Dictionary<string, Func<HttpListenerRequest, Task<string>>> routes
            = new Dictionary<string, Func<HttpListenerRequest, Task<string>>>();

        private static ServerConfig ServerConfig;
        private static List<InventoryItem> PlayerItems;

        public static HttpListener listener;
        public static string NewURL;
        public static int RequestCount = 0;

        public static void Main(string[] args)
        {
            ServerConfig = ConfigLoader.LoadConfig($"{AppDomain.CurrentDomain.BaseDirectory}{ServerConfig.CONFIG_FILE}");
            string itemsPath = $"{AppDomain.CurrentDomain.BaseDirectory}{ServerConfig.ItemsLocation}";

            if (Directory.Exists(itemsPath))
            {
                PlayerItems = new();
                var files = Directory.GetFiles(itemsPath);
                if (files.Length > 0)
                {
                    foreach (var f in files)
                    {
                        PlayerItems.Add(InventoryItem.LoadItemFile(f));
                    }

                    Console.WriteLine($"Loaded items from {itemsPath}");
                }
            }
            else
            {
                Directory.CreateDirectory(itemsPath);
                PlayerItems = ServerSetup.InitPlayerInventory(ServerConfig.CustomsPath);

                foreach (var item in PlayerItems)
                {
                    InventoryItem.SaveItemFile(item, itemsPath);
                }

                if (PlayerItems.Count > 0)
                    Console.WriteLine("Loaded items from dumped customs and created Items directory!");
                else
                    Console.WriteLine("Failed to load any items!");
            }

            NewURL = $"{ServerConfig.Address}:{ServerConfig.Port}/";

            listener = new HttpListener();
            listener.Prefixes.Add(NewURL);
            listener.Start();

            Console.WriteLine("Listening on " + NewURL);

#if _DEBUG // -> Testing custom items
            InventoryItem item = new InventoryItem();
            item.Desc = "This is a test description!\n";
            item.ItemId = "TestID";
            item.ItemInstanceId = "Test name";
            item.CatalogVersion = HttpServerSettings.CATALOG_VERSION;
            item.PriceEmeralds = 0;
            PlayerItems.Add(item);
#endif
            RegisterRoutes();
            HandleIncomingConnections().GetAwaiter().GetResult();
            listener.Close();
        }

        private static void RegisterRoutes()
        {
            AddRoute("POST", "/Client/LoginWithEmailAddress", HandleLogin);
            AddRoute("POST", "/Client/GetAccountInfo", HandleGetAccountInfo);
            AddRoute("POST", "/Client/GetUserInventory", HandleGetInventory);
        }

        private static void AddRoute(string method, string path, Func<HttpListenerRequest, Task<string>> handler)
        {
            routes[$"{method}:{path}"] = handler;
        }

        public static async Task HandleIncomingConnections()
        {
            while (true)
            {
                var ctx = await listener.GetContextAsync();
                _ = Task.Run(() => ProcessRequest(ctx));
            }
        }

        private static async Task ProcessRequest(HttpListenerContext ctx)
        {
            var req = ctx.Request;
            var resp = ctx.Response;

            Console.WriteLine($"#{++RequestCount} {req.HttpMethod} {req.Url.AbsolutePath}");

            string key = $"{req.HttpMethod}:{req.Url.AbsolutePath}";

            try
            {
                if (routes.TryGetValue(key, out var handler))
                {
                    string json = await handler(req);
                    await SendJsonResponse(resp, json);
                }
                else
                {
                    string body = await ReadRequestBody(req);

                    Console.WriteLine("UNKNOWN:");
                    Console.WriteLine(req.Url.AbsolutePath);
                    Console.WriteLine(body);

                    await SendJsonResponse(resp, JsonSerializer.Serialize(new
                    {
                        code = 404,
                        status = "Error",
                        data = new { }
                    }));
                }
            }
            catch (Exception ex)
            {
                await SendJsonResponse(resp, JsonSerializer.Serialize(new
                {
                    error = ex.Message
                }), 500);
            }
        }

        // ---------------- RESPONSE ----------------

        public static async Task SendJsonResponse(HttpListenerResponse resp, string json, int statusCode = 200)
        {
            byte[] data = Encoding.UTF8.GetBytes(json);

            resp.StatusCode = statusCode;
            resp.ContentType = "application/json";
            resp.ContentEncoding = Encoding.UTF8;
            resp.Headers.Add("X-RequestId", Guid.NewGuid().ToString());
            resp.ContentLength64 = data.Length;
            await resp.OutputStream.WriteAsync(data, 0, data.Length);
            resp.Close();
        }

        public static async Task<string> ReadRequestBody(HttpListenerRequest req)
        {
            Stream stream = req.InputStream;

            string encoding = req.Headers["Content-Encoding"]?.ToLower();

            if (encoding == "gzip")
                stream = new GZipStream(stream, CompressionMode.Decompress);
            else if (encoding == "deflate")
                stream = new DeflateStream(stream, CompressionMode.Decompress);

            using var reader = new StreamReader(stream, Encoding.UTF8);
            return await reader.ReadToEndAsync();
        }

        // ---------------- HANDLERS ----------------

        private static async Task<string> HandleLogin(HttpListenerRequest req)
        {
            string body = await ReadRequestBody(req);
            Console.WriteLine("Login Body: " + body);

            var response = new
            {
                code = 200,
                status = "OK",
                data = new
                {
                    SessionTicket = HttpServerSettings.SESSION_TICKET,
                    PlayFabId = HttpServerSettings.PLAYFAB_ID,
                    NewlyCreated = false,

                    EntityToken = new
                    {
                        EntityToken = HttpServerSettings.ENTITY_TOKEN,
                        TokenExpiration = DateTime.UtcNow.AddYears(1).ToString("o"),
                        Entity = new
                        {
                            Id = HttpServerSettings.PLAYFAB_ID, // MUST MATCH
                            Type = "title_player"
                        }
                    }
                }
            };

            return JsonSerializer.Serialize(response);
        }

        private static async Task<string> HandleGetAccountInfo(HttpListenerRequest req)
        {
            var response = new
            {
                code = 200,
                status = "OK",
                data = new
                {
                    AccountInfo = new
                    {
                        PlayFabId = HttpServerSettings.PLAYFAB_ID,
                        Username = HttpServerSettings.PLAYFAB_USERNAME,
                        Created = DateTime.UtcNow.ToString("o"),

                        TitleInfo = new
                        {
                            DisplayName = HttpServerSettings.PLAYFAB_USERNAME
                        }
                    }
                }
            };

            return JsonSerializer.Serialize(response);
        }

        private static async Task<string> HandleGetInventory(HttpListenerRequest req)
        {

            if (ServerConfig.UnlockAll)
            {
                var response = new
                {
                    code = 200,
                    status = "OK",
                    data = new
                    {
                        PlayFabId = HttpServerSettings.PLAYFAB_ID,
                        Inventory = PlayerItems,
                        VirtualCurrency = new
                        {
                            EM = Int32.MaxValue,
                            AM = Int32.MaxValue,
                        },

                        VirtualCurrencyRechargeTimes = new { }
                    }
                };

                return JsonSerializer.Serialize(response);
            }
            else
            {
                var response = new
                {
                    code = 200,
                    status = "OK",
                    data = new
                    {
                        PlayFabId = HttpServerSettings.PLAYFAB_ID,
                        Inventory = new object[] { },
                        VirtualCurrency = new
                        {
                            EM = Int32.MaxValue,
                            AM = Int32.MaxValue,
                        },

                        VirtualCurrencyRechargeTimes = new { }
                    }
                };

                return JsonSerializer.Serialize(response);
            }
        }
    }
}