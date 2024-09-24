using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PB303Fashion.DataAccessLayer;
using PB303Fashion.DataAccessLayer.Entities;
using PB303Fashion.Extensions;
using PB303Fashion.Models;

namespace PB303Fashion.Areas.AdminPanel.Controllers;

public class TopTrendingController : AdminController
{
    private readonly AppDbContext _dbContext;

    public TopTrendingController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IActionResult> Index()
    {
        var topTrendings = await _dbContext.TopTrendings.ToListAsync();
        return View(topTrendings);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TopTrending topTrending)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        if (!topTrending.ImageFile.IsImage())
        {
            ModelState.AddModelError("ImageFile", "Sekil secmelisiz");

            return View();
        }

        if (!topTrending.ImageFile.IsAllowedSize(1))
        {
            ModelState.AddModelError("ImageFile", "Sekil olcusu max 1mb olmalidir");

            return View();
        }

        var imageName = await topTrending.ImageFile.GenerateFileAsync(Constants.TopTrendingImagePath);

        topTrending.ImgUrl = imageName;


        await _dbContext.TopTrendings.AddAsync(topTrending);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
