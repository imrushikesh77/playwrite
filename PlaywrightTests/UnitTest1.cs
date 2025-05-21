using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace PlaywrightTests;

public class UnitTest1 : IAsyncLifetime
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    protected IPage? Page;

    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();

        // Launch Chrome (not headless)
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false, // Make browser visible
            Channel = "chrome", // Use Chrome (must be installed)
            SlowMo = 100, // Optional: slows down actions to watch it

        });

        var context = await _browser.NewContextAsync();
        Page = await context.NewPageAsync();
    }

    public async Task DisposeAsync()
    {
        await _browser.CloseAsync();
        _playwright.Dispose();
    }

    [Fact]
    public async Task AmazonAddToCartFlow()
    {
        // Open Amazon
        await Page.GotoAsync("https://www.amazon.in");

        // Search text for iPhone
        await Page.FillAsync("input#twotabsearchtextbox", "iPhone");
        // Click on search button
        await Page.ClickAsync("input#nav-search-submit-button");


        // Step 1: Listen for the new page (tab) before clicking the product
        var newPageTask = Page.Context.WaitForPageAsync();


        // Click on the first product
        await Page.ClickAsync("div[data-cel-widget='search_result_2'] a.a-link-normal");


        // Step 3: Get the new page (tab)
        var productPage = await newPageTask;


        // Step 4: Wait for the page to load fully
        await productPage.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        // Wait until Add to Cart button is visible and click it
        await productPage
                .GetByRole(AriaRole.Button, new() { Name = "Add to Cart", Exact = true })
                .ClickAsync();


        // Open cart page
        await productPage.GotoAsync("https://www.amazon.in/gp/cart/view.html?ref_=nav_cart");


        // Increase quantity by 1
        await productPage
                .GetByRole(AriaRole.Button, new() { Name = "Increase quantity by one", Exact = true })
                .ClickAsync();

        // Click on Proceed to buy
        await productPage.Locator("span#sc-buy-box-ptc-button").ClickAsync();



        // Wait for 5 seconds
        await Task.Delay(5000);
    }
}