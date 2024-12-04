using Microsoft.AspNetCore.Mvc;
using phloTestAssessment.Model;
using System.Text.Json;

namespace phloTestAssessment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        private static List<Product> _products = new List<Product>
        {
            new Product { Id = 1, Name = "Product1", Description = "A great green product", Price = 10, Size = "medium" },
            new Product { Id = 2, Name = "Product2", Description = "A blue and red item", Price = 15, Size = "large" },
            new Product { Id = 3, Name = "Product3", Description = "A small yellow item", Price = 5, Size = "small" },
            new Product { Id = 4, Name = "Product4", Description = "A green and blue item", Price = 20, Size = "medium" },
            new Product { Id = 5, Name = "Product5", Description = "A large purple item", Price = 25, Size = "large" }
        };

        // The GET endpoint that accepts the query parameters
        [HttpGet("filter")]
        public IActionResult FilterProducts(
            [FromQuery] decimal? minprice,
            [FromQuery] decimal? maxprice,
            [FromQuery] string size,
            [FromQuery] string highlight)
        {
            var filteredProducts = _products.AsQueryable();

            // Filter by min price
            if (minprice.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.Price >= minprice.Value);
            }

            // Filter by max price
            if (maxprice.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.Price <= maxprice.Value);
            }

            // Filter by size
            if (!string.IsNullOrEmpty(size))
            {
                filteredProducts = filteredProducts.Where(p => p.Size.Equals(size, StringComparison.OrdinalIgnoreCase));
            }

            // Highlight words in description
            if (!string.IsNullOrEmpty(highlight))
            {
                var highlightWords = highlight.Split(',').Select(h => h.Trim()).ToList();

                // Apply highlighting logic
                foreach (var product in filteredProducts)
                {
                    foreach (var word in highlightWords)
                    {
                        product.Description = product.Description.Replace(word, $"**{word}**", StringComparison.OrdinalIgnoreCase);
                    }
                }
            }

            return Ok(filteredProducts.ToList());
        }
        private static readonly HttpClient _httpClient = new HttpClient();
        private const string ProductsUrl = "https://pastebin.com/raw/JucRNpWs";

        // The GET endpoint that accepts the query parameters
        [HttpGet("filter")]
        public async Task<IActionResult> FilteredProducts(
            [FromQuery] decimal? minprice,
            [FromQuery] decimal? maxprice,
            [FromQuery] string size,
            [FromQuery] string highlight)
        {
            // Fetch the products from the URL
            var products = await GetProductsFromUrlAsync();

            if (products == null)
            {
                return NotFound("Products not found.");
            }

            var filteredProducts = products.AsQueryable();

            // Filter by min price
            if (minprice.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.Price >= minprice.Value);
            }

            // Filter by max price
            if (maxprice.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.Price <= maxprice.Value);
            }

            // Filter by size
            if (!string.IsNullOrEmpty(size))
            {
                filteredProducts = filteredProducts.Where(p => p.Size.Equals(size, StringComparison.OrdinalIgnoreCase));
            }

            // Highlight words in description
            if (!string.IsNullOrEmpty(highlight))
            {
                var highlightWords = highlight.Split(',').Select(h => h.Trim()).ToList();

                // Apply highlighting logic
                foreach (var product in filteredProducts)
                {
                    foreach (var word in highlightWords)
                    {
                        product.Description = product.Description.Replace(word, $"**{word}**", StringComparison.OrdinalIgnoreCase);
                    }
                }
            }

            return Ok(filteredProducts.ToList());
        }

        // Method to fetch products from the URL
        private async Task<List<Product>> GetProductsFromUrlAsync()
        {
            try
            {
                // Fetch the data from the URL
                var response = await _httpClient.GetStringAsync(ProductsUrl);

                // Deserialize the JSON response into a list of products
                var products = JsonSerializer.Deserialize<List<Product>>(response);

                return products;
            }
            catch (Exception ex)
            {
                // Handle any exceptions that might occur (e.g., network issues)
                return null;
            }
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
