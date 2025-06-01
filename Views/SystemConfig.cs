using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using SP2.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Avalonia.Interactivity;

namespace SP2.Views;

public partial class SystemConfig : UserControl
{
    private Canvas _hoverImageCanvas;
    private Image _hoverImage;
    private Dictionary<string, Bitmap> _unitImages;
    private Thickness _imageOffset;

    public SystemConfig()
    {
        InitializeComponent();
        
        // Get references to UI elements
        _hoverImageCanvas = this.FindControl<Canvas>("HoverImageCanvas");
        _hoverImage = this.FindControl<Image>("HoverImage");
        _imageOffset = (Thickness)this.Resources["HoverImageOffset"];
        
        // Load unit images
        LoadUnitImages();
        
        // Initialize toggle states
        InitializeToggleStates();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    private void InitializeToggleStates()
    {
        // Get all production units and set toggle states based on their availability
        var units = AssetManager.GetProdUnits();
        
        foreach (var unit in units)
        {
            ToggleSwitch toggle = unit.Name switch
            {
                "Gas Boiler 1" => this.FindControl<ToggleSwitch>("GasBoiler1Toggle"),
                "Gas Boiler 2" => this.FindControl<ToggleSwitch>("GasBoiler2Toggle"),
                "Oil Boiler 1" => this.FindControl<ToggleSwitch>("OilBoiler1Toggle"),
                "Gas Motor 1" => this.FindControl<ToggleSwitch>("GasMotor1Toggle"),
                "Heat Pump 1" => this.FindControl<ToggleSwitch>("HeatPump1Toggle"),
                _ => null
            };
            
            if (toggle != null)
            {
                toggle.IsChecked = unit.IsAvailable;
                // Also update visual state
                UpdateUnitVisualState(unit.Name, unit.IsAvailable);
            }
        }
    }
    
    private void LoadUnitImages()
    {
        try
        {
            _unitImages = new Dictionary<string, Bitmap>();
            
            // Get production units from AssetManager
            var units = AssetManager.GetProdUnits();
            
            // Map unit types to dictionary keys
            var typeMapping = new Dictionary<string, string>
            {
                { "Gas Boiler 1", "GasBoiler" },
                { "Gas Boiler 2", "GasBoiler" },
                { "Oil Boiler 1", "OilBoiler" },
                { "Gas Motor 1", "GasMotor" },
                { "Heat Pump 1", "HeatPump" }
            };
            
            // Process each unit
            foreach (var unit in units)
            {
                if (typeMapping.TryGetValue(unit.Name, out var key) && !_unitImages.ContainsKey(key))
                {
                    // Extract filename from path
                    string fileName = Path.GetFileName(unit.ImagePath);
                    Console.WriteLine($"Loading image for {unit.Name}: {unit.ImagePath} (filename: {fileName})");
                    
                    // Try to load the image using the full path first
                    Stream stream = GetAssetStream(unit.ImagePath);
                    
                    // If that fails, try with just the filename
                    if (stream == null)
                    {
                        stream = GetAssetStream(fileName);
                    }
                    
                    if (stream != null)
                    {
                        try
                        {
                            _unitImages[key] = new Bitmap(stream);
                            Console.WriteLine($"Successfully loaded image for {key}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error creating bitmap for {fileName}: {ex.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Could not load image file for {unit.Name}: {fileName}");
                        
                        // Skip creating a placeholder for now - we'll just log the warning
                        Console.WriteLine($"Skipping placeholder creation for {key} - will show warning only");
                    }
                }
            }
            
            // Check if all required images were loaded
            var requiredKeys = new[] { "GasBoiler", "OilBoiler", "GasMotor", "HeatPump" };
            foreach (var key in requiredKeys)
            {
                if (!_unitImages.ContainsKey(key))
                {
                    Console.WriteLine($"Warning: Image for {key} was not loaded");
                }
            }
        }
        catch (Exception ex)
        {
            // Handle image loading errors
            Console.WriteLine($"Error loading unit images: {ex.Message}");
        }
    }
    
    private void Border_PointerEntered(object sender, PointerEventArgs e)
    {
        if (sender is Border border && _unitImages != null)
        {
            string unitType = border.Tag?.ToString();
            
            if (!string.IsNullOrEmpty(unitType) && _unitImages.TryGetValue(unitType, out var image))
            {
                // Set the image source
                _hoverImage.Source = image;
                
                // Show the canvas
                _hoverImageCanvas.IsVisible = true;
                
                // Position the image at the cursor
                UpdateImagePosition(e);
                
                // Mark the event as handled
                e.Handled = true;
            }
            else
            {
                // If we don't have an image for this unit type, don't show the hover image
                _hoverImageCanvas.IsVisible = false;
                Console.WriteLine($"No image available for hover: {unitType}");
            }
        }
    }
    
    private void Border_PointerExited(object sender, PointerEventArgs e)
    {
        // Hide the canvas when pointer leaves the border
        _hoverImageCanvas.IsVisible = false;
    }
    
    private void Border_PointerMoved(object sender, PointerEventArgs e)
    {
        // Update the image position as the pointer moves
        if (_hoverImageCanvas.IsVisible)
        {
            UpdateImagePosition(e);
        }
    }
    
    private void UpdateImagePosition(PointerEventArgs e)
    {
        // Get the pointer position relative to the canvas
        Point position = e.GetPosition(_hoverImageCanvas);
        
        // Apply offset to position the image relative to the cursor
        Canvas.SetLeft(_hoverImage, position.X + _imageOffset.Left);
        Canvas.SetTop(_hoverImage, position.Y + _imageOffset.Top);
    }
    
    private void ProductionUnitToggle_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            string unitName = toggleSwitch.Tag?.ToString();
            bool isEnabled = toggleSwitch.IsChecked ?? false;
            
            if (!string.IsNullOrEmpty(unitName))
            {
                // Update the production unit availability in AssetManager
                AssetManager.SetProdUnit(unitName, isEnabled);
                
                Console.WriteLine($"Production unit '{unitName}' {(isEnabled ? "enabled" : "disabled")}");
                
                // Optional: Add visual feedback by changing the border appearance
                UpdateUnitVisualState(unitName, isEnabled);
            }
        }
    }
    
