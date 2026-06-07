using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Desk.Services;
using Desk.Data;
using Desk.Models;
using System.Text;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Desk.Constants;
using Microsoft.EntityFrameworkCore;

namespace Desk.Controllers
{
    public class CsvController : Controller
    {
        private readonly DeskContext _context;
        private readonly ILogger<CsvController> _logger;

        public CsvController(DeskContext context, ILogger<CsvController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult ImportCsv()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file selected");

           
            var userList = new List<ADUser>();

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    TrimOptions = TrimOptions.Trim,
                    IgnoreBlankLines = true,
                    Delimiter = ",",
                    MissingFieldFound = null
                };

                using (var csvReader = new CsvHelper.CsvReader(reader, csvConfig))
                {
                    csvReader.Read();
                    csvReader.ReadHeader();
                    List<string> exceptions = new List<string> { "", "Nem besorolt" };

                    while (csvReader.Read())
                    {
                        string department = "";
                        for (int i = 1; i < 7; i++)
                        {
                            department += csvReader.GetField<string>("dxmidou" + i);

                            var nextField = csvReader.GetField<string>("dxmidou" + (i + 1));
                            if (nextField == null || exceptions.Contains(nextField)) break;
                            else department += " > ";
                        }

                        var record = new ADUser
                        {
                            UserName = csvReader.GetField<string>("samaccountname"),
                            FullName = csvReader.GetField<string>("displayName"),
                            Email = csvReader.GetField<string>("mail"),
                            Phone = csvReader.GetField<string>("telephoneNumber"),
                            IsVip = csvReader.GetField<string>("IsManager") == "True" ? true : false,
                            Rank = csvReader.GetField<string>("rank"),
                            Sid = csvReader.GetField<string>("objectSid"),
                            Department = department
                        };
                        userList.Add(record);

                    }
                }

            }

            AddOrUpdateADUser(userList);
            await _context.SaveChangesAsync();

            TempData["success"] = "Az AD userek importálása sikerült";

            return RedirectToAction(nameof(Index), "ADUsers");
        }

        public void AddOrUpdateADUser(IEnumerable<ADUser> userList)
        {
            foreach (var user in userList)
            {
                var existingUser = _context.ADUsers.FirstOrDefault(a => a.Sid == user.Sid);

                if (existingUser != null)
                {
                    // Update existing entity's properties (without changing the existing id)
                    user.Id = existingUser.Id;
                    _context.Entry(existingUser).State = EntityState.Detached;
                    _context.Attach(user);
                    _context.Entry(user).State = EntityState.Modified;
                }
                else
                {
                    // Add new entity
                    _context.ADUsers.Add(user);
                }
            }
        }

    }

}