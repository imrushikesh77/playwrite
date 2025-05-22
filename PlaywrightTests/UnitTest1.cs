using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using System.Text.Json;

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

    // [Fact]
    // public async Task AmazonAddToCartFlow()
    // {
    //     // Open Amazon
    //     await Page.GotoAsync("https://www.amazon.in");

    //     // Search text for iPhone
    //     await Page.FillAsync("input#twotabsearchtextbox", "iPhone");
    //     // Click on search button
    //     await Page.ClickAsync("input#nav-search-submit-button");


    //     // Step 1: Listen for the new page (tab) before clicking the product
    //     var newPageTask = Page.Context.WaitForPageAsync();


    //     // Click on the first product
    //     await Page.ClickAsync("div[data-cel-widget='search_result_2'] a.a-link-normal");


    //     // Step 3: Get the new page (tab)
    //     var productPage = await newPageTask;


    //     // Step 4: Wait for the page to load fully
    //     await productPage.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

    //     // Wait until Add to Cart button is visible and click it
    //     await productPage
    //             .GetByRole(AriaRole.Button, new() { Name = "Add to Cart", Exact = true })
    //             .ClickAsync();


    //     // Open cart page
    //     await productPage.GotoAsync("https://www.amazon.in/gp/cart/view.html?ref_=nav_cart");


    //     // Increase quantity by 1
    //     await productPage
    //             .GetByRole(AriaRole.Button, new() { Name = "Increase quantity by one", Exact = true })
    //             .ClickAsync();

    //     // Click on Proceed to buy
    //     await productPage.Locator("span#sc-buy-box-ptc-button").ClickAsync();



    //     // Wait for 5 seconds
    //     await Task.Delay(5000);
    // }


    // Dino game automation
    [Fact]
    public async Task DinoGameAutomation()
    {
        await Page.GotoAsync("https://elgoog.im/dino/");

        await Page.Keyboard.PressAsync("Space");

        while (true)
        {
            try
            {
                var resultObj = await Page.EvaluateAsync(@"() => {
                                                                    const runner = Runner.instance_;
                                                                    const tRex = runner.tRex;
                                                                    const obstacles = runner.horizon.obstacles;

                                                                    if (obstacles.length === 0)
                                                                        return { dist: 9999, jumping: tRex.jumping, type: '', y: 0 };

                                                                    const obs = obstacles[0];
                                                                    return {
                                                                        dist: obs.xPos - tRex.xPos,
                                                                        jumping: tRex.jumping,
                                                                        type: obs.typeConfig.type,
                                                                        y: obs.yPos
                                                                    };
                                                                }");

                string resultJson = resultObj.ToString(); // Convert from Playwright's object to JSON string

                using JsonDocument doc = JsonDocument.Parse(resultJson);
                var root = doc.RootElement;

                int distance = root.GetProperty("dist").GetInt32();
                bool isJumping = root.GetProperty("jumping").GetBoolean();
                string type = root.GetProperty("type").GetString();
                int yPos = root.GetProperty("y").GetInt32();

                bool isBird = type == "PTERODACTYL";
                bool isCactus = !isBird;

                if (distance < 100 && !isJumping)
                {
                    if (isBird)
                    {
                        if (yPos < 75)
                        {
                            // High flying bird – do nothing
                        }
                        else if (yPos >= 75 && yPos < 100)
                        {
                            // Low flying bird – duck
                            await Page.Keyboard.DownAsync("ArrowDown");
                            await Task.Delay(300); // Duck for a short duration
                            await Page.Keyboard.UpAsync("ArrowDown");
                        }
                        else if (yPos >= 100)
                        {
                            // Bird at surface level – jump
                            await Page.Keyboard.PressAsync("ArrowUp");
                        }
                    }
                    else if (isCactus)
                    {
                        // Cactus – jump
                        await Page.Keyboard.PressAsync("ArrowUp");
                    }
                }




                await Task.Delay(30); // ~30 FPS
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                break;
            }
        }
    }
}