using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using mongoAuth.Models;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Microsoft.AspNetCore.Http;
using System.Text;
using MongoDB.Bson;

namespace mongoAuth.Controllers;

public class HomeController : Controller
{

    private IMongoDatabase db;
    private IGridFSBucket gridFS;

    public HomeController()
    {
        string connectionString = "mongodb+srv://admin:Wk2hn25gmfv1JpFh@comp4956.9hedzlx.mongodb.net/";
        MongoClient client = new MongoClient(connectionString);
        db = client.GetDatabase("CodeCraft");
        gridFS = new GridFSBucket(db);
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpPost]
    public ActionResult UploadText(string name, string content)
    {
        var fileName = name + ".txt";

        byte[] contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var stream = new MemoryStream(contentBytes);

        ObjectId fileId = gridFS.UploadFromStream(fileName, stream);

        return RedirectToAction("Index");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
