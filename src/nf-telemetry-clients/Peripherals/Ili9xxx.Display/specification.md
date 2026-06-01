# Graphics driver specification

## Overview

Graphics driver provides functionality for drawing bitmaps, and text on Ili9488, Ili9342 and Ili9341 series of displays. 

## Initialization
- Driver assumes display has already been initialized and does not setup SPI pin configuration or send any initialization commands to display.
- Driver is provided a reference to a Bitmap called screen used for drawing operations.
- Driver is provided width, height and orientation of the display.
- Driver is provided font resource files with their size in pixels and color.
- Driver is provided a correctly sized image for views.

## Architecture

### View system
- The driver holds a reference to the active `IliView` via `SetView(IliView view)`.
- Switching views calls `oldView.Dispose()`, then `newView.Render(screen)`, then `screen.Flush()`.
- `screen.Flush()` is only called on view switches — never on individual metric updates.
- The driver is not thread-safe. The caller is responsible for marshalling all `UpdateMetric` calls through a single thread.

### `IliView` base class
```
abstract class IliView
    void Render(Bitmap screen)   // draws initial view, called on SetView
    void Dispose()               // releases resources, called before Render of next view
```

### Metric definitions
- Metrics are typed: `TextMetric` and `BitmapMetric`, both extending an abstract `MetricDefinition` base.
- Each `MetricDefinition` holds: position (x, y), size (width, height), and background placeholder color.
- `TextMetric` additionally holds: font resource reference and text color.
- `BitmapMetric` additionally holds: a single `Bitmap` reference.
- Metric definitions are constructed by the caller and passed to the view at construction.
- The caller holds direct references to metric instances and passes them to `UpdateMetric`.

### Metric updates (Dashboard view)
- `DashboardView.UpdateMetric(TextMetric metric, string value)` — clears placeholder region with `BgColor`, then draws text bottom-left aligned.
- `DashboardView.UpdateMetric(BitmapMetric metric, Bitmap bitmap)` — clears placeholder region with `BgColor`, then draws bitmap.
- Text position within the region is bottom-left. Font width is calculated via `Font.ComputeExtent`. Font height is fixed per font resource.
- Each update draws immediately with no buffering or batching.

## Specification

Driver supports different 'views' with drawing operations overlayed.
Supported views are:
- Splash
- Dashboard
- Generic

### View: Splash
View with a single bitmap covering the entire display.

### View: Dashboard
View with a background bitmap covering the entire display. Text and bitmaps can be drawn on top of it.
Background bitmap is structured to provide single-color placeholder regions for each metric. On update, the placeholder region is filled with its background color before the new value is drawn.
View consists of:
- Throttle gauge, 0-100% with 1% increments.
- Water depth, 0-10m with 0.1m increments.
- Water temperature with 0.5 degree increments.
- Battery percentage, 0-100% with 1% increments.
- Battery current draw, 0.5A increments with a range of 0-100A.
- Temperature of left and right speed controller (ESC), with 1 degree increments.
- Indication if controls are armed or disarmed (bitmap metric).

### View: Generic
View with a single color background.

## Drawing operations
- Draw/clear text at position
- Draw/clear bitmap at position

## Scope
- Driver is to be implemented in managed C# code and not in native C++ code.
- Single buffer is used for drawing operations. No double buffering.
- Partial updates are preferred. `screen.Flush()` is only called on full view switches.
- Font width is calculated via `Font.ComputeExtent`. Font height is fixed per font resource.
- Text and bitmap metrics are aligned bottom-left within their placeholder region.
- Fonts are assigned per metric at construction time. Different metrics may use different font resources.
- Orientation is fixed at driver initialization and cannot be changed at runtime.

## Limitations
- Driver executes on a resource-constrained ESP32 microcontroller.
- Communication with the display is done via SPI with limited bandwidth.
- A fixed set of font resources and their sizes are supported. Size and weight cannot be changed at runtime.
- Display is used in landscape orientation only. Cannot be rotated/changed at runtime.

## References
- Native C++ graphics driver implementation
  - https://github.com/mikmog/nf-interpreter/blob/add-18-bit-spi-graphics/src/nanoFramework.Graphics/Graphics/Displays/Generic_SPI.cpp
  - https://github.com/mikmog/nf-interpreter/blob/add-18-bit-spi-graphics/src/nanoFramework.Graphics/Graphics/Displays/Spi_To_Display.cpp
