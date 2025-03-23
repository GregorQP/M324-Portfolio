using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Playwright;
using NUnit.Framework;

[TestFixture]
public class TodoTests
{
    private IPlaywright _playwright;
    private IBrowser _browser;

    [SetUp]
    public async Task Setup()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
    }

    [TearDown]
    public async Task Teardown()
    {
        await _browser.CloseAsync();
        _playwright.Dispose();
    }

    [Test]
    public async Task TestAddingTodo()
    {
        var context = await _browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync("https://localhost:7070");

        // Add a new task
        await page.FillAsync("#taskInput", "Test Task");
        await page.ClickAsync("button:text('Hinzufügen')");

        // Verify task is added
        var taskText = await page.InnerTextAsync("ul > li:first-child");
        Assert.IsTrue(taskText.Contains("Test Task"));
    }

    [Test]
    public async Task TestRemovingTodo()
    {
        var context = await _browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync("https://localhost:7070");

        // check my existing tasks
        var origTaskCount = await page.Locator("ul > li").CountAsync();
        // Add a new task
        await page.FillAsync("#taskInput", "Test Task Removing");
        await page.ClickAsync("button:text('Hinzufügen')");

        // Delete the first task

        // Verify task is removed
        var taskCount = await page.Locator("ul > li").CountAsync();
        Assert.AreEqual(origTaskCount, taskCount);
    }
}
