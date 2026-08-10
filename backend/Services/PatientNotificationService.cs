using System.Text;
using System.Text.Json;
using backend.Models;
using MongoDB.Driver;

namespace backend.Services;

public class PatientNotificationService
{
    private readonly MongoDbContext _db;
    private readonly IConfiguration _config;
    private readonly IHttpClientFactory _httpClientFactory;

    public PatientNotificationService(
        MongoDbContext db,
        IConfiguration config,
        IHttpClientFactory httpClientFactory)
    {
        _db = db;
        _config = config;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<NotificationLog> SendWelcomeMessageAsync(Patient patient)
    {
        var message = (_config["Notifications:WelcomeMessage"]
            ?? "Hola {name}, te informamos de que has sido dado de alta en el sistema. ¡Bienvenido!")
            .Replace("{name}", patient.Name);

        var provider = _config["Notifications:Provider"] ?? "none";

        var log = new NotificationLog
        {
            PatientId = patient.Id ?? string.Empty,
            PatientName = patient.Name,
            Phone = patient.Phone,
            Channel = provider,
            Message = message,
            Status = "skipped",
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            var status = provider.ToLowerInvariant() switch
            {
                "telegram" => await SendTelegramAsync(patient, message),
                "whatsapp" => await SendWhatsAppAsync(patient, message),
                _ => "skipped"
            };
            log.Status = status;
        }
        catch (Exception ex)
        {
            log.Status = "failed";
            log.Error = ex.Message;
        }

        try
        {
            await _db.NotificationLogs.InsertOneAsync(log);
        }
        catch
        {
            // El registro del log nunca debe impedir la creación del paciente.
        }

        return log;
    }

    private async Task<string> SendTelegramAsync(Patient patient, string message)
    {
        var token = _config["Notifications:TelegramBotToken"];
        var chatId = _config["Notifications:TelegramChatId"] ?? patient.Phone;
        if (string.IsNullOrWhiteSpace(token))
            return "failed";

        using var client = _httpClientFactory.CreateClient("Notifications");
        var payload = new { chat_id = chatId, text = message };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"https://api.telegram.org/bot{token}/sendMessage", content);

        return response.IsSuccessStatusCode ? "sent" : "failed";
    }

    private async Task<string> SendWhatsAppAsync(Patient patient, string message)
    {
        var token = _config["Notifications:WhatsAppToken"];
        var phoneNumberId = _config["Notifications:WhatsAppPhoneNumberId"];
        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(phoneNumberId))
            return "failed";

        using var client = _httpClientFactory.CreateClient("Notifications");
        var request = new HttpRequestMessage(HttpMethod.Post, $"https://graph.facebook.com/v19.0/{phoneNumberId}/messages");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        request.Content = new StringContent(JsonSerializer.Serialize(new
        {
            messaging_product = "whatsapp",
            to = patient.Phone,
            type = "text",
            text = new { body = message }
        }), Encoding.UTF8, "application/json");

        var response = await client.SendAsync(request);
        return response.IsSuccessStatusCode ? "sent" : "failed";
    }
}