    private void UpdateUnitVisualState(string unitName, bool isEnabled)
    {
        // Find the corresponding border element and update its visual state
        Border border = unitName switch
        {
            "Gas Boiler 1" => this.FindControl<Border>("GasBoiler1Border"),
            "Gas Boiler 2" => this.FindControl<Border>("GasBoiler2Border"),
            "Oil Boiler 1" => this.FindControl<Border>("OilBoiler1Border"),
            "Gas Motor 1" => this.FindControl<Border>("GasMotor1Border"),
            "Heat Pump 1" => this.FindControl<Border>("HeatPump1Border"),
            _ => null
        };
        
        if (border != null)
        {
            // Change opacity to indicate disabled state
            border.Opacity = isEnabled ? 1.0 : 0.6;
        }
    }

    private Stream GetAssetStream(string fileName)
    {
        try
        {
            string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", fileName);
            Console.WriteLine($"Trying path: {basePath}");
            if (File.Exists(basePath))
            {
                Console.WriteLine($"Found file at: {basePath}");
                return File.OpenRead(basePath);
            }
            
            string currentDirPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", fileName);
            Console.WriteLine($"Trying path: {currentDirPath}");
            if (File.Exists(currentDirPath))
            {
                Console.WriteLine($"Found file at: {currentDirPath}");
                return File.OpenRead(currentDirPath);
            }
            
            string projectPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Assets", fileName));
            Console.WriteLine($"Trying path: {projectPath}");
            if (File.Exists(projectPath))
            {
                Console.WriteLine($"Found file at: {projectPath}");
                return File.OpenRead(projectPath);
            }
            
            try
            {
                var uri = new Uri($"avares://SP2/Assets/{fileName}");
                Console.WriteLine($"Trying Avalonia asset URI: {uri}");
                
                var assetStream = AssetLoader.Open(uri);
                if (assetStream != null)
                {
                    Console.WriteLine($"Found asset using Avalonia asset loader");
                    return assetStream;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Avalonia asset loader error: {ex.Message}");
            }
            
            var assembly = Assembly.GetExecutingAssembly();
            var resourceNames = assembly.GetManifestResourceNames();
            Console.WriteLine($"Available embedded resources: {string.Join(", ", resourceNames)}");
            
            var resourceFormats = new[]
            {
                $"SP2.Assets.{fileName}",
                $"Assets.{fileName}",
                fileName
            };
            
            foreach (var resourceName in resourceFormats)
            {
                Console.WriteLine($"Trying resource name: {resourceName}");
                var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream != null)
                {
                    Console.WriteLine($"Found resource: {resourceName}");
                    return stream;
                }
            }
            
            Console.WriteLine($"Could not find asset: {fileName}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading asset {fileName}: {ex.Message}");
            return null;
        }
    }
}
