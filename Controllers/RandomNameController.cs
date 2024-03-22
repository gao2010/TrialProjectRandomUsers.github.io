using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TrialProjectRandomUsers.Models;
using System.Xml.Linq;
namespace TrialProjectRandomUsers.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RandomNameController : ControllerBase
    {
        private static readonly string[] states = new string[]
        {
            "AL", "AK", "AZ", "AR", "CA", "CO", "CT", "DE", "FL", "GA",
            "HI", "ID", "IL", "IN", "IA", "KS", "KY", "LA", "ME", "MD",
            "MA", "MI", "MN", "MS", "MO", "MT", "NE", "NV", "NH", "NJ",
            "NM", "NY", "NC", "ND", "OH", "OK", "OR", "PA", "RI", "SC",
            "SD", "TN", "TX", "UT", "VT", "VA", "WA", "WV", "WI", "WY"
        };
        //gender	name.title	name.first	name.last	location.street.number	location.street.name	location.city	location.state	location.country	location.postcode	location.coordinates.latitude	location.coordinates.longitude	location.timezone.offset	location.timezone.description	email	login.uuid	login.username	login.password	login.salt	login.md5	login.sha1	login.sha256	dob.date	dob.age	registered.date	registered.age	phone	cell	id.name	id.value	picture.large	picture.medium	picture.thumbnail	nat

        public async Task<Name> GetPersonAsync()
        {
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri("https://api.randomuser.me")
            };
            //HttpResponseMessage response = await client.GetAsync("https://randomuser.me/api/");
            HttpResponseMessage response = await client.GetAsync("https://randomuser.me/api/?results=10&nat=us&format=json");
   
            response.EnsureSuccessStatusCode();

            var stringResult = await response.Content.ReadAsStringAsync();
            Person root = JsonConvert.DeserializeObject<Person>(stringResult);
            Console.WriteLine(root.Results[0].name.last);
            return root.Results[0].name;
        }


        [HttpGet]
        public IActionResult DownloadReport()
        {
            // Generate report data (e.g., string, byte array)
            string reportData = "This is some report data";
            var contentEncoding = Encoding.ASCII;
            // Set response headers
            return Content(reportData, "text/plain",contentEncoding);
        }
        public static string GetSuggestedFileFormat(HttpRequest request)
        {
            if (request.Headers.TryGetValue("Accept", out var acceptValues))
            {
                foreach (var acceptValue in acceptValues)
                {
                    // Check for common media type prefixes
                    if (acceptValue.StartsWith("application/"))
                    {
                        var parts = acceptValue.Split('/');
                        if (parts.Length > 1)
                        {
                            return parts[1].ToLower(); // Return suggested format (lowercase)
                        }
                    }
                    // Check for specific extensions based on known patterns
                    else if (acceptValue.Contains(".json"))
                    {
                        return "json";
                    }
                    else if (acceptValue.Contains(".xml"))
                    {
                        return "xml";
                    }
                }
            }

            // No clear format found, return null
            return null;
        }
        /**
         * 
        public IEnumerable<RandomName> GetPercentages(List<Result> TempNames, List<Location> TempLocations)
        {
            //1. Percentage of gender in each category

            var TotalCount = TempNames.Count();
            var FemaleCount = (from Result in TempNames
                               where Result.gender == "Female"
                               select Result.gender).Count();
            var MaleCount = (from Result in TempNames
                             where Result.gender == "Male"
                             select Result.gender).Count();
            var OtherGenderCount = TotalCount - FemaleCount - MaleCount;



            //2. Percentage of first names that start with A-M versus N-Z
            var FirstNameAMCount = (from Result in TempNames
                                    where (Regex.Match(Result.name.first, "^[a-m]").Success)
                                    select Result.gender).Count();
            var FirstNameNZCount = TotalCount - FirstNameAMCount;

            //3. Percentage of last names that start with A-M versus N-Z
            var LastNameAMCount = (from Result in TempNames
                                   where (Regex.Match(Result.name.last, "^[a-m]").Success)
                                   select Result.gender).Count();
            var LastNameNZCount = TotalCount - LastNameAMCount;


            //4. Percentage of people in each state, up to the top 10 most populous states
            List<Result> peopleData = new List<Result>(); // Replace with your data source

            // Group data by state
            var statePopulations = peopleData
              .GroupBy(person => peopleData.location.state)
              .Select(group => new { State = group.Key, TotalPopulation = group.Count() });


            foreach (var state in statePopulations)
            {
                Console.WriteLine($"{state.State}: {state.TotalPopulation}");

            }

            //select top 10 state Order by Totalpopulation
            var statePopulations1 = (from Location in TempLocations where (Location.state != "") select Location.state).Count();



            //5. Percentage of females in each state, up to the top 10 most populous states
            var states = new List<Location>();
            var stateFemaleCounts = states.GroupBy(state => state)
                              .Select(group => new { State = group.Key, FemaleCount = group.Where(s => s.gender == "Female").Count() });

            // 2. Order states by population descending and pick top 10
            var top10States = stateFemaleCounts.OrderByDescending(state => state.State.Population)
                                               .Take(10);

            // 3. Calculate total females in top 10 states
            var totalFemales = top10States.Sum(state => state.FemaleCount);

            // 4. (Optional) Include total population calculation in the query
            // You can modify the stateFemaleCounts query to include total population:

            var stateData = states.GroupBy(state => state)
                                 .Select(group => new
                                 {
                                     State = group.Key,
                                     FemaleCount = group.Where(s => s.IsFemale).Count(),
                                     TotalPopulation = group.Sum(s => s.Population)
                                 });

            //6. Percentage of males in each state, up to the top 10 most populous states



            var query = from order in orders
                        join customer in customers on order.CustomerID equals customer.CustomerID
                        select new
                        {
                            OrderID = order.OrderID,
                            CustomerName = customer.Name,
                            Rank = (from innerOrder in orders
                                    where innerOrder.CustomerID == order.CustomerID && innerOrder.Amount >= order.Amount
                                    orderby innerOrder.Amount descending
                                    select order.OrderID).Take(1).SingleOrDefault() == order.OrderID ? 1 : 0
                        };

            var query = products.GroupBy(product => product.Category)
                     .Select(group => group.OrderByDescending(product => product.Price)
                                          .Take(10))
                     .SelectMany(topProducts => topProducts);
            //7. Percentage of people in the following age ranges: 0-20, 21-40, 41-60, 61-80, 81-100, 100+


            int[] ageGroupThresholds = { 0, 21, 41, 61, 81, 101, 200 }; // Example thresholds (0-17, 18-64, 65+)

            // 1. Calculate age for each person (assuming DateOfBirth is DateTime)
            var peopleWithAge = people.Select(person => new
            {
                Name = person.Name,
                Age = DateTime.Now.Year - person.DateOfBirth.Year -
                      (DateTime.Now.DayOfYear < person.DateOfBirth.DayOfYear ? 1 : 0)
            });

            // 2. Group by age group based on thresholds
            var peopleByAgeGroup = peopleWithAge.GroupBy(person =>
            {
                int age = person.Age;
                for (int i = 0; i < ageGroupThresholds.Length - 1; i++)
                {
                    if (age >= ageGroupThresholds[i] && age < ageGroupThresholds[i + 1])
                    {
                        return i; // Age group index
                    }
                }
                return ageGroupThresholds.Length - 1; // Default to last group for outliers
            })
            .Select(group => new
            {
                AgeGroup = $"{(group.Key == 0 ? "0-" : ageGroupThresholds[group.Key])}{(ageGroupThresholds[group.Key + 1] == 120 ? "+" : "-" + ageGroupThresholds[group.Key + 1])}",
                Count = group.Count()
            });


            return null;
        }




        **/




        [HttpPost]
        public async Task<IActionResult> UploadNameList()
        {
            // Check if request has form data
            if (!Request.HasFormContentType)
            {
                return BadRequest("Expected form data");
            }

            // Get form data
            var form = await Request.ReadFormAsync();

            // Extract name list and file type
            string nameListString = form["nameList"];
            string fileType = form["fileType"];

            // Validate data
            if (string.IsNullOrEmpty(nameListString) || string.IsNullOrEmpty(fileType))
            {
                return BadRequest("Missing name list or file type");
            }

            // Process name list
            List<string> nameList = nameListString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            // Handle file based on type (optional)
            if (fileType == "txt")
            {
                // Save list to a temporary text file
                string tempFilePath = Path.GetTempFileName() + ".txt";
                await System.IO.File.WriteAllLinesAsync(tempFilePath, nameList);
            }
            else if (fileType == "json")
            {
                // Implement logic to handle CSV file (parsing etc.)
            }
            else if (fileType == "xml")
            {
                // Implement logic to handle CSV file (parsing etc.)
            }
            else
            {
                return BadRequest("Unsupported file type");
            }

            // Return success message
            return Ok("NameList and file type uploaded successfully");
        }
    }
}