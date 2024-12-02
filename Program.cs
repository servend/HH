using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq; // Для работы с JSON

class Program
{
    private static readonly HttpClient client = new HttpClient();

    static async Task Main(string[] args)
    {
        string accessToken = "USERSMDHMFOITR1C7A38M78VH26TKBRS7AB2T5GHLNOD9FREITF7ID40JF64NRKM"; // Ваш токен доступа
        string vacancyId = "110959075"; // ID вакансии для получения откликов

        try
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("api-test-agent");

            // Проверка существования вакансии
            HttpResponseMessage vacancyResponse = await client.GetAsync($"https://api.hh.ru/vacancies/{vacancyId}");
            if (vacancyResponse.IsSuccessStatusCode)
            {
                Console.WriteLine("Вакансия найдена. Запрашиваем отклики...");

                // Получение откликов на вакансию
                HttpResponseMessage responsesResponse = await client.GetAsync($"https://api.hh.ru/vacancies/{vacancyId}/responses");

                if (responsesResponse.IsSuccessStatusCode)
                {
                    string jsonResponse = await responsesResponse.Content.ReadAsStringAsync();
                    var responses = JObject.Parse(jsonResponse);
                    var items = responses["items"];

                    // Выводим ФИО и телефон кандидатов
                    foreach (var item in items)
                    {
                        string fio = item["applicant"]["name"].ToString(); // ФИО кандидата
                        string phone = item["applicant"]["contact"]["phones"]?[0]?["number"]?.ToString() ?? "Не указано"; // Номер телефона

                        Console.WriteLine($"ФИО: {fio}, Телефон: {phone}");
                    }
                }
                else
                {
                    Console.WriteLine($"Ошибка при получении откликов: {responsesResponse.StatusCode} - {responsesResponse.ReasonPhrase}");
                }
            }
            else
            {
                Console.WriteLine($"Ошибка: Вакансия не найдена: {vacancyResponse.StatusCode} - {vacancyResponse.ReasonPhrase}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка: {ex.Message}");
        }
    }
}