Using Web API in Your APPLICATION
1) Install-Package Microsoft.AspNet.WebApi.Client
Add the Model Class

Add the following class to the application:

class Product
{
    public string Name { get; set; }
    public double Price { get; set; }
    public string Category { get; set; }
}
This class matches the data model used in the "ProductStore" Web API project.

Create and Initialize HttpClient

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ProductStoreClient
{
    class Program
    {
        static void Main()
        {
            RunAsync().Wait();
        }

        static async Task RunAsync()
        {
            using (var client = new HttpClient())
            {
                // TODO - Send HTTP requests
            }
        }
    }
}
Notice that the Main function calls an async method named RunAsync and then blocks until RunAsyncc completes. Many of the HttpClient methods are async, because they perform network I/O. In the RunAsync method, I'll show the correct way to invoke those methods asynchronously. It's OK to block the main thread in a console application, but in a GUI application, you should never block the UI thread.

The using statement creates an HttpClient instance and disposes it when the instance goes out of scope. Inside the using statement, add the following code:

using (var client = new HttpClient())
{
    // New code:
    client.BaseAddress = new Uri("http://localhost:9000/");
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
}
This code sets the base URI for HTTP requests, and sets the Accept header to "application/json", which tells the server to send data in JSON format.

Getting a Resource (HTTP GET)

The following code sends a GET request for a product:

using (var client = new HttpClient())
{
    client.BaseAddress = new Uri("http://localhost:9000/");
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

    // New code:
    HttpResponseMessage response = await client.GetAsync("api/products/1");
    if (response.IsSuccessStatusCode)
    {
        Product product = await response.Content.ReadAsAsync>Product>();
        Console.WriteLine("{0}\t${1}\t{2}", product.Name, product.Price, product.Category);
    }
}
The GetAsync method sends the HTTP GET request. The method is asynchronous, because it performs network I/O. The await keyword suspends execution until the asynchronous method completes. When the method completes, it returns an HttpResponseMessage that contains the HTTP response.

If the status code in the response is a success code, the response body contains the JSON representation of a product. Call ReadAsAsync to deserialize the JSON payload to a Product instance. The ReadAsync method is asynchronous because the response body can be arbitrarily large.

A note about error handling: HttpClient does not throw an exception when the HTTP response contains an error code. Instead, the IsSuccessStatusCode property is false if the status is an error code.

If you prefer to treat HTTP error codes as exceptions, call the EnsureSuccessStatusCode method. This method throws an exception if the response status is not a success code:

try
{
    HttpResponseMessage response = await client.GetAsync("api/products/1");
    resp.EnsureSuccessStatusCode();    // Throw if not a success code.

    // ...
}
catch (HttpRequestException e)
{
    // Handle exception.
}
HttpClient can can throw exceptions for other reasons as well — for example, if the request times out.

Using Media-Type Formatters in ReadAsync

When ReadAsAsync is called with no parameters, the method uses the default set of media-type formatters to read the response body. The default formatters support JSON, XML, and Form-url-encoded data.

You can also specify a list of formatters, which is useful if you have a custom media-type formatter:

var formatters = new List<MediaTypeFormatter>() {
    new MyCustomFormatter(),
    new JsonMediaTypeFormatter(),
    new XmlMediaTypeFormatter()
};
resp.Content.ReadAsAsync<IEnumerable<Product>>(formatters);
Creating a Resource (HTTP POST)

The following code sends a POST request that contains a Product instance in JSON format:

// HTTP POST
var gizmo = new Product() { Name = "Gizmo", Price = 100, Category = "Widget" };
response = await client.PostAsJsonAsync("api/products", gizmo);
if (response.IsSuccessStatusCode)
{
    // Get the URI of the created resource.
    Uri gizmoUrl = response.Headers.Location;
}
The PostAsJsonAsync method serializes an object to JSON and then sends the JSON payload in a POST request. To send XML, use the PostAsXmlAsync method. To use another formatter, call PostAsync:

MediaTypeFormatter formatter = new MyCustomFormatter();
response = await client.PostAsync("api/products", gizmo, formatter);
Updating a Resource (HTTP PUT)

The following code sends a PUT request to update a product.

// HTTP PUT
gizmo.Price = 80;   // Update price
response = await client.PutAsJsonAsync(gizmoUrl, gizmo);
The PutAsJsonAsync method works like PostAsJsonAsync, except that it sends a PUT request instead of POST.

Deleting a Resource (HTTP DELETE)

The following code sends a DELETE request to delete a product.

// HTTP DELETE
response = await client.DeleteAsync(gizmoUrl);
Like GET, a DELETE request does not have a request body, so you don't need to specify JSON or XML format.

Complete Code Example

Here is the complete code for this tutorial. The code is very simple and doesn't show error handling, but it shows the basic CRUD operations using HttpClient.

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ProductStoreClient
{
    class Product
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public string Category { get; set; }
    }

    class Program
    {
        static void Main()
        {
            RunAsync().Wait();
        }

        static async Task RunAsync()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:9000/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // HTTP GET
                HttpResponseMessage response = await client.GetAsync("api/products/1");
                if (response.IsSuccessStatusCode)
                {
                    Product product = await response.Content.ReadAsAsync<Product>();
                    Console.WriteLine("{0}\t${1}\t{2}", product.Name, product.Price, product.Category);
                }

                // HTTP POST
                var gizmo = new Product() { Name = "Gizmo", Price = 100, Category = "Widget" };
                response = await client.PostAsJsonAsync("api/products", gizmo);
                if (response.IsSuccessStatusCode)
                {
                    Uri gizmoUrl = response.Headers.Location;

                    // HTTP PUT
                    gizmo.Price = 80;   // Update price
                    response = await client.PutAsJsonAsync(gizmoUrl, gizmo);

                    // HTTP DELETE
                    response = await client.DeleteAsync(gizmoUrl);
                }
            }
        }
    }
}