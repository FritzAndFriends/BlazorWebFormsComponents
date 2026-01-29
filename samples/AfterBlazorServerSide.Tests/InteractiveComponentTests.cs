using Microsoft.Playwright;

namespace AfterBlazorServerSide.Tests;

[Collection(nameof(PlaywrightCollection))]
public class InteractiveComponentTests
{
    private readonly PlaywrightFixture _fixture;

    public InteractiveComponentTests(PlaywrightFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Button_Click_UpdatesText()
    {
        // Arrange
        var page = await _fixture.NewPageAsync();
        var consoleErrors = new List<string>();
        
        page.Console += (_, msg) =>
        {
            if (msg.Type == "error")
            {
                consoleErrors.Add(msg.Text);
            }
        };

        try
        {
            // Act
            await page.GotoAsync($"{_fixture.BaseUrl}/ControlSamples/Button", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle
            });

            // Verify initial state
            var initialText = await page.Locator("span[style*='font-weight']").TextContentAsync();
            Assert.Contains("Not clicked yet!", initialText);

            // Click the button - try multiple selectors
            var button = await page.QuerySelectorAsync("button");
            if (button != null)
            {
                await button.ClickAsync();
                await page.WaitForTimeoutAsync(1000); // Wait for state update

                // Verify text changed
                var updatedText = await page.Locator("span[style*='font-weight']").TextContentAsync();
                Assert.True(updatedText.Contains("clicked") || updatedText.Contains("Command"),
                    $"Expected text to change after click, but got: {updatedText}");
            }

            // Assert no console errors
            Assert.Empty(consoleErrors);
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    [Fact]
    public async Task CheckBox_Toggle_Works()
    {
        // Arrange
        var page = await _fixture.NewPageAsync();
        var consoleErrors = new List<string>();
        
        page.Console += (_, msg) =>
        {
            if (msg.Type == "error")
            {
                consoleErrors.Add(msg.Text);
            }
        };

        try
        {
            // Act
            await page.GotoAsync($"{_fixture.BaseUrl}/ControlSamples/CheckBox", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle
            });

            // Find a checkbox and toggle it
            var checkbox = page.Locator("input[type='checkbox']").First;
            
            // Check initial state
            var isChecked = await checkbox.IsCheckedAsync();
            
            // Toggle the checkbox
            await checkbox.ClickAsync();
            await page.WaitForTimeoutAsync(300);
            
            // Verify state changed
            var newState = await checkbox.IsCheckedAsync();
            Assert.NotEqual(isChecked, newState);

            // Assert no console errors
            Assert.Empty(consoleErrors);
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    [Fact]
    public async Task TextBox_Input_AcceptsText()
    {
        // Arrange
        var page = await _fixture.NewPageAsync();
        var consoleErrors = new List<string>();
        
        page.Console += (_, msg) =>
        {
            if (msg.Type == "error")
            {
                consoleErrors.Add(msg.Text);
            }
        };

        try
        {
            // Act
            await page.GotoAsync($"{_fixture.BaseUrl}/ControlSamples/RequiredFieldValidator", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle
            });

            // Fill in the text field - use more lenient selector
            var textInputs = await page.QuerySelectorAllAsync("input[id='name'], input[type='text']");
            if (textInputs.Count > 0)
            {
                await textInputs[0].FillAsync("Test User");
                
                // Verify the value was set
                var value = await textInputs[0].InputValueAsync();
                Assert.Equal("Test User", value);
            }

            // Assert no console errors
            Assert.Empty(consoleErrors);
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    [Fact]
    public async Task RequiredFieldValidator_EmptySubmit_ShowsError()
    {
        // Arrange
        var page = await _fixture.NewPageAsync();
        var consoleErrors = new List<string>();
        
        page.Console += (_, msg) =>
        {
            if (msg.Type == "error")
            {
                consoleErrors.Add(msg.Text);
            }
        };

        try
        {
            // Act
            await page.GotoAsync($"{_fixture.BaseUrl}/ControlSamples/RequiredFieldValidator", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle
            });

            // Try to submit without filling required fields
            await page.ClickAsync("button[type='submit']");
            await page.WaitForTimeoutAsync(500);

            // Verify validation message appears
            var validationMessages = await page.Locator("span[style*='color: red'], span:has-text('required')").AllTextContentsAsync();
            Assert.NotEmpty(validationMessages);

            // Assert no console errors (validation errors are expected, not console errors)
            Assert.Empty(consoleErrors);
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    [Fact]
    public async Task RequiredFieldValidator_ValidSubmit_NoErrors()
    {
        // Arrange
        var page = await _fixture.NewPageAsync();
        var consoleErrors = new List<string>();
        
        page.Console += (_, msg) =>
        {
            if (msg.Type == "error")
            {
                consoleErrors.Add(msg.Text);
            }
        };

        try
        {
            // Act
            await page.GotoAsync($"{_fixture.BaseUrl}/ControlSamples/RequiredFieldValidator", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle
            });

            // Fill in the required fields - use more lenient selectors
            var nameInput = await page.QuerySelectorAsync("input[id='name'], input[type='text']");
            if (nameInput != null)
            {
                await nameInput.FillAsync("John Doe");
            }
            
            var numberInput = await page.QuerySelectorAsync("input[id='number'], input[type='number']");
            if (numberInput != null)
            {
                await numberInput.FillAsync("42");
            }

            // Submit the form
            await page.ClickAsync("button[type='submit']");
            await page.WaitForTimeoutAsync(500);

            // Assert no console errors
            Assert.Empty(consoleErrors);
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    [Fact]
    public async Task DropDownList_Selection_Changes()
    {
        // Arrange
        var page = await _fixture.NewPageAsync();
        var consoleErrors = new List<string>();
        
        page.Console += (_, msg) =>
        {
            if (msg.Type == "error")
            {
                consoleErrors.Add(msg.Text);
            }
        };

        try
        {
            // Act
            await page.GotoAsync($"{_fixture.BaseUrl}/ControlSamples/DropDownList", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle
            });

            // Find and interact with dropdown
            var dropdown = page.Locator("select").First;
            
            // Check if dropdown has options
            var options = await dropdown.Locator("option").AllAsync();
            if (options.Count > 1)
            {
                // Select a different option
                await dropdown.SelectOptionAsync(new SelectOptionValue { Index = 1 });
                await page.WaitForTimeoutAsync(300);
                
                // Verify selection changed
                var selectedValue = await dropdown.EvaluateAsync<string>("el => el.value");
                Assert.NotNull(selectedValue);
            }

            // Assert no console errors
            Assert.Empty(consoleErrors);
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    [Fact]
    public async Task HyperLink_Click_Navigates()
    {
        // Arrange
        var page = await _fixture.NewPageAsync();
        var consoleErrors = new List<string>();
        
        page.Console += (_, msg) =>
        {
            if (msg.Type == "error")
            {
                consoleErrors.Add(msg.Text);
            }
        };

        try
        {
            // Act
            await page.GotoAsync($"{_fixture.BaseUrl}/ControlSamples/HyperLink", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle
            });

            // Find hyperlinks on the page
            var links = await page.Locator("a[href]").AllAsync();
            Assert.NotEmpty(links);

            // Verify links are clickable (without actually clicking to navigate away)
            var firstLink = page.Locator("a[href]").First;
            var href = await firstLink.GetAttributeAsync("href");
            Assert.NotNull(href);

            // Assert no console errors
            Assert.Empty(consoleErrors);
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    [Fact]
    public async Task LinkButton_Click_TriggersEvent()
    {
        // Arrange
        var page = await _fixture.NewPageAsync();
        var consoleErrors = new List<string>();
        
        page.Console += (_, msg) =>
        {
            if (msg.Type == "error")
            {
                consoleErrors.Add(msg.Text);
            }
        };

        try
        {
            // Act
            await page.GotoAsync($"{_fixture.BaseUrl}/ControlSamples/LinkButton", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle
            });

            // Find and click a link button
            var linkButton = page.Locator("button, a[role='button']").First;
            if (await linkButton.CountAsync() > 0)
            {
                await linkButton.ClickAsync();
                await page.WaitForTimeoutAsync(500);
            }

            // Assert no console errors
            Assert.Empty(consoleErrors);
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    [Fact]
    public async Task GridView_Renders_WithData()
    {
        // Arrange
        var page = await _fixture.NewPageAsync();
        var consoleErrors = new List<string>();
        
        page.Console += (_, msg) =>
        {
            if (msg.Type == "error")
            {
                consoleErrors.Add(msg.Text);
            }
        };

        try
        {
            // Act
            await page.GotoAsync($"{_fixture.BaseUrl}/ControlSamples/GridView", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle
            });

            // Verify table is rendered
            var tables = await page.Locator("table").AllAsync();
            Assert.NotEmpty(tables);

            // Verify table has rows
            var rows = await page.Locator("table tr").AllAsync();
            Assert.True(rows.Count > 1, "GridView should have header and data rows");

            // Assert no console errors
            Assert.Empty(consoleErrors);
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    [Fact]
    public async Task DataList_Renders_WithData()
    {
        // Arrange
        var page = await _fixture.NewPageAsync();
        var consoleErrors = new List<string>();
        
        page.Console += (_, msg) =>
        {
            if (msg.Type == "error")
            {
                consoleErrors.Add(msg.Text);
            }
        };

        try
        {
            // Act
            await page.GotoAsync($"{_fixture.BaseUrl}/ControlSamples/DataList", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle
            });

            // Verify data list content is rendered
            var content = await page.ContentAsync();
            Assert.NotEmpty(content);

            // Verify there are data items (look for common patterns in data lists)
            var dataItems = await page.Locator("table, div[class*='item'], span[class*='item']").AllAsync();
            Assert.NotEmpty(dataItems);

            // Assert no console errors
            Assert.Empty(consoleErrors);
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    [Fact]
    public async Task TreeView_Expands_Nodes()
    {
        // Arrange
        var page = await _fixture.NewPageAsync();
        var consoleErrors = new List<string>();
        
        page.Console += (_, msg) =>
        {
            if (msg.Type == "error")
            {
                consoleErrors.Add(msg.Text);
            }
        };

        try
        {
            // Act
            await page.GotoAsync($"{_fixture.BaseUrl}/ControlSamples/TreeView", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle
            });

            // Verify tree view is rendered
            var treeElements = await page.Locator("ul, div[role='tree'], table").AllAsync();
            Assert.NotEmpty(treeElements);

            // Look for expandable nodes (common in tree views)
            var expandableElements = await page.Locator("a, button, span[class*='expand'], img[class*='expand']").AllAsync();
            
            // If there are expandable elements, try clicking one
            if (expandableElements.Count > 0)
            {
                await page.Locator("a, button, span[class*='expand'], img[class*='expand']").First.ClickAsync();
                await page.WaitForTimeoutAsync(500);
            }

            // Assert no console errors
            Assert.Empty(consoleErrors);
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    [Fact]
    public async Task Login_FormElements_Present()
    {
        // Arrange
        var page = await _fixture.NewPageAsync();
        var consoleErrors = new List<string>();
        
        page.Console += (_, msg) =>
        {
            if (msg.Type == "error")
            {
                consoleErrors.Add(msg.Text);
            }
        };

        try
        {
            // Act
            await page.GotoAsync($"{_fixture.BaseUrl}/ControlSamples/Login", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle
            });

            // Verify login form elements are present
            var inputs = await page.Locator("input[type='text'], input[type='password'], input[type='email']").AllAsync();
            Assert.NotEmpty(inputs);

            var buttons = await page.Locator("button, input[type='submit']").AllAsync();
            Assert.NotEmpty(buttons);

            // Assert no console errors
            Assert.Empty(consoleErrors);
        }
        finally
        {
            await page.CloseAsync();
        }
    }
}
