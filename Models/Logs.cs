namespace Praxisarbeit_M295.Models
{
    public class Log
    {
        public int Id { get; set; } // Primärschlüssel
        public string Endpoint { get; set; } // Aufgerufener API-Endpunkt
        public string HttpMethod { get; set; } // HTTP-Methode (GET, POST, etc.)
        public string StatusCode { get; set; } // HTTP-Statuscode der Antwort
        public DateTime Timestamp { get; set; } // Zeitpunkt der Anfrage
        public string? Message { get; set; } // Zusätzliche Informationen, z.B. Fehler
    }
}
