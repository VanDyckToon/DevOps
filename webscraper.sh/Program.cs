using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using CsvHelper;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

class Program
{
    static void Main()
    {

        Console.WriteLine("Choose an option:");
        Console.WriteLine("1. YouTube");
        Console.WriteLine("2. ictjob.be");
        Console.WriteLine("3. ikea.com");

        int choice = Convert.ToInt32(Console.ReadLine());

        switch (choice)
        {
            case 1:
                Console.WriteLine("Enter the search term for YouTube:");
                string youTubeSearchTerm = Console.ReadLine();
                List<VideoData> youTubeVideos = ScrapeYouTube(youTubeSearchTerm);

                Console.WriteLine("\nTop 5 Most Recent YouTube Videos:");
                string videoDataString = GetVideoDataString(youTubeVideos);
                PrintVideoData(youTubeVideos);

                File.AppendAllText("results.csv", videoDataString);
                File.AppendAllText("results.json", JsonConvert.SerializeObject(youTubeVideos, Formatting.Indented));
                break;

            case 2:
                Console.WriteLine("Enter the search term for ictjob.be:");
                string ictJobSearchTerm = Console.ReadLine();
                List<JobData> ictJobJobs = ScrapeIctJob(ictJobSearchTerm);

                Console.WriteLine("\nTop 5 Most Recent Jobs on ictjob.be:");
                string ictDataString = GetIctDataString(ictJobJobs);
                PrintJobData(ictJobJobs);

                File.AppendAllText("results.csv", ictDataString);
                File.AppendAllText("results.json", JsonConvert.SerializeObject(ictJobJobs, Formatting.Indented));
                break;

            case 3:
                Console.WriteLine("Enter the search term for ikea.com:");
                string ikeaSearchTerm = Console.ReadLine();

                Console.WriteLine("Do you want a specific color?");
                Console.WriteLine("1. White");
                Console.WriteLine("2. Black");
                Console.WriteLine("3. Grey");
                Console.WriteLine("4. Blue");
                Console.WriteLine("5. Beige");
                Console.WriteLine("6. Green");
                Console.WriteLine("7. Red");
                Console.WriteLine("8. No specific color");

                int colorChoice = Convert.ToInt32(Console.ReadLine());

                string colorCode = GetColorCode(colorChoice);

                List<ProductData> ikeaProducts = ScrapeIkea(ikeaSearchTerm, colorCode);

                Console.WriteLine("\nTop 5 Products on ikea.com:");
                string productDataString = GetProductDataString(ikeaProducts);
                PrintProductData(ikeaProducts);

                File.AppendAllText("results.csv", productDataString);
                File.AppendAllText("results.json", JsonConvert.SerializeObject(ikeaProducts, Formatting.Indented));
                break;

            default:
                Console.WriteLine("Invalid choice. Exiting.");
                break;
        }
        static string GetColorCode(int colorChoice)
        {
            switch (colorChoice)
            {
                case 1:
                    return "%3A10156"; // White
                case 2:
                    return "%3A10139"; // Black
                case 3:
                    return "%3A10028"; // Grey
                case 4:
                    return "%3A10007"; // Blue
                case 5:
                    return "%3A10003"; // Beige
                case 6:
                    return "%3A10033"; // Green
                case 7:
                    return "%3A10124"; // Red
                default:
                    return ""; // No specific color
            }
        }
        static string GetVideoDataString(List<VideoData> videos)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var video in videos)
            {
                sb.AppendLine($"Title: {video.Title}");
                sb.AppendLine($"Link: {video.Link}");
                sb.AppendLine($"Uploader: {video.Uploader}");
                sb.AppendLine($"Views: {video.Views}\n");
            }

