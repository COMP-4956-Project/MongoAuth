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
        ViewBag.FileNames = ListAllFiles();
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpPost]
    public ActionResult UploadText(string name, string content)
    {
        var fileName = name + ".json";
        byte[] contentBytes = System.Text.Encoding.UTF8.GetBytes(content);
        var stream = new MemoryStream(contentBytes);

        ObjectId fileId = gridFS.UploadFromStream(fileName, stream);

        return RedirectToAction("Index");
    }

   public ActionResult DisplayFileContents(string fileName)
{
    using (var stream = gridFS.OpenDownloadStreamByName(fileName))
    {
        var reader = new StreamReader(stream);
        var fileContent = reader.ReadToEnd();
        ViewBag.FileName = fileName;
        return View((object)fileContent);
    }
}

    public List<string> ListAllFiles()
    {
        var filter = Builders<GridFSFileInfo>.Filter.Empty;
        var filesInfo = gridFS.Find(filter).ToList();

        List<string> fileNames = new List<string>();
        foreach (var fileInfo in filesInfo)
        {
            fileNames.Add(fileInfo.Filename);
        }

        return fileNames;
    }

    public ActionResult DeleteFile(string fileName)
{
    var filter = Builders<GridFSFileInfo>.Filter.Eq(x => x.Filename, fileName);
    var fileInfo = gridFS.Find(filter).FirstOrDefault();

    if (fileInfo != null)
    {
        gridFS.Delete(fileInfo.Id);
    }

    return RedirectToAction("Index");
}

[HttpPost]
public ActionResult SaveFile(string fileName, string fileContents)
{
    // Delete the existing file
    DeleteFile(fileName);

    // Create a new file with the updated content
    using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContents)))
    {
        gridFS.UploadFromStream(fileName, stream);
    }

    return RedirectToAction("Index");
}

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
