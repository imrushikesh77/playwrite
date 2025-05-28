using Microsoft.Playwright;
using Xunit;

namespace monkeytypeAuto;

public class UnitTest1
{
    [Fact]
    public async Task AutoTypeWordsTest()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new() { Headless = false });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync("https://monkeytype.com");

        var acceptBtn = page.Locator("div.main > div.buttons > button.acceptAll");
        if (await acceptBtn.IsVisibleAsync())
            await acceptBtn.ClickAsync();

        await page.WaitForSelectorAsync("div#typingTest > div#wordsWrapper");

        var input = page.Locator("input#wordsInput");
        await input.FocusAsync();

        int wordsTyped = 0;
        var testEndTime = DateTime.UtcNow.AddSeconds(30); // stop after 30s, or make dynamic

        while (DateTime.UtcNow < testEndTime)
        {
            // Locate the current word
            var currentWord = await page.Locator("div.word.active").ElementHandleAsync();
            if (currentWord == null) break;

            var letters = await currentWord.QuerySelectorAllAsync("letter");
            string word = string.Join("", await Task.WhenAll(letters.Select(l => l.InnerTextAsync())));

            Console.Write($"{word} ");

            // Type the word + space
            foreach (char c in word)
            {
                await page.Keyboard.InsertTextAsync(c.ToString());
                await Task.Delay(25);
            }

            await page.Keyboard.PressAsync(" ");
            await Task.Delay(5);

            wordsTyped++;
        }

        Console.WriteLine($"\nWords typed: {wordsTyped}");

        await Task.Delay(5000);
        await browser.CloseAsync();
    }
}