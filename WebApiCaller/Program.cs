using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace WebApiCaller
{
    internal class Program
    {
        private static void Main(string[] args)
        {
        start:
            Console.WriteLine("1.Read\n2.Insert\n3.Exit");
            int choice = int.Parse(Console.ReadLine());
            switch (choice)
            {
                case 1:
                    Task<List<Item>> items = RunAsync();
                    foreach (Item item in items.Result)
                        Console.WriteLine("{0}\t{1}\t{2}\t{3}", item.ID, item.ItemName, item.ItemCount, item.ItemUnit);
                    goto start;
                case 2:
                    Console.WriteLine("Enter the item name");
                    string itemName = Console.ReadLine();
                    Console.WriteLine("Enter the item Count");
                    double itemCount = double.Parse(Console.ReadLine());
                    Console.WriteLine("Enter the item Unit");
                    string itemUnit = Console.ReadLine();
                    InsertItem(itemName, itemCount, itemUnit).Wait();
                    goto start;
            }
        }

        private static async Task<List<Item>> RunAsync()
        {
            List<Item> items = new List<Item>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:30080/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // HTTP GET
                HttpResponseMessage response = await client.GetAsync("api/Items");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    JavaScriptSerializer jSerializer = new JavaScriptSerializer();
                    items = jSerializer.Deserialize<List<Item>>(data);
                }
            }
            return items;
        }

        private static async Task InsertItem(string itemName, double itemCount, string itemUnit)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:30080/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                Item item = new Item { ItemName = itemName, ItemCount = itemCount, ItemUnit = itemUnit };
                // HTTP POST
                HttpResponseMessage response = await client.PostAsJsonAsync("api/Items", item);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Product inserted successfully.");
                }
            }
        }
    }
}