            return sb.ToString();

        }
        static string GetIctDataString(List<JobData> jobs)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var job in jobs)
            {
                sb.AppendLine($"Title: {job.Title}");
                sb.AppendLine($"Company: {job.Bedrijf}");
                sb.AppendLine($"Location: {job.Locatie}");
                sb.AppendLine($"Keywords: {job.Keywords}");
                sb.AppendLine($"Link: {job.Link}\n");
            }

            return sb.ToString();

        }
        static string GetProductDataString(List<ProductData> products)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var product in products)
            {
                sb.AppendLine($"Product Name: {product.Name}");
                sb.AppendLine($"Price: {product.Price}");
                sb.AppendLine($"Description: {product.Description}");
                sb.AppendLine($"Link: {product.Link}\n");
            }

            return sb.ToString();

        }





        static void PrintVideoData(List<VideoData> videos)
        {
            foreach (var video in videos)
            {
                Console.WriteLine($"Title: {video.Title}");
                Console.WriteLine($"Link: {video.Link}");
                Console.WriteLine($"Uploader: {video.Uploader}");
                Console.WriteLine($"Views: {video.Views}\n");
            }
        }

        static void PrintJobData(List<JobData> jobs)
        {
            foreach (var job in jobs)
            {
                Console.WriteLine($"Title: {job.Title}");
                Console.WriteLine($"Company: {job.Bedrijf}");
                Console.WriteLine($"Location: {job.Locatie}");
                Console.WriteLine($"Keywords: {job.Keywords}");
                Console.WriteLine($"Link: {job.Link}\n");
            }
        }

        static void PrintProductData(List<ProductData> products)
        {
            foreach (var product in products)
            {
                Console.WriteLine($"Product Name: {product.Name}");
                Console.WriteLine($"Price: {product.Price}");
                Console.WriteLine($"Description: {product.Description}");
                Console.WriteLine($"Link: {product.Link}\n");
            }
        }

        static List<VideoData> ScrapeYouTube(string searchTerm)
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--headless");
            IWebDriver driver = new ChromeDriver(options);

            try
            {
                driver.Navigate().GoToUrl($"https://www.youtube.com/results?search_query={searchTerm}&sp=CAM%253D");

                System.Threading.Thread.Sleep(3000);

                var videoElements = driver.FindElements(By.CssSelector("ytd-video-renderer"));

                List<VideoData> videos = new List<VideoData>();

                for (int i = 0; i < Math.Min(5, videoElements.Count); i++)
                {
                    var videoElement = videoElements[i];

                    string title = videoElement.FindElement(By.Id("video-title")).Text;
                    string link = videoElement.FindElement(By.Id("video-title")).GetAttribute("href");
                    string uploader = videoElement.FindElement(By.CssSelector(".ytd-video-renderer #channel-info #text-container yt-formatted-string a")).Text;
                    string views = videoElement.FindElement(By.CssSelector("span.style-scope.ytd-video-meta-block")).Text;

                    videos.Add(new VideoData { Title = title, Link = link, Uploader = uploader, Views = views });
                }

                return videos;
            }
            finally
            {
                driver.Quit();
            }
        }

        static List<JobData> ScrapeIctJob(string searchTerm)
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--headless");
            IWebDriver driver = new ChromeDriver(options);

            try
            {
                driver.Navigate().GoToUrl($"https://www.ictjob.be/en/search-it-jobs?keywords={searchTerm}&sp=CAM%253D");

                System.Threading.Thread.Sleep(3000);

                var jobElements = driver.FindElements(By.CssSelector(".job-info"));

                List<JobData> jobs = new List<JobData>();

                for (int i = 0; i < Math.Min(5, jobElements.Count); i++)
                {
                    var jobElement = jobElements[i];

                    string title = jobElement.FindElement(By.CssSelector(".job-title")).Text;
                    string company = jobElement.FindElement(By.CssSelector(".job-company")).Text;
                    string location = jobElement.FindElement(By.CssSelector(".job-location")).Text;
                    string keywords = jobElement.FindElement(By.CssSelector(".job-keywords")).Text;
                    string link = jobElement.FindElement(By.CssSelector(".job-title")).GetAttribute("href");

                    jobs.Add(new JobData { Title = title, Bedrijf = company, Locatie = location, Keywords = keywords, Link = link });
                }

                return jobs;
            }
            finally
            {
                driver.Quit();
            }
        }

        static List<ProductData> ScrapeIkea(string searchTerm, string colorCode)
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--headless");
            IWebDriver driver = new ChromeDriver(options);

            try
            {
                driver.Navigate().GoToUrl($"https://www.ikea.com/us/en/search/?q={searchTerm}&filters=f-colors{colorCode}&sp=CAM%253D");

                System.Threading.Thread.Sleep(3000);

                var productElements = driver.FindElements(By.CssSelector(".plp-fragment-wrapper"));

                List<ProductData> products = new List<ProductData>();

                for (int i = 0; i < Math.Min(5, productElements.Count); i++)
                {
                    var productElement = productElements[i];

                    string name = productElement.FindElement(By.CssSelector(".plp-price-module__product-name")).Text;
                    string price = productElement.FindElement(By.CssSelector(".plp-price__sr-text")).Text;
                    string description = productElement.FindElement(By.CssSelector(".plp-price-module__description")).Text;
                    string link = productElement.FindElement(By.CssSelector(".plp-price-link-wrapper")).GetAttribute("href");

                    products.Add(new ProductData { Name = name, Price = price, Description = description, Link = link });
                }

                return products;
            }
            finally
            {
                driver.Quit();
            }
        }
    }

    class VideoData
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Uploader { get; set; }
        public string Views { get; set; }
    }

    class JobData
    {
        public string Title { get; set; }
        public string Bedrijf { get; set; }
        public string Locatie { get; set; }
        public string Keywords { get; set; }
        public string Link { get; set; }
    }

    class ProductData
    {
        public string Name { get; set; }
        public string Price { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
    }
}